using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
//using UnityEditorInternal;

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Flux;

namespace FluxEditor
{

	public static class FUtility
	{
		#region Preferences
		private static bool _preferencesLoaded = false;

		// keys
		private const string FLUX_KEY = "Flux.";

		private const string TIME_FORMAT_KEY = FLUX_KEY + "TimeFormat";
		private const string FRAME_RATE_KEY = FLUX_KEY + "FrameRate";
		private const string OPEN_SEQUENCE_ON_SELECT_KEY = FLUX_KEY + "OpenSequenceOnSelect";
		private const string RENDER_ON_EDITOR_PLAY_KEY = FLUX_KEY + "ReplainOnEditorPlay";
		private const string COLLAPSE_COMMENT_TRACKS_KEY = FLUX_KEY + "CollapseCommentTracks";

		// defaults
		private const TimeFormat DEFAULT_TIME_FORMAT = TimeFormat.Frames;
		private const int DEFAULT_FRAME_RATE = FSequence.DEFAULT_FRAMES_PER_SECOND;
		private const bool DEFAULT_OPEN_SEQUENCE_ON_SELECT = false;
		private const bool DEFAULT_RENDER_ON_EDITOR_PLAY = true;
		private const bool DEFAULT_COLLAPSE_COMMENT_TRACKS = false;

		// current values
		private static TimeFormat _timeFormat = DEFAULT_TIME_FORMAT;
		public static TimeFormat TimeFormat { get { return _timeFormat; } }

		private static int _frameRate = DEFAULT_FRAME_RATE;
		public static int FrameRate { get { return _frameRate; } }

		private static bool _openSequenceOnSelect = DEFAULT_OPEN_SEQUENCE_ON_SELECT;
		public static bool OpenSequenceOnSelect { get { return _openSequenceOnSelect; } }

		private static bool _renderOnEditorPlay = DEFAULT_RENDER_ON_EDITOR_PLAY;
		public static bool RenderOnEditorPlay { get { return _renderOnEditorPlay; } }

		private static bool _collapseCommentTracks = DEFAULT_COLLAPSE_COMMENT_TRACKS;
		public static bool CollapseCommentTracks { get { return _collapseCommentTracks; } }

		[PreferenceItem("Flux Editor")]
		public static void PreferencesGUI()
		{
			if( !_preferencesLoaded )
				LoadPreferences();

			EditorGUI.BeginChangeCheck();

			_timeFormat = (TimeFormat)EditorGUILayout.EnumPopup( new GUIContent("Time Format", "In what format is time represented? E.g. for 60fps:\n\tFrames: 127\n\tSeconds: 2.12\n\tSeconds Formatted: 2.7"), _timeFormat );
			_frameRate = EditorGUILayout.IntSlider( new GUIContent("Frame Rate", "How many frames per second have the Sequences by default?"), _frameRate, 1, 120 );
			_openSequenceOnSelect = EditorGUILayout.Toggle( new GUIContent("Open Sequence On Select", "Should it open Sequences in Flux Editor window when they are selected?"), _openSequenceOnSelect );
			_renderOnEditorPlay = EditorGUILayout.Toggle( new GUIContent("Render On Editor Play", "Render Flux Editor window when editor is in Play mode? Turn it off it you want to avoid the redraw costs of Flux when selected sequence is playing."), _renderOnEditorPlay );
			_collapseCommentTracks = EditorGUILayout.Toggle( new GUIContent("Collapse Comment Tracks", "Collapse the comment tracks so that they are slimmer and don't build preview textures."), _collapseCommentTracks );

			if( EditorGUI.EndChangeCheck() )
				SavePreferences();
		}
		
		public static void LoadPreferences()
		{
			_preferencesLoaded = true;

			_timeFormat = (TimeFormat)EditorPrefs.GetInt( TIME_FORMAT_KEY, (int)DEFAULT_TIME_FORMAT );
			_frameRate = EditorPrefs.GetInt( FRAME_RATE_KEY, DEFAULT_FRAME_RATE );
			_openSequenceOnSelect = EditorPrefs.GetBool( OPEN_SEQUENCE_ON_SELECT_KEY, DEFAULT_OPEN_SEQUENCE_ON_SELECT );
			_renderOnEditorPlay = EditorPrefs.GetBool( RENDER_ON_EDITOR_PLAY_KEY, DEFAULT_RENDER_ON_EDITOR_PLAY );
			_collapseCommentTracks = EditorPrefs.GetBool( COLLAPSE_COMMENT_TRACKS_KEY, DEFAULT_COLLAPSE_COMMENT_TRACKS );
		}

