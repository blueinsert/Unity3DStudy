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
                obj1.massInverse + obj2.massInverse +
                Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(collisionPoint1Local, collisionNormal), collisionPoint1Local)) * obj1.inertiaVerse +
                Vector3.Dot(collisionNormal, Vector3.Cross(Vector3.Cross(collisionPoint2Local, collisionNormal), collisionPoint2Local)) * obj2.inertiaVerse
                );
            obj1.rigidBody.m_velocity += (j * collisionNormal) * obj1.massInverse;
            obj1.rigidBody.m_angularVelocityLocal += Vector3.Cross(collisionPoint1Local, j * collisionNormal) * obj1.inertiaVerse;
            obj2.rigidBody.m_velocity += (-j * collisionNormal) * obj2.massInverse;
            obj2.rigidBody.m_angularVelocityLocal += Vector3.Cross(collisionPoint2Local, -j * collisionNormal) * obj2.inertiaVerse;
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
                        CollisionResult collisionResult;
                        PhysicsUtils2D.CollideTest(m_objs[i].collider, m_objs[j].collider, out collisionResult);
                        if (collisionResult != null && (collisionResult.m_type == CollisionResultType.Collision || collisionResult.m_type == CollisionResultType.Penetrating))
                        {
                            m_objs[i].rigidBody.m_position += collisionResult.m_recoverVector * (m_objs[i].rigidBody.m_speed / (m_objs[i].rigidBody.m_speed + m_objs[j].rigidBody.m_speed));
                            m_objs[j].rigidBody.m_position += -collisionResult.m_recoverVector * (m_objs[j].rigidBody.m_speed / (m_objs[i].rigidBody.m_speed + m_objs[j].rigidBody.m_speed));
                            PhysicsUtils2D.GetCollisionPointInfo(m_objs[i].collider, m_objs[j].collider, out collisionResult.m_pointInfos);
                            if (collisionResult.m_pointInfos.Count != 0)
                            {
                                foreach(var collisionPointInfo in collisionResult.m_pointInfos)
                                {
                                    ApplyImpulse(m_objs[i], m_objs[j], collisionPointInfo);
                                }
                            }
                        }
                    }
                }
            }
           
            DebugDraw(deltaTime);
        }
    }
}
