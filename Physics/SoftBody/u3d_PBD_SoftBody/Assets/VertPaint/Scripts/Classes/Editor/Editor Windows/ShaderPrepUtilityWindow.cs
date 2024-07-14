using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace VertPaint
{
    public class ShaderPrepUtilityWindow : EditorWindow
    {
        #region Constants

        private const string defaultOutputDirectory = "Assets/VertPaint/Output/Textures/";
        private const string defaultFileName = "Repacked Texture.png";

        private const ColorComponent defaultSourceChannelR = ColorComponent.R;
        private const ColorComponent defaultSourceChannelG = ColorComponent.R;
        private const ColorComponent defaultSourceChannelB = ColorComponent.R;
        private const ColorComponent defaultSourceChannelA = ColorComponent.A;

        private const PostRepackAction defaultPostRepackAction = PostRepackAction.ResetInputTextures;

        private readonly Color32 black = new Color32(0, 0, 0, 255);
        private readonly Color32 white = new Color32(255, 255, 255, 255);
        private readonly Color32 redPersonalUI = new Color32(255, 0, 0, 85);
        private readonly Color32 greenPersonalUI = new Color32(0, 255, 0, 85);
        private readonly Color32 bluePersonalUI = new Color32(0, 0, 255, 85);

        private readonly GUIContent resetAllGUIContent = new GUIContent("Reset all", "Reset all shader prep utility settings/fields back to their default values.");
        private readonly GUIContent resetInputTexturesGUIContent = new GUIContent("Reset input textures", "Reset the input texture fields back to null.");
        private readonly GUIContent repackGUIContent = new GUIContent("Repack", "Repack the specified input textures' source channels into a single texture asset and save it out to disk at the specified output file path.");
        private readonly GUIContent postRepackActionGUIContent = new GUIContent("Post-repack action:", "Decide what should happen with this shader prep utility instance after clicking on \"Repack\".");
        private readonly GUIContent textureOutputDirectoryGUIContent = new GUIContent("Output directory:", "This is the output directory into which all repacked texture assets will be placed.");
        private readonly GUIContent fileNameGUIContent = new GUIContent("File name:", "Define how the repacked texture asset should be called.");

        #endregion

        #region Events

        /// <summary>
        /// This event is raised each time a set of <see cref="Texture2D"/> channels 
        /// has been repacked and saved to disk with the shader prep utility.<para> </para>
        /// The resulting <see cref="Texture2D"/> (along with other repacking relevant data) 
        /// is passed to the subscribers within the <see cref="ShaderPrepEventArgs"/> event arguments class.
        /// </summary>
        public event EventHandler<ShaderPrepEventArgs> Repacked;

        #endregion

        [SerializeField]
        private Texture2D rInputTexture;

        [SerializeField]
        private ColorComponent rSourceChannel = defaultSourceChannelR;

        [SerializeField]
        private Texture2D gInputTexture;

        [SerializeField]
        private ColorComponent gSourceChannel = defaultSourceChannelG;

        [SerializeField]
        private Texture2D bInputTexture;

        [SerializeField]
        private ColorComponent bSourceChannel = defaultSourceChannelB;

        [SerializeField]
        private Texture2D aInputTexture;

        [SerializeField]
        private ColorComponent aSourceChannel = defaultSourceChannelA;

        [SerializeField]
        private Texture2D preview;

        [SerializeField]
        private string textureOutputDirectory = defaultOutputDirectory;

        [SerializeField]
        private string fileName = defaultFileName;

        [SerializeField]
        private PostRepackAction postRepackAction = defaultPostRepackAction;

        private List<int> widths = new List<int>(4);

        //-------------------------------------------------------------------------------------

        /// <summary>
        /// Opens the Shader Prep Utility window.
        /// </summary>
        public static void Open()
        {
            var window = GetWindow(typeof(ShaderPrepUtilityWindow), true) as ShaderPrepUtilityWindow;
            if (window == null)
            {
                Debug.LogError("VertPaint: Failed to open Shader Prep Utility!");
                return;
            }

            window.Show();
            window.minSize = new Vector2(405, 205);
            window.maxSize = new Vector2(600, 205);
            window.titleContent = new GUIContent("VertPaint - Shader Prep Utility");
        }

        private void ResetAll()
        {
            Undo.RegisterCompleteObjectUndo(this, "reset shader prep utility");

            ResetInputTextures();
            rSourceChannel = gSourceChannel = bSourceChannel = ColorComponent.R;
            aSourceChannel = ColorComponent.A;
            postRepackAction = PostRepackAction.ResetInputTextures;
            textureOutputDirectory = defaultOutputDirectory;
            fileName = defaultFileName;

            Repaint();
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private void ResetInputTextures()
        {
            Undo.RecordObject(this, "reset shader prep utility input textures");
            rInputTexture = gInputTexture = bInputTexture = aInputTexture = preview = null;
            Repaint();
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private void OnEnable()
        {
            if (!Directory.Exists(textureOutputDirectory))
            {
                Directory.CreateDirectory(textureOutputDirectory);
            }

            string rGuid = EditorPrefs.GetString("vp_sp_util_r", string.Empty);
            if (!string.IsNullOrEmpty(rGuid))
            {
                string path = AssetDatabase.GUIDToAssetPath(rGuid);
                if (!string.IsNullOrEmpty(path))
                {
                    rInputTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            rSourceChannel = (ColorComponent)EditorPrefs.GetInt("vp_sp_util_r_src_chl", 0);

            string gGuid = EditorPrefs.GetString("vp_sp_util_g", string.Empty);
            if (!string.IsNullOrEmpty(gGuid))
            {
                string path = AssetDatabase.GUIDToAssetPath(gGuid);
                if (!string.IsNullOrEmpty(path))
                {
                    gInputTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            gSourceChannel = (ColorComponent)EditorPrefs.GetInt("vp_sp_util_g_src_chl", 0);

            string bGuid = EditorPrefs.GetString("vp_sp_util_b", string.Empty);
            if (!string.IsNullOrEmpty(bGuid))
            {
                string path = AssetDatabase.GUIDToAssetPath(bGuid);
                if (!string.IsNullOrEmpty(path))
                {
                    bInputTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            bSourceChannel = (ColorComponent)EditorPrefs.GetInt("vp_sp_util_b_src_chl", 0);

            string aGuid = EditorPrefs.GetString("vp_sp_util_a", string.Empty);
            if (!string.IsNullOrEmpty(aGuid))
            {
                string path = AssetDatabase.GUIDToAssetPath(aGuid);
                if (!string.IsNullOrEmpty(path))
                {
                    aInputTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            aSourceChannel = (ColorComponent)EditorPrefs.GetInt("vp_sp_util_a_src_chl", 3);

            fileName = EditorPrefs.GetString("vp_sp_util_1", defaultFileName);
            textureOutputDirectory = EditorPrefs.GetString("vp_sp_util_2", defaultOutputDirectory);
            postRepackAction = (PostRepackAction)EditorPrefs.GetInt("vp_sp_util_3", 2);

            UpdatePreview();
            Repaint();
            Undo.ClearUndo(this);
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void OnDisable()
        {
            EditorPrefs.SetString("vp_sp_util_r", AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(rInputTexture)));
            EditorPrefs.SetInt("vp_sp_util_r_src_chl", (int)rSourceChannel);

            EditorPrefs.SetString("vp_sp_util_g", AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(gInputTexture)));
            EditorPrefs.SetInt("vp_sp_util_g_src_chl", (int)gSourceChannel);

            EditorPrefs.SetString("vp_sp_util_b", AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(bInputTexture)));
            EditorPrefs.SetInt("vp_sp_util_b_src_chl", (int)bSourceChannel);

            EditorPrefs.SetString("vp_sp_util_a", AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(aInputTexture)));
            EditorPrefs.SetInt("vp_sp_util_a_src_chl", (int)aSourceChannel);

            EditorPrefs.SetString("vp_sp_util_1", fileName);
            EditorPrefs.SetString("vp_sp_util_2", textureOutputDirectory);
            EditorPrefs.SetInt("vp_sp_util_3", (int)postRepackAction);

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            Repaint();
        }

        private void OnGUI()
        {
            Undo.RecordObject(this, "change shader prep settings");

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Box(preview == null ? Texture2D.blackTexture : preview, GUILayout.Width(100), GUILayout.Height(100));

                EditorGUILayout.BeginVertical();
                {
                    EditorGUI.BeginChangeCheck();

                    // --------------- RED ---------------

                    if (EditorGUIUtility.isProSkin)
                        GUI.color = Color.red;
                    else
                        GUI.color = redPersonalUI;

                    EditorGUILayout.BeginVertical("Box");
                    {
                        GUI.color = Color.white;
                        VertPaintGUI.TextureChannelExtractionField("Red channel", 100, ref rInputTexture, ref rSourceChannel);
                    }
                    EditorGUILayout.EndVertical();

                    // -------------- GREEN --------------

                    if (EditorGUIUtility.isProSkin)
                        GUI.color = Color.green;
                    else
                        GUI.color = greenPersonalUI;

                    EditorGUILayout.BeginVertical("Box");
                    {
                        GUI.color = Color.white;
                        VertPaintGUI.TextureChannelExtractionField("Green channel", 100, ref gInputTexture, ref gSourceChannel);
                    }
                    EditorGUILayout.EndVertical();

                    // --------------- BLUE ---------------

                    if (EditorGUIUtility.isProSkin)
                        GUI.color = Color.blue;
                    else
                        GUI.color = bluePersonalUI;

                    EditorGUILayout.BeginVertical("Box");
                    {
                        GUI.color = Color.white;
                        VertPaintGUI.TextureChannelExtractionField("Blue channel", 100, ref bInputTexture, ref bSourceChannel);
                    }
                    EditorGUILayout.EndVertical();

                    // -------------- ALPHA --------------

                    EditorGUILayout.BeginVertical("Box");
                    {
                        VertPaintGUI.TextureChannelExtractionField("Alpha channel", 100, ref aInputTexture, ref aSourceChannel);
                    }
                    EditorGUILayout.EndVertical();

                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdatePreview();
                        Repaint();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            // File name:
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(fileNameGUIContent, GUILayout.Width(120));

                EditorGUI.BeginChangeCheck();
                fileName = EditorGUILayout.TextField(fileName);
                if (EditorGUI.EndChangeCheck())
                {
                    if (!fileName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        fileName += ".png";
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // Texture output directory:
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(textureOutputDirectoryGUIContent, GUILayout.Width(120));
                if (GUILayout.Button(textureOutputDirectory, EditorStyles.textField, GUILayout.Height(16.0f)))
                {
                    var path = EditorUtility.OpenFolderPanel("Select texture output folder", "Assets/", string.Empty);
                    if (!string.IsNullOrEmpty(path))
                    {
                        textureOutputDirectory = path.Substring(Application.dataPath.Length - 6) + '/';
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // Post-repack action:
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(postRepackActionGUIContent, GUILayout.Width(120));
                postRepackAction = (PostRepackAction)EditorGUILayout.EnumPopup(postRepackAction);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                // Reset input textures button
                if (GUILayout.Button(resetInputTexturesGUIContent, GUILayout.Height(25)))
                {
                    ResetInputTextures();
                }

                // Reset all button
                if (GUILayout.Button(resetAllGUIContent, GUILayout.Height(25)))
                {
                    ResetAll();
                }

                // Repack button
                GUI.enabled = preview != null;
                if (GUILayout.Button(repackGUIContent, GUILayout.Height(25)))
                {
                    Repack();
                }

                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void UpdatePreview()
        {
            widths.Clear();

            if (rInputTexture != null)
            {
                if (IsSquare(rInputTexture))
                {
                    widths.Add(rInputTexture.width);
                }
                else
                {
                    Debug.LogError("VertPaint - Shader Prep Utility: The texture \"" + rInputTexture.name + "\" isn't a square format.");
                    rInputTexture = null;
                }
            }

            if (gInputTexture != null)
            {
                if (IsSquare(gInputTexture))
                {
                    widths.Add(gInputTexture.width);
                }
                else
                {
                    Debug.LogError("VertPaint - Shader Prep Utility: The texture \"" + gInputTexture.name + "\" isn't a square format.");
                    gInputTexture = null;
                }
            }

            if (bInputTexture != null)
            {
                if (IsSquare(bInputTexture))
                {
                    widths.Add(bInputTexture.width);
                }
                else
                {
                    Debug.LogError("VertPaint - Shader Prep Utility: The texture \"" + bInputTexture.name + "\" isn't a square format.");
                    bInputTexture = null;
                }
            }

            if (aInputTexture != null)
            {
                if (IsSquare(aInputTexture))
                {
                    widths.Add(aInputTexture.width);
                }
                else
                {
                    Debug.LogError("VertPaint - Shader Prep Utility: The texture \"" + aInputTexture.name + "\" isn't a square format.");
                    aInputTexture = null;
                }
            }

            if (widths.Count == 0)
            {
                preview = null;
                return;
            }

            // The input textures have to share the same, identical resolution,
            // otherwise the repacking can't take place. If a texture is assigned 
            // with a different resolution, the input texture fields are cleared.
            if (widths.Distinct().Count() > 1)
            {
                Debug.LogError("VertPaint - Shader Prep Utility: The specified input textures don't share the same resolution.");
                ResetInputTextures();
                return;
            }

            // At this point we know that all the specified input textures are
            // perfect squares with an aspect ratio of 1 and identical resolutions.
            // This allows us to calculate the resolution just by squaring the width.
            int width = widths[0];
            int resolution = width * width;

            // Declare pixel arrays for each color channel.
            Color32[] rPixels = null;
            Color32[] gPixels = null;
            Color32[] bPixels = null;
            Color32[] aPixels = null;

            // Get the pixels of each input texture 
            // and assign them to the above defined arrays.
            if (rInputTexture != null) rPixels = GetPixels32(rInputTexture);
            if (gInputTexture != null) gPixels = GetPixels32(gInputTexture);
            if (bInputTexture != null) bPixels = GetPixels32(bInputTexture);
            if (aInputTexture != null) aPixels = GetPixels32(aInputTexture);

            // Create default channels for unassigned input textures
            // (plain black for R, G and B; white for Alpha).
            Color32[] defBlackPixels = new Color32[resolution];
            for (int i = defBlackPixels.Length - 1; i >= 0; i--)
                defBlackPixels[i] = black;

            Color32[] defWhitePixels = new Color32[resolution];
            for (int i = defWhitePixels.Length - 1; i >= 0; i--)
                defWhitePixels[i] = white;

            if (rPixels == null) rPixels = defBlackPixels;
            if (gPixels == null) gPixels = defBlackPixels;
            if (bPixels == null) bPixels = defBlackPixels;
            if (aPixels == null) aPixels = defWhitePixels;

            // Create the final pixels array containing the 
            // remapped color channels for the preview texture.
            Color32[] finalPixels = new Color32[resolution];
            for (int i = finalPixels.Length - 1; i >= 0; i--)
            {
                byte r = GetPixelColorComponent(rPixels[i], rSourceChannel);
                byte g = GetPixelColorComponent(gPixels[i], gSourceChannel);
                byte b = GetPixelColorComponent(bPixels[i], bSourceChannel);
                byte a = GetPixelColorComponent(aPixels[i], aSourceChannel);

                finalPixels[i] = new Color32(r, g, b, a);
            }

            // Apply the pixels to a new preview texture.
            preview = new Texture2D(width, width, TextureFormat.RGBA32, false, true);
            preview.SetPixels32(finalPixels);
            preview.Apply();
        }

        /// <summary>
        /// Checks if the passed <see cref="Texture2D"/> is a square image or not 
        /// (non-square formats aren't supported).
        /// </summary>
        /// <param name="texture">Texture to check against square format.</param>
        private bool IsSquare(Texture2D texture)
        {
            return texture.width == texture.height;
        }

        /// <summary>
        /// Extracts the single color component (R, G, B or A) of type <see cref="byte"/> [0, 255] from a single pixel.
        /// </summary>
        /// <returns>The specified pixel's single color component 
        /// (R, G, B or A of type <see cref="byte"/> to be used with <see cref="Color32"/>).</returns>
        /// <param name="pixel">Source pixel whose color component byte you want to get.</param>
        /// <param name="component">Color component to extract.</param>
        private byte GetPixelColorComponent(Color32 pixel, ColorComponent component)
        {
            switch (component)
            {
                case ColorComponent.R:
                    return pixel.r;
                case ColorComponent.G:
                    return pixel.g;
                case ColorComponent.B:
                    return pixel.b;
                case ColorComponent.A:
                    return pixel.a;
                default:
                    return default(byte);
            }
        }

        /// <summary>
        /// Gets the pixels (Color32[] array) of a texture, even if it's non-readable.
        /// </summary>
        /// <returns>The input texture's pixels array (Color32[]).</returns>
        private Color32[] GetPixels32(Texture2D texture)
        {
            // Create a temporary RenderTexture of the same size as the input texture.
            RenderTexture tempRT = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            // Blit the texture's pixels to the temporary RenderTexture.
            Graphics.Blit(texture, tempRT);

            // Keep track of the current RT; we'll restore it once we're done here.
            RenderTexture previousRT = RenderTexture.active;

            // Set the current RenderTexture 
            // to the temporary one we created.
            RenderTexture.active = tempRT;

            // Create a new (readable) Texture2D to copy the pixels into it.
            Texture2D tempTex = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, true);

            // Copy the pixels from the RenderTexture to the new Texture.
            tempTex.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            tempTex.Apply();

            // Reset the active RenderTexture.
            RenderTexture.active = previousRT;

            // Release the temporary RenderTexture.
            RenderTexture.ReleaseTemporary(tempRT);

            return tempTex.GetPixels32();
        }

        /// <summary>
        /// Repacks the selected input textures' channels into one final texture asset, 
        /// which is then saved out to disk at the specified file path (output directory + file name).
        /// </summary>
        private void Repack()
        {
            if (string.IsNullOrEmpty(textureOutputDirectory) || string.IsNullOrEmpty(fileName) || preview == null)
            {
                return;
            }

            // Create the output directory if it doesn't exist yet.
            if (!Directory.Exists(textureOutputDirectory))
            {
                Directory.CreateDirectory(textureOutputDirectory);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            // Generate a unique file path for the output texture asset.
            string filePath = AssetDatabase.GenerateUniqueAssetPath(textureOutputDirectory + fileName);

            // Create the final png file based on the preview.
            byte[] pngBytes = preview.EncodeToPNG();

            // Save it out to disk at the specified file path.
            File.WriteAllBytes(filePath, pngBytes);
            AssetDatabase.Refresh();

            // Highlight the repacked texture asset inside the project panel.
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityEngine.Object)));

            // Raise the Repacked event for any interested subscribers.
            OnRepacked(new ShaderPrepEventArgs(preview, pngBytes, filePath));

            // Decide what to do once finished 
            // (according to the user's preference).
            switch (postRepackAction)
            {
                case PostRepackAction.Close:
                    Close();
                    break;
                case PostRepackAction.ResetAll:
                    ResetAll();
                    break;
                case PostRepackAction.ResetInputTextures:
                    ResetInputTextures();
                    break;
            }
        }

        /// <summary>
        /// Invokes the <see cref="Repacked"/> event with the passed event args.
        /// </summary>
        /// <param name="eventArgs">Event arguments.</param>
        protected virtual void OnRepacked(ShaderPrepEventArgs eventArgs)
        {
            if (Repacked != null)
            {
                Repacked.Invoke(this, eventArgs);
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com