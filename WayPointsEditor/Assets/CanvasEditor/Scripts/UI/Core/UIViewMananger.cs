using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIViewMananger : Singleton<UIViewMananger>
{
	string sceneName{
		get{ 
			return GlobalSingleton.Instance.SceneName;
		}
	}


	List<ViewData> viewDatas = new List<ViewData> ();

	#region Interface
	/// <summary>
	/// 获取某个界面
	/// </summary>
	/// <returns>The view.</returns>
	/// <typeparam name="T1">The 1st type parameter.</typeparam>
	public T1 GetView<T1>() where T1 : UIBaseView{
		string _viewName = typeof(T1).ToString ();
		ViewData _viewData = getView<T1> (_viewName);
		return (T1)_viewData.view;
	}

	/// <summary>
	/// 显示界面
	/// </summary>
	/// <param name="_fatherView">在某个界面下打开</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T1 OpenView<T1> (ViewDepth _depth, UIBaseView _parentView = null) where T1 : UIBaseView
	{
		// 关闭同级界面
		var _otherDepthView = getViewByDepth (_depth);
		if (_otherDepthView != null) {
			CloseView (_otherDepthView.viewName);
		}

		string _viewName = typeof(T1).ToString ();
		Debug.Log ("Want Open View " + _viewName);
		var _view = getView<T1> (_viewName);
		if (_view == null) {
			Debug.LogError ("打开界面 :[" + _viewName + "] 失败，请检查是否有该界面");
			return null;
		}

		if (_parentView != null) {
			var _parentViewData = getView (_parentView.viewName);
			if (_parentViewData != null)
				_parentViewData.AddChildView (_view.view);
		}


		_view.view.nowDepth = (int)_depth;
		_view.view.OnOpenView ();
		_view.viewEnable = true;
		return (T1)_view.view;
	}

	public void CloseView (string _viewName)
	{
		for (int i = 0; i < viewDatas.Count; i++) {
			if (viewDatas [i].viewName.Equals (_viewName)) { // 遍历关闭该界面下的所有子界面
				for (int j = 0; j < viewDatas [i].childViews.Count; j++) {
					viewDatas [i].childViews [j].OnCloseView ();
				}
				viewDatas [i].view.OnCloseView ();
				viewDatas [i].viewEnable = false;

				Debug.Log ("关闭界面成功");
				return;
			}
		}
	}

	#endregion

	#region Private

	ViewData getViewByDepth (ViewDepth _depth)
	{
		for (int i = 0; i < viewDatas.Count; i++) {
			if (viewDatas[i].viewEnable && viewDatas [i].view.nowDepth == ((int)_depth))
				return viewDatas [i];
		}

		return null;
	}

	ViewData getView<T1> (string _viewName) where T1 : UIBaseView
	{
		ViewData _result = getView (_viewName);

		if (_result == null) {
			var go = ObjManager.instance.GetGameObject (GamePath.UIViewPath + sceneName + "Scene/" + _viewName + "/", _viewName);
			go.transform.parent = GlobalSingleton.Instance.UIRoot.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			go.gameObject.SetActive (false);
			if (go == null)
				return null;

			var code = go.GetComponent<T1> ();

			ViewData _data = new ViewData (_viewName, code);
			viewDatas.Add (_data);
			_result = _data;
		}

		return _result;
	}
	/// <summary>
	/// 从现已打开的界面中获取
	/// </summary>
	/// <returns>The view.</returns>
	/// <param name="_viewName">View name.</param>
	ViewData getView(string _viewName){
		ViewData _result = null;

		for (int i = 0; i < viewDatas.Count; i++) {
			if (viewDatas [i].viewName.Equals (_viewName)) {
				_result = viewDatas [i];
				break;
			}
		}
		return _result;
	}

	#endregion
}

public class ViewData
{
	public string viewName;
	public UIBaseView view;
	public bool viewEnable;
	public List<UIBaseView> childViews = new List<UIBaseView> ();

	public ViewData (string _viewName, UIBaseView _view)
	{
		viewName = _viewName;
		view = _view;
	}

	public void AddChildView (UIBaseView  _child)
	{
		childViews.Add (_child);
	}

}

public enum ViewDepth
{
	enDepthNull = -1,
	enDepthOne = 0,
	enDepthTwo,
	enDepthThree,
	enDepthFour,
	enDepthFive,
	enDetphMiddle = 10,
	enDetphTop = 15,

}
	

