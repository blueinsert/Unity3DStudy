using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class One{
	public static string name = "One";
	public static string GetName{
		get{ 
			return name;
		}
	}

}

public class Two : One{
	public string name = "Two";

	public static string GetName{
		get{ 
			return "Two";
		}
	}

}

