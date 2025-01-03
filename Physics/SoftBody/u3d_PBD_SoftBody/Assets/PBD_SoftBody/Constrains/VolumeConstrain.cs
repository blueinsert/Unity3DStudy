using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VolumeConstrain : ConstrainBase
{
    public float m_volumeCompliance = 0.0f;

    public int m_actorId;
    public int m_tetIndex;

    Vector3[] m_grads = new Vector3[4];

    public VolumeConstrain() : base(ConstrainType.Volume)
    {
    }

    public void SetParams(float compliance)
    {
        m_volumeCompliance = compliance;
    }

    public override void Solve(float dt)
    {
        float alpha = m_volumeCompliance / (dt * dt);
        
        var ids = m_solveEnv.GetTetVertexIndex(m_actorId, m_tetIndex);
        
        var p1 = m_solveEnv.GetParticlePosition(m_actorId, ids.x);
        var p2 = m_solveEnv.GetParticlePosition(m_actorId, ids.y);
        var p3 = m_solveEnv.GetParticlePosition(m_actorId, ids.z);
        var p4 = m_solveEnv.GetParticlePosition(m_actorId, ids.w);
        m_grads[0] = Vector3.Cross(p4 - p2, p3 - p2);
        m_grads[1] = Vector3.Cross(p3 - p1, p4 - p1);
        m_grads[2] = Vector3.Cross(p4 - p1, p2 - p1);
        m_grads[3] = Vector3.Cross(p2 - p1, p3 - p1);

        float w = 0;
        for (int j = 0; j < 4; j++)
        {
            w += m_solveEnv.GetParticleInvMass(m_actorId, ids[j]) * Mathf.Pow(m_grads[j].magnitude, 2.0f);
        }
        var vol = TetMesh.CalcTetVolume(p1,p2,p3,p4);
        var restVol = m_solveEnv.GetTetRestVolume(m_actorId, m_tetIndex);
        float C = (vol - restVol) * 6f;
        float s = -C / (w + alpha);
        for (int j = 0; j < 4; j++)
        {
            var id = ids[j];
            var dp = m_grads[j] * s * m_solveEnv.GetParticleInvMass(m_actorId, id);
            m_solveEnv.ModifyParticelPosition(m_actorId, id, dp);
        }

    }
}
