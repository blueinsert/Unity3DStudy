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
    protected Vector3[] m_externalForces = null;

    protected TetMesh m_tetMesh = null;

    public MeshFilter m_meshFilter = null;
    public Mesh m_mesh = null;

    public int GetParticleCount()
    {
        return m_X.Length;
    }

    public Vector3 GetParticlePosition(int index)
    {
        return m_X[index];
    }

    public Vector3 GetParticleVel(int index)
    {
        return m_V[index];
    }

    public void ModifyParticelPosition(int particleId, Vector3 deltaPos)
    {
        if (!m_tetMesh.IsParticleFixed(particleId))
        {
            m_X[particleId] += deltaPos;
        }
    }

    public virtual float  GetEdgeRestLen(int edgeIndex)
    {
        return m_tetMesh.GetEdgeRestLen(edgeIndex);
    }

    public float GetParticleInvMass(int particleId)
    {
        return m_tetMesh.GetParticleInvMass(particleId);
    }

    public Vector4Int GetTetVertexIndex(int tetIndex)
    {
        return m_tetMesh.GetTetVertexIndex(tetIndex);
    }

    public virtual float GetTetRestVolume(int tetIndex)
    {
        return m_tetMesh.GetTetRestVolume(tetIndex);
    }

    public Vector2Int GetEdgeParticles(int edgeIndex)
    {
        return m_tetMesh.GetEdgeParticles(edgeIndex);
    }

    public void SetParticleExternalForce(int particleId, Vector3 force)
    {
        m_externalForces[particleId] = force;
    }

    public virtual void Initialize()
    {
        m_tetMesh = GetComponent<TetMesh>();

        m_X = m_tetMesh.m_pos;
        m_X_last = new Vector3[m_X.Length];
        Util.CopyArray(m_X, m_X_last);
        m_V = new Vector3[m_X.Length];
        m_externalForces = new Vector3[m_X.Length];

        m_meshFilter = GetComponent<MeshFilter>();
        m_mesh = m_meshFilter.mesh;
    }

    public virtual void PreSubStep(float dt, Vector3 g) {
        Util.CopyArray(m_X, m_X_last);
        for (int i = 0; i < m_X.Length; i++)
        {
            if (!m_tetMesh.IsParticleFixed(i))
            {
                m_V[i] += (g + m_externalForces[i]* GetParticleInvMass(i)) * dt;
                m_X[i] += m_V[i] * dt;
                m_externalForces[i] = Vector3.zero;
            }
        }

    }

    public virtual void PostSubStep(float dt, float velDamp)
    {
        for (int i = 0; i < m_X.Length; i++)
        {
            m_V[i] = (m_X[i] - m_X_last[i]) / dt;
            m_V[i] *= velDamp;
        }
    }

    public virtual void PreStep() { }

    public virtual void PostStep() {

        SyncMesh();
    }

    public void SyncMesh()
    {
        m_mesh.vertices = m_X;
        //Debug.Log($"y:{m_X[0].y} vy:{m_V[0].y}");
        m_mesh.RecalculateNormals();
    }
}
