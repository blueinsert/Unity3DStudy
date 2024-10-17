using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBizerCurve : MonoBehaviour
{
    public GameObject m_player;

    public BizerCurve m_curve = null;
    public float m_speed = 10f;

    public CurvePosition m_curPosition = null;

    private int m_curveSegment;
    private float m_curCurveLen;
    private float m_speedT;

    private bool m_isMove = false;

    // Start is called before the first frame update
    void Start()
    {
        var len = m_curve.GetCurveLength();
        Debug.Log(string.Format("curve len:{0}", len));
        var segment = m_curve.GetCurveSegmentCount();
        Debug.Log(string.Format("curve seg:{0}", segment));
        m_curveSegment = segment;
        m_curPosition = new CurvePosition()
        {
            m_segmentIndex = 0,
            m_t = 0,
        };
        if (m_curveSegment > 0)
        {
            m_curCurveLen = m_curve.GetCurveLength(0);
            StartMove();
        }
    }

    void StartMove()
    {
        m_speedT = m_curCurveLen / m_speed;
        m_isMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMove)
        {
            var pos = m_curve.GetPosition(m_curPosition);
            m_player.transform.position = pos;
            var tangent = m_curve.GetTangent(m_curPosition);
            m_player.transform.LookAt(pos + tangent);

            m_curPosition.m_t += m_speedT * Time.deltaTime;
            if (m_curPosition.m_t > 1.0f)
            {
                m_curPosition.m_segmentIndex++;
                m_curPosition.m_t = 0f;
                if (m_curPosition.m_segmentIndex >= m_curveSegment)
                {
                    //m_isMove = false;
                    m_curPosition.m_segmentIndex = 0;
                    m_curPosition.m_t = 0;
                }
                else
                {
                    m_curCurveLen = m_curve.GetCurveLength(m_curPosition.m_segmentIndex);
                    m_speedT = m_speed / m_curCurveLen;
                }
            }
        }
    }
}
