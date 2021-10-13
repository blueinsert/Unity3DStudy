using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bluebean
{
    public enum WayPointsEMode
    {
        None,
        AddEdge,
    }

    public class WayPointsEditorWindow : CanvasEditorWindow
    {
        private WayPointsData m_wayPointsData = new WayPointsData();
        private WayPointsEMode m_mode = WayPointsEMode.None;
        private Color m_addEdgeColor = Color.red;
        private Point m_edgeStart;
        private CanvasElementBase m_lastClickElement;
        private int m_lastClickFrame;

        [MenuItem("Windows/OpenWayPointsEditorWindow")]
        public static void OpenWayPointsEditorWindow()
        {
            var win = GetWindow<WayPointsEditorWindow>(typeof(WayPointsEditorWindow).Name);
            win.wantsMouseMove = true;
            win.InitSubAreaContext();
            win.RegisterEvent();
        }
  
        private void RegisterEvent()
        {
            if (m_defaultCanvasCtx != null)
            {
                m_wayPointsData.onAddElement += m_defaultCanvasCtx.AddElement;
                m_wayPointsData.onRemoveElement += m_defaultCanvasCtx.RemoveElement;
            }
        }

        private void OnDestroy()
        {
            if (m_defaultCanvasCtx != null)
            {
                m_wayPointsData.onAddElement -= m_defaultCanvasCtx.AddElement;
                m_wayPointsData.onRemoveElement -= m_defaultCanvasCtx.RemoveElement;
            }
        }


        #region 窗口绘制

        protected override void OnDrawCanvas(CanvasContext canvasCtx)
        {
            base.OnDrawCanvas(canvasCtx);
            foreach(var point in m_wayPointsData.m_points)
            {
                point.Draw(canvasCtx.IsSelected(point) ? Color.red : Color.blue);
            }
            foreach(var edge in m_wayPointsData.m_edges)
            {
                edge.Draw(canvasCtx.IsSelected(edge) ? Color.red : Color.blue);
            }
            if(m_mode == WayPointsEMode.AddEdge)
            {
                GLHelper.DrawLine(m_edgeStart.m_position, canvasCtx.m_curLocalPosition, m_addEdgeColor);
            }
        }

        #endregion


        #region 事件处理

        private void TryDelete(CanvasContext canvasCtx)
        {
            foreach(var element in new List<CanvasElementBase>(canvasCtx.m_selectedElements))
            {
                if (element is Point)
                {
                    m_wayPointsData.RemovePoint(element as Point);
                }
                else if (element is Edge)
                {
                    m_wayPointsData.RemoveEdge(element as Edge);
                }
            }
            canvasCtx.ClearAllSelected();
        }

        protected override void ProcessEventInCanvas(CanvasContext canvasCtx)
        {
            base.ProcessEventInCanvas(canvasCtx);
            if(m_mode == WayPointsEMode.None)
            {
                if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    //双击边改变方向
                    var element = canvasCtx.TestPoint(canvasCtx.m_curLocalPosition);
                    if(element != null)
                    {
                        if (element is Edge && element == m_lastClickElement)
                        {
                            if (m_frameCount - m_lastClickFrame < 40)
                            {
                                m_lastClickFrame = m_frameCount;
                                (element as Edge).ChangeDir();
                            }
                        }
                        m_lastClickFrame = m_frameCount;
                        Debug.Log(m_lastClickFrame);
                        m_lastClickElement = element;
                    }
                    //添加点
                    if (Event.current.control)
                    {
                        m_wayPointsData.AddPoint(canvasCtx.m_curLocalPosition);
                    }
                    
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    //在点上点击鼠标右键开始添加边
                    var element = canvasCtx.TestPoint(canvasCtx.m_curLocalPosition);
                    if(element != null && element is Point)
                    {
                        m_edgeStart = element as Point;
                        m_mode = WayPointsEMode.AddEdge;
                    }
                }
                //删除所选
                if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
                {
                    TryDelete(canvasCtx);
                }
            }else if(m_mode == WayPointsEMode.AddEdge)
            {
                var element = canvasCtx.TestPoint(canvasCtx.m_curLocalPosition);
                bool canAddEdge = element != null && element is Point && element != m_edgeStart && !m_wayPointsData.ContainEdge(m_edgeStart, element as Point);
                if (canAddEdge)
                {
                    m_addEdgeColor = Color.green;
                }
                else
                {
                    m_addEdgeColor = Color.red;
                }
                //松开鼠标右键添加边
                if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
                {
                    if(canAddEdge)
                    {
                        m_wayPointsData.AddEdge(m_edgeStart as Point, element as Point);
                    }
                    m_mode = WayPointsEMode.None;
                }else if(Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    //添加点自动连成边，并继续添加边
                    var node = m_wayPointsData.AddPoint(canvasCtx.m_curLocalPosition);
                    m_wayPointsData.AddEdge(m_edgeStart as Point, node);
                    m_edgeStart = node;
                }
            }

        }

        #endregion

    }
}
