using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTransit : MonoBehaviour
{
    public CinemachineBrain m_brain;
    public CinemachineVirtualCamera[] m_virtualCameras;
    public CinemachineVirtualCamera m_curCamera;

    public Vector3 m_oldAgentPosition;
    public Quaternion m_oldAgentRotation;
    public float m_oldFov;
    public float m_oldDist;
    public Vector3 m_newAgentPosition;
    public Quaternion m_newAgentRotation;
    public float m_newFov;
    public float m_newDist;
    public float m_transitStartTime;
    public float m_transitDuration;

    private void Start()
    {
        m_curCamera = (CinemachineVirtualCamera)m_brain.ActiveVirtualCamera;
    }

    private void SwitchCamera(CinemachineVirtualCamera newCamera, float duration)
    {
        var oldPersonFollow = m_curCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_oldAgentPosition = m_curCamera.Follow.transform.position;
        m_oldAgentRotation = m_curCamera.Follow.transform.rotation;
        m_oldFov = m_curCamera.m_Lens.FieldOfView;
        m_oldDist = oldPersonFollow.CameraDistance;
        m_newAgentPosition = newCamera.Follow.transform.position;
        m_newAgentRotation = newCamera.Follow.transform.rotation;
        var newPersonFollow = newCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_newFov = newCamera.m_Lens.FieldOfView;
        m_newDist = newPersonFollow.CameraDistance;

        m_transitStartTime = Time.time;
        m_transitDuration = duration;

        newCamera.Follow.transform.position = m_oldAgentPosition;
        newCamera.Follow.transform.rotation = m_oldAgentRotation;
        newCamera.m_Lens.FieldOfView = m_oldFov;
        newPersonFollow.CameraDistance = m_oldDist;
        m_curCamera.enabled = false;
        newCamera.enabled = true;
        m_curCamera = newCamera;

    }

    public void Update()
    {
        if (Time.time > m_transitStartTime && Time.time < (m_transitStartTime + m_transitDuration))
        {
            var factor = (Time.time - m_transitStartTime) / m_transitDuration;
            m_curCamera.Follow.transform.position = Vector3.Lerp(m_oldAgentPosition, m_newAgentPosition, factor);
            m_curCamera.Follow.transform.rotation = Quaternion.Lerp(m_oldAgentRotation, m_newAgentRotation, factor);
            m_curCamera.m_Lens.FieldOfView = (m_newFov - m_oldFov) * factor + m_oldFov;
            var newPersonFollow = m_curCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            newPersonFollow.CameraDistance = (m_newDist - m_oldDist) * factor + m_oldDist;
        }
    }

    public void OnGUI()
    {
        foreach (var camera in m_virtualCameras)
        {
            if (GUILayout.Button(camera.name))
            {
                SwitchCamera(camera, 2.0f);
            }
        }

    }
}
