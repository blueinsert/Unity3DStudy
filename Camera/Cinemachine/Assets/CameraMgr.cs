using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMgr : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;
    public int i = 0;

    public void OnGUI()
    {
        if (GUI.Button(new Rect(0,0,100,100), "EnableNext"))
        {
            var cur = cameras[i];
            cur.enabled = false;
            int next = i + 1;
            if (next > cameras.Length - 1)
            {
                next = 0;
            }
            var nextCamera = cameras[next];
            nextCamera.enabled = true;
            i = next;
        }
    }
}
