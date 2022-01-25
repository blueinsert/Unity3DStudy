using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using Flux;
using FluxEditor;

namespace FluxEditor
{
	[CustomEditor(typeof(FSequence))]
	public class FSequenceInspector : Editor {

		private FSequence _sequence;

		private bool _advancedInspector = false;

		private SerializedProperty _content = null;

		void OnEnable()
		{
			_sequence = (FSequence)target;

			_content = serializedObject.FindProperty("_content");
		}

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			EditorGUILayout.Space();

			if( GUILayout.Button( "Open In Flux Editor" ) )
			{
				FSequenceEditorWindow.Open( _sequence );
			}

			EditorGUILayout.Space();

			if( GUILayout.Button( _advancedInspector ? "Normal Inspector" : "Advanced Inspector") )
				_advancedInspector = !_advancedInspector;

			if( _advancedInspector )
			{
				EditorGUILayout.PropertyField( _content );

				bool showContent = (_sequence.Content.hideFlags & HideFlags.HideInHierarchy) == 0;

				EditorGUI.BeginChangeCheck();
				showContent = EditorGUILayout.Toggle( "Show Content", showContent );
				if( EditorGUI.EndChangeCheck() )
				{
					if( showContent )
					{
						_sequence.Content.transform.hideFlags &= ~HideFlags.HideInHierarchy;
					}
					else
					{
						_sequence.Content.transform.hideFlags |= HideFlags.HideInHierarchy;
					}

				}
			}
			serializedObject.ApplyModifiedProperties();
		}

	}
}
