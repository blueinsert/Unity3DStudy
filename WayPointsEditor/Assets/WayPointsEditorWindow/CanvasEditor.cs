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
            GUILayout.Label("MousePosition:" + Event.current.mousePosition);
            GUILayout.Label("ScreenWidth:" + Screen.width);
            GUILayout.Label("ScreenHeight:" + Screen.height);
            GUILayout.Label("CanvasOffset:" + string.Format("[{0},{1}]", m_canvasOffset.x, m_canvasOffset.y));
            GUILayout.Label("CanvasScale:" + m_canvasScale);
            GUILayout.EndArea();
        }


        protected virtual void OnDraw()
        {
            GLHelper.DrawGrid(50, new Rect(-100, -100, 200, 200));
            GLHelper.DrawCircle(new Vector2(0, 40), 2, 36, Color.blue);
            GLHelper.DrawCircle(new Vector2(30, 0), 2, 36, Color.blue);
            GLHelper.DrawCircle(new Vector2(0, 0), 2, 36, Color.blue);
        }

        private void Draw()
        {
            ///*
            GL.LoadIdentity();
            var trs = Matrix4x4.identity;
            var translate = new Vector2(250, 250) + m_canvasOffset;
            trs.SetTRS(translate, Quaternion.identity, new Vector3(1 / m_canvasScale, -1 / m_canvasScale, 1));
            GL.MultMatrix(trs);
            GL.PushMatrix();
            //*/
            //GL.PushMatrix();
            OnDraw();
            GL.PopMatrix();
        }

        private void OnDrawCanvasBegin()
        {
            GUILayout.BeginArea(canvasAreaRect);
            if (Event.current.type == EventType.Repaint)
            {
                new GUIStyle("flow background").Draw(new Rect(0, 0, m_canvasSize.x, m_canvasSize.y), false, false, false, false);
                Draw();
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
                canvasScale = canvasScale + Event.current.delta.y * CanvasEditorSettings.MouseWheelZoomRadio;
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