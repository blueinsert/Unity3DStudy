using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace bluebean.ShaderToyBrowser
{
    public class ConfigLoader {

        public static T Load<T>(string path) where T:class {
            var p = Application.dataPath + path;
            var file = File.OpenText(p);          
            var config = JsonUtility.FromJson<T>(file.ReadToEnd());
            file.Close();
            return config;
        }

        public static void Save<T>(string path, T obj) {
            var p = Application.dataPath + path;
            string json = JsonUtility.ToJson(obj);
            var stream = File.CreateText(p);
            stream.Write(json);
            stream.Close();
        }
    }
}
