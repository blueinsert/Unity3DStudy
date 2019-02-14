using System.Collections;
using System.Collections.Generic;
using Math = UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;
using bluebean;

namespace bluebean
{
    public enum CollisionResultType
    {
        Collision,
        Penetrating,
        Separate,
    }

    public class PhysicsUtils2D
    {
        const float COLLISIONTOLERANCE = 0.01f;
        const float ctol = COLLISIONTOLERANCE;

        private static Vector3 GetPlaneNormalVector(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 v1 = p2 - p1;
            Vector3 v2 = p3 - p1;
            Vector3 normal = Vector3.Cross(v1, v2);
            normal.Normalize();
            return normal;
        }

        private static Vector3 GetLineNormalVector(Vector3 p1, Vector3 p2)
        {
            return GetPlaneNormalVector(p1, p2, p1 + new Vector3(0,0,1));
        }


        //轴分离相交检测算法，长方体
        public static bool AxisSeparateIntersectionTest3D(Vector3[] C1Points, Vector3[] C2Points)
        {
            Vector3[] normalVectors = new Vector3[6];
            normalVectors[0] = GetPlaneNormalVector(C1Points[0], C1Points[3], C1Points[2]);
            normalVectors[1] = GetPlaneNormalVector(C1Points[0], C1Points[4], C1Points[7]);
            normalVectors[2] = GetPlaneNormalVector(C1Points[3], C1Points[2], C1Points[6]);

            normalVectors[3] = GetPlaneNormalVector(C2Points[0], C2Points[3], C2Points[2]);
            normalVectors[4] = GetPlaneNormalVector(C2Points[0], C2Points[4], C2Points[7]);
            normalVectors[5] = GetPlaneNormalVector(C2Points[3], C2Points[2], C2Points[6]);

            bool isIntersect = true;
            for (int i = 0; i < 6; i++)
            {
                Vector3 normal = normalVectors[i];
                //
                float c1Max = float.MinValue;
                float c1Min = float.MaxValue;
                foreach (var v in C1Points)
                {
                    float projectValue = Vector3.Dot(v, normal);
                    if (projectValue > c1Max)
                    {
                        c1Max = projectValue;
                    }
                    if (projectValue < c1Min)
                    {
                        c1Min = projectValue;
                    }
                }
                //
                float c2Max = float.MinValue;
                float c2Min = float.MaxValue;
                foreach (var v in C2Points)
                {
                    float projectValue = Vector3.Dot(v, normal);
                    if (projectValue > c2Max)
                    {
                        c2Max = projectValue;
                    }
                    if (projectValue < c2Min)
                    {
                        c2Min = projectValue;
                    }
                }
                //
                if (c2Min > c1Max || c1Min > c2Max)
                {
                    isIntersect = false;
                    //Debug.Log("c1Min:" + c1Min + " c1Max" + c1Max + " c2Min:" + c2Min + " c2Max" + c2Max);
                    break;
                }

            }//for six separate axis
            return isIntersect;
        }

        //轴分离相交检测算法，长方形
        public static bool AxisSeparateIntersectionTest2D(Vector3[] C1Points, Vector3[] C2Points)
        {
            Vector3[] normalVectors = new Vector3[4];
            normalVectors[0] = GetLineNormalVector(C1Points[0], C1Points[1]);
            normalVectors[1] = GetLineNormalVector(C1Points[1], C1Points[2]);

            normalVectors[2] = GetLineNormalVector(C2Points[0], C2Points[1]);
            normalVectors[3] = GetLineNormalVector(C2Points[1], C2Points[2]);

            bool isIntersect = true;
            for (int i = 0; i < 4; i++)
            {
                Vector3 normal = normalVectors[i];
                //
                float c1Max = float.MinValue;
                float c1Min = float.MaxValue;
                foreach (var v in C1Points)
                {
                    float projectValue = Vector3.Dot(v, normal);
                    if (projectValue > c1Max)
                    {
                        c1Max = projectValue;
                    }
                    if (projectValue < c1Min)
                    {
                        c1Min = projectValue;
                    }
                }
                //
                float c2Max = float.MinValue;
                float c2Min = float.MaxValue;
                foreach (var v in C2Points)
                {
                    float projectValue = Vector3.Dot(v, normal);
                    if (projectValue > c2Max)
                    {
                        c2Max = projectValue;
                    }
                    if (projectValue < c2Min)
                    {
                        c2Min = projectValue;
                    }
                }
                //
                if (c2Min > c1Max || c1Min > c2Max)
                {
                    isIntersect = false;
                    //Debug.Log("c1Min:" + c1Min + " c1Max" + c1Max + " c2Min:" + c2Min + " c2Max" + c2Max);
                    break;
                }

            }//for six separate axis
            return isIntersect;
        }

