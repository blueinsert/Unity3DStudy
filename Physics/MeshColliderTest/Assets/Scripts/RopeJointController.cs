using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeJointController : MonoBehaviour
{

    public ConfigurableJoint m_configurableJoint = null;
    public Rigidbody m_rigidbody;

    public void InitPhysicsComps(RopeJointController prev,int index,int all)
    {
        this.gameObject.name = "joint" + index;
        var rigibody = this.gameObject.GetComponent<Rigidbody>();
        m_rigidbody = rigibody;
        //rigibody.mass = 0.01f;
        //rigibody.useGravity = true;
        if (prev != null)
        {
            var joint = this.gameObject.GetComponent<ConfigurableJoint>();
            joint.connectedBody = prev.GetComponent<Rigidbody>();
            m_configurableJoint = joint;
        }
        else
        {
            rigibody.isKinematic = true;
            var joint = this.gameObject.GetComponent<ConfigurableJoint>();
            Destroy(joint);
        }
    }


    public void FixedUpdate()
    {
        
    }

}
