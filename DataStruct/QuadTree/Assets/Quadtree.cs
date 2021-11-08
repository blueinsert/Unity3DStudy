using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 四分树
/// </summary>
public class QuadTree
{
    public int LeafSize = 5;

    class DNode
    {
        public Vector2 position;
        public Object data;
        public DNode next;
    }

    struct TNode
    {
        public int child00;
        public DNode linkedList;
        public byte count;

        public void Add(DNode data)
        {
            data.next = linkedList;
            linkedList = data;
            count++;
        }

        /// <summary>
        /// Distribute the agents in this node among the children.
        /// Used after subdividing the node.
        /// </summary>
        public void Distribute(TNode[] nodes, Rect r)
        {
            Vector2 c = r.center;
            //遍历当前节点的agent
            while (linkedList != null)
            {
                var nx = linkedList.next;
                //根据所在象限将其分别到子节点中
                var index = child00 + (linkedList.position.x > c.x ? 2 : 0) + (linkedList.position.y > c.y ? 1 : 0);
                nodes[index].Add(linkedList);
                linkedList = nx;
            }
            count = 0;
        }

    }

    TNode[] m_nodes = new TNode[16];
    DNode[] m_dNodes = new DNode[10000];
    int m_filledDnodeIndex = 0;
    int m_filledNodes = 1;
    Rect m_bounds;

    public QuadTree()
    {
        for (int i = 0; i < m_dNodes.Length; i++)
        {
            m_dNodes[i] = new DNode();
        }
    }
    /// <summary>Removes all agents from the tree</summary>
    public void Clear()
    {
        m_nodes[0] = new TNode();
        m_filledNodes = 1;
        m_filledDnodeIndex = 0;
    }

    public void SetBounds(Rect r)
    {
        m_bounds = r;
    }

    public Rect GetBounds()
    {
        return m_bounds;
    }

    int GetNodeIndex()
    {
        if (m_filledNodes + 4 >= m_nodes.Length)
        {
            var nds = new TNode[m_nodes.Length * 2];
            for (int i = 0; i < m_nodes.Length; i++) nds[i] = m_nodes[i];
            m_nodes = nds;
        }
        m_nodes[m_filledNodes] = new TNode();
        m_nodes[m_filledNodes].child00 = m_filledNodes;
        m_filledNodes++;
        m_nodes[m_filledNodes] = new TNode();
        m_nodes[m_filledNodes].child00 = m_filledNodes;
        m_filledNodes++;
        m_nodes[m_filledNodes] = new TNode();
        m_nodes[m_filledNodes].child00 = m_filledNodes;
        m_filledNodes++;
        m_nodes[m_filledNodes] = new TNode();
        m_nodes[m_filledNodes].child00 = m_filledNodes;
        m_filledNodes++;
        return m_filledNodes - 4;
    }

    private DNode GetDNode()
    {
        if (m_filledDnodeIndex > m_dNodes.Length-1)
        {
            var temp = new DNode[m_dNodes.Length * 2];
            for (int i = 0; i < m_dNodes.Length; i++) temp[i] = m_dNodes[i];
            m_dNodes = temp;
        }
        return m_dNodes[m_filledDnodeIndex++];
    }

    /// <summary>
    /// Add a new agent to the tree.
    /// Warning: Agents must not be added multiple times to the same tree
    /// </summary>
    public void Insert(Vector2 position, Object data)
    {
        int i = 0;
        Rect r = m_bounds;
        Vector2 p = new Vector2(position.x, position.y);

        var dNode = GetDNode();
        dNode.position = position;
        dNode.data = data;
        dNode.next = null;

        int depth = 0;

        while (true)
        {
            depth++;

            if (m_nodes[i].child00 == i)
            {
                // Leaf node. Break at depth 10 in case lots of agents ( > LeafSize ) are in the same spot
                if (m_nodes[i].count < LeafSize || depth > 10)
                {
                    m_nodes[i].Add(dNode);
                    break;
                }
                else
                {
                    // Split
                    m_nodes[i].child00 = GetNodeIndex();//分配子节点
                                                        //拆分，将agent分配到子节点空间中
                    m_nodes[i].Distribute(m_nodes, r);
                }
            }
            // Note, no else
            if (m_nodes[i].child00 != i)
            {
                // Not a leaf node
                Vector2 c = r.center;
                if (p.x > c.x)
                {
                    if (p.y > c.y)
                    {
                        //第一象限
                        i = m_nodes[i].child00 + 3;
                        r = Rect.MinMaxRect(c.x, c.y, r.xMax, r.yMax);
                    }
                    else
                    {
                        //第四象限
                        i = m_nodes[i].child00 + 2;
                        r = Rect.MinMaxRect(c.x, r.yMin, r.xMax, c.y);
                    }
                }
                else
                {
                    if (p.y > c.y)
                    {
                        //第二象限
                        i = m_nodes[i].child00 + 1;
                        r = Rect.MinMaxRect(r.xMin, c.y, c.x, r.yMax);
                    }
                    else
                    {
                        //第三象限
                        i = m_nodes[i].child00;
                        r = Rect.MinMaxRect(r.xMin, r.yMin, c.x, c.y);
                    }
                }
            }
        }
    }

    
    public void Query (Vector2 p, float radius, Rect bounds, out List<Object> finds) {
        finds = new List<Object>();
        new QuadtreeQuery {
            p = p, 
            radius = radius,
            nodes = m_nodes,
        }.QueryRec(0, bounds, finds);
    }

