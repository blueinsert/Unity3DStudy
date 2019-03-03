using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class Point : CanvasElementBase
    {
        public override bool canDrag { get { return true; } }

        public override Vector2 position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        public Vector2 m_position;
        public float m_radius;

        public Point(Vector2 pos, float radius)
        {
            m_position = pos;
            m_radius = radius;
        }  

        public override void Draw(Color color)
        {
            GLHelper.DrawCircleSolid(m_position, m_radius, 36, color);
        }

        public override bool TestPoint(Vector2 pos)
        {
            return (pos - m_position).magnitude < m_radius;
        }

        public override bool TestRect(Rect rect)
        {
            return rect.Contains(m_position);
        }
    }
}
