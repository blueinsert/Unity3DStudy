using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CollideConstrain
{
    public int m_index;//¶¥µãË÷Òý
    public Vector3 m_normal;
    public Vector3 m_entryPosition;
    public Collider m_collider;

    public bool m_isDynamic;
}

[RequireComponent(typeof(TetMesh))]
public class PBDSoftBodySolver : MonoBehaviour
{
    public Mesh m_mesh = null;
    private TetMesh m_tetMesh = null;

    float m_dt = 0.0333f;
    float m_damping = 0.99f;
    float m_damping_subStep = 0.99f;
    [Range(0f, 0.1f)]
    public float m_edgeCompliance = 0.0f;
    [Range(0f, 1f)]
    public float m_volumeCompliance = 0.0f;

    Vector3[] m_X_last = null;
    Vector3[] m_X = null;
    Vector3[] m_V = null;
    [Range(7, 55)]
    public int m_subStep = 22;

    [Range(0f, 1f)]
    public float m_collideCompliance = 0.0f;
    public CollideConstrain[] m_collideConstrain = new CollideConstrain[CollideConstrainCountMax];
    public int m_collideConstrainCount = 0;
    const int CollideConstrainCountMax = 40 * 40;

    public float m_planeY = -10;

    public float[] m_initRestVol = null;
    public float[] m_restVol = null;
    public float m_scale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        var mesh = new Mesh();
        m_mesh = mesh;
        m_tetMesh = this.GetComponent<TetMesh>();
        //m_X = new Vector3[m_bunnyData.m_pos.Length];
        //CopyX2Y(m_bunnyData.m_pos, m_X);
        m_X = m_tetMesh.m_pos;
        m_X_last = new Vector3[m_X.Length];
        CopyX2Y(m_X, m_X_last);
        m_V = new Vector3[m_X.Length];

        m_mesh.vertices = m_tetMesh.m_pos;
        m_mesh.triangles = BunnyMeshData.TetSurfaceTriIds;
        m_mesh.RecalculateNormals();

        m_damping_subStep = Mathf.Pow(m_damping, 1.0f / m_subStep);

        this.GetComponent<MeshFilter>().mesh = m_mesh;
    }

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

    void Solve()
    {
        SolveEdge();
        SolveVolume();
        Solve_CollideConstrain();
    }


    void CopyX2Y(Vector3[] xarray, Vector3[] yarray)
    {
        for (int i = 0; i < xarray.Length; i++)
        {
            yarray[i] = xarray[i];
        }
    }

    void PreUpdate()
    {
        for (int i = 0; i < m_X.Length; i++)
        {
            //if (i == 0 || i == 20) continue;
            m_V[i] += new Vector3(0, -9.8f, 0) * m_dt;
            m_X[i] += m_V[i] * m_dt;
        }
    }

    void GenerateCollideConstrains()
    {
        m_collideConstrainCount = 0;

        for (int i = 0; i < m_X.Length; i++)
        {
            var p = m_X[i];
            float planeY = m_planeY;
            if (p.y < planeY && m_collideConstrainCount < CollideConstrainCountMax - 1)
            {
                m_collideConstrain[m_collideConstrainCount].m_index = i;
                m_collideConstrain[m_collideConstrainCount].m_normal = new Vector3(0, 1, 0);
                m_collideConstrain[m_collideConstrainCount].m_entryPosition = new Vector3(p.x, planeY, p.z);
                m_collideConstrainCount++;
            }
        }
    }

    void Step()
    {
        PreUpdate();

        GenerateCollideConstrains();

        CopyX2Y(this.m_X, this.m_X_last);

        Solve();

        for (int i = 0; i < m_X.Length; i++)
        {
            m_V[i] += (m_X[i] - m_X_last[i]) / m_dt;
            m_V[i] *= m_damping_subStep;
        }

    }

    // Update is called once per frame
    void Update()
    {
        m_dt = 1.0f / 60f / m_subStep;
        for (int i = 0; i < m_subStep; i++)
        {
            Step();
        }
        float speed = 3.0f;
        if (Input.GetKey(KeyCode.O))
        {
            m_scale += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.P))
        {
            m_scale -= speed * Time.deltaTime;
        }
        m_scale = Mathf.Clamp(m_scale, 1.0f, 20.0f);
        m_mesh.vertices = this.m_X;
        m_mesh.RecalculateNormals();
    }


}
