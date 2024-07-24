using bluebean.UGFramework.DataStruct;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace bluebean.UGFramework.Physics
{
    [BurstCompile]
    public struct VolumeConstrainSolveJob : IJobParallelFor
    {
        /// <summary>
        /// �����嶥������
        /// </summary>
        [ReadOnly] public NativeArray<int4> m_tets;
        /// <summary>
        /// �������ʼ���
        /// </summary>
        [ReadOnly] public NativeArray<float> m_restVolumes;
        /// <summary>
        /// Լ�����
        /// </summary>
        public NativeArray<float> m_compliances;
        /// <summary>
        /// ����λ������
        /// </summary>
        [ReadOnly] public NativeArray<float4> m_positions;
        /// <summary>
        /// ��������
        /// </summary>
        [ReadOnly] public NativeArray<float> m_invMasses;
        /// <summary>
        /// ����Լ����������λ���ݶ�
        /// </summary>
        [NativeDisableContainerSafetyRestriction][NativeDisableParallelForRestriction] public NativeArray<float4> m_gradients;
        /// <summary>
        /// ����Լ����������λ�ñ仯
        /// </summary>
        [NativeDisableContainerSafetyRestriction][NativeDisableParallelForRestriction] public NativeArray<float4> m_deltas;
        /// <summary>
        /// ÿ�����㱻�ۼƴ���
        /// </summary>
        [NativeDisableContainerSafetyRestriction][NativeDisableParallelForRestriction] public NativeArray<int> m_counts;

        [ReadOnly] public float m_deltaTimeSqr;

        public void Execute(int index)
        {
            //index ��Լ������������������
            float alpha = m_compliances[index] / m_deltaTimeSqr;

            var tet = m_tets[index];
            var p1Index = tet.x;
            var p2Index = tet.y;
            var p3Index = tet.z;
            var p4Index = tet.w;
            var p1 = m_positions[p1Index];
            var p2 = m_positions[p2Index];
            var p3 = m_positions[p3Index];
            var p4 = m_positions[p4Index];
            Unity.Collections.NativeList<int> particleIndices = new Unity.Collections.NativeList<int>(4, Allocator.Temp);
            particleIndices.Add(p1Index);
            particleIndices.Add(p2Index);
            particleIndices.Add(p3Index);
            particleIndices.Add(p4Index);
            for (int j = 0; j < 4; ++j)
                m_gradients[particleIndices[j]] = float4.zero;
            m_gradients[p1Index] = new float4(math.cross((p4 - p2).xyz, (p3 - p2).xyz),0);
            m_gradients[p2Index] = new float4(math.cross((p3 - p2).xyz, (p4 - p1).xyz), 0);
            m_gradients[p3Index] = new float4(math.cross((p4 - p1).xyz, (p2 - p1).xyz), 0);
            m_gradients[p4Index] = new float4(math.cross((p2 - p1).xyz, (p3 - p1).xyz), 0);
            float w = 0;
            for (int j = 0; j < 4; j++)
            {
                int p = particleIndices[j];
                w += m_invMasses[p] * math.lengthsq(m_gradients[p]);
            }
            var vol = BurstMath.CalcTetVolume(p1.xyz, p2.xyz, p3.xyz, p4.xyz);
            var restVol = m_restVolumes[index];
            float C = (vol - restVol) * 6f;
            float s = -C / (w + alpha);
            for (int j = 0; j < 4; j++)
            {
                var p = particleIndices[j];
                m_deltas[p] += m_gradients[p] * s * m_invMasses[p];
                m_counts[p]++;
            }
        }

    }
}
