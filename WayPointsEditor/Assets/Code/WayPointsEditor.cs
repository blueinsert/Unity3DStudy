#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean {

    public enum EMode {
        None,
        AddEdge,
    }

    [ExecuteInEditMode]
    public class WayPointsEditor : MonoBehaviour {
        public GameObject rootMap;
        public GameObject rootNode;
        public GameObject rootEdge;
        public GameObject rootPlatform;

        private WayPointsEditorModule m_module;
        private WayPointsEditorView m_view;

        private MonoBehaviour m_curSelected;
        private Vector3 m_lastMiddleMouseStartDragPosition = Vector3.zero;

        float m_lastClickTime;

        EMode m_mode = EMode.None;
        Color m_addEdgeColor;

        public void Awake() {
            m_view = new WayPointsEditorView();
            m_view.rootMap = this.rootMap;
            m_view.rootNode = this.rootNode;
            m_view.rootEdge = this.rootEdge;
            m_view.rootPlatform = this.rootPlatform;
            this.m_module = new WayPointsEditorModule(m_view);
        }

        private void ViewPortUpdate() {
            //鼠标滚轮进行缩放
            if (Input.mouseScrollDelta.y != 0f) {
                Camera.main.orthographicSize = Camera.main.orthographicSize * (1f - Input.mouseScrollDelta.y / 10f);
            }
            //按住鼠标中键进行拖动
            if (Input.GetMouseButtonDown(2))
            {
                m_lastMiddleMouseStartDragPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(2))
            {
                if (m_lastMiddleMouseStartDragPosition != Input.mousePosition)
                {
                    //var offset = Input.mousePosition - lastMiddleMouseStartDragPosition;

                    var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f)) - Camera.main.ScreenToWorldPoint(new Vector3(m_lastMiddleMouseStartDragPosition.x, m_lastMiddleMouseStartDragPosition.y, 0f));
                    Camera.main.transform.position -= worldPos;
                    m_lastMiddleMouseStartDragPosition = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButton(2))
            {
                m_lastMiddleMouseStartDragPosition = Vector2.zero;
            }
        }

        void TrySelect() {
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            var hits = Physics2D.OverlapPointAll(new Vector2(worldPos.x, worldPos.y));
            if (hits.Length > 0) {
                foreach (var h in hits) {
                    var node = h.transform.gameObject.GetComponent<EditorNode>();
                    var edge = h.transform.gameObject.GetComponent<EditorEdge>();
                    if (node != null) {
                        Selection.activeGameObject = h.transform.gameObject;
                        m_curSelected = node;
                        break;
                    }
                    if (edge != null) {
                        if (edge == m_curSelected && (Time.time - m_lastClickTime) < 0.2f)
                        {
                            if (edge.direction == EditorEdge.EDirection.Single)
                            {
                                edge.direction = EditorEdge.EDirection.Inverse;
                            }
                            else if (edge.direction == EditorEdge.EDirection.Inverse)
                            {
                                edge.direction = EditorEdge.EDirection.Dual;
                            }
                            else if (edge.direction == EditorEdge.EDirection.Dual)
                            {
                                edge.direction = EditorEdge.EDirection.Single;
                            }
                        }else
                        {
                            Selection.activeGameObject = h.transform.gameObject;
                            m_curSelected = edge;
                        }
                        
                        break;
                    }
                }
            }
        }

        void TryDelete() {
            if (m_curSelected != null && Selection.activeGameObject == m_curSelected.gameObject) {
                if (m_curSelected is EditorEdge) {
                    Selection.activeGameObject = null;
                    this.m_module.RemoveEdge(m_curSelected as EditorEdge);
                    m_curSelected = null;
                } else if (m_curSelected is EditorNode) {
                    Selection.activeGameObject = null;
                    this.m_module.RemoveNode(m_curSelected as EditorNode);
                    m_curSelected = null;
                }
            }
        }

        EditorNode TestNodeOnMousePosition() {
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            var hits = Physics2D.OverlapPointAll(new Vector2(worldPos.x, worldPos.y));
            if (hits.Length > 0) {
                foreach (var h in hits) {
                    var node = h.transform.gameObject.GetComponent<EditorNode>();
                    if (node != null) {
                        return node;
                    }
                }
            }
            return null;
        }

        private void TryAddElement() {
            if (m_mode == EMode.None)
            {
                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
                {
                    var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                    this.m_module.AddNode(worldPos);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    var node = TestNodeOnMousePosition();
                    if (node != null)
                    {
                        if (node == (m_curSelected as EditorNode))
                        {
                            m_mode = EMode.AddEdge;
                        }
                        else
                        {
                            m_curSelected = node;
                            Selection.activeGameObject = node.gameObject;
                            m_mode = EMode.AddEdge;
                        }
                    }
                } 
            }
            else if (m_mode == EMode.AddEdge)
            {
                var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                var hits = Physics2D.OverlapPointAll(new Vector2(worldPos.x, worldPos.y));

                m_addEdgeColor = Color.red;
                if (hits.Length > 0)
                {
                    foreach (var h in hits)
                    {
                        if (h.transform.gameObject.GetComponent<EditorNode>() != null)
                        {
                            m_addEdgeColor = Color.green;
                            break;
                        }
                    }
                }

                if (Input.GetMouseButtonUp(1))
                {
                    if (hits.Length != 0)
                    {
                        EditorNode overlapNode = null;
                        foreach (var h in hits)
                        {
                            if (h.transform.gameObject.GetComponent<EditorNode>() != null)
                            {
                                overlapNode = h.transform.gameObject.GetComponent<EditorNode>();
                                break;
                            }
                        }
                        if (overlapNode != m_curSelected && overlapNode != null)
                        {
                            this.m_module.AddEdge(m_curSelected as EditorNode, overlapNode);
                        }
                    }
                    m_mode = EMode.None;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    var node = this.m_module.AddNode(worldPos);
                    var edge = this.m_module.AddEdge((m_curSelected as EditorNode), node); 
                    m_curSelected = node;
                    Selection.activeGameObject = node.gameObject;
                    //ChangeToNoneMode();
                }
            }

        }

        public void Update() {
            ViewPortUpdate();
            if (Input.GetMouseButtonDown(0)) {
                TrySelect();
                m_lastClickTime = Time.time;
            }
            if (Input.GetKeyDown(KeyCode.Delete)) {
                TryDelete();
            }
            TryAddElement();
        }

        void OnDrawGizmos() {
            // Gizmos.color = UnityEngine.Color.green;
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            if (m_mode == EMode.AddEdge) {
                Gizmos.color = m_addEdgeColor;
                if (m_curSelected is EditorNode) {
                    Gizmos.DrawLine((m_curSelected as EditorNode).position, worldPos);
                }
            }
        }

        void OnGUI() {
            GUILayout.BeginHorizontal();
           
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
           
            if (GUILayout.Button("Clear WayPoints", GUILayout.Width(150))) {
                this.m_module.Clear();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Export", GUILayout.Width(100))) {
               
            }
        }

    }//class
}//namespace
#endif