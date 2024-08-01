using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace bluebean
{
    [CustomEditor(typeof(VertexPaintTool)), CanEditMultipleObjects]
    public class VertexPaintToolInspector : Editor
    {
        private VertexPaintTool m_vertexPaintTool = null;
        private IVertexPropertyHolder m_vertexPropertyHolder = null;
        private RaycastBrush paintBrush;
        private BrushModeContainer m_brushModeContainer = null;
        private Material paintMaterial;

        private bool m_isInEditing = false;

        //private Vector4 m_value = Vector4.one;
        private float m_value = 1.0f;
        private Color m_brushWireframeColor = Color.green;
        public void OnEnable()
        {
            var vertexPaintTool = (VertexPaintTool)target;
            m_vertexPaintTool = vertexPaintTool;
            m_vertexPropertyHolder = vertexPaintTool.GetVertexPropertyHolder();
            paintBrush = new RaycastBrush(null,
                                                         () =>
                                                         {
                                                             // As RecordObject diffs with the end of the current frame,
                                                             // and this is a multi-frame operation, we need to use RegisterCompleteObjectUndo instead.
                                                             Undo.RegisterCompleteObjectUndo(target, "Paint influences");
                                                         },
                                                         () =>
                                                         {
                                                             SceneView.RepaintAll();
                                                         },
                                                         () =>
                                                         {
                                                             EditorUtility.SetDirty(target);
                                                         });

            m_brushModeContainer = new BrushModeContainer(this.m_vertexPropertyHolder);

            if (paintMaterial == null)
                paintMaterial = Resources.Load<Material>("PropertyGradientMaterial");

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            m_isInEditing = GUILayout.Toggle(m_isInEditing, new GUIContent("Paint Influence"), "Button");
            if (EditorGUI.EndChangeCheck())
                SceneView.RepaintAll();

            if (m_isInEditing && paintBrush != null)
            {
                //DrawBrushMode();
                m_brushModeContainer.DrawBrushModes(paintBrush);

                //if (paintBrush.brushMode.needsInputValue)
                //    m_brushModeContainer.PropertyField();
                m_value = EditorGUILayout.Slider("Value", m_value, 0, 1);
                m_brushModeContainer.SetExpectValue(m_value);
                
                if (GUILayout.Button("Clear"))
                {
                    m_vertexPropertyHolder.Clear();
                }
                paintBrush.m_brushColor = EditorGUILayout.ColorField("BrushColor", paintBrush.m_brushColor);
                m_brushWireframeColor = EditorGUILayout.ColorField("BrushWireframeColor", m_brushWireframeColor);
                paintBrush.radius = EditorGUILayout.Slider("Brush size", paintBrush.radius, 0.0001f, 0.5f);
                paintBrush.innerRadius = EditorGUILayout.Slider("Brush inner size", paintBrush.innerRadius, 0, 1);
                paintBrush.opacity = EditorGUILayout.Slider("Brush opacity", paintBrush.opacity, 0, 1);
            }

            // Apply changes to the serializedProperty
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void OnSceneGUI()
        {
            if (m_isInEditing)
            {

                SkinnedMeshRenderer skin = m_vertexPaintTool.GetComponent<SkinnedMeshRenderer>();

                if (skin != null && skin.sharedMesh != null)
                {
                    var bakedMesh = new Mesh();
                    skin.BakeMesh(bakedMesh);

                    if (Event.current.type == EventType.Repaint)
                        DrawMesh(bakedMesh);

                    if (Camera.current != null)
                    {
                        paintBrush.raycastTarget = bakedMesh;
                        paintBrush.raycastTransform = skin.transform.localToWorldMatrix;

                        // TODO: do better.
                        var v = bakedMesh.vertices;
                        Vector3[] worldSpaceVertexList = new Vector3[v.Length];
                        for (int i = 0; i < worldSpaceVertexList.Length; ++i)
                            worldSpaceVertexList[i] = paintBrush.raycastTransform.MultiplyPoint3x4(v[i]);

                        paintBrush.DoBrush(worldSpaceVertexList);
                    }

                    DestroyImmediate(bakedMesh);
                }

            }
        }

        private void DrawMesh(Mesh mesh)
        {
            if (paintMaterial.SetPass(0))
            {
                Color[] colors = new Color[mesh.vertexCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    var property = m_vertexPropertyHolder.GetVertexProperty(i);
                    colors[i] = new Color(property.x, property.y, property.z);
                }

                mesh.colors = colors;
                Graphics.DrawMeshNow(mesh, paintBrush.raycastTransform);

                if (paintMaterial.SetPass(1))
                {
                    Color wireColor = m_brushWireframeColor;
                    for (int i = 0; i < paintBrush.weights.Length; i++)
                    {
                        colors[i] = wireColor * paintBrush.weights[i];
                    }

                    mesh.colors = colors;
                    GL.wireframe = true;
                    Graphics.DrawMeshNow(mesh, paintBrush.raycastTransform);
                    GL.wireframe = false;
                }
            }
        }


    }
}