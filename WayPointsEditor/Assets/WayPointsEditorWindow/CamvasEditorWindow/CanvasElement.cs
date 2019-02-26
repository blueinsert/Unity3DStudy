using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public abstract class CanvasElementBase
    {
        public abstract bool TestPoint(Vector2 pos);
        public abstract bool canDrag { get; }
        public abstract void Draw(Color color);
    }

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

        public CanvasElementPoint( Vector2 position, float radius)
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
    }

    public class CanvasElementEdge : CanvasElementBase
    {
        protected CanvasElementPoint m_point1;
        protected CanvasElementPoint m_point2;

        public CanvasElementPoint point1 {
            get { return m_point1; }
        }

        public CanvasElementPoint point2
        {
            get { return m_point2; }
        }

        public override bool canDrag
        {
            get
            {
                return false;
            }
        }

        public CanvasElementEdge(CanvasElementPoint point1, CanvasElementPoint point2)
        {
            this.m_point1 = point1;
            this.m_point2 = point2;
        }

        public override bool TestPoint(Vector2 pos)
        {
            return false;
        }

        public override void Draw(Color color)
        {
            GLHelper.DrawLine(point1.position, point2.position, Color.blue);
        }
    }


}
