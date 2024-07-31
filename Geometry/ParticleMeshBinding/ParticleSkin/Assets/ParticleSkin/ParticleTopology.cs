using System.Linq;
using UnityEngine;

namespace bluebean
{
    public partial class ParticleTopology : MonoBehaviour, IParticleCollection
    {
        public Transform[] m_particles = null;

        public Transform[] Particles
        {
            get
            {
                InitParticles();
                
                return m_particles;
            }
        }

        private void InitParticles()
        {
            if (m_particles == null || m_particles.Length==0)
            {
                var particlesNode = this.transform.Find("Particles");
                var count = particlesNode.childCount;
                m_particles = new Transform[count];
                for (int i = 0; i < count; i++)
                {
                    m_particles[i] = particlesNode.GetChild(i);
                }
            }
        }

        void Awake()
        {
            InitParticles();
        }

        public int ParticleCount => Particles == null ? 0 : Particles.Length;

        public Color GetParticleColor(int index)
        {
            return Color.red;
        }

        public Vector3 GetParticlePosition(int index)
        {
            return Particles[index].position;
        }

        public float GetParticleRadius(int index)
        {
            return 1;
        }

        public int GetParticleRuntimeIndex(int index)
        {
            return index;
        }
    }
}
