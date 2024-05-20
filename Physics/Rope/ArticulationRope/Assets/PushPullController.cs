using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PushPullState
{
    Push=1,
    Pull=-1,
    Fixed=0,
}

public class PushPullController : MonoBehaviour
{
    PushPullState m_state = PushPullState.Fixed;
    public float m_speed;
    ArticulationBody m_articulationBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            m_state = PushPullState.Pull;
        }else if(Input.GetKey(KeyCode.S))
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
        if(m_state!= PushPullState.Fixed)
        {
            var cur = m_articulationBody.jointPosition[0];
            var value = cur + ((int)m_state) * m_speed * Time.fixedDeltaTime;
            //Debug.Log(string.Format("cur:{0} {1} {2}", cur[0], 0, 0));
            var yDriver = m_articulationBody.yDrive;
            yDriver.target = value;
            m_articulationBody.yDrive = yDriver;
        }
        
    }
}
