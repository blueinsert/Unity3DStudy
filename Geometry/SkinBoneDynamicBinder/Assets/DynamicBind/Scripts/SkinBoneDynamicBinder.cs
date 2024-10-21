using bluebean;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class SkinBoneDynamicBinder : MonoBehaviour
{
    public struct BoneWeightComparer : IComparer<BoneWeight1>
    {
        public int Compare(BoneWeight1 x, BoneWeight1 y)
        {
            return y.weight.CompareTo(x.weight);
        }
    }

    [Tooltip("Maximum amount of bone influences for each vertex.")]
    [Range(byte.MinValue, byte.MaxValue)]
    [Header("������Ӱ������������")]
    public byte m_maxBonesPerVertex = 4;
    [Tooltip("The ratio at which the cluster's influence on a vertex falls off with distance.")]
    [Header("����Ȩ�ؾ���˥��ϵ��")]
    public float m_SkinningFalloff = 1.0f;
    [Tooltip("The maximum distance a cluster can be from a vertex before it will not influence it any more.")]
    [Header("�������Ӱ�����")]
    public float m_SkinningMaxDistance = 0.5f;

    [SerializeField]
    public List<Transform> m_boneTransforms = null;
    [SerializeField]
    public SkinnedMeshRenderer m_skinnedMeshRender = null;

    [Header("����bingPos��������")]
    //[HideInInspector]
    [SerializeField] List<Matrix4x4> m_Bindposes = new List<Matrix4x4>();
    /// <summary>
    /// ���еĶ��������Ȩ������
    /// </summary>
    //[HideInInspector]
    [Header("�������Ȩ������")]
    [SerializeField] NativeBoneWeightList m_BoneWeights;
    /// <summary>
    /// ÿ��byte��ÿ�����㱻�󶨵�bone����
    /// </summary>
    //[HideInInspector]
    [Header("���������������")]
    [SerializeField] NativeByteList m_BonesPerVertex;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    public IEnumerator BindSkin()
    {
        yield return null;
        if (m_BoneWeights == null)
            m_BoneWeights = new NativeBoneWeightList();
        if (m_BonesPerVertex == null)
            m_BonesPerVertex = new NativeByteList();
        m_Bindposes = new List<Matrix4x4>();
        if (m_boneTransforms.Count == 0)
            yield break;

        Matrix4x4 target2w = m_skinnedMeshRender.localToWorldMatrix;
        m_Bindposes.Capacity = m_boneTransforms.Count;

        var boneCenters = new List<Vector3>(m_boneTransforms.Count);
        //����Bindposes����
        for (int i = 0; i < m_boneTransforms.Count; ++i)
        {
            var tran = m_boneTransforms[i];

            boneCenters.Add(tran.position);

            var bone2World = tran.localToWorldMatrix;
            m_Bindposes.Add(bone2World.inverse * target2w);

            yield return new CoroutineJob.ProgressInfo("calculating bind poses...", i);
        }

        var bakedMesh = new Mesh();
        m_skinnedMeshRender.BakeMesh(bakedMesh);

        Vector3[] vertices = bakedMesh.vertices;

        var newBoneWeightOffset = 0;
        BoneWeightComparer comparer = new BoneWeightComparer();

        //�������㣬����ÿ���������Ӱ�������Ȩ��
        for (var j = 0; j < vertices.Length; j++)
        {
            var vertexPosition = target2w.MultiplyPoint3x4(vertices[j]);

            int newBoneCount = 0;
            //���������������Ƿ�Ӱ����������Ȩ��
            for (int i = 0; i < boneCenters.Count; ++i)
            {
                float distance = Vector3.Distance(vertexPosition, boneCenters[i]);

                if (distance <= m_SkinningMaxDistance)
                {
                    var boneWeight = new BoneWeight1();

                    boneWeight.boneIndex = i;
                    boneWeight.weight = distance > 0 ? m_SkinningMaxDistance / distance : 100;
                    boneWeight.weight = Mathf.Pow(boneWeight.weight, m_SkinningFalloff);
                    m_BoneWeights.Add(boneWeight);
                    newBoneCount++;
                }
            }

            // normalize new weights only:
            NormalizeWeights(newBoneWeightOffset, newBoneCount);

            // Sort bones by decreasing weight:

            var slice = m_BoneWeights.AsNativeArray<BoneWeight1>().Slice(newBoneWeightOffset, newBoneCount);
#if false
            slice.Sort(comparer);
#else
                var sorted = slice.OrderByDescending(x => x.weight).ToList();
                for (int i = 0; i < newBoneCount; ++i)
                    m_BoneWeights[newBoneWeightOffset + i] = sorted[i];
#endif

            newBoneCount = (byte)Mathf.Min(newBoneCount, m_maxBonesPerVertex);
            m_BoneWeights.RemoveRange(newBoneWeightOffset + newBoneCount, m_BoneWeights.count - (newBoneWeightOffset + newBoneCount));

            NormalizeWeights(newBoneWeightOffset, newBoneCount);

            //���¶����������
            m_BonesPerVertex.Add((byte)newBoneCount);
            newBoneWeightOffset += newBoneCount;

            yield return new CoroutineJob.ProgressInfo("calculating bone weights...", j / (float)vertices.Length);
        }
    }
}
