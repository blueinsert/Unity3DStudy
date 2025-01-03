using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TetMesh))]
public class SoftBodyActor : PDBActor
{
    const int CollideConstrainCountMax = 500;
    CollideConstrain[] m_collideConstrains = new CollideConstrain[CollideConstrainCountMax];
    int m_collideConstrainCount = 0;

    private PBDSolver m_solver = null;

    private float m_scale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        m_solver = GetComponentInParent<PBDSolver>();
        m_solver.RegisterActor(this);
        PushStretchConstrains2Solver();
        PushVolumeConstrains2Solver();
        for (int i = 0; i < CollideConstrainCountMax; i++)
        {
            m_collideConstrains[i] = new CollideConstrain();
        }

    }

    public override void PreSubStep(float dt, Vector3 g)
    {
        base.PreSubStep(dt, g);
        var solver = GetComponentInParent<PBDSolver>();
        solver.ClearConstrain(ActorId, ConstrainType.Collide);
        GenerateCollideConstrains();
        PushCollideConstrains2Solver();
    }

    void GenerateCollideConstrains()
    {
        m_collideConstrainCount = 0;

        for (int i = 0; i < m_X.Length; i++)
        {
            var p = m_X[i];
            float planeY = -2;
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
        for (int i = 0; i < m_collideConstrainCount; i++)
        {
            var constrain = m_collideConstrains[i];
            m_solver.RegisterConstrain(constrain);
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

    public override float GetTetRestVolume(int tetIndex)
    {
        var volume = base.GetTetRestVolume(tetIndex);
        var tet = m_tetMesh.m_tet[tetIndex];
        var fixedCount = 0;
        fixedCount += m_tetMesh.IsParticleFixed(tet[0]) ? 1 : 0;
        fixedCount += m_tetMesh.IsParticleFixed(tet[1]) ? 1 : 0;
        fixedCount += m_tetMesh.IsParticleFixed(tet[2]) ? 1 : 0;
        fixedCount += m_tetMesh.IsParticleFixed(tet[3]) ? 1 : 0;
        switch (fixedCount)
        {
            case 0:
                volume = volume * m_scale * m_scale * m_scale;
                break;
            case 1:
                volume = volume * m_scale * m_scale * m_scale;
                break;
            case 2: 
                volume = volume * m_scale * m_scale;
                break;
            case 3:
                volume = volume * m_scale;
                break;
            case 4:break;
        }
        return volume;
    }

    public override float GetEdgeRestLen(int edgeIndex)
    {
        var edge = m_tetMesh.m_edge[edgeIndex];
        var len = m_tetMesh.GetEdgeRestLen(edgeIndex);
        if (m_tetMesh.IsParticleFixed(edge[0]) && m_tetMesh.IsParticleFixed(edge[1]))
        {
            return len;
        }
        return len * m_scale;
    }

    public void Update()
    {

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
