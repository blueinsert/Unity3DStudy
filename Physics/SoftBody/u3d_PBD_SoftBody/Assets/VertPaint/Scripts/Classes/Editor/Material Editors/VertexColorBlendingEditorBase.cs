using VertPaint;
using UnityEngine;
using UnityEditor;

internal abstract class VertexColorBlendingEditorBase : MaterialEditor
{
    private readonly GUIContent albedoGUIContent = new GUIContent("Albedo", "The RGB albedo (or base color) of this layer.");
    private readonly GUIContent normalmapGUIContent = new GUIContent("Normals", "The RGB tangent space normal map of this layer.");
    private readonly GUIContent mshaoGUIContent = new GUIContent("Mshao", "Mshao stands for \"Metallic, Smoothness, Height, Ambient Occlusion\".\n\nIt's an RGBA texture map that holds the metallic map in its red channel, the smoothness map in the green channel, height map in blue and AO in the alpha channel.");
    private readonly GUIContent shaderPrepUtilButtonGUIContent = new GUIContent("Open Shader Prep Utility", "The Shader Prep Utility is a simple tool that allows you to quickly repack your texture maps into a single texture.\n\nUse this to prepare your textures for usage in a shader that requires specific channel arrangements (like, for instance, the mshao setup for the vertex color blending shaders included in VertPaint).");

    protected void DrawLayerFieldsGUI(Color layerColor, string layerTitle, MaterialProperty color, MaterialProperty albedo, MaterialProperty normalmap, MaterialProperty mshao, MaterialProperty smoothness, MaterialProperty normalmapStrength)
    {
        if (!EditorGUIUtility.isProSkin)
            layerColor.a = 0.3f;

        var previousColor = GUI.color;
        GUI.color = layerColor;

        EditorGUILayout.BeginVertical("Box");
        {
            GUI.color = previousColor;

            EditorGUILayout.LabelField(layerTitle, EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            {
                float thumbnailSize = Mathf.Clamp(EditorGUIUtility.currentViewWidth * 0.3333333f - 20.0f, 25.0f, 130.0f);

                #region Albedo

                EditorGUILayout.BeginVertical();
                {
                    EditorGUI.BeginChangeCheck();

                    VertPaintGUI.TextureFieldWithCenteredTitle(albedo, albedoGUIContent, thumbnailSize, thumbnailSize, this);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Repaint();
                    }
                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("Tint", GUILayout.Width(25));
                        color.colorValue = EditorGUILayout.ColorField(color.colorValue, GUILayout.MaxWidth(thumbnailSize * .75f));
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                #endregion

                #region Normal map

                EditorGUILayout.BeginVertical();
                {
                    EditorGUI.BeginChangeCheck();

                    VertPaintGUI.TextureFieldWithCenteredTitle(normalmap, normalmapGUIContent, thumbnailSize, thumbnailSize, this);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Repaint();
                    }
                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        float lw = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 55;
                        normalmapStrength.floatValue = EditorGUILayout.FloatField("Strength", normalmapStrength.floatValue, GUILayout.MaxWidth(thumbnailSize - 5));
                        EditorGUIUtility.labelWidth = lw;
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                #endregion

                #region Mshao

                EditorGUILayout.BeginVertical();
                {
                    EditorGUI.BeginChangeCheck();

                    VertPaintGUI.TextureFieldWithCenteredTitle(mshao, mshaoGUIContent, thumbnailSize, thumbnailSize, this);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Repaint();
                    }
                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        float lw = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 73;
                        smoothness.floatValue = EditorGUILayout.Slider("Smoothness", smoothness.floatValue, 0.0f, 2.0f, GUILayout.MaxWidth(thumbnailSize - 5));
                        EditorGUIUtility.labelWidth = lw;
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                #endregion
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(7);
    }

    protected void DrawShaderPrepUtilButton()
    {
        if (GUILayout.Button(shaderPrepUtilButtonGUIContent, GUILayout.Height(25.0f)))
        {
            OpenShaderPrepUtil(new MenuCommand(target));
        }
    }

    [MenuItem("CONTEXT/Material/Open Shader Prep Utility")]
    private static void OpenShaderPrepUtil(MenuCommand menuCommand)
    {
        ShaderPrepUtilityWindow.Open();
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com