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

}
