using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace VertPaint
{
    /// <summary>
    /// Static utility class meant to extend the <see cref="UnityEngine.GUI"/> 
    /// class with useful methods that should be called from OnGUI only.
    /// </summary>
    public static class VertPaintGUI
    {
        private static GUIStyle centeredLabelStyle;

        /// <summary>
        /// The standard <see cref="EditorStyles.label"/> but centered (<see cref="TextAnchor.MiddleCenter"/>).
        /// </summary>
        public static GUIStyle CenteredLabelStyle
        {
            get
            {
                if (centeredLabelStyle == null)
                    centeredLabelStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
                return centeredLabelStyle;
            }
        }

        private static GUIStyle boldLabelButtonStyle;

        /// <summary>
        /// <see cref="GUIStyle"/> for the bold label buttons (the ones used for expanding/collapsing foldout sections in the VertPaint window). 
        /// </summary>
        public static GUIStyle BoldLabelButtonStyle
        {
            get
            {
                if (boldLabelButtonStyle == null)
                {
                    boldLabelButtonStyle = new GUIStyle(EditorStyles.boldLabel);
                    boldLabelButtonStyle.padding.top -= 3;
                }

                return boldLabelButtonStyle;
            }
        }

        /// <summary>
        /// The <see cref="MaterialProperty"/> to be edited by the object picker.
        /// </summary>
        private static MaterialProperty objPickerMaterialProperty;

        /// <summary>
        /// Displays a drag 'n' drop capable texture field (with preview) for a custom <see cref="MaterialEditor"/>.<para> </para>
        /// Left-clicking on it highlights the linked texture asset in the project panel (or opens the file picker dialog if the field is unassigned).<para> </para>
        /// Right-clicking erases the field.
        /// </summary>
        /// <param name="texture">The <see cref="MaterialProperty"/> containing the subject texture.</param>
        /// <param name="titleGUIContent">The title <see cref="GUIContent"/> to display as a centered bold label above the texture box.</param>
        /// <param name="width">Width of the texture field.</param>
        /// <param name="height">Height of the texture field.</param>
        /// <param name="editor">The inspector <see cref="Editor"/> instance to repaint when picking textures with the object picker (optional).</param>
        public static void TextureFieldWithCenteredTitle(MaterialProperty texture, GUIContent titleGUIContent = null, float width = 90, float height = 90, Editor editor = null)
        {
            if (titleGUIContent == null)
            {
                titleGUIContent = GUIContent.none;
            }

            // Centered label:
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(titleGUIContent, CenteredLabelStyle, GUILayout.Width(width));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                // Define Rect and GUIContent for the drop area.
                Rect dropArea = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                GUIContent dropGUIContent = new GUIContent(AssetPreview.GetAssetPreview(texture.textureValue), titleGUIContent.tooltip);

                // Define what happens when left-mouse clicking inside the drag 'n' drop box.
                Action leftClick = () =>
                {
                    if (texture.textureValue != null)
                    {
                        // When left-clicking on an assigned field, 
                        // highlight the linked texture asset inside the project panel.
                        EditorGUIUtility.PingObject(texture.textureValue);
                    }
                    else
                    {
                        // Show the texture picker window when 
                        // left-clicking on an empty texture field.
                        objPickerMaterialProperty = texture;
                        EditorGUIUtility.ShowObjectPicker<Texture2D>(null, false, string.Empty, GUIUtility.GetControlID(FocusType.Keyboard));
                    }
                };

                // Retreive, filter and assign the texture asset selected inside the object picker.
                if (string.CompareOrdinal(Event.current.commandName, "ObjectSelectorUpdated") == 0)
                {
                    var pickedTexture = EditorGUIUtility.GetObjectPickerObject() as Texture;
                    if (pickedTexture != null)
                    {
                        objPickerMaterialProperty.textureValue = pickedTexture;
                        EditorApplication.delayCall += editor.Repaint;
                    }
                }

                Texture2D droppedTexture = DragAndDropArea(
                    dropArea,
                    dropGUIContent,
                    leftClick: leftClick,
                    rightClick: () => texture.textureValue = null,
                    validityCheck: (draggedObj) => draggedObj is Texture2D,
                    dragAndDropVisualMode: DragAndDropVisualMode.Link) as Texture2D;

                if (droppedTexture != null)
                {
                    texture.textureValue = droppedTexture;
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Tiling/Offset:", GUILayout.Width(80));
                    float lw = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = width * 0.5f;

                    Vector4 v4 = texture.textureScaleAndOffset;
                    Vector2 xy = new Vector2(v4.x, v4.y);
                    Vector2 zw = new Vector2(v4.z, v4.w);

                    xy = EditorGUILayout.Vector2Field(GUIContent.none, xy);
                    zw = EditorGUILayout.Vector2Field(GUIContent.none, zw);

                    texture.textureScaleAndOffset = new Vector4(xy.x, xy.y, zw.x, zw.y);

                    EditorGUIUtility.labelWidth = lw;
                }
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Horizontally draws a <see cref="Texture2D"/> field with a <see cref="ColorComponent"/> source channel selector field to its right.
        /// </summary>
        /// <param name="labelText">The label to display to the left of the texture field.</param>
        /// <param name="labelWidth">The width of the label.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="colorComponent">The RGBA selector enum (source channel).</param>
        public static void TextureChannelExtractionField(string labelText, float labelWidth, ref Texture2D texture, ref ColorComponent colorComponent)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(labelText, GUILayout.Width(labelWidth));
                texture = EditorGUILayout.ObjectField(texture, typeof(Texture2D), false) as Texture2D;
                colorComponent = (ColorComponent)EditorGUILayout.EnumPopup(colorComponent, GUILayout.Width(30));
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws a <see cref="GUI.Box(Rect, GUIContent)"/> with drag 'n' drop behaviour.<para> </para>
        /// The method returns the dragged object if it's of the correct type (the one specified in the parameters). 
        /// Call this from OnGUI! Calling Repaint(); immediately after this is recommended.
        /// </summary>
        /// <returns>The dragged 'n' dropped object.</returns>
        /// <param name="dropAreaRect">The drag 'n' drop field <see cref="Rect"/>.</param>
        /// <param name="guiContent">The <see cref="GUIContent"/> to display inside the drag 'n' drop box.</param>
        /// <param name="validityCheck">The validator function of return type bool that specifies whether the dragged object (which is passed as parameter) is valid or not.<para> </para>If this is null, any dragged <see cref="UnityEngine.Object"/> can be dropped.<para> </para>A common check is, for instance, comparing the dragged object's type in order to filter by type (e.g. accept only drag 'n' drop for <see cref="Texture2D"/>s).</param>
        /// <param name="controlID">IMGUI Control identifier.</param>
        /// <param name="leftClick">Left click action.</param>
        /// <param name="rightClick">Right click action.</param>
        /// <param name="dragAndDropVisualMode">The mouse cursor's visual appearance when dragging and dropping a valid object.</param>
        public static Object DragAndDropArea(Rect dropAreaRect, GUIContent guiContent = null, Func<Object, bool> validityCheck = null, int? controlID = null, Action leftClick = null, Action rightClick = null, DragAndDropVisualMode dragAndDropVisualMode = DragAndDropVisualMode.Generic)
        {
            if (guiContent == null)
            {
                guiContent = GUIContent.none;
            }

            GUI.Box(dropAreaRect, guiContent);

            if (!dropAreaRect.Contains(Event.current.mousePosition))
                return default(Object);

            switch (controlID != null ? Event.current.GetTypeForControl(controlID.Value) : Event.current.type)
            {
                // Mouse click (L+R) behaviour:
                case EventType.MouseDown:
                {
                    switch (Event.current.button)
                    {
                        case 0:
                            if (leftClick != null)
                            {
                                leftClick.Invoke();
                                GUI.changed = true;
                                Event.current.Use();
                            }

                            break;
                        case 1:
                            if (rightClick != null)
                            {
                                rightClick.Invoke();
                                GUI.changed = true;
                                Event.current.Use();
                            }

                            break;
                    }

                    break;
                }

                // Drag and drop behaviour:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                {
                    // Get the dragged object and test its validity via the specified check function.
                    Object draggedObj = DragAndDrop.objectReferences[0];
                    bool validDrop = DragAndDrop.objectReferences.Length == 1 && (validityCheck == null || validityCheck.Invoke(draggedObj));

                    // Display the correct mouse cursor style based 
                    // on whether the dragged object is valid or not.
                    DragAndDrop.visualMode = validDrop ? dragAndDropVisualMode : DragAndDropVisualMode.Rejected;

                    // If the dragged object is valid, return it.
                    if (validDrop && Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        GUI.changed = true;
                        Event.current.Use();
                        return draggedObj;
                    }

                    break;
                }
            }

            // Avoid losing focus whilst dragging.
            if (EditorWindow.mouseOverWindow != null)
            {
                EditorWindow.mouseOverWindow.Focus();
                EditorWindow.mouseOverWindow.Repaint();
            }

            return default(Object);
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com