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

    private HingeJoint m_hingeJoint = null;

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

    public void InitPhysicsComps(FourPipelineJointController prev,int index, int all)
    {
        var rigibody = this.gameObject.GetComponent<Rigidbody>();
        //rigibody.mass = 0.001f;
        //rigibody.useGravity = true;
        if (prev != null)
        {
            var hingeJoint = this.gameObject.GetComponent<HingeJoint>();
            hingeJoint.connectedBody = prev.GetComponent<Rigidbody>();
            if(prev.m_type == FourPipelineJointType.Horizonal)
            {
                hingeJoint.axis = new Vector3(0, 0, 90);
            }
            hingeJoint.useLimits = true;
            hingeJoint.limits = new JointLimits() {
                min = m_rotateLimit.x,
                max = m_rotateLimit.y,
            };
            //hingeJoint.useSpring = true;
            //hingeJoint.spring = new JointSpring() {
            //    spring = 10000,
            //    damper = 100,
            //    targetPosition = 0,
            //};
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
            var springSetting = m_hingeJoint.spring;
            springSetting.targetPosition = m_targetValue;
            m_hingeJoint.spring = springSetting;

            //m_hingeJoint.limits = new JointLimits()
            //{
            //    min = m_targetValue,
            //    max = m_targetValue,
            //};
        } 
    }

    public void FixedUpdate()
    {
        
    }

}
