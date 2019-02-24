using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zEditorWindow;

public class Test2 : MonoBehaviour {

	void Awake(){
		Test3 t3 = new Test3 ();
		System.Type typ = System.Type.GetType ("Test3");
//		t3.GetComponent<typ> ();
	}

}


public class Test3 {

	public static string name = "Test3";
	public static string GetName{
		get{ 
			return name;
		}
	}


	public void GetComponent<T> () where T : Test3 {
		Debug.Log (typeof(T).ToString ());
	}

}
