using UnityEngine;
using System.Collections.Generic;
using RVO;

public class GroupController : MonoBehaviour
{
    public bool m_adjustCamera = true;
    RVO.Simulator m_sim;

    Camera m_cam;

    public void Start()
    {
        m_cam = Camera.main;

        m_sim = RVO.Simulator.Instance;
    }

    public void Update()
    {

        if (m_adjustCamera)
        {
            int count = m_sim.GetAgentCount();

            float max = 0;
            for (int i = 0; i < count; i++)
            {
                var pos = m_sim.GetAgentPos(i);
                float d = Mathf.Max(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
                if (d > max)
                {
                    max = d;
                }
            }

            float hh = max / Mathf.Tan((m_cam.fieldOfView * Mathf.Deg2Rad / 2.0f));
            float hv = max / Mathf.Tan(Mathf.Atan(Mathf.Tan(m_cam.fieldOfView * Mathf.Deg2Rad / 2.0f) * m_cam.aspect));

            var yCoord = Mathf.Max(hh, hv) * 1.1f;
            yCoord = Mathf.Max(yCoord, 20);
            m_cam.transform.position = Vector3.Lerp(m_cam.transform.position, new Vector3(0, yCoord, 0), Time.smoothDeltaTime * 2);
        }
    }

}


