using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class Rope2D
    {
        const float SPRING_D = 20;
        const float SPRING_K = 1000;

        List<Particle> m_objects = new List<Particle>();
        List<Spring> m_springs = new List<Spring>();

        public  Rope2D(Vector3 root, Vector3 dir, float length, int springCount) {
            dir.Normalize();
            int objectCount = springCount + 1;
            for(int i = 0; i < objectCount; i++)
            {
                var p = new Particle();
                p.m_position = root + dir * (length / springCount) * i;
                p.m_mass = 1;
                if(i == 0)
                {
                    p.m_mass = 0;
                }
                m_objects.Add(p);
            }
            for(int i =0; i < springCount; i++)
            {
                Spring spring = new Spring();
                spring.m_end1 = i;
                spring.m_end2 = i + 1;
                spring.m_k = SPRING_K;
                spring.m_d = SPRING_D;
                spring.m_initialLength = (m_objects[spring.m_end1].m_position - m_objects[spring.m_end2].m_position).magnitude;
                m_springs.Add(spring);
            }
        }


        public void UpdateSimulation(float deltaTime)
        {
            for(int i = 0; i< m_objects.Count; i++)
            {
                m_objects[i].m_externForce = Vector3.zero;
            }
            for(int i = 0; i < m_springs.Count; i++)
            {
                var spring = m_springs[i];
                var pt1 = m_objects[spring.m_end1].m_position;
                var v1 = m_objects[spring.m_end1].m_velocity;
                var pt2 = m_objects[spring.m_end2].m_position;
                var v2 = m_objects[spring.m_end2].m_velocity;
                var r = (pt2 - pt1).normalized;
                var vr = v2 - v1;
                var deltaD = (pt2 - pt1).magnitude - spring.m_initialLength;
                var springForce = r * spring.m_k * deltaD + spring.m_d * Vector3.Dot(vr, r)*r;
                m_objects[spring.m_end1].m_externForce += springForce;
                m_objects[spring.m_end2].m_externForce -= springForce;
            }
            foreach(var obj in m_objects)
            {
                obj.UpdateBodyEuler(deltaTime);
            }
        }

        public void Render(Color color)
        {
            for(int i = 0; i < m_springs.Count; i++)
            {
                var pt1 = m_objects[m_springs[i].m_end1].m_position;
                var pt2 = m_objects[m_springs[i].m_end2].m_position;
                Debug.DrawLine(pt1, pt2, color, Time.deltaTime);
            }
        }
    }
}
