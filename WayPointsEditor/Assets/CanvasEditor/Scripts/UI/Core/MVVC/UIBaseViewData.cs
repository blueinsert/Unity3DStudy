using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseViewData
{

	#region Member



	protected MyObj _myobj;

	protected MyObj myobj {
		get {
			if (_myobj == null) {
				_myobj = view.GetComponent<MyObj> ();
			}
			return _myobj;
		}
	}

	Transform view;

	Dictionary<string, ViewData_Base> viewDatas = new Dictionary<string, ViewData_Base> ();


	#endregion

	public void Init (Transform _tr)
	{
		view = _tr;

		for (int i = 0; i < myobj.GetCount (); i++) {
			MyObj.MyObjData tdata = myobj.GetObjByIndex (i);
			if (tdata.objName.Contains ("lab_")) {
				viewDatas.Add (tdata.objName, new ViewData_Lab (tdata.objName, tdata.obj.GetComponent<UILabel> ()));
			} else if (tdata.objName.Contains ("slider_")) {
				viewDatas.Add (tdata.objName, new ViewData_Slider (tdata.objName, tdata.obj.GetComponent<UISlider> ()));
			} else if (tdata.objName.Contains ("sprite_")) {
				viewDatas.Add (tdata.objName, new ViewData_Sprite (tdata.objName, tdata.obj.GetComponent<UISprite> ()));
			}
		}
	}

	#region UI 各种调用


	public void SetLab (string _name, string _value)
	{
		if (!viewDatas.ContainsKey (_name))
			return;

		viewDatas [_name].stringValue.Value = _value;
	}

	public void SetSlider (string _name, float _value)
	{
		if (!viewDatas.ContainsKey (_name))
			return;

		viewDatas [_name].floatValue.Value = _value;
	}

	public void SetSprite (string _name, string _value)
	{
		if (!viewDatas.ContainsKey (_name))
			return;
		
		viewDatas [_name].stringValue.Value = _value;
	}

	public UILabel GetLab (string _name)
	{
		if (!viewDatas.ContainsKey (_name))
			return null;
		
		return (viewDatas [_name] as ViewData_Lab).GetLab ();
	}

	public UISprite GetSprite (string _name)
	{
		if (!viewDatas.ContainsKey (_name))
			return null;

		return (viewDatas [_name] as ViewData_Sprite).GetSprite ();
	}

	public UISlider GetSlider (string _name)
	{
		if (!viewDatas.ContainsKey (_name))
			return null;

		return (viewDatas [_name] as ViewData_Slider).GetSlider ();
	}

	#endregion

}

#region UI 数据类

public abstract class ViewData_Base
{
	public string key;
	public TData<int> intValue;
	public TData<float> floatValue;
	public TData<string> stringValue;

	public ViewData_Base (string _key)
	{
		key = _key;
		intValue = new TData<int> ();
		floatValue = new TData<float> ();
		stringValue = new TData<string> ();
	}

}

public class ViewData_Lab : ViewData_Base
{
	UILabel lab;

	public ViewData_Lab (string _key, UILabel _lab) : base (_key)
	{
		lab = _lab;
		stringValue.BindValueChange (OnValueChange);
	}

	protected void OnValueChange (string _value)
	{
		lab.text = stringValue.Value;
	}

	public UILabel GetLab ()
	{
		return lab;
	}
}

public class ViewData_Sprite : ViewData_Base
{
	UISprite sprite;

	public ViewData_Sprite (string _key, UISprite _sprite) : base (_key)
	{
		sprite = _sprite;
		stringValue.BindValueChange (OnValueChange);
	}

	protected void OnValueChange (string _value)
	{
		sprite.spriteName = _value;
	}

	public UISprite GetSprite ()
	{
		return sprite;
	}
}

public class ViewData_Slider : ViewData_Base
{
	UISlider slider;

	public ViewData_Slider (string _key, UISlider _slider) : base (_key)
	{
		slider = _slider;
		floatValue.BindValueChange (OnValueChange);
	}

	protected void OnValueChange (float _value)
	{
		slider.value = floatValue.Value;
	}

	public UISlider GetSlider ()
	{
		return slider;
	}
}



public class TData<T>
{
	T _value;

	event System.Action<T> onValueChange;

	public T Value {
		get { 
			return _value;
		}
		set {
			if (!_value.Equals (value)) {
				_value = value;
				onValueChange (_value);
			}
		}
	}

	public void BindValueChange (System.Action<T> _callback)
	{
		onValueChange += _callback;
	}

	public void UnBindValueChange (System.Action<T> _callback)
	{
		onValueChange -= _callback;
	}
}

#endregion


