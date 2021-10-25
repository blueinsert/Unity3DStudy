using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour {
	public static FollowCamera Instance;
	public Camera m_camera;
	public GameObject m_agent;
	public GameObject m_target;
	public float m_distance;
	public Vector3 m_headOffset;
	public float m_minXRotate = -5f;
	public float m_maxXRotate = 89f;
	public float m_minDistance = 1.0f;
	public float m_maxDistance = 10.0f;
	[Header("旋转灵敏度")]
	public float m_rotateXRate = 1.0f;
	[Header("旋转灵敏度")]
	public float m_rotateYRate = 1.0f;
	void Awake() {
		Instance = this;
	}

	public Vector3 GetForward()
	{
		return m_agent.transform.forward;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	void UpdatePosition()
	{
		m_agent.transform.position = m_target.transform.position;
	}

	void UpdateManualRotation(Vector2 value)
	{
		var euler = m_agent.transform.localEulerAngles;
		euler.y += value.x;
		euler.x += value.y;
		if (euler.x > 180)
		{
			euler.x -= 360;
		}
		euler.x = Mathf.Clamp(euler.x, m_minXRotate, m_maxXRotate);
		var newRotate = Quaternion.Euler(euler.x, euler.y, 0);
		m_agent.transform.rotation = newRotate;
	}

	void UpdateRotation()
	{
		if (InputHandler.Instance == null)
			return;
		var rotate = InputHandler.Instance.RotateValue;
		if (rotate.x == 0 && rotate.y == 0)
			return;
		rotate.y = -rotate.y;
		rotate.x /= Screen.width;
		rotate.y /= Screen.height;
		rotate.x *= 360* m_rotateXRate;
		rotate.y *= 180 * m_rotateYRate;
		UpdateManualRotation(rotate);
	}

	void UpdateZoom()
	{
		if (InputHandler.Instance == null)
			return;
		var scrollVaule = InputHandler.Instance.ScrollValue.y;
		if (scrollVaule == 0)
			return;
		if (scrollVaule < 0)
		{
			m_distance *= 1 + 0.1f;
		}
		else
		{
			m_distance /= (1 + 0.1f);
		}
		m_distance = Mathf.Clamp(m_distance, m_minDistance, m_maxDistance);
	}

	// Update is called once per frame
	void Update () {
		UpdateZoom();
		UpdatePosition();
		UpdateRotation();
		var targetPos = m_agent.transform.position + m_headOffset;
		var forward = m_agent.transform.forward;
		var cameraPos = targetPos - forward * m_distance;
		m_camera.transform.position = cameraPos;
		m_camera.transform.LookAt(targetPos);
	}
}
