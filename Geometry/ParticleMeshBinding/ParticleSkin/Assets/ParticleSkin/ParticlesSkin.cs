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
        public float m_SkinningFalloff = 1.0f;
        [Tooltip("The maximum distance a cluster can be from a vertex before it will not influence it any more.")]
        public float m_SkinningMaxDistance = 0.5f;

        /// <summary>
        /// softbone骨骼的转换matrix
        /// bindPos是骨骼变换矩阵的逆矩阵
        /// 骨骼变换矩阵:将坐标从骨骼本地坐标系变化到物体本地坐标值
        /// </summary>
        [HideInInspector]
        [SerializeField] List<Matrix4x4> m_Bindposes = new List<Matrix4x4>();
        /// <summary>
        /// 所有的顶点骨骼绑定权重数组
        /// </summary>
        [HideInInspector]
        [SerializeField] NativeBoneWeightList m_BoneWeights;
        /// <summary>
        /// 每个byte是每个顶点被绑定的bone数量
        /// </summary>
        [HideInInspector]
        [SerializeField] NativeByteList m_BonesPerVertex;

        public ParticleTopology m_particleTopology = null;
        private IParticleCollection m_particleCollection = null;

        public SkinnedMeshRenderer m_skinnedMeshRender = null;

        private Mesh m_BakedMesh;
        private Mesh m_OriginalMesh;
        //[HideInInspector][SerializeField]
        private Mesh m_SoftMesh;
        private List<Transform> m_SoftBones = new List<Transform>();

        public void Awake()
        {
            // autoinitialize "target" with the first skinned mesh renderer we find up our hierarchy.
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
            DestroyImmediate(m_BakedMesh);
            m_BoneWeights.Dispose();
            m_BonesPerVertex.Dispose();
        }

        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            if (m_OriginalMesh != null)
                m_skinnedMeshRender.sharedMesh = m_OriginalMesh;

            if (m_SoftMesh)
                DestroyImmediate(m_SoftMesh);

            RemoveSoftBones();
        }

        private void RemoveSoftBones()
        {
            if (m_skinnedMeshRender != null)
            {
                var bones = m_skinnedMeshRender.bones;
                Array.Resize(ref bones, bones.Length - m_SoftBones.Count);
                m_skinnedMeshRender.bones = bones;
            }

            foreach (Transform t in m_SoftBones)
                if (t) Destroy(t.gameObject);

            m_SoftBones.Clear();
        }

        public IEnumerator BindSkin()
        {
            m_particleCollection = m_particleTopology as IParticleCollection;

            if (m_particleCollection == null)
            {
                yield break;
            }

            if (m_BoneWeights == null)
                m_BoneWeights = new NativeBoneWeightList();
            if (m_BonesPerVertex == null)
                m_BonesPerVertex = new NativeByteList();

            // bake skinned mesh:
            if (m_BakedMesh == null)
                m_BakedMesh = new Mesh();
            m_skinnedMeshRender.BakeMesh(m_BakedMesh);

            // get the amount of vertices and bones per vertex.
            Vector3[] vertices = m_BakedMesh.vertices;
            var bindPoses = m_skinnedMeshRender.sharedMesh.bindposes;
            var bonesPerVertex = m_skinnedMeshRender.sharedMesh.GetBonesPerVertex();
            var boneWeights = m_skinnedMeshRender.sharedMesh.GetAllBoneWeights();
            var bonesOffset = bindPoses.Length;

            int clusterCount = m_particleCollection.ParticleCount;
            var clusterCenters = new List<Vector3>(clusterCount);

            // prepare lists to store new bindposes and weights:
            m_Bindposes.Clear();
            m_Bindposes.Capacity = clusterCount;
            m_BonesPerVertex.Clear();
            m_BoneWeights.Clear();

            Matrix4x4 target2w = transform.localToWorldMatrix;

            // Create bind pose matrices, one per shape matching cluster:
            for (int i = 0; i < m_particleCollection.ParticleCount; ++i)
            {
                var pos = m_particleCollection.GetParticlePosition(i);

                Vector3 clusterCenter = pos;
                Quaternion clusterOrientation = Quaternion.identity;

                clusterCenters.Add(clusterCenter);

                var bone2World = Matrix4x4.TRS(clusterCenter, clusterOrientation, Vector3.one);
                m_Bindposes.Add(bone2World.inverse* target2w);

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

                // calculate and append new weights:
                int newBoneCount = 0;
                for (int i = 0; i < clusterCenters.Count; ++i)
                {
                    float distance = Vector3.Distance(vertexPosition, clusterCenters[i]);

                    if (distance <= m_SkinningMaxDistance)
                    {
                        var boneWeight = new BoneWeight1();

                        boneWeight.boneIndex = bonesOffset + i;
                        boneWeight.weight = distance > 0 ? m_SkinningMaxDistance / distance : 100;
                        boneWeight.weight = Mathf.Pow(boneWeight.weight, m_SkinningFalloff);
                        m_BoneWeights.Add(boneWeight);
                        newBoneCount++;
                    }
                }

                // normalize new weights only:
                NormalizeWeights(newBoneWeightOffset, newBoneCount);

                // scale new weights:
                for (int i = 0; i < newBoneCount; ++i)
                {
                    var boneWeight = m_BoneWeights[newBoneWeightOffset + i];
                    boneWeight.weight *= softInfluence;
                    m_BoneWeights[newBoneWeightOffset + i] = boneWeight;
                }

                // scale existing weights:
                for (int i = 0; i < originalBoneCount; ++i)
                {
                    var boneWeight = boneWeights[originalBoneWeightOffset + i];
                    if (newBoneCount > 0)
                        boneWeight.weight *= 1 - softInfluence;
                    m_BoneWeights.Add(boneWeight);
                }

                originalBoneWeightOffset += originalBoneCount;

                // calculate total bone count for this vertex (original bones + new bones):
                int totalBoneCount = originalBoneCount + newBoneCount;

                // renormalize all weights:
                NormalizeWeights(newBoneWeightOffset, totalBoneCount);

                // Sort bones by decreasing weight:
                var slice = m_BoneWeights.AsNativeArray<BoneWeight1>().Slice(newBoneWeightOffset, totalBoneCount);
#if true
                slice.Sort(comparer);
#else
                var sorted = slice.OrderByDescending(x => x.weight).ToList();
                for (int i = 0; i < totalBoneCount; ++i)
                    m_BoneWeights[newBoneWeightOffset + i] = sorted[i];
#endif

                // Limit the amount of bone  influences:
                totalBoneCount = (byte)Mathf.Min(totalBoneCount, m_maxBonesPerVertex);
                m_BoneWeights.RemoveRange(newBoneWeightOffset + totalBoneCount, m_BoneWeights.count - (newBoneWeightOffset + totalBoneCount));

                // Renormalize all weights:
                NormalizeWeights(newBoneWeightOffset, totalBoneCount);

                // Append total bone count
                m_BonesPerVertex.Add((byte)totalBoneCount);
                newBoneWeightOffset += totalBoneCount;

                yield return new CoroutineJob.ProgressInfo("calculating bone weights...", j / (float)vertices.Length);
            }
        }

        private void NormalizeWeights(int offset, int count)
        {
            float weightSum = 0;
            for (int i = 0; i < count; ++i)
                weightSum += m_BoneWeights[offset + i].weight;

            if (weightSum > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    var boneWeight = m_BoneWeights[offset + i];
                    boneWeight.weight /= weightSum;
                    m_BoneWeights[offset + i] = boneWeight;
                }
            }
        }

        private void SetBoneWeights()
        {
            if (m_BoneWeights != null && m_BoneWeights.count > 0)
            {
                m_SoftMesh.SetBoneWeights(m_BonesPerVertex.AsNativeArray<byte>(), m_BoneWeights.AsNativeArray<BoneWeight1>());
            }
        }

        private void AppendBindposes()
        {
            List<Matrix4x4> bindposes = new List<Matrix4x4>(m_SoftMesh.bindposes);
            bindposes.AddRange(m_Bindposes);
            m_SoftMesh.bindposes = bindposes.ToArray();
        }

        private void AppendSoftBones()
        {
            m_SoftBones.Clear();
            var boneRoot = this.transform.Find("Bones");
            for (int i = 0; i < m_particleCollection.ParticleCount; ++i)
            {
                var pos = m_particleCollection.GetParticlePosition(i);

                GameObject bone = new GameObject("bone" + i);
                bone.transform.parent = boneRoot;
                bone.transform.position = pos;
                bone.transform.localRotation = Quaternion.identity;
                //bone.hideFlags = HideFlags.HideAndDontSave;
                m_SoftBones.Add(bone.transform);
            }

            // append our bone list to the original one:
            var bones = new List<Transform>(m_skinnedMeshRender.bones);
            bones.AddRange(m_SoftBones);
            m_skinnedMeshRender.bones = bones.ToArray();
        }

        private void Setup()
        {
            if (Application.isPlaying)
            {
                // Setup the mesh from scratch, in case it is not a clone of an already setup mesh.
                if (m_SoftMesh == null)
                {
                    // Create a copy of the original mesh:
                    m_SoftMesh = Instantiate(m_skinnedMeshRender.sharedMesh);

                    SetBoneWeights();
                    AppendBindposes();
                    AppendSoftBones();

                    // Set the new mesh:
                    m_SoftMesh.RecalculateBounds();
                    m_OriginalMesh = m_skinnedMeshRender.sharedMesh;
                    m_skinnedMeshRender.sharedMesh = m_SoftMesh;

                    // Recalculate bounds:
                    m_skinnedMeshRender.localBounds = m_SoftMesh.bounds;
                    m_skinnedMeshRender.rootBone = this.transform;
                }
  
            }
        }

        /// <summary>
        /// 从逻辑数据中同步骨骼位置
        /// </summary>
        private void UpdateSoftBones()
        {
            if (m_SoftBones.Count > 0)
            {
                if (m_particleCollection != null)
                {
                    for (int i = 0; i < m_particleCollection.ParticleCount; i++)
                    {
                        var pos = m_particleCollection.GetParticlePosition(i);
                        m_SoftBones[i].position = pos;
                        m_SoftBones[i].rotation = Quaternion.identity;
                    }

                }
            }
            else
                Setup();
        }

    }
}
