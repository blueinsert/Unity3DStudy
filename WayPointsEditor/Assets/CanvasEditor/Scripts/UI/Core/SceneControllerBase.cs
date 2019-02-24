using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneControllerBase : MonoBehaviour {

	protected virtual string sceneName{
		get{ 
			return "";
		}
	}

	void Awake(){
		OnPreloadBegin ();
	}


	// Update is called once per frame
	void Update () {
		OnUpdate ();
	}

	protected virtual void OnPreloadBegin(){
		GlobalSingleton.Instance.LoadingView.LoadToLevel (sceneName);
		GlobalSingleton.Instance.LoadingView.SetPreloadFinishCallback (OnPreloadFinish);
	}

	/// <summary>
	/// 预加载结束 (有预加载的场景才会触发)
	/// </summary>
	protected virtual void OnPreloadFinish(){
		
	}

	protected virtual void OnStart(){
		
	}
	protected virtual void OnUpdate(){
		
	}
}
