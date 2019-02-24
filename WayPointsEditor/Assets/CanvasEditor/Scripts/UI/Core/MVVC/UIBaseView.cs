using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseView : MonoBehaviour {

	#region Member

	protected UIBaseViewController controller;

	public virtual string viewName{
		get{ 
			return "BaseView";
		}
	}

	public int nowDepth{
		get{ 
			return controller.GetDepth ();
		}
		set{ 
			controller.SetDepth (value);
		}
	}

	#endregion


	void Awake(){
		InitController ();
	}

	public virtual void InitController(){
		
	}

	public virtual void OnOpenView(){
		gameObject.SetActive (true);
		controller.OnOpenViewAnimation ();
	}
	public virtual void OnCloseView(){
		gameObject.SetActive (false);
		controller.OnCloseViewAnimation ();
	}

}
