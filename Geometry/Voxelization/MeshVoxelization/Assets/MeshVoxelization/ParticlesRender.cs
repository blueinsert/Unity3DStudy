using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class ParticlesRender : MonoBehaviour
    {
        public GameObject m_particlePrefab = null;
        private GameObjectPool<ParticleController> m_particlePool = null;

        private IParticleCollection m_particleCollection;

        private void OnValidate()
        {
            if (m_particlePool != null)
            {
                m_particlePool.Destroy();
                m_particlePool = null;
            }
            
        }

        private GameObjectPool<ParticleController> ParticlePool
        {
            get
            {
                if(m_particlePool == null)
                {
                    m_particlePool = new GameObjectPool<ParticleController>();
                    var child = this.transform.Find("Particles");
                    if (child != null)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                    GameObject particleContent = new GameObject("Particles");
                    particleContent.transform.SetParent(this.transform, false);
                    m_particlePool.Setup(m_particlePrefab, particleContent);
                }
                return m_particlePool;
            }
        }



        private void Awake()
        {

        }

        private void OnEnable()
        {
            Debug.Log("ParticleRender:OnEnable");
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnParticlesChanged(IParticleCollection particleCollection)
        {
            m_particleCollection = particleCollection;
            var pool = ParticlePool;
            var count = m_particleCollection.ParticleCount;
            pool.Deactive();
            for(int i = 0; i < count; i++)
            {
                var ctrl = pool.Allocate();
                var pos = m_particleCollection.GetParticlePosition(m_particleCollection.GetParticleRuntimeIndex(i));
                var radius = m_particleCollection.GetParticleRadius(m_particleCollection.GetParticleRuntimeIndex(i));
                ctrl.SetLocalPos(pos);
                ctrl.SetRadius(radius);
            }
        }
    }
}
