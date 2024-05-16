using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum FourPipelineJointType
{
    None = 0,
    Horizonal,
    Vertical,
}

public class FourPipelineJointController : MonoBehaviour
{
    public GameObject m_linkHNode;
    public GameObject m_linkVNode;

    public Vector2 m_rotateLimit = new Vector2(-15, 15);

    public FourPipelineJointType m_type;

    public float m_targetValue;

    public float m_speed = 5.0f;
    public bool m_useMotor = true;

    private HingeJoint m_hingeJoint = null;

    PID m_pid = new PID();

    private void Awake()
    {
        m_pid.Setup(0.5f, 0.5f, 0.7f);
    }

    public void SetLinkType(FourPipelineJointType type)
    {
        m_type = type;
        m_linkHNode.SetActive(m_type == FourPipelineJointType.Horizonal);
        m_linkVNode.SetActive(m_type == FourPipelineJointType.Vertical);
    }

    public void HideLinks()
    {
        m_linkHNode.SetActive(false);
        m_linkVNode.SetActive(false);
    }

    public void InitPhysicsComps(FourPipelineJointController prev, int index, int all)
    {
        var rigibody = this.gameObject.GetComponent<Rigidbody>();
        //rigibody.mass = 0.001f;
        //rigibody.useGravity = true;
        if (prev != null)
        {
            var hingeJoint = this.gameObject.GetComponent<HingeJoint>();
            hingeJoint.connectedBody = prev.GetComponent<Rigidbody>();
            if (prev.m_type == FourPipelineJointType.Horizonal)
            {
                hingeJoint.axis = new Vector3(0, 0, 90);
            }
            m_hingeJoint = hingeJoint;
        }
        else
        {
            rigibody.isKinematic = true;
        }
    }

    public void SetRotateLimit(float max)
    {
        m_rotateLimit = new Vector2(-max, max);
    }

    public void SetValue(float value)
    {
        value = (value + 1.0f) / 2;
        value = Mathf.Clamp01(value);
        m_targetValue = Mathf.Lerp(m_rotateLimit.x, m_rotateLimit.y, value);

        if (m_hingeJoint != null)
        {
            var limit = m_hingeJoint.limits;
            limit.min = -180;
            limit.max = 180;
            limit.bounciness = 9999999999;
            m_hingeJoint.limits = limit;
        }

        if (!m_useMotor)
        {
            if (m_hingeJoint != null)
            {
                var springSetting = m_hingeJoint.spring;
                springSetting.targetPosition = m_targetValue;
                m_hingeJoint.spring = springSetting;
                m_hingeJoint.useSpring = true;
                m_hingeJoint.useMotor = false;
                //m_hingeJoint.limits = new JointLimits()
                //{
                //    min = m_targetValue,
                //    max = m_targetValue,
                //};
            }

        }
        else
        {
            if (m_hingeJoint != null)
            {
                m_hingeJoint.useSpring = false;
                m_hingeJoint.useMotor = true;
            }
        }

    }

    public void FixedUpdate()
    {
        if (m_useMotor)
        {
            if (m_hingeJoint != null)
            {
                var cur = m_hingeJoint.angle;
                var bet = m_targetValue - cur;
                m_pid.Update(Time.deltaTime, m_targetValue, cur);
                var motor = m_hingeJoint.motor;
                motor.force = 100000000.0f;
                motor.targetVelocity = m_pid.GetOutput();

                m_hingeJoint.motor = motor;
                m_hingeJoint.useMotor = true;
                m_hingeJoint.useSpring = false;
            }
        }
        

    }

}
