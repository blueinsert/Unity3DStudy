using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace bluebean
{
    public enum WayPointsComponentEditMode
    {
        None,
        Editing,
    }

    [CustomEditor(typeof(WayPointsComponent))]
    public class WayPointsComponentEditor : Editor
    {
        public SerializedObject EditTarget;
        protected WayPointsComponent m_wayPointsComponent;
        private WayPointsComponentEditMode m_editMode = WayPointsComponentEditMode.None;

        public virtual void OnEnable()
        {
            m_wayPointsComponent = target as WayPointsComponent;
            EditTarget = new SerializedObject(m_wayPointsComponent);
        }

        public override void OnInspectorGUI()
        {
            EditTarget.Update();

            DrawDefaultInspector();
            EditorGUILayout.Separator();
            if(m_editMode == WayPointsComponentEditMode.None)
            {
                if (GUILayout.Button("Edit"))
                {
                    BeginEdit();
                }
            }else if (m_editMode == WayPointsComponentEditMode.Editing)
            {
                if (GUILayout.Button("EndEdit"))
                {
                    EndEdit();
                }
            }
            if (GUILayout.Button("Clear"))
            {
                Clear();
            }
            EditTarget.ApplyModifiedProperties();
        }

        private void Clear()
        {
            m_wayPointsComponent.Clear();
        }

        private void SetInspectorLockState(bool state)
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            var window = EditorWindow.GetWindow(type);
            PropertyInfo propertyInfo = type.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
            bool isLocked = (bool)propertyInfo.GetValue(window, null);
            if (isLocked != state)
                propertyInfo.SetValue(window, state, null);
        }

        private void BeginEdit()
        {
            m_wayPointsComponent.Clear();
            m_editMode = WayPointsComponentEditMode.Editing;
            SetInspectorLockState(true);
        }

        private void EndEdit()
        {
            m_editMode = WayPointsComponentEditMode.None;
            SetInspectorLockState(false);
            Selection.activeGameObject = m_wayPointsComponent.gameObject;
        }

        //获得与x-y平面的交点
        private bool GetIntersectPoint(Ray ray, out Vector3 intersectPoint)
        {
            intersectPoint = Vector3.zero;
            if (ray.origin.z*ray.direction.z > 0)
            {
                return false;
            }
            //平面法线
            Vector3 normal = new Vector3(0, 0, 1);
            float n = ray.direction.magnitude / Mathf.Abs(Vector3.Dot(normal, ray.direction));
            intersectPoint = ray.origin + ray.direction * Mathf.Abs(ray.origin.z) * n;
            intersectPoint.z = 0;
            return true;
        }

        private void OnSceneGUI()
        {
            if(Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                var mousePos = Event.current.mousePosition;
                var ray = HandleUtility.GUIPointToWorldRay(mousePos);
                Vector3 intersectPoint;
                if(GetIntersectPoint(ray, out intersectPoint))
                {
                    m_wayPointsComponent.AddNode(intersectPoint);
                }
            }
        }


    }
}
