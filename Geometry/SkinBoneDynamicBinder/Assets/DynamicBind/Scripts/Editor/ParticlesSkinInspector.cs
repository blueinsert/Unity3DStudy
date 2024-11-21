using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace bluebean
{
    [CustomEditor(typeof(SkinBoneDynamicBinder))]
    public class ParticlesSkinInspector : Editor
    {
        private SerializedObject m_editTarget;
        private SkinBoneDynamicBinder m_binder;
        protected IEnumerator routine;

        public virtual void OnEnable()
        {
            m_binder = target as SkinBoneDynamicBinder;
            m_editTarget = new SerializedObject(m_binder);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            if (GUILayout.Button("BindSkin"))
            {
                CoroutineJob job = new CoroutineJob();
                routine = job.Start(m_binder.BindSkin());
                EditorCoroutine.ShowCoroutineProgressBar("BindSkin...", ref routine);
                Debug.Log("BindSkin complete");
                EditorUtility.SetDirty(m_binder);
                EditorGUIUtility.ExitGUI();
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
