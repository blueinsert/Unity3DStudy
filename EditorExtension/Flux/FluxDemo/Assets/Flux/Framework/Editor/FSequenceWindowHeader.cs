using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using Flux;

namespace FluxEditor
{
    public class FSequenceWindowHeader
    {
        // padding on top, bottom, left and right
        private const float PADDING = 5;
        // space between labels and the fields
        private const float LABEL_SPACE = 5;
        // space between elements (i.e. label+field pairs)
        private const float ELEMENT_SPACE = 20;
        // height of the header
        public const float HEIGHT = 20 + PADDING + PADDING;
        private const float MAX_SEQUENCE_POPUP_WIDTH = 250;
        private const float FRAMERATE_FIELD_WIDTH = 40;
        private const float LENGTH_FIELD_WIDTH = 100;

        private FSequenceEditorWindow _sequenceWindow;
        private SerializedObject _sequenceSO;
        private SerializedProperty _sequenceLength;

        private GUIContent _loadLabel = new GUIContent("加载", "加载技能");
        private Rect _loadRect;

        // sequence selection
        // sequence selection popup variables
        private GUIContent _sequenceLabel = new GUIContent("选择", string.Format("选择{0}...", FSettings.SequenceName));
        // rect of the sequence label
        private Rect _sequenceLabelRect;
        // rect of the sequence name
        private Rect _sequencePopupRect;

        private FSequence[] _sequences;
        private GUIContent[] _sequenceNames;
        private int _selectedSequenceIndex;

        //add Container
        private GUIContent _addContainerLabel = new GUIContent(string.Empty, "Add Container To Sequence");
        private Rect _addContainerRect;

        private GUIContent _saveLabel = new GUIContent("Save", "保存当前技能");
        private Rect _saveRect;

        private GUIContent _saveAllLabel = new GUIContent("保存所有", "保存当前技能");
        private Rect _saveAllRect;

        // length UI variables
        private GUIContent _lengthLabel = new GUIContent("Length", "What's the length of the sequence");
        private Rect _lengthLabelRect;
        private Rect _lengthFieldRect;

        // open inspector
        private GUIContent _openInspectorLabel = new GUIContent(string.Empty, "Open Flux Inspector");
        private Rect _openInspectorRect;

        // cached number field style, since we want numbers centered
        private GUIStyle _numberFieldStyle;

        public FSequenceWindowHeader(FSequenceEditorWindow sequenceWindow)
        {
            _sequenceWindow = sequenceWindow;

            RebuildSequenceList();

            //EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;

            _addContainerLabel.image = FUtility.GetFluxTexture("AddFolder.png");
            _openInspectorLabel.image = FUtility.GetFluxTexture("Inspector.png");
        }

        private void OnHierarchyChanged()
        {
            RebuildSequenceList();
        }

        private void RebuildSequenceList()
        {
            _sequences = GameObject.FindObjectsOfType<FSequence>();
            System.Array.Sort<FSequence>(_sequences, delegate (FSequence x, FSequence y) { return x.name.CompareTo(y.name); });

            _sequenceNames = new GUIContent[_sequences.Length + 1];
            for (int i = 0; i != _sequences.Length; ++i)
            {
                _sequenceNames[i] = new GUIContent(_sequences[i].name);
            }

            _sequenceNames[_sequenceNames.Length - 1] = new GUIContent(string.Format("[创建新{0}]", FSettings.SequenceName));
            _selectedSequenceIndex = -1;
        }

        public void RebuildLayout(Rect rect)
        {
            rect.xMin += PADDING;
            rect.yMin += PADDING;
            rect.xMax -= PADDING;
            rect.yMax -= PADDING;

            float width = rect.width;

            _loadRect = rect;
            _saveAllRect = rect;
            _saveRect = rect;
            _openInspectorRect = rect;
            _lengthLabelRect = _lengthFieldRect = rect;
            _sequenceLabelRect = rect;
            _sequencePopupRect = rect;

            _loadRect.width = EditorStyles.label.CalcSize(_loadLabel).x + LABEL_SPACE;
            _saveAllRect.width = EditorStyles.label.CalcSize(_saveAllLabel).x + LABEL_SPACE;
            _saveRect.width = EditorStyles.label.CalcSize(_saveLabel).x + LABEL_SPACE;
            _lengthLabelRect.width = EditorStyles.label.CalcSize(_lengthLabel).x + LABEL_SPACE;
            _lengthFieldRect.width = LENGTH_FIELD_WIDTH;
            _sequenceLabelRect.width = EditorStyles.label.CalcSize(_sequenceLabel).x + LABEL_SPACE;
            _sequencePopupRect.width = Mathf.Min(width - _sequenceLabelRect.width, MAX_SEQUENCE_POPUP_WIDTH);
            _addContainerRect = new Rect(0, 3, 22, 22);

            _loadRect.xMin = rect.xMin;
            _sequenceLabelRect.xMin = _loadRect.xMax + LABEL_SPACE;
            _sequencePopupRect.xMin = _sequenceLabelRect.xMax;
            _addContainerRect.x = _sequencePopupRect.xMax + LABEL_SPACE;
            _openInspectorRect.xMin = _openInspectorRect.xMax - 22;
            _lengthFieldRect.x = rect.xMax - 22 - PADDING - _lengthFieldRect.width;
            _lengthLabelRect.x = _lengthFieldRect.xMin - _lengthLabelRect.width;
            _saveRect.x = _lengthLabelRect.xMin - _saveRect.width;
            _saveAllRect.x = _saveRect.xMin - _saveAllRect.width;
            _numberFieldStyle = new GUIStyle(EditorStyles.numberField);
            _numberFieldStyle.alignment = TextAnchor.MiddleCenter;
        }

