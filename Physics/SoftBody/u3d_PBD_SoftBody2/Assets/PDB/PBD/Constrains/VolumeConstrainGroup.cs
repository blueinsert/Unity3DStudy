using bluebean.UGFramework.DataStruct;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace bluebean.UGFramework.Physics
{
    public class VolumeConstrainGroup : ConstrainGroup
    {
        /// <summary>
        /// 四面体顶点索引数组
        /// </summary>
        public NativeInt4List m_tetList = new NativeInt4List();
        /// <summary>
        /// 每个约束(四面体)的初始体积
        /// </summary>
        public NativeFloatList m_restVolumeList = new NativeFloatList();
        /// <summary>
        /// 约束柔度
        /// </summary>
        public NativeFloatList m_complianceList = new NativeFloatList();

        private NativeArray<int4> m_tets;
        private NativeArray<float> m_restVolumes;
        private NativeArray<float> m_compliances;

        public VolumeConstrainGroup(ISolver solver) : base(ConstrainType.Volume, solver)
        {
            
        }

        private void OnConstrainCountChanged()
        {
            m_tets = m_tetList.AsNativeArray<int4>();
            m_restVolumes = m_restVolumeList.AsNativeArray<float>();
            m_compliances = m_complianceList.AsNativeArray<float>();
        }

        public void AddConstrain(VectorInt4 tet, float restVolume,float compliance)
        {
            m_tetList.Add(tet);
            m_restVolumeList.Add(restVolume);
            m_complianceList.Add(compliance);
            m_constrainCount++;

            OnConstrainCountChanged();
        }

        public override JobHandle Apply(JobHandle inputDeps, float substepTime)
        {
            var job = new VolumeConstrainApplyJob() {
                m_tets = m_tets,
                m_positions = this.m_solver.ParticlePositions,
                m_deltas = this.m_solver.PositionDeltas,
                m_counts = this.m_solver.PositionConstraintCounts,
                m_particleProperties = this.m_solver.ParticleProperties,
            };
            return job.Schedule(m_constrainCount, 4, inputDeps);
        }

        public override JobHandle Solve(JobHandle inputDeps, float substepTime)
        {
            var job = new VolumeConstrainSolveJob() {
                m_tets = this.m_tets,
                m_restVolumes = this.m_restVolumes,
                m_compliances = this.m_compliances,
                m_invMasses = this.m_solver.InvMasses,
                m_positions = this.m_solver.ParticlePositions,
                m_gradients = this.m_solver.Gradients,
                m_deltas = this.m_solver.PositionDeltas,
                m_counts = this.m_solver.PositionConstraintCounts,
                m_deltaTimeSqr = substepTime*substepTime,
            };
            return job.Schedule(m_constrainCount, 4, inputDeps);
        }
    }
}
