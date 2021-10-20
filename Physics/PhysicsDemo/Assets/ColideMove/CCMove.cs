using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCMove : MonoBehaviour
{
    public CharacterController m_cc;

    void Start()
    {
        
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
