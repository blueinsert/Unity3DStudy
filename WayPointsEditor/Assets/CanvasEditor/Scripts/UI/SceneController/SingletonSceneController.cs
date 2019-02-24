using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SingletonSceneController : SceneControllerBase {
	protected override string sceneName {
		get {
			return "SingletonScene";
		}
	}

	[Header("跳到哪个场景")]
	public string ToScene;

	protected override void OnPreloadBegin ()
	{
		base.OnPreloadBegin ();
	}

	protected override void OnPreloadFinish ()
	{
		base.OnPreloadFinish ();
		GlobalSingleton.Instance.Init ();
		GlobalSingleton.Instance.SceneName = ToScene;
		SceneManager.LoadScene (ToScene);
	}

}
