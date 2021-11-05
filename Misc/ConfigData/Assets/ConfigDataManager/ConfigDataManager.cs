using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ConfigDataManager : MonoBehaviour {

	public const string AssetPath = "Assets/Resources/ConfigData";
	const int DataStartRow = 3;
	public static ConfigDataManager Instance;

	public static void CreateInstance(GameObject attach)
	{
		var cfgMgr = attach.GetComponent<ConfigDataManager>();
		if (cfgMgr == null)
		{
			cfgMgr = attach.AddComponent<ConfigDataManager>();
		}
		cfgMgr.Init();
		Instance = cfgMgr;
	}

	public void Init()
	{

	}

	private void ParseConfigDataDic<T>(List<string[]> content, Dictionary<int,T> dic) where T : ConfigDataBase
	{
		dic.Clear();
		if (content.Count < DataStartRow)
			return;
		var fieldNames = content[0];
		for(int i = DataStartRow; i < content.Count; i++)
		{
			var rowContent = content[i];
			int id = int.Parse(rowContent[0]);
			var configData = ParseConfigData<T>(content[i]);
			dic.Add(id, configData);
		}
	}

	private T ParseConfigData<T>(string[] content) where T : ConfigDataBase
	{
		var type = typeof(T);
		var fields = type.GetFields();
		var instance = System.Activator.CreateInstance<T>();
		for(int i = 0; i < fields.Length; i++)
		{
			var fieldInfo = fields[i];
			var fieldName = fieldInfo.Name;
			var data = GetData(fieldInfo.FieldType, content[i]);
			fieldInfo.SetValue(instance, data);
		}
		return instance;
	}

	private object GetData(System.Type t,string content)
	{
		object res = null;
		if (t == typeof(int))
		{
			res = int.Parse(content);
		}
		return res;
	}
}
