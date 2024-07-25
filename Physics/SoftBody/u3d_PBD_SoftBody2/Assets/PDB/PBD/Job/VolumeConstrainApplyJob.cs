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

    /// <summary>
    /// ����
    /// </summary>
    [BurstCompile]
    public struct VolumeConstrainSummarizeJob : IJobParallelFor
    {

        [ReadOnly] public NativeArray<int4> m_tets;
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
            var tet = m_tets[constrainIndex];
            var startIndex = constrainIndex * 4;
            for (int j = 0; j < 4; j++)
            {
                int p = tet[j];
                m_deltas[p] += m_positionDeltasPerConstrain[startIndex+j];
                m_counts[p]++;
            }
        }
    }
}
