using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    /**
     * Helper class that voxelizes a mesh.
     */
    [Serializable]
    public class MeshVoxelizer
    {
        [Flags]
        public enum Voxel
        {
            Empty = 0,
            Inside = 1 << 0,
            Boundary = 1 << 1,
            Outside = 1 << 2,
        }

        public readonly static Vector3Int[] fullNeighborhood =
        {
            // face neighbors: 
            new Vector3Int(-1,0,0),
            new Vector3Int(1,0,0),
            new Vector3Int(0,-1,0),
            new Vector3Int(0,1,0),
            new Vector3Int(0,0,-1),
            new Vector3Int(0,0,1),

            // edge neighbors:
            new Vector3Int(-1,-1,0),
            new Vector3Int(-1,0,-1),
            new Vector3Int(-1,0,1),
            new Vector3Int(-1,1,0),
            new Vector3Int(0,-1,-1),
            new Vector3Int(0,-1,1),
            new Vector3Int(0,1,-1),
            new Vector3Int(0,1,1),
            new Vector3Int(1,-1,0),
            new Vector3Int(1,0,-1),
            new Vector3Int(1,0,1),
            new Vector3Int(1,1,0),

            // vertex neighbors:
            new Vector3Int(-1,-1,-1),
            new Vector3Int(-1,-1,1),
            new Vector3Int(-1,1,-1),
            new Vector3Int(-1,1,1),
            new Vector3Int(1,-1,-1),
            new Vector3Int(1,-1,1),
            new Vector3Int(1,1,-1),
            new Vector3Int(1,1,1)
        };

        public readonly static Vector3Int[] edgefaceNeighborhood =
        {
            new Vector3Int(-1,-1,0),
            new Vector3Int(-1,0,-1),
            new Vector3Int(-1,0,0),
            new Vector3Int(-1,0,1),
            new Vector3Int(-1,1,0),
            new Vector3Int(0,-1,-1),
            new Vector3Int(0,-1,0),
            new Vector3Int(0,-1,1),
            new Vector3Int(0,0,-1),
            new Vector3Int(0,0,1),
            new Vector3Int(0,1,-1),
            new Vector3Int(0,1,0),
            new Vector3Int(0,1,1),
            new Vector3Int(1,-1,0),
            new Vector3Int(1,0,-1),
            new Vector3Int(1,0,0),
            new Vector3Int(1,0,1),
            new Vector3Int(1,1,0)
        };

        public readonly static Vector3Int[] faceNeighborhood =
    {
            new Vector3Int(-1,0,0),
            new Vector3Int(1,0,0),
            new Vector3Int(0,-1,0),
            new Vector3Int(0,1,0),
            new Vector3Int(0,0,-1),
            new Vector3Int(0,0,1)
        };

        public readonly static Vector3Int[] edgeNeighborhood =
        {
            new Vector3Int(-1,-1,0),
            new Vector3Int(-1,0,-1),
            new Vector3Int(-1,0,1),
            new Vector3Int(-1,1,0),
            new Vector3Int(0,-1,-1),
            new Vector3Int(0,-1,1),
            new Vector3Int(0,1,-1),
            new Vector3Int(0,1,1),
            new Vector3Int(1,-1,0),
            new Vector3Int(1,0,-1),
            new Vector3Int(1,0,1),
            new Vector3Int(1,1,0)
        };

        public readonly static Vector3Int[] vertexNeighborhood =
        {
            new Vector3Int(-1,-1,-1),
            new Vector3Int(-1,-1,1),
            new Vector3Int(-1,1,-1),
            new Vector3Int(-1,1,1),
            new Vector3Int(1,-1,-1),
            new Vector3Int(1,-1,1),
            new Vector3Int(1,1,-1),
            new Vector3Int(1,1,1)
        };

        [NonSerialized] public Mesh m_inputMesh;

        /// <summary>
        /// 体素类型数组
        /// </summary>
        [HideInInspector][SerializeField] private Voxel[] m_voxels;
        /// <summary>
        /// 体素尺寸
        /// </summary>
        public float m_voxelSize;
        /// <summary>
        /// 体素数量：长、宽、高
        /// </summary>
        public Vector3Int m_resolution;
        /// <summary>
        /// 每个体素覆盖的三角形索引
        /// </summary>
        private List<int>[] m_triangleIndices; // temporary structure to hold triangles overlapping each voxel.
        /// <summary>
        /// 体素坐标原点
        /// </summary>
        private Vector3Int m_origin;

        public Vector3Int Origin
        {
            get { return m_origin; }
        }

        public int voxelCount
        {
            get { return m_resolution.x * m_resolution.y * m_resolution.z; }
        }

        public MeshVoxelizer(Mesh input, float voxelSize)
        {
            this.m_inputMesh = input;
            this.m_voxelSize = voxelSize;
        }

        public Voxel this[int x, int y, int z]
        {
            get { return m_voxels[GetVoxelIndex(x, y, z)]; }
            set { m_voxels[GetVoxelIndex(x, y, z)] = value; }
        }

        public float GetDistanceToNeighbor(int i)
        {
            if (i > 17) return PhysicsUtil.sqrt3 * m_voxelSize;
            if (i > 5) return PhysicsUtil.sqrt2 * m_voxelSize;
            return m_voxelSize;
        }

        public int GetVoxelIndex(int x, int y, int z)
        {
            return x + m_resolution.x * (y + m_resolution.y * z);
        }

        public Vector3 GetVoxelCenter(in Vector3Int coords)
        {
            return new Vector3(Origin.x + coords.x + 0.5f,
                               Origin.y + coords.y + 0.5f,
                               Origin.z + coords.z + 0.5f) * m_voxelSize;
        }

        private Bounds GetTriangleBounds(in Vector3 v1, in Vector3 v2, in Vector3 v3)
        {
            Bounds b = new Bounds(v1, Vector3.zero);
            b.Encapsulate(v2);
            b.Encapsulate(v3);
            return b;
        }

        public List<int> GetTrianglesOverlappingVoxel(int voxelIndex)
        {
            if (voxelIndex >= 0 && voxelIndex < m_triangleIndices.Length)
                return m_triangleIndices[voxelIndex];
            return null;
        }

        /// <summary>
        /// 位置坐标转体素坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3Int GetPointVoxel(in Vector3 point)
        {
            return new Vector3Int(Mathf.FloorToInt(point.x / m_voxelSize),
                                  Mathf.FloorToInt(point.y / m_voxelSize),
                                  Mathf.FloorToInt(point.z / m_voxelSize));
        }

        public bool VoxelExists(in Vector3Int coords)
        {
            return VoxelExists(coords.x, coords.y, coords.z);
        }

        public bool VoxelExists(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 &&
                   x < m_resolution.x &&
                   y < m_resolution.y &&
                   z < m_resolution.z;
        }

        /// <summary>
        /// 判断三角形是否与体素相交，并加入缓存
        /// 同时将相交体素设置为Boundary
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="triangleIndex"></param>
        private void AppendOverlappingVoxels(in Bounds bounds, in Vector3 v1, in Vector3 v2, in Vector3 v3, int triangleIndex)
        {

            Vector3Int min = GetPointVoxel(bounds.min);
            Vector3Int max = GetPointVoxel(bounds.max);

            for (int x = min.x; x <= max.x; ++x)
                for (int y = min.y; y <= max.y; ++y)
                    for (int z = min.z; z <= max.z; ++z)
                    {
                        Bounds voxel = new Bounds(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) * m_voxelSize, Vector3.one * m_voxelSize);

                        if (IsIntersecting(voxel, v1, v2, v3))
                        {
                            int index = GetVoxelIndex(x - m_origin.x, y - m_origin.y, z - m_origin.z);
                            m_voxels[index] = Voxel.Boundary;

                            if (m_triangleIndices != null)
                                m_triangleIndices[index].Add(triangleIndex);
                        }
                    }
        }

        /// <summary>
        /// 体素化
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="generateTriangleIndices"></param>
        /// <returns></returns>
        public IEnumerator Voxelize(Matrix4x4 transform, bool generateTriangleIndices = false)
        {
            m_voxelSize = Mathf.Max(0.0001f, m_voxelSize);

            var xfBounds = m_inputMesh.bounds.Transform(transform);

            // Calculate min and max voxels, adding a 1-voxel margin.
            m_origin = GetPointVoxel(xfBounds.min) - new Vector3Int(1, 1, 1);
            Vector3Int max = GetPointVoxel(xfBounds.max) + new Vector3Int(1, 1, 1);

            m_resolution = new Vector3Int(max.x - m_origin.x + 1, max.y - m_origin.y + 1, max.z - m_origin.z + 1);

            // Allocate voxels array, and initialize them to "inside" the mesh:
            m_voxels = new Voxel[m_resolution.x * m_resolution.y * m_resolution.z];

            for (int x = 0; x < m_resolution.x; ++x)
                for (int y = 0; y < m_resolution.y; ++y)
                    for (int z = 0; z < m_resolution.z; ++z)
                        this[x, y, z] = Voxel.Inside;

            // Allocate triangle lists:
            if (generateTriangleIndices)
            {
                m_triangleIndices = new List<int>[m_voxels.Length];
                for (int i = 0; i < m_triangleIndices.Length; ++i)
                    m_triangleIndices[i] = new List<int>(4);
            }
            else
                m_triangleIndices = null;

            // Get input triangles and vertices:
            int[] triIndices = m_inputMesh.triangles;
            Vector3[] vertices = m_inputMesh.vertices;

            // Generate surface voxels:
            for (int i = 0; i < triIndices.Length; i += 3)
            {
                Vector3 v1 = transform.MultiplyPoint3x4(vertices[triIndices[i]]);
                Vector3 v2 = transform.MultiplyPoint3x4(vertices[triIndices[i + 1]]);
                Vector3 v3 = transform.MultiplyPoint3x4(vertices[triIndices[i + 2]]);

                Bounds triBounds = GetTriangleBounds(v1, v2, v3);

                AppendOverlappingVoxels(triBounds, v1, v2, v3, i / 3);

                if (i % 150 == 0)
                    yield return new CoroutineJob.ProgressInfo("Voxelizing mesh...", i / (float)triIndices.Length);
            }

            // Flood fill outside the mesh. This deals with multiple disjoint regions, and non-watertight models.
            var fillCoroutine = FloodFillCalcVoxelType();
            while (fillCoroutine.MoveNext())
                yield return fillCoroutine.Current;
        }

        /// <summary>
        /// boundary体素调整
        /// </summary>
        public void BoundaryThinning()
        {
            for (int x = 0; x < m_resolution.x; ++x)
                for (int y = 0; y < m_resolution.y; ++y)
                    for (int z = 0; z < m_resolution.z; ++z)
                        if (this[x, y, z] == Voxel.Boundary)
                            this[x, y, z] = Voxel.Inside;

            for (int x = 0; x < m_resolution.x; ++x)
                for (int y = 0; y < m_resolution.y; ++y)
                    for (int z = 0; z < m_resolution.z; ++z)
                    {
                        int sum = 0;
                        for (int j = 0; j < faceNeighborhood.Length; ++j)
                        {
                            var index = faceNeighborhood[j];
                            if (VoxelExists(index.x + x, index.y + y, index.z + z) && this[index.x + x, index.y + y, index.z + z] != Voxel.Outside)
                            {
                                sum++;
                            }
                        }
                        //调整为完全根据体素的领域关系，而不是之前的mesh相交
                        //sum > 0 && sum < faceNeighborhood.Length
                        if (sum % faceNeighborhood.Length != 0 && this[x, y, z] == Voxel.Inside)
                            this[x, y, z] = Voxel.Boundary;
                    }
        }

        /// <summary>
        /// 使用泛洪算法计算每个体素的类型Outside,Inside,或者Boundary
        /// </summary>
        /// <returns></returns>
        private IEnumerator FloodFillCalcVoxelType()
        {
            //Voxel数组初始化为inside
            //上一步已将和mesh相交体素设置为Boundary

            Queue<Vector3Int> queue = new Queue<Vector3Int>();
            queue.Enqueue(new Vector3Int(0, 0, 0));
            //[0,0,0]一定是Outside,从它开始检查连接性，
            //相邻的是outside
            this[0, 0, 0] = Voxel.Outside;

            int i = 0;
            while (queue.Count > 0)
            {
                Vector3Int c = queue.Dequeue();
                Vector3Int v;
                //检测右领域，与outside相邻，并且不是Boundary，那它也是outside
                if (c.x < m_resolution.x - 1 && this[c.x + 1, c.y, c.z] == Voxel.Inside)
                {
                    v = new Vector3Int(c.x + 1, c.y, c.z);
                    this[v.x, v.y, v.z] = Voxel.Outside;
                    queue.Enqueue(v);//加入种子队列
                }
                if (c.x > 0 && this[c.x - 1, c.y, c.z] == Voxel.Inside)
                {
                    v = new Vector3Int(c.x - 1, c.y, c.z);
                    this[v.x, v.y, v.z] = Voxel.Outside;
                    queue.Enqueue(v);
                }
                if (c.y < m_resolution.y - 1 && this[c.x, c.y + 1, c.z] == Voxel.Inside)
                {
                    v = new Vector3Int(c.x, c.y + 1, c.z);
                    this[v.x, v.y, v.z] = Voxel.Outside;
                    queue.Enqueue(v);
                }
                if (c.y > 0 && this[c.x, c.y - 1, c.z] == Voxel.Inside)
                {
                    v = new Vector3Int(c.x, c.y - 1, c.z);
                    this[v.x, v.y, v.z] = Voxel.Outside;
                    queue.Enqueue(v);
                }

                if (c.z < m_resolution.z - 1 && this[c.x, c.y, c.z + 1] == Voxel.Inside)
                {
                    v = new Vector3Int(c.x, c.y, c.z + 1);
                    this[v.x, v.y, v.z] = Voxel.Outside;
                    queue.Enqueue(v);
                }
                if (c.z > 0 && this[c.x, c.y, c.z - 1] == Voxel.Inside)
                {
                    v = new Vector3Int(c.x, c.y, c.z - 1);
                    this[v.x, v.y, v.z] = Voxel.Outside;
                    queue.Enqueue(v);
                }

                if (++i % 150 == 0)
                    yield return new CoroutineJob.ProgressInfo("Filling mesh...", i / (float)m_voxels.Length);
            }
        }

        public static bool IsIntersecting(in Bounds box, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            v1 -= box.center;
            v2 -= box.center;
            v3 -= box.center;

            var ab = v2 - v1;
            var bc = v3 - v2;
            var ca = v1 - v3;

            //cross with (1, 0, 0)
            var a00 = new Vector3(0, -ab.z, ab.y);
            var a01 = new Vector3(0, -bc.z, bc.y);
            var a02 = new Vector3(0, -ca.z, ca.y);

            //cross with (0, 1, 0)
            var a10 = new Vector3(ab.z, 0, -ab.x);
            var a11 = new Vector3(bc.z, 0, -bc.x);
            var a12 = new Vector3(ca.z, 0, -ca.x);

            //cross with (0, 0, 1)
            var a20 = new Vector3(-ab.y, ab.x, 0);
            var a21 = new Vector3(-bc.y, bc.x, 0);
            var a22 = new Vector3(-ca.y, ca.x, 0);

            if (
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a00) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a01) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a02) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a10) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a11) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a12) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a20) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a21) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, a22) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, Vector3.right) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, Vector3.up) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, Vector3.forward) ||
                !TriangleAabbSATTest(v1, v2, v3, box.extents, Vector3.Cross(ab, bc))
            )
                return false;

            return true;
        }

        static bool TriangleAabbSATTest(in Vector3 v0, in Vector3 v1, in Vector3 v2, in Vector3 aabbExtents, in Vector3 axis)
        {
            float p0 = Vector3.Dot(v0, axis);
            float p1 = Vector3.Dot(v1, axis);
            float p2 = Vector3.Dot(v2, axis);

            float r = aabbExtents.x * Mathf.Abs(axis.x) +
                      aabbExtents.y * Mathf.Abs(axis.y) +
                      aabbExtents.z * Mathf.Abs(axis.z);

            float maxP = Mathf.Max(p0, Mathf.Max(p1, p2));
            float minP = Mathf.Min(p0, Mathf.Min(p1, p2));

            return !(Mathf.Max(-maxP, minP) > r);
        }

    }

}
