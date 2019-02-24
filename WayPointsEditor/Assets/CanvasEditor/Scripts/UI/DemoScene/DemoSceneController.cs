using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSceneController : SceneControllerBase
{

	protected override string sceneName {
		get {
			return "Demo";
		}
	}

	protected override void OnPreloadBegin ()
	{
		base.OnPreloadBegin ();
	}

	protected override void OnPreloadFinish ()
	{
		base.OnPreloadFinish ();
		UIViewMananger.Instance.OpenView<Demo_View> ( ViewDepth.enDepthOne);
	}
}
