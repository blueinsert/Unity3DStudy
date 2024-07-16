using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TetMesh))]
public class PDBActor : MonoBehaviour
{
    public int m_actorId;
    public int ActorId { get { return m_actorId; } }

    //runtime data
    protected Vector3[] m_X_last = null;
    protected Vector3[] m_X = null;
    protected Vector3[] m_V = null;

    protected TetMesh m_tetMesh = null;

    public Vector3 GetParticlePosition(int index)
    {
        return m_X[index];
    }

    public void ModifyParticelPosition(int particleId, Vector3 deltaPos)
    {
        m_X[particleId] += deltaPos;
    }

    public float GetEdgeRestLen(int edgeIndex)
    {
        return m_tetMesh.GetEdgeRestLen(edgeIndex);
    }

    public float GetParticleInvMass(int particleId)
    {
        return m_tetMesh.GetParticleInvMass(particleId);
    }

    public int[] GetTetVertexIndex(int tetIndex)
    {
        return m_tetMesh.GetTetVertexIndex(tetIndex);
    }

    public float GetTetRestVolume(int tetIndex)
    {
        return m_tetMesh.GetTetRestVolume(tetIndex);
    }

    public Vector2Int GetEdgeParticles(int edgeIndex)
    {
        return m_tetMesh.GetEdgeParticles(edgeIndex);
    }

    public virtual void Initialize()
    {
        m_tetMesh = GetComponent<TetMesh>();

        m_X = m_tetMesh.m_pos;
        m_X_last = new Vector3[m_X.Length];
        Util.CopyArray(m_X, m_X_last);
        m_V = new Vector3[m_X.Length];
    }

    public virtual void PreSubStep(float dt) {
        for (int i = 0; i < m_X.Length; i++)
        {
            m_V[i] += new Vector3(0, -9.8f, 0) * dt;
            m_X[i] += m_V[i] * dt;
        }

    }

    public virtual void PostSubStep(float dt, float velDamp)
    {
        for (int i = 0; i < m_X.Length; i++)
        {
            m_V[i] += (m_X[i] - m_X_last[i]) / dt;
            m_V[i] *= velDamp;
        }
    }

    public virtual void PreStep() { }

    public virtual void PostStep() {

        SyncMesh(m_X);
    }

    public void SyncMesh(Vector3[] x)
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            var mesh = meshFilter.sharedMesh;
            mesh.vertices = x;
            //mesh.RecalculateBounds();
            meshFilter.sharedMesh = mesh;
        }
    }
}
