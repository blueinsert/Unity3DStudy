using System.Xml;
using UnityEngine;

namespace VertPaint
{
    public partial class VertPaintWindow
    {
        /// <summary>
        /// The current VertPaint version (major version number). <para> </para>
        /// <see href="https://semver.org/"/>
        /// </summary>
        public const int VERSION_MAJOR = 1;

        /// <summary>
        /// The current VertPaint version (minor version number).<para> </para>
        /// <see href="https://semver.org/"/>
        /// </summary>
        public const int VERSION_MINOR = 1;

        /// <summary>
        /// The current VertPaint version (hotfix number).<para> </para>
        /// <see href="https://semver.org/"/>
        /// </summary>
        public const int VERSION_HOTFIX = 1;

        // VertPaintWindow settings:
        private const int MIN_WINDOW_WIDTH = 387;
        private const int MIN_WINDOW_HEIGHT = 34;
        private const int MAX_WINDOW_WIDTH = 750;
        private const int MAX_WINDOW_HEIGHT = 595;
        
        // Various default values for the VertPaint settings:
        private const bool DEFAULT_ENABLED = false;
        private const bool DEFAULT_TOGGLE_PREVIEW = false;
        private const bool DEFAULT_HIDE_TRANSFORM_HANDLE = true;

        private const float DEFAULT_RADIUS = 1.5f;
        private const float DEFAULT_MAX_RADIUS = 10.0f;
        private const float DEFAULT_DELAY = 0.025f;
        private const float DEFAULT_OPACITY = 0.25f;
        private const float DEFAULT_ALPHA = 0.2f;

        private const BrushStyle DEFAULT_BRUSH_STYLE = BrushStyle.Disc;
        private const bool DEFAULT_BLINK_BRUSH_WHILE_RESIZING = true;

        private const KeyCode DEFAULT_PAINT_KEY = KeyCode.Mouse0;
        private const KeyCode DEFAULT_PREVIEW_KEY = KeyCode.C;
        private const KeyCode DEFAULT_MODIFY_RADIUS_KEY = KeyCode.X;

        // Constant strings and paths that are used often throughout this document:
        private const string TRUE_STRING = "true";
        private const string FALSE_STRING = "false";
        private const string DEFAULT_MESH_OUTPUT_DIRECTORY = "Assets/VertPaint/Output/Meshes/";
        private const string DEFAULT_AUTOSAVE_DIRECTORY = "Assets/VertPaint/Templates/Autosave/";

        private const string HELP_TEXT =
            "VertPaint is a simple vertex color painting utility that ships with a set of simple example vertex color blending shaders, a shader prep utility to prepare your texture maps for your materials and some example scenes.\n\nTo paint vertex colors on your currently selected mesh, press the specified paint input key (default is the left mouse button). The vertices inside the brush area will be colored according to the specified falloff, opacity, color and delay value. Hover your mouse cursor over each setting’s label for more information about what each setting does.\n\nTo erase the painted vertex colors, paint with the shift key down. You can also start from scratch by clicking on \"Discard\". Changing the brush radius can be achieved interactively inside the scene view by keeping the \"Modify radius key\" down and dragging the mouse cursor; once happy, let go of the key and the sampled value will be your new brush radius.\n\nYou can also preview the vertex colors by pressing and holding the \"Preview vertex colors\" key.\n\nIf you’re happy with your result, click on \"Apply\" and the painted vertex colors will be automatically saved out to a mesh asset on disk (which is then highlighted inside the project view for you).\n\nSince you can only work on one mesh simultaneously, the controls for selecting GameObjects are slightly different than the standard Unity editor controls: to change the mesh you’re currently working on, select the new one in the scene view whilst holding down control (command on Mac). Note that by reselecting (or also by closing the VertPaint window), the changes you've made to the vertex colors will be automatically discarded.\nWhenever you come up with a result you like, hit apply! It doesn't cost you anything. You can always clean up later with the \"Clean up\" button.";

        private static readonly Color DEFAULT_COLOR = Color.red;
        private static readonly AnimationCurve DEFAULT_FALLOFF = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

        // XmlWriterSettings and XmlReaderSettings for saving/loading VertPaint templates.
        private static readonly XmlWriterSettings XML_WRITER_SETTINGS = new XmlWriterSettings { Indent = true, CloseOutput = true };
        private static readonly XmlReaderSettings XML_READER_SETTINGS = new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true };

        // Cached shader property IDs:
        private static readonly int METALLIC_PROP_ID = Shader.PropertyToID("_Metallic");
        private static readonly int GLOSSINESS_PROP_ID = Shader.PropertyToID("_Glossiness");
        private static readonly int SRC_BLEND_PROP_ID = Shader.PropertyToID("_SrcBlend");
        private static readonly int DST_BLEND_PROP_ID = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWRITE_PROP_ID = Shader.PropertyToID("_ZWrite");
        private static readonly int EMISSION_COLOR_PROP_ID = Shader.PropertyToID("_EmissionColor");
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com