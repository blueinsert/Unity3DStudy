using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VertPaint
{
    public partial class VertPaintWindow
    {
        /// <summary>
        /// Loads up a VertPaint template file (overwriting the current settings).
        /// </summary>
        /// <param name="filePath">The full file path (including the .xml extension) to the template file.</param>
        /// <returns>True if the template was loaded successfully, false if the loading procedure failed in some way.</returns>
        public bool Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                if (string.CompareOrdinal(filePath, AutosaveFilePath) != 0)
                {
                    Debug.LogError("VertPaint: The specified template file path is either null/empty or doesn't point to an existing file. Loading procedure aborted; returning false...");
                }

                return false;
            }

            using XmlReader reader = XmlReader.Create(filePath, XML_READER_SETTINGS);

            try
            {
                while (reader.Read())
                {
                    if (!reader.IsStartElement())
                        continue;

                    string baseLocalName = reader.LocalName;

                    if (string.CompareOrdinal(baseLocalName, "enabled") == 0)
                        enabled = string.CompareOrdinal(reader.ReadString(), TRUE_STRING) == 0;

                    if (string.CompareOrdinal(baseLocalName, "togglePreview") == 0)
                        togglePreview = string.CompareOrdinal(reader.ReadString(), TRUE_STRING) == 0;

                    if (string.CompareOrdinal(baseLocalName, "hideTransformHandle") == 0)
                        hideTransformHandle = string.CompareOrdinal(reader.ReadString(), TRUE_STRING) == 0;

                    if (string.CompareOrdinal(baseLocalName, "brushSettings") == 0)
                    {
                        using XmlReader brushSettingsReader = reader.ReadSubtree();

                        // Prepare a list of keyframes for parsing the falloff curve.
                        IList<Keyframe> falloffKeys = new List<Keyframe>(5);

                        while (brushSettingsReader.Read())
                        {
                            if (!brushSettingsReader.IsStartElement())
                                continue;

                            string brushSettingLocalName = brushSettingsReader.LocalName;

                            if (string.CompareOrdinal(brushSettingLocalName, "radius") == 0)
                                radius = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "maxRadius") == 0)
                                maxRadius = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "delay") == 0)
                                delay = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "opacity") == 0)
                                opacity = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "key") == 0)
                            {
                                using XmlReader falloffReader = brushSettingsReader.ReadSubtree();

                                var keyframe = new Keyframe();
                                while (falloffReader.Read())
                                {
                                    if (falloffReader.IsStartElement())
                                    {
                                        string keyElementLocalName = falloffReader.LocalName;

                                        if (string.CompareOrdinal(keyElementLocalName, "time") == 0)
                                            keyframe.time = float.Parse(falloffReader.ReadString());

                                        if (string.CompareOrdinal(keyElementLocalName, "value") == 0)
                                            keyframe.value = float.Parse(falloffReader.ReadString());

                                        if (string.CompareOrdinal(keyElementLocalName, "inTangent") == 0)
                                            keyframe.inTangent = float.Parse(falloffReader.ReadString());

                                        if (string.CompareOrdinal(keyElementLocalName, "outTangent") == 0)
                                            keyframe.outTangent = float.Parse(falloffReader.ReadString());
                                    }
                                }

                                falloffKeys.Add(keyframe);
                            }

                            if (string.CompareOrdinal(brushSettingLocalName, "r") == 0)
                                color.r = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "g") == 0)
                                color.g = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "b") == 0)
                                color.b = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "a") == 0)
                                color.a = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "style") == 0)
                                style = (BrushStyle)int.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "alpha") == 0)
                                alpha = float.Parse(brushSettingsReader.ReadString());

                            if (string.CompareOrdinal(brushSettingLocalName, "blink") == 0)
                                blinkBrushWhileResizing = string.CompareOrdinal(reader.ReadString(), TRUE_STRING) == 0;
                        }

                        // Apply the parsed falloff curve.
                        falloff = new AnimationCurve(falloffKeys.ToArray());
                    }

                    if (string.CompareOrdinal(baseLocalName, "keyBindings") == 0)
                    {
                        using XmlReader keyBindingsReader = reader.ReadSubtree();

                        while (keyBindingsReader.Read())
                        {
                            if (!keyBindingsReader.IsStartElement())
                                continue;

                            string keyBindingLocalName = keyBindingsReader.LocalName;

                            if (string.CompareOrdinal(keyBindingLocalName, "paint") == 0)
                                paintKey = (KeyCode)Enum.Parse(typeof(KeyCode), keyBindingsReader.ReadString());

                            if (string.CompareOrdinal(keyBindingLocalName, "preview") == 0)
                                previewKey = (KeyCode)Enum.Parse(typeof(KeyCode), keyBindingsReader.ReadString());

                            if (string.CompareOrdinal(keyBindingLocalName, "modifyRadius") == 0)
                                modifyRadiusKey = (KeyCode)Enum.Parse(typeof(KeyCode), keyBindingsReader.ReadString());
                        }
                    }

                    if (string.CompareOrdinal(baseLocalName, "templates") == 0)
                    {
                        using XmlReader templatesReader = reader.ReadSubtree();

                        while (templatesReader.Read())
                        {
                            if (!templatesReader.IsStartElement())
                                continue;

                            string templatesSettingLocalName = templatesReader.LocalName;

                            if (string.CompareOrdinal(templatesSettingLocalName, "autosaveDirectory") == 0)
                                autosaveDirectory = templatesReader.ReadString();
                        }
                    }

                    if (string.CompareOrdinal(baseLocalName, "meshOutputDirectory") == 0)
                        meshOutputDirectory = reader.ReadString();

                    if (string.CompareOrdinal(baseLocalName, "foldouts") != 0)
                        continue;

                    using XmlReader foldoutsReader = reader.ReadSubtree();

                    while (foldoutsReader.Read())
                    {
                        if (!foldoutsReader.IsStartElement())
                            continue;

                        string foldoutLocalName = foldoutsReader.LocalName;

                        if (string.CompareOrdinal(foldoutLocalName, "help") == 0)
                            showHelp = bool.Parse(foldoutsReader.ReadString());

                        if (string.CompareOrdinal(foldoutLocalName, "brushSettings") == 0)
                            showBrushSettings = bool.Parse(foldoutsReader.ReadString());

                        if (string.CompareOrdinal(foldoutLocalName, "keyBindings") == 0)
                            showKeyBindings = bool.Parse(foldoutsReader.ReadString());

                        if (string.CompareOrdinal(foldoutLocalName, "templates") == 0)
                            showTemplates = bool.Parse(foldoutsReader.ReadString());
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VertPaint: Template loading procedure failed; returning false... Error message: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Loads the favorite templates list from the last known favorites list autosave location.
        /// </summary>
        /// <returns>True if the favorite templates list was loaded successfully, false if the loading procedure failed in some way.</returns>
        public bool LoadFavoriteTemplates()
        {
            string path = FavoriteTemplatesFilePath;

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("VertPaint: there was an error loading the favorite templates list. Perhaps check the involved file paths...");
                return false;
            }

            using XmlReader reader = XmlReader.Create(path, XML_READER_SETTINGS);

            try
            {
                favoriteTemplates.Clear();

                while (reader.Read())
                {
                    if (!reader.IsStartElement() || string.CompareOrdinal(reader.LocalName, "template") != 0)
                    {
                        continue;
                    }

                    using XmlReader templateReader = reader.ReadSubtree();

                    while (templateReader.Read())
                    {
                        if (!templateReader.IsStartElement())
                        {
                            continue;
                        }

                        string templatePath = string.Empty;

                        if (string.CompareOrdinal(templateReader.LocalName, "path") == 0)
                        {
                            templatePath = templateReader.ReadString();
                        }

                        if (!string.IsNullOrEmpty(templatePath) && File.Exists(templatePath))
                        {
                            favoriteTemplates.Add(templatePath);
                            break;
                        }

                        if (string.CompareOrdinal(templateReader.LocalName, "guid") == 0)
                        {
                            templatePath = AssetDatabase.GUIDToAssetPath(templateReader.ReadString());
                        }

                        if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                        {
                            continue;
                        }

                        favoriteTemplates.Add(templatePath);
                        break;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VertPaint: Couldn't load the list of favorite templates; returning false... Error message: " + e.Message);
                return false;
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com