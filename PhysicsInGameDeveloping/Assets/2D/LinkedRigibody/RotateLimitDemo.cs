using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean;

public class RotateLimitDemo : MonoBehaviour
{
    const float SPRING_D = 15;
    const float SPRING_K = 100;
    const float _SPRING_LENGTH = 0.1f;
    const float _OBJ_WIDTH = 1;
    const float _OBJ_Length = 10;
    const float _OBJ_COUNT = 15;
    List<RigidBody2D> m_objects = new List<RigidBody2D>();
    List<SpringEP> m_springs = new List<SpringEP>();

    private void Initial()
    {
        m_objects.Add(new RigidBody2D());
        m_objects.Add(new RigidBody2D());
        m_objects[0].m_orientation = 0;
        m_objects[0].m_position.x = _SPRING_LENGTH + _OBJ_Length / 2;
        m_objects[0].m_position.y = 0;
        m_objects[1].m_orientation = 90;
        m_objects[1].m_position.x = _SPRING_LENGTH + _OBJ_Length;
        m_objects[1].m_position.y = _SPRING_LENGTH + _OBJ_Length / 2;

        m_springs.Add(new SpringEP());
        m_springs.Add(new SpringEP());
        m_springs.Add(new SpringEP());
        m_springs[0].m_id = 0;
        m_springs[0].m_k = SPRING_K;
        m_springs[0].m_d = SPRING_D;
        m_springs[0].m_end1.m_index = -1;
        m_springs[0].m_end1.m_ptLocal = new Vector3(0, 0, 0);
        m_springs[0].m_end2.m_index = 0;
        m_springs[0].m_end2.m_ptLocal = new Vector3(-_OBJ_Length / 2, 0, 0);
        m_springs[0].m_initialLength = _SPRING_LENGTH;

        m_springs[1].m_id = 1;
        m_springs[1].m_k = SPRING_K;
        m_springs[1].m_d = SPRING_D;
        m_springs[1].m_end1.m_index = 0;
        m_springs[1].m_end1.m_ptLocal = new Vector3(_OBJ_Length/2, 0, 0);
        m_springs[1].m_end2.m_index = 1;
        m_springs[1].m_end2.m_ptLocal = new Vector3(-_OBJ_Length / 2, 0, 0);
        m_springs[1].m_initialLength = _SPRING_LENGTH;

        m_springs[2].m_id = 2;
        m_springs[2].m_k = SPRING_K;
        m_springs[2].m_d = SPRING_D;
        m_springs[2].m_end1.m_index = 0;
        m_springs[2].m_end1.m_ptLocal = new Vector3(0, 0, 0);
        m_springs[2].m_end2.m_index = 1;
        m_springs[2].m_end2.m_ptLocal = new Vector3(0, 0, 0);
        m_springs[2].m_initialLength = (m_objects[0].m_position - m_objects[1].m_position).magnitude;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initial();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSimulation(Time.deltaTime);
        Render(Color.blue);
    }

    private void UpdateSimulation(float deltaTime)
    {
        for (int i = 0; i < m_objects.Count; i++)
        {
            m_objects[i].ClearExternForces();
        }
        for (int i = 0; i < m_springs.Count; i++)
        {
            var spring = m_springs[i];
            Vector3 pt1;
            Vector3 v1;
            if (spring.m_end1.m_index == -1)
            {
                pt1 = spring.m_end1.m_ptLocal;
                v1 = Vector3.zero;
            }
            else
            {
                var obj = m_objects[spring.m_end1.m_index];
                pt1 = spring.m_end1.m_ptLocal.Rotate(obj.m_orientation) + obj.m_position;
                v1 = obj.m_velocity + Vector3.Cross(obj.m_angularVelocityLocal, spring.m_end1.m_ptLocal).Rotate(obj.m_orientation);
            }
            Vector3 pt2;
            Vector3 v2;
            if (spring.m_end2.m_index == -1)
            {
                pt2 = spring.m_end1.m_ptLocal;
                v2 = Vector3.zero;
            }
            else
            {
                var obj = m_objects[spring.m_end2.m_index];
                pt2 = spring.m_end2.m_ptLocal.Rotate(obj.m_orientation) + obj.m_position;
                v2 = obj.m_velocity + Vector3.Cross(obj.m_angularVelocityLocal, spring.m_end2.m_ptLocal).Rotate(obj.m_orientation);
            }

            var r = (pt2 - pt1).normalized;
            var vr = v2 - v1;
            var deltaD = (pt2 - pt1).magnitude - spring.m_initialLength;
            var springForce = r * spring.m_k * deltaD + spring.m_d * Vector3.Dot(vr, r) * r;
            if (spring.m_end1.m_index != -1)
            {
                m_objects[spring.m_end1.m_index].SetForce(spring.m_id, springForce, spring.m_end1.m_ptLocal);
            }
            if (spring.m_end2.m_index != -1)
            {
                m_objects[spring.m_end2.m_index].SetForce(spring.m_id, -springForce, spring.m_end2.m_ptLocal);
            }
        }
        foreach (var obj in m_objects)
        {
            obj.UpdateBodyEuler(deltaTime);
        }
    }

    private void Render(Color color)
    {
        for (int i = 0; i < m_springs.Count; i++)
        {
            var spring = m_springs[i];
            Vector3 pt1;
            if (spring.m_end1.m_index == -1)
            {
                pt1 = spring.m_end1.m_ptLocal;

            }
            else
            {
                var obj = m_objects[spring.m_end1.m_index];
                pt1 = spring.m_end1.m_ptLocal.Rotate(obj.m_orientation) + obj.m_position;
            }
            Vector3 pt2;
            if (spring.m_end2.m_index == -1)
            {
                pt2 = spring.m_end2.m_ptLocal;

            }
            else
            {
                var obj = m_objects[spring.m_end2.m_index];
                pt2 = spring.m_end2.m_ptLocal.Rotate(obj.m_orientation) + obj.m_position;
            }
            Debug.DrawLine(pt1, pt2, color, Time.deltaTime);
        }
        for (int i = 0; i < m_objects.Count; i++)
        {
            var obj = m_objects[i];
            Vector3[] locals = new Vector3[] { new Vector3(-_OBJ_Length / 2, _OBJ_WIDTH / 2), new Vector3(_OBJ_Length / 2, _OBJ_WIDTH / 2), new Vector3(_OBJ_Length / 2, -_OBJ_WIDTH / 2), new Vector3(-_OBJ_Length / 2, -_OBJ_WIDTH / 2) };
            Vector3[] globals = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                globals[j] = locals[j].Rotate(obj.m_orientation) + obj.m_position;
            }
            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                {
                    Debug.DrawLine(globals[0], globals[3], color, Time.deltaTime);
                }
                else
                {
                    Debug.DrawLine(globals[j], globals[j - 1], color, Time.deltaTime);
                }

            }
        }
    }
}
