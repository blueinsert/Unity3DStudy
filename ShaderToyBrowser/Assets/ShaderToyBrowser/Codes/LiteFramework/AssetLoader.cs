using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean.ShaderToyBrowser
{
    public class AssetLoader
    {
        public static T Load<T>(string path) where T: UnityEngine.Object
        {
            if (typeof(T) == typeof(Sprite)) {
                var objs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var o in objs) {
                    if(o.GetType() == typeof(Sprite))
                    {
                        return o as T;
                    }
                }
                return null;
            }
            else
            {
                var obj = AssetDatabase.LoadAssetAtPath<T>(path);
                return obj;
            }
        }
       
    }
}
