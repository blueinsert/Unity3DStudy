using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class RigidBody2D
    {
        public float m_mass;
        public float m_inertia;//转动惯量
        public float m_inertiaInverse;//1/inertia
        public Vector3 m_position; //global coordinates
        public Vector3 m_velocity; //global coordinates
        public Vector3 m_velocityLocal; //local coordinates
        public Vector3 m_angularVelocityLocal; //角速度，本地坐标系, 单位：rad/s
        float m_speed;
        public float m_orientation; //朝向，绕z轴旋转的角度，单位：deg
        Vector3 m_forces; //受力和
        Vector3 m_moment; //力矩和
        float m_thrustForce; //主推力大小
        Vector3 m_lThrustLocal, m_rThrustLocal;//侧向推力
        //几何大小
        public float m_width;
        public float m_length;
        public float m_height;
        Vector3 m_dragCenterLocal; //阻力受力点
        Vector3 m_thrustCenterLocal;//推力受力点
        Vector3 m_leftThrustCenterLocal; //左侧推力受力点
        Vector3 m_rightThrustCenterLocal; //右侧推力受力点
        float m_projectedArea; //在与运动方向垂直平面的投影面积，计算阻力用

        public Vector3[] m_vertexListLocal;
        public Vector3[] m_vertexListGlobal;

        public Vector3 m_collisionPoint;

        const float THRUSTFORCE = 5.0f;//初始时的推力大小
        const float MAXTHRUST = 10.0f; //最大主推力
        const float MINTHRUST = 0f;  //最小主推力
        const float DTHRUST = 0.001f; //调节时，推力增量的最小值, deltaThrust
        const float STEERINGFORCE = 3f;//侧向推力大小
        const float LINEARDRAGCOEFFICIENT = 1.25f; //阻力系数

        public RigidBody2D() {
            m_mass = 100;
            m_inertia = 500;
            m_inertiaInverse = 1 / m_inertia;
            m_position.x = 0; m_position.y = 0;
            m_width = 10;
            m_length = 20;
            m_height = 5;
            m_orientation = 0;

            m_dragCenterLocal.x = 0;
            m_dragCenterLocal.y = -0.25f * m_length;
            m_dragCenterLocal.z = 0;

            m_thrustCenterLocal.x = 0;
            m_thrustCenterLocal.y = -0.5f * m_length;
            m_thrustCenterLocal.z = 0;

            m_leftThrustCenterLocal.x = -0.5f * m_width;
            m_leftThrustCenterLocal.y = 0.5f * m_length;
            m_leftThrustCenterLocal.z = 0;

            m_rightThrustCenterLocal.x = 0.5f * m_width;
            m_rightThrustCenterLocal.y = 0.5f * m_length;
            m_rightThrustCenterLocal.z = 0;

            m_projectedArea = (m_length + m_width) / 2 * m_height; //估算
            m_thrustForce = THRUSTFORCE;

            m_vertexListLocal = new Vector3[5];
            for(int i = 0; i < 5; i++)
            {
                m_vertexListLocal[i] = new Vector3();
            }
            m_vertexListLocal[0].x = -m_width / 2; m_vertexListLocal[0].y = m_length / 2;
            m_vertexListLocal[1].x = -m_width / 2; m_vertexListLocal[1].y = -m_length / 2;
            m_vertexListLocal[2].x = m_width / 2;  m_vertexListLocal[2].y = -m_length / 2;
            m_vertexListLocal[3].x = m_width / 2;  m_vertexListLocal[3].y = m_length / 2;
            m_vertexListLocal[4].x = 0;            m_vertexListLocal[4].y = m_length / 2 * 1.5f;
            m_vertexListGlobal = new Vector3[5];
            for(int i = 0; i < 5; i++)
            {
                m_vertexListGlobal[i] = new Vector3();
            }
        }

        public void CopyTo(RigidBody2D target)
        {
            target.m_mass = this.m_mass;
            target.m_inertia = this.m_inertia;
            target.m_inertiaInverse = this.m_inertiaInverse;
            target.m_position = this.m_position;
            target.m_width = this.m_width;
            target.m_length = this.m_length;
            target.m_height = this.m_height;
            target.m_orientation = this.m_orientation;

            target.m_dragCenterLocal = this.m_dragCenterLocal; 

            target.m_thrustCenterLocal = this.m_thrustCenterLocal;

            target.m_leftThrustCenterLocal = this.m_leftThrustCenterLocal;

            target.m_rightThrustCenterLocal = this.m_rightThrustCenterLocal;

            target.m_projectedArea = this.m_projectedArea;
            target.m_thrustForce = this.m_thrustForce;

            for (int i = 0; i < 5; i++)
            {
                target.m_vertexListLocal[i] = this.m_vertexListLocal[i];
            }
           
        }

        void CalcLoads() {
            Vector3 sumForceLocal = Vector3.zero;
            Vector3 sumMomentLocal = Vector3.zero;
            Vector3 mainThrustLocal = new Vector3(0, m_thrustForce, 0);

            //在摩檫力受力点计算摩檫力对于受力和及力矩和的贡献
            Vector3 localVelocity = Vector3.zero;
            Vector3 dragVector = Vector3.zero;
            var angularLinearVelocity = Vector3.Cross(m_angularVelocityLocal, m_dragCenterLocal);
            localVelocity = m_velocityLocal + angularLinearVelocity;
            var localSpeed = localVelocity.magnitude;
            if(localSpeed > 0.001f)
            {
                dragVector = -localVelocity.normalized;
                var dragForce = 0.5f * 1 * localSpeed * localSpeed * m_projectedArea * dragVector * LINEARDRAGCOEFFICIENT;
                sumForceLocal += dragForce;
                sumMomentLocal += Vector3.Cross(m_dragCenterLocal, dragForce);
            }

            //左侧推力
            sumForceLocal += m_lThrustLocal;
            sumMomentLocal += Vector3.Cross(m_leftThrustCenterLocal, m_lThrustLocal);
            //右侧推力
            sumForceLocal += m_rThrustLocal;
            sumMomentLocal += Vector3.Cross(m_rightThrustCenterLocal, m_rThrustLocal);
            //主推力
            sumForceLocal += mainThrustLocal;

            m_forces = PhysicsWorld.VRotate2D(m_orientation, sumForceLocal);
            m_moment = sumMomentLocal;

        }

        public void UpdateBodyEuler(float dt) {
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

        public void SetThrusters(bool l, bool r) {
            m_lThrustLocal.x = 0; m_lThrustLocal.y = 0;
            m_rThrustLocal.x = 0; m_rThrustLocal.y = 0;
            if (l)
                m_lThrustLocal.x = -STEERINGFORCE;
            if (r)
                m_rThrustLocal.x = STEERINGFORCE;
        }

        public void ModulateThrust(bool up) {
            float deltaThrust = up ? DTHRUST : -DTHRUST;
            m_thrustForce += deltaThrust;
            if (m_thrustForce > MAXTHRUST)
                m_thrustForce = MAXTHRUST;
            if (m_thrustForce < MINTHRUST)
                m_thrustForce = MINTHRUST;
        }

        public void Draw(Color color)
        {
            for(int i = 0; i < 5; i++)
            {
                m_vertexListGlobal[i] = PhysicsWorld.VRotate2D(m_orientation, m_vertexListLocal[i]);
                m_vertexListGlobal[i] = m_vertexListGlobal[i] + m_position;
            }
            Debug.DrawLine(m_vertexListGlobal[0], m_vertexListGlobal[1], color, Time.deltaTime);
            Debug.DrawLine(m_vertexListGlobal[1], m_vertexListGlobal[2], color, Time.deltaTime);
            Debug.DrawLine(m_vertexListGlobal[2], m_vertexListGlobal[3], color, Time.deltaTime);
            Debug.DrawLine(m_vertexListGlobal[3], m_vertexListGlobal[4], color, Time.deltaTime);
            Debug.DrawLine(m_vertexListGlobal[4], m_vertexListGlobal[0], color, Time.deltaTime);
        }
    }
}
