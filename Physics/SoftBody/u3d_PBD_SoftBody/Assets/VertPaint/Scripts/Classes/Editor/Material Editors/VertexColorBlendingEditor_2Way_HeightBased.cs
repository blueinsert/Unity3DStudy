using UnityEngine;
using UnityEditor;

internal class VertexColorBlendingEditor_2Way_HeightBased : VertexColorBlendingEditorBase
{
    // The material properties:
    private MaterialProperty color1, albedo1, normalmap1, mshao1, smoothness1, normalmapStrength1;
    private MaterialProperty color2, albedo2, normalmap2, mshao2, smoothness2, normalmapStrength2;
    private MaterialProperty blend;

    public override void OnEnable()
    {
        base.OnEnable();

        var targetObjects = serializedObject.targetObjects;
        if (targetObjects == null || targetObjects.Length == 0)
            return;

        color1 = GetMaterialProperty(targetObjects, 0);
        albedo1 = GetMaterialProperty(targetObjects, 1);
        normalmap1 = GetMaterialProperty(targetObjects, 2);
        mshao1 = GetMaterialProperty(targetObjects, 3);
        smoothness1 = GetMaterialProperty(targetObjects, 4);
        normalmapStrength1 = GetMaterialProperty(targetObjects, 5);

        color2 = GetMaterialProperty(targetObjects, 6);
        albedo2 = GetMaterialProperty(targetObjects, 7);
        normalmap2 = GetMaterialProperty(targetObjects, 8);
        mshao2 = GetMaterialProperty(targetObjects, 9);
        smoothness2 = GetMaterialProperty(targetObjects, 10);
        normalmapStrength2 = GetMaterialProperty(targetObjects, 11);

        blend = GetMaterialProperty(targetObjects, 12);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLayerFieldsGUI(Color.white, "Background Layer", color1, albedo1, normalmap1, mshao1, smoothness1, normalmapStrength1);
        DrawLayerFieldsGUI(Color.red, "Red Layer", color2, albedo2, normalmap2, mshao2, smoothness2, normalmapStrength2);

        blend.floatValue = EditorGUILayout.Slider("Blend", blend.floatValue, -3, 1);
        GUILayout.Space(5);

        DrawShaderPrepUtilButton();

        serializedObject.ApplyModifiedProperties();
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com