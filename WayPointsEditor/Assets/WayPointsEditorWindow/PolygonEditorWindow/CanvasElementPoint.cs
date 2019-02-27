using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class CanvasElementPoint : CanvasElementBase
    {
        protected Vector2 m_position;
        protected float m_radius;

        public Vector2 position
        {
            get { return m_position; }
        }

        public float radius
        {
            get { return m_radius; }
        }

        public override bool canDrag
        {
            get
            {
                return true;
            }
        }

        public override bool canSelected
        {
            get { return true; }
        }

        public CanvasElementPoint(Vector2 position, float radius)
        {
            this.m_position = position;
            this.m_radius = radius;
        }

        public void SetPosition(Vector2 pos)
        {
            m_position = pos;
        }

        public override bool TestPoint(Vector2 pos)
        {
            return (pos - this.m_position).magnitude < m_radius;
        }

        public override void Draw(Color color)
        {
            GLHelper.DrawCircle(position, radius, 36, color);
        }

        public override bool Overlap(Rect rect)
        {
            return rect.Contains(this.m_position);
        }
    }

}
