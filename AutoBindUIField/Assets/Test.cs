using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UIManager.Instance.Create(new UIIntent() { name = "TestUI", prefabName = "TestUI", uiControllerName = "TestUIController" });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
