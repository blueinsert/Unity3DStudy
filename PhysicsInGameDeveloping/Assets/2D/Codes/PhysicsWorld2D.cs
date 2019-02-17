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
            if (m_objsForAdd.Count != 0)
            {
                m_objs.AddRange(m_objsForAdd);
                m_objsForAdd.Clear();
            }
            foreach (var obj in m_objs)
            {
                obj.rigidBody.UpdateBodyEuler(deltaTime);
            }
            if (m_objs.Count > 1)
            {
                //避免彼此相互穿越
                int maxIterNum = 10;
                int iterNum = 0;
                bool tryNext = true;
                Dictionary<LogicGameObject2D, LogicGameObject2D> collisionPairs = new Dictionary<LogicGameObject2D, LogicGameObject2D>();
                while (iterNum < maxIterNum && tryNext)
                {
                    for (int i = 0; i < m_objs.Count; i++)
                    {
                        tryNext = false;
                        for (int j = i + 1; j < m_objs.Count; j++)
                        {
                            CollisionResult collisionResult;
                            PhysicsUtils2D.CollideTest(m_objs[i].collider, m_objs[j].collider, out collisionResult);
                            if (collisionResult != null && (collisionResult.m_type == CollisionResultType.Collision || collisionResult.m_type == CollisionResultType.Penetrating))
                            {
                                // m_objs[i].rigidBody.m_position += collisionResult.m_recoverVector;
                                //var rv = m_objs[i].rigidBody.m_velocity - m_objs[j].rigidBody.m_velocity;
                                var recoverDir = collisionResult.m_recoverVector.normalized;
                                var recoverDist = collisionResult.m_recoverVector.magnitude;

                                var rv1 = Vector3.Dot(recoverDir, m_objs[i].rigidBody.m_velocity);
                                var rv2 = Vector3.Dot(-recoverDir, m_objs[j].rigidBody.m_velocity);
                                //UnityEngine.Debug.Assert(rv1 + rv2 < 0, "rv1 + rv2 >= 0");
                                var s1 = m_objs[i].rigidBody.m_speed;
                                var s2 = m_objs[j].rigidBody.m_speed;
                                //m_objs[i].rigidBody.m_position += collisionResult.m_recoverVector * (rv1 / (rv1 + rv2));
                                //m_objs[j].rigidBody.m_position += -collisionResult.m_recoverVector * (rv2 / (rv1 + rv2));
                                m_objs[i].rigidBody.m_position += collisionResult.m_recoverVector * (s1 / (s1 + s2));
                                m_objs[j].rigidBody.m_position += -collisionResult.m_recoverVector * (s2 / (s1 + s2));
                                tryNext = true;
                                collisionPairs[m_objs[i]] = m_objs[j];
                                //计算碰撞点，应用碰撞冲量

                            }
                        }
                    }
                    iterNum++;
                }//while
                foreach (var collisionPair in collisionPairs)
                {
                    List<CollisionPointInfo> collisionPointInfos = new List<CollisionPointInfo>();
                    PhysicsUtils2D.GetCollisionPointInfo(collisionPair.Key.collider, collisionPair.Value.collider, out collisionPointInfos);
                    if (collisionPointInfos.Count != 0)
                    {
                        foreach (var collisionPointInfo in collisionPointInfos)
                        {
                            ApplyImpulse(collisionPair.Key, collisionPair.Value, collisionPointInfo);
                        }
                    }
                }
                UnityEngine.Debug.Log("iterNum:" + iterNum);
            }
            DebugDraw(deltaTime);
        }
    }
}
