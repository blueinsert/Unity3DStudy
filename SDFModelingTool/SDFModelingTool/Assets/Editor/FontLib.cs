using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace bluebean
{
    public class FontLib
    {
        private const string FontFolder = "Font";

        private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        private static string getFullPath(string root, string targetFolderName)
        {
            string[] dirs = Directory.GetDirectories(root, targetFolderName, SearchOption.AllDirectories);

            // Return first occurance containing targetFolderName.
            if (dirs.Length != 0)
            {
                return dirs[0];
            }

            // Could not find anything.
            return "";
        }

        public static string GetFontFolderPath()
        {
            string fullpath = getFullPath(Application.dataPath, FontFolder);
            if (!string.IsNullOrEmpty(fullpath))
            {
                // Return the texture folder path relative to Unity's Asset folder.
                int index = fullpath.IndexOf("Assets");
                string localPath = fullpath.Substring(index);
                return localPath;
            }

            Debug.LogError("Could not find folder: " + FontFolder);
            return "";
        }

        public static void Clear() {
            _fonts.Clear();
        }

        public static void LoadFont(string name) {
            string filename = name + "." + "fontsettings";
            string path = GetFontFolderPath() + "/" + filename;
            var font = AssetDatabase.LoadAssetAtPath<Font>(path);
            if (font != null)
            {
                _fonts.Add(name, font);
            }
            else
            {
                Debug.LogError("The font: " + path + " could not be found.");
            }
        }

        public static Font GetFont(string name) {
            if (_fonts.ContainsKey(name))
            {
                return _fonts[name];
            }

            Debug.LogError(name + " is not loaded in the font library.");
            return null;
        }
    }
}
