using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
	public static InputHandler Instance;
	public Vector2 m_moveValue;
	public Vector2 m_rotateValue;
	public Vector2 MoveValue { get { return m_moveValue; } }
	public Vector2 RotateValue { get { return m_rotateValue; } }
	public Vector3 m_lastMousePosition;
	public Vector2 m_scrollValue;
	public Vector2 ScrollValue { get { return m_scrollValue; } }

	void Awake() {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		m_moveValue.x = 0;
		m_moveValue.y = 0;
		if (Input.GetKey(KeyCode.W))
		{
			m_moveValue.y = 1;
		}
		if (Input.GetKey(KeyCode.S))
		{
			m_moveValue.y = -1;
		}
		if (Input.GetKey(KeyCode.A))
		{
			m_moveValue.x = -1;
		}
		if (Input.GetKey(KeyCode.D))
		{
			m_moveValue.x = 1;
		}
		m_moveValue = m_moveValue.normalized;

		var mouseDelta = Input.mousePosition - m_lastMousePosition;
		m_rotateValue = new Vector2(mouseDelta.x, mouseDelta.y);
		m_lastMousePosition = Input.mousePosition;
		m_scrollValue = Input.mouseScrollDelta;
	}
}
