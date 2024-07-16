using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TetMesh))]
public class SoftBodyActor : PDBActor
{
    const int CollideConstrainCountMax = 100;
    CollideConstrain[] m_collideConstrains = new CollideConstrain[CollideConstrainCountMax];
    int m_collideConstrainCount = 0;

    private PDBSolver m_solver = null;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        m_solver = GetComponentInParent<PDBSolver>();
        m_solver.RegisterActor(this);
        //PushStretchConstrains2Solver();
        //PushVolumeConstrains2Solver();
        for(int i=0;i< CollideConstrainCountMax; i++)
        {
            m_collideConstrains[i] = new CollideConstrain();
        }
    }

    public override void PreSubStep(float dt)
    {
        base.PreSubStep(dt);
        var solver = GetComponentInParent<PDBSolver>();
        solver.ClearConstrain(ActorId, ConstrainType.Collide);
        //GenerateCollideConstrains();
        //PushCollideConstrains2Solver();
    }

    void GenerateCollideConstrains()
    {
        m_collideConstrainCount = 0;

        for (int i = 0; i < m_X.Length; i++)
        {
            var p = m_X[i];
            float planeY = -15;
            if (p.y < planeY && m_collideConstrainCount < CollideConstrainCountMax - 1)
            {
                m_collideConstrains[m_collideConstrainCount].m_actorId = this.ActorId;
                m_collideConstrains[m_collideConstrainCount].m_index = i;
                m_collideConstrains[m_collideConstrainCount].m_normal = new Vector3(0, 1, 0);
                m_collideConstrains[m_collideConstrainCount].m_entryPosition = new Vector3(p.x, planeY, p.z);
                m_collideConstrainCount++;
            }
        }
    }

    void PushCollideConstrains2Solver()
    {
        var solver = GetComponentInParent<PDBSolver>();
        for (int i = 0; i < m_collideConstrainCount; i++)
        {
            var constrain = m_collideConstrains[m_collideConstrainCount];
            solver.RegisterConstrain(constrain);
        }
    }

    void PushStretchConstrains2Solver()
    {
        for (int e = 0; e < m_tetMesh.m_numEdges; e++)
        {
            var edge = m_tetMesh.m_edge[e];
            StretchConstrain constrain = new StretchConstrain()
            {
                m_actorId = this.ActorId,
                m_edgeIndex = e,
            };
            m_solver.RegisterConstrain(constrain);
        }
    }

    void PushVolumeConstrains2Solver()
    {
        for (int i = 0; i < m_tetMesh.m_numTets; i++)
        {
            var tet = m_tetMesh.m_tet[i];
            VolumeConstrain constrain = new VolumeConstrain()
            {
                m_actorId = this.ActorId,
                m_tetIndex = i,
            };
            m_solver.RegisterConstrain(constrain);
        }
    }

    /*
    void SolveEdge()
    {
        var vertices = this.m_X;
        //Apply PBD here.
        //jacobi approach
        Vector3[] x_new = new Vector3[vertices.Length];
        int[] n = new int[vertices.Length];

        float alpha = m_edgeCompliance / (m_dt * m_dt);
        for (int e = 0; e < m_tetMesh.m_numEdges; e++)
        {
            var l_e = m_tetMesh.m_restLen[e];
            var edge = m_tetMesh.m_edge[e];
            var i = edge.x;
            var j = edge.y;
            var x_i = vertices[i];
            var x_j = vertices[j];
            var x_ij = x_j - x_i;
            var dir = x_ij.normalized;
            var grads = dir;
            var len = x_ij.magnitude;
            {//xpdb
                var inv_mass_i = m_tetMesh.m_invMass[i];
                var inv_mass_j = m_tetMesh.m_invMass[j];
                float C = len - l_e;
                float w = inv_mass_i + inv_mass_j;
                var s = -C / (w + alpha);
                vertices[i] -= grads * s * inv_mass_i;
                vertices[j] += grads * s * inv_mass_j;
            }
        }
    }

    void SolveVolume()
    {
        float alpha = m_volumeCompliance / (m_dt * m_dt);
        Vector3[] grads = new Vector3[4];
        int[] ids = new int[4];
        for(int i = 0; i < m_tetMesh.m_numTets; i++)
        {
            var tet = m_tetMesh.m_tet[i];
            ids[0] = (int)tet[0];
            ids[1] = (int)tet[1];
            ids[2] = (int)tet[2];
            ids[3] = (int)tet[3];
            var p1 = m_X[ids[0]];
            var p2 = m_X[ids[1]];
            var p3 = m_X[ids[2]];
            var p4 = m_X[ids[3]];
            grads[0] = Vector3.Cross(p4 - p2, p3 - p2);
            grads[1] = Vector3.Cross(p3 - p1, p4 - p1);
            grads[2] = Vector3.Cross(p4 - p1, p2 - p1);
            grads[3] = Vector3.Cross(p2 - p1, p3 - p1);

            float w = 0;
            for(int j = 0; j < 4; j++)
            {
                w += m_tetMesh.m_invMass[ids[j]] * Mathf.Pow(grads[j].magnitude, 2.0f);
            }
            var vol = m_tetMesh.TetVolume(i);
            float C = (vol - m_tetMesh.m_restVol[i] * m_scale) * 6f;
            float s = -C / (w + alpha);
            for(int j = 0;j< 4;j++) {
                var id = ids[j];
                var dp = grads[j] * s * m_tetMesh.m_invMass[id];
                m_X[id] += dp;
            }
        }
    }

    void Solve_CollideConstrain()
    {
        var vertices = this.m_X;
        var alpha = m_collideCompliance / (m_dt * m_dt);
        for (int i = 0; i < m_collideConstrainCount; i++)
        {
            var constrain = m_collideConstrain[i];
            var pos = vertices[constrain.m_index];
            float C = Vector3.Dot((pos - constrain.m_entryPosition), constrain.m_normal);
            if (C < 0)
            {
                Vector3 grads = constrain.m_normal;
                float invMass = m_tetMesh.m_invMass[constrain.m_index];
                float w = invMass;
                float s = -C / (w + alpha);
                if (!constrain.m_isDynamic)
                {
                    Vector3 dp = invMass * s * grads;
                    vertices[constrain.m_index] += dp;
                }
                else
                {
                    Vector3 dp = invMass * s * grads;
                    vertices[constrain.m_index] += dp;
                }
            }

        }
    }

    void CopyX2Y(Vector3[] xarray, Vector3[] yarray)
    {
        for (int i = 0; i < xarray.Length; i++)
        {
            yarray[i] = xarray[i];
        }
    }

   */


}
