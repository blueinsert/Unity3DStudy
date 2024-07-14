using System;
using UnityEngine;

namespace VertPaint
{
    public class ShaderPrepEventArgs : EventArgs
    {
        /// <summary>
        /// The repacked <see cref="Texture2D"/> result.
        /// </summary>
        public Texture2D RepackedTexture { get; private set; }

        /// <summary>
        /// The repacked texture file bytes (encoded to .png).
        /// </summary>
        public byte[] RepackedTextureBytes { get; private set; }

        /// <summary>
        /// The full file path (including the .png extension) to where
        /// the repacked <see cref="RepackedTexture"/> texture asset is stored.
        /// </summary>
        public string FilePath { get; private set; }

        public ShaderPrepEventArgs(Texture2D repackedTexture, byte[] repackedTextureBytes, string filePath)
        {
            RepackedTexture = repackedTexture;
            RepackedTextureBytes = repackedTextureBytes;
            FilePath = filePath;
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com