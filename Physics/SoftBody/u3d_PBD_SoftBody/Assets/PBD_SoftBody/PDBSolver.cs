using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class PDBSolver : MonoBehaviour, ISolverEnv
{
    public float m_dtSubStep = 0.0333f;
    public float m_dtStep = 0.0333f;
    public float m_damping = 0.99f;
    public float m_damping_subStep = 0.99f;
    [Range(0f, 0.1f)]
    public float m_edgeCompliance = 0.0f;
    [Range(0f, 1f)]
    public float m_volumeCompliance = 0.0f;
    [Range(1, 55)]
    public int m_subStep = 22;
    [Range(0f, 1f)]
    public float m_collideCompliance = 0.0f;

    public List<VolumeConstrain> m_volumeConstrains = new List<VolumeConstrain>();
    public List<StretchConstrain> m_stretchConstrains = new List<StretchConstrain>();
    public List<CollideConstrain> m_collideConstrains = new List<CollideConstrain>();

    public List<PDBActor> m_actors = new List<PDBActor>();
    public Dictionary<int, PDBActor> m_actorDic = new Dictionary<int, PDBActor>();

    public void Awake()
    {
        Application.targetFrameRate = 30;
        m_damping_subStep = Mathf.Pow(m_damping, 1.0f / m_subStep);
        m_dtSubStep = m_dtStep / m_subStep;
    }

    public void RegisterActor(PDBActor actor)
    {
        m_actorDic.Add(actor.ActorId, actor);
        m_actors.Add(actor);
    }

    public void UnRegisterActor(PDBActor actor)
    {
        m_actors.Remove(actor);
        m_actorDic.Remove(actor.ActorId);
    }

    public void RegisterConstrain(ConstrainBase constrain)
    {
        constrain.SetSolveEnv(this);
        switch (constrain.m_type)
        {
            case ConstrainType.Stretch:
                m_stretchConstrains.Add(constrain as StretchConstrain);
                break;
            case ConstrainType.Volume:
                m_volumeConstrains.Add(constrain as VolumeConstrain);
                break;
            case ConstrainType.Collide:
                m_collideConstrains.Add(constrain as CollideConstrain);
                break;
        }
    }

    private void ClearStretchConstrains(int actorId)
    {
        List<StretchConstrain> toRemoves = new List<StretchConstrain>();
        foreach(var constrain in m_stretchConstrains)
        {
            if(constrain.m_actorId == actorId)
            {
                toRemoves.Add(constrain);
            }
        }
        foreach(var constrain in toRemoves)
        {
            m_stretchConstrains.Remove(constrain);
        }

    }

    private void ClearVolumeConstrains(int actorId)
    {
        List<VolumeConstrain> toRemoves = new List<VolumeConstrain>();
        foreach (var constrain in m_volumeConstrains)
        {
            if (constrain.m_actorId == actorId)
            {
                toRemoves.Add(constrain);
            }
        }
        foreach (var constrain in toRemoves)
        {
            m_volumeConstrains.Remove(constrain);
        }

    }

    private void ClearCollideConstrains(int actorId)
    {
        List<CollideConstrain> toRemoves = new List<CollideConstrain>();
        foreach (var constrain in m_collideConstrains)
        {
            if (constrain.m_actorId == actorId)
            {
                toRemoves.Add(constrain);
            }
        }
        foreach (var constrain in toRemoves)
        {
            m_collideConstrains.Remove(constrain);
        }

    }

    public void ClearConstrain(int actorId, ConstrainType type)
    {
        switch (type)
        {
            case ConstrainType.Stretch:
                ClearStretchConstrains(actorId);
                break;
            case ConstrainType.Volume:
                ClearVolumeConstrains(actorId);
                break;
            case ConstrainType.Collide:
                ClearCollideConstrains(actorId);
                break;
        }
    }

    void PreSubStep()
    {
        for (int i = 0; i < m_actors.Count; i++)
        {
            m_actors[i].PreSubStep(m_dtSubStep);
        }
    }

    void PostSubStep()
    {
        for (int i = 0; i < m_actors.Count; i++)
        {
            m_actors[i].PostSubStep(m_dtSubStep,m_damping_subStep);
        }
    }

    void PreStep()
    {
        for (int i = 0; i < m_actors.Count; i++)
        {
            m_actors[i].PreStep();
        }
    }

    void PostStep()
    {
        for (int i = 0; i < m_actors.Count; i++)
        {
            m_actors[i].PostStep();
        }
    }

    void SolveStrethConstrains()
    {
       foreach(var constrain in m_stretchConstrains)
        {
            constrain.SetParams(m_edgeCompliance);
            constrain.Solve(m_dtSubStep);
        }
    }

    void SolveVolumeConstrains()
    {
        foreach (var constrain in m_volumeConstrains)
        {
            constrain.SetParams(m_volumeCompliance);
            constrain.Solve(m_dtSubStep);
        }
    }

    void SolveCollideConstrains()
    {
        foreach (var constrain in m_collideConstrains)
        {
            constrain.SetParams(m_collideCompliance);
            constrain.Solve(m_dtSubStep);
        }
    }

    void Solve()
    {
        SolveStrethConstrains();
        SolveVolumeConstrains();
        SolveCollideConstrains();
    }

    void SubStep()
    {
        PreSubStep();

        Solve();

        PostSubStep();

    }

    void Step()
    {
        PreStep();
        for (int i = 0; i < m_subStep; i++)
        {
            SubStep();
        }
        PostStep();
    }

    // Update is called once per frame
    void Update()
    {
        Step();
        /*
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
        */
    }

    #region ISolverEnv
    public PDBActor GetActor(int id)
    {
        return m_actorDic[id];
    }

    public Vector3 GetParticlePosition(int actorId, int particleId)
    {
        var actor = GetActor(actorId);
        return actor.GetParticlePosition(particleId);
    }

    public float GetParticleInvMass(int actorId, int particleId)
    {
        var actor = GetActor(actorId);
        return actor.GetParticleInvMass(particleId);
    }

    public void ModifyParticelPosition(int actorId, int particleId, Vector3 deltaPos)
    {
        var actor = GetActor(actorId);
        actor.ModifyParticelPosition(particleId, deltaPos);
    }

    public Vector4Int GetTetVertexIndex(int actorId, int tetIndex)
    {
        var actor = GetActor(actorId);
        return actor.GetTetVertexIndex(tetIndex);
    }

    public float GetTetRestVolume(int actorId, int tetIndex)
    {
        var actor = GetActor(actorId);
        return actor.GetTetRestVolume(tetIndex);
    }

    public float GetEdgeRestLen(int actorId, int edgeIndex)
    {
        var actor = GetActor(actorId);
        return actor.GetEdgeRestLen(edgeIndex);
    }

    public Vector2Int GetEdgeParticles(int actorId, int edgeIndex)
    {
        var actor = GetActor(actorId);
        return actor.GetEdgeParticles(edgeIndex);
    }
    #endregion
}
