using UnityEngine;
using UnityEditor;
using System.Collections;

namespace zEditorWindow
{
	public class CanvasScriptable<T> : ScriptableObject where T : ScriptableObject
	{
		static string AssetPath {
			get {
				return "Assets/zCanvasEditor/CanvasData/" + typeof(T).ToString() + ".asset";
			}
		}

		static T _instance;

		public static T Instance {
			get {
				if (_instance == null) {
					_instance = AssetDatabase.LoadAssetAtPath<T> (AssetPath);
					if (_instance == null) {
						var tScriptable = ScriptableObject.CreateInstance<T> ();
						AssetDatabase.CreateAsset (tScriptable, AssetPath);
						AssetDatabase.Refresh ();
						_instance = AssetDatabase.LoadAssetAtPath<T> (AssetPath);
					}
				}

				return _instance;
			}
		}
	}

}