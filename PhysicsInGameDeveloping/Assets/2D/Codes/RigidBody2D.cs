using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class ForceDesc
    {
        public int id;
        public Vector3 m_force;
        public Vector3 m_pointLocal;
    }
    public class RigidBody2D
    {
        Vector3 _G = new Vector3(0, -9.8f, 0);
        public float m_mass;
        public float massInverse
        {
            get
            {
                if (m_mass <= 0)
                {
                    return 0;
                }
                else
                {
                    return 1 / m_mass;
                }
            }
        }
        public float m_inertia;//转动惯量

        public float inertiaInverse
        {
            get
            {
                if (m_inertia <= 0)
                {
                    return 0;
                }
                else
                {
                    return 1 / m_inertia;
                }
            }
        }
        public Vector3 m_position; //global coordinates
        public Vector3 m_velocity; //global coordinates
        public Vector3 m_velocityLocal; //local coordinates
        public Vector3 m_angularVelocityLocal; //角速度，本地坐标系, 单位：rad/s
        public float m_speed;
        public float m_orientation; //朝向，绕z轴旋转的角度，单位：deg
        public Dictionary<int, ForceDesc> m_externForces;
        Vector3 m_forces; //受力和
        Vector3 m_moment; //力矩和

        public RigidBody2D()
        {
            m_mass = 1;
            m_inertia = 10;
            m_position.x = 0; m_position.y = 0;
            m_orientation = 0;
            m_externForces = new Dictionary<int, ForceDesc>();
        }

        public void ClearExternForces()
        {
            foreach (var force in m_externForces)
            {
                force.Value.m_force = Vector3.zero;
            }
        }

        public void SetForce(int id, Vector3 force, Vector3 pointLocal)
        {
            ForceDesc forceDesc;
            if (!m_externForces.TryGetValue(id, out forceDesc))
            {
                forceDesc = new ForceDesc();
                forceDesc.id = id;
                m_externForces[id] = forceDesc;
            }
            forceDesc.m_force = force;
            forceDesc.m_pointLocal = pointLocal;
        }

        void CalcLoads()
        {
            Vector3 sumForce = Vector3.zero;
            Vector3 sumMoment = Vector3.zero;
            sumForce += m_mass * _G;
            foreach (var forceDesc in m_externForces)
            {
                sumForce += forceDesc.Value.m_force;
                sumMoment += Vector3.Cross(forceDesc.Value.m_pointLocal.Rotate(m_orientation), forceDesc.Value.m_force);
            }
            m_forces = sumForce;
            m_moment = sumMoment;
        }

        public void UpdateBodyEuler(float dt)
        {
            if (this.m_mass == 0)
                return;
            Vector3 a = new Vector3();
            Vector3 dv = new Vector3();
            Vector3 ds = new Vector3();
            float aa;
            float dav;
            float dr;
            CalcLoads();
            //线性运动，显示欧拉积分
            a = m_forces / m_mass;
            dv = a * dt;
            m_velocity += dv;
            ds = m_velocity * dt;
            m_position += ds;
            //旋转运动
            aa = m_moment.z / m_inertia;
            dav = aa * dt;
            m_angularVelocityLocal.z += dav;
            dr = Mathf.Rad2Deg * m_angularVelocityLocal.z * dt;
            m_orientation += dr;

            m_speed = m_velocity.magnitude;
            m_velocityLocal = PhysicsWorld.VRotate2D(-m_orientation, m_velocity);
        }

    }
}
