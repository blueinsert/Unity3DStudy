using System;

namespace VertPaint
{
    public class TemplateSavedEventArgs : EventArgs
    {
        /// <summary>
        /// The time at which the template was saved.
        /// </summary>
        public DateTime SaveTime { get; private set; }

        /// <summary>
        /// The full destination file path (including the extension) to where the template was saved.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Was this template saved automatically (autosave functionality)?
        /// </summary>
        public bool Autosave { get; private set; }

        /// <summary>
        /// Construct a <see cref="TemplateSavedEventArgs"/> data class to be used with template-related events.
        /// </summary>
        /// <param name="saveTime">The time at which the template was saved.</param>
        /// <param name="filePath">The full destination file path (including the extension) to where the template was saved.</param>
        /// <param name="autosave">Was this template saved automatically (autosave functionality)?</param>
        public TemplateSavedEventArgs(DateTime saveTime, string filePath, bool autosave)
        {
            SaveTime = saveTime;
            FilePath = filePath;
            Autosave = autosave;
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com