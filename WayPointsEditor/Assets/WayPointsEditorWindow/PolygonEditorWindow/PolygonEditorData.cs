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

        public CanvasElementBase IntersectTest(Vector2 pos)
        {
            foreach (var point in this.nodes)
            {
                if (point.TestPoint(pos))
                    return point;
            }
            return null;
        }

        public CanvasElementBase IntersectTest(Vector2 point1, Vector2 point2)
        {
            var center = (point1 + point2) / 2;
            var width = Mathf.Abs(point1.x - point2.x);
            var height = Mathf.Abs(point1.y - point2.y);
            Vector2 size = new Vector2(width, height);
            Rect rect = new Rect(center - size/2, size);
            foreach (var point in this.nodes)
            {
                if (rect.Contains(point.position))
                    return point;
            }
            return null;
        }

    }
}
