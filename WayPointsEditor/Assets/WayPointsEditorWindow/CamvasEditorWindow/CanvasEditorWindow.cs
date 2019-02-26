using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean
{
    public class CanvasEditorWindow : EditorWindow
    {
        [MenuItem("Windows/CanvasEditorWindow")]
        public static void OpenWindow()
        {
            var win = GetWindow<CanvasEditorWindow>(typeof(CanvasEditorWindow).Name);
            win.wantsMouseMove = true;
        }

        #region 窗口绘制

        protected virtual void OnDrawMainTool()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            GUILayout.Label("xx");
            GUILayout.EndHorizontal();
        }

        private void DrawMainTool()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            OnDrawMainTool();
            GUILayout.EndHorizontal();
        }

        #endregion

        protected virtual void OnDrawCanvas(SubAreaDef subAreaDef, Rect rect)
        {

        }

        private void DrawCanvas(SubAreaDef subAreaDef, Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var ctx = GetCanvasContex(subAreaDef.m_id);
                GL.LoadIdentity();
                GL.MultMatrix(ctx.GetTRSMatrix(rect.size));
                GL.PushMatrix();
                GLHelper.DrawGrid(50, new Rect(-100, -100, 200, 200));
                OnDrawCanvas(subAreaDef, rect);
                if (ctx.m_isDrawSelection)
                {
                    if (ctx.m_dragStartPosition != m_curLocalPosition)
                    {
                        GLHelper.DrawRect(ctx.m_dragStartPosition, m_curLocalPosition, new Color(0, 0, 0, 0.5f));
                    }
                }
                GL.PopMatrix();
            }
        }

        private void DrawDebugInfos()
        {
            GUILayout.Label("ScreenWidth:" + Screen.width);
            GUILayout.Label("ScreenHeight:" + Screen.height);
            GUILayout.Label("MousePosition:" + m_curMousePosition);
            GUILayout.Label("NormalizedMousePosition:" + m_curNormalizePosition);
            GUILayout.Label("CurSubArea:" + (m_curSubArea == null ? "null" : m_curSubArea.m_showName));
            GUILayout.Label("NormalizePositionInSubArea:" +  m_curNormalizePositionInSubArea);
            GUILayout.Label("LocalPosition:" + m_curLocalPosition);
        }

        protected virtual void OnDrawSubArea(string name)
        {
            GUILayout.Label(name);
            if(name == "DebugInfos")
            {
                DrawDebugInfos();
            }
        }

        protected virtual void DrawSubArea(SubAreaDef subAreaDef, Rect rect)
        {
            GUILayout.BeginArea(rect);
            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle("flow background").Draw(new Rect(Vector2.zero, rect.size), false, false, false, false);
                //绘制背景颜色
                GL.PushMatrix();
                GLHelper.DrawRect(new Vector2(0, 0), rect.size, subAreaDef.m_bgColor);
                GL.PopMatrix();  
            }
            if (subAreaDef.m_isCanvas)
            {
                DrawCanvas(subAreaDef, rect);
            }
            else
            {
                OnDrawSubArea(subAreaDef.m_showName);
            }
            GUILayout.EndArea();
        }

        void OnGUI()
        {
            OnEvent();
            DrawMainTool();
            foreach (var subArea in layoutDef)
            {
                DrawSubArea(subArea, GetRealRect(subArea));
            }
        }

        private Rect GetRealRect(SubAreaDef subArea)
        {
            var pos = new Vector2(subArea.m_position.x * width, (height - toolBarHeight) * subArea.m_position.y + toolBarHeight);
            var size = new Vector2(subArea.m_size.x * width, (height - toolBarHeight) * subArea.m_size.y);
            var rect = new Rect(pos, size);
            return rect;
        }

        private SubAreaDef GetLocateSubArea(Vector2 normalizedPos)
        {
            foreach(var subArea in layoutDef)
            {
                var rect = new Rect(subArea.m_position, subArea.m_size);
                if (rect.Contains(normalizedPos))
                {
                    return subArea;
                }
            }
            return null;
        }

        private void ProcessEventInCanvas(SubAreaDef subAreaDef)
        {
            var canvasCtx = GetCanvasContex(subAreaDef.m_id);
            //鼠标中键拖动视图
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                canvasCtx.m_offset += Event.current.delta;
                Repaint();
            }
            //鼠标滚轮缩放视图
            if (Event.current.type == EventType.ScrollWheel)
            {
                canvasCtx.m_scale += Event.current.delta.y * -0.01f;
                Repaint();
            }
            //按下鼠标左键，开始绘制选区
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (canvasCtx.m_curSelected == null)
                {
                    canvasCtx.m_isDrawSelection = true;
                    canvasCtx.m_dragStartPosition = m_curLocalPosition;
                    Repaint();
                }
            }
            //松开鼠标左键
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                //结束绘制选区，根据画出的选取选择对象
                if (canvasCtx.m_isDrawSelection == true)
                {
                    canvasCtx.m_isDrawSelection = false;
                    Repaint();
                    /*
                    var ele = m_data.IntersectTest(m_dragStartPosition, m_curLocalPosition);
                    this.m_curSelected = ele;
                    shouldRepaint = true;
                    */
                }
            }
        }

        private void OnEvent()
        {
            var lastMousePosition = m_curMousePosition;
            m_curMousePosition = Event.current.mousePosition;
            m_curNormalizePosition = new Vector2(m_curMousePosition.x / width, (m_curMousePosition.y - toolBarHeight) / (height - toolBarHeight));
            if(new Rect(0, 0, 1, 1).Contains(m_curNormalizePosition))
            {
                m_curSubArea = GetLocateSubArea(m_curNormalizePosition);
                var relativePos = m_curNormalizePosition - m_curSubArea.m_position;
                m_curNormalizePositionInSubArea = new Vector2(relativePos.x / m_curSubArea.m_size.x, relativePos.y / m_curSubArea.m_size.y);
                if (m_curSubArea.m_isCanvas)
                {
                    ProcessEventInCanvas(m_curSubArea);
                    var canvasCtx = GetCanvasContex(m_curSubArea.m_id);
                    m_curLocalPosition = canvasCtx.GetLocalPos(m_curNormalizePositionInSubArea, GetRealRect(m_curSubArea).size);
                }
                
            }
            if (lastMousePosition != m_curMousePosition)
                Repaint();
        }

        protected CanvasContex GetCanvasContex(int id)
        {
            if (!m_canvasContex.ContainsKey(id))
            {
                m_canvasContex.Add(id, new CanvasContex());
            }
            return m_canvasContex[id];
        }

        public Vector2 m_curMousePosition = Vector2.zero;

        public Vector2 m_curNormalizePosition = Vector2.zero;

        public SubAreaDef m_curSubArea;

        public Vector2 m_curNormalizePositionInSubArea = Vector2.zero;

        public Vector2 m_curLocalPosition = Vector2.zero;

       // public Vector2 m_curLocalPosition = Vector2.zero;

        protected float width { get { return Screen.width; } }
        protected float height { get { return Screen.height; } }
        protected float toolBarHeight { get { return EditorStyles.toolbar.fixedHeight; } }

        protected virtual List<SubAreaDef> layoutDef
        {
            get { return m_layoutDef; }
        }

        private List<SubAreaDef> m_layoutDef = new List<SubAreaDef> {
            new SubAreaDef() { m_id = 0, m_showName = "OperateArea", m_bgColor = new Color(1f, 0, 0, 0.2f), m_position = new Vector2(0, 0),       m_size = new Vector2(0.2f, 1f),   m_isCanvas = false },
            new SubAreaDef() { m_id = 1, m_showName = "Canvas",      m_bgColor = new Color(0, 1f, 1f, 0.2f), m_position = new Vector2(0.2f, 0),    m_size = new Vector2(0.6f, 1f),   m_isCanvas = true  },
            new SubAreaDef() { m_id = 2, m_showName = "Tips",        m_bgColor = new Color(0, 0, 1f, 0.2f), m_position = new Vector2(0.8f, 0),    m_size = new Vector2(0.2f, 0.5f), m_isCanvas = false },
            new SubAreaDef() { m_id = 3, m_showName = "DebugInfos",  m_bgColor = new Color(0, 1f, 0, 0.2f), m_position = new Vector2(0.8f, 0.5f), m_size = new Vector2(0.2f, 0.5f), m_isCanvas = false },
        };

        private Dictionary<int, CanvasContex> m_canvasContex = new Dictionary<int, CanvasContex>();
    }
}

