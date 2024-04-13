using UnityEngine;

public class FourPipelineJointController_Configurable : MonoBehaviour
{
    public Vector2 m_rotateLimit = new Vector2(-15, 15);

    public FourPipelineJointType m_type;

    public float m_targetValue;

    private ConfigurableJoint m_physicsJoint = null;

    public void SetLinkType(FourPipelineJointType type)
    {
        m_type = type;
    }

    public void HideLinks()
    {

    }

    public void InitPhysicsComps(FourPipelineJointController_Configurable prev,int index, int all)
    {
        var rigibody = this.gameObject.GetComponent<Rigidbody>();
        if (prev != null)
        {
            var physicJoint = this.gameObject.GetComponent<ConfigurableJoint>();
            physicJoint.connectedBody = prev.GetComponent<Rigidbody>();
            if(prev.m_type == FourPipelineJointType.Horizonal)
            {
                physicJoint.axis = new Vector3(1, 0, 0);
            }else if(prev.m_type == FourPipelineJointType.Vertical)
            {
                physicJoint.axis = new Vector3(0, 0, 1);
            }

            m_physicsJoint = physicJoint;
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

        if (m_physicsJoint != null)
        {
            m_physicsJoint.targetRotation = Quaternion.Euler(m_targetValue, 0, 0);
            //var low = m_physicsJoint.lowAngularXLimit;
            //low.limit = m_targetValue;
            //m_physicsJoint.lowAngularXLimit = low;

            //var high = m_physicsJoint.highAngularXLimit;
            //high.limit = m_targetValue;
            //m_physicsJoint.highAngularXLimit = high;
        } 
    }

    public void FixedUpdate()
    {
        
    }

}
