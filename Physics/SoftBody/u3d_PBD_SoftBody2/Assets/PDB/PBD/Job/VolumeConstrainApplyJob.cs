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
    public struct VolumeConstrainApplyJob : IJobParallelFor
    {

        /// <summary>
        /// 四面体顶点索引数组
        /// </summary>
        [ReadOnly] public NativeArray<int4> m_tets;

        [ReadOnly] public float sorFactor;

        [ReadOnly] public NativeArray<float4> m_particleProperties;

        /// <summary>
        /// 粒子位置数组
        /// </summary>
        [NativeDisableContainerSafetyRestriction][NativeDisableParallelForRestriction] public NativeArray<float4> m_positions;
        /// <summary>
        /// 本次约束求解产生的位置变化
        /// </summary>
        [NativeDisableContainerSafetyRestriction][NativeDisableParallelForRestriction] public NativeArray<float4> m_deltas;
        /// <summary>
        /// 每个顶点被累计次数
        /// </summary>
        [NativeDisableContainerSafetyRestriction][NativeDisableParallelForRestriction] public NativeArray<int> m_counts;


        public void Execute(int index)
        {
            //index 是约束索引或四面体索引

            var tet = m_tets[index];
            var p1Index = tet.x;
            var p2Index = tet.y;
            var p3Index = tet.z;
            var p4Index = tet.w;
            Unity.Collections.NativeList<int> particleIndices = new Unity.Collections.NativeList<int>(4, Allocator.Temp);
            particleIndices.Add(p1Index);
            particleIndices.Add(p2Index);
            particleIndices.Add(p3Index);
            particleIndices.Add(p4Index);
            for (int j = 0; j < 4; j++)
            {
                var p = particleIndices[j];
                if (m_counts[p] > 0)
                {
                    float4 property = m_particleProperties[p];
                    if (!PBDUtil.IsParticleFixed(property))
                        m_positions[p] += m_deltas[p] / m_counts[p];
                    m_deltas[p] = float4.zero;
                    m_counts[p] = 0;
                }
            }
        }

    }
}
