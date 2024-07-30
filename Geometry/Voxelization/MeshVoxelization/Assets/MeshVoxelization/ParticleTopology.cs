using UnityEngine;

namespace bluebean
{
    public partial class ParticleTopology : MonoBehaviour, IParticleCollection
    {
        
        [HideInInspector][SerializeField] protected Bounds m_bounds = new Bounds();
        [HideInInspector] public Vector3[] m_positions = null;           /**< Particle positions.*/
        [HideInInspector] public float[] m_invMasses = null;             /**< Particle inverse masses*/
        [HideInInspector] public float[] m_radius = null;      /**< Particle ellipsoid principal radii. These are the ellipsoid radius in each axis.*/
        [HideInInspector] public Color[] m_colors = null;                /**< Particle colors (not used by all actors, can be null)*/
        /** Simplices **/
        [HideInInspector] public int[] m_points = null;
        [HideInInspector] public int[] m_edges = null;
        [HideInInspector] public int[] m_triangles = null;

        public int ParticleCount => m_positions == null ? 0 : m_positions.Length;

        public Color GetParticleColor(int index)
        {
            return m_colors[index];
        }

        public Vector3 GetParticlePosition(int index)
        {
            return m_positions[index];
        }

        public float GetParticleRadius(int index)
        {
            return m_radius[index];
        }

        public int GetParticleRuntimeIndex(int index)
        {
            return index;
        }
    }
}