        public static bool ArePointsEqual(Vector3 p1, Vector3 p2)
        {
            if (Math.Abs(p1.x - p2.x) <= ctol &&
                Math.Abs(p1.y - p2.y) <= ctol &&
                Math.Abs(p1.z - p2.z) <= ctol)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static CollisionResultType CollideTest(Collider2D collider1, Collider2D collider2, out List<CollisionPointInfo> collisionPointInfos)
        {
            collisionPointInfos = null;
            if (collider1 is Box2DCollider && collider2 is Box2DCollider)
            {
                return CollideTest(collider1 as Box2DCollider, collider2 as Box2DCollider, out collisionPointInfos);
            }
            return CollisionResultType.Separate;
        }

        public static CollisionResultType CollideTest(Box2DCollider collider1, Box2DCollider collider2, out List<CollisionPointInfo> collisionPointInfos)
        {
            collisionPointInfos = new List<CollisionPointInfo>();
            //检测外接包围圆是否分离
            {
                var p1 = collider1.position;
                var p2 = collider2.position;
                float dist = (p1 - p2).magnitude;
                var r1 = Math.Sqrt(collider1.width * collider1.width + collider1.length * collider1.length) / 2;
                var r2 = Math.Sqrt(collider2.width * collider2.width + collider2.length * collider2.length) / 2;
                if (dist > r1 + r2)
                    return CollisionResultType.Separate;
            }
            //如果外接圆相交，则进行进一步的检测
            //检测点与点之间是否相交
            foreach (var point1 in collider1.points)
            {
                foreach (var point2 in collider2.points)
                {
                    if (ArePointsEqual(point1, point2))
                    {
                        Vector3 collisionNormal = (collider1.position - collider2.position).normalized;
                        Vector3 v1 = collider1.velocity + Vector3.Cross(collider1.angularVelocityLocal, point1 - collider1.position);
                        Vector3 v2 = collider2.velocity + Vector3.Cross(collider2.angularVelocityLocal, point2 - collider2.position);
                        Vector3 relativeVelocity = v1 - v2;
                        float vrn = Vector3.Dot(relativeVelocity, collisionNormal);
                        //两点相互靠近
                        if (vrn < 0)
                        {
                            CollisionPointInfo collisionPointInfo = new CollisionPointInfo();
                            collisionPointInfo.m_collisionNormal = collisionNormal;
                            collisionPointInfo.m_point = (point1 + point2) / 2;
                            collisionPointInfo.m_relativeVelocity = relativeVelocity;
                            collisionPointInfos.Add(collisionPointInfo);
                        }
                    }
                }
            }
            //点与边之间检测相交
            foreach (var point1 in collider1.points)
            {
                for (int i = 0; i < collider2.points.Length; i++)
                {
                    Vector3 edge;
                    if (i == 0)
                    {
                        edge = collider1.points[collider2.points.Length - 1] - collider2.points[0];
                    }
                    else
                    {
                        edge = collider2.points[i] - collider2.points[i - 1];
                    }
                    Vector3 u = edge.normalized;
                    Vector3 p = point1 - collider2.points[i];
                    float proj = Vector3.Dot(p, u);
                    float dist = Vector3.Cross(p, u).magnitude;
                    //点距离边很近
                    if (proj > 0 && proj <= edge.magnitude && dist <= ctol)
                    {
                        Vector3 collisionNormal = Vector3.Cross(Vector3.Cross(u, p), u).normalized;
                        Vector3 v1 = collider1.velocity + Vector3.Cross(collider1.angularVelocityLocal, point1 - collider1.position);
                        Vector3 v2 = collider2.velocity + Vector3.Cross(collider2.angularVelocityLocal, point1 - collider2.position);
                        Vector3 relativeVelocity = v1 - v2;
                        float vrn = Vector3.Dot(relativeVelocity, collisionNormal);
                        if (vrn < 0)
                        {
                            CollisionPointInfo collisionPointInfo = new CollisionPointInfo();
                            collisionPointInfo.m_collisionNormal = collisionNormal;
                            collisionPointInfo.m_point = (point1 + point1) / 2;
                            collisionPointInfo.m_relativeVelocity = relativeVelocity;
                            collisionPointInfos.Add(collisionPointInfo);
                        }
                    }
                }
            }
            foreach (var point2 in collider2.points)
            {
                for (int i = 0; i < collider1.points.Length; i++)
                {
                    Vector3 edge;
                    if (i == 0)
                    {
                        edge = collider1.points[collider2.points.Length - 1] - collider2.points[0];
                    }
                    else
                    {
                        edge = collider2.points[i] - collider2.points[i - 1];
                    }
                    Vector3 u = edge.normalized;
                    Vector3 p = point2 - collider1.points[i];
                    float proj = Vector3.Dot(p, u);
                    float dist = Vector3.Cross(p, u).magnitude;
                    //点距离边很近
                    if (proj > 0 && proj <= edge.magnitude && dist <= ctol)
                    {
                        Vector3 collisionNormal = -Vector3.Cross(Vector3.Cross(u, p), u).normalized;
                        Vector3 v1 = collider1.velocity + Vector3.Cross(collider1.angularVelocityLocal, point2 - collider1.position);
                        Vector3 v2 = collider2.velocity + Vector3.Cross(collider2.angularVelocityLocal, point2 - collider2.position);
                        Vector3 relativeVelocity = v1 - v2;
                        float vrn = Vector3.Dot(relativeVelocity, collisionNormal);
                        if (vrn < 0)
                        {
                            CollisionPointInfo collisionPointInfo = new CollisionPointInfo();
                            collisionPointInfo.m_collisionNormal = collisionNormal;
                            collisionPointInfo.m_point = (point2 + point2) / 2;
                            collisionPointInfo.m_relativeVelocity = relativeVelocity;
                            collisionPointInfos.Add(collisionPointInfo);
                        }
                    }
                }
            }
            //去除重复的碰撞点
            if (collisionPointInfos.Count > 1)
            {
                List<CollisionPointInfo> removedCollisionPointInfo = new List<CollisionPointInfo>();
                for (int i = 0; i < collisionPointInfos.Count; i++)
                {
                    for (int j = i + 1; j < collisionPointInfos.Count; i++)
                    {
                        if (ArePointsEqual(collisionPointInfos[i].m_point, collisionPointInfos[j].m_point))
                        {
                            removedCollisionPointInfo.Add(collisionPointInfos[i]);
                        }
                    }
                }
                foreach (var removeItem in removedCollisionPointInfo)
                {
                    collisionPointInfos.Remove(removeItem);
                }
            }
            if (collisionPointInfos.Count > 0)
            {
                return CollisionResultType.Collision;
            }
            if (AxisSeparateIntersectionTest2D(collider1.points, collider2.points))
            {
                return CollisionResultType.Penetrating;
             }
            return CollisionResultType.Separate;
        }

        public static bool SegmentIntersectionTest(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, ref Vector3 point)
        {
            var s10_x = p1.x - p0.x;
            var s10_y = p1.y - p0.y;
            var s32_x = p3.x - p2.x;
            var s32_y = p3.y - p2.y;

            var denom = s10_x * s32_y - s32_x * s10_y;

            if (denom == 0) return false; // no collision

            var denom_is_positive = denom > 0;

            var s02_x = p0.x - p2.x;
            var s02_y = p0.y - p2.y;

            var s_numer = s10_x * s02_y - s10_y * s02_x;

            if ((s_numer < 0) == denom_is_positive) return false; // no collision

            var t_numer = s32_x * s02_y - s32_y * s02_x;

            if ((t_numer < 0) == denom_is_positive) return false; // no collision

            if ((s_numer > denom) == denom_is_positive || (t_numer > denom) == denom_is_positive) return false; // no collision

            // collision detected

            var t = t_numer / denom;

            point.x = (int)(p0.x + (t * s10_x));
            point.y = (int)(p0.y + (t * s10_y));

            return true;
        }


    }
}
