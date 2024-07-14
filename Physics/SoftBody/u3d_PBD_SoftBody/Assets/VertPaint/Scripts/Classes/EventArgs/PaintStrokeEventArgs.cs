using System;
using UnityEngine;

namespace VertPaint
{
    /// <summary>
    /// The paint stroke relevant arguments passed to event subscribers.<para> </para>
    /// These are NOT the linked brush settings used to perform the brush stroke, 
    /// as those can be found by casting the passed "object sender" to (VertPaintWindow) and accessing its public properties.
    /// </summary>
    public class PaintStrokeEventArgs : EventArgs
    {
        public RaycastHit BrushRaycastHit { get; protected set; }
        public DateTime PaintTime { get; protected set; }

        public PaintStrokeEventArgs(RaycastHit brushRaycastHit, DateTime paintTime)
        {
            BrushRaycastHit = brushRaycastHit;
            PaintTime = paintTime;
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com