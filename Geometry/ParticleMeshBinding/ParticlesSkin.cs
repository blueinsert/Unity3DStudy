using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace bluebean
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class ParticlesSkin : MonoBehaviour
    {
        public struct BoneWeightComparer : IComparer<BoneWeight1>
        {
            public int Compare(BoneWeight1 x, BoneWeight1 y)
            {
                return y.weight.CompareTo(x.weight);
            }
        }

        [Tooltip("Influence of the softbody in the resulting skin.")]
        [Range(0, 1)]
        public float m_softbodyInfluence = 1;
        [Tooltip("Maximum amount of bone influences for each vertex.")]
        [Range(byte.MinValue, byte.MaxValue)]
        public byte m_maxBonesPerVertex = 4;
        [Tooltip("The ratio at which the cluster's influence on a vertex falls off with distance.")]
        public float m_skinningFalloff = 1.0f;
        [Tooltip("The maximum distance a cluster can be from a vertex before it will not influence it any more.")]
        public float m_skinningMaxDistance = 0.5f;

        /// <summary>
        /// softbone骨骼的转换matrix
        /// bindPos是骨骼变换矩阵的逆矩阵
        /// 骨骼变换矩阵:将坐标从骨骼本地坐标系变化到物体本地坐标值
        /// </summary>
        [HideInInspector]
        [SerializeField] List<Matrix4x4> m_softBoneBindposes = new List<Matrix4x4>();
        /// <summary>
        /// 所有的顶点骨骼绑定权重数组
        /// </summary>
        [HideInInspector]
        [SerializeField] NativeBoneWeightList m_boneWeights;
        /// <summary>
        /// 每个byte是每个顶点被绑定的bone数量
        /// </summary>
        [HideInInspector]
        [SerializeField] NativeByteList m_bonesPerVertex;

        public ParticleTopology m_particleTopology = null;
        private IParticleCollection m_particleCollection = null;

        public SkinnedMeshRenderer m_skinnedMeshRender = null;

        private Mesh m_bakedMesh;
        private Mesh m_originalMesh;
        [HideInInspector][SerializeField] private Mesh m_softMesh;
        /// <summary>
        /// 动态创建的骨骼数组
        /// </summary>
        private List<Transform> m_softBones = new List<Transform>();

        public void Awake()
        {
            m_skinnedMeshRender = GetComponent<SkinnedMeshRenderer>();
            //InitializeInfluences();
            m_particleCollection = m_particleTopology as IParticleCollection;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSoftBones();
        }

        public void OnDestroy()
        {
            DestroyImmediate(m_bakedMesh);
            m_boneWeights.Dispose();
            m_bonesPerVertex.Dispose();
        }

        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            if (m_originalMesh != null)
                m_skinnedMeshRender.sharedMesh = m_originalMesh;

            if (m_softMesh)
                DestroyImmediate(m_softMesh);

            RemoveSoftBones();
        }

        private void RemoveSoftBones()
        {
            if (m_skinnedMeshRender != null)
            {
                var bones = m_skinnedMeshRender.bones;
                Array.Resize(ref bones, bones.Length - m_softBones.Count);
                m_skinnedMeshRender.bones = bones;
            }

            foreach (Transform t in m_softBones)
                if (t) Destroy(t.gameObject);

            m_softBones.Clear();
        }

        public IEnumerator BindSkin()
        {
            m_particleCollection = m_particleTopology as IParticleCollection;

            if (m_particleCollection == null)
            {
                yield break;
            }

            if (m_boneWeights == null)
                m_boneWeights = new NativeBoneWeightList();
            if (m_bonesPerVertex == null)
                m_bonesPerVertex = new NativeByteList();

            // bake skinned mesh:
            if (m_bakedMesh == null)
                m_bakedMesh = new Mesh();
            m_skinnedMeshRender.BakeMesh(m_bakedMesh);

            // get the amount of vertices and bones per vertex.
            Vector3[] vertices = m_bakedMesh.vertices;
            //原有的骨骼绑定数据
            var bindPoses = m_skinnedMeshRender.sharedMesh.bindposes;
            var bonesPerVertex = m_skinnedMeshRender.sharedMesh.GetBonesPerVertex();
            var boneWeights = m_skinnedMeshRender.sharedMesh.GetAllBoneWeights();
            var bonesOffset = bindPoses.Length;

            int clusterCount = m_particleCollection.ParticleCount;
            var clusterCenters = new List<Vector3>(clusterCount);

            // prepare lists to store new bindposes and weights:
            m_softBoneBindposes.Clear(); 
            m_softBoneBindposes.Capacity = clusterCount;
            m_bonesPerVertex.Clear();
            m_boneWeights.Clear();

            Matrix4x4 source2w = m_particleCollection.Local2World;
            Quaternion source2wRot = source2w.rotation;
            Matrix4x4 target2w = transform.localToWorldMatrix;

            // Create bind pose matrices, one per shape matching cluster:
            for (int i = 0; i < m_particleCollection.ParticleCount; ++i)
            {
                var pos = m_particleCollection.GetParticleRestPosition(i);
                // world space bone center/orientation:
                Vector3 clusterCenter = source2w.MultiplyPoint3x4(pos);
                Quaternion clusterOrientation = source2wRot;

                clusterCenters.Add(clusterCenter);

                var boneLocal2World = Matrix4x4.TRS(clusterCenter, clusterOrientation, Vector3.one);
                // local space -> world space -> bone local
                m_softBoneBindposes.Add(boneLocal2World.inverse * target2w);

                yield return new CoroutineJob.ProgressInfo("calculating bind poses...", i);
            }


            BoneWeightComparer comparer = new BoneWeightComparer();

            // Calculate skin weights and bone indices:
            int originalBoneWeightOffset = 0;
            int newBoneWeightOffset = 0;
            for (var j = 0; j < vertices.Length; j++)
            {
                var originalBoneCount = j < bonesPerVertex.Length ? bonesPerVertex[j] : byte.MinValue;
                var vertexPosition = target2w.MultiplyPoint3x4(vertices[j]);
                var softInfluence = 1;// softbodyInfluence * m_softbodyInfluences[j];

                //遍历每顶点每粒子，根据范围阈值，进行绑定
                // calculate and append new weights:
                int newBoneCount = 0;
                for (int i = 0; i < clusterCenters.Count; ++i)
                {
                    float distance = Vector3.Distance(vertexPosition, clusterCenters[i]);

                    if (distance <= m_skinningMaxDistance)
                    {
                        var boneWeight = new BoneWeight1();

                        boneWeight.boneIndex = bonesOffset + i;
                        boneWeight.weight = distance > 0 ? m_skinningMaxDistance / distance : 100;
                        boneWeight.weight = Mathf.Pow(boneWeight.weight, m_skinningFalloff);
                        m_boneWeights.Add(boneWeight);
                        newBoneCount++;
                    }
                }

                // normalize new weights only:
                NormalizeWeights(newBoneWeightOffset, newBoneCount);

                // scale new weights:
                for (int i = 0; i < newBoneCount; ++i)
                {
                    var boneWeight = m_boneWeights[newBoneWeightOffset + i];
                    boneWeight.weight *= softInfluence;
                    m_boneWeights[newBoneWeightOffset + i] = boneWeight;
                }

                // scale existing weights:
                for (int i = 0; i < originalBoneCount; ++i)
                {
                    var boneWeight = boneWeights[originalBoneWeightOffset + i];
                    if (newBoneCount > 0)
                        boneWeight.weight *= 1 - softInfluence;
                    m_boneWeights.Add(boneWeight);
                }

                originalBoneWeightOffset += originalBoneCount;

                // calculate total bone count for this vertex (original bones + new bones):
                int totalBoneCount = originalBoneCount + newBoneCount;

                // renormalize all weights:
                NormalizeWeights(newBoneWeightOffset, totalBoneCount);

                // Sort bones by decreasing weight:
                var slice = m_boneWeights.AsNativeArray<BoneWeight1>().Slice(newBoneWeightOffset, totalBoneCount);
#if false
                slice.Sort(comparer);
#else
                var sorted = slice.OrderByDescending(x => x.weight).ToList();
                for (int i = 0; i < totalBoneCount; ++i)
                    m_boneWeights[newBoneWeightOffset + i] = sorted[i];
#endif

                // Limit the amount of bone  influences:
                totalBoneCount = (byte)Mathf.Min(totalBoneCount, m_maxBonesPerVertex);
                m_boneWeights.RemoveRange(newBoneWeightOffset + totalBoneCount, m_boneWeights.count - (newBoneWeightOffset + totalBoneCount));

                // Renormalize all weights:
                NormalizeWeights(newBoneWeightOffset, totalBoneCount);

                // Append total bone count
                m_bonesPerVertex.Add((byte)totalBoneCount);
                newBoneWeightOffset += totalBoneCount;

                yield return new CoroutineJob.ProgressInfo("calculating bone weights...", j / (float)vertices.Length);
            }
        }

        private void NormalizeWeights(int offset, int count)
        {
            float weightSum = 0;
            for (int i = 0; i < count; ++i)
                weightSum += m_boneWeights[offset + i].weight;

            if (weightSum > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    var boneWeight = m_boneWeights[offset + i];
                    boneWeight.weight /= weightSum;
                    m_boneWeights[offset + i] = boneWeight;
                }
            }
        }

        private void SetBoneWeights()
        {
            if (m_boneWeights != null && m_boneWeights.count > 0)
            {
                m_softMesh.SetBoneWeights(m_bonesPerVertex.AsNativeArray<byte>(), m_boneWeights.AsNativeArray<BoneWeight1>());
            }
        }

        private void AppendBindposes()
        {
            List<Matrix4x4> bindposes = new List<Matrix4x4>(m_softMesh.bindposes);
            bindposes.AddRange(m_softBoneBindposes);
            m_softMesh.bindposes = bindposes.ToArray();
        }

        private void AppendSoftBones()
        {
            // Calculate softbody local to world matrix, and target to world matrix.
            Matrix4x4 source2w = m_particleCollection.Local2World;
            Quaternion source2wRot = source2w.rotation;
            m_softBones.Clear();
            var boneRoot = this.transform.Find("Bones");
            for (int i = 0; i < m_particleCollection.ParticleCount; ++i)
            {
                var pos = m_particleCollection.GetParticleRestPosition(i);

                GameObject bone = new GameObject("bone" + i);
                bone.transform.parent = boneRoot;
                bone.transform.position = source2w.MultiplyPoint3x4(pos);
                bone.transform.rotation = source2wRot;
                //bone.hideFlags = HideFlags.HideAndDontSave;
                m_softBones.Add(bone.transform);
            }

            // append our bone list to the original one:
            var bones = new List<Transform>(m_skinnedMeshRender.bones);
            bones.AddRange(m_softBones);
            m_skinnedMeshRender.bones = bones.ToArray();
        }

        // Copies existing softbones from the skinned mesh renderer. Useful when reusing an existing mesh instead of creating an instance.
        private void CopySoftBones()
        {
            int boneCount = m_particleCollection.ParticleCount;

            m_softBones.Clear();
            m_softBones.Capacity = boneCount;

            for (int i = m_skinnedMeshRender.bones.Length - boneCount; i < m_skinnedMeshRender.bones.Length; ++i)
                m_softBones.Add(m_skinnedMeshRender.bones[i]);
        }

        private void Setup()
        {
            if (Application.isPlaying)
            {
                // Setup the mesh from scratch, in case it is not a clone of an already setup mesh.
                if (m_softMesh == null)
                {
                    // Create a copy of the original mesh:
                    m_softMesh = Instantiate(m_skinnedMeshRender.sharedMesh);

                    SetBoneWeights();
                    AppendBindposes();
                    AppendSoftBones();
                }
                // Reuse the same mesh, just copy bone references as we need to update bones every frame.
                else
                {
                    CopySoftBones();
                }

                // Set the new mesh:
                m_softMesh.RecalculateBounds();
                m_originalMesh = m_skinnedMeshRender.sharedMesh;
                m_skinnedMeshRender.sharedMesh = m_softMesh;

                // Recalculate bounds:
                m_skinnedMeshRender.localBounds = m_softMesh.bounds;
                m_skinnedMeshRender.rootBone = this.transform;
            }
        }

        /// <summary>
        /// 从逻辑数据中同步骨骼位置
        /// </summary>
        private void UpdateSoftBones()
        {
            if (m_softBones.Count > 0)
            {
                if (m_particleCollection != null)
                {
                    var source2W = m_particleCollection.Local2World;
                    var sourceRotation = source2W.rotation;
                    for (int i = 0; i < m_particleCollection.ParticleCount; i++)
                    {
                        var pos = m_particleCollection.GetParticlePosition(i);
                        m_softBones[i].position = source2W.MultiplyPoint3x4(pos);
                        m_softBones[i].rotation = Quaternion.identity;
                    }

                }
            }
            else
                Setup();
        }

    }
}
