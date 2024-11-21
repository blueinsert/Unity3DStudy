using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

namespace bluebean
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(CurvePathProvider))]
    public class ObiRopeExtrudedRenderer : MonoBehaviour
    {
        static ProfilerMarker m_UpdateExtrudedRopeRendererChunksPerfMarker = new ProfilerMarker("UpdateCurveExtrudedRenderer");

        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector4> tangents = new List<Vector4>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<Color> vertColors = new List<Color>();
        private List<int> tris = new List<int>();

        CurvePathProvider pathProvider;

        [HideInInspector][NonSerialized] public Mesh extrudedMesh;

        [Range(0, 1)]
        public float uvAnchor = 0;                  /**< Normalized position of texture coordinate origin along rope.*/

        public Vector2 uvScale = Vector2.one;       /**< Scaling of uvs along rope.*/

        public bool normalizeV = true;

        public CrossSection section = null;       /**< Section asset to be extruded along the rope.*/

        public float thicknessScale = 0.8f;  /**< Scales section thickness.*/

        void OnEnable()
        {
            pathProvider = GetComponent<CurvePathProvider>();
            CreateMeshIfNeeded();
        }

        void OnDisable()
        {
            GameObject.DestroyImmediate(extrudedMesh);
        }

        private void CreateMeshIfNeeded()
        {
            if (extrudedMesh == null)
            {
                extrudedMesh = new Mesh();
                extrudedMesh.name = "extrudedMesh";
                extrudedMesh.MarkDynamic();
                GetComponent<MeshFilter>().mesh = extrudedMesh;
            }
        }

        public void UpdateRenderer()
        {
            using (m_UpdateExtrudedRopeRendererChunksPerfMarker.Auto())
            {
                if (section == null)
                    return;

                CreateMeshIfNeeded();
                ClearMeshData();

                int sectionIndex = 0;
                int sectionSegments = section.Segments;
                int verticesPerSection = sectionSegments + 1;           // the last vertex in each section must be duplicated, due to uv wraparound.
                float vCoord = -uvScale.y * pathProvider.RestLength * uvAnchor; // v texture coordinate.

                Vector3 vertex = Vector3.zero, normal = Vector3.zero;
                Vector4 texTangent = Vector4.zero;
                Vector2 uv = Vector2.zero;

                var curve = pathProvider.GetPath();

                if (curve != null)
                {
                    for (int i = 0; i < curve.Count; ++i)
                    {
                        // Calculate previous and next curve indices:
                        int prevIndex = Mathf.Max(i - 1, 0);

                        // advance v texcoord:
                        vCoord += uvScale.y * (Vector3.Distance(curve[i].position, curve[prevIndex].position) /
                                                   pathProvider.RestLength);

                        // calculate section thickness and scale the basis vectors by it:
                        float sectionThickness = curve[i].thickness * thicknessScale;

                        // Loop around each segment:
                        int nextSectionIndex = sectionIndex + 1;
                        for (int j = 0; j <= sectionSegments; ++j)
                        {
                            // make just one copy of the section vertex:
                            Vector2 sectionVertex = section.vertices[j];

                            // calculate normal using section vertex, curve normal and binormal:
                            //以曲线点的normal和binormal为坐标轴建立垂直于曲线方向的平面坐标系，
                            //将section中顶点的x，y坐标认为是定义在该坐标系中的坐标，
                            //下面的变化将点坐标从局部变化到世界；原点不变，在曲线上，因此得到的就是曲线外扩网格表面点的法线
                            //p_global = p_l.x*dir_x_local+p_l.y*dir_y_local
                            //p_global.x = p_l.x*dir_x_local.x + p_l.y*dir_y_local.x
                            normal.x = (sectionVertex.x * curve[i].normal.x + sectionVertex.y * curve[i].binormal.x) * sectionThickness;
                            normal.y = (sectionVertex.x * curve[i].normal.y + sectionVertex.y * curve[i].binormal.y) * sectionThickness;
                            normal.z = (sectionVertex.x * curve[i].normal.z + sectionVertex.y * curve[i].binormal.z) * sectionThickness;

                            // offset curve position by normal:
                            vertex.x = curve[i].position.x + normal.x;
                            vertex.y = curve[i].position.y + normal.y;
                            vertex.z = curve[i].position.z + normal.z;

                            // cross(normal, curve tangent)
                            //曲线外扩网格表面点的副切线，垂直于曲线走向，该方向uv.y不变
                            texTangent.x = normal.y * curve[i].tangent.z - normal.z * curve[i].tangent.y;
                            texTangent.y = normal.z * curve[i].tangent.x - normal.x * curve[i].tangent.z;
                            texTangent.z = normal.x * curve[i].tangent.y - normal.y * curve[i].tangent.x;
                            texTangent.w = -1;

                            uv.x = (j / (float)sectionSegments) * uvScale.x;
                            uv.y = vCoord;

                            vertices.Add(vertex);
                            normals.Add(normal);
                            tangents.Add(texTangent);
                            vertColors.Add(curve[i].color);
                            uvs.Add(uv);

                            //添加三角形索引
                            if (j < sectionSegments && i < curve.Count - 1)
                            {
                                //每次添加一个顶点
                                //添加当前环(section)当前添加的顶点和下个将要添加的顶点构成的线段
                                //与下一个环对应线段构成的两个三角形
                                tris.Add(sectionIndex * verticesPerSection + j);
                                tris.Add(nextSectionIndex * verticesPerSection + j);
                                tris.Add(sectionIndex * verticesPerSection + (j + 1));

                                tris.Add(sectionIndex * verticesPerSection + (j + 1));
                                tris.Add(nextSectionIndex * verticesPerSection + j);
                                tris.Add(nextSectionIndex * verticesPerSection + (j + 1));
                            }
                        }
                        sectionIndex++;
                    }
                }

                CommitMeshData();
            }
        }

        public void Update()
        {
            UpdateRenderer();
        }

        private void ClearMeshData()
        {
            extrudedMesh.Clear();
            vertices.Clear();
            normals.Clear();
            tangents.Clear();
            uvs.Clear();
            vertColors.Clear();
            tris.Clear();
        }

        private void CommitMeshData()
        {
            extrudedMesh.SetVertices(vertices);
            extrudedMesh.SetNormals(normals);
            extrudedMesh.SetTangents(tangents);
            extrudedMesh.SetColors(vertColors);
            extrudedMesh.SetUVs(0, uvs);
            extrudedMesh.SetTriangles(tris, 0, true);
        }
    }
}


