using System.Collections;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;

namespace bluebean
{
    public class PhysicsWorld2D
    {
        private List<LogicGameObject2D> m_objs = new List<LogicGameObject2D>();
        private List<LogicGameObject2D> m_objsForAdd = new List<LogicGameObject2D>();

        public void AddObj(LogicGameObject2D obj)
        {
            m_objsForAdd.Add(obj);
        }

        private void DebugDraw(float deltaTime)
        {
            foreach (var obj in m_objs)
            {
                obj.collider.DebugDraw(UnityEngine.Color.blue, deltaTime);
            }
        }
        void ApplyImpulse(LogicGameObject2D obj1, LogicGameObject2D obj2, CollisionPointInfo collisionPointInfo)
        {
            var collisionNormal = collisionPointInfo.m_collisionNormal;
            var relativeVelocity = collisionPointInfo.m_relativeVelocity;
            var collisionPoint = collisionPointInfo.m_point;
            float j = -(1 + 0.5f) * Vector3.Dot(relativeVelocity, collisionNormal) /
                (
                1 / obj1.mass + 1 / obj2.mass +
                Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(collisionPoint, collisionNormal), collisionPoint)) / obj1.inertia +
                Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(collisionPoint, collisionNormal), collisionPoint)) / obj2.inertia
                );
            obj1.rigidBody.m_velocity += (j * collisionNormal) / obj1.mass;
            obj1.rigidBody.m_angularVelocityLocal += Vector3.Cross(body1.m_collisionPoint, j * m_collisionNormal) / body1.m_inertia;
            body2.m_velocity += (-j * m_collisionNormal) / body2.m_mass;
            body2.m_angularVelocityLocal += Vector3.Cross(body2.m_collisionPoint, -j * m_collisionNormal) / body2.m_inertia;
        }

        public void Update(float deltaTime)
        {
            if(m_objsForAdd.Count != 0)
            {
                m_objs.AddRange(m_objsForAdd);
                m_objsForAdd.Clear();
            }
            foreach(var obj in m_objs)
            {
                obj.rigidBody.UpdateBodyEuler(deltaTime);
            }
            if (m_objs.Count > 1)
            {
                for (int i = 0; i < m_objs.Count; i++)
                {
                    for (int j = i + 1; j < m_objs.Count; j++)
                    {
                        List<CollisionPointInfo> collisionPointInfos;
                        var result = PhysicsUtils.CollideTest(m_objs[i].collider, m_objs[j].collider, out collisionPointInfos);
                        if(result == CollisionResultType.Collision)
                        {

                        }
                    }
                }
            }
           
            DebugDraw(deltaTime);
        }
    }
}
