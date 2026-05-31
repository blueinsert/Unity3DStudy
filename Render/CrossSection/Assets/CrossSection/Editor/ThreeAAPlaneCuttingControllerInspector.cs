using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ThreeAAPlaneCuttingController))]
public class ThreeAAPlaneCuttingControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Ë¢ÐÂ"))
        {
            (target as ThreeAAPlaneCuttingController).RefreshFromExternal();
        }
    }
}
