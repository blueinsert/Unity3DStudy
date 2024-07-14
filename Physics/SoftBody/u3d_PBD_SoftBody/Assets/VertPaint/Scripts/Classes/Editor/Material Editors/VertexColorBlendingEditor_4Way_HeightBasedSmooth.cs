using UnityEngine;
using UnityEditor;

internal class VertexColorBlendingEditor_4Way_HeightBasedSmooth : VertexColorBlendingEditorBase
{
    // The material properties:
    private MaterialProperty color1, albedo1, normalmap1, mshao1, smoothness1, normalmapStrength1;
    private MaterialProperty color2, albedo2, normalmap2, mshao2, smoothness2, normalmapStrength2;
    private MaterialProperty color3, albedo3, normalmap3, mshao3, smoothness3, normalmapStrength3;
    private MaterialProperty color4, albedo4, normalmap4, mshao4, smoothness4, normalmapStrength4;
    private MaterialProperty heightShift1, heightShift2, heightShift3, heightShift4, transitionSmoothness;

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
        heightShift1 = GetMaterialProperty(targetObjects, 6);

        color2 = GetMaterialProperty(targetObjects, 7);
        albedo2 = GetMaterialProperty(targetObjects, 8);
        normalmap2 = GetMaterialProperty(targetObjects, 9);
        mshao2 = GetMaterialProperty(targetObjects, 10);
        smoothness2 = GetMaterialProperty(targetObjects, 11);
        normalmapStrength2 = GetMaterialProperty(targetObjects, 12);
        heightShift2 = GetMaterialProperty(targetObjects, 13);

        color3 = GetMaterialProperty(targetObjects, 14);
        albedo3 = GetMaterialProperty(targetObjects, 15);
        normalmap3 = GetMaterialProperty(targetObjects, 16);
        mshao3 = GetMaterialProperty(targetObjects, 17);
        smoothness3 = GetMaterialProperty(targetObjects, 18);
        normalmapStrength3 = GetMaterialProperty(targetObjects, 19);
        heightShift3 = GetMaterialProperty(targetObjects, 20);

        color4 = GetMaterialProperty(targetObjects, 21);
        albedo4 = GetMaterialProperty(targetObjects, 22);
        normalmap4 = GetMaterialProperty(targetObjects, 23);
        mshao4 = GetMaterialProperty(targetObjects, 24);
        smoothness4 = GetMaterialProperty(targetObjects, 25);
        normalmapStrength4 = GetMaterialProperty(targetObjects, 26);
        heightShift4 = GetMaterialProperty(targetObjects, 27);

        transitionSmoothness = GetMaterialProperty(targetObjects, 28);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawLayerFieldsGUI(Color.white, "Background Layer", color1, albedo1, normalmap1, mshao1, smoothness1, normalmapStrength1);
        DrawLayerFieldsGUI(Color.red, "Red Layer", color2, albedo2, normalmap2, mshao2, smoothness2, normalmapStrength2);
        DrawLayerFieldsGUI(Color.green, "Green Layer", color3, albedo3, normalmap3, mshao3, smoothness3, normalmapStrength3);
        DrawLayerFieldsGUI(Color.blue, "Blue Layer", color4, albedo4, normalmap4, mshao4, smoothness4, normalmapStrength4);

        heightShift1.floatValue = EditorGUILayout.Slider("Background Height Shift", heightShift1.floatValue, -3, 3);
        heightShift2.floatValue = EditorGUILayout.Slider("Red Height Shift", heightShift2.floatValue, -3, 3);
        heightShift3.floatValue = EditorGUILayout.Slider("Green Height Shift", heightShift3.floatValue, -3, 3);
        heightShift4.floatValue = EditorGUILayout.Slider("Blue Height Shift", heightShift4.floatValue, -3, 3);

        transitionSmoothness.floatValue = EditorGUILayout.Slider("Transition Smoothness", transitionSmoothness.floatValue, 0.0001f, 1.0f);
        GUILayout.Space(5);

        DrawShaderPrepUtilButton();

        serializedObject.ApplyModifiedProperties();
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com