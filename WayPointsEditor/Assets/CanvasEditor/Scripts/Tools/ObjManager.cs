using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjManager : MonoBehaviour {
	public class cachedGOStruct
	{
		public bool isShowing;
		public float delayDespawn;
		public float timer;
		public string path;
		public string name;
		public GameObject go;
		public DespawnParent despawnParent;

		public bool IsEquals(string _path, string _name){
			if (path.Equals (_path) && name.Equals (_name))
				return true;
			return false;
		}
	}

	public enum DespawnParent
	{
		/// <summary>
		/// 3d 物体
		/// </summary>
		enDefaultObj,
		/// <summary>
		/// ui 物体
		/// </summary>
		enDefaultUI,
	}

	#region Member

	static ObjManager _instance;
	public static ObjManager instance{
		get{
			if (_instance == null) {
				_instance = new GameObject ("ObjManager").AddComponent<ObjManager> ();
				_instance.transform.position = Vector3.one * 2000;
			}
			return _instance;
		}
	}

	public Transform defaultParent {
		get { 
			return transform;
		}
	}

	Transform _defaultUIParent;
	public Transform defaultUIParent{
		get{
			if (_defaultUIParent == null) {
				var _uiRoot = GameObject.Find ("UI Root");
				if (_uiRoot == null) {
					_uiRoot = Resources.Load<GameObject> (GamePath.UIRootPath);
				}

				_defaultUIParent = new GameObject ("UIDespawn").transform;
				_defaultUIParent.transform.parent = _uiRoot.transform;
				_defaultUIParent.transform.localScale = Vector3.one;
			}

			return _defaultUIParent;
		}
	}

	List<cachedGOStruct> cachedGO =new List<cachedGOStruct>();

	bool canUpdate = true;
	#endregion

	#region Interface
	/// <summary>
	/// 获取一个物体
	/// </summary>
	/// <returns>The game object.</returns>
	/// <param name="path">预制路径</param>
	/// <param name="name">预制名字</param>
	/// <param name="_defaultParent">父节点</param>
	/// <param name="_despawnParent">回收后父节点</param>
	/// <param name="delayDespawn">延迟回收</param>
	public GameObject GetGameObject(string path, string name,Transform _defaultParent, DespawnParent _despawnParent, float delayDespawn = -1){
		for(int i = 0;i < cachedGO.Count;i++){
			if(cachedGO[i].IsEquals(path, name) && !cachedGO[i].isShowing){
				cachedGO [i].delayDespawn = delayDespawn;
				cachedGO [i].timer = 0;
				cachedGO [i].isShowing = true;
				if (_defaultParent != null) {
					cachedGO [i].go.transform.parent = _defaultParent;
					cachedGO [i].go.transform.localPosition = Vector3.zero;
				}

				cachedGO [i].go.SetActive (true);
				return cachedGO [i].go;
			}
		}
		 
		// 缓存中没有对应的数据
		return CreateGameObject(path, name, _defaultParent, _despawnParent, delayDespawn);

	}

	public GameObject GetGameObject(string path, string name, Transform _parent){
		return GetGameObject (path, name ,_parent, DespawnParent.enDefaultObj, -1);
	}

	public GameObject GetGameObject(string path, string name,  DespawnParent _despawnParent){
		return GetGameObject (path, name, null, _despawnParent,  -1);
	}
	public GameObject GetGameObject(string path, string name){
		return GetGameObject (path, name, null, DespawnParent.enDefaultObj,  -1);
	}


	public void Despawn(GameObject _go){
		for(int i = 0;i < cachedGO.Count;i++){
			if (cachedGO [i].go == _go && cachedGO[i].isShowing) {
				if (cachedGO [i].despawnParent == DespawnParent.enDefaultObj)
					cachedGO [i].go.transform.parent = defaultParent;
				else if (cachedGO [i].despawnParent == DespawnParent.enDefaultUI)
					cachedGO [i].go.transform.parent = defaultUIParent;
				cachedGO [i].go.transform.parent = transform;
				cachedGO [i].go.transform.localPosition = Vector3.zero;
				cachedGO [i].go.transform.localScale = Vector3.one;
				cachedGO [i].go.SetActive (false);
				cachedGO [i].isShowing = false;
				return;
			}
		}

		Debug.Log ("回收错误  物体名字 :[" + _go.name);
	}

	#endregion

	#region Private

	GameObject CreateGameObject(string path, string name, Transform defaultParent, DespawnParent _despawnParent, float delayDespawn){
		var tGO = Resources.Load<GameObject>(path + name);
		if (tGO == null) {
			Debug.LogError ("路径 :[" + path + "] 下不存在资源 :[" + name + "] !!! 请检查路径或资源名是否正确");
			return null;
		}

		var GO = GameObject.Instantiate<GameObject> (tGO);
	
		if (_despawnParent == DespawnParent.enDefaultUI) {
			GO.transform.parent = defaultUIParent;
		}
		else if(_despawnParent == DespawnParent.enDefaultObj){
			if (defaultParent == null)
				GO.transform.parent = transform;
			else
				GO.transform.parent = defaultParent;
		}

		GO.transform.localPosition = Vector3.zero;
		GO.transform.localScale = Vector3.one;

		var cgoStruct = new cachedGOStruct ();
		cgoStruct.path = path;
		cgoStruct.name = name;
		cgoStruct.timer = 0;
		cgoStruct.isShowing = true;
		cgoStruct.go = GO;
		cgoStruct.despawnParent = _despawnParent;
		cgoStruct.delayDespawn = delayDespawn;
		cachedGO.Add (cgoStruct);
		return cgoStruct.go;
	}

	#endregion

	#region Awake 等方法

	void Awake(){
		transform.position = new Vector3 (1000,0,1000);
	}
	
	// Update is called once per frame
	void Update () {
		if (!canUpdate)
			return;

		for(int i = 0;i < cachedGO.Count;i++){
			if(cachedGO[i].isShowing && cachedGO[i].delayDespawn > -1){
				cachedGO [i].timer += Time.deltaTime;

				if (cachedGO [i].timer >= cachedGO [i].delayDespawn) {
					Despawn (cachedGO[i].go);
				}
			}
		}

	}

	#endregion
}
