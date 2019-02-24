using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseViewController{

	#region Member


	UIBaseView view;
	protected UIBaseViewData viewData;
	UIPanel panel;


	#endregion

	public virtual void Init(UIBaseViewData _data, UIBaseView _view){
		view = _view;
		panel = view.gameObject.GetComponent<UIPanel> ();
		viewData = _data;
		viewData.Init (view.transform);
	}





	#region UI 控件各种调用

	public UILabel GetLab(string _name){
		return viewData.GetLab (_name);
	}

	public UISprite GetSprite(string _name){
		return viewData.GetSprite (_name);
	}
	public UISlider GetSlider (string _name){
		return viewData.GetSlider (_name);
	}
		

	public void SetLab(string _name, string _value){
		viewData.SetLab (_name, _value);
	}

	public void SetSlider (string _name, float _value){
		viewData.SetSlider (_name, _value);
	}

	public void SetSprite(string _name, string _value){
		viewData.SetSprite (_name, _value);
	}

	public virtual void OnOpenViewAnimation(){
		
	}
	public virtual void OnCloseViewAnimation(){
	
	}


	#endregion

	#region 控制界面接口

	public void SetDepth(int _depth){
		panel.depth = _depth;
	}
	public int GetDepth(){
		return panel.depth;
	}


	public Transform GetTransform(){
		return view.transform;
	}



	#endregion

}
