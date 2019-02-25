using UnityEditor;
using UnityEngine;

namespace bluebean
{
    public enum ShapeEditMode
    {
        None,
        Editing,
    }

    public class PolygonEditor : EditorWindow
    {
        [MenuItem("Windows/PolygonEditor")]
        public static void onInit()
        {
            var win = Instance;
        }

        private static PolygonEditor m_instance;

        public static PolygonEditor Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = GetWindow<PolygonEditor>();
                    m_instance.wantsMouseMove = true;
                }
                return m_instance;
            }
        }

        #region 窗口绘制
       
        private void OnDrawMainTool()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            GUILayout.Label("xx");
            GUILayout.EndHorizontal();
            GUILayout.Label("YY");
            GUILayout.EndHorizontal();
        }

        private void OnDrawOperateArea()
        {
            GUILayout.BeginArea(operateAreaRect);
            GUILayout.Button("Edit");
            GUILayout.EndArea();
        }

        private void DrawCanvas()
        {
            ///*
            GL.LoadIdentity();
            GL.MultMatrix(canvasTRS);
            GL.PushMatrix();
            //*/
            //GL.PushMatrix();
            OnDrawCanvas();
            GL.PopMatrix();
        }

        private void OnDrawCanvasBegin()
        {
            GUILayout.BeginArea(canvasAreaRect);
            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle("flow background").Draw(new Rect(0, 0, canvasSize.x, canvasSize.y), false, false, false, false);
                DrawCanvas();
            }
        }

        private void OnDrawCanvasEnd()
        {
            GUILayout.EndArea();
        }

        private void OnDrawDebugInfos()
        {
            GUILayout.BeginArea(debugAreaRect);
            GUILayout.Label("Debug Infos");
            GUILayout.Label("MousePosition:" + m_curMousePosition);
            GUILayout.Label("LocalPosition:" + m_curLocalPosition);
            GUILayout.Label("ScreenWidth:" + Screen.width);
            GUILayout.Label("ScreenHeight:" + Screen.height);
            GUILayout.Label("CanvasOffset:" + string.Format("[{0},{1}]", m_canvasOffset.x, m_canvasOffset.y));
            GUILayout.Label("CanvasScale:" + m_canvasScale);
            GUILayout.EndArea();
        }

        private void OnDrawTipsArea()
        {
            GUILayout.BeginArea(tipsAreaRect);
            GUILayout.Label("TIPS:");
            GUILayout.Label("1 Shift + A 进入编辑模式");
            GUILayout.Label("  1.1 Ctrl + LMB 添加点");
            GUILayout.Label("  1.2 创建至少三个点后，LMB点击第一个点完成编辑");
            GUILayout.Label("  1.3 RMB放弃编辑");
            GUILayout.EndArea();
        }

        void OnGUI()
        {
            OnEvent();
            OnDrawMainTool();
            OnDrawOperateArea();
            OnDrawCanvasBegin();
            OnDrawCanvasEnd();
            OnDrawDebugInfos();
            OnDrawTipsArea();
        }

        #endregion

        #region 画布元素管理

        protected void AddCanvasElementPoint(Vector2 pos)
        {
            m_data.AddNode(pos);
            if (m_editMode == ShapeEditMode.Editing)
            {
                if (m_data.nodes.Count >= 2)
                {
                    this.m_data.AddEdge(m_data.nodes[m_data.nodes.Count - 1], m_data.nodes[m_data.nodes.Count - 2]);
                }
            }
        }

        protected void ClearCanvasElement()
        {
            m_data.Clear();
        }

        #endregion

        #region 坐标转换

        protected Vector2 GlobalPositionToLocal(Vector2 globalPos)
        {
            var posInCanvas = globalPos - canvasAreaRect.position;
            var localPos = CanvasPositionToLocal(posInCanvas);
            return localPos;
        }

        protected Vector2 CanvasPositionToLocal(Vector2 canvasPos)
        {
            var res = canvasTRS.inverse * new Vector4(canvasPos.x, canvasPos.y, 0, 1);
            return new Vector2(res.x, res.y);
        }

        protected Vector2 LocalPositionToCanvas(Vector2 pos)
        {
            var res = canvasTRS * new Vector4(pos.x, pos.y, 0, 1);
            return new Vector2(res.x, res.y);
        }

        protected Rect CanvasRectToLocal(Rect rect)
        {
            var pos = CanvasPositionToLocal(rect.position);
            var size = rect.size * m_canvasScale;
            return new Rect(pos, size);
        }

        #endregion

        #region 画布绘制

        private void DrawCanvasElementPoint(CanvasElementPoint point, Color color)
        {
            GLHelper.DrawCircle(point.position, point.radius, 36, color);
        }

        private void DrawCanvasElementEdge(CanvasElementEdge edge)
        {
            GLHelper.DrawLine(edge.point1.position, edge.point2.position, Color.blue);
        }

        private void DrawCanvasElement(CanvasElementBase canvasElement)
        {
            if (canvasElement is CanvasElementPoint)
            {
                if (canvasElement == m_curSelected)
                {
                    DrawCanvasElementPoint(canvasElement as CanvasElementPoint, PolygonEditorSettings.CanvasElementSelectedColor);
                }
                else
                {
                    DrawCanvasElementPoint(canvasElement as CanvasElementPoint, PolygonEditorSettings.CanvasElementUnSelectedColor);
                }
            }else if (canvasElement is CanvasElementEdge)
            {
                DrawCanvasElementEdge(canvasElement as CanvasElementEdge);
            }
        }

        protected virtual void OnDrawCanvas()
        {
            GLHelper.DrawGrid(50, new Rect(-100, -100, 200, 200));
            //GLHelper.DrawGrid(50, CanvasRectToLocal(new Rect(0, 0, canvasSize.x, canvasSize.y)));
            foreach (var point in m_data.nodes)
            {
                DrawCanvasElement(point);
            }
            foreach (var edge in m_data.edges)
            {
                DrawCanvasElement(edge);
            }
            
            if (m_editMode == ShapeEditMode.Editing)
            {
                var localPos = GlobalPositionToLocal(m_curMousePosition);
                GLHelper.DrawCircle(localPos, PolygonEditorSettings.CanvasElementPointRadius, 36, Color.red);
                if(m_data.nodes.Count > 0)
                    GLHelper.DrawLine(m_data.nodes[m_data.nodes.Count - 1].position, localPos, Color.blue);
            }else if (m_editMode == ShapeEditMode.None)
            {
                if ( m_isDrawSelection)
                {
                    var localPos = GlobalPositionToLocal(m_curMousePosition);
                    if (m_dragStartPosition != localPos)
                    {
                        GLHelper.DrawRect(m_dragStartPosition, localPos, new Color(0,0,0,0.5f));
                    }
                }
            }
            
        }

        #endregion

        #region UI事件处理

        //在画布内发生时需要响应的事件
        private bool OnEventInCanvas()
        {
            bool shouldRepaint = false;
            //鼠标中键拖动视图
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                m_canvasOffset += Event.current.delta;
                shouldRepaint = true;
            }
            //鼠标滚轮缩放视图
            if (Event.current.type == EventType.ScrollWheel)
            {
                canvasScale = canvasScale + Event.current.delta.y * PolygonEditorSettings.MouseWheelZoomRadio;
                shouldRepaint = true;
            }
             
            if (m_editMode == ShapeEditMode.Editing)
            {
                //编辑时点击鼠标右键取消编辑
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    ChangeEditMode(ShapeEditMode.None);
                    m_data.Clear();
                    shouldRepaint = true;
                }
                
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    //编辑时点击ctrl+鼠标左键添加点
                    if (Event.current.control)
                    {
                        AddCanvasElementPoint(m_curLocalPosition);
                    }
                    //点击第一个点完成编辑
                    else
                    {
                        if (m_data.nodes.Count >= 3 && m_data.nodes[0].TestPoint(m_curLocalPosition))
                        {
                            this.m_data.AddEdge(m_data.nodes[m_data.nodes.Count - 1], m_data.nodes[0]);
                            EndEdit();
                            shouldRepaint = true;
                        }
                    }
                    
                }                
            }
            else if (m_editMode == ShapeEditMode.None)
            {
                
                //点击空白区域，取消所选;更新所选
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    var ele = m_data.IntersectTest(m_curLocalPosition);
                    if (ele != m_curSelected)
                        shouldRepaint = true;
                    m_curSelected = ele;
                }
                
                //拖动选择物体
                if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    if (m_curSelected != null && m_curSelected is CanvasElementPoint)
                    {
                        var canvasElementPoint = m_curSelected as CanvasElementPoint;
                        var posInCanvas = m_curMousePosition - canvasAreaRect.position;
                        var localPos = CanvasPositionToLocal(posInCanvas);
                        canvasElementPoint.SetPosition(localPos);
                        shouldRepaint = true;
                    }
                }
                //按下鼠标左键，开始绘制选区
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    if (m_curSelected == null)
                    {
                        m_isDrawSelection = true;
                        m_dragStartPosition = m_curLocalPosition;
                    } 
                }
            }
            return shouldRepaint;
        }

        private void OnEvent()
        {
            bool shouldRepaint = false;
            var lastMousePosition = m_curMousePosition;
            m_curMousePosition = Event.current.mousePosition;
            m_curLocalPosition = GlobalPositionToLocal(m_curMousePosition);

            bool isInCanvas = canvasAreaRect.Contains(m_curMousePosition);
            if (isInCanvas)
            {
                shouldRepaint = OnEventInCanvas();
            }
            if (m_curMousePosition != lastMousePosition)
            {
                shouldRepaint = true;
            }  
            if (m_editMode == ShapeEditMode.None)
            {   //松开鼠标左键
                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    //结束绘制选区，根据画出的选取选择对象
                    if (m_isDrawSelection == true)
                    {
                        m_isDrawSelection = false;
                        var ele = m_data.IntersectTest(m_dragStartPosition, m_curLocalPosition);
                        this.m_curSelected = ele;
                        shouldRepaint = true;
                    }
                }
            }
           
            //shift + a 开启编辑
            if (Event.current.shift && Event.current.keyCode == KeyCode.A && Event.current.type == EventType.KeyDown)
            {
                if (m_editMode == ShapeEditMode.None)
                {
                    BeginEdit();
                    shouldRepaint = true;
                }
            }
            
            if (shouldRepaint)
            {
                Repaint();
            }
        }

        #endregion

        protected void BeginEdit()
        {
            m_data.Clear();
            ChangeEditMode(ShapeEditMode.Editing);
        }

        protected void EndEdit()
        {
            ChangeEditMode(ShapeEditMode.None);
        }

        protected void ChangeEditMode(ShapeEditMode mode)
        {
            m_editMode = mode;
            UnityEngine.Debug.Log("change editMode:" + mode);
        }

        void Update()
        {
            //UnityEngine.Debug.Log("Update");
            /*
            if (m_editMode == ShapeEditMode.Editing)
            {
                if (canvasAreaRect.Contains(Event.current.mousePosition))
                {
                    var posInCanvas = Event.current.mousePosition - canvasAreaRect.position;
                    var localPos = CanvasPositionToLocal(posInCanvas);
                    GLHelper.DrawCircle(posInCanvas, StarGroupEditorSettings.CanvasElementPointRadius, 36, Color.red);
                }
            }
            */
            if (m_editMode == ShapeEditMode.Editing)
            {
                //Repaint();
                /*
                if (m_lastMousePosition != Event.current.mousePosition)
                {
                    m_lastMousePosition = Event.current.mousePosition;
                    Repaint();
                }
                */
            }
            //UnityEngine.Debug.Log("MousePosition:" + Input.mousePosition);      //no use      
        }

        #region 内部变量和属性

        public bool m_isDrawSelection = false; 

        public Vector2 m_dragStartPosition = Vector2.zero;

        public Vector2 m_curMousePosition = Vector2.zero;

        public Vector2 m_curLocalPosition = Vector2.zero;

        public PolygonEditorData m_data = new PolygonEditorData();

        private CanvasElementBase m_curSelected;

        private Vector2 m_operateAreaSize = new Vector2(400, 600);

        private Rect operateAreaRect
        {
            get
            {
                return new Rect(0, EditorStyles.toolbar.fixedHeight, m_operateAreaSize.x, m_operateAreaSize.y);
            }
        }

        private Vector2 canvasSize
        {
            get
            {
                return new Vector2(Screen.width - m_operateAreaSize.x - tipsAreaSize.x, Screen.height - EditorStyles.toolbar.fixedHeight);
            }
        }

        private Rect canvasAreaRect
        {
            get
            {
                return new Rect(m_operateAreaSize.x, EditorStyles.toolbar.fixedHeight, canvasSize.x, canvasSize.y);
            }
        }

        private Vector2 tipsAreaSize
        {
            get
            {
                return new Vector2(400, (Screen.height - EditorStyles.toolbar.fixedHeight) / 2);  
            }
        } 

        private Rect tipsAreaRect
        {
            get
            {
                return new Rect(Screen.width - tipsAreaSize.x, EditorStyles.toolbar.fixedHeight, tipsAreaSize.x, tipsAreaSize.y);
            }
        }

        private Vector2 debugAreaSize
        {
            get
            {
                return new Vector2(400, (Screen.height - EditorStyles.toolbar.fixedHeight) / 2);
            }
        }

        private Rect debugAreaRect
        {
            get
            {
                return new Rect(Screen.width - tipsAreaSize.x, EditorStyles.toolbar.fixedHeight + (Screen.height - EditorStyles.toolbar.fixedHeight) / 2, debugAreaSize.x, debugAreaSize.y);
            }
        }

        private Vector2 m_canvasOffset = Vector2.zero;

        private float m_canvasScale = 1;

        private float canvasScale
        {
            get
            {
                return m_canvasScale;
            }
            set
            {
                m_canvasScale = Mathf.Clamp(value, 0.1f, 10);
            }
        }

        protected Matrix4x4 canvasTRS
        {
            get
            {
                var trs = Matrix4x4.identity;
                var translate = canvasSize / 2 + m_canvasOffset;
                trs.SetTRS(translate, Quaternion.identity, new Vector3(1 / m_canvasScale, -1 / m_canvasScale, 1));
                return trs;
            }
        }

        protected ShapeEditMode m_editMode = ShapeEditMode.None;

        #endregion

    }
}