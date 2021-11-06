using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public partial class ConfigDataManager : MonoBehaviour {

	public const string AssetPath = "Assets/Resources/ConfigData";
	const int DataStartRow = 3;
	const char ArraySplitMark = '|';

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
		var csvs = Resources.LoadAll<TextAsset>("ConfigData/");
		foreach(var csv in csvs)
		{
			var name = csv.name;
			string str = System.Text.Encoding.GetEncoding("utf-8").GetString(csv.bytes);
			var strGrid = CsvParser.Parse(str);
			var initMethodName = string.Format("Init{0}ConfigDic", name);
			var initMethod = this.GetType().GetMethod(initMethodName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (initMethod != null)
			{
				initMethod.Invoke(this, new object[] { strGrid });
			}
		}
		Debug.Log(m_TestConfigDic.Count);
		var cfg = m_TestConfigDic[1];
		Debug.Log(string.Format("{0} {1} {2} {3}", cfg.ID, cfg.Desc, cfg.Field1, cfg.Field2));
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
		}else if(t == typeof(float))
		{
			res = float.Parse(content);
		}
		else if(t == typeof(string))
		{
			res = content;
		}else if(t == typeof(int[]))
		{
			var splits = content.Split(new char[] { ArraySplitMark });
			var data = new int[splits.Length];
			for(int i = 0; i < splits.Length; i++)
			{
				data[i] = (int)GetData(typeof(int), splits[i]);
			}
			res = data;
		}
		return res;
	}
}
