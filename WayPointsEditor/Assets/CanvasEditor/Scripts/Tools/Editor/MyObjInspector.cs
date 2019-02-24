using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MyObj))]
public class MyObjInspector : Editor {

	MyObj _myobj;

	GameObject go;
	bool autoName = true;
	string promptStr = "提示附"; // 提示字符
	string selectName = "";
	GameObject selectGo = null;

	void OnEnable(){
		if (_myobj == null)
			_myobj = target as MyObj;


	}

	public override void OnInspectorGUI ()
	{
		autoName = EditorGUILayout.Toggle("自动设置名字", autoName);	
		Color cachedColor = GUI.backgroundColor;
	
		drawPrompt ();

		GUI.backgroundColor = cachedColor;

		EditorGUILayout.BeginVertical ("box");

		showListData ();

		drawNullLine (ref selectName, ref selectGo);

		addNullLineData ();


		EditorGUILayout.EndVertical ();

		Repaint ();

		if (GUI.changed) {
			Undo.RegisterCompleteObjectUndo (target, "save");

		}

	} 

	/// <summary>
	/// 画提示
	/// </summary>
	void drawPrompt(){
		GUI.backgroundColor = Color.red;
		EditorGUILayout.BeginVertical ("box");
		EditorGUILayout.LabelField ("提示 : ", promptStr);
		EditorGUILayout.EndHorizontal ();
	}

	/// <summary>
	/// 显示列表中的数据
	/// </summary>
	void showListData(){
		for(int i = 0;i < _myobj.datas.Count;i++){
			if (drawLine (i, _myobj.datas [i].objName, _myobj.datas [i].obj)) {
				_myobj.datas.RemoveAt (i);
				i--;
			}

		}
	}

	/// <summary>
	/// 画一行
	/// </summary>
	/// <returns><c>true</c> 是否删除此行 <c>false</c> otherwise.</returns>
	/// <param name="_index">数据的下标</param>
	/// <param name="_key">Key.</param>
	/// <param name="_go">Go.</param>
	bool drawLine(int _index, string _key, GameObject _go){
		bool result = false;

		EditorGUILayout.BeginHorizontal ();
		var tStruct = _myobj.datas [_index];
		EditorGUILayout.LabelField ("ID:" + _index, GUILayout.Width(40.0f));
		tStruct.objName = EditorGUILayout.TextField (_key, GUILayout.MaxWidth(50.0f));
		EditorGUILayout.LabelField ("obj", GUILayout.Width(25.0f));
		tStruct.obj = EditorGUILayout.ObjectField (_go, typeof(GameObject), true) as GameObject;
		if (GUILayout.Button ("-"))
			result = true;

		_myobj.datas [_index] = tStruct;
		EditorGUILayout.EndHorizontal ();

		return result;
	}


	/// <summary>
	/// 画空行，用于添加物体
	/// </summary>
	/// <param name="_key">Key.</param>
	/// <param name="_obj">Object.</param>
	void drawNullLine(ref string _key, ref GameObject _obj){
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("ID:" + _myobj.datas.Count, GUILayout.Width(40.0f));
		_key = EditorGUILayout.DelayedTextField (_key);
		EditorGUILayout.LabelField ("obj", GUILayout.Width(25.0f));
		_obj = EditorGUILayout.ObjectField (_obj, typeof(GameObject), true) as GameObject;
		EditorGUILayout.EndHorizontal ();
	}

	/// <summary>
	/// 添加空行的数据
	/// </summary>
	void addNullLineData(){
		if (autoName && selectGo != null)
			selectName = selectGo.name;

		if(selectGo != null && selectName != ""){
			

			bool ishas = false;
			for(int i = 0;i < _myobj.datas.Count;i++){
				if (_myobj.datas [i].objName == selectName) {
					ishas = true;
					promptStr = "不能添加相同的 key";
				}
			}

			if (!ishas) {
				_myobj.datas.Add (new MyObj.MyObjData (selectName, selectGo));
				promptStr = "";
			}
			selectName = "";
			selectGo = null;
		}
	}
}
