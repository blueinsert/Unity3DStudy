using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class Particle
    {
        Vector3 G = new Vector3(0, -9.8f, 0);
        public float m_mass = 1;
        public Vector3 m_position;
        public Vector3 m_velocity;
        public Vector3 m_externForce;
        Vector3 m_forces; //受力和

        public Particle()
        {
            m_position = Vector3.zero;
            m_velocity = Vector3.zero;
            m_externForce = Vector3.zero;
            m_forces = Vector3.zero;
        }

        void CalcLoads()
        {
            m_forces = m_mass * G + m_externForce;
        }

        public void UpdateBodyEuler(float dt)
        {
            Vector3 a = new Vector3();
            Vector3 dv = new Vector3();
            Vector3 ds = new Vector3();
            CalcLoads();
            if (m_mass == 0)
                return;
            //线性运动，显示欧拉积分
            a = m_forces / m_mass;
            dv = a * dt;
            m_velocity += dv;
            ds = m_velocity * dt;
            m_position += ds;
        }
    }
}
