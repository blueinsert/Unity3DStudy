using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace VertPaint
{
    /// <summary>
    /// VertPaint is a small vertex color painting utility that allows you
    /// to paint vertex colors onto your meshes. In combination with a good
    /// set of vertex color blending shaders you can achieve fantastic looking
    /// results like height-based blending of various materials, etc...      
    /// <para> </para>
    /// The complete source code is provided and is open for you to read, 
    /// understand, modify, extend or whatever other use you have for it.
    /// Feel free to integrate it and adapt it to your projects as you wish.
    /// You can for instance derive from this class, override stuff, subscribe 
    /// methods to its public events and react to stuff happening in the <see cref="VertPaintWindow"/>.
    /// It's all there, easily accessible, clean, modular and well formatted. I hope you enjoy it!
    /// </summary>
    public partial class VertPaintWindow : EditorWindow, IHasCustomMenu
    {
        #region Paths

        [SerializeField]
        private string autosaveDirectory = DEFAULT_AUTOSAVE_DIRECTORY;

        /// <summary>
        /// The project-local path to the directory where VertPaint will deposit its autosave and list of favorite templates.
        /// </summary>
        public string AutosaveDirectory => autosaveDirectory;

        /// <summary>
        /// The full path to the VertPaint autosave file 
        /// (this depends on the <see cref="AutosaveDirectory"/>).
        /// </summary>
        public string AutosaveFilePath => autosaveDirectory + "VertPaint Autosave.xml";

        /// <summary>
        /// The full path to the file that contains  
        /// the list of favorite VertPaint templates.
        /// </summary>
        public string FavoriteTemplatesFilePath => autosaveDirectory + "VertPaint Favorite Templates.xml";

        #endregion

        #region Foldout states

        [SerializeField]
        private bool showHelp;

        [SerializeField]
        private bool showBrushSettings = true;

        [SerializeField]
        private bool showKeyBindings = true;

        [SerializeField]
        private bool showTemplates = true;

        #endregion

        #region Main toggles

        [SerializeField]
        private bool enabled = DEFAULT_ENABLED;

        /// <summary>
        /// This toggle enables and disables the brush.<para> </para> 
        /// A disabled brush won't appear in the scene view and you'll not be able to paint any vertex colors with it.
        /// </summary>
        public bool Enabled => enabled;

        [SerializeField]
        private bool hideTransformHandle = DEFAULT_HIDE_TRANSFORM_HANDLE;

        /// <summary>
        /// Hide/unhide the transform handle inside the scene view.
        /// </summary>
        public bool HideTransformHandle => hideTransformHandle;

        [SerializeField]
        private bool togglePreview = DEFAULT_TOGGLE_PREVIEW;

        /// <summary>
        /// Should previewing the vertex colors be triggered by holding down the preview key or toggling it on/off?
        /// </summary>
        public bool TogglePreview => togglePreview;

        #endregion

        #region Brush settings

        /// <summary>
        /// The last time the user painted with this <see cref="VertPaintWindow"/> instance.
        /// </summary>
        public DateTime LastPaintTime { get; private set; }

        [SerializeField]
        private float radius = DEFAULT_RADIUS;

        /// <summary>
        /// The radius is defined in meters and affects the size of the paint brush.
        /// </summary>
        public float Radius => radius;

        [SerializeField]
        private float maxRadius = DEFAULT_MAX_RADIUS;

        /// <summary>
        /// The maximum brush radius size (in meters).
        /// </summary>
        public float MaxRadius => maxRadius;

        [SerializeField]
        private float delay = DEFAULT_DELAY;

        /// <summary>
        /// The minimum delay in seconds between paint strokes.
        /// </summary>
        public float Delay => delay;

        [SerializeField]
        private float opacity = DEFAULT_OPACITY;

        /// <summary>
        /// The opacity value controls the maximum intensity of the painted colors.<para> </para> 
        /// Maximum opacity will result in fully opaque (full alpha) colors, 
        /// whereas a value of 50% would halve the intensity of the colors 
        /// (thus making each paint stroke blander).
        /// </summary>
        public float Opacity => opacity;

        [SerializeField]
        private AnimationCurve falloff = DEFAULT_FALLOFF;

        /// <summary>
        /// The falloff curve controls the opacity 
        /// of the painted vertex colors in relation to 
        /// their distance to the brush's center.<para> </para> 
        /// t = 0 represents the center of the brush.<para> </para> 
        /// t = 1 translates to the outer edge of the brush area.<para> </para>
        /// By default, this is a linear falloff that circularly fades out the opacity the further away you go from the brush's center.
        /// </summary>
        public AnimationCurve Falloff => falloff;

        [SerializeField]
        private Color color = Color.red;

        /// <summary>
        /// The vertex color to paint.
        /// </summary>
        public Color Color => color;

        [SerializeField]
        private BrushStyle style = DEFAULT_BRUSH_STYLE;

        /// <summary>
        /// The esthetic appearance of the brush inside the scene view.
        /// </summary>
        public BrushStyle Style => style;

        [SerializeField]
        private bool blinkBrushWhileResizing = DEFAULT_BLINK_BRUSH_WHILE_RESIZING;

        /// <summary>
        /// Whether the brush should blink while resizing it or not.
        /// </summary>
        public bool BlinkBrushWhileResizing => blinkBrushWhileResizing;

        [SerializeField]
        private float alpha = DEFAULT_ALPHA;

        /// <summary>
        /// The alpha of the used brush (0 = fully transparent, 1 = fully opaque). This only affects the appearance of the brush!
        /// </summary>
        public float Alpha => alpha;

        [SerializeField]
        private string meshOutputDirectory = DEFAULT_MESH_OUTPUT_DIRECTORY;

        /// <summary>
        /// The project-local path to the directory where VertPaint should deposit the applied vertex color mesh assets.
        /// </summary>
        public string MeshOutputDirectory => meshOutputDirectory;

        #endregion

        #region Key bindings

        [SerializeField]
        private KeyCode paintKey = DEFAULT_PAINT_KEY;

        /// <summary>
        /// The <see cref="KeyCode"/> for painting vertex colors. Hold shift pressed whilst painting to erase.
        /// </summary>
        public KeyCode PaintKey => paintKey;

        [SerializeField]
        private KeyCode modifyRadiusKey = DEFAULT_MODIFY_RADIUS_KEY;

        /// <summary>
        /// The <see cref="KeyCode"/> for modifying the VertPaint brush radius.
        /// </summary>
        public KeyCode ModifyRadiusKey => modifyRadiusKey;

        [SerializeField]
        private KeyCode previewKey = DEFAULT_PREVIEW_KEY;

        /// <summary>
        /// The <see cref="KeyCode"/> for enabling/disabling the vertex colors preview shader.
        /// </summary>
        public KeyCode PreviewKey => previewKey;

        #endregion

        #region Private

        private Material previewMaterial;
        private Material sphereBrushMaterial;

        [SerializeField]
        private List<string> favoriteTemplates = new List<string>(10);

        private Tool lastTool;

        private Action brushModeAction;

        private bool holdingPaintKey;
        private bool holdingPreviewKey;
        private bool previewingVertexColors;
        private bool tempCollider;

        // ScrollView position vectors 
        // (for the scrollbars in the VertPaint window).
        private Vector2 mainScrollPosition;
        private Vector2 favoritesListScrollPosition;

        // The location where the radius sampling procedure started.
        private Vector2 radiusSamplingMousePos;
        private RaycastHit radiusSamplingRaycastHit;

        // The sphere brush model's transform.
        private Transform sphereBrush;

        // The various cached components 
        // of the currently selected mesh:
        private Transform selectedTransform;
        private MeshFilter selectedMeshFilter;
        private MeshRenderer selectedMeshRenderer;
        private MeshCollider selectedMeshCollider;

        #endregion

        //-------------------------------------------------------------------------------------

        /// <summary>
        /// Opens a VertPaint window.
        /// </summary>
        [MenuItem("Window/VertPaint %#v")]
        public static void Open()
        {
            // Get an existing open window or, if there is none, make a new one.
            var window = GetWindow(typeof(VertPaintWindow), false) as VertPaintWindow;

            if (window != null)
            {
                window.Show();

                // Configure the window's size and title.
                window.titleContent = new GUIContent("VertPaint");
                window.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
                window.maxSize = new Vector2(MAX_WINDOW_WIDTH, MAX_WINDOW_HEIGHT);
            }
        }

        /// <summary>
        /// Adds the VertPaint options to its window/tab context menu.
        /// </summary>
        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Open shader prep utility"), false, ShaderPrepUtilityWindow.Open);
            menu.AddItem(new GUIContent("Restore default window size"), false, RestoreWindowSize);
        }

        /// <summary>
        /// Restores the VertPaint window to the default size and undocked state.
        /// </summary>
        public void RestoreWindowSize()
        {
            var window = GetWindow<VertPaintWindow>();

            if (window == null)
                return;

            Rect pos = window.position;
            pos.width = 387;
            pos.height = 595;
            window.position = pos;
        }

        /// <summary>
        /// Reverts all VertPaint settings to their default values.
        /// </summary>
        public void Reset()
        {
            // Revert foldouts:
            showHelp = false;
            showBrushSettings = showKeyBindings = showTemplates = true;

            // Revert brush settings:
            enabled = DEFAULT_ENABLED;
            togglePreview = DEFAULT_TOGGLE_PREVIEW;
            hideTransformHandle = DEFAULT_HIDE_TRANSFORM_HANDLE;
            radius = DEFAULT_RADIUS;
            maxRadius = DEFAULT_MAX_RADIUS;
            delay = DEFAULT_DELAY;
            opacity = DEFAULT_OPACITY;
            falloff = DEFAULT_FALLOFF;
            color = DEFAULT_COLOR;
            style = DEFAULT_BRUSH_STYLE;
            blinkBrushWhileResizing = DEFAULT_BLINK_BRUSH_WHILE_RESIZING;
            alpha = DEFAULT_ALPHA;
            meshOutputDirectory = DEFAULT_MESH_OUTPUT_DIRECTORY;

            // Revert key bindings:
            paintKey = DEFAULT_PAINT_KEY;
            modifyRadiusKey = DEFAULT_MODIFY_RADIUS_KEY;
            previewKey = DEFAULT_PREVIEW_KEY;

            // Revert template settings:
            autosaveDirectory = DEFAULT_AUTOSAVE_DIRECTORY;

            Repaint();
        }

        private string GetPreviewShaderName()
        {
            if (GraphicsSettings.currentRenderPipeline)
            {
                return "VertPaint_HDRP_VertexColorPreview";
            }

            return "VertPaint_Legacy_VertexColorPreview";
        }

        private void OnEnable()
        {
            lastTool = Tools.current;
            LastPaintTime = DateTime.Now;

            GatherMeshComponents();

            #region Inspector initialization

            CollapseMeshRendererAndCollider();

            #endregion

            InitInspectorGUI();

            // Preview material initialization:

            if (previewMaterial == null)
            {
                previewMaterial = Resources.Load<Material>(GetPreviewShaderName());
            }

            #region Sphere brush initialization

            if (sphereBrushMaterial == null)
            {
                sphereBrushMaterial = new Material(Shader.Find("Standard"))
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                sphereBrushMaterial.SetFloat(METALLIC_PROP_ID, 0.0f);
                sphereBrushMaterial.SetFloat(GLOSSINESS_PROP_ID, 0.0f);
                sphereBrushMaterial.SetInt(SRC_BLEND_PROP_ID, (int)UnityEngine.Rendering.BlendMode.One);
                sphereBrushMaterial.SetInt(DST_BLEND_PROP_ID, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                sphereBrushMaterial.SetInt(ZWRITE_PROP_ID, 0);
                sphereBrushMaterial.DisableKeyword("_ALPHATEST_ON");
                sphereBrushMaterial.DisableKeyword("_ALPHABLEND_ON");
                sphereBrushMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                sphereBrushMaterial.renderQueue = 3000;
            }

            if (sphereBrush == null)
            {
                sphereBrush = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                sphereBrush.gameObject.hideFlags = HideFlags.HideAndDontSave;

                var collider = sphereBrush.GetComponent<Collider>();
                if (collider != null)
                {
                    DestroyImmediate(collider);
                }

                var renderer = sphereBrush.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterials = new Material[] { sphereBrushMaterial };
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                sphereBrush.gameObject.SetActive(false);
            }

            #endregion

            #region Brush action initialization

            // Default brush mode is paint.
            if (brushModeAction == null)
            {
                brushModeAction = BrushMode_Paint;
            }

            #endregion

            #region I/O initialization

            // Load the autosave directory path from the EditorPrefs.
            autosaveDirectory = EditorPrefs.GetString("vp_autosave_dir", DEFAULT_AUTOSAVE_DIRECTORY);

            // Load up the last autosave (or revert settings to their
            // default values if there's no autosave file available).
            if (!Load(AutosaveFilePath))
            {
                Reset();
            }

            LoadFavoriteTemplates();

            #endregion

            #region Event and delegate subscriptions

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            Selection.selectionChanged += OnSelectionChanged;
            PreviewStateChanged += VertPaintWindow_PreviewStateChanged;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            #endregion
        }

        private void OnDisable()
        {
            // Set the brush mode action to null in order to 
            // avoid its accidental invocation from inside 
            // OnSceneGUI after the EditorWindow's closure.
            brushModeAction = null;

            // Remove the sphere brush object in the scene
            // as it's not needed when VertPaint isn't open.
            if (sphereBrush != null)
            {
                DestroyImmediate(sphereBrush.gameObject);
            }

            // Destroy any assets created in OnEnable to clean up.
            if (sphereBrushMaterial != null)
            {
                DestroyImmediate(sphereBrushMaterial);
            }

            Resources.UnloadAsset(previewMaterial);

            // Avoid leaving the vertex colors preview 
            // shader on the mesh when leaving VertPaint.
            if (previewingVertexColors)
            {
                OnPreviewStateChanged(new PreviewStateChangedEventArgs(false));
            }

            // If the MeshCollider was temporary, destroy it.
            // VertPaint shouldn't leave any noticeable traces!
            if (tempCollider && selectedMeshCollider != null)
            {
                DestroyImmediate(selectedMeshCollider);
            }

            // Autosave on close.
            Save(AutosaveFilePath);
            SaveFavoriteTemplates();

            // Set the tool back to what it was before opening VertPaint.
            Tools.current = lastTool;

            // Save the autosave directory path to the editor prefs 
            // (otherwise we lose that information between VertPaint sessions).
            EditorPrefs.SetString("vp_autosave_dir", autosaveDirectory);

            // Discard the painted vertex colors 
            // (if you want to keep them, apply them!).
            DiscardPaintedVertexColors();

            // Unsubscribe events (clean up before leaving).
            PreviewStateChanged -= VertPaintWindow_PreviewStateChanged;
            SceneView.duringSceneGui -= OnSceneGUI;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Selection.selectionChanged -= OnSelectionChanged;

            // Clean up.
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        private void OnFocus()
        {
            LoadFavoriteTemplates();
        }

        private void OnLostFocus()
        {
            // Autosave on defocus.
            Save(AutosaveFilePath);
            SaveFavoriteTemplates();
        }

        private void OnUndoRedoPerformed()
        {
            if (selectedTransform != null)
            {
                Selection.activeTransform = selectedTransform;
            }

            Mesh additionalVertexStreams = selectedMeshRenderer.additionalVertexStreams;

            if (selectedMeshRenderer != null && additionalVertexStreams != null)
            {
                additionalVertexStreams.colors = selectedMeshRenderer.additionalVertexStreams.colors;
            }

            Focus();
            Repaint();
            SceneView.RepaintAll();
        }

        private void OnSelectionChanged()
        {
            // Reload templates list in case the user renamed one or more of them with the VertPaint window open.
            LoadFavoriteTemplates();

            // Remove the vertex color preview shader from the deselected mesh.
            if (selectedMeshRenderer != null && selectedMeshRenderer.sharedMaterials.Contains(previewMaterial))
            {
                IList<Material> matList = selectedMeshRenderer.sharedMaterials.ToList();
                matList.Remove(previewMaterial);
                selectedMeshRenderer.sharedMaterials = matList.ToArray();
            }

            if (tempCollider && selectedMeshCollider != null)
            {
                DestroyImmediate(selectedMeshCollider);
                selectedMeshCollider = null;
                tempCollider = false;
            }

            DiscardPaintedVertexColors();

            GatherMeshComponents();

            // Add the vertex color preview shader to the freshly selected object (if needed).
            if (previewingVertexColors && selectedMeshRenderer != null && !selectedMeshRenderer.sharedMaterials.Contains(previewMaterial))
            {
                IList<Material> matList = selectedMeshRenderer.sharedMaterials.ToList();

                if (!matList.Contains(previewMaterial))
                {
                    matList.Add(previewMaterial);
                }

                selectedMeshRenderer.sharedMaterials = matList.ToArray();
            }

            // Collapse the MeshCollider/MeshRenderer component 
            // to hide the green wireframe overlay on the mesh.
            CollapseMeshRendererAndCollider();

            Repaint();

            if (NoMeshSelected())
            {
                enabled = false;
            }
            else
            {
                if (!enabled && Input.GetKey(KeyCode.LeftControl)) // TODO: FIX THIS! Should enable if ctrl+click a mesh..
                {
                    enabled = true;
                }
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (EditorApplication.isPlaying || !enabled)
                return;

            // Invoke the current brush mode.
            brushModeAction?.Invoke();

            // Hide or show the transform handle according to the setting.
            Tools.current = hideTransformHandle ? Tool.None : lastTool;

            // Keep selected no more than one object at a time.
            if (Selection.transforms.Length > 1)
            {
                Transform t = Selection.activeTransform;
                Selection.activeTransform = null;
                Selection.activeTransform = t;
            }
        }

        /// <summary>
        /// Checks if no meshes are currently selected.
        /// </summary>
        /// <returns>Whether no mesh is currently selected in the scene.</returns>
        private bool NoMeshSelected()
        {
            return selectedTransform == null || selectedMeshFilter == null || selectedMeshRenderer == null || selectedMeshCollider == null;
        }

        /// <summary>
        /// Collapses the inspected <see cref="MeshRenderer"/> and <see cref="MeshCollider"/> components.
        /// </summary>
        private void CollapseMeshRendererAndCollider()
        {
            if (selectedMeshCollider == null || selectedMeshRenderer == null)
                return;

            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshCollider, true);
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshRenderer, true);

            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshCollider, false);
            UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(selectedMeshRenderer, false);
        }

        /// <summary>
        /// Assigns the selected mesh to the <see cref="selectedTransform"/> variable 
        /// and caches its relevant components, such as <see cref="selectedMeshFilter"/>, 
        /// <see cref="selectedMeshRenderer"/>, etc...
        /// </summary>
        private void GatherMeshComponents()
        {
            selectedTransform = Selection.activeTransform;

            if (selectedTransform == null)
                return;

            selectedMeshFilter = selectedTransform.GetComponent<MeshFilter>();
            selectedMeshRenderer = selectedTransform.GetComponent<MeshRenderer>();
            selectedMeshCollider = selectedTransform.GetComponent<MeshCollider>();

            if (selectedMeshCollider == null && selectedMeshFilter != null)
            {
                tempCollider = true;
                selectedMeshCollider = selectedMeshFilter.gameObject.AddComponent<MeshCollider>();
            }
            else tempCollider = false;
        }

        #region Brush

        private void DrawBrush(Vector3 position, Vector3 normal, Color color, BrushStyle style)
        {
            Handles.color = color;

            if (sphereBrush != null)
            {
                sphereBrush.gameObject.SetActive(style == BrushStyle.Sphere);
            }

            switch (style)
            {
                case BrushStyle.Circle:
                {
                    Handles.DrawWireDisc(position, normal, radius);
                    HandleUtility.Repaint();
                    break;
                }
                case BrushStyle.Disc:
                {
                    Handles.DrawSolidDisc(position, normal, radius);
                    HandleUtility.Repaint();
                    break;
                }
                case BrushStyle.Sphere:
                {
                    sphereBrush.position = position;
                    sphereBrush.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
                    sphereBrushMaterial.color = color;
                    sphereBrushMaterial.SetColor(EMISSION_COLOR_PROP_ID, color);

                    if (SceneView.lastActiveSceneView != null)
                        SceneView.lastActiveSceneView.Repaint();

                    break;
                }
            }
        }

        private void BrushMode_Paint()
        {
            RaycastHit hit = default(RaycastHit);

            if (selectedMeshCollider != null && selectedMeshCollider.Raycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), out hit, 15000.0f))
            {
                if (style != BrushStyle.None)
                {
                    DrawBrush(hit.point, hit.normal, new Color(color.r, color.g, color.b, style == BrushStyle.Circle ? 1.0f : alpha), style);
                }

                if (Event.current.isKey)
                {
                    if (Event.current.keyCode == paintKey)
                    {
                        PaintVertexColors(hit);
                        Event.current.Use();
                    }
                }
                else if (Event.current.isMouse)
                {
                    if ((Event.current.button == 0 && paintKey == KeyCode.Mouse0) ||
                        (Event.current.button == 1 && paintKey == KeyCode.Mouse1) ||
                        (Event.current.button == 2 && paintKey == KeyCode.Mouse2))
                    {
                        if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                        {
                            EventModifiers modifiers = Event.current.modifiers;

                            if (modifiers != EventModifiers.Control && modifiers != EventModifiers.Command && modifiers != EventModifiers.Alt)
                            {
                                PaintVertexColors(hit);
                                Event.current.Use();
                            }
                        }
                    }
                }
            }

            if (Event.current.isKey)
            {
                // Handle the vertex color previewing based on the
                // specified input key and the user settings (hold or toggle).
                if (Event.current.keyCode == previewKey)
                {
                    if (!holdingPreviewKey && !togglePreview && Event.current.type == EventType.KeyDown)
                    {
                        holdingPreviewKey = true;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(true));
                    }

                    if (Event.current.type == EventType.KeyUp)
                    {
                        holdingPreviewKey = false;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(!previewingVertexColors));
                    }

                    Event.current.Use();
                }

                // Switch to radius sampling mode once
                // the "modify radius" key has been pressed.
                if (Event.current.keyCode == modifyRadiusKey)
                {
                    if (Event.current.type == EventType.KeyDown)
                    {
                        radiusSamplingMousePos = Event.current.mousePosition;
                        radiusSamplingRaycastHit = hit;
                        brushModeAction = BrushMode_ModifyRadius;
                    }

                    Event.current.Use();
                }
            }
            else if (Event.current.isMouse)
            {
                if ((Event.current.button == 0 && previewKey == KeyCode.Mouse0) ||
                    (Event.current.button == 1 && previewKey == KeyCode.Mouse1) ||
                    (Event.current.button == 2 && previewKey == KeyCode.Mouse2))
                {
                    if (!holdingPreviewKey && !togglePreview && Event.current.type == EventType.MouseDown)
                    {
                        holdingPreviewKey = true;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(true));
                    }

                    if (Event.current.type == EventType.MouseUp)
                    {
                        holdingPreviewKey = false;
                        OnPreviewStateChanged(new PreviewStateChangedEventArgs(!previewingVertexColors));
                    }

                    Event.current.Use();
                }

                if ((Event.current.button == 0 && modifyRadiusKey == KeyCode.Mouse0) ||
                    (Event.current.button == 1 && modifyRadiusKey == KeyCode.Mouse1) ||
                    (Event.current.button == 2 && modifyRadiusKey == KeyCode.Mouse2))
                {
                    if (Event.current.type == EventType.MouseDown)
                    {
                        radiusSamplingRaycastHit = hit;
                        radiusSamplingMousePos = Event.current.mousePosition;
                        brushModeAction = BrushMode_ModifyRadius;
                    }

                    Event.current.Use();
                }
            }

            // Unless we're trying to change our selection, 
            // go for a passive focus type to avoid accidental deselection whilst painting.
            if (Event.current.modifiers != EventModifiers.Control && Event.current.modifiers != EventModifiers.Command)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
        }

        private void BrushMode_ModifyRadius()
        {
            // Adjust the radius by dragging the mouse away from
            // or towards the location where we started sampling.
            radius = Vector2.Distance(radiusSamplingMousePos, Event.current.mousePosition) * .025f;

            // Make the brush flicker whilst sampling the new radius.
            float brushAlpha = Mathf.Clamp(Mathf.Sin(Time.realtimeSinceStartup * 15.0f), 0.0f, style == BrushStyle.Circle ? 1.0f : alpha);

            DrawBrush
            (
                position: radiusSamplingRaycastHit.point,
                normal: radiusSamplingRaycastHit.normal,
                color: new Color
                (
                    r: color.r,
                    g: color.g,
                    b: color.b,
                    a: blinkBrushWhileResizing ? brushAlpha : alpha
                ),
                style: style
            );

            // Repaint the VertPaint window 
            // to keep the slider synchronized.
            Repaint();

            Event currentEvent = Event.current;

            // Exit the sampling mode if the user lets go of the key (or mouse button).
            if (currentEvent.isKey && currentEvent.keyCode == modifyRadiusKey)
            {
                if (currentEvent.type == EventType.KeyUp)
                {
                    brushModeAction = BrushMode_Paint;
                }

                currentEvent.Use();
            }
            else if (Event.current.isMouse)
            {
                int currentButton = currentEvent.button;

                if ((currentButton == 0 && modifyRadiusKey == KeyCode.Mouse0) ||
                    (currentButton == 1 && modifyRadiusKey == KeyCode.Mouse1) ||
                    (currentButton == 2 && modifyRadiusKey == KeyCode.Mouse2))
                {
                    if (currentEvent.type == EventType.MouseUp)
                    {
                        brushModeAction = BrushMode_Paint;
                    }

                    currentEvent.Use();
                }
            }
        }

        #endregion

        #region Vertex color operations

        /// <summary>
        /// Checks if the <see cref="selectedMeshRenderer"/>'s additionalVertexStreams property is null.
        /// If it is, the instantiated <see cref="selectedMeshFilter"/>'s sharedMesh is assigned to it.<para> </para>
        /// Once the additionalVertexStreams' existance can be guaranteed, its colors property is checked.
        /// If there's no vertex color data stored in it, a new <see cref="Color"/>[] array is created and assigned to it.
        /// </summary>
        private void InitializeAdditionalVertexStreams()
        {
            if (selectedMeshRenderer == null || selectedMeshFilter == null)
            {
                return;
            }

            if (selectedMeshRenderer.additionalVertexStreams == null)
            {
                selectedMeshRenderer.additionalVertexStreams = Instantiate(selectedMeshFilter.sharedMesh);
            }

            Color[] colors = selectedMeshRenderer.additionalVertexStreams.colors;

            if (colors == null || colors.Length == 0)
            {
                colors = new Color[selectedMeshFilter.sharedMesh.vertices.Length];
            }

            selectedMeshRenderer.additionalVertexStreams.colors = colors;
        }

        /// <summary>
        /// Paints the specified vertex color to the vertices inside the brush's area.
        /// </summary>
        /// <param name="brushHit">The brush's RaycastHit where the vertex color should be painted.</param>
        private void PaintVertexColors(RaycastHit brushHit)
        {
            if (selectedMeshRenderer == null)
            {
                Debug.LogError("VertPaint: The specified MeshRenderer is null; couldn't paint vertex colors.");
                return;
            }

            // Respect the specified delay between paint strokes value.
            if (LastPaintTime.AddSeconds(delay) > DateTime.Now)
            {
                return;
            }

            InitializeAdditionalVertexStreams();

            // Add the paint stroke to the undo stack.
            Undo.RecordObject(selectedMeshRenderer.additionalVertexStreams, "paint vertex colors");

            Mesh additionalVertexStreams = selectedMeshRenderer.additionalVertexStreams;
            Vector3[] verts = additionalVertexStreams.vertices;
            Color[] colors = additionalVertexStreams.colors;

            Vector3 brushHitPoint = brushHit.point;
            Transform brushHitTransform = brushHit.transform;

            for (int i = colors.Length - 1; i >= 0; --i)
            {
                Vector3 currentVertex = verts[i];

                // Calculate the distance between the center of the brush and the current vertex.
                // If it's greater than the brush radius, it means that the vert is outside of the brush area and can safely be ignored.
                float distance = Vector3.Distance(brushHitPoint, brushHitTransform.TransformPoint(currentVertex));

                if (distance <= radius)
                {
                    float t = opacity * color.a * falloff.Evaluate(distance / radius);
                    colors[i] = Color.Lerp(colors[i], Event.current.modifiers == EventModifiers.Shift ? Color.clear : color, t);
                }
            }

            // Apply the vertex colors based on falloff and opacity.
            additionalVertexStreams.colors = colors;
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());

            // Erasing is basically like painting black color, so check against that
            // and raise the correct event based on the painted vertex color.
            if (color.r + color.g + color.b < float.Epsilon)
            {
                OnErased(new PaintStrokeEventArgs(brushHit, DateTime.Now));
            }
            else
            {
                OnPainted(new PaintStrokeEventArgs(brushHit, DateTime.Now));
            }

            LastPaintTime = DateTime.Now;
        }

        /// <summary>
        /// Discards the painted vertex colors.
        /// </summary>
        private void DiscardPaintedVertexColors()
        {
            if (selectedMeshRenderer == null)
            {
                return;
            }

            selectedMeshRenderer.additionalVertexStreams = null;

            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }

        /// <summary>
        /// Applies the painted vertex colors to the selected mesh by 
        /// writing them to an asset on disk (located inside the specified <see cref="meshOutputDirectory"/>).
        /// </summary>
        private void ApplyPaintedVertexColors()
        {
            if (selectedMeshFilter != null && selectedMeshRenderer != null && selectedMeshRenderer.additionalVertexStreams != null)
            {
                var mesh = Instantiate(selectedMeshRenderer.additionalVertexStreams);
                mesh.name = selectedMeshRenderer.name;

                StringBuilder path = new StringBuilder(meshOutputDirectory).Append(SceneManager.GetActiveScene().name).Append('/').Append(selectedMeshRenderer.name).Append(".asset");
                string dir = Path.GetDirectoryName(path.ToString());

                if (!Directory.Exists(dir) && !string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }

                AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path.ToString()));

                selectedMeshFilter.sharedMesh = mesh;
                EditorGUIUtility.PingObject(mesh);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        #region Default event subscribers

        private void VertPaintWindow_PreviewStateChanged(object sender, PreviewStateChangedEventArgs e)
        {
            previewingVertexColors = e.Previewing;

            if (previewingVertexColors)
            {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            if (selectedMeshRenderer == null)
                return;

            IList<Material> matList = selectedMeshRenderer.sharedMaterials.ToList();

            if (previewingVertexColors && !matList.Contains(previewMaterial))
            {
                matList.Add(previewMaterial);
            }
            else if (matList.Contains(previewMaterial))
            {
                matList.Remove(previewMaterial);
            }

            selectedMeshRenderer.sharedMaterials = matList.ToArray();
        }

        #endregion

        #region I/O

        /// <summary>
        /// Checks whether the specified object's file extension is ".xml" or not.
        /// </summary>
        /// <returns>True if the <paramref name="obj"/> is an xml file, false if otherwise.</returns>
        /// <param name="obj">The object to check.</param>
        private bool IsXml(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return string.CompareOrdinal(Path.GetExtension(path).ToLowerInvariant(), ".xml") == 0;
        }

        /// <summary>
        /// Cleans the mesh output directory from unused mesh assets.<para> </para>
        /// Meshes that are never referenced in any <see cref="MeshFilter"/> in the 
        /// scene are considered unused and are thus moved into a folder named "_old" 
        /// (since deleting them would be too intrusive).
        /// </summary>
        private void CleanMeshOutputDirectory()
        {
            // Build the final output directory path based on the specified
            // mesh output directory and the currently active scene's name.
            // The result is a sub-folder of the mesh output directory named after the currently active scene.
            StringBuilder dir = new StringBuilder(meshOutputDirectory).Append(SceneManager.GetActiveScene().name).Append('/');

            // If the output directory doesn't even exist 
            // it means that there aren't any meshes whatsoever.
            if (!Directory.Exists(dir.ToString()))
            {
                return;
            }

            // Get all mesh asset paths from the output directory.
            string[] meshPaths = Directory.GetFiles(dir.ToString(), "*.asset");
            if (meshPaths.Length == 0)
            {
                return;
            }

            // Create a new dictionary to store the meshes 
            // from the output folder along with their paths.
            Dictionary<Mesh, string> meshes = new Dictionary<Mesh, string>(meshPaths.Length);

            // Populate the dictionary with valid KeyValuePairs.
            for (int i = meshPaths.Length - 1; i >= 0; i--)
            {
                var mesh = AssetDatabase.LoadAssetAtPath(meshPaths[i], typeof(Mesh)) as Mesh;
                if (mesh != null)
                {
                    meshes.Add(mesh, meshPaths[i]);
                }
            }

            // Gather all unused mesh paths.
            // It's: all mesh paths minus the ones currently
            // in use, thanks to LINQ's Enumerable.Except method.
            IEnumerable<string> unusedMeshPaths = meshPaths.Except(FindObjectsOfType<MeshFilter>()
                    .Where(meshFilter => meshes.ContainsKey(meshFilter.sharedMesh))
                    .Select(meshFilter => meshes[meshFilter.sharedMesh])
                    .Distinct())
                .ToList();

            // Safety-check the target "_old" folder 
            // and move the unused mesh assets into it.
            if (unusedMeshPaths.Any())
            {
                string destinationDirectory = dir.Append("_old/").ToString();

                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                }

                foreach (string path in unusedMeshPaths)
                {
                    AssetDatabase.MoveAsset(path, AssetDatabase.GenerateUniqueAssetPath(destinationDirectory + Path.GetFileName(path)));
                }
            }

            AssetDatabase.Refresh();
        }

        #endregion
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com