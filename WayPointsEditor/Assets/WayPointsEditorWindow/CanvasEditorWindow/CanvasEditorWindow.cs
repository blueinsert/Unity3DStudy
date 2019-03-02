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
            win.InitSubAreaContext();
        }
        
        public void InitSubAreaContext()
        {
            if (m_isInited)
                return;
            foreach(var subAreaDef in layoutDef)
            {
                if (subAreaDef.m_isCanvas)
                {
                    CanvasContext canvasContext = new CanvasContext(subAreaDef);
                    if(m_defaultCanvasCtx == null)
                    {
                        m_defaultCanvasCtx = canvasContext;
                    }
                    m_subAreaContexts.Add(subAreaDef.m_id, canvasContext);
                }else
                {
                    m_subAreaContexts.Add(subAreaDef.m_id, new SubAreaContext(subAreaDef));
                }
            }
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
                    GUILayout.Label("LocalPosition:" + canvasCtx.m_curLocalPosition);
                    GUILayout.Label("Offset:" + canvasCtx.m_offset);
                    GUILayout.Label("Scale:" + canvasCtx.m_scale);
                }
            }    
        }

        private void DrawCanvas(CanvasContext canvasCtx)
        {
            var size = canvasCtx.GetRealSize(width, height - toolBarHeight);
            if (Event.current.type == EventType.Repaint)
            {
                GL.PushMatrix();
                CreateLineMaterial();
                lineMaterial.SetPass(0);
                GL.LoadIdentity();
                GL.MultMatrix(canvasCtx.GetTRSMatrix(size));
                var p1 = canvasCtx.GetLocalPos(new Vector2(0, 1), size);
                var p2 = canvasCtx.GetLocalPos(new Vector2(1, 0), size);
                GLHelper.DrawGrid(100, new Rect(p1, p2 - p1));
                OnDrawCanvas(canvasCtx);
                if (canvasCtx.m_isDrawSelection)
                {
                    if (canvasCtx.m_dragStartPosition != canvasCtx.m_curLocalPosition)
                    {
                        GLHelper.DrawRectSolid(canvasCtx.m_dragStartPosition, canvasCtx.m_curLocalPosition, new Color(0, 0, 0, 0.5f));
                    }
                }
                GL.PopMatrix();
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
            var rect = subAreaCtx.GetRealRect(width, height - toolBarHeight);
            rect.position += new Vector2(0, toolBarHeight);
            GUILayout.BeginArea(rect);
            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle("flow background").Draw(new Rect(Vector2.zero, rect.size), false, false, false, false);
                //绘制背景颜色
                GL.PushMatrix();
                CreateLineMaterial();
                lineMaterial.SetPass(0);
                GLHelper.DrawRectSolid(new Vector2(0, 0), rect.size, subAreaCtx.m_areaDef.m_bgColor);
                GL.PopMatrix();  
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
            DrawMainTool();
            foreach (var subAreaCtx in m_subAreaContexts.Values)
            {
                DrawSubArea(subAreaCtx);
            }
        }
        #endregion

        #region 事件处理

        private SubAreaContext GetLocateSubArea(Vector2 normalizedPos)
        {
            foreach (var subArea in layoutDef)
            {
                var rect = new Rect(subArea.m_position, subArea.m_size);
                if (rect.Contains(normalizedPos))
                {
                    return m_subAreaContexts[subArea.m_id];
                }
            }
            return null;
        }

        protected virtual void OnLeaveSubArea(SubAreaContext subAreaCtx)
        {
            if (subAreaCtx == null)
                return;
            UnityEngine.Debug.Log("Leave SubArea:" + subAreaCtx.m_areaDef.m_showName);
        }

        protected virtual void OnEnterSubArea(SubAreaContext subAreaCtx)
        {
            UnityEngine.Debug.Log("Leave SubArea:" + subAreaCtx.m_areaDef.m_showName);
        }

        private void ProcessEventInCanvas(CanvasContext canvasCtx)
        {
            canvasCtx.m_curLocalPosition = canvasCtx.GetLocalPos(canvasCtx.m_curMouseNormalizePosition, canvasCtx.GetRealSize(width, height - toolBarHeight));
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
                if (canvasCtx.m_selectedElements.Count == 0)
                {
                    canvasCtx.m_isDrawSelection = true;
                    canvasCtx.m_dragStartPosition = canvasCtx.m_curLocalPosition;
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
                }
            }
        }

        private void ProcessEventInSubArea(SubAreaContext subAreaCtx)
        {

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

        //划线使用的材质球
        static Material lineMaterial;
        /// <summary>
        /// 创建一个材质球
        /// </summary>
        static void CreateLineMaterial()
        {
            //如果材质球不存在
            if (!lineMaterial)
            {
                //用代码的方式实例一个材质球
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                //设置参数
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                //设置参数
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                //设置参数
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        protected SubAreaContext GetSubAreaContext(int id)
        { 
            return m_subAreaContexts[id];
        }

        public Vector2 m_curMousePosition = Vector2.zero;

        public Vector2 m_curNormalizePosition = Vector2.zero;

        public SubAreaContext m_curSubAreaCtx;

        public CanvasContext m_defaultCanvasCtx;

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

        private bool m_isInited = false;
        private Dictionary<int, SubAreaContext> m_subAreaContexts = new Dictionary<int, SubAreaContext>();
    }
}

