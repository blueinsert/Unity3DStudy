using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VertPaint
{
    public partial class VertPaintWindow
    {
        // GUIContent icons:
        private GUIContent addFavButtonGUIContent = null;
        private GUIContent templateSlotGUIContent = null;
        private GUIContent saveGUIContent = null;

        // GUIContent labels and tooltips for the VertPaint window:
        private readonly GUIContent enabledGUIContent = new GUIContent("Enabled", "This toggle enables and disables the brush. A disabled brush won't appear in the scene view and you'll not be able to paint any vertex colors with it.");
        private readonly GUIContent togglePreviewGUIContent = new GUIContent("Toggle preview", "Should previewing the vertex colors be triggered by holding down the preview key or toggling on/off?");
        private readonly GUIContent hideTransformHandleGUIContent = new GUIContent("Hide transform handle", "Hide/unhide the transform handle in the scene view.");
        private readonly GUIContent helpGUIContent = new GUIContent("Help", "Click here to learn how to use VertPaint.");
        private readonly GUIContent radiusGUIContent = new GUIContent("Radius", "The radius is defined in meters and affects the size of the paint brush.");
        private readonly GUIContent delayGUIContent = new GUIContent("Delay", "The minimum delay in seconds between paint strokes.");
        private readonly GUIContent opacityGUIContent = new GUIContent("Opacity", "The opacity value controls the maximum intensity of the painted colors. Maximum opacity will result in fully opaque (full alpha) colors, whereas a value of 50% would halve the intensity of the colors (thus making them blander).");
        private readonly GUIContent falloffGUIContent = new GUIContent("Falloff", "The falloff determines how the painted vertex color's opacity behaves in relation to the distance to the brush center. The painted color's alpha is multiplied by the evaluated falloff value. On the curve, t=0 represents the center of the brush and t=1 its outermost edge.");
        private readonly GUIContent colorGUIContent = new GUIContent("Color", "The vertex color to paint with the brush.");
        private readonly GUIContent styleGUIContent = new GUIContent("Style", "The style defines the esthetic appearance of the brush.");
        private readonly GUIContent blinkBrushWhileResizingGUIContent = new GUIContent("Blink while resizing brush", "Should the brush gizmo blink while you're resizing it or not?");
        private readonly GUIContent alphaGUIContent = new GUIContent("Alpha", "This is the transparency of the brush object and does not affect the final vertex colors. It's only there to help you see better whilst painting.");
        private readonly GUIContent meshOutputDirectoryGUIContent = new GUIContent("Mesh output directory: ", "This is the directory where VertPaint will store the vertex colored mesh assets.");
        private readonly GUIContent paintKeyGUIContent = new GUIContent("Paint", "This is the key used for painting.");
        private readonly GUIContent modifyRadiusKeyGUIContent = new GUIContent("Modify radius", "Keep this key pressed and drag the mouse away from and to the center of the brush to adjust its radius.");
        private readonly GUIContent previewKeyGUIContent = new GUIContent("Preview vertex colors", "Press this key (or hold it, depending on the setting in the top right corner of the window) to preview the painted vertex colors on the mesh.");
        private readonly GUIContent removeFavButtonGUIContent = new GUIContent("-", "Remove the last (bottom-most) entry from the list.");
        private readonly GUIContent clearFavsButtonGUIContent = new GUIContent("C", "Clear the list of favorite templates.");
        private readonly GUIContent fillGUIContent = new GUIContent("Fill", "Fill all vertex colors with the current brush color. Hold down shift to fill with clear black (0,0,0,0).");
        private readonly GUIContent invertGUIContent = new GUIContent("Invert", "Invert all vertex colors.");
        private readonly GUIContent discardGUIContent = new GUIContent("Discard", "Discard the changes made to the vertex colors.");
        private readonly GUIContent applyGUIContent = new GUIContent("Apply", "Apply the painted vertex colors by saving them out to an asset on disk (inside the specified mesh output directory).");
        private readonly GUIContent resetGUIContent = new GUIContent("Reset", "Revert all VertPaint settings back to their default values, except for the favorite templates list (which you can clear with the C button if you want).");
        private readonly GUIContent cleanMeshOutputDirectoryGUIContent = new GUIContent("Clean up", "Clean up the mesh output directory by moving unreferenced and unneeded mesh assets in it into a sub-folder called \"_old\".");

        private void OnGUI()
        {
            Undo.RecordObject(this, "change VertPaint settings");

            EditorGUIUtility.labelWidth = 70.0f;
            GUILayout.Space(6.0f);

            EditorGUILayout.BeginVertical(GUILayout.MaxHeight(588.0f));
            {
                mainScrollPosition = GUILayout.BeginScrollView(mainScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
                {
                    #region Main settings

                    EditorGUILayout.BeginHorizontal();
                    {
                        bool prev = GUI.enabled;
                        GUI.enabled = selectedTransform != null && selectedMeshCollider != null;
                        GUI.color = enabled ? Color.green : Color.red;
                        EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false), GUILayout.Width(50.0f));
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                enabled = EditorGUILayout.Toggle(GUIContent.none, enabled, GUILayout.Width(15.0f));
                                EditorGUILayout.LabelField(enabledGUIContent, GUILayout.Width(50.0f), GUILayout.ExpandWidth(false));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                        GUI.color = Color.white;
                        GUI.enabled = prev;

                        EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false), GUILayout.Width(55.0f));
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                togglePreview = EditorGUILayout.Toggle(GUIContent.none, togglePreview, GUILayout.Width(15.0f));
                                EditorGUILayout.LabelField(togglePreviewGUIContent, GUILayout.Width(95.0f), GUILayout.ExpandWidth(false));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false), GUILayout.Width(55.0f));
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUI.BeginChangeCheck();
                                hideTransformHandle = EditorGUILayout.Toggle(GUIContent.none, hideTransformHandle, GUILayout.Width(15.0f));
                                EditorGUILayout.LabelField(hideTransformHandleGUIContent, GUILayout.Width(132.0f), GUILayout.ExpandWidth(false));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    SceneView.RepaintAll();
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                    #endregion

                    if (EditorApplication.isPlaying)
                    {
                        EditorGUILayout.HelpBox("Warning:\n\nCan't paint vertex colors when in play mode.\n\nExit play mode first and then hold down control (command on Mac) and left-mouse click on the desired mesh to start painting vertex colors.\n\nCheck out the help section or Documentation.pdf for more information.", MessageType.Warning);
                    }
                    else if (NoMeshSelected())
                    {
                        EditorGUILayout.HelpBox("Warning:\n\nNo mesh selected.\n\nPlease select a mesh and tick the \"Enabled\" checkbox above to start painting vertex colors.\n\nWhile painting, the object is locked and you can't deselect it: hold down control (command on Mac) and left-mouse click on the desired mesh to select (or deselect).\n\nCheck out the help section or Documentation.pdf for more information.", MessageType.Warning);
                    }
                    else
                    {
                        GUILayout.Space(1.0f);

                        #region Help section

                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button(helpGUIContent, VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showHelp = !showHelp;
                            }

                            GUI.color = Color.white;

                            if (showHelp)
                            {
                                EditorGUILayout.HelpBox(HELP_TEXT, MessageType.None);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Brush settings

                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button("Brush Settings", VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showBrushSettings = !showBrushSettings;
                            }

                            GUI.color = Color.white;

                            if (showBrushSettings)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    radius = EditorGUILayout.Slider(radiusGUIContent, radius, 0.01f, maxRadius);
                                    maxRadius = EditorGUILayout.FloatField(GUIContent.none, maxRadius, GUILayout.Width(30.0f));
                                }
                                EditorGUILayout.EndHorizontal();

                                delay = EditorGUILayout.Slider(delayGUIContent, delay, 0.001f, 1.0f);
                                opacity = EditorGUILayout.Slider(opacityGUIContent, opacity, 0.0f, 1.0f);

                                EditorGUILayout.BeginHorizontal();
                                {
                                    falloff = EditorGUILayout.CurveField(falloffGUIContent, falloff, Color.green, new Rect(0.0f, 0.0f, 1.0f, 1.0f));

                                    if (GUILayout.Button("→", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        falloff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
                                    }

                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("↘", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        falloff = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
                                    }

                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("↗", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        falloff = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
                                    }

                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("rnd", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        Keyframe[] keyframes = new Keyframe[Random.Range(4, 16)];
                                        for (int i = keyframes.Length - 1; i >= 0; i--)
                                        {
                                            float t = i == 1 ? 1.0f : i == 0 ? 0.0f : Random.value;
                                            keyframes[i] = new Keyframe(t, Random.value, Random.value, Random.value);
                                        }

                                        falloff = new AnimationCurve(keyframes);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    color = EditorGUILayout.ColorField(
                                        label: colorGUIContent,
                                        value: color,
                                        showEyedropper: true,
                                        showAlpha: true,
                                        hdr: false,
                                        options: null
                                    );

                                    GUI.color = Color.red;
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.red;
                                    }

                                    GUI.color = Color.green;
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.green;
                                    }

                                    GUI.color = Color.blue;
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.blue;
                                    }

                                    GUI.color = Color.white;
                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button(string.Empty, GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        color = Color.white;
                                    }

                                    GUILayout.Space(-3.0f);
                                    if (GUILayout.Button("1-x", GUILayout.Width(30.0f), GUILayout.Height(15.0f)))
                                    {
                                        var c = color;
                                        color = new Color(1.0f - c.r, 1.0f - c.g, 1.0f - c.b);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Space(1.25f);

                                EditorGUILayout.BeginHorizontal();
                                {
                                    float fw = EditorGUIUtility.fieldWidth;
                                    EditorGUIUtility.fieldWidth = 65.0f;
                                    style = (BrushStyle)EditorGUILayout.EnumPopup(styleGUIContent, style);
                                    EditorGUIUtility.fieldWidth = fw;

                                    if (style != BrushStyle.None && style != BrushStyle.Circle)
                                    {
                                        GUILayout.Space(3.0f);
                                        EditorGUIUtility.labelWidth = 50.0f;
                                        alpha = EditorGUILayout.Slider(alphaGUIContent, alpha, 0.01f, 1.0f);
                                    }

                                    EditorGUIUtility.labelWidth = 70.0f;
                                }
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Space(1.25f);

                                EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(false), GUILayout.Width(110.0f));
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        blinkBrushWhileResizing = EditorGUILayout.Toggle(GUIContent.none, blinkBrushWhileResizing, GUILayout.Width(15.0f));
                                        EditorGUILayout.LabelField(blinkBrushWhileResizingGUIContent, GUILayout.Width(150.0f), GUILayout.ExpandWidth(false));
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUILayout.EndVertical();

                                GUILayout.Space(1.0f);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Key bindings

                        float lw = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 150.0f;
                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button("Key Bindings", VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showKeyBindings = !showKeyBindings;
                            }

                            GUI.color = Color.white;

                            if (showKeyBindings)
                            {
                                paintKey = (KeyCode)EditorGUILayout.EnumPopup(paintKeyGUIContent, paintKey);
                                modifyRadiusKey = (KeyCode)EditorGUILayout.EnumPopup(modifyRadiusKeyGUIContent, modifyRadiusKey);
                                previewKey = (KeyCode)EditorGUILayout.EnumPopup(previewKeyGUIContent, previewKey);
                            }
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUIUtility.labelWidth = lw;

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Templates

                        EditorGUILayout.BeginVertical("Box");
                        {
                            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                            if (GUILayout.Button("Templates", VertPaintGUI.BoldLabelButtonStyle))
                            {
                                showTemplates = !showTemplates;
                            }

                            GUI.color = Color.white;

                            if (showTemplates)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.BeginVertical(GUILayout.Width(80.0f));
                                    {
                                        // Draw button for saving out the current configuration to a template file.
                                        if (GUILayout.Button(saveGUIContent, GUILayout.Width(85.0f), GUILayout.Height(85.0f), GUILayout.ExpandWidth(false)))
                                        {
                                            string path = EditorUtility.SaveFilePanel("Save VertPaint brush template", "Assets/VertPaint/Templates", "<insert_template_name_here>", "xml");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                Save(path);
                                            }
                                        }

                                        // Create drag 'n' drop field rect for loading VertPaint templates.
                                        Rect dropArea = GUILayoutUtility.GetRect(83.0f, 83.0f, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                                        dropArea.x += 5.0f;
                                        dropArea.y += 5.0f;

                                        // Clicking inside the drag 'n' drop area should show
                                        // the file picker dialog (for manual template selection).
                                        Action click = () =>
                                        {
                                            string templatePath = EditorUtility.OpenFilePanel("Select VertPaint template to load", "Assets/VertPaint/Templates", "xml");
                                            if (!string.IsNullOrEmpty(templatePath))
                                            {
                                                Load(templatePath.Substring(Application.dataPath.Length - 6));
                                            }
                                        };

                                        // Draw the drag 'n' drop field 
                                        // and get the dropped template object.
                                        var droppedObj = VertPaintGUI.DragAndDropArea(
                                            dropArea,
                                            leftClick: click,
                                            rightClick: click,
                                            validityCheck: IsXml,
                                            guiContent: templateSlotGUIContent,
                                            dragAndDropVisualMode: DragAndDropVisualMode.Link
                                        );

                                        Repaint();

                                        if (droppedObj != null)
                                        {
                                            Load(AssetDatabase.GetAssetPath(droppedObj));
                                            Repaint();
                                        }
                                    }
                                    EditorGUILayout.EndVertical();

                                    EditorGUILayout.BeginVertical();
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            EditorGUILayout.LabelField("Favorite templates");

                                            // Build the rect for a drag 'n' drop field that
                                            // is used to add VertPaint templates to the favorites list.
                                            Rect dropArea = GUILayoutUtility.GetRect(25.5f, 16.75f, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                                            dropArea.x -= 1.5f;
                                            dropArea.y += 3.5f;

                                            // Clicking instead of dragging & dropping into the plus icon  
                                            // opens the file picker dialog, with which you can manually 
                                            // select the template file to add to the favorites list.
                                            Action click = () =>
                                            {
                                                string templatePath = EditorUtility.OpenFilePanel("Select favorite VertPaint template", "Assets/VertPaint/Templates", "xml");
                                                if (!string.IsNullOrEmpty(templatePath))
                                                {
                                                    favoriteTemplates.Add(templatePath.Substring(Application.dataPath.Length - 6));
                                                    SaveFavoriteTemplates();
                                                }
                                            };

                                            // Draw the drag 'n' drop field and 
                                            // get the dropped template object.
                                            var droppedObj = VertPaintGUI.DragAndDropArea(
                                                dropArea,
                                                leftClick: click,
                                                rightClick: click,
                                                validityCheck: IsXml,
                                                guiContent: addFavButtonGUIContent,
                                                dragAndDropVisualMode: DragAndDropVisualMode.Copy,
                                                controlID: GUIUtility.GetControlID(FocusType.Passive)
                                            );

                                            Repaint();

                                            // Add the dropped template to the favorites list.
                                            if (droppedObj != null)
                                            {
                                                favoriteTemplates.Add(AssetDatabase.GetAssetPath(droppedObj));
                                                SaveFavoriteTemplates();
                                                Repaint();
                                            }

                                            GUI.enabled = favoriteTemplates.Count > 0;

                                            if (GUILayout.Button(removeFavButtonGUIContent, GUILayout.Width(25.0f)))
                                            {
                                                favoriteTemplates.RemoveAt(favoriteTemplates.Count - 1);
                                                SaveFavoriteTemplates();
                                            }

                                            if (GUILayout.Button(clearFavsButtonGUIContent, GUILayout.Width(25.0f)))
                                            {
                                                favoriteTemplates.Clear();
                                                SaveFavoriteTemplates();
                                            }

                                            GUI.enabled = true;
                                        }
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginVertical("Box", GUILayout.MaxHeight(160.0f));
                                        {
                                            favoritesListScrollPosition = GUILayout.BeginScrollView(favoritesListScrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
                                            {
                                                for (int i = 0; i < favoriteTemplates.Count; ++i)
                                                {
                                                    EditorGUILayout.BeginHorizontal();
                                                    {
                                                        if (GUILayout.Button("...", GUILayout.Width(30.0f)))
                                                        {
                                                            string templatePath = EditorUtility.OpenFilePanel("Reassign favorite VertPaint template", "Assets/VertPaint/Templates", "xml");
                                                            if (!string.IsNullOrEmpty(templatePath))
                                                            {
                                                                favoriteTemplates[i] = templatePath.Substring(Application.dataPath.Length - 6);
                                                                SaveFavoriteTemplates();
                                                            }
                                                        }

                                                        if (GUILayout.Button(Path.GetFileNameWithoutExtension(favoriteTemplates[i]), GUILayout.Width(position.width - 224.0f)))
                                                        {
                                                            if (!Load(favoriteTemplates[i]))
                                                            {
                                                                favoriteTemplates.RemoveAt(i);
                                                                SaveFavoriteTemplates();
                                                            }
                                                        }

                                                        if (GUILayout.Button("-", GUILayout.Width(30.0f)))
                                                        {
                                                            favoriteTemplates.RemoveAt(i);
                                                            SaveFavoriteTemplates();
                                                        }
                                                    }
                                                    EditorGUILayout.EndHorizontal();
                                                }
                                            }
                                            EditorGUILayout.EndScrollView();
                                        }
                                        EditorGUILayout.EndVertical();
                                    }
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Autosave directory: ", GUILayout.Width(120.0f));
                                    if (GUILayout.Button(autosaveDirectory, EditorStyles.textField, GUILayout.Height(16.0f)))
                                    {
                                        var path = EditorUtility.OpenFolderPanel("Select VertPaint autosave directory", DEFAULT_AUTOSAVE_DIRECTORY, string.Empty);
                                        if (!string.IsNullOrEmpty(path))
                                        {
                                            autosaveDirectory = path.Substring(Application.dataPath.Length - 6) + '/';
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                GUILayout.Space(1.0f);
                            }
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Mesh output directory

                        EditorGUILayout.BeginVertical("Box");
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField(meshOutputDirectoryGUIContent, GUILayout.Width(138.0f));
                                if (GUILayout.Button(new GUIContent(meshOutputDirectory, "This is the directory where VertPaint will store the vertex colored mesh assets (after clicking on \"Apply\")."), EditorStyles.textField, GUILayout.Height(16.0f)))
                                {
                                    var path = EditorUtility.OpenFolderPanel("Select mesh output folder", "Assets/", string.Empty);
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        meshOutputDirectory = path.Substring(Application.dataPath.Length - 6) + '/';
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();

                        #endregion

                        GUILayout.Space(1.0f);

                        #region Quick-action buttons

                        EditorGUILayout.BeginHorizontal();
                        {
                            // Fill
                            if (GUILayout.Button(fillGUIContent, GUILayout.Height(20.0f)))
                            {
                                InitializeAdditionalVertexStreams();

                                if (selectedMeshRenderer != null)
                                {
                                    // Allow the user to undo the fill action.
                                    Undo.RecordObject(selectedMeshRenderer.additionalVertexStreams, "fill " + color.ToString());

                                    // Fill all vertex colors with the current brush colors 
                                    // or hold down shift whilst clicking on "Fill" to fill with black.
                                    Mesh additionalVertexStreams = selectedMeshRenderer.additionalVertexStreams;
                                    Color[] colors = additionalVertexStreams.colors.Select(c => Event.current.modifiers == EventModifiers.Shift ? Color.clear : color).ToArray();
                                    additionalVertexStreams.colors = colors;
                                    EditorUtility.SetDirty(additionalVertexStreams);

                                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                                }
                            }

                            // Invert
                            if (GUILayout.Button(invertGUIContent, GUILayout.Height(20.0f)))
                            {
                                InitializeAdditionalVertexStreams();

                                if (selectedMeshRenderer != null)
                                {
                                    Undo.RecordObject(selectedMeshRenderer.additionalVertexStreams, "invert vertex colors");

                                    // Invert all vertex colors.
                                    Mesh additionalVertexStreams = selectedMeshRenderer.additionalVertexStreams;
                                    Color[] colors = additionalVertexStreams.colors.Select(c => new Color(1.0f - c.r, 1.0f - c.g, 1.0f - c.b)).ToArray();
                                    additionalVertexStreams.colors = colors;
                                    EditorUtility.SetDirty(additionalVertexStreams);

                                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                                }
                            }

                            // Discard
                            if (GUILayout.Button(discardGUIContent, GUILayout.Height(20.0f)))
                            {
                                DiscardPaintedVertexColors();
                            }

                            // Apply
                            if (GUILayout.Button(applyGUIContent, GUILayout.Height(20.0f)))
                            {
                                ApplyPaintedVertexColors();
                            }

                            // Clean up
                            if (GUILayout.Button(cleanMeshOutputDirectoryGUIContent, GUILayout.Height(20.0f)))
                            {
                                CleanMeshOutputDirectory();
                            }

                            // Reset
                            if (GUILayout.Button(resetGUIContent, GUILayout.Height(20.0f)))
                            {
                                Reset();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        #endregion

                        GUILayout.Space(2.0f);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(3.0f);
        }

        private void InitInspectorGUI()
        {
            if (saveGUIContent == null)
            {
                saveGUIContent = new GUIContent(EditorIcons.SaveIcon, "Save the current VertPaint configuration out to a template file.");
            }

            if (templateSlotGUIContent == null)
            {
                templateSlotGUIContent = new GUIContent(EditorIcons.TemplateSlot, "Load a VertPaint template file by dragging it into this field. Only valid template files with the correct format can be loaded! Loading will overwrite the current settings.");
            }

            if (addFavButtonGUIContent == null)
            {
                addFavButtonGUIContent = new GUIContent(EditorIcons.AddIcon, "Add a VertPaint template to your favourites list below by dragging and dropping it into this field, or by clicking here and selecting one manually.");
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com