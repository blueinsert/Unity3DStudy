using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour  where T : MonoBehaviour
{

	static T _instance;

	public static T Instance {
		get {
			if (_instance == null) {
				_instance = new GameObject (typeof(T).ToString ()).AddComponent<T> ();			
				GameObject.DontDestroyOnLoad (_instance);
			}
		
			return _instance;
		}
	}

	public void Init(){}


}
