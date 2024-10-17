using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPullController2 : MonoBehaviour
{
    public PushPullState m_state = PushPullState.Fixed;
    public float m_speed;
    ArticulationBody m_articulationBody;
    public bool m_kinematic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_state = PushPullState.Pull;
        }else if (Input.GetKey(KeyCode.UpArrow))
        {
            m_state = PushPullState.Push;
        }
        else
        {
            m_state = PushPullState.Fixed;
        }
        
    }

    private void FixedUpdate()
    {
        UpdatePullPush();
    }

    void UpdatePullPush()
    {
        if (m_articulationBody == null)
        {
            m_articulationBody = GetComponent<ArticulationBody>();
        }
        m_articulationBody.velocity = new Vector3(0,0, 0);
        if (m_state!= PushPullState.Fixed)
        {            
            if (!m_kinematic)
            {
                m_articulationBody.velocity = new Vector3(0, m_state == PushPullState.Push ? -m_speed : m_speed, 0);
            }
            else
            {
                var cur = m_articulationBody.transform.position;
                var value = cur + ((int)m_state) * m_speed * Time.fixedDeltaTime*new Vector3(0,-1,0);
                m_articulationBody.TeleportRoot(value, this.transform.rotation);
            }
        } 
    }
}
