using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������� tetrahedron Mesh
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class TetMesh : MonoBehaviour
{
    //���ӣ���������
    public int m_numParticles = 0;
    //������
    public int m_numEdges = 0;
    //����������
    public int m_numTets = 0;
    //���������ε���������һ��mesh������������
    public int m_numSurfs = 0;
    //����or����λ������
    public Vector3[] m_pos = null;
    //�����嶥���������飬ÿһ��Ԫ�ش���һ�������壬�ĸ����������ĸ�����
    public Vector4[] m_tet = null;
    //ÿ��Ԫ�ش���һ���ߣ�������������������
    public Vector2Int[] m_edge = null;
    //ÿ��Ԫ�ش���һ�����棬������������������
    public Vector3Int[] m_surf = null;
    //�����ƽ����ʽ
    public int[] m_tetSurfaceTriIds;
    //������ĳ�ʼ�����
    public float[] m_restVol = null;
    //�ߵĳ�ʼ������
    public float[] m_restLen = null;
    //ÿ������ĳ�ʼ����
    public float[] m_mass = null;
    //ÿ������ĳ�ʼ��������
    public float[] m_invMass = null;

    public bool m_isInitialized = false;

    public virtual void Init() { }
    public virtual string GetMeshName()
    {
        return "NewMesh";
    }

    public static float CalcTetVolume(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        var temp = Vector3.Cross(p2 - p1, p3 - p1);
        float res = Vector3.Dot(p4 - p1, temp);
        res *= (1.0f / 6);
        if (res < -1E-06)
        {
            Debug.LogError(string.Format("volume < 0,{0}", res));
        }
        return res;
    }

    /// <summary>
    /// ��ȡ���������
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public float TetVolume(int index)
    {
        var ids = m_tet[index];
        var id1 = (int)ids[0];
        var id2 = (int)ids[1];
        var id3 = (int)ids[2];
        var id4 = (int)ids[3];
        var p1 = m_pos[id1];
        var p2 = m_pos[id2];
        var p3 = m_pos[id3];
        var p4 = m_pos[id4];
        var res = CalcTetVolume(p1, p2, p3, p4);
        return res;
    }

    public void Sync2Mesh4Editor()
    {
        if (!m_isInitialized) return;
        var meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            var mesh = new Mesh();
            mesh.name = GetMeshName();
            mesh.vertices = this.m_pos;
            mesh.triangles = this.m_tetSurfaceTriIds;
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;
        }
    }

    public float GetEdgeRestLen(int edgeIndex) {
        return m_restLen[edgeIndex];
    }


    public float GetParticleInvMass(int particleIndex)
    {
        return m_invMass[particleIndex];
    }

    public float GetTetRestVolume(int tetIndex)
    {
        return m_restVol[tetIndex];
    }

    public int[] GetTetVertexIndex(int tetIndex)
    {
        var tet = m_tet[tetIndex];
        int[] res = new int[4] { (int)tet.x, (int)tet.y, (int)tet.z, (int)tet.w };
        return res;
    }

    public Vector2Int GetEdgeParticles(int edgeIndex)
    {
        return m_edge[edgeIndex];
    }
}
