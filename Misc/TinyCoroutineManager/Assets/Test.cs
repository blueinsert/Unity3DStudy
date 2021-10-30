using UnityEngine;
using System.Collections;
using bluebean.utils;
using WaitForSeconds = bluebean.utils.WaitForSeconds;

public class Test : MonoBehaviour {

    TinyCorcoutineManager corcoutineManager = new TinyCorcoutineManager();

	// Use this for initialization
	void Start () {
        corcoutineManager.StartCorcoutine(Test1());
        //StartCoroutine(Test1());
    }
	
	// Update is called once per frame
	void Update () {
        corcoutineManager.Update();
	}

    //test case for TinyCorcoutineManager
    IEnumerator Test1()
    {
        print("WaitForFames start:" + Time.frameCount);
        yield return new WaitForFrames(15);
        print("WaitForFames end:" + Time.frameCount);
        print("WaitForSeconds start" + Time.time);
        yield return new WaitForSeconds(2);
        print("WaitForSeconds end" + Time.time);
        yield return Child();
        print(Time.frameCount);
        yield return new WaitForNextFrame();
        print(Time.frameCount);
        yield return new WaitForNextFrame();
        print(Time.frameCount);
        yield break;
    }

    IEnumerator Child()
    {
        print("in child, " + Time.frameCount);
        yield return null;
        print("in child, " + Time.frameCount);
        yield return ChildChild();
        yield return new WaitForNextFrame();
        print("in child, " + Time.frameCount);
    }


    IEnumerator ChildChild()
    {
        print("in child'child, " + Time.frameCount);
        yield return null;
        print("in child'child, " + Time.frameCount);
        yield return new WaitForNextFrame();
        print("in child'child, 提前退出, " + Time.frameCount);
        yield break;
        print("others be skipped");
    }

}
