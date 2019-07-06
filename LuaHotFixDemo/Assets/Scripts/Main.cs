using System;
using System.Collections;
using System.Collections.Generic;
using SLua;
using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LuaManager.CreateLuaManager().Initlize("LuaRoot");
        LuaManager.Instance.StartLuaSvr((res)=> { });
        HotFixTestScript testObj = new HotFixTestScript();
        Debug.Log(testObj.Add(1, 3));
        HotFixTestScript testObj2 = new HotFixTestScript();
        Debug.Log(testObj2.Add(1, 3));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
