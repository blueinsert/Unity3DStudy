using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public abstract class CanvasElementBase
    {
        public abstract bool TestPoint(Vector2 pos);
        public abstract bool TestRect(Rect rect);
        public abstract bool canDrag { get; }
        public Vector2 m_dragStartPosition;
        public abstract Vector2 position { get; set; }
        public abstract void Draw(Color color);
    }

}
