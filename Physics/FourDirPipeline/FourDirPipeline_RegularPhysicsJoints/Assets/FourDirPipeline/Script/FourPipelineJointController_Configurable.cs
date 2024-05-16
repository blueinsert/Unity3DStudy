using UnityEngine;

public class FourPipelineJointController_Configurable : MonoBehaviour
{
    public Vector2 m_rotateLimit = new Vector2(-15, 15);

    public FourPipelineJointType m_type;

    public float m_targetValue;

    public float m_curValue;
    public float m_curAngleAbs;

    private ConfigurableJoint m_physicsJoint = null;

    public float m_rotateSpeed = 3.0f;
    public bool m_useProgressive = false;

    private void Awake()
    {
        SetParams();
    }

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
        m_physicsJoint = this.gameObject.GetComponent<ConfigurableJoint>();
        if(m_physicsJoint != null)
        {
            var low = m_physicsJoint.lowAngularXLimit;
            low.limit =-180;
            m_physicsJoint.lowAngularXLimit = low;

            var high = m_physicsJoint.highAngularXLimit;
            high.limit = 180;
            m_physicsJoint.highAngularXLimit = high;
        }
    }

    public void SetValue(float value)
    {
        value = (value + 1.0f) / 2;
        value = Mathf.Clamp01(value);
        m_targetValue = Mathf.Lerp(m_rotateLimit.x, m_rotateLimit.y, value);

        if (m_physicsJoint != null)
        {
            if(!m_useProgressive)
                m_physicsJoint.targetRotation = Quaternion.Euler(m_targetValue, 0, 0);
            
        } 
    }


    public void SetParams()
    {
        var joint = GetComponent<ConfigurableJoint>();
        if (joint != null)
        {
            var driver = joint.angularXDrive;
            driver.positionSpring = 3000000;
            driver.positionDamper = 60000;
            //driver.maximumForce = 10000000;
            joint.angularXDrive = driver;
        }

        var collider = GetComponent<Collider>();
        collider.excludeLayers = LayerMask.NameToLayer("Pipeline");

        var rigid = GetComponent<Rigidbody>();
        rigid.drag = 0.2f;
        rigid.angularDrag = 0.2f;
    }

    float ConvertAngle(float angle)
    {
        if (angle >= 0 && angle < 180)
            return angle;
        if (angle >= 180 && angle < 360)
            return angle - 360;
        return angle;
    }

    float GetCurValue()
    {
        var joint = GetComponent<ConfigurableJoint>();
        if (joint == null)
        {
            return 0;
        }
        var parent = joint.connectedBody;
        if(parent != null)
        {
            m_curAngleAbs = Quaternion.Angle(this.transform.rotation, parent.transform.rotation);
            var rotateAxis = this.m_physicsJoint.axis.x * parent.transform.right + this.m_physicsJoint.axis.y*parent.transform.up + this.m_physicsJoint.axis.z * parent.transform.forward;
            var forward1 = parent.transform.up;
            var forward2 = this.transform.up;
            var temp1 = Vector3.Cross(forward2, forward1);
            var temp2 = Vector3.Dot(temp1, rotateAxis);
            var sign = temp2 > 0 ? 1 : -1;
            return sign * m_curAngleAbs;
            var parentEuler = parent.transform.eulerAngles;
            if (m_type == FourPipelineJointType.Horizonal)
            {
                return -(ConvertAngle(this.transform.eulerAngles.z) - ConvertAngle(parentEuler.z));
            }
            else if (m_type == FourPipelineJointType.Vertical)
            {
                return -(ConvertAngle(this.transform.eulerAngles.x) - ConvertAngle(parentEuler.x));
            }
        }
        return 0;
    }

    private void Update()
    {
        
        
    }

    public void FixedUpdate()
    {
        m_curValue = GetCurValue();
        if (m_useProgressive) {
            
            if (Mathf.Abs(m_targetValue - m_curValue) > 0.15f)
            {
                var sign = Mathf.Sign(m_targetValue - m_curValue);
                var value = m_curValue + sign * m_rotateSpeed * Time.fixedDeltaTime;
                if (m_physicsJoint != null)
                {
                    Debug.Log(string.Format("value:{0}", value));
                    m_physicsJoint.targetRotation = Quaternion.Euler(value, 0, 0);
                }
            }
            else
            {
                m_physicsJoint.targetRotation = Quaternion.Euler(m_targetValue, 0, 0);
            }
        }
       
    }

}
