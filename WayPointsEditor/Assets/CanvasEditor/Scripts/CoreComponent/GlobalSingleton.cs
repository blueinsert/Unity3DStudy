using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zPreload;

public class GlobalSingleton : Singleton<GlobalSingleton> {

	public string SceneName;

	UIRoot _uiroot;
	public UIRoot UIRoot{
		get{ 
			if (_uiroot == null) {
				_uiroot = GameObject.FindObjectOfType<UIRoot> ();
			}
			if (_uiroot == null) {
				var _obj = Resources.Load<GameObject> (GamePath.UIRootPath);
				var go = GameObject.Instantiate<GameObject> (_obj);
				_uiroot = go.GetComponent<UIRoot> ();
			}

			return _uiroot;
		}
	}

	Loading_View _loadingView;
	public Loading_View LoadingView{
		get{ 
			if (_loadingView == null){
				var _obj = Resources.Load<GameObject> (GamePath.UILoadingViewPath);
				var go = GameObject.Instantiate<GameObject> (_obj);
				go.transform.parent = UIRoot.transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localScale = Vector3.one;
				_loadingView = go.GetComponent<Loading_View> ();
			}

			return _loadingView;
		}
	}

}
