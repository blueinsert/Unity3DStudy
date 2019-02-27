using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public abstract class CanvasElementBase
    {
        public abstract bool TestPoint(Vector2 pos);
        public abstract bool canDrag { get; }
        public abstract bool canSelected { get; }
        public abstract bool Overlap(Rect rect);
        public abstract void Draw(Color color);

        public virtual Color selectedColor {
            get
            {
                return Color.red;
            }
        }

        public virtual Color unSelectedColor
        {
            get { return Color.blue; }
        }

    }

}
