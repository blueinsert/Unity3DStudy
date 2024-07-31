using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace bluebean
{
    [CustomEditor(typeof(ParticlesSkin))]
    public class ParticlesSkinInspector : Editor
    {
        private SerializedObject m_editTarget;
        private ParticlesSkin m_particlesSkin;
        protected IEnumerator routine;

        public virtual void OnEnable()
        {
            m_particlesSkin = target as ParticlesSkin;
            m_editTarget = new SerializedObject(m_particlesSkin);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("BindSkin"))
            {
                CoroutineJob job = new CoroutineJob();
                routine = job.Start(m_particlesSkin.BindSkin());
                EditorCoroutine.ShowCoroutineProgressBar("BindSkin...", ref routine);
                Debug.Log("BindSkin complete");
            }
        }
    }
}
