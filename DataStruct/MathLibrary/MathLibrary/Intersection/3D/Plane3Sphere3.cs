

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Plane3 and Sphere3
		/// </summary>
		public struct Plane3Sphere3Intr
		{
			/// <summary>
			/// Equals to IntersectionType.Point if a sphere is touching a plane, IntersectionType.Other if a sphere intersects a plane, otherwise IntersectionType.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Contains intersection circle of a sphere and a plane in case of IntersectionType.Other
			/// </summary>
			public Circle3 Circle;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a plane intersects a plane. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestPlane3Sphere3(ref Plane3 plane, ref Sphere3 sphere)
			{
				double signedDistance = plane.SignedDistanceTo(ref sphere.Center);
				return System.Math.Abs(signedDistance) <= sphere.Radius + Mathex.ZeroTolerance;
			}

			/// <summary>
			/// Tests if a plane intersects a plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindPlane3Sphere3(ref Plane3 plane, ref Sphere3 sphere, out Plane3Sphere3Intr info)
			{
				double signedDistance = plane.SignedDistanceTo(ref sphere.Center);
				double distance = System.Math.Abs(signedDistance);

				if (distance <= sphere.Radius + Mathex.ZeroTolerance)
				{
					// The sphere intersects the plane in a circle.  The circle is
					// degenerate when distance is equal to sphere radius, in which
					// case the circle radius is zero.

					if (distance >= sphere.Radius - Mathex.ZeroTolerance)
					{
						info.IntersectionType = IntersectionTypes.Point;
						info.Circle = new Circle3();
					}
					else
					{
						Vector3D center = sphere.Center - signedDistance * plane.Normal;
						double radius = System.Math.Sqrt(System.Math.Abs(sphere.Radius * sphere.Radius - distance * distance));

						info.IntersectionType = IntersectionTypes.Other;
						info.Circle = new Circle3(ref center, ref plane.Normal, radius);
					}
					return true;
				}
				else
				{
					// Additional indication that the circle is invalid.
					info.IntersectionType = IntersectionTypes.Empty;
					info.Circle = new Circle3();
					return false;
				}
			}
		}
	}
}
