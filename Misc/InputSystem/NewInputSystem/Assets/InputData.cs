using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputData : MonoBehaviour
{
    [SerializeField] private InputActionPhase m_movePhase;
    [SerializeField] private Vector2 m_moveValue;

    public InputActionPhase m_rotatePhase;
    public Vector2 m_rotationValue;

    [SerializeField] private InputActionPhase m_scrollPhase;
    [SerializeField] private Vector2 m_scrollValue;

    private void Start()
    {
       
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_movePhase = context.phase;
        m_moveValue = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        var mousepos = Mouse.current.position.ReadValue();
        m_rotatePhase = context.phase;
        m_rotationValue = context.ReadValue<Vector2>();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        Debug.Log(string.Format("{0 }Scroll {1}", Time.frameCount, context.ReadValue<Vector2>()));
        m_scrollPhase = context.phase;
        m_scrollValue = context.ReadValue<Vector2>();
    }
}
