using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MyObj : MonoBehaviour {

	Dictionary<string, MyObjData> objs = new Dictionary<string, MyObjData>();
//	List<string> keys = new List<string>();
	public List<MyObjData> datas = new List<MyObjData>();

	// Use this for initialization
	void Awake () {
		for(int i = 0;i < datas.Count;i++){
			if (objs.ContainsKey (datas [i].objName))
				continue;

			objs.Add (datas[i].objName, datas[i]);
		}
	}

	/// <summary>
	/// 通过key 获取对象
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="_key">Key.</param>
	public GameObject GetObj(string _key){
		if (!objs.ContainsKey (_key))
			return null;

		return objs [_key].obj;
	}
	public MyObjData GetObjByIndex(int _index){
		if (_index >= datas.Count)
			return new MyObjData();

		return datas [_index];

	}

	/// <summary>
	/// 获取所有对象的数量
	/// </summary>
	/// <returns>The count.</returns>
	public int GetCount(){
		return objs.Count;
	}


	[Serializable]
	public struct MyObjData{
		public string objName;
		public GameObject obj;


		public MyObjData(string _name, GameObject _obj){
			objName = _name;
			obj = _obj;
		}
	}
}