		public static void SavePreferences()
		{
			EditorPrefs.SetInt( TIME_FORMAT_KEY, (int)_timeFormat );
			EditorPrefs.SetInt( FRAME_RATE_KEY, _frameRate );
			EditorPrefs.SetBool( OPEN_SEQUENCE_ON_SELECT_KEY, _openSequenceOnSelect );
			EditorPrefs.SetBool( RENDER_ON_EDITOR_PLAY_KEY, _renderOnEditorPlay );
			EditorPrefs.SetBool( COLLAPSE_COMMENT_TRACKS_KEY, _collapseCommentTracks );
		}

		#endregion Preferences

		#region Paths / Resource Loading

		private static string _fluxPath = null;
		private static string _fluxEditorPath = null;
		private static string _fluxSkinPath = null;

		private static EditorWindow _gameView = null;

		private static GUISkin _fluxSkin = null;
		private static GUIStyle _evtStyle = null;
		private static GUIStyle _simpleButtonStyle = null;
//		private static GUIStyle _commentEvtStyle = null;

		public static string FindFluxDirectory()
		{
			string[] directories = Directory.GetDirectories("Assets", "Flux", SearchOption.AllDirectories);
			return directories.Length > 0 ? directories[0] : string.Empty;
		}

		
		public static string GetFluxPath()
		{
			if( _fluxPath == null )
			{
				_fluxPath = FindFluxDirectory()+'/';
				_fluxEditorPath = _fluxPath + "Framework/Editor/";
				_fluxSkinPath = _fluxEditorPath + "Skin/";
			}
			return _fluxPath;
		}
		
		public static string GetFluxEditorPath()
		{
			if( _fluxEditorPath == null ) GetFluxPath();
			return _fluxEditorPath;
		}
		
		public static string GetFluxSkinPath()
		{
			if( _fluxSkinPath == null ) GetFluxPath();
			return _fluxSkinPath;
		}

		public static GUISkin GetFluxSkin()
		{
			if( _fluxSkin == null )
				_fluxSkin = (GUISkin)AssetDatabase.LoadAssetAtPath( GetFluxSkinPath()+"FSkin.guiskin", typeof(GUISkin) );
			return _fluxSkin;
		}

		public static Texture2D GetFluxTexture( string textureFile )
		{
			return (Texture2D)AssetDatabase.LoadAssetAtPath( GetFluxSkinPath() + textureFile, typeof(Texture2D) );
		}

		public static GUIStyle GetEventStyle()
		{
			if( _evtStyle == null )
				_evtStyle = GetFluxSkin().GetStyle("Event");
			return _evtStyle;
		}

		public static GUIStyle GetCommentEventStyle()
		{
			return GetEventStyle();
//			if( _commentEvtStyle == null )
//				_commentEvtStyle = GetFluxSkin().GetStyle("CommentEvent");
//			return _commentEvtStyle;
		}

		public static GUIStyle GetSimpleButtonStyle()
		{
			if( _simpleButtonStyle == null )
				_simpleButtonStyle = GetFluxSkin().GetStyle("SimpleButton");
			return _simpleButtonStyle;
		}

		private static FSettings _settings = null;

		public static Color GetEventColor( string eventTypeStr )
		{
			return GetSettings().GetEventColor( eventTypeStr );
		}

		public static FSettings GetSettings()
		{
			if( _settings == null )
				_settings = (FSettings)AssetDatabase.LoadAssetAtPath( GetFluxEditorPath() + "FluxSettings.asset", typeof(FSettings) );
			return _settings;
		}

		#endregion Paths / Resource Loading

