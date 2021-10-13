using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    /// <summary>
    /// waypoints graph
    /// </summary>
    public class WayPointsData
    {
        public List<Point> m_points = new List<Point>();
        public List<Edge> m_edges = new List<Edge>();
        public event Action<CanvasElementBase> onAddElement = (e)=> { };
        public event Action<CanvasElementBase> onRemoveElement = (e) => { };

        public Point AddPoint(Vector2 position)
        {
            Point point = new Point(position, 2);
            m_points.Add(point);
            onAddElement(point);
            return point;
        }

        public void RemovePoint(Point point)
        {
            if (m_points.Contains(point))
            {
                m_points.Remove(point);
                onRemoveElement(point);
                foreach (var edge in new List<Edge>(m_edges))
                {
                    if (edge.m_start == point || edge.m_end == point)
                        RemoveEdge(edge);
                }
            } 
        }

        public Edge AddEdge(Point start, Point end, EdgeDir edgeDir = EdgeDir.Dual)
        {
            Edge edge = new Edge(start, end, edgeDir);
            m_edges.Add(edge);
            onAddElement(edge);
            return edge;
        }

        public void RemoveEdge(Edge edge)
        {
            if (m_edges.Contains(edge))
            {
                m_edges.Remove(edge);
                onRemoveElement(edge);
            }
        } 

        public bool ContainEdge(Point start, Point end)
        {
            foreach(var edge in m_edges)
            {
                if((edge.m_start == start && edge.m_end == end) || (edge.m_end == start && edge.m_start == end))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
