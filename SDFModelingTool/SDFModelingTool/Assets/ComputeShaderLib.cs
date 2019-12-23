using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace bluebean
{
    public class ComputeShaderLib
    {
        private const string Folder = "ComputeShader";

        private static Dictionary<string, ComputeShader> _computeShaders = new Dictionary<string, ComputeShader>();

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

        private static string GetFontFolderPath()
        {
            string fullpath = getFullPath(Application.dataPath, Folder);
            if (!string.IsNullOrEmpty(fullpath))
            {
                // Return the texture folder path relative to Unity's Asset folder.
                int index = fullpath.IndexOf("Assets");
                string localPath = fullpath.Substring(index);
                return localPath;
            }

            Debug.LogError("Could not find folder: " + Folder);
            return "";
        }

        public static void Clear()
        {
            _computeShaders.Clear();
        }

        public static void LoadComputeShader(string name)
        {
            string filename = name + "." + "fontsettings";
            string path = GetFontFolderPath() + "/" + filename;
            var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(path);
            if (shader != null)
            {
                _computeShaders.Add(name, shader);
            }
            else
            {
                Debug.LogError("The computeShader: " + path + " could not be found.");
            }
        }

        public static ComputeShader GetComputeShader(string name)
        {
            if (_computeShaders.ContainsKey(name))
            {
                return _computeShaders[name];
            }
            Debug.LogError(name + " is not loaded in the computeShader library.");
            return null;
        }
    }
}