		#region Events

//		public static void Resize( FEvent evt, FrameRange newFrameRange )
//		{
//			if( evt.FrameRange == newFrameRange || newFrameRange.Start > newFrameRange.End )
//				return;
//			
//			if( newFrameRange.Length < evt.GetMinLength() || newFrameRange.Length > evt.GetMaxLength() )
//			{
//				Debug.LogError( string.Format("Trying to resize an Event to [{0},{1}] (length: {2}) which isn't a valid length, should be between [{3},{4}]", newFrameRange.Start, newFrameRange.End, newFrameRange.Length, evt.GetMinLength(), evt.GetMaxLength() ), evt );
//				return;
//			}
//			
//			bool changedLength = evt.Length != newFrameRange.Length;
//
//			if( !evt.GetTrack().CanAdd( evt, newFrameRange ) )
//				return;
//
//			Undo.RecordObject( evt, changedLength ? "Resize Event" : "Move Event" );
//			
//			evt.Start = newFrameRange.Start;
//			evt.End = newFrameRange.End;
//			
//			if( changedLength )
//			{
//				if( evt is FPlayAnimationEvent )
//				{
//					FPlayAnimationEvent animEvt = (FPlayAnimationEvent)evt;
//					
//					if( animEvt.IsAnimationEditable() )
//					{
//						FAnimationEventInspector.ScaleAnimationClip( animEvt._animationClip, animEvt.FrameRange );
//					}
//				}
//				else if( evt is FTimescaleEvent )
//				{
//					ResizeAnimationCurve( (FTimescaleEvent)evt, newFrameRange );
//				}
//			}
//			
//			EditorUtility.SetDirty( evt );
//			
//			if( FSequenceEditorWindow.instance != null )
//				FSequenceEditorWindow.instance.Repaint();
//		}
		
		public static void Rescale( FEvent evt, FrameRange newFrameRange )
		{
			if( evt.FrameRange == newFrameRange )
				return;
			
			Undo.RecordObject( evt, string.Empty );
			
			evt.Start = newFrameRange.Start;
			evt.End = newFrameRange.End;
			
			EditorUtility.SetDirty( evt );
		}

		#endregion Events


		public static void RepaintGameView()
		{
			if( _gameView == null )
				_gameView = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType( "UnityEditor.GameView" ));

			_gameView.Repaint();
			SceneView.RepaintAll();
//			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}

		public static string GetTime( int frame, int frameRate )
		{
			switch( _timeFormat )
			{
			case TimeFormat.Frames:
				return frame.ToString();
			case TimeFormat.Seconds:
				return ((float)frame / frameRate).ToString("0.##");
			case TimeFormat.SecondsFormatted:
				return string.Format( "{0}:{1}", frame / frameRate, frame % frameRate );
			}

			return frame.ToString();
		}

		#region Audio Utilities

		public static float[] GetAudioMinMaxData( AudioClip clip ) 
		{
			if( clip == null )
				return null;

			AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( clip ) );

			MethodInfo getMinMaxdata = typeof(Editor).Assembly.GetType("UnityEditor.AudioUtil").GetMethod("GetMinMaxData", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);

			return (float[])getMinMaxdata.Invoke( null, new object[]{importer} );
		}

		#endregion

		#region Editor Window

		public static EditorWindow GetWindowIfExists( Type windowType )
		{
			UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll( windowType );

			foreach( UnityEngine.Object o in objs )
			{
				if( o.GetType() == windowType )
					return (EditorWindow)o;
			}

			return null;
		}

//		public static EditorWindow GetAnimationWindow( bool showWindow )
//		{
//			System.Type t = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");
//			if( showWindow )
//			{
//				return EditorWindow.GetWindow( t );
//			}
//
//			System.Reflection.MethodInfo getWindow = typeof(EditorWindow).GetMethod( "GetWindowDontShow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static  );
//			System.Reflection.MethodInfo getWindowGeneric = getWindow.MakeGenericMethod( t );
//			return (EditorWindow)getWindowGeneric.Invoke(null, null);
//		}

		#endregion

		#region Upgrade Sequences

		public static void Upgrade( FSequence sequence )
		{
			
		}


		#endregion
	}
}
