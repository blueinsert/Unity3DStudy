using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCMove : MonoBehaviour
{
    public CharacterController m_cc;

    void Start()
    {
        
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var cc = hit.collider as CharacterController;
        if (cc != null)
        {
            var dir = cc.transform.position - this.transform.position;
            cc.Move(dir.normalized * 1.0f * Time.deltaTime);
            return;
        }
        Rigidbody rig = hit.collider.attachedRigidbody;
        if (rig != null && !rig.isKinematic)
        {
            var dir = rig.transform.position - this.transform.position;
            rig.velocity = dir.normalized * 1.0f;
            return;
        }
        Debug.Log(hit.collider);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_cc == null)
            return;
        float speed = 2.0f;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_cc.Move(new Vector3(0,0,speed) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            m_cc.Move(new Vector3(0,0,-speed) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_cc.Move(new Vector3(-speed, 0, 0) * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            m_cc.Move(new Vector3(speed, 0, 0) * Time.deltaTime);
        }
    }
}
