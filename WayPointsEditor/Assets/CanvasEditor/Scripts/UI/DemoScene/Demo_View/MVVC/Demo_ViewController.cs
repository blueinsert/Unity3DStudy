using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_ViewController : UIBaseViewController {

	public Demo_ViewData demoData{
		get{ 
			return viewData as Demo_ViewData;
		}
	}

	public override void Init (UIBaseViewData _data, UIBaseView _view)
	{
		base.Init (_data, _view);
	}

}
