using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class PolygonEditorData
    {
        public List<CanvasElementPoint> nodes = new List<CanvasElementPoint>();
        public List<CanvasElementEdge> edges = new List<CanvasElementEdge>();

        public CanvasContex owner { get; set; }

        public CanvasElementPoint AddNode(Vector2 pos)
        {
            CanvasElementPoint node = new CanvasElementPoint(pos, 20);
            this.nodes.Add(node);
            return node;
        }

        public void RemoveNode(CanvasElementPoint node)
        {
           //todo
        }

        public CanvasElementEdge AddEdge(CanvasElementPoint startNode, CanvasElementPoint endNode)
        {
            var edge = new CanvasElementEdge(startNode, endNode);
            edges.Add(edge);
            return edge;
        }

        public void RemoveEdge(CanvasElementEdge edge)
        {
            //todo
        }

        public void Clear()
        {
            nodes.Clear();
            edges.Clear();
        }

    }
}
