using System;

namespace VertPaint
{
    public class PreviewStateChangedEventArgs : EventArgs
    {
        public bool Previewing { get; protected set; }

        public PreviewStateChangedEventArgs(bool nowPreviewing)
        {
            Previewing = nowPreviewing;
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com