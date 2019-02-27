using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class CanvasElementEdge : CanvasElementBase
    {
        protected CanvasElementPoint m_point1;
        protected CanvasElementPoint m_point2;

        public CanvasElementPoint point1
        {
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

        public override bool canSelected
        {
            get { return false; }
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

        public override bool Overlap(Rect rect)
        {
            return false;
        }
    }
}