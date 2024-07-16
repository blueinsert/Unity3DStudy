using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchConstrain : ConstrainBase
{
    public float m_edgeCompliance = 0.0f;

    public int m_actorId;
    public int m_edgeIndex;

    public StretchConstrain() : base(ConstrainType.Stretch)
    {
    }

    public void SetParams(float compliance)
    {
        m_edgeCompliance = compliance;
    }

    public override void Solve(float dt)
    {
        float alpha = m_edgeCompliance / (dt * dt);
        var l_e = m_solveEnv.GetEdgeRestLen(m_actorId, m_edgeIndex);
        var e = m_solveEnv.GetEdgeParticles(m_actorId, m_edgeIndex);
        var i = e[0];
        var j = e[1];
        var x_i = m_solveEnv.GetParticlePosition(m_actorId, i);
        var x_j = m_solveEnv.GetParticlePosition(m_actorId, j);
        var x_ij = x_j - x_i;
        var dir = x_ij.normalized;
        var grads = dir;
        var len = x_ij.magnitude;
        {//xpdb
            var inv_mass_i = m_solveEnv.GetParticleInvMass(m_actorId, i);
            var inv_mass_j = m_solveEnv.GetParticleInvMass(m_actorId, j);
            float C = len - l_e;
            float w = inv_mass_i + inv_mass_j;
            var s = -C / (w + alpha);
            var delta1 = -grads * s * inv_mass_i;
            var delta2 = grads * s * inv_mass_j;
            m_solveEnv.ModifyParticelPosition(m_actorId, i, delta1);
            m_solveEnv.ModifyParticelPosition(m_actorId, j, delta2);
        }

    }
}
