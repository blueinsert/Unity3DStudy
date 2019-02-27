using UnityEditor;
using UnityEngine;

namespace bluebean
{
    public enum ShapeEditMode
    {
        None,
        Editing,
    }

    public class PolygonEditor : CanvasEditorWindow
    {
        [MenuItem("Windows/OpenPolygonEditor")]
        public static void OpenPolygonEditor()
        {
            var win = GetWindow<PolygonEditor>(typeof(PolygonEditor).Name);
            win.wantsMouseMove = true;
            win.Initialize();
        }

        #region 窗口绘制

        #region override

        protected override void OnDrawCanvas(CanvasContex canvasContex)
        {
            foreach (var ele in canvasContex.m_elements)
            {
                ele.Draw(canvasContex.IsSelected(ele) ? ele.selectedColor : ele.unSelectedColor);
            } 
            if (m_editMode == ShapeEditMode.Editing)
            {
                GLHelper.DrawCircle(canvasContex.m_curLocalPosition, 20, 36, Color.red);
                if (m_data.nodes.Count > 0)
                    GLHelper.DrawLine(m_data.nodes[m_data.nodes.Count - 1].position, canvasContex.m_curLocalPosition, Color.blue);
            } 
        }

        #endregion

        #endregion

        #region UI事件处理

        #region override

        protected override void ProcessEventInCanvas(CanvasContex canvasContex)
        {
            base.ProcessEventInCanvas(canvasContex);
            if (m_editMode == ShapeEditMode.Editing)
            {
                //编辑时点击鼠标右键取消编辑
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    ChangeEditMode(ShapeEditMode.None);
                    ClearAllCanvasElement(canvasContex);
                    Repaint();
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    //编辑时点击ctrl+鼠标左键添加点
                    if (Event.current.control)
                    {
                        AddCanvasElementPoint(canvasContex);
                    }
                    //点击第一个点完成编辑
                    else
                    {
                        if (m_data.nodes.Count >= 3 && m_data.nodes[0].TestPoint(canvasContex.m_curLocalPosition))
                        {
                            AddCanvasElementEdge(canvasContex, m_data.nodes[m_data.nodes.Count - 1], m_data.nodes[0]);
                            EndEdit(canvasContex);
                            Repaint();
                        }
                    }

                }
            }else if (m_editMode == ShapeEditMode.None)
            {
                //shift + a 开启编辑
                if (Event.current.shift && Event.current.keyCode == KeyCode.A && Event.current.type == EventType.KeyDown)
                {
                    if (m_editMode == ShapeEditMode.None)
                    {
                        BeginEdit(canvasContex);
                        Repaint();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region 内部方法

        private void AddCanvasElementPoint(CanvasContex canvasContex)
        {
            var elementPoint = m_data.AddNode(canvasContex.m_curLocalPosition);
            canvasContex.AddElement(elementPoint);

            if (m_editMode == ShapeEditMode.Editing)
            {
                if (m_data.nodes.Count >= 2)
                {
                    AddCanvasElementEdge(canvasContex, m_data.nodes[m_data.nodes.Count - 1], m_data.nodes[m_data.nodes.Count - 2]);
                }
            }
        }

        private void AddCanvasElementEdge(CanvasContex canvasContex, CanvasElementPoint p1, CanvasElementPoint p2)
        {
            var elementEdge = this.m_data.AddEdge(p1, p2);
            canvasContex.AddElement(elementEdge);
        }

        private void ClearAllCanvasElement(CanvasContex canvasContex)
        {
            m_data.Clear();
            canvasContex.ClearAllElement();
        }

        private void BeginEdit(CanvasContex canvasContex)
        {
            ClearAllCanvasElement(canvasContex);
            ChangeEditMode(ShapeEditMode.Editing);
        }

        private void EndEdit(CanvasContex canvasContex)
        {
            ChangeEditMode(ShapeEditMode.None);
        }

        private void ChangeEditMode(ShapeEditMode mode)
        {
            m_editMode = mode;
            UnityEngine.Debug.Log("change editMode:" + mode);
        }


        #endregion

        #region 内部变量和属性

        public PolygonEditorData m_data = new PolygonEditorData();


        protected ShapeEditMode m_editMode = ShapeEditMode.None;

        #endregion
    }
}