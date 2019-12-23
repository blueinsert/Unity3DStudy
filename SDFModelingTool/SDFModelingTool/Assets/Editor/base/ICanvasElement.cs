using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public interface ICanvasElement
    {
        bool TestPoint(Vector2 pos);
       
        Color Color { get; }
        Color SelectedColor { get; }
        void Draw(CanvasContext ctx);

        Rect Rect { get; }
        Vector2 Position { get; set; }

        
    }

}
