using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TwoPointFiveDimensionAAQuad))]
public class TwoPointFiveDimensionAAQuadEditor : Editor
{
    TwoPointFiveDimensionAAQuad m_target;

    private void OnEnable()
    {
        m_target = target as TwoPointFiveDimensionAAQuad;
    }

    public override void OnInspectorGUI()
    {
        m_target.m_axis = (TwoPointFiveDimensionAAQuadAxis)EditorGUILayout.EnumPopup("Axis",m_target.m_axis);
        switch (m_target.m_axis)
        {
            case TwoPointFiveDimensionAAQuadAxis.x:
                m_target.yz_plane = EditorGUILayout.FloatField("x", m_target.yz_plane);
                m_target.m_min.x = m_target.yz_plane;
                m_target.m_max.x = m_target.yz_plane;
                break;
            case TwoPointFiveDimensionAAQuadAxis.y:
                m_target.xz_plane = EditorGUILayout.FloatField("y", m_target.xz_plane);
                m_target.m_min.y = m_target.xz_plane;
                m_target.m_max.y = m_target.xz_plane;
                break;
            case TwoPointFiveDimensionAAQuadAxis.z:
                m_target.xy_plane = EditorGUILayout.FloatField("z", m_target.xy_plane);
                m_target.m_min.z = m_target.xy_plane;
                m_target.m_max.z = m_target.xy_plane;
                break;
        }
        m_target.m_min = EditorGUILayout.Vector3Field("min", m_target.m_min);
        m_target.m_max = EditorGUILayout.Vector3Field("max", m_target.m_max);
        
        m_target.m_localCoord = (TwoPointFiveDimensionCoordianteSystem)EditorGUILayout.ObjectField("LocalCoord", m_target.m_localCoord, typeof(TwoPointFiveDimensionCoordianteSystem));

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}