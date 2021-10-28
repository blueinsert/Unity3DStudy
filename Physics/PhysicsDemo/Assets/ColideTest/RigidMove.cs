using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMove : MonoBehaviour {
	public Rigidbody m_rigidbody;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (m_rigidbody == null)
            return;
        float speed = 2.0f;
        Vector3 vel = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            vel = new Vector3(0, 0, speed); 
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            vel = new Vector3(0, 0, -speed);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            vel = new Vector3(-speed, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            vel = new Vector3(speed, 0, 0);
        }
        m_rigidbody.velocity = vel;
        //m_rigidbody.MovePosition(this.transform.position + vel * Time.deltaTime);
    }
}
