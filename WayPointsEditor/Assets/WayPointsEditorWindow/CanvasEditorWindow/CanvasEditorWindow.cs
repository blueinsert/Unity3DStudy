using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean
{
    public class CanvasEditorWindow : EditorWindow
    {
        [MenuItem("Windows/OpenCanvasEditorWindow")]
        public static void OpenCanvasEditorWindow()
        {
            var win = GetWindow<CanvasEditorWindow>(typeof(CanvasEditorWindow).Name);
            win.wantsMouseMove = true;
            win.Initialize();
        }

        protected virtual void Initialize()
        {
            if (isInited)
                return;
            foreach (var subArea in layoutDef)
            {
                if (subArea.m_isCanvas)
                {
                    m_subAreaContexDic.Add(subArea.m_id, new CanvasContex(subArea));
                }
                else
                {
                    m_subAreaContexDic.Add(subArea.m_id, new SubAreaContex(subArea));
                }
            }
        }

        #region 窗口绘制

        #region virtual

        protected virtual void OnDrawMainTool()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            GUILayout.Label("xx");
            GUILayout.EndHorizontal();
        }

        protected virtual void OnDrawCanvas(CanvasContex canvasContex)
        {

        }

        protected virtual void OnDrawSubArea(SubAreaContex areaCtx)
        {
            GUILayout.Label(areaCtx.m_areaDef.m_showName);
            if (name == "DebugInfos")
            {
                DrawDebugInfos();
            }
        }

        #endregion

        #region 内部功能

        private void DrawMainTool()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            OnDrawMainTool();
            GUILayout.EndHorizontal();
        }

        private void DrawCanvas(CanvasContex canvasContex)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GL.LoadIdentity();
                GL.MultMatrix(canvasContex.GetTRSMatrix(canvasContex.GetRealRect(width, height, toolBarHeight).size));
                GL.PushMatrix();
                var p1 = canvasContex.GetLocalPos(new Vector2(0, 1),
                    canvasContex.GetRealRect(width, height, toolBarHeight).size);
                var p2 = canvasContex.GetLocalPos(new Vector2(1, 0),
                    canvasContex.GetRealRect(width, height, toolBarHeight).size);
                GLHelper.DrawGrid(100, new Rect(p1, p2 - p1));
                //GLHelper.DrawGrid(100, new Rect(-100,-100, 500,500));
                OnDrawCanvas(canvasContex);
                if (canvasContex.m_isDrawSelection)
                {
                    if (canvasContex.m_dragStartPosition != canvasContex.m_curLocalPosition)
                    {
                        GLHelper.DrawRect(canvasContex.m_dragStartPosition, canvasContex.m_curLocalPosition, new Color(0, 0, 0, 0.5f));
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
            GUILayout.Label("CurSubArea:" + (m_curSubAreaCtx == null ? "null" : m_curSubAreaCtx.m_areaDef.m_showName));
            GUILayout.Label("NormalizePositionInSubArea:" + (m_curSubAreaCtx == null ? "null" : m_curSubAreaCtx.m_curNormalizeMousePosition.ToString()));
            if (m_curSubAreaCtx != null && m_curSubAreaCtx is CanvasContex)
            {
                var canvasContex = m_curSubAreaCtx as CanvasContex;
                GUILayout.Label("LocalPosition:" + canvasContex.m_curLocalPosition);
            }
        }

        private void DrawSubArea(SubAreaContex areaCtx)
        {
            var rect = areaCtx.GetRealRect(width, height, toolBarHeight);
            GUILayout.BeginArea(rect);
            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle("flow background").Draw(new Rect(Vector2.zero, rect.size), false, false, false, false);
                //绘制背景颜色
                GL.PushMatrix();
                GLHelper.DrawRect(new Vector2(0, 0), rect.size, areaCtx.m_areaDef.m_bgColor);
                GL.PopMatrix();
            }
            if (areaCtx.m_areaDef.m_isCanvas)
            {
                DrawCanvas(areaCtx as CanvasContex);
            }
            else
            {
                OnDrawSubArea(areaCtx);
            }
            GUILayout.EndArea();
        }

        #endregion


        void OnGUI()
        {
            OnEvent();
            DrawMainTool();
            foreach (var subAreaCtx in m_subAreaContexDic.Values)
            {
                DrawSubArea(subAreaCtx);
            }
        }

        #endregion

        #region 事件处理

        protected virtual void OnLeaveSubArea(SubAreaContex ctx)
        {
            if (ctx is CanvasContex)
            {
                var canvasContex = ctx as CanvasContex;
                if (canvasContex.m_isDrawSelection == true)
                {
                    canvasContex.m_isDrawSelection = false;
                    Repaint();
                }
            }   
        }

        protected virtual void OnEnterSubArea(SubAreaContex ctx)
        {
            
        }

        protected virtual void ProcessEventInCanvas(CanvasContex canvasContex)
        {
            canvasContex.m_curLocalPosition = canvasContex.GetLocalPos(canvasContex.m_curNormalizeMousePosition, canvasContex.GetRealRect(width, height, toolBarHeight).size);
            //鼠标中键拖动视图
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                canvasContex.m_offset += Event.current.delta;
                Repaint();
            }
            //鼠标滚轮缩放视图
            if (Event.current.type == EventType.ScrollWheel)
            {
                canvasContex.m_scale += Event.current.delta.y * -0.01f;
                Repaint();
            }
            //按下鼠标左键
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                canvasContex.m_dragStartPosition = canvasContex.m_curLocalPosition;
                var intersectElem = canvasContex.Overlap(canvasContex.m_curLocalPosition);
                if (intersectElem == null)
                {
                    canvasContex.ClearAllSelected();
                }
                if (intersectElem == null)
                {
                    //开始绘制选区
                    canvasContex.m_isDrawSelection = true;
                    canvasContex.ClearAllSelected();
                    Repaint();
                }
                else
                {
                    //开始拖动
                    if (!canvasContex.m_selectedElements.Contains(intersectElem))
                    {
                        canvasContex.ClearAllSelected();
                        canvasContex.AddSelected(intersectElem);
                    }
                    canvasContex.m_isDragSelections = true;
                    foreach (var ele in canvasContex.m_selectedElements)
                    {
                        if (ele.canDrag)
                        {
                            ele.m_dragStartPos = ele.position;
                        }
                    }
                }
            }
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                if (canvasContex.m_isDragSelections)
                {
                    foreach (var ele in canvasContex.m_selectedElements)
                    {
                        if (ele.canDrag)
                        {
                            ele.position = ele.m_dragStartPos + (canvasContex.m_curLocalPosition - canvasContex.m_dragStartPosition);
                        }
                    }
                }
            }
            //松开鼠标左键
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                //结束绘制选区，根据画出的选取选择对象
                if (canvasContex.m_isDrawSelection == true)
                {
                    canvasContex.m_isDrawSelection = false;
                    var overlaps = canvasContex.Overlap(canvasContex.m_dragStartPosition, canvasContex.m_curLocalPosition);
                    canvasContex.AddSelected(overlaps);
                    Repaint();  
                }
                //结束拖动
                if (canvasContex.m_isDragSelections)
                {
                    canvasContex.m_isDragSelections = false;
                }
            }
        }

        protected virtual void ProcessEventInSubArea(SubAreaContex ctx)
        {
            var relativePos = m_curNormalizePosition - ctx.m_areaDef.m_position;
            ctx.m_curNormalizeMousePosition = new Vector2(relativePos.x / ctx.m_areaDef.m_size.x, relativePos.y / ctx.m_areaDef.m_size.y);
            if (ctx is CanvasContex)
            {
                ProcessEventInCanvas(ctx as CanvasContex);
            }
        }

        private void OnEvent()
        {
            var lastMousePosition = m_curMousePosition;
            m_curMousePosition = Event.current.mousePosition;
            m_curNormalizePosition = new Vector2(m_curMousePosition.x / width, (m_curMousePosition.y - toolBarHeight) / (height - toolBarHeight));
            if (new Rect(0, 0, 1, 1).Contains(m_curNormalizePosition))
            {
                var lastSubAreaCtx = m_curSubAreaCtx;
                m_curSubAreaCtx = GetLocateSubArea(m_curNormalizePosition);
                if (m_curSubAreaCtx != lastSubAreaCtx)
                {
                    OnLeaveSubArea(lastSubAreaCtx);
                    OnEnterSubArea(m_curSubAreaCtx);
                }
                ProcessEventInSubArea(m_curSubAreaCtx); 
            }
            if (lastMousePosition != m_curMousePosition)
                Repaint();
        }

        #endregion

        private SubAreaContex GetLocateSubArea(Vector2 normalizedPos)
        {
            foreach (var subAreaCtx in m_subAreaContexDic.Values)
            {
                var rect = new Rect(subAreaCtx.m_areaDef.m_position, subAreaCtx.m_areaDef.m_size);
                if (rect.Contains(normalizedPos))
                {
                    return subAreaCtx;
                }
            }
            return null;
        }

        public Vector2 m_curMousePosition = Vector2.zero;

        public Vector2 m_curNormalizePosition = Vector2.zero;

        public SubAreaContex m_curSubAreaCtx;
      
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

        protected bool isInited = false;
        private Dictionary<int, SubAreaContex> m_subAreaContexDic = new Dictionary<int, SubAreaContex>();
    }
}

