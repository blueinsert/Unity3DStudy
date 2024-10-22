using bluebean;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
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
    public Transform m_emptyBoneTransform = null;
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

    public bool m_drawGizmons = false;

    private Mesh m_OriginalMesh;
    private Mesh m_mesh;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_mesh == null)
            Setup();
    }

    public void OnDestroy()
    {
        m_BoneWeights.Dispose();
        m_BonesPerVertex.Dispose();
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
        m_BoneWeights.Clear();
        m_BonesPerVertex.Clear();

        if (m_boneTransforms.Count == 0)
            yield break;

        Matrix4x4 target2w = m_skinnedMeshRender.localToWorldMatrix;
        m_Bindposes.Capacity = m_boneTransforms.Count;

        var boneCenters = new List<Vector3>(m_boneTransforms.Count);
        //����Bindposes����
        m_Bindposes.Add(m_emptyBoneTransform.localToWorldMatrix.inverse * target2w);
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
                float fluenceDistance = GetBoneFluenceDistance(m_boneTransforms[i]);
                if (distance <= fluenceDistance)
                {
                    var boneWeight = new BoneWeight1();

                    boneWeight.boneIndex = i+1;//��1������emptyBone
                    boneWeight.weight = distance > 0 ? fluenceDistance / distance : 100;
                    boneWeight.weight = Mathf.Pow(boneWeight.weight, m_SkinningFalloff);
                    m_BoneWeights.Add(boneWeight);
                    newBoneCount++;
                }
            }

            if (newBoneCount == 0)
            {
                newBoneCount = 1;
                var boneWeight = new BoneWeight1();
                boneWeight.boneIndex = 0;//emptyBone
                boneWeight.weight = 1.0f;
                m_BoneWeights.Add(boneWeight);
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


    private void Setup()
    {
        if (Application.isPlaying)
        {
            // Setup the mesh from scratch, in case it is not a clone of an already setup mesh.
            if (m_mesh == null)
            {
                // Create a copy of the original mesh:
                m_mesh = Instantiate(m_skinnedMeshRender.sharedMesh);

                //SetBoneWeights
                if (m_BoneWeights != null && m_BoneWeights.count > 0)
                {
                    m_mesh.SetBoneWeights(m_BonesPerVertex.AsNativeArray<byte>(), m_BoneWeights.AsNativeArray<BoneWeight1>());
                }
                //AppendBindposes
                List<Matrix4x4> bindposes = new List<Matrix4x4>(m_mesh.bindposes);
                bindposes.AddRange(m_Bindposes);
                m_mesh.bindposes = bindposes.ToArray();
                //set bones
                var bones = new List<Transform>(m_skinnedMeshRender.bones);
                bones.Add(m_emptyBoneTransform);
                bones.AddRange(m_boneTransforms);
                m_skinnedMeshRender.bones = bones.ToArray();

                // Set the new mesh:
                m_mesh.RecalculateBounds();
                m_OriginalMesh = m_skinnedMeshRender.sharedMesh;
                m_skinnedMeshRender.sharedMesh = m_mesh;

                // Recalculate bounds:
                m_skinnedMeshRender.localBounds = m_mesh.bounds;
                m_skinnedMeshRender.rootBone = this.transform;
            }

        }
    }

    private float GetBoneFluenceDistance(Transform transform)
    {
        var boneDesc = transform.GetComponent<BoneDesc>();
        if (boneDesc != null)
        {
            return boneDesc.m_skinningMaxDistance;
        }
        return m_SkinningMaxDistance;
    }

    private void OnDrawGizmosSelected()
    {
        if (!m_drawGizmons) return;
        Gizmos.color = Color.green;
        foreach(var bone in m_boneTransforms)
        {
            Gizmos.DrawWireSphere(bone.transform.position, GetBoneFluenceDistance(bone)+0.02f);
        }
    }
}
