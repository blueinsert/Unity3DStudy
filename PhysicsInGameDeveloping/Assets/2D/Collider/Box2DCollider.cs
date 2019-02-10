using System.Collections;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;

namespace bluebean
{
    public class Box2DCollider : Collider2D
    {
        private float m_l;
        private float m_w;
        public float length { get { return m_l; } }
        public float width { get { return m_w; } }
        private Vector3[] m_pointsLocal;
        private Vector3[] m_points;
        public Vector3[] pointsLocal { get { return m_pointsLocal; } }
        public Vector3[] points {
            get {
                for(int i = 0; i< m_pointsLocal.Length; i++)
                {
                    m_pointsLocal[i] = m_points[i].Rotate(m_owner.orientation) + m_owner.position;
                }
                return m_points;
            }
        }

        public Box2DCollider(float length, float width)
        {
            this.m_l = length;
            this.m_w = width;
            m_pointsLocal = new Vector3[4];
            m_pointsLocal[0] = new Vector3(-m_l / 2, m_w / 2, 0);
            m_pointsLocal[1] = new Vector3(m_l / 2, m_w / 2, 0);
            m_pointsLocal[2] = new Vector3(m_l / 2, -m_w / 2, 0);
            m_pointsLocal[3] = new Vector3(-m_l / 2, -m_w / 2, 0);
            m_points = new Vector3[m_pointsLocal.Length];
            for(int i = 0; i < m_pointsLocal.Length; i++)
            {
                m_points[i] = new Vector3();
            }
        }

        public override void DebugDraw(UnityEngine.Color color, float deltaTime)
        {
            var points = this.m_points;
            for(int i = 0; i < 4; i++)
            {
                if(i == 0)
                {
                    UnityEngine.Debug.DrawLine(points[3], points[0], color, deltaTime);
                }
                else
                {
                    UnityEngine.Debug.DrawLine(points[i+1], points[i], color, deltaTime);
                }
            }
        }
    }
}
