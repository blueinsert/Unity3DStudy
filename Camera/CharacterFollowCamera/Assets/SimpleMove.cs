using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public enum MoveType
    {
        CharacterController,
        Rigidbody,
        Transform,
    }

    public MoveType m_moveType;
    public Transform m_target;
    public CharacterController m_cc;
    public Rigidbody m_rigidbody;
    public float m_speed = 2.0f;

    void Awake()
    {
        switch (m_moveType)
        {
            case MoveType.CharacterController:
                break;
            case MoveType.Rigidbody:
                break;
            case MoveType.Transform:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Move(Vector3 vel)
    {
        switch (m_moveType)
        {
            case MoveType.CharacterController:
                m_cc.Move(vel * Time.deltaTime);
                break;
            case MoveType.Rigidbody:
                m_rigidbody.velocity = vel;
                break;
            case MoveType.Transform:
                transform.position += vel * Time.deltaTime;
                break;
        }
    }

    void UpdateMove()
    {
        var moveValue = InputHandler.Instance.MoveValue;
        if (moveValue.x == 0 && moveValue.y == 0)
            return;
        var forward = FollowCamera.Instance != null ? FollowCamera.Instance.GetForward() : Vector3.forward;
        forward.y = 0;
        var left = - Vector3.Cross(forward, Vector2.up);
        left = left.normalized;
        var dir = forward * moveValue.y + left * moveValue.x;
        dir = dir.normalized;
        var velocity = dir * m_speed;
        Move(velocity);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target == null)
            return;
        UpdateMove();
    }
}
