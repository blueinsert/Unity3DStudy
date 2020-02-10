using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean
{
    public class CanvasEditorWindow : EditorWindow,IWindow
    {
        public int Width { get { return Screen.width; } }
        public int Height { get { return Screen.height - (int)ToolBarHeight; } }

        protected float ToolBarHeight { get { return EditorStyles.toolbar.fixedHeight; } }

        protected int m_frameCount = 0;

        private Vector2 m_curMousePosition = Vector2.zero;

        private Vector2 m_curNormalizePosition = Vector2.zero;

        private SubAreaContext m_curSubAreaCtx;

        private CanvasContext m_defaultCanvasCtx;

        protected virtual List<SubAreaDef> LayoutDef
        {
            get { return m_layoutDef; }
        }

        private List<SubAreaDef> m_layoutDef = new List<SubAreaDef> {
            new SubAreaDef() { m_id = 0, m_renderOrder = 1, m_showName = "OperateArea", m_bgColor = new Color(1f, 0, 0, 0.2f), m_position = new Vector2(0, 0),        m_size = new Vector2(0.2f, 1f),   m_isCanvas = false },
            new SubAreaDef() { m_id = 1, m_renderOrder = 0, m_showName = "Canvas",      m_bgColor = new Color(0, 1f, 1f, 0.2f), m_position = new Vector2(0.2f, 0),    m_size = new Vector2(0.6f, 1f),   m_isCanvas = true  },
            new SubAreaDef() { m_id = 2, m_renderOrder = 2, m_showName = "Tips",        m_bgColor = new Color(0, 0, 1f, 0.2f), m_position = new Vector2(0.8f, 0),     m_size = new Vector2(0.2f, 0.5f), m_isCanvas = false },
            new SubAreaDef() { m_id = 3, m_renderOrder = 3, m_showName = "DebugInfos",  m_bgColor = new Color(0, 1f, 0, 0.2f), m_position = new Vector2(0.8f, 0.5f),  m_size = new Vector2(0.2f, 0.5f), m_isCanvas = false },
        };

        private bool m_isInited = false;
        private List<SubAreaContext> m_subAreaContexts = new List<SubAreaContext>();

        //[MenuItem("Windows/OpenCanvasEditorWindow")]
        public static void OpenCanvasEditorWindow()
        {
            var win = GetWindow<CanvasEditorWindow>(typeof(CanvasEditorWindow).Name);
            win.wantsMouseMove = true;
            win.InitSubAreaContext();
        }
        
        public void InitSubAreaContext()
        {
            if (m_isInited)
                return;
            foreach(var subAreaDef in LayoutDef)
            {
                if (subAreaDef.m_isCanvas)
                {
                    CanvasContext canvasContext = new CanvasContext(this, subAreaDef);
                    if(m_defaultCanvasCtx == null)
                    {
                        m_defaultCanvasCtx = canvasContext;
                    }
                    m_subAreaContexts.Add(canvasContext);
                }else
                {
                    m_subAreaContexts.Add(new SubAreaContext(this, subAreaDef));
                }
            }
            m_subAreaContexts.Sort((a, b) => { return a.m_areaDef.m_renderOrder - b.m_areaDef.m_renderOrder; });
            m_isInited = true;
        }

        #region 窗口绘制

        protected virtual void OnDrawMainTool()
        {
           
        }

        private void DrawMainTool()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            OnDrawMainTool();
            GUILayout.EndHorizontal();
        }

        protected virtual void OnDrawCanvas(CanvasContext canvasCtx)
        {

        }

        private void DrawDebugInfos()
        {
            GUILayout.Label("ScreenWidth:" + Screen.width);
            GUILayout.Label("ScreenHeight:" + Screen.height);
            GUILayout.Label("MousePosition:" + m_curMousePosition);
            GUILayout.Label("NormalizedMousePosition:" + m_curNormalizePosition);
            if(m_curSubAreaCtx != null)
            {
                GUILayout.Label("CurSubArea:" + m_curSubAreaCtx.m_areaDef.m_showName);
                GUILayout.Label("NormalizePositionInSubArea:" + m_curSubAreaCtx.m_curMouseNormalizePosition);
                if(m_curSubAreaCtx is CanvasContext)
                {
                    var canvasCtx = m_curSubAreaCtx as CanvasContext;
                    GUILayout.Label("LocalPosition:" + canvasCtx.MousePosition);
                    GUILayout.Label("Offset:" + canvasCtx.m_offset);
                    GUILayout.Label("Scale:" + canvasCtx.m_scale);
                }
            }    
        }

        private void DrawCanvas(CanvasContext ctx)
        {
            if (Event.current.type == EventType.Repaint)
            {
                ctx.Color = new Color(0.5f, 0.5f, 0.5f);
                ctx.DrawGrid();
                OnDrawCanvas(ctx);
            }
        }

        protected virtual void OnDrawSubArea(SubAreaContext subAreaCtx)
        {
            GUILayout.Label(subAreaCtx.m_areaDef.m_showName);
            if(subAreaCtx.m_areaDef.m_showName == "DebugInfos")
            {
                DrawDebugInfos();
            }
        }

        protected virtual void DrawSubArea(SubAreaContext subAreaCtx)
        {
            var rect = subAreaCtx.Rect;
            rect.position += new Vector2(0, ToolBarHeight);
            GUILayout.BeginArea(rect, new GUIStyle("flow background"));
            if (Event.current.type == EventType.Repaint)
            {
                //new GUIStyle("flow background").Draw(new Rect(Vector2.zero, rect.size), false, false, false, false);
            }
            if (subAreaCtx.m_areaDef.m_isCanvas)
            {
                DrawCanvas(subAreaCtx as CanvasContext);
            }
            else
            {
                OnDrawSubArea(subAreaCtx);
            }
            GUILayout.EndArea();
        }

        void OnGUI()
        {
            OnEvent();
            foreach (var subAreaCtx in m_subAreaContexts)
            {
                DrawSubArea(subAreaCtx);
            }
            DrawMainTool();  
        }
        #endregion

        #region 事件处理

        private SubAreaContext GetLocateSubArea(Vector2 normalizedPos)
        {
            foreach (var subAreaCtx in m_subAreaContexts)
            {
                var subArea = subAreaCtx.m_areaDef;
                var rect = new Rect(subArea.m_position, subArea.m_size);
                if (rect.Contains(normalizedPos))
                {
                    return subAreaCtx;
                }
            }
            return null;
        }

        protected virtual void OnLeaveSubArea(SubAreaContext subAreaCtx)
        {
            if (subAreaCtx == null)
                return;
            //UnityEngine.Debug.Log("Leave SubArea:" + subAreaCtx.m_areaDef.m_showName);
        }

        protected virtual void OnEnterSubArea(SubAreaContext subAreaCtx)
        {
            //UnityEngine.Debug.Log("Leave SubArea:" + subAreaCtx.m_areaDef.m_showName);
        }

        protected virtual void ProcessEventInCanvas(CanvasContext ctx)
        {
            Vector2 posInCanvas = new Vector2(ctx.Width * ctx.m_curMouseNormalizePosition.x, ctx.Height * ctx.m_curMouseNormalizePosition.y);
            ctx.MousePosition = ctx.CanvasToLogicPos(posInCanvas);
            //鼠标中键拖动视图
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                ctx.m_offset += Event.current.delta;
                Repaint();
            }
            //鼠标滚轮缩放视图
            if (Event.current.type == EventType.ScrollWheel)
            {
                ctx.Scale += Event.current.delta.y * -0.03f;
                Repaint();
            }
        }

        protected virtual void ProcessEventInSubArea(SubAreaContext subAreaCtx)
        {

        }

        private void OnEvent()
        {
            var lastMousePosition = m_curMousePosition;
            m_curMousePosition = Event.current.mousePosition;
            m_curNormalizePosition = new Vector2(m_curMousePosition.x / Width, (m_curMousePosition.y - ToolBarHeight) / (Height - ToolBarHeight));
            if (new Rect(0, 0, 1, 1).Contains(m_curNormalizePosition))
            {
                var lastSubAreaCtx = m_curSubAreaCtx;
                m_curSubAreaCtx = GetLocateSubArea(m_curNormalizePosition);
                if (m_curSubAreaCtx != lastSubAreaCtx)
                {
                    OnLeaveSubArea(lastSubAreaCtx);
                    OnEnterSubArea(m_curSubAreaCtx);
                }
                var relativePos = m_curNormalizePosition - m_curSubAreaCtx.m_areaDef.m_position;
                m_curSubAreaCtx.m_curMouseNormalizePosition = new Vector2(relativePos.x / m_curSubAreaCtx.m_areaDef.m_size.x, relativePos.y / m_curSubAreaCtx.m_areaDef.m_size.y);
                if (m_curSubAreaCtx is CanvasContext)
                {
                    ProcessEventInCanvas(m_curSubAreaCtx as CanvasContext);             
                }else
                {
                    ProcessEventInSubArea(m_curSubAreaCtx);
                }
            }
            if (lastMousePosition != m_curMousePosition)
                Repaint();
        }
        #endregion

        private void Update()
        {
            m_frameCount++;
        }

        
    }
}

