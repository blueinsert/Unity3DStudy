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
    public bool m_isOnGround = false;
    public Vector3 m_vVel;
    public Vector3 m_hvel;
    public Vector3 m_frontDir;
    [Header("重力加速度")]
    public Vector3 m_g = new Vector3(0,-9.7f,0);
    [Header("行走速度")]
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
        m_frontDir = vel;
        m_frontDir.y = 0;
        m_frontDir.Normalize();
        m_target.transform.LookAt(m_target.transform.position + m_frontDir);
    }

    void UpdateMove()
    {
        var moveValue = InputHandler.Instance.MoveValue;
        m_hvel = Vector3.zero;
        if (moveValue.x != 0 || moveValue.y != 0) {
            var forward = FollowCamera.Instance != null ? FollowCamera.Instance.GetForward() : Vector3.forward;
            forward.y = 0;
            var left = -Vector3.Cross(forward, Vector2.up);
            left = left.normalized;
            var dir = forward * moveValue.y + left * moveValue.x;
            dir = dir.normalized;
            m_hvel = dir * m_speed;
        }
        if(!m_isOnGround)
            m_vVel += m_g * Time.deltaTime;
        else
        {
            m_vVel = Vector3.zero;
            m_vVel += m_g * Time.deltaTime;
        }
        Move(m_hvel + m_vVel);
    }

    void UpdateRotate()
    {
        var curForward = m_target.transform.forward;
        curForward.y = 0;
        var rotate = Quaternion.FromToRotation(curForward, m_frontDir);
        m_target.transform.rotation = Quaternion.Lerp(m_target.transform.rotation, rotate * m_target.transform.rotation, 2f * Time.deltaTime);
    }

    void UpdateIsOnGround()
    {
        if(m_moveType == MoveType.CharacterController)
        {
            m_isOnGround = m_cc.isGrounded;
        }else if(m_moveType == MoveType.Rigidbody || m_moveType == MoveType.Transform)
        {
            int layerMask = LayerMask.GetMask("Scene");
            var collides = Physics.OverlapSphere(this.transform.position, 0.03f, layerMask);
            m_isOnGround = collides.Length != 0;
        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target == null)
            return;
        UpdateMove();
        UpdateRotate();
        UpdateIsOnGround();
    }
}
