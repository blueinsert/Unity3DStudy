using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ConfigDataManager.CreateInstance(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
