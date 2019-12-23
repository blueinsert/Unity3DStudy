using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace bluebean
{
    public enum OpMode
    {
        None,
        Move,
    }

    public class MoveOpData
    {
        public TreeNode opObj;
        public Vector2 moveStartPos;
    }

    delegate bool NodeAction(TreeNode node, bool isShow);
    delegate bool NodeAction2(TreeNode node);
    delegate void ConnectLineAction(ConnectPoint point1, ConnectPoint point2, int childIndex, bool isShow);
    delegate void ConnectLineAction2(TreeNode parent, TreeNode child, int childIndex);

    public class TreeEditorWindow : CanvasEditorWindow
    {

        [MenuItem("SDFModellingTool/TreeEditor")]
        public static void OpenTreeEditorWindow()
        {
            var win = GetWindow<TreeEditorWindow>(typeof(TreeEditorWindow).Name);
            win.wantsMouseMove = true;
            win.InitSubAreaContext();
            win.Init();
        }

        #region 变量
        public const float kToolbarButtonWidth = 50f;
        public const float kToolbarHeight = 20f;

        private TreeNode Root {
            get {
                return m_nodes.Find((elem) => { return elem.m_type == TreeNodeType.Root; });
            }
        }
        /// <summary>
        /// 所有节点的List
        /// </summary>
        private List<TreeNode> m_nodes = new List<TreeNode>();

        private TreeNode m_curSelected;
        private Vector2 m_mouseDragStartPos;
        private Vector2 m_objDragStartPos;

        private ConnectPoint m_curOpConnectPoint;

        private OpMode m_curOpMode = OpMode.None;
        private MoveOpData m_moveOpData;

        #endregion

        #region Tree操作

        private void AddTreeNode(Vector2 position, TreeNodeType type)
        {
            TreeNode node = new TreeNode(type.ToString(), position, type);
            m_nodes.Add(node);
            OnDSChanged();
        }

        void DeleteTreeNode(TreeNode node)
        {
            var oldParent = node.Parent;
            if (oldParent != null)
            {
                if (oldParent.m_lChild == node)
                {
                    oldParent.m_lChild = null;
                }
                if (oldParent.m_rChild == node)
                {
                    oldParent.m_rChild = null;
                }
            }
            m_nodes.Remove(node);
            OnDSChanged();
        }

        void SetParent(TreeNode parent, TreeNode child, int index)
        {
            var oldParent = child.Parent;
            if (oldParent != null)
            {
                if (oldParent.m_lChild == child)
                {
                    oldParent.m_lChild = null;
                }
                if (oldParent.m_rChild == child)
                {
                    oldParent.m_rChild = null;
                }
            }
            switch (index)
            {
                case 0:
                    parent.m_lChild = child;
                    break;
                case 1:
                    parent.m_rChild = child;
                    break;
            }
            child.Parent = parent;
            OnDSChanged();
        }

        /// <summary>
        /// 广度遍历
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="action"></param>
        private void Traversal(TreeNode tree, Action<TreeNode> action)
        {
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                action(current);
                if (current.m_lChild != null)
                {
                    queue.Enqueue(current.m_lChild);
                }
                if (current.m_rChild != null)
                {
                    queue.Enqueue(current.m_rChild);
                }
            }
        }

        private void Traversal(TreeNode tree, NodeAction2 nodeAction, ConnectLineAction2 connectLineAction)
        {
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!nodeAction(current))
                {
                    return;
                }
                if (current.m_lChild != null)
                {
                    queue.Enqueue(current.m_lChild);
                    connectLineAction(current, current.m_lChild, 0);
                }
                if (current.m_rChild != null)
                {
                    queue.Enqueue(current.m_rChild);
                    connectLineAction(current, current.m_rChild, 1);
                }
            }
        }

        /// <summary>
        /// 遍历Nodes数组
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="nodeAction"></param>
        /// <param name="connectLineAction"></param>
        private void Traversal(List<TreeNode> nodes, NodeAction nodeAction, ConnectLineAction connectLineAction)
        {
            int[] visited = new int[nodes.Count];
            for (int i = 0; i < visited.Length; i++)
            {
                visited[i] = 0;
            }
            bool[] shouldDraw = new bool[nodes.Count];
            for (int i = 0; i < shouldDraw.Length; i++)
            {
                shouldDraw[i] = true;
            }
            Queue<TreeNode> queue = new Queue<TreeNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (visited[i] == 1)
                    continue;
                queue.Enqueue(nodes[i]);
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    int index = nodes.FindIndex((elem) => { return elem == current; });
                    if (!nodeAction(current, shouldDraw[index]))
                    {
                        return;//中断遍历
                    }

                    //设置为已访问
                    visited[index] = 1;

                    if (current.m_lChild != null)
                    {
                        queue.Enqueue(current.m_lChild);
                        var lIndex = m_nodes.FindIndex((elem) => { return elem == current.m_lChild; });
                        if (!current.m_isExpand || !shouldDraw[index])
                        {
                            shouldDraw[lIndex] = false;
                        }
                        connectLineAction(current.LOutPoint, current.m_lChild.InPoint, 0, shouldDraw[index] && shouldDraw[lIndex]);
                    }
                    if (current.m_rChild != null)
                    {
                        queue.Enqueue(current.m_rChild);
                        var rIndex = m_nodes.FindIndex((elem) => { return elem == current.m_rChild; });
                        if (!current.m_isExpand || !shouldDraw[index])
                        {
                            shouldDraw[rIndex] = false;
                        }
                        connectLineAction(current.ROutPoint, current.m_rChild.InPoint, 1, shouldDraw[index] && shouldDraw[rIndex]);
                    }
                }
            }
        }

        private void DeleteTree(TreeNode tree)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            Traversal(tree, (node) =>
            {
                nodes.Add(node);
            });
            foreach (var node in nodes)
            {
                DeleteTreeNode(node);
            }
        }

        /// <summary>
        /// 整体位移
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="position"></param>
        private void ResetTreePosition(TreeNode tree, Vector2 position)
        {
            var diff = position - tree.Position;
            Traversal(tree, (node) =>
            {
                node.Position += diff;
            });
        }

        private void UnexpandTree(TreeNode tree)
        {
            tree.m_isExpand = false;
        }

        private void ExpandTree(TreeNode tree)
        {
            tree.m_isExpand = true;
        }

        ConnectPoint GetClickOutConnectPoint(Vector2 position)
        {
            ConnectPoint connectPoint = null;
            Traversal(m_nodes, (node, isShow) =>
            {
                if (isShow)
                {
                    if (node.m_isExpand)
                    {
                        if ((connectPoint = node.GetClickOutConnectPoint(position)) != null)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }, (p1, p2, childIndex, isShow) => { });
            return connectPoint;
        }

        ConnectPoint GetClickInConnectPoint(Vector2 position)
        {
            ConnectPoint connectPoint = null;
            Traversal(m_nodes, (node, isShow) =>
            {
                if (isShow)
                {
                    if ((connectPoint = node.GetClickInConnectPoint(position)) != null)
                    {
                        return false;
                    }
                }
                return true;
            }, (p1, p2, childIndex, isShow) => { });
            return connectPoint;
        }

        TreeNode GetClickNode(Vector2 position)
        {
            TreeNode res = null;
            Traversal(m_nodes, (node, isShow) =>
            {
                if (isShow)
                {
                    if (node.TestPoint(position))
                    {
                        res = node;
                        return false;
                    }
                }
                return true;
            }, (p1, p2, childIndex, isShow) => { });
            return res;
        }

        #endregion

        private void SaveTree(TreeNode tree)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                nodes.Add(current);
                if (current.m_lChild != null)
                {
                    queue.Enqueue(current.m_lChild);
                }
                if (current.m_rChild != null)
                {
                    queue.Enqueue(current.m_rChild);
                }
            }
            queue.Clear();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                current.m_parentIndex = -1;
                current.m_lIndex = -1;
                current.m_rIndex = -1;
                if (current.Parent != null)
                {
                    current.m_parentIndex = nodes.FindIndex((elem) => { return elem == current.Parent; });
                }
                if (current.m_lChild != null)
                {
                    queue.Enqueue(current.m_lChild);
                    current.m_lIndex = nodes.FindIndex((elem) => { return elem == current.m_lChild; });
                }
                if (current.m_rChild != null)
                {
                    queue.Enqueue(current.m_rChild);
                    current.m_rIndex = nodes.FindIndex((elem) => { return elem == current.m_rChild; });
                }
            }
            string json = EditorJsonUtility.ToJson(new Serialization<TreeNode>(nodes), true);
            string filePath = EditorUtility.SaveFilePanel("Save This Tree", "Assets/Editor/SubTreeDefs", tree.m_name, "tree");
            if (string.IsNullOrEmpty(filePath) == false)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
                {
                    file.Write(json);
                }
            }
            Debug.Log(json);
        }



        private void LoadTree(Vector2 pos)
        {
            List<TreeNode> nodes = null;
            string filePath = EditorUtility.OpenFilePanel("Load This Tree", "Assets/Editor/SubTreeDefs", "tree");
            if (string.IsNullOrEmpty(filePath) == false)
            {
                using (System.IO.StreamReader file = new System.IO.StreamReader(filePath))
                {
                    var json = file.ReadToEnd();
                    nodes = JsonUtility.FromJson<Serialization<TreeNode>>(json).ToList();
                }
            }
            if (nodes == null || nodes.Count == 0)
            {
                Debug.LogError("LoadTree nodes is null");
                return;
            }
            TreeNode tree = nodes[0];
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.m_parentIndex != -1)
                {
                    current.Parent = nodes[current.m_parentIndex];
                }
                if (current.m_lIndex != -1)
                {
                    current.m_lChild = nodes[current.m_lIndex];
                    queue.Enqueue(current.m_lChild);
                }
                if (current.m_rIndex != -1)
                {
                    current.m_rChild = nodes[current.m_rIndex];
                    queue.Enqueue(current.m_rChild);
                }
            }
            m_nodes.AddRange(nodes);
            ResetTreePosition(tree, pos);
        }

        private void Init()
        {
            TextureLib.LoadStandardTextures();
            TextureLib.LoadTexture("Sphere");
            TextureLib.LoadTexture("Box");
            TextureLib.LoadTexture("Union");
            TextureLib.LoadTexture("Substraction");
            TextureLib.LoadTexture("Intersection");
            TextureLib.LoadTexture("Displace");
            TextureLib.LoadTexture("Rotate");
            TextureLib.LoadTexture("Scale");

            FontLib.Clear();
            FontLib.LoadFont("font");
        }

        void Empty() { }

        #region 绘制

        private bool DropdownButton(string name, float width)
        {
            return GUILayout.Button(name, EditorStyles.toolbarDropDown, GUILayout.Width(width));
        }

        /// <summary>
        /// 文件菜单
        /// </summary>
        private void createFileMenu()
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Create New"), false, Empty);
            menu.AddItem(new GUIContent("Load"), false, Empty);

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Save"), false, Empty);
            menu.AddItem(new GUIContent("Save As"), false, Empty);

            menu.DropDown(new Rect(5f, kToolbarHeight, 0f, 0f));
        }

        /// <summary>
        /// 主菜单条
        /// </summary>
        protected override void OnDrawMainTool()
        {
            if (DropdownButton("File", kToolbarButtonWidth))
            {
                createFileMenu();
            }
            if (DropdownButton("Edit", kToolbarButtonWidth))
            {
                //createEditMenu();
            }
            if (DropdownButton("View", kToolbarButtonWidth))
            {
                //createViewMenu();
            }
            if (DropdownButton("Settings", kToolbarButtonWidth + 10f))
            {
                //createSettingsMenu();
            }
            if (DropdownButton("Tools", kToolbarButtonWidth))
            {
                //createToolsMenu();
            }
        }

        private void IndentBlock(System.Action action)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            action();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawOperateArea()
        {
            if (GUILayout.Button("Refrest"))
            {
                OnDSChanged();
            }
            IndentBlock(() =>
            {
                if (m_curSelected == null)
                    return;
                var node = m_curSelected as TreeNode;
                if (node != null)
                {
                    GUILayout.Label("Type:" + node.m_type.ToString());
                    node.m_name = EditorGUILayout.TextField("Name:", node.m_name);
                    if (node.m_type == TreeNodeType.Sphere)
                    {
                        node.m_data.W = Mathf.Clamp(EditorGUILayout.FloatField("radius", (float)node.m_data.W), 0, float.MaxValue);
                    }
                    else if (node.m_type == TreeNodeType.Box)
                    {
                        node.m_data.X = Mathf.Clamp(EditorGUILayout.FloatField("length:", (float)node.m_data.X), 0, float.MaxValue);
                        node.m_data.Y = Mathf.Clamp(EditorGUILayout.FloatField("width:", (float)node.m_data.Y), 0, float.MaxValue);
                        node.m_data.Z = Mathf.Clamp(EditorGUILayout.FloatField("height:", (float)node.m_data.Z), 0, float.MaxValue);
                    }else if(node.m_type == TreeNodeType.RoundBox)
                    {
                        node.m_data.X = Mathf.Clamp(EditorGUILayout.FloatField("length:", (float)node.m_data.X), 0, float.MaxValue);
                        node.m_data.Y = Mathf.Clamp(EditorGUILayout.FloatField("width:", (float)node.m_data.Y), 0, float.MaxValue);
                        node.m_data.Z = Mathf.Clamp(EditorGUILayout.FloatField("height:", (float)node.m_data.Z), 0, float.MaxValue);
                        node.m_data.W = Mathf.Clamp(EditorGUILayout.FloatField("r:", (float)node.m_data.W), 0, float.MaxValue);
                    }else if(node.m_type == TreeNodeType.Torus)
                    {
                        node.m_data.X = Mathf.Clamp(EditorGUILayout.FloatField("diameter:", (float)node.m_data.X), 0, float.MaxValue);
                        node.m_data.Y = Mathf.Clamp(EditorGUILayout.FloatField("thickness:", (float)node.m_data.Y), 0, float.MaxValue);
                    }
                    else if (node.m_type == TreeNodeType.Cylinder)
                    {
                        node.m_data.X = Mathf.Clamp(EditorGUILayout.FloatField("diameter:", (float)node.m_data.X), 0, float.MaxValue);
                        node.m_data.Y = Mathf.Clamp(EditorGUILayout.FloatField("height:", (float)node.m_data.Y), 0, float.MaxValue);
                    }
                    else if (node.m_type == TreeNodeType.Cone)
                    {
                        var angle = Mathf.Clamp(EditorGUILayout.FloatField("angle:", (float)node.m_data.Z), 0, float.MaxValue);
                        node.m_data.X = Mathf.Sin(angle);
                        node.m_data.Y = Mathf.Cos(angle);
                        node.m_data.Z = angle;
                    }
                    else if (node.m_type == TreeNodeType.HexPrism)
                    {
                        node.m_data.X = Mathf.Clamp(EditorGUILayout.FloatField("x:", (float)node.m_data.X), 0, float.MaxValue);
                        node.m_data.Y = Mathf.Clamp(EditorGUILayout.FloatField("y:", (float)node.m_data.Y), 0, float.MaxValue);
                    }
                    else if (node.m_type == TreeNodeType.TriPrism)
                    {
                        node.m_data.X = Mathf.Clamp(EditorGUILayout.FloatField("x:", (float)node.m_data.X), 0, float.MaxValue);
                        node.m_data.Y = Mathf.Clamp(EditorGUILayout.FloatField("y:", (float)node.m_data.Y), 0, float.MaxValue);
                    }
                    else if (node.m_type == TreeNodeType.Capsule)
                    {
                        node.m_data.Position1 = EditorGUILayout.Vector3Field("position1", node.m_data.Position1);
                        node.m_data.Position2 = EditorGUILayout.Vector3Field("position2", node.m_data.Position2);
                        node.m_data.R = Mathf.Clamp(EditorGUILayout.FloatField("R", node.m_data.R),0,float.MaxValue);
                    }
                    else if (node.m_type == TreeNodeType.Triangle)
                    {
                        node.m_data.Position1 = EditorGUILayout.Vector3Field("position1", node.m_data.Position1);
                        node.m_data.Position2 = EditorGUILayout.Vector3Field("position2", node.m_data.Position2);
                        node.m_data.Position3 = EditorGUILayout.Vector3Field("position3", node.m_data.Position3);
                    }
                    else if (node.m_type == TreeNodeType.Quad)
                    {
                        node.m_data.Position1 = EditorGUILayout.Vector3Field("position1", node.m_data.Position1);
                        node.m_data.Position2 = EditorGUILayout.Vector3Field("position2", node.m_data.Position2);
                        node.m_data.Position3 = EditorGUILayout.Vector3Field("position3", node.m_data.Position3);
                        node.m_data.Position4 = EditorGUILayout.Vector3Field("position4", node.m_data.Position4);
                    }
                    else if (node.m_type == TreeNodeType.Position || node.m_type == TreeNodeType.Rotate)
                    {
                        node.m_data.X = EditorGUILayout.FloatField("x:", (float)node.m_data.X);
                        node.m_data.Y = EditorGUILayout.FloatField("y:", (float)node.m_data.Y);
                        node.m_data.Z = EditorGUILayout.FloatField("z:", (float)node.m_data.Z);
                    }
                    if (node.m_type == TreeNodeType.Scale)
                    {
                        node.m_data.W = Mathf.Clamp(EditorGUILayout.FloatField("scale:", (float)node.m_data.W), 0, float.MaxValue);
                    }
                    else if (node.m_type == TreeNodeType.Union || node.m_type == TreeNodeType.Subtraction || node.m_type == TreeNodeType.Intersection)
                    {
                        //empty
                    }

                }
            });
        }

        protected override void OnDrawSubArea(SubAreaContext subAreaCtx)
        {
            base.OnDrawSubArea(subAreaCtx);
            if (subAreaCtx.m_areaDef.m_showName == "OperateArea")
            {
                DrawOperateArea();
            }
        }

        protected override void OnDrawCanvas(CanvasContext ctx)
        {
            base.OnDrawCanvas(ctx);
            Dictionary<ConnectPoint, ConnectPoint> lines = new Dictionary<ConnectPoint, ConnectPoint>();
            List<TreeNode> drawNodes = new List<TreeNode>();
            Traversal(m_nodes, (node, isShow) =>
            {
                if (isShow)
                {
                    drawNodes.Add(node);
                }
                return true;
            }, (p1, p2, childIndex, isShow) =>
            {
                if (isShow)
                {
                    lines.Add(p1, p2);
                }
            });
            //绘制节点
            foreach (var node in drawNodes)
            {
                ctx.Color = m_curSelected == node ? node.SelectedColor : node.Color;
                node.Draw(ctx);
            }
            //绘制连线
            foreach (var line in lines)
            {
                ctx.DrawBezier(line.Key.Position, line.Value.Position, 12, new Color(0.1f, 0.5f, 0.9f, 0.9f));
            }
            if (m_curOpConnectPoint != null)
            {
                ctx.DrawBezier(m_curOpConnectPoint.Position, ctx.MousePosition, 12, new Color(0.9f,0.1f,0.2f,0.9f));
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 鼠标右键点击菜单
        /// </summary>
        /// <param name="ctx"></param>
        private void ProcessContextMenu(CanvasContext ctx)
        {
            var elem = GetClickNode(ctx.MousePosition);
            if (elem != null && elem is TreeNode)
            {
                var node = elem as TreeNode;
                GenericMenu genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("SaveThisTree"), false, () => SaveTree(elem as TreeNode));
                //genericMenu.AddSeparator("");
                genericMenu.ShowAsContext();
            }
            else
            {
                GenericMenu genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("LoadTree"), false, () => LoadTree(ctx.MousePosition));
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("Create/Root"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Root));
                genericMenu.AddSeparator("Create/");
                genericMenu.AddItem(new GUIContent("Create/Primitives/Sphere"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Sphere));
                genericMenu.AddItem(new GUIContent("Create/Primitives/Box"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Box));
                genericMenu.AddItem(new GUIContent("Create/Primitives/RoundBox"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.RoundBox));
                genericMenu.AddItem(new GUIContent("Create/Primitives/Torus"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Torus));
                genericMenu.AddItem(new GUIContent("Create/Primitives/Cylinder"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Cylinder));
                genericMenu.AddItem(new GUIContent("Create/Primitives/Cone"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Cone));
                genericMenu.AddItem(new GUIContent("Create/Primitives/HexPrism"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.HexPrism));
                genericMenu.AddItem(new GUIContent("Create/Primitives/TriPrism"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.TriPrism));
                genericMenu.AddItem(new GUIContent("Create/Primitives/Capsule"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Capsule));
                genericMenu.AddItem(new GUIContent("Create/Primitives/Triangle"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Triangle));
                genericMenu.AddItem(new GUIContent("Create/Primitives/Quad"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Quad));
                genericMenu.AddSeparator("Create/");
                genericMenu.AddItem(new GUIContent("Create/BoolOp/Union"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Union));
                genericMenu.AddItem(new GUIContent("Create/BoolOp/Substract"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Subtraction));
                genericMenu.AddItem(new GUIContent("Create/BoolOp/Intersect"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Intersection));
                genericMenu.AddSeparator("Create/");
                genericMenu.AddItem(new GUIContent("Create/Op/Position"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Position));
                genericMenu.AddItem(new GUIContent("Create/Op/Rotate"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Rotate));
                genericMenu.AddItem(new GUIContent("Create/Op/Scale"), false, () => AddTreeNode(ctx.MousePosition, TreeNodeType.Scale));
                genericMenu.ShowAsContext();
            }
        }

        protected override void ProcessEventInCanvas(CanvasContext ctx)
        {
            base.ProcessEventInCanvas(ctx);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && m_curOpMode == OpMode.None)
            {
                ProcessContextMenu(ctx);
            }
            //鼠标左键点击
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                m_curSelected = null;
                m_curOpConnectPoint = null;
                if ((m_curOpConnectPoint = GetClickOutConnectPoint(ctx.MousePosition)) == null)
                {
                    m_curSelected = GetClickNode(ctx.MousePosition);
                    Repaint();
                }
            }
            //鼠标左键松开
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (m_curOpConnectPoint != null)
                {
                    var inConnPoint = GetClickInConnectPoint(ctx.MousePosition);
                    if (inConnPoint != null)
                    {
                        SetParent(m_curOpConnectPoint.Owner, inConnPoint.Owner, m_curOpConnectPoint == m_curOpConnectPoint.Owner.LOutPoint ? 0 : 1);
                    }
                    m_curOpConnectPoint = null;
                }
                Repaint();
            }
            if (Event.current.keyCode == KeyCode.Delete)
            {
                if (m_curSelected != null)
                {
                    if (m_curSelected.m_isExpand)
                        DeleteTreeNode(m_curSelected as TreeNode);
                    else
                        DeleteTree(m_curSelected);
                }
                Repaint();
            }
            if (Event.current.keyCode == KeyCode.M)
            {
                if (m_curOpMode == OpMode.None && m_curSelected != null)
                {
                    m_moveOpData = new MoveOpData() { opObj = m_curSelected, moveStartPos = m_curSelected.Position };
                    m_curOpMode = OpMode.Move;
                }
            }
            if (m_curOpMode == OpMode.Move)
            {
                m_moveOpData.opObj.Position = ctx.MousePosition;
                //鼠标左键点击
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    if (!m_moveOpData.opObj.m_isExpand)
                    {
                        m_moveOpData.opObj.Position = m_moveOpData.moveStartPos;
                        ResetTreePosition(m_moveOpData.opObj, ctx.MousePosition);
                    }
                    m_curOpMode = OpMode.None;
                }
                //鼠标右键点击
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    m_moveOpData.opObj.Position = m_moveOpData.moveStartPos;
                    m_curOpMode = OpMode.None;
                }
            }
            if (Event.current.keyCode == KeyCode.E && Event.current.type == EventType.KeyDown)
            {
                if (m_curSelected != null && m_curSelected.ChildCount != 0)
                {
                    if (m_curSelected.m_isExpand)
                    {
                        UnexpandTree(m_curSelected);
                    }
                    else
                    {
                        ExpandTree(m_curSelected);
                    }
                }
                Repaint();
            }
        }

        #endregion

        private void OnNodeDataChanged(TreeNode treeNode)
        {
            RayMarchSceneRender raymarch = RayMarchSceneRender.Instance;
            if (raymarch != null)
            {
                int index = m_nodes.FindIndex((elem) => { return elem == treeNode; });
                raymarch.m_nodes[index].param1 = treeNode.m_data.m_param1;//  new Vector4(treeNode.m_data.X, treeNode.m_data.Y, treeNode.m_data.Z, treeNode.m_data.W);
                raymarch.m_nodes[index].param2 = treeNode.m_data.m_param2;
                raymarch.m_nodes[index].param3 = treeNode.m_data.m_param3;
                raymarch.m_nodes[index].param4 = treeNode.m_data.m_param4;
                raymarch.m_nodes[index].param5 = treeNode.m_data.m_param5;
                raymarch.m_nodes[index].param6 = treeNode.m_data.m_param6;
                raymarch.m_nodes[index].param7 = treeNode.m_data.m_param7;
                raymarch.m_nodes[index].param8 = treeNode.m_data.m_param8;
                raymarch.m_nodes[index].param9 = treeNode.m_data.m_param9;
                raymarch.m_nodes[index].param10 = treeNode.m_data.m_param10;
                raymarch.m_nodes[index].param11 = treeNode.m_data.m_param11;
                raymarch.m_nodes[index].param12 = treeNode.m_data.m_param12;
                if ((int)treeNode.m_type < (int)TreeNodeType.Union && (int)treeNode.m_type > (int)TreeNodeType.Root)
                {
                    //primitive
                    raymarch.ReloadComputeBuff();
                }
                else if ((int)treeNode.m_type <= (int)TreeNodeType.Scale && (int)treeNode.m_type >= (int)TreeNodeType.Position)
                {
                    raymarch.RebuildNodeTransform();
                    raymarch.ReloadComputeBuff();
                }
            }
        }

        private void OnDSChanged()
        {
            RayMarchSceneRender rayMarch = RayMarchSceneRender.Instance;
            if (rayMarch == null)
            {
                Debug.LogWarning("RayMarch is null");
                return;
            }
            var root = Root;
            if (root == null)
            {
                Debug.LogWarning("m_root is null");
                return;
            }
            //遍历，保证以root为根的树中没有未设置子节点的节点
            bool passed = true;
            Traversal(root, (node) =>
            {
                if (node.ChildCount == 1)
                {
                    if (node.m_lChild == null)
                    {
                        Debug.LogError(string.Format("node {0} has not set children", node.m_name));
                        passed = false;
                    }
                }
                if (node.ChildCount == 2)
                {
                    if (node.m_lChild == null || node.m_rChild == null)
                    {
                        Debug.LogError(string.Format("node {0} has not set children", node.m_name));
                        passed = false;
                    }
                }
            });
            if (!passed)
                return;

            //更新rayMarch的数据结构
            for (int i = 0; i < m_nodes.Count; i++)
            {
                var node = m_nodes[i];
                rayMarch.m_nodes[i] = new _Node() {
                    type = (int)node.m_type,
                    param1 = node.m_data.m_param1,
                    param2 = node.m_data.m_param2,
                    param3 = node.m_data.m_param3 ,
                    param4 = node.m_data.m_param4 ,
                    param5 = node.m_data.m_param5 ,
                    param6 = node.m_data.m_param6 ,
                    param7 = node.m_data.m_param7,
                    param8 = node.m_data.m_param8,
                    param9 = node.m_data.m_param9,
                    param10 = node.m_data.m_param10,
                    param11 = node.m_data.m_param11,
                    param12 = node.m_data.m_param12,
                };
                _TreeNode treeNode = new _TreeNode() { index = i, data = i, parent = -1, lchild = -1, rchild = -1, next = -1 };
                rayMarch.m_treeNodes[i] = treeNode;
                node.m_data.EventOnChange = (data) =>
                {
                    OnNodeDataChanged(node);
                };
            }
            rayMarch.m_nodeCount = m_nodes.Count;
            rayMarch.m_treeNodeCount = m_nodes.Count;
            rayMarch.m_treeRoot = m_nodes.FindIndex((elem) => { return elem == root; });
            //层次遍历m_nodes，建立_TreeNode的索引
            Traversal(root, (node) =>
            {
                return true;
            }, (parent, child, childIndexForParent) =>
            {
                int index = m_nodes.FindIndex((elem) => { return elem == parent; });
                int childIndex = m_nodes.FindIndex((elem) => { return elem == child; });
                rayMarch.m_treeNodes[childIndex].parent = index;
                if (childIndexForParent == 0)
                {
                    rayMarch.m_treeNodes[index].lchild = childIndex;
                }
                else if (childIndexForParent == 1)
                {
                    rayMarch.m_treeNodes[index].rchild = childIndex;
                }
            });
            rayMarch.UpdateDS();
            Debug.Log("OnDSChange finish");

        }
    }
}