        public void OnGUI()
        {
            FSequence sequence = _sequenceWindow.GetSequenceEditor().Sequence;

            if ((_selectedSequenceIndex < 0 && sequence != null) || (_selectedSequenceIndex >= 0 && _sequences[_selectedSequenceIndex] != sequence))
            {
                for (int i = 0; i != _sequences.Length; ++i)
                {
                    if (_sequences[i] == sequence)
                    {
                        _selectedSequenceIndex = i;
                        break;
                    }
                }
            }
            if (FGUI.Button(_loadRect, _loadLabel))
            {
                LoadSequence();
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.PrefixLabel(_sequenceLabelRect, _sequenceLabel);
            int newSequenceIndex = EditorGUI.Popup(_sequencePopupRect, _selectedSequenceIndex, _sequenceNames);
            if (EditorGUI.EndChangeCheck())
            {
                if (newSequenceIndex == _sequenceNames.Length - 1)
                {
                    FSequence newSequence = FSequenceEditorWindow.CreateSequence();
                    Selection.activeTransform = newSequence.transform;
                    _sequenceWindow.GetSequenceEditor().OpenSequence(newSequence);
                }
                else
                {
                    _selectedSequenceIndex = newSequenceIndex;
                    _sequenceWindow.GetSequenceEditor().OpenSequence(_sequences[_selectedSequenceIndex]);
                    _sequenceWindow.RemoveNotification();
                }
                EditorGUIUtility.keyboardControl = 0; // deselect it
                EditorGUIUtility.ExitGUI();
            }
            
            if (sequence == null)
                return;

            if (_sequenceSO == null || _sequenceSO.targetObject != sequence)
            {
                _sequenceSO = new SerializedObject(sequence);
                _sequenceLength = _sequenceSO.FindProperty("_length");
            }
            _sequenceSO.Update();

            EditorGUI.PrefixLabel(_lengthLabelRect, _lengthLabel);
            _sequenceLength.intValue = Mathf.Clamp(EditorGUI.IntField(_lengthFieldRect, _sequenceLength.intValue, _numberFieldStyle), 1, int.MaxValue);

            GUIStyle s = new GUIStyle(EditorStyles.miniButton);
            s.padding = new RectOffset(1, 1, 1, 1);

            if (FGUI.Button(_addContainerRect, _addContainerLabel))
            {
                AddContainer();
            }

            if (FGUI.Button(_openInspectorRect, _openInspectorLabel))
            {
                FInspectorWindow.Open();
            }
            if (FGUI.Button(_saveRect, _saveLabel))
            {
                Save(sequence);
            }
            if (FGUI.Button(_saveAllRect, _saveAllLabel))
            {
                SaveAll();
            }
            
            _sequenceSO.ApplyModifiedProperties();

            GUI.enabled = true;
        }

        private void AddContainer()
        {
            _sequenceWindow.GetSequenceEditor().CreateContainer();
        }

        private void Save(FSequence sequence)
        {
            var savePath = FSettings.SequenceSavePath;
            var path = savePath + "/" + sequence.gameObject.name + ".prefab";


            PrefabUtility.SaveAsPrefabAsset(sequence.gameObject, path);
        }

        private void SaveAll()
        {
            foreach (var sequence in _sequences)
            {
                Save(sequence);
            }
        }

        private void LoadSequence()
        {
            var path = EditorUtility.OpenFilePanel("SelectSkills", FSettings.SequenceSavePath, "prefab");
            if (!string.IsNullOrEmpty(path))
            {
                var assetPath = FUtility.GetAssetPathFromFullPath(path);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                var go = GameObject.Instantiate(prefab);
                go.name = FUtility.RemoveCloneStr(go.name);
                var sequence = go.GetComponent<FSequence>();
                _sequenceWindow.GetSequenceEditor().OpenSequence(sequence);
            }
        }
    }
}
