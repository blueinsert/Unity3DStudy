/// <summary>
/// Set FPS
/// this script use to set FPS if platform is mobile
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowFPS : MonoBehaviour
{
    public float FPS { get { return fps; } }

    private float updateInterval = 0.5f;
    private float lastInterval;
    private int frames = 0;
    private float fps;

    public static ShowFPS Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //设置帧率
        Application.targetFrameRate = 60;
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    // Update is called once per frame  
    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow >= lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }
    }
    void OnGUI()
    {
        GUI.Label(new Rect(200, 40, 300, 100), fps.ToString());
    }
}