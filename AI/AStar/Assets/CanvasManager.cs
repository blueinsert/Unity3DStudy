using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace bluebean
{
    public class Setting {
        public static Color StartColor = Color.blue;
        public static Color EndColor = Color.red;
    }

    public enum EditMode {
        None = 0,
        SelectStart,
        SelectEnd,
        DrawWeight,
    }

    public class Path : List<NodePosition>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; ++i)
            {
                sb.Append(string.Format("Node {0}: {1}, {2}", i, this[i].x, this[i].y));

                if (i < Count - 1)
                {
                    sb.Append(" - ");
                }
            }

            return sb.ToString();
        }
    }

    public class Grid {
        public NodePosition m_position;
        public int m_cost = 1;

        public Grid(int x, int y) {
            m_position = new NodePosition(x, y);
        }

        public void Draw(Color c) {
            Handles.DrawSolidRectangleWithOutline(new Rect(new Vector2(m_position.x,m_position.y), new Vector2(1, 1)), c, Color.black);
            Handles.Label(new Vector3(m_position.x+0.5f, m_position.y + 0.5f, 0), m_cost.ToString());
        }
    }

    public class CanvasManager : EditorWindow
    {
        [MenuItem("AStar/OpenCanvasManager")]
        public static void OpenSceneViewCanvasEditorWindow()
        {
            var win = GetWindow<CanvasManager>(typeof(CanvasManager).Name);
            win.wantsMouseMove = true;
            win.Show();
        }

        bool _enabled;
        SceneView _sceneView;
        private bool _restoreCamera2DMode;
        private Vector3 _restoreCamearPosition;
        private Vector3 _mousePosition;

        private const int GridWidth = 20;
        private const int GridHeight = 10;

        public Grid[] m_grids = new Grid[GridWidth* GridHeight];

        private int m_selectedOperMode = 0;
        private string[] m_modeStrings = new string[] { "None","SelectStart", "SelectEnd", "DrawWeight"};

        private Grid m_startGrid;
        private Grid m_endGrid;
        private Path m_path;
        private float m_curDrawWeight = 0;

        private bool ValidPosition()
        {
            return !(_mousePosition.x < 0 || _mousePosition.x > GridWidth
                        || _mousePosition.y < 0 || _mousePosition.y > GridHeight);
        }

        private int LeftdownToIndex(Vector2Int leftdown) {
            int i = leftdown.x;
            int j = leftdown.y;
            int index = GridWidth * j + i;
            return index;
        }

        private void InitGrids()
        {
            m_grids = new Grid[GridWidth * GridHeight];
            int index = 0;
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    Grid grid = new Grid(x, y);
                    m_grids[index++] = grid;
                }
            }
        }

        private int[] GetMapData(out int width, out int height)
        {
            width = GridWidth;
            height = GridHeight;
            int[] mapData = new int[width * height];
            for (int i = 0; i < m_grids.Length; i++) {
                mapData[i] = m_grids[i].m_cost;
            }
            return mapData;
        }

        private void OnEnter() {
            InitGrids();
            SetEnabled(true);
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        private void OnExit() {
            m_grids = null;
            m_path = null;
            m_startGrid = null;
            m_endGrid = null;
            SetEnabled(false);
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        private void OnDestroy()
        {
            OnExit();
        }

        private void SetEnabled(bool value)
        {
            if (_enabled != value)
            {
                if (value)
                {
                    _sceneView = SceneView.lastActiveSceneView;
                    if (_sceneView == null) return;
                    _restoreCamera2DMode = _sceneView.in2DMode;
                    _restoreCamearPosition = _sceneView.pivot;

                    _sceneView.in2DMode = true;
                    _sceneView.LookAt(new Vector3(GridWidth/2, GridHeight/2, -10));
                    _sceneView.Repaint();
                }
                else
                {
                    _sceneView.in2DMode = _restoreCamera2DMode;
                    _sceneView.LookAt(_restoreCamearPosition);
                }
                _enabled = value;
            }
        }


        void OnSceneGUI(SceneView scene)
        {
            Event e = Event.current;
            Vector2 mousePosition = e.mousePosition;

            // View point to world point translation function in my game.
            this._mousePosition = SceneScreenToWorldPoint(mousePosition);

            // Block SceneView's built-in behavior
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);

            if (m_selectedOperMode == (int)EditMode.SelectStart) {
                if ((Event.current.type == EventType.MouseDown) && Event.current.button == 0)
                {
                    //Debug.Log(e.mousePosition + ":" + _mousePosition);
                    if (ValidPosition())
                    {
                        var leftDown = new Vector2Int(Mathf.FloorToInt(_mousePosition.x), Mathf.FloorToInt(_mousePosition.y));
                        var index = LeftdownToIndex(leftDown);
                        m_startGrid = m_grids[index];
                        scene.Repaint();
                    }

                }
            }else if (m_selectedOperMode == (int)EditMode.SelectEnd)
            {
                if ((Event.current.type == EventType.MouseDown) && Event.current.button == 0)
                {
                    //Debug.Log(e.mousePosition + ":" + _mousePosition);
                    if (ValidPosition())
                    {
                        var leftDown = new Vector2Int(Mathf.FloorToInt(_mousePosition.x), Mathf.FloorToInt(_mousePosition.y));
                        var index = LeftdownToIndex(leftDown);
                        m_endGrid = m_grids[index];
                        scene.Repaint();
                    }

                }
            }
            else if (m_selectedOperMode == (int)EditMode.DrawWeight)
            {
                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
                {
                    //Debug.Log(e.mousePosition + ":" + _mousePosition);
                    if (ValidPosition())
                    {
                        var leftDown = new Vector2Int(Mathf.FloorToInt(_mousePosition.x), Mathf.FloorToInt(_mousePosition.y));
                        var index = LeftdownToIndex(leftDown);
                        var grid = m_grids[index];
                        grid.m_cost = (int)(m_curDrawWeight*8 + 1);
                        scene.Repaint();
                    }

                }
            }

            foreach (var grid in m_grids) {
                if (grid == m_startGrid)
                {
                    grid.Draw(Setting.StartColor);
                }
                else if (grid == m_endGrid) {
                    grid.Draw(Setting.EndColor);
                }
                else
                {
                    var c = Color.Lerp(Color.gray, Color.black, (grid.m_cost-1)/8.0f);
                    grid.Draw(c);
                }
                
            }

            if (m_path != null && m_path.Count >2) {
                for (int i = 0; i < m_path.Count - 1; i++) {
                    var nodePos = m_path[i];
                    Handles.DrawSolidRectangleWithOutline(new Rect(new Vector2(nodePos.x, nodePos.y), new Vector2(1, 1)), new Color(0, 0, 0.5f, 0.5f), Color.black);
                }
            }

            if (Event.current.type == EventType.MouseDown) Event.current.Use();
            if (Event.current.type == EventType.MouseMove) Event.current.Use();
            if(Event.current.type == EventType.ScrollWheel) Event.current.Use();
        }

        private Vector3 SceneScreenToWorldPoint(Vector3 sceneScreenPoint)
        {
            Camera sceneCamera = _sceneView.camera;
            float screenHeight = sceneCamera.orthographicSize * 2f;
            float screenWidth = screenHeight * sceneCamera.aspect;

            Vector3 worldPos = new Vector3(
                (sceneScreenPoint.x / sceneCamera.pixelWidth) * screenWidth - screenWidth * 0.5f,
                ((-(sceneScreenPoint.y) / sceneCamera.pixelHeight) * screenHeight + screenHeight * 0.5f),
                0f);

            worldPos += sceneCamera.transform.position;
            worldPos.z = 0f;

            return worldPos;
        }

        private void FindPath() {
            AStarPathfinder pathfinder = new AStarPathfinder();
            NodePosition startPos = m_startGrid.m_position;
            NodePosition endPos = m_endGrid.m_position;
            int width, height;
            var mapData = GetMapData(out width, out height);
            var map = new Map(mapData, width, height);
            pathfinder.InitiatePathfind(map);
            MapSearchNode nodeStart = pathfinder.AllocateMapSearchNode(startPos);
            MapSearchNode nodeEnd = pathfinder.AllocateMapSearchNode(endPos);
            pathfinder.SetStartAndGoalStates(nodeStart, nodeEnd);

            AStarPathfinder.SearchState searchState = AStarPathfinder.SearchState.Searching;
            uint searchSteps = 0;
            do
            {
                searchState = pathfinder.SearchStep();
                searchSteps++;
            }
            while (searchState == AStarPathfinder.SearchState.Searching);

            // Search complete
            bool pathfindSucceeded = (searchState == AStarPathfinder.SearchState.Succeeded);
            if (pathfindSucceeded)
            {
                // Success
                Path newPath = new Path();
                int numSolutionNodes = 0;   // Don't count the starting cell in the path length

                // Get the start node
                MapSearchNode node = pathfinder.GetSolutionStart();
                newPath.Add(node.position);
                ++numSolutionNodes;

                // Get all remaining solution nodes
                for (; ; )
                {
                    node = pathfinder.GetSolutionNext();

                    if (node == null)
                    {
                        break;
                    }

                    ++numSolutionNodes;
                    newPath.Add(node.position);
                };

                // Once you're done with the solution we can free the nodes up
                pathfinder.FreeSolutionNodes();
                m_path = newPath;
                _sceneView.Repaint();
                Debug.Log("Solution path length: " + numSolutionNodes);
                Debug.Log("Solution: " + newPath.ToString());
            }
            else if (searchState == AStarPathfinder.SearchState.Failed)
            {
                // FAILED, no path to goal
                Debug.Log("Pathfind FAILED!");
            }
        }

        /// <summary>
        /// todo 1.设置起点
        /// 2.设施终点
        /// 3.设置地形权重
        /// </summary>
        void OnGUI()
        {
            if (!_enabled)
            {
                if (GUILayout.Button("Enter"))
                {
                    OnEnter();
                }
            }
            else
            {
                if (GUILayout.Button("Exit"))
                {
                    OnExit();
                }
                m_selectedOperMode = GUILayout.Toolbar(m_selectedOperMode, m_modeStrings);
                if (m_selectedOperMode == (int)EditMode.DrawWeight) {
                    GUILayout.Label("Weight:");
                    m_curDrawWeight = GUILayout.HorizontalSlider(m_curDrawWeight, 0, 1.0f);
                }
                if (GUILayout.Button("FindPath")) {
                    FindPath();
                }
            }
        }

    }
}
