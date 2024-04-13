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

    public int m_index;

    public void SetIndex(int index)
    {
        m_index = index;
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

    public void InitPhysicsComps(FourPipelineJointController prev)
    {
        var rigibody = this.gameObject.AddComponent<Rigidbody>();
        rigibody.mass = 0.01f;
        //rigibody.useGravity = false;
        rigibody.isKinematic = true;
    }

    public void SetRotateLimit(float max)
    {
        m_rotateLimit = new Vector2(-max, max);
    }

    public void SetValue(float value)
    {
        if (m_index == 0)
            return;
        value = (value + 1.0f) / 2;
        value = Mathf.Clamp01(value);
        m_targetValue = Mathf.Lerp(m_rotateLimit.x, m_rotateLimit.y, value);

        if(m_type == FourPipelineJointType.Horizonal)
        {
            var ruo = Quaternion.Euler(m_targetValue, 0, 0);
            this.transform.localRotation = ruo;
        }else if(m_type == FourPipelineJointType.Vertical)
        {
            var ruo = Quaternion.Euler(0, 0, m_targetValue);
            this.transform.localRotation = ruo;
        }
    }
    
}
