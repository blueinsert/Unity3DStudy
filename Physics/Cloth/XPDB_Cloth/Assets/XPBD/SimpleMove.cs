using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public Vector3 m_prePos;
    public Vector3 m_velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PreUpdate(float dt)
    {
        //m_velocity += Physics.gravity * dt;
        //var pos = this.transform.position;
        //pos += m_velocity * dt;
        //this.transform.position = pos;
    }

    public void MoveDelta(Vector3 delta)
    {
        //var pos = this.transform.position;
        //pos += delta;
        //this.transform.position = pos;
    }
}