    struct QuadtreeQuery {
        public Vector2 p;
        public float radius;
        public TNode[] nodes;

        public void QueryRec (int i, Rect r, List<Object> finds) {

            if (nodes[i].child00 == i) {
                // Leaf node
                for (var a = nodes[i].linkedList; a != null; a = a.next) {
                    float d = (a.position - p).magnitude;
                    if (d < radius) {
                        finds.Add(a.data);
                    }
                }
            } else {
                // Not a leaf node
                Vector2 c = r.center;
                if (p.x-radius < c.x) {
                    if (p.y-radius < c.y) {
                        QueryRec(nodes[i].child00, Rect.MinMaxRect(r.xMin, r.yMin, c.x, c.y), finds);
                    }
                    if (p.y+radius > c.y) {
                        QueryRec(nodes[i].child00+1, Rect.MinMaxRect(r.xMin, c.y, c.x, r.yMax), finds);
                    }
                }

                if (p.x+radius > c.x) {
                    if (p.y-radius < c.y) {
                        QueryRec(nodes[i].child00+2, Rect.MinMaxRect(c.x, r.yMin, r.xMax, c.y), finds);
                    }
                    if (p.y+radius > c.y) {
                        QueryRec(nodes[i].child00+3, Rect.MinMaxRect(c.x, c.y, r.xMax, r.yMax), finds);
                    }
                }
            }
        }
    }
    

    public void DebugDraw()
    {
        DebugDrawRec(0, m_bounds);
    }

    void DebugDrawRec(int i, Rect r)
    {
        Debug.DrawLine(new Vector3(r.xMin, 0, r.yMin), new Vector3(r.xMax, 0, r.yMin), Color.white);
        Debug.DrawLine(new Vector3(r.xMax, 0, r.yMin), new Vector3(r.xMax, 0, r.yMax), Color.white);
        Debug.DrawLine(new Vector3(r.xMax, 0, r.yMax), new Vector3(r.xMin, 0, r.yMax), Color.white);
        Debug.DrawLine(new Vector3(r.xMin, 0, r.yMax), new Vector3(r.xMin, 0, r.yMin), Color.white);

        if (m_nodes[i].child00 != i)
        {
            // Not a leaf node
            Vector2 c = r.center;
            DebugDrawRec(m_nodes[i].child00 + 3, Rect.MinMaxRect(c.x, c.y, r.xMax, r.yMax));
            DebugDrawRec(m_nodes[i].child00 + 2, Rect.MinMaxRect(c.x, r.yMin, r.xMax, c.y));
            DebugDrawRec(m_nodes[i].child00 + 1, Rect.MinMaxRect(r.xMin, c.y, c.x, r.yMax));
            DebugDrawRec(m_nodes[i].child00 + 0, Rect.MinMaxRect(r.xMin, r.yMin, c.x, c.y));
        }

        for (DNode a = m_nodes[i].linkedList; a != null; a = a.next)
        {
            var p = m_nodes[i].linkedList.position;
            Debug.DrawLine(new Vector3(p.x, 0, p.y) + Vector3.up, new Vector3(a.position.x, 0, a.position.y) + Vector3.up, new Color(1, 1, 0, 0.5f));
        }
    }
}

