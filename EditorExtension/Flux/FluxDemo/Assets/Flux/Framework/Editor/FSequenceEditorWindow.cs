using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Flux;

namespace FluxEditor
{
    public enum TimeFormat
    {
        Frames = 0,
        Seconds,
        SecondsFormatted
    }

    public class FSequenceEditorWindow : EditorWindow
    {
       
        #region Menus		
        [MenuItem(FSettings.MenuPath + FSettings.ProductName + "/Open Editor %&c", false, 0)]
        public static void Open()
        {
            FSequenceEditorWindow window = GetWindow<FSequenceEditorWindow>();
            window.Show();

            window.titleContent = new GUIContent(FSettings.WindownName);

            window.Update();
        }

        public static void Open(FSequence sequence)
        {
            Open();

            instance._sequenceEditor.OpenSequence(sequence);
        }

        public static FSequence CreateSequence()
        {
            // find new name & priority for sequence
            string sequenceNameFormat = FSettings.SequenceName + "_{0}";

            int sequenceId = 0;

            string sequenceName = string.Format(sequenceNameFormat, sequenceId.ToString("000"));

            FSequence[] sequences = FindObjectsOfType<FSequence>();
            for (int i = 0, limit = sequences.Length; i != limit; ++i)
            {
                if (sequences[i].name == sequenceName)
                {
                    // try new name
                    ++sequenceId;
                    sequenceName = string.Format(sequenceNameFormat, sequenceId.ToString("000"));
                    i = -1; // restart search
                }
            }

            FSequence sequence = FSequence.CreateSequence(sequenceName);
            sequence.name = sequenceName;
            sequence.Length = FSettings.DefaultLength;

            Undo.RegisterCreatedObjectUndo(sequence.gameObject, "Create Sequence");

            return sequence;
        }

        #endregion

        public static FSequenceEditorWindow instance = null;

        // size of the whole window, cached to determine if it was resized
        private Rect _windowRect;

        // rect used for the header at the top of the window
        private Rect _windowHeaderRect;

        // header
        private FSequenceWindowHeader _windowHeader;

        // toolbar
        private FSequenceWindowToolbar _toolbar;

        // area for the toolbar
        private Rect _toolbarRect;

        [SerializeField]
        private FSequenceEditor _sequenceEditor;

        void OnEnable()
        {
            instance = this;
            wantsMouseMove = true;

            minSize = new Vector2(450, 300);

            _windowHeader = new FSequenceWindowHeader(this);

            _toolbar = new FSequenceWindowToolbar(this);

            _windowRect = new Rect();

            FUtility.LoadPreferences();
        }

        void OnSelectionChange()
        {
            if (!FUtility.OpenSequenceOnSelect)
                return;

            FSequence sequence = Selection.activeGameObject == null || PrefabUtility.GetPrefabType(Selection.activeGameObject) == PrefabType.Prefab ? null : Selection.activeGameObject.GetComponent<FSequence>();

            if (sequence != null)
            {
                Open(sequence);
            }
        }

        public FSequenceEditor GetSequenceEditor()
        {
            return _sequenceEditor;
        }

        void OnDestroy()
        {
            if (_sequenceEditor != null)
            {
                DestroyImmediate(_sequenceEditor);
            }
        }

        void OnLostFocus()
        {
        }

        #region Editor state changes hookups
        private void OnDidOpenScene()
        {
            if (_sequenceEditor)
                _sequenceEditor.OpenSequence(null);
        }

        #endregion


        void Update()
        {
#if FLUX_PROFILE
			Profiler.BeginSample("flux Update");
#endif
            if (_sequenceEditor == null)
            {
                _sequenceEditor = FSequenceEditor.CreateInstance<FSequenceEditor>();
                _sequenceEditor.Init(this);
            }

            FSequence sequence = _sequenceEditor.Sequence;

            if (Application.isPlaying && sequence != null && FUtility.RenderOnEditorPlay)
            {
                Repaint();
            }

#if FLUX_PROFILE
			Profiler.EndSample();
#endif
        }

        private void RebuildLayout()
        {
            _windowRect = position;
            _windowRect.x = 0;
            _windowRect.y = 0;

            _windowHeaderRect = _windowRect;
            _windowHeaderRect.height = FSequenceWindowHeader.HEIGHT;

            _windowHeader.RebuildLayout(_windowHeaderRect);

            _toolbarRect = _windowRect;
            _toolbarRect.yMin = _toolbarRect.yMax - FSequenceWindowToolbar.HEIGHT;

            _toolbar.RebuildLayout(_toolbarRect);

            Rect timelineViewRect = _windowRect;
            timelineViewRect.yMin += FSequenceWindowHeader.HEIGHT;
            timelineViewRect.yMax -= FSequenceWindowToolbar.HEIGHT;

            _sequenceEditor.RebuildLayout(timelineViewRect);

            Repaint();
        }


        public void Refresh()
        {
            if (_sequenceEditor != null)
            {
                _sequenceEditor.OpenSequence(_sequenceEditor.Sequence);
            }

            Repaint();
        }

        public static void RefreshIfOpen()
        {
            if (instance != null)
                instance.Refresh();
        }

        void OnGUI()
        {
#if FLUX_PROFILE
			Profiler.BeginSample("Flux OnGUI");
#endif
            if (_sequenceEditor == null)
                return;

            Rect currentWindowRect = position;
            currentWindowRect.x = 0;
            currentWindowRect.y = 0;

            if (currentWindowRect != _windowRect)
            {
                RebuildLayout();
            }

            if (!FUtility.RenderOnEditorPlay && EditorApplication.isPlaying && !EditorApplication.isPaused)
            {
                GUI.Label(_windowRect, "Draw in play mode is disabled. You can change it on Flux Preferences");
                return;
            }

            FSequence sequence = _sequenceEditor.Sequence;

            if (sequence == null)
                ShowNotification(new GUIContent("Select Or Create Sequence"));
            else if (Event.current.isKey)
            {
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                {
                    EditorGUIUtility.keyboardControl = 0;
                    Event.current.Use();
                    Repaint();
                }
            }

            // header
            _windowHeader.OnGUI();

            if (sequence == null)
                return;

            // toolbar
            _toolbar.OnGUI();

            switch (Event.current.type)
            {
                case EventType.KeyDown:
                    if (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete)
                    {
                        _sequenceEditor.DestroyEvents(_sequenceEditor.EventSelection.Editors);
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDown:
                    break;

                case EventType.MouseUp:
                    break;
            }

            if (Event.current.type == EventType.ValidateCommand)
            {
                Repaint();
            }

            _sequenceEditor.OnGUI();


            // because of a bug with windows editor, we have to not catch right button
            // otherwise ContextClick doesn't get called
            if (Event.current.type == EventType.MouseUp && Event.current.button != 1)
            {
                Event.current.Use();
            }

            if (Event.current.type == EventType.Ignore)
            {
                EditorGUIUtility.hotControl = 0;
            }

            if (Event.current.type == EventType.Repaint)
            {
                Handles.DrawLine(new Vector3(_windowHeaderRect.xMin, _windowHeaderRect.yMax, 0), new Vector3(_windowHeaderRect.xMax - FSequenceEditor.RIGHT_BORDER, _windowHeaderRect.yMax, 0));
            }
#if FLUX_PROFILE
			Profiler.EndSample();
#endif
        }

    }
}
