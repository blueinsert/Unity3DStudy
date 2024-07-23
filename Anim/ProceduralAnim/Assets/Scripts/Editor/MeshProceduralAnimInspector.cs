using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshProceduralAnim), true)]
public class MeshProceduralAnimInspector : UnityEditor.Editor
{
    private MeshProceduralAnim m_data;

    public void OnEnable()
    {
        m_data = (MeshProceduralAnim)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10);

       
        if (GUILayout.Button("GenerateMesh", GUILayout.Width(150)))
        {
            m_data.GenerateMesh();
        }
        var last = m_data.m_normalizedTime;
        m_data.m_normalizedTime = GUILayout.HorizontalSlider(m_data.m_normalizedTime, 0, 1);
        if (last != m_data.m_normalizedTime)
        {
            m_data.m_isDirty = true;
            m_data.AnimSample(m_data.m_normalizedTime);
        }
            

        EditorGUILayout.Space(10);
        EditorGUILayout.Space(10);
        EditorGUILayout.Space(10);

        EditorGUILayout.EndVertical();
    }
}
