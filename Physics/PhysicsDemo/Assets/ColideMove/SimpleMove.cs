using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public Transform m_target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target == null)
            return;
        float speed = 2.0f;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0, 0, speed * Time.deltaTime);
        }else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
    }
}
