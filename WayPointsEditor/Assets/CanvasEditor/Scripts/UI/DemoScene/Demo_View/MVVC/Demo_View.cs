using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_View : UIBaseView {

	#region 固定格式

	public override string viewName {
		get {
			return "Demo_View";
		}
	}

	Demo_ViewController demoController{
		get{
			return controller as Demo_ViewController;
		}
	}

	public override void InitController ()
	{
		controller = new Demo_ViewController ();
		controller.Init (new Demo_ViewData(), this);
	}
	#endregion



	void Click_Commit(zNGUINotifyData _data){
		Debug.Log ("Click " + _data.obj.name + "   press :[" + _data.isPress + "]");
	}
	void Click_monster(zNGUINotifyData _data){
		Debug.Log ("Click  " + _data.obj.name + "  press :[" + _data.isPress + "]   index :[" + _data.index);
	}


	void DoubleClick_Commit(zNGUINotifyData _data){
		UIViewMananger.Instance.OpenView<Skill_View> (ViewDepth.enDepthOne);
	}
	void Press_Commit(zNGUINotifyData _data){
		Debug.Log ("Press   go :[" + _data.obj.name + "]  press :[" + _data.isPress + "]    index :[" + _data.index + "]");
	}
	void LongPress_Commit(zNGUINotifyData _data){
		Debug.Log ("LongPress go :[" + _data.obj.name + "]  index :[" + _data.index + "]");
	}
}
