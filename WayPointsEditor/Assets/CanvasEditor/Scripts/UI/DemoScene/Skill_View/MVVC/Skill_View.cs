using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_View : UIBaseView {

	#region Member
	public override string viewName {
		get {
			return "Skill_View";
		}
	}

	Skill_ViewController skillController{
		get{ 
			return (controller as Skill_ViewController);
		}
	}

	#endregion

	public override void InitController ()
	{
		controller = new Skill_ViewController ();
		controller.Init (new Skill_ViewData(), this);
		base.InitController ();
	}

	void Click_ToDemo(zNGUINotifyData _data){
		UIViewMananger.Instance.OpenView<Demo_View> (ViewDepth.enDepthOne);
	}
}
