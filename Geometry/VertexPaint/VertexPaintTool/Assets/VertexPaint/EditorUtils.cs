using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace bluebean
{

    public static class EditorUtils
	{
        public static int DoToolBar(int selected, GUIContent[] items)
        {
            // Keep the selected index within the bounds of the items array
            selected = selected < 0 ? 0 : selected >= items.Length ? items.Length - 1 : selected;

            GUIStyle style = GUI.skin.FindStyle("Button");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int i = 0; i < items.Length; i++)
            {
                if (i == 0 && items.Length > 1)
                    style = GUI.skin.FindStyle("ButtonLeft");
                else if (items.Length > 1 && i == items.Length-1)
                    style = GUI.skin.FindStyle("ButtonRight");
                else if (i > 0)
                    style = GUI.skin.FindStyle("ButtonMid");
                    

                // Display toggle. Get if toggle changed.
                bool change = GUILayout.Toggle(selected == i, items[i],style,GUILayout.Height(24));
                // If changed, set selected to current index.
                if (change)
                    selected = i;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Return the currently selected item's index
            return selected;
        }
	}
}


