

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
            /// Tests if a capsule intersects another capsule. Returns true if intersection occurs false otherwise.
			/// </summary>
            public static bool TestCapsule3Capsule3(ref Capsule3 capsule0, ref Capsule3 capsule1)
			{
                double rSum = capsule0.Radius + capsule1.Radius;
                return Distance.SqrSegment3Segment3(ref capsule0.Segment, ref capsule1.Segment) <= rSum * rSum;
			}

            /// <summary>
            /// Tests if a Capsule intersects another capsule. Returns true if intersection occurs false otherwise.
            /// Output contact normal and point if intersection occurs 
            /// </summary>
            public static bool TestCapsule3Capsule3(ref Capsule3 capsule0, ref Capsule3 capsule1, out ContactInfo contact)
            {
                double rSum = capsule0.Radius + capsule1.Radius;
                Vector3D closestPoint0, closestPoint1;
                double disSqr = Distance.SqrSegment3Segment3(ref capsule0.Segment, ref capsule1.Segment, out closestPoint0, out closestPoint1);
                if (disSqr <= rSum * rSum)
                {
                    if (disSqr > Mathex.ZeroTolerance)
                    {
                        double dis = System.Math.Sqrt(disSqr);
                        contact.Normal = (closestPoint0 - closestPoint1) / dis;
                        contact.Point = closestPoint1 + contact.Normal * capsule1.Radius;
                        contact.Depth = capsule0.Radius + capsule1.Radius - dis;                        
                    }
                    else
                    {
                        Vector3D closestPoint;
                        double disSqr2 = Distance.SqrPoint3Segment3(ref capsule0.Segment.Center, ref capsule1.Segment, out closestPoint);
                        if (disSqr2 > Mathex.ZeroTolerance)
                        {
                            double dis2 = System.Math.Sqrt(disSqr2);
                            contact.Normal = (capsule0.Segment.Center - closestPoint) / dis2;
                            contact.Point = closestPoint1 + contact.Normal * capsule1.Radius;
                        }
                        else
                        {
                            contact.Normal = new Vector3D(1, 0, 0);
                            contact.Point = closestPoint1;
                        }
                        contact.Depth = capsule0.Radius + capsule1.Radius;
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
