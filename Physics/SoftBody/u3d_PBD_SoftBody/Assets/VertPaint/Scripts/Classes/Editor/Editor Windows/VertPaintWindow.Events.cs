using System;

namespace VertPaint
{
    public partial class VertPaintWindow
    {
        /// <summary>
        /// The <see cref="Painted"/> event is raised on each paint stroke and 
        /// the passed <see cref="PaintStrokeEventArgs"/> argument contains information 
        /// about the affected mesh as well as the paint stroke itself.<para> </para>
        /// To access the paint stroke related brush settings, cast the "object sender"
        /// to the <see cref="VertPaintWindow"/> type and access its public properties (such as <see cref="VertPaintWindow.Radius"/>, <see cref="VertPaintWindow.Opacity"/>, etc...).
        /// </summary>
        public event EventHandler<PaintStrokeEventArgs> Painted;

        /// <summary>
        /// The <see cref="Erased"/> event is raised every time the user erases vertex colors with the brush.<para> </para>
        /// The passed <see cref="PaintStrokeEventArgs"/> argument contains information about the affected mesh as well as the erase stroke itself.
        /// </summary>
        public event EventHandler<PaintStrokeEventArgs> Erased;

        /// <summary>
        /// The <see cref="PreviewStateChanged"/> event is raised whenever the vertex color preview shader has been enabled/disabled.
        /// </summary>
        public event EventHandler<PreviewStateChangedEventArgs> PreviewStateChanged;

        /// <summary>
        /// This event is raised every time a template has been saved.
        /// </summary>
        public event EventHandler<TemplateSavedEventArgs> TemplateSaved;
        
        // Dispatchers:
        
        protected virtual void OnPainted(PaintStrokeEventArgs eventArgs)
        {
            Painted?.Invoke(this, eventArgs);
        }

        protected virtual void OnErased(PaintStrokeEventArgs eventArgs)
        {
            Erased?.Invoke(this, eventArgs);
        }

        protected virtual void OnPreviewStateChanged(PreviewStateChangedEventArgs eventArgs)
        {
            PreviewStateChanged?.Invoke(this, eventArgs);
        }

        protected virtual void OnTemplateSaved(TemplateSavedEventArgs eventArgs)
        {
            TemplateSaved?.Invoke(this, eventArgs);
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com