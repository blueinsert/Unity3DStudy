using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FluxEditor
{
	[CustomEditor(typeof(FSettings))]
	public class FSettingsInspector : Editor {

		private FSettings _fluxSettings = null;

		void OnEnable()
		{
			_fluxSettings = (FSettings)target;			
		}

		public override void OnInspectorGUI ()
		{
			GUIStyle centeredLabel = new GUIStyle( EditorStyles.largeLabel );
			centeredLabel.alignment = TextAnchor.MiddleCenter;

			EditorGUI.BeginChangeCheck();

			base.OnInspectorGUI();

			if( EditorGUI.EndChangeCheck() )
			{
				RebuildSettingsCache();
			}
		}

		private void RebuildSettingsCache()
		{
			_fluxSettings.Init();
			if( FSequenceEditorWindow.instance != null )
				FSequenceEditorWindow.instance.Repaint();
		}
	}
}
