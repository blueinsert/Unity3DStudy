using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace bluebean
{
    [CustomEditor(typeof(ParticleTopology))]
    public class ParticleTopologyInspector : Editor
    {
        private SerializedObject m_editTarget;
        private ParticleTopology m_particleTopology;
        protected IEnumerator routine;

        public virtual void OnEnable()
        {
            m_particleTopology = target as ParticleTopology;
            m_editTarget = new SerializedObject(m_particleTopology);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate"))
            {
                CoroutineJob job = new CoroutineJob();
                routine = job.Start(m_particleTopology.Generate());
                EditorCoroutine.ShowCoroutineProgressBar("Generating ParticleTopology...", ref routine);
                Debug.Log("Generate complete");
                //var render = m_particleTopology.GetComponent<ParticlesRender>();
                //if (render != null)
                //{
                //    render.OnParticlesChanged(m_particleTopology);
                //}
            }
        }
    }
}
