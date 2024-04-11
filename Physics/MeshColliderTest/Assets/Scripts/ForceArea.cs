using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceArea : MonoBehaviour
{
    public bool m_isEnable = false;

    private Vector3 m_dir;

    public float m_speed = 0;

    public List<RopeJointController> m_joints = new List<RopeJointController>();

    // Start is called before the first frame update
    void Start()
    {
        m_joints.Clear();
        m_dir = this.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isEnable)
            return;
        foreach (var ctrl in m_joints)
        {
            var pos = ctrl.transform.position;
            pos += m_dir * m_speed * Time.deltaTime;
            ctrl.transform.position = pos;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!m_isEnable)
            return;
        if (other.gameObject == null)
        {
            Debug.Log(string.Format("ForceArea:OnTriggerEnter {0}", other));
            return;
        }
        Debug.Log(string.Format("ForceArea:OnTriggerEnter {0}", other.gameObject.name));
        RopeJointController ctrl = other.GetComponent<RopeJointController>();
        if(ctrl==null )
        {
            //Debug.LogError(string.Format("ForceArea:OnTriggerEnter ctrl==null", other));
            return;
        }
        if (ctrl.m_rigidbody == null)
        {
            Debug.LogError(string.Format("ForceArea:OnTriggerEnter ctrl.m_rigidbody=null", other));
        }
        ctrl.m_rigidbody.isKinematic = true;
        m_joints.Add(ctrl);
    }

    protected void OnTriggerExit(Collider other)
    {
        if (!m_isEnable)
            return;
        Debug.Log(string.Format("ForceArea:OnTriggerExit {0}", other.gameObject.name));
        RopeJointController ctrl = other.GetComponent<RopeJointController>();
        m_joints.Remove(ctrl);
        ctrl.m_rigidbody.isKinematic = false;
    }
}
