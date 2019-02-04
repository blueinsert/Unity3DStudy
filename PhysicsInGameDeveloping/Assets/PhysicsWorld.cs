using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public enum CollisionResult
    {
        Collision,
        Penetrating,
        AwayFrom,
    }

    public class PhysicsWorld
    {
        const float LINEARDRAGCOEFFICIENT = 0.25f;
        const float COEFFICIENTOFRESTITUTION = 0.5f;
        const float COLLISIONTOLERANCE = 2.0f;

        int m_frameCount = 0;
        RigidBody2D m_craft;
        RigidBody2D m_craft2;
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
        }

        public void UpdateSimulation(float dt)
        {
            float deltaTime = dt;
            bool tryAgain = true;
            CollisionResult collisionResult = CollisionResult.AwayFrom;
            RigidBody2D craft1Copy = new RigidBody2D();
            RigidBody2D craft2Copy = new RigidBody2D();

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
            float tol = 0.001f;
            while(tryAgain && deltaTime > tol)
            {
                tryAgain = false;
                m_craft.CopyTo(craft1Copy);
                m_craft2.CopyTo(craft2Copy);
                craft1Copy.UpdateBodyEuler(deltaTime);
                craft2Copy.UpdateBodyEuler(deltaTime);
                collisionResult = CheckForCollision(craft1Copy, craft2Copy);
                if(collisionResult == CollisionResult.Penetrating)
                {
                    deltaTime /= 2;
                    tryAgain = true;
                }else if(collisionResult == CollisionResult.Collision)
                {
                    ApplyImpulse(craft1Copy, craft2Copy);
                }
            }
            craft1Copy.CopyTo(m_craft);
            craft2Copy.CopyTo(m_craft2);
            if (collisionResult == CollisionResult.Collision && deltaTime < dt)
            {
                m_craft.UpdateBodyEuler(dt - deltaTime);
                m_craft2.UpdateBodyEuler(dt - deltaTime);
            }
            DrawCraft(m_craft, Color.blue);
            DrawCraft(m_craft2, Color.yellow);
            m_frameCount++;
        }

        CollisionResult CheckForCollision(RigidBody2D body1, RigidBody2D body2)
        {
            bool haveNodeNode = false;
            bool haveNodeEdge = false;
            bool interpenetrating = false;
            float r = Mathf.Sqrt(body1.m_length * body1.m_length / 4 + body2.m_length * body2.m_length / 4);
            Vector3 d = body1.m_position - body2.m_position;
            float s = d.magnitude - r;
            if(s <= ctol)
            {
                for(int i = 0; i < 5 && !haveNodeNode; i++)
                {
                    for(int j = 0; j < 5 && !haveNodeNode; j++)
                    {
                        var p1 = body1.m_vertexListGlobal[i]; var p2 = body2.m_vertexListGlobal[j];
                        var p1Local = body1.m_vertexListLocal[i]; var p2Local = body2.m_vertexListLocal[j];
                        if (ArePointsEqual(p1, p2))
                        {
                            body1.m_collisionPoint = p1Local;
                            body2.m_collisionPoint = p2Local;
                            Vector3 collisionNormal = body1.m_position - body2.m_position;
                            collisionNormal.Normalize();
                            Vector3 v1 = body1.m_velocity + Vector3.Cross(body1.m_angularVelocityLocal, p1 - body1.m_position);
                            Vector3 v2 = body2.m_velocity + Vector3.Cross(body2.m_angularVelocityLocal, p2 - body2.m_position);
                            Vector3 relativeVelocity = v1 - v2;
                            float vrn = Vector3.Dot(relativeVelocity, collisionNormal);
                            if (vrn < 0)
                            {
                                haveNodeNode = true;
                            }
                        }
                    }    
                }
                if (!haveNodeNode)
                {
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
                            Vector3 proj = Vector3.Dot(p, u) * u;
                            float dist = Vector3.Cross(p, u).magnitude;
                            if(proj.magnitude > 0 && proj.magnitude <= edge.magnitude && dist <= ctol)
                            {
                                Vector3 collisionNormal = Vector3.Cross(Vector3.Cross(u, p), u);
                                collisionNormal.Normalize();
                                Vector3 point = body1.m_vertexListGlobal[i];
                                Vector3 v1 = body1.m_velocity + Vector3.Cross(body1.m_angularVelocityLocal, point - body1.m_position);
                                Vector3 v2 = body2.m_velocity + Vector3.Cross(body2.m_angularVelocityLocal, point - body2.m_position);
                                Vector3 relativeVelocity = v1 - v2;
                                float vrn = Vector3.Dot(relativeVelocity, collisionNormal);
                                if(vrn < 0)
                                {
                                    haveNodeEdge = true;
                                }
                            }
                        }
                    }
                }
                if (!haveNodeEdge && !haveNodeNode)
                {
                    for (int i = 0; i < 5 && !interpenetrating; i++)
                    {
                        for(int j = 0; j < 5 && !interpenetrating; j++)
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

            }
            CollisionResult result = CollisionResult.AwayFrom;
            if (interpenetrating)
            {
                result = CollisionResult.Penetrating;
            }else if(haveNodeNode || haveNodeEdge)
            {
                result = CollisionResult.Collision;
            }else
            {
                result = CollisionResult.AwayFrom;
            }
            return result;
        }

        void ApplyImpulse(RigidBody2D body1, RigidBody2D body2)
        {
            float j = 100;
            body1.m_velocity += (j * m_collisionNormal) / body1.m_mass;
            body1.m_angularVelocityLocal += Vector3.Cross(body1.m_collisionPoint, j * m_collisionNormal) / body1.m_inertia;
            body2.m_velocity += (j * m_collisionNormal) / body2.m_mass;
            body2.m_angularVelocityLocal -= Vector3.Cross(body2.m_collisionPoint, j * m_collisionNormal) / body2.m_inertia;
        }

        public static bool ArePointsEqual(Vector3 p1, Vector3 p2)
        {
            if(Mathf.Abs(p1.x - p2.x) <= ctol &&
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
