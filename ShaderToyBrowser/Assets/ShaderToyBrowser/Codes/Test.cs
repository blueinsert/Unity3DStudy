using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean.ShaderToyBrowser;

public class Test : MonoBehaviour {

    IEnumerator Coro_Test() {
        
        //var config = ConfigLoader.Load<UserData>("Assets/ShaderToyBrowser/Config/shaders.json.txt");
       // Debug.Log(JsonUtility.ToJson(config));
        UIManager.Instance.PushUIJob<MainPageUIJob>(new CustomParams());
        yield return new WaitForSeconds(1.0f);
        //UIManager.Instance.PopUIJob();
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(Coro_Test());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
