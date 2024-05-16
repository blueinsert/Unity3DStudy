using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PID
{
    public float m_kp;
    public float m_ki;
    public float m_kd;

    public float m_output;

    public float m_lastError;

    public float m_iError;
    public float m_dError;

    public float GetOutput()
    {
        return m_output;
    }

    public void Setup(float kp,float ki,float kd)
    {
        m_kp = kp;
        m_ki = ki;
        m_kd = kd;
    }

    public void Update(float dt, float expected, float curValue)
    {
        var error = expected - curValue;
        m_iError += error*dt;
        m_dError = (error - m_lastError)/dt;
        m_lastError = error;

        m_output = m_kp * error + m_ki * m_iError + m_kd * m_dError;
    }
}
