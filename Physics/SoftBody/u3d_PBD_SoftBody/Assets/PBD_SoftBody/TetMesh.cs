using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 四面体网络 tetrahedron Mesh
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class TetMesh : MonoBehaviour
{
    //粒子，顶点数量
    public int m_numParticles = 0;
    //边数量
    public int m_numEdges = 0;
    //四面体数量
    public int m_numTets = 0;
    //表面三角形的数量，即一般mesh的三角形数量
    public int m_numSurfs = 0;
    //顶点or粒子位置数组
    public Vector3[] m_pos = null;
    //四面体顶点索引数组，每一个元素代表一个四面体，四个分量代表四个顶点
    public Vector4[] m_tet = null;
    //每个元素代表一条边，两个分量代表顶点索引
    public Vector2Int[] m_edge = null;
    //每个元素代表一个表面，三个分量代表顶点索引
    public Vector3Int[] m_surf = null;
    //上面的平铺形式
    public int[] m_tetSurfaceTriIds;
    //四面体的初始化体积
    public float[] m_restVol = null;
    //边的初始化长度
    public float[] m_restLen = null;
    //每个顶点的初始质量
    public float[] m_mass = null;
    //每个顶点的初始质量倒数
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
    /// 获取四面体体积
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
