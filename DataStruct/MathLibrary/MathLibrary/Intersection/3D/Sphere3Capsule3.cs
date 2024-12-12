

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
            /// Tests if a sphere intersects a capsule. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSphere3Capsule3(ref Sphere3 sphere, ref Capsule3 capsule)
			{
                double rSum = sphere.Radius + capsule.Radius;
                return Distance.SqrPoint3Segment3(ref sphere.Center, ref capsule.Segment) <= rSum * rSum;
			}

            /// <summary>
            /// Tests if a sphere intersects a capsule. Returns true if intersection occurs false otherwise.
            /// Output contact normal and point if intersection occurs 
            /// </summary>
            public static bool TestSphere3Capsule3(ref Sphere3 sphere, ref Capsule3 capsule, out ContactInfo contact)
            {
                double rSum = sphere.Radius + capsule.Radius;
                Vector3D closestPoint;
                double disSqr = Distance.SqrPoint3Segment3(ref sphere.Center, ref capsule.Segment, out closestPoint);
                if (disSqr <= rSum * rSum)
                {
                    if (disSqr > Mathex.ZeroTolerance)
                    {
                        double dis = System.Math.Sqrt(disSqr);
                        contact.Normal = (sphere.Center - closestPoint) / dis;
                        contact.Point = closestPoint + contact.Normal * capsule.Radius;
                        contact.Depth = sphere.Radius + capsule.Radius - dis;
                    }
                    else
                    {
                        contact.Normal = new Vector3D(1, 0, 0);
                        contact.Point = sphere.Center;
                        contact.Depth = sphere.Radius + capsule.Radius;
                    }
                    return true;
                }
                else
                {
                    contact.Point = contact.Normal = Vector3D.zero;
                    contact.Depth = 0;
                    return false;
                }
            }

            /// <summary>
            /// Tests if a capsule intersects a sphere. Returns true if intersection occurs false otherwise.
            /// Output contact normal and point if intersection occurs 
            /// </summary>
            public static bool TestCapsule3Sphere3(ref Capsule3 capsule, ref Sphere3 sphere, out ContactInfo contact)
            {
                double rSum = sphere.Radius + capsule.Radius;
                Vector3D closestPoint;
                double disSqr = Distance.SqrPoint3Segment3(ref sphere.Center, ref capsule.Segment, out closestPoint);
                if (disSqr <= rSum * rSum)
                {
                    if (disSqr > Mathex.ZeroTolerance)
                    {
                        double dis = System.Math.Sqrt(disSqr);
                        contact.Normal = (closestPoint - sphere.Center) / dis;
                        contact.Point = sphere.Center + contact.Normal * sphere.Radius;
                        contact.Depth = sphere.Radius + capsule.Radius - dis;
                    }
                    else
                    {
                        contact.Normal = new Vector3D(1, 0, 0);
                        contact.Point = sphere.Center;
                        contact.Depth = sphere.Radius + capsule.Radius;
                    }
                    return true;
                }
                else
                {
                    contact.Point = contact.Normal = Vector3D.zero;
                    contact.Depth = 0;
                    return false;
                }
            }
		}
	}
}
