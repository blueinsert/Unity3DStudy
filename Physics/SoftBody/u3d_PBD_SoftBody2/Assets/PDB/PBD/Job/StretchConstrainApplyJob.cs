using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;
using System;

namespace bluebean.UGFramework.Physics
{
    /// <summary>
    /// ����
    /// </summary>
    [BurstCompile]
    public struct StretchConstrainSummarizeJob : IJobParallelFor
    {
        /// <summary>
        /// �߶�����������
        /// </summary>
        [ReadOnly] public NativeArray<int2> m_edges;
        [ReadOnly] public NativeArray<float4> m_positionDeltasPerConstrain;

        /// <summary>
        /// ����Լ����������λ�ñ仯
        /// </summary>
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction]
        public NativeArray<float4> m_deltas;
        /// <summary>
        /// ÿ�����㱻�ۼƴ���
        /// </summary>
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction]
        public NativeArray<int> m_counts;

        public void Execute(int index)
        {
            int constrainIndex = index;

            var e = m_edges[constrainIndex];
            var i = e[0];
            var j = e[1];
            var startIndex = constrainIndex * 2;
            m_counts[i]++;
            m_counts[j]++;
            m_deltas[i] += m_positionDeltasPerConstrain[startIndex];
            m_deltas[j] += m_positionDeltasPerConstrain[startIndex + 1];


        }
    }

}
