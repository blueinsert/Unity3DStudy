using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera[] m_virtualCameras;

    public void OnGUI()
    {
        foreach(var camera in m_virtualCameras)
        {
            if (GUILayout.Button(camera.name))
            {
                foreach (var c in m_virtualCameras)
                {
                    c.enabled = false;
                }
                camera.enabled = true;
            }
        }
        
    }
}
