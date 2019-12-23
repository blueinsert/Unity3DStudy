using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean
{
    public enum TreeNodeType
    {
        Root = 0,
        Sphere,
        Plane,
        Box,
        RoundBox,
        Torus,
        Cylinder,
        Cone,
        HexPrism,
        TriPrism,
        Capsule,
        Triangle,
        Quad,
        Union,
        Subtraction,
        Intersection,
        Position,
        Rotate,
        Scale,
    }

    [Serializable]
    public class NodeData {

        public Action<NodeData> EventOnChange;

        [SerializeField]
        public float m_param1;
        [SerializeField]
        public float m_param2;
        [SerializeField]
        public float m_param3;
        [SerializeField]
        public float m_param4;
        [SerializeField]
        public float m_param5;
        [SerializeField]
        public float m_param6;
        [SerializeField]
        public float m_param7;
        [SerializeField]
        public float m_param8;
        [SerializeField]
        public float m_param9;
        [SerializeField]
        public float m_param10;
        [SerializeField]
        public float m_param11;
        [SerializeField]
        public float m_param12;

        public float X { get { return m_param1; }
            set {
                if (m_param1 != value) {
                    m_param1 = value;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }
        public float Y { get { return m_param2; }
            set {
                if (m_param2 != value) {
                    m_param2 = value;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }
        public float Z { get { return m_param3; }
            set {
                if (m_param3 != value) {
                    m_param3 = value;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }
        public float W { get { return m_param4; }
            set {
                if (m_param4 != value) {
                    m_param4 = value;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }

        public Vector3 Position1 {
            get
            {
                return new Vector3(m_param1, m_param2, m_param3);
            }
            set
            {
                if(m_param1!=value.x || m_param2!=value.y || m_param3 != value.z)
                {
                    m_param1 = value.x;
                    m_param2 = value.y;
                    m_param3 = value.z;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }

        public Vector3 Position2
        {
            get
            {
                return new Vector3(m_param4, m_param5, m_param6);
            }
            set
            {
                if (m_param4 != value.x || m_param5 != value.y || m_param6 != value.z)
                {
                    m_param4 = value.x;
                    m_param5 = value.y;
                    m_param6 = value.z;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }

        public float R {
            get {
                return m_param7;
            }
            set
            {
                if (m_param7 != value)
                {
                    m_param7 = value;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }

        public Vector3 Position3
        {
            get
            {
                return new Vector3(m_param7, m_param8, m_param9);
            }
            set
            {
                if (m_param7 != value.x || m_param8 != value.y || m_param9 != value.z)
                {
                    m_param7 = value.x;
                    m_param8 = value.y;
                    m_param9 = value.z;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }

        public Vector3 Position4
        {
            get
            {
                return new Vector3(m_param10, m_param11, m_param12);
            }
            set
            {
                if (m_param10 != value.x || m_param11 != value.y || m_param12 != value.z)
                {
                    m_param10 = value.x;
                    m_param11 = value.y;
                    m_param12 = value.z;
                    if (EventOnChange != null)
                        EventOnChange(this);
                }
            }
        }
    }

    [Serializable]
    public class TreeNode : ICanvasElement
    {
        public static readonly Vector2 SIZE = new Vector2(125, 100);

        public Vector2 Position { get { return m_position; } set { m_position = value; } }
        public Color Color { get { return m_color; } }
        public Color SelectedColor { get { return m_selectedColor; } }
        public Rect Rect { get { return new Rect(m_position.x-SIZE.x/2,m_position.y- SIZE.y/2, SIZE.x, SIZE.y); } }
        private Texture2D ShowTexture
        {
            get
            {
                if (m_showTexture == null)
                {
                    string texture = "";
                    switch (m_type)
                    {
                        case TreeNodeType.Box:
                            texture = "Box"; break;
                        case TreeNodeType.Sphere:
                            texture = "Sphere"; break;
                        case TreeNodeType.Position:
                            texture = ""; break;
                        case TreeNodeType.Rotate:
                            texture = ""; break;
                        case TreeNodeType.Scale:
                            texture = ""; break;
                        case TreeNodeType.Union:
                            texture = ""; break;
                        case TreeNodeType.Subtraction:
                            texture = ""; break;
                        case TreeNodeType.Intersection:
                            texture = ""; break;
                        case TreeNodeType.Root:
                            texture = ""; break;
                    }
                    if(!string.IsNullOrEmpty(texture))
                        m_showTexture = TextureLib.GetTexture(texture);
                }
                return m_showTexture;
            }
        }
        public int ChildCount
        {
            get
            {
                int childCount = 0;
                switch (m_type)
                {
                    case TreeNodeType.Box:
                        childCount = 0;
                        break;
                    case TreeNodeType.Sphere:
                        childCount = 0;
                        break;
                    case TreeNodeType.Position:
                    case TreeNodeType.Rotate:
                    case TreeNodeType.Scale:
                        childCount = 1;
                        break;
                    case TreeNodeType.Union:
                    case TreeNodeType.Subtraction:
                    case TreeNodeType.Intersection:
                        childCount = 2;
                        break;
                    case TreeNodeType.Root:
                        childCount = 1;
                        break;
                }
                return childCount;
            }
        }

        public ConnectPoint InPoint
        {
            get
            {
                if (m_type!= TreeNodeType.Root && m_inPoint == null) {
                    m_inPoint = new ConnectPoint(this, new Vector2(0, SIZE.y / 2), SIZE.x / 10, ConnectPointType.In);
                }
                return m_inPoint;
            }
        }

        public ConnectPoint LOutPoint {
            get
            {
                if (ChildCount == 0)
                    return null;
                if (m_outPoint1 == null) {
                    float segLen = SIZE.x / (ChildCount + 1);
                    m_outPoint1 = new ConnectPoint(this, new Vector2(-SIZE.x / 2 + (0 + 1) * segLen, -SIZE.y / 2), SIZE.x / 10, ConnectPointType.Out);
                }
                return m_outPoint1;
            }
        }

        public ConnectPoint ROutPoint
        {
            get
            {
                if (ChildCount <= 1)
                    return null;
                if (m_outPoint2 == null)
                {
                    float segLen = SIZE.x / (ChildCount + 1);
                    m_outPoint2 = new ConnectPoint(this, new Vector2(-SIZE.x / 2 + (1 + 1) * segLen, -SIZE.y / 2), SIZE.x / 10, ConnectPointType.Out);
                }
                return m_outPoint2;
            }
        }

        public List<ConnectPoint> ConnectPoints {
            get {
                if (m_connectPoints == null) {
                    m_connectPoints = new List<ConnectPoint>() { InPoint, LOutPoint, ROutPoint };
                }
                return m_connectPoints;
            }
        }
        public TreeNode Parent {
            get { return m_parent; }
            set { m_parent = value; }
        }

        [NonSerialized]
        private TreeNode m_parent;
        [NonSerialized]
        private Color m_color = new Color(0.9f, 0.9f, 0.9f,1f);
        [NonSerialized]
        private Color m_selectedColor = new Color(0.6f, 0.6f, 0.6f,1f);
        [NonSerialized]
        private Texture2D m_showTexture;//显示图片
        [NonSerialized]
        private ConnectPoint m_inPoint;
        [NonSerialized]
        private ConnectPoint m_outPoint1;
        [NonSerialized]
        private ConnectPoint m_outPoint2;
        [NonSerialized]
        private List<ConnectPoint> m_connectPoints;
        [NonSerialized]
        public TreeNode m_lChild = null;
        [NonSerialized]
        public TreeNode m_rChild = null;
        [NonSerialized]
        public bool m_isExpand = true;

        #region 序列化属性
        [SerializeField]
        private Vector2 m_position;
        [SerializeField]
        public string m_name;
        [SerializeField]
        public TreeNodeType m_type;
        [SerializeField]
        public NodeData m_data = new NodeData();
        [SerializeField]
        public int m_parentIndex;
        [SerializeField]
        public int m_lIndex;
        [SerializeField]
        public int m_rIndex;
        #endregion

        public TreeNode() { }

        public TreeNode(string name, Vector2 position, TreeNodeType type) {
            m_name = name;
            m_type = type;
            m_position = position;
            switch (m_type)
            {
                case TreeNodeType.Box:
                    m_data.X = 1; m_data.Y = 1; m_data.Z = 1;
                    break;
                case TreeNodeType.Sphere:
                    m_data.W = 1;
                    break;
                case TreeNodeType.Scale:
                    m_data.X = 1; m_data.Y = 1; m_data.Z = 1;
                    break;
            }

        }

        public void Draw(CanvasContext ctx)
        {
            //Debug.Log("draw node name:" + m_name);
            ctx.DrawTexture(Rect, ShowTexture);
            foreach (var cp in ConnectPoints) {
                if (cp != null && (m_isExpand || (!m_isExpand && cp.Type == ConnectPointType.In))) {
                    ctx.DrawCircleSolid(cp.Position, SIZE.x / 10, new Color(0, 0, 0));
                }
            }
            ctx.SetFont("font");
            ctx.Color = new Color(0.2f,0.2f,0.2f,1);
            ctx.DrawText(Rect, m_name,1f);
            if (!m_isExpand)
            {
                ctx.DrawSolidTriangle(Rect.position, new Vector2(Rect.position.x + Rect.size.x / 2, Rect.position.y - SIZE.y/5), new Vector2(Rect.position.x + Rect.size.x, Rect.position.y), new Color(0.1f, 0.3f, 0.9f, 0.9f));
            }
        }

        public ConnectPoint GetClickOutConnectPoint(Vector2 pos) {
            if (ConnectPoints == null || ConnectPoints.Count <= 0)
                return null;
            foreach (var cp in ConnectPoints)
            {
                if (cp!=null && cp.Type == ConnectPointType.Out && cp.TestPoint(pos))
                {
                    return cp;
                }
            }
            return null;
        }

        public ConnectPoint GetClickInConnectPoint(Vector2 pos)
        {
            if (InPoint == null)
                return null;
            if (InPoint.TestPoint(pos))
            {
                return InPoint;
            }
            return null;
        }

        public bool TestPoint(Vector2 pos)
        {
           if(GetClickInConnectPoint(pos)!=null)
                return false;
           return Rect.Contains(pos);
        }

    }
}
