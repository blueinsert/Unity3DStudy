using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean
{

    public class CanvasEditor : EditorWindow
    {
        [MenuItem("Assets/TestCanvasEditor")]
        public static void onInit()
        {
            var win = Instance;
        }

        private static CanvasEditor m_instance;
        public static CanvasEditor Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = GetWindow<CanvasEditor>();
                }
                return m_instance;
            }
        }

        private void OnDrawDebugInfos()
        {
            GUILayout.BeginArea(debugAreaRect);

            GUILayout.Label("ScreenWidth:" + Screen.width);
            GUILayout.Label("ScreenHeight:" + Screen.height);
            GUILayout.EndArea();
        }

        void DrawGrid()
        {
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            Vector2 originInCanvas = LocalToCanvas(Vector2.zero, new Rect(0, 0, 500, 500), m_canvasOffset, canvasScale);

            this.DrawGridLines(new Rect(0, 0, 500, 500), 100, m_canvasOffset, canvasScale, new Color(0f, 0f, 0f, 0.28f));
            this.DrawGridLines(new Rect(0, 0, 500, 500), 10, m_canvasOffset, canvasScale, new Color(0f, 0f, 0f, 0.18f));
            GL.End();
            GL.PopMatrix();
        }

        Vector2 CanvasToLocal(Vector2 canvasPos, Rect showRect, Vector2 offset, float scale)
        {
            Vector2 originInCanvas = showRect.center + offset;
            var pos = (canvasPos - originInCanvas) * scale;
            return pos;
        }

        Vector2 LocalToCanvas(Vector2 pos, Rect showRect, Vector2 offset, float scale)
        {
            Vector2 originInCanvas = showRect.center + offset;
            var canvasPos = pos / scale + originInCanvas;
            return canvasPos;
        }

        void DrawGridLines(Rect rect, int gridSize, Vector2 offset, float scale, Color gridColor)
        {
            GL.Color(gridColor);
            Rect rectLocal = new Rect(CanvasToLocal(rect.center, rect, offset, scale) - rect.size * scale / 2, rect.size * scale);

            for (int x = (int)rectLocal.xMin; x < (int)rectLocal.xMax; x++)
            {
                if (x % gridSize == 0)
                {
                    DrawLine(LocalToCanvas(new Vector2(x, rectLocal.yMin), rect, offset, scale), LocalToCanvas(new Vector2(x, rectLocal.yMax), rect, offset, scale));
                }  
            }
            for (int y = (int)rectLocal.yMin; y < (int)rectLocal.yMax; y++)
            {
                if(y % gridSize == 0)
                {
                    DrawLine(LocalToCanvas(new Vector2(rectLocal.xMin, y), rect, offset, scale), LocalToCanvas(new Vector2(rectLocal.xMax, y), rect, offset, scale));
                }
            }
        }

        void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }


        private void OnDrawCanvasBegin()
        {
            GUILayout.BeginArea(canvasAreaRect);
            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle("flow background").Draw(new Rect(0, 0, m_canvasSize.x, m_canvasSize.y), false, false, false, false);
                DrawGrid();
            }
        }

        private void OnDrawCanvasEnd()
        {
            GUILayout.EndArea();
        }

        void OnDrawMainTool()
        {

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.BeginHorizontal(GUILayout.Width(400));
            GUILayout.Label("xx");
            GUILayout.EndHorizontal();
            GUILayout.Label("YY");
            GUILayout.EndHorizontal();
        }

        void OnEvent()
        {
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                m_canvasOffset += Event.current.delta;
                Event.current.Use();
            }
            if (Event.current.type == EventType.ScrollWheel)
            {
                m_canvasScale = m_canvasScale + Event.current.delta.y * -0.01f;
                Event.current.Use();
            }
        }

        void OnGUI()
        {
            OnEvent();
            OnDrawMainTool();
            OnDrawDebugInfos();
            OnDrawCanvasBegin();
            OnDrawCanvasEnd();
        }

        private Vector2 m_debugAreaSize = new Vector2(400, 600);

        private Rect debugAreaRect
        {
            get
            {
                return new Rect(0, EditorStyles.toolbar.fixedHeight, m_debugAreaSize.x, m_debugAreaSize.y);
            }
        }


        private Vector2 m_canvasSize = new Vector2(500, 500);
        private Rect canvasAreaRect
        {
            get
            {
                return new Rect(m_debugAreaSize.x, EditorStyles.toolbar.fixedHeight, m_canvasSize.x, m_canvasSize.y);
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

    }

}