using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public enum EdgeDir
    {
        Single = 0,
        Inverse = 1,
        Dual = 2,
        Count = 3
    }

    public class Edge : CanvasElementBase
    {
        public override bool canDrag { get { return false; } }
        public EdgeDir m_dir = EdgeDir.Dual;
        public Point m_start;
        public Point m_end;

        public override Vector2 position
        {
            get
            {
                return (m_start.position + m_end.position)/2;
            }
            set { }
        }

        public Edge(Point start, Point end, EdgeDir dir = EdgeDir.Dual)
        {
            m_start = start;
            m_end = end;
            m_dir = dir;
        }

        public void ChangeDir()
        {
            int value = (int)m_dir;
            int count = (int)EdgeDir.Count;
            value = value + 1 == count ? 0 : value + 1;
            m_dir = (EdgeDir)value;
        }

        public override void Draw(Color color)
        {
            if(m_dir == EdgeDir.Single)
            {
                GLHelper.DrawSingleArrowLine(m_start.m_position, m_end.m_position, color);
            }else if(m_dir == EdgeDir.Inverse)
            {
                GLHelper.DrawSingleArrowLine(m_end.m_position, m_start.m_position, color);
            }else
            {
                GLHelper.DrawDoubleArrowLine(m_start.m_position, m_end.m_position, color);
            }
        }

        public override bool TestPoint(Vector2 pos)
        {
            if(Vector2.Dot((m_end.m_position - m_start.m_position),(pos - m_start.m_position)) > 0 &&
               Vector2.Dot((m_start.m_position - m_end.m_position), (pos - m_end.m_position)) > 0  )
            {
                var normal = (m_end.m_position - m_start.m_position).GetNormal().normalized;
                var dist = Vector2.Dot((pos - m_start.m_position), normal);
                if(Mathf.Abs(dist) < 2)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool TestRect(Rect rect)
        {
            return false;
        }
    }
}
