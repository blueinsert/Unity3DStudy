using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public enum CollisionResultType
    {
        Collision,
        Penetrating,
        Separate,
    }

    public class CollisionResult
    {
        public CollisionResultType m_type;
        public Vector3 m_collisionNormal;
        public Vector3 m_relativeVelocity;
        public RigidBody2D m_body1;
        public RigidBody2D m_body2;
    }

    public class PhysicsWorld
    {
        const float LINEARDRAGCOEFFICIENT = 0.25f;
        const float COEFFICIENTOFRESTITUTION = 0.5f;
        const float COLLISIONTOLERANCE = 0.01f;

        int m_frameCount = 0;
        RigidBody2D m_craft;
        RigidBody2D m_craft2;
        RigidBody2D m_craft1Copy;
        RigidBody2D m_craft2Copy;
        CollisionResult collisionResult = new CollisionResult();
        Vector3 m_collisionNormal;
        Vector3 m_relativeVelocity;
        float fcr = COEFFICIENTOFRESTITUTION;
        const float ctol = COLLISIONTOLERANCE;

        public void Initialize()
        {
            m_craft = new RigidBody2D();
            m_craft2 = new RigidBody2D();
            m_craft2.m_orientation = -90;
            m_craft2.m_position = new Vector3(-30, 30);
            m_craft1Copy = new RigidBody2D();
            m_craft2Copy = new RigidBody2D();
        }

        public void UpdateSimulation(float dt)
        {
            float deltaTime = dt;
            bool tryAgain = true;
            CollisionResultType collisionResultType = CollisionResultType.Separate;


            m_craft.SetThrusters(false, false);
            if (Input.GetKey(KeyCode.S))
            {
                m_craft.ModulateThrust(false);
            }
            if (Input.GetKey(KeyCode.W))
            {
                m_craft.ModulateThrust(true);
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_craft.SetThrusters(true, false);
            }
            if (Input.GetKey(KeyCode.D))
            {
                m_craft.SetThrusters(false, true);
            }
            float tol = 0.0001f;
            while (tryAgain && deltaTime > tol)
            {
                tryAgain = false;
                m_craft.CopyTo(m_craft1Copy);
                m_craft2.CopyTo(m_craft2Copy);
                m_craft1Copy.UpdateBodyEuler(deltaTime);
                m_craft2Copy.UpdateBodyEuler(deltaTime);
                collisionResultType = CheckForCollision(m_craft1Copy, m_craft2Copy, collisionResult);
                if (collisionResultType == CollisionResultType.Collision)
                {
                    Debug.Log("ApplyImpulse");
                    ApplyImpulse(m_craft1Copy, m_craft2Copy);
                }
                //Debug.Log(collisionResultType);
                /*
                collisionResult = CheckForCollision(m_craft1Copy, m_craft2Copy);
                if (collisionResult == CollisionResultType.Penetrating)
                {
                    deltaTime /= 2;
                    tryAgain = true;
                }
                else if (collisionResult == CollisionResultType.Collision)
                {
                    Debug.Log("ApplyImpulse");
                    ApplyImpulse(m_craft1Copy, m_craft2Copy);
                }
                */
            }
            /*
            if (collisionResult == CollisionResult.Collision && deltaTime < dt)
            {
                m_craft.UpdateBodyEuler(dt - deltaTime);
                m_craft2.UpdateBodyEuler(dt - deltaTime);
            }
            */
            m_craft1Copy.CopyTo(m_craft);
            m_craft2Copy.CopyTo(m_craft2);
            DrawCraft(m_craft, Color.blue);
            DrawCraft(m_craft2, Color.yellow);
            m_frameCount++;
        }

        CollisionResultType CheckForCollision(RigidBody2D body1, RigidBody2D body2, CollisionResult collisionResult)
        {
            bool haveNodeNode = false;
            bool haveNodeEdge = false;
            bool interpenetrating = false;
            collisionResult.m_type = CollisionResultType.Separate;
            //检查是否点与点之间发生碰撞
            for (int i = 0; i < 5 && !haveNodeNode; i++)
            {
                for (int j = 0; j < 5 && !haveNodeNode; j++)
                {
                    var p1 = body1.m_vertexListGlobal[i]; var p2 = body2.m_vertexListGlobal[j];
                    var p1Local = body1.m_vertexListLocal[i]; var p2Local = body2.m_vertexListLocal[j];
                    if (ArePointsEqual(p1, p2))
                    {
                        Debug.Log("nodeNode very near");
                        Vector3 collisionNormal = (body1.m_position - body2.m_position).normalized;
                        Vector3 v1 = body1.m_velocityLocal + Vector3.Cross(body1.m_angularVelocityLocal, p1Local);
                        v1 = VRotate2D(body1.m_orientation, v1);
                        Vector3 v2 = body2.m_velocityLocal + Vector3.Cross(body2.m_angularVelocityLocal, p2Local);
                        v2 = VRotate2D(body2.m_orientation, v2);
                        Vector3 relativeVelocity = v1 - v2;
                        float vrn = Vector3.Dot(relativeVelocity, collisionNormal);
                        if (vrn < 0)
                        {
                            Debug.Log("haveNodeNode");
                            haveNodeNode = true;
                            m_collisionNormal = collisionNormal;
                            m_relativeVelocity = relativeVelocity;
                            body1.m_collisionPoint = p1 - body1.m_position;
                            body2.m_collisionPoint = p2 - body2.m_position;
                        }
                    }
                }
            }
            if (!haveNodeNode)
            {
                //检查点与边是否发生碰撞
                for (int i = 0; i < 5 && !haveNodeEdge; i++)
                {
                    for (int j = 0; j < 5 && !haveNodeEdge; j++)
                    {
                        Vector3 edge;
                        if (j == 4)
                        {
                            edge = body2.m_vertexListGlobal[0] - body2.m_vertexListGlobal[4];
                        }
                        else
                        {
                            edge = body2.m_vertexListGlobal[j + 1] - body2.m_vertexListGlobal[j];
                        }
                        Vector3 u = edge.normalized;
                        Vector3 p = body1.m_vertexListGlobal[i] - body2.m_vertexListGlobal[j];
                        float proj = Vector3.Dot(p, u);
                        float dist = Vector3.Cross(p, u).magnitude;
                        if (proj > 0 && proj <= edge.magnitude && dist <= ctol)
                        {
                            Debug.Log("node edge very near");
                            Vector3 collisionNormal = Vector3.Cross(Vector3.Cross(u, p), u).normalized;
                            Vector3 point = body1.m_vertexListGlobal[i];
                            Vector3 v1 = body1.m_velocity + Vector3.Cross(body1.m_angularVelocityLocal, point - body1.m_position);
                            Vector3 v2 = body2.m_velocity + Vector3.Cross(body2.m_angularVelocityLocal, point - body2.m_position);
                            Vector3 relativeVelocity = v1 - v2;
                            float vrn = Vector3.Dot(relativeVelocity, collisionNormal);
                            if (vrn < 0)
                            {
                                Debug.Log("haveNodeEdge");
                                haveNodeEdge = true;
                                m_collisionNormal = collisionNormal;
                                m_relativeVelocity = relativeVelocity;
                                body1.m_collisionPoint = point - body1.m_position;
                                body2.m_collisionPoint = point - body2.m_position;
                            }
                        }
                    }
                }
            }
            /*
            if (!haveNodeEdge && !haveNodeNode)
            {
                //检查是否发生了穿越
                for (int i = 0; i < 5 && !interpenetrating; i++)
                {
                    for (int j = 0; j < 5 && !interpenetrating; j++)
                    {
                        Vector3 point = body1.m_vertexListGlobal[i];
                        Vector3 edge;
                        if (j == 4)
                        {
                            edge = body2.m_vertexListGlobal[0] - body2.m_vertexListGlobal[4];
                        }
                        else
                        {
                            edge = body2.m_vertexListGlobal[j + 1] - body2.m_vertexListGlobal[j];
                        }
                        Vector3 p = point - body2.m_vertexListGlobal[j];
                        if (Vector3.Dot(p, edge) < 0)
                            interpenetrating = true;
                    }
                }
            }
            */

            CollisionResultType result = CollisionResultType.Separate;
            if (interpenetrating)
            {
                result = CollisionResultType.Penetrating;
            }
            else if (haveNodeNode || haveNodeEdge)
            {
                result = CollisionResultType.Collision;
            }
            return result;
        }

        void ApplyImpulse(RigidBody2D body1, RigidBody2D body2)
        {
            float j = -(1 + fcr) * Vector3.Dot(m_relativeVelocity, m_collisionNormal) /
                (
                1 / body1.m_mass + 1 / body2.m_mass +
                Vector3.Dot(m_collisionNormal, Vector3.Cross(Vector3.Cross(body1.m_collisionPoint, m_collisionNormal) / body1.m_inertia, body1.m_collisionPoint)) +
                Vector3.Dot(m_collisionNormal, Vector3.Cross(Vector3.Cross(body2.m_collisionPoint, m_collisionNormal) / body2.m_inertia, body2.m_collisionPoint))
                );
            body1.m_velocity += (j * m_collisionNormal) / body1.m_mass;
            body1.m_angularVelocityLocal += Vector3.Cross(body1.m_collisionPoint, j * m_collisionNormal) / body1.m_inertia;
            body2.m_velocity += (j * m_collisionNormal) / body2.m_mass;
            body2.m_angularVelocityLocal -= Vector3.Cross(body2.m_collisionPoint, j * m_collisionNormal) / body2.m_inertia;
        }

        public static bool ArePointsEqual(Vector3 p1, Vector3 p2)
        {
            if (Mathf.Abs(p1.x - p2.x) <= ctol &&
                Mathf.Abs(p1.y - p2.y) <= ctol &&
                Mathf.Abs(p1.z - p2.z) <= ctol)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void DrawCraft(RigidBody2D craft, Color color)
        {
            craft.Draw(color);
        }

        public static Vector3 VRotate2D(float angle, Vector3 u)
        {
            float x, y;
            x = u.x * Mathf.Cos(Mathf.Deg2Rad * angle) - u.y * Mathf.Sin(Mathf.Deg2Rad * angle);
            y = u.x * Mathf.Sin(Mathf.Deg2Rad * angle) + u.y * Mathf.Cos(Mathf.Deg2Rad * angle);
            return new Vector3(x, y, 0);
        }


    }
}
