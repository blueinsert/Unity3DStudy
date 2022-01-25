using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

using Flux;

namespace FluxEditor
{
	[CustomEditor( typeof( Flux.FEvent ), true )]
	[CanEditMultipleObjects]
	public class FEventInspector : Editor
	{
		private const string FRAMERANGE_START_FIELD_ID = "FrameRange.Start";

	    private FEvent _evt;

		private bool _allEventsSameType = true;


	    protected virtual void OnEnable()
	    {
	        if( target == null )
	        {
	            DestroyImmediate( this );
	            return;

	        }
	        _evt = (Flux.FEvent)target;

			Type evtType = _evt.GetType();

			for( int i = 0; i != targets.Length; ++i )
			{
				FEvent evt = (FEvent)targets[i];
				if( evtType != evt.GetType() )
				{
					_allEventsSameType = false;
					break;
				}
			}

	    }

	    public override void OnInspectorGUI()
	    {

	        float startFrame = _evt.Start;
	        float endFrame = _evt.End;

			FrameRange validRange = _evt.Track != null ? _evt.Track.GetValidRange( _evt ) : new FrameRange(_evt.Start, _evt.End);

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel( "Range" );
			GUILayout.Label( "S:", EditorStyles.label );
			GUI.SetNextControlName( FRAMERANGE_START_FIELD_ID );
			startFrame = EditorGUILayout.IntField( _evt.Start );
			GUILayout.Label( "E:", EditorStyles.label );
			endFrame = EditorGUILayout.IntField( _evt.End );
			EditorGUILayout.EndHorizontal();
	        
			if( EditorGUI.EndChangeCheck() )
			{
				bool changedStart = GUI.GetNameOfFocusedControl() == FRAMERANGE_START_FIELD_ID;
				
				for( int i = 0; i != targets.Length; ++i )
				{
					FEvent evt = (FEvent)targets[i];
					
					FrameRange newFrameRange = evt.FrameRange;
					if( changedStart )
					{
						if( startFrame <= newFrameRange.End )
							newFrameRange.Start = (int)startFrame;
					}
					else if( endFrame >= newFrameRange.Start )
						newFrameRange.End = (int)endFrame;
					
					if( newFrameRange.Length >= evt.GetMinLength() && newFrameRange.Length <= evt.GetMaxLength() )
					{
						FSequenceEditorWindow.instance.GetSequenceEditor().MoveEvent( evt, newFrameRange );
						FEventEditor.FinishMovingEventEditors();
					}
				}
			}

			if( targets.Length == 1 )
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space( EditorGUIUtility.labelWidth );
				float sliderStartFrame = startFrame;
				float sliderEndFrame = endFrame;
		        EditorGUILayout.MinMaxSlider( ref sliderStartFrame, ref sliderEndFrame, validRange.Start, validRange.End );
				EditorGUILayout.EndHorizontal();
				if( EditorGUI.EndChangeCheck() )
				{
					FrameRange newFrameRange = new FrameRange( (int)sliderStartFrame, (int)sliderEndFrame );
					if( newFrameRange.Length < _evt.GetMinLength() )
					{
						if( sliderStartFrame != startFrame ) // changed start
							newFrameRange.Start = newFrameRange.End - _evt.GetMinLength();
						else
							newFrameRange.Length = _evt.GetMinLength();
					}

					FSequenceEditorWindow.instance.GetSequenceEditor().MoveEvent( _evt, newFrameRange );
					FEventEditor.FinishMovingEventEditors();
				}
			}

	        
			if( _allEventsSameType )
			{
				serializedObject.ApplyModifiedProperties();
				base.OnInspectorGUI();
			}
	    }

		public static void OnInspectorGUIGeneric( List<FEvent> evts )
		{
			if( evts.Count == 0 )
				return;

			int startFrame = evts[0].Start;
			int endFrame = evts[0].End;

			bool startFrameMatch = true;
			bool endFrameMatch = true;

			for( int i = 1; i < evts.Count; ++i )
			{
				if( evts[i].Start != startFrame )
				{
					startFrameMatch = false;
				}
				if( evts[i].End != endFrame )
				{
					endFrameMatch = false;
				}
			}
						
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel( "Range" );
			GUILayout.Label( "S:", EditorStyles.label );
			GUI.SetNextControlName( FRAMERANGE_START_FIELD_ID );
			startFrame = EditorGUILayout.IntField( startFrame, startFrameMatch ? EditorStyles.numberField : "PR TextField" );
			GUILayout.Label( "E:", EditorStyles.label );
			endFrame = EditorGUILayout.IntField( endFrame, endFrameMatch ? EditorStyles.numberField : "PR TextField"  );
			EditorGUILayout.EndHorizontal();
			
			if( EditorGUI.EndChangeCheck() )
			{

			}
		}
	}
}
