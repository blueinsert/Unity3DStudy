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
            var collisionPoint1Local = collisionPointInfo.m_point - obj1.position;
            var collisionPoint2Local = collisionPointInfo.m_point - obj2.position;

            float j = -(1 + 0.5f) * Vector3.Dot(relativeVelocity, collisionNormal) /
                (
                1 / obj1.mass + 1 / obj2.mass +
                Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(collisionPoint1Local, collisionNormal), collisionPoint1Local)) / obj1.inertia +
                Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(collisionPoint2Local, collisionNormal), collisionPoint2Local)) / obj2.inertia
                );
            obj1.rigidBody.m_velocity += (j * collisionNormal) / obj1.mass;
            obj1.rigidBody.m_angularVelocityLocal += Vector3.Cross(collisionPoint1Local, j * collisionNormal) / obj1.inertia;
            obj2.rigidBody.m_velocity += (-j * collisionNormal) / obj2.mass;
            obj2.rigidBody.m_angularVelocityLocal += Vector3.Cross(collisionPoint2Local, -j * collisionNormal) / obj2.inertia;
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
