using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISolverEnv
{
    public PDBActor GetActor(int id);

    public Vector2Int GetEdgeParticles(int actorId, int edgeIndex);

    public float GetEdgeRestLen(int actorId, int edgeIndex);

    public Vector3 GetParticlePosition(int actorId, int particleId);

    public float GetParticleInvMass(int actorId, int particleId);

    public void ModifyParticelPosition(int actorId, int particleId, Vector3 deltaPos);

    public int[] GetTetVertexIndex(int actorId, int tetIndex);

    public float GetTetRestVolume(int actorId, int tetIndex);
}
