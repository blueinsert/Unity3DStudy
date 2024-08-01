using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bluebean
{
    public class BrushModeContainer
    {
        public IVertexPropertyHolder vertexPropertyHolder;

        protected List<IBrushMode> brushModes = new List<IBrushMode>();
        private int selectedBrushMode;

        private float m_curExpectedValue = 0;

        public BrushModeContainer(IVertexPropertyHolder skinEditor)
        {
            this.vertexPropertyHolder = skinEditor;
            brushModes.Add(new BrushModePaint(this));
            brushModes.Add(new BrushModeAdd(this));
            brushModes.Add(new BrushModeSmooth(this));
        }

        protected void Initialize(BrushBase paintBrush)
        {
            // Initialize the brush if there's no brush mode set:
            if (paintBrush.brushMode == null && brushModes.Count > 0)
            {
                selectedBrushMode = 0;
                paintBrush.brushMode = brushModes[selectedBrushMode];
            }
        }

        public void DrawBrushModes(BrushBase paintBrush)
        {
            Initialize(paintBrush);

            GUIContent[] contents = new GUIContent[brushModes.Count];
            for (int i = 0; i < brushModes.Count; ++i)
                contents[i] = new GUIContent(brushModes[i].name);

            EditorGUI.BeginChangeCheck();
            selectedBrushMode = EditorUtils.DoToolBar(selectedBrushMode, contents);
            if (EditorGUI.EndChangeCheck())
            {
                paintBrush.brushMode = brushModes[selectedBrushMode];
            }
        }


        public float Get(int index)
        {
            return vertexPropertyHolder.GetVertexProperty(index).x;
        }
        public void Set(int index, float value)
        {
            vertexPropertyHolder.SetVertexProperty(index, new UnityEngine.Vector4(value, value, value, value));
        }
        public bool Masked(int index)
        {
            return false;//!skinEditor.facingCamera[index];
        }

        public float GetDefault() 
        { 
            return m_curExpectedValue;//todo
        }

        public void SetExpectValue(float value)
        {
            m_curExpectedValue = value;
        }
    }
}
