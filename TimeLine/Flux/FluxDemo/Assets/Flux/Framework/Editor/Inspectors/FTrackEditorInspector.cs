using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using Flux;

namespace FluxEditor
{
	[Serializable]
	public class FTrackEditorInspector : FEditorInspector<FTrackEditor, FTrack> {

		public override string Title {
			get {
				if( _editors.Count == 1 )
					return "Track:";
				return "Tracks:";
			}
		}

		public FTrackEditorInspector()
		{
		}

		public override void OnInspectorGUI( float contentWidth )
		{
			base.OnInspectorGUI(contentWidth);

			if( _editors.Count == 1 && _editors[0].HasTools() )
			{
				GUILayout.Space( 10 );

				EditorGUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Width(contentWidth));
				EditorGUILayout.LabelField("Track Tools:", EditorStyles.boldLabel);
				_editors[0].OnToolsGUI();
				EditorGUILayout.EndVertical();

			}
		}

		public override void Refresh()
		{
			base.Refresh();

			if( _inspector != null && _inspector is FTrackInspector )
			{
				((FTrackInspector)_inspector).ShowEvents = false;
			}
		}
	}
}
