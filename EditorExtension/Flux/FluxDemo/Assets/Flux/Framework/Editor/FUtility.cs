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

		private const bool DEFAULT_OPEN_SEQUENCE_ON_SELECT = false;
		private const bool DEFAULT_RENDER_ON_EDITOR_PLAY = true;
		private const bool DEFAULT_COLLAPSE_COMMENT_TRACKS = false;

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

			_openSequenceOnSelect = EditorGUILayout.Toggle( new GUIContent("Open Sequence On Select", "Should it open Sequences in Flux Editor window when they are selected?"), _openSequenceOnSelect );
			_renderOnEditorPlay = EditorGUILayout.Toggle( new GUIContent("Render On Editor Play", "Render Flux Editor window when editor is in Play mode? Turn it off it you want to avoid the redraw costs of Flux when selected sequence is playing."), _renderOnEditorPlay );
			_collapseCommentTracks = EditorGUILayout.Toggle( new GUIContent("Collapse Comment Tracks", "Collapse the comment tracks so that they are slimmer and don't build preview textures."), _collapseCommentTracks );

			if( EditorGUI.EndChangeCheck() )
				SavePreferences();
		}
		
		public static void LoadPreferences()
		{
			_preferencesLoaded = true;

			_openSequenceOnSelect = EditorPrefs.GetBool( OPEN_SEQUENCE_ON_SELECT_KEY, DEFAULT_OPEN_SEQUENCE_ON_SELECT );
			_renderOnEditorPlay = EditorPrefs.GetBool( RENDER_ON_EDITOR_PLAY_KEY, DEFAULT_RENDER_ON_EDITOR_PLAY );
			_collapseCommentTracks = EditorPrefs.GetBool( COLLAPSE_COMMENT_TRACKS_KEY, DEFAULT_COLLAPSE_COMMENT_TRACKS );
		}

		public static void SavePreferences()
		{
			EditorPrefs.SetBool( OPEN_SEQUENCE_ON_SELECT_KEY, _openSequenceOnSelect );
			EditorPrefs.SetBool( RENDER_ON_EDITOR_PLAY_KEY, _renderOnEditorPlay );
			EditorPrefs.SetBool( COLLAPSE_COMMENT_TRACKS_KEY, _collapseCommentTracks );
		}

		#endregion Preferences

		#region Paths / Resource Loading

		private static string _fluxPath = null;
		private static string _fluxEditorPath = null;
		private static string _fluxSkinPath = null;

		private static GUISkin _fluxSkin = null;
		private static GUIStyle _evtStyle = null;
		private static GUIStyle _simpleButtonStyle = null;

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


		public static string GetTime( int frame)
		{
			return frame.ToString();
		}

		#region Upgrade Sequences

		public static void Upgrade( FSequence sequence )
		{
			
		}


		#endregion

		public static string GetAssetPathFromFullPath(string fullPath)
        {
			var index = fullPath.IndexOf("Assets");
			var res = fullPath.Substring(index);
			return res;
		}

		public static string RemoveCloneStr(string fileName)
        {
			var index = fileName.IndexOf("(Clone)");
			var res = fileName.Substring(0, index);
			return res;
        }
	}
}
