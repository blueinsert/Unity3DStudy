using System;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace VertPaint
{
    public partial class VertPaintWindow
    {
        /// <summary>
        /// Saves the current VertPaint configuration out to a template file at the specified path.
        /// </summary>
        /// <param name="filePath">The full file path (including the .xml extension) to where the template file should be stored.</param>
        /// <returns>True if the operation was successful, false if the saving procedure failed in some way.</returns>
        public bool Save(string filePath)
        {
            // Null or empty file paths should be ignored (and avoided altogether).
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("VertPaint: Couldn't save template because the specified file path string is either null or empty.");
                return false;
            }

            try
            {
                // Create a new, well formatted xml document 
                // with all the VertPaint settings in it.
                using (XmlWriter writer = XmlWriter.Create(filePath, XML_WRITER_SETTINGS))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("vertPaintTemplate");
                    writer.WriteAttributeString("version", $"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_HOTFIX}");
                    {
                        writer.WriteElementString("enabled", enabled ? TRUE_STRING : FALSE_STRING);
                        writer.WriteElementString("togglePreview", togglePreview ? TRUE_STRING : FALSE_STRING);
                        writer.WriteElementString("hideTransformHandle", hideTransformHandle ? TRUE_STRING : FALSE_STRING);

                        writer.WriteStartElement("brushSettings");
                        {
                            writer.WriteElementString("radius", radius.ToString());
                            writer.WriteElementString("maxRadius", maxRadius.ToString());
                            writer.WriteElementString("delay", delay.ToString());
                            writer.WriteElementString("opacity", opacity.ToString());
                            writer.WriteStartElement("falloff");
                            {
                                foreach (Keyframe key in falloff.keys)
                                {
                                    writer.WriteStartElement("key");
                                    {
                                        writer.WriteElementString("time", key.time.ToString());
                                        writer.WriteElementString("value", key.value.ToString());
                                        writer.WriteElementString("inTangent", key.inTangent.ToString());
                                        writer.WriteElementString("outTangent", key.outTangent.ToString());
                                    }
                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteEndElement();
                            writer.WriteStartElement("color");
                            {
                                writer.WriteElementString("r", color.r.ToString());
                                writer.WriteElementString("g", color.g.ToString());
                                writer.WriteElementString("b", color.b.ToString());
                                writer.WriteElementString("a", color.a.ToString());
                            }
                            writer.WriteEndElement();
                            writer.WriteElementString("style", ((int)style).ToString());
                            writer.WriteElementString("alpha", alpha.ToString());
                            writer.WriteElementString("blink", blinkBrushWhileResizing ? TRUE_STRING : FALSE_STRING);
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("keyBindings");
                        {
                            writer.WriteElementString("paint", paintKey.ToString());
                            writer.WriteElementString("preview", previewKey.ToString());
                            writer.WriteElementString("modifyRadius", modifyRadiusKey.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("templates");
                        {
                            writer.WriteElementString("autosaveDirectory", autosaveDirectory);
                        }
                        writer.WriteEndElement();

                        writer.WriteElementString("meshOutputDirectory", meshOutputDirectory);

                        writer.WriteStartElement("foldouts");
                        {
                            writer.WriteElementString("help", showHelp ? TRUE_STRING : FALSE_STRING);
                            writer.WriteElementString("brushSettings", showBrushSettings ? TRUE_STRING : FALSE_STRING);
                            writer.WriteElementString("keyBindings", showKeyBindings ? TRUE_STRING : FALSE_STRING);
                            writer.WriteElementString("templates", showTemplates ? TRUE_STRING : FALSE_STRING);
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                // Raise the TemplateSaved event with all the related arguments.
                OnTemplateSaved(new TemplateSavedEventArgs(DateTime.Now, filePath, string.CompareOrdinal(filePath, AutosaveFilePath) == 0));

                // Refresh the asset database to make the new 
                // template appear in the project view instantly.
                AssetDatabase.Refresh();

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VertPaint: Template saving procedure failed. Returning false... Error message: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Saves the favorite VertPaint templates list.
        /// </summary>
        /// <returns>True if the saving operation was successful; false if the procedure failed in some way.</returns>
        public bool SaveFavoriteTemplates()
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(autosaveDirectory + "VertPaint Favorite Templates.xml", XML_WRITER_SETTINGS))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("favoriteVertPaintTemplates");
                    writer.WriteAttributeString("version", $"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_HOTFIX}");
                    {
                        for (int i = 0; i < favoriteTemplates.Count; ++i)
                        {
                            string path = favoriteTemplates[i];
                            writer.WriteStartElement("template");
                            {
                                writer.WriteElementString("guid", AssetDatabase.AssetPathToGUID(path));
                                writer.WriteElementString("path", path);
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                AssetDatabase.Refresh();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("VertPaint: Couldn't save favorite templates list. Error message: " + e.Message);
                return false;
            }
        }
    }
}

// Copyright (C) Raphael Beck, 2017-2021 | https://glitchedpolygons.com