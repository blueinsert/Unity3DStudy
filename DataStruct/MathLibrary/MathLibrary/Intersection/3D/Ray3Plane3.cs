

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray3 and Plane3
		/// </summary>
		public struct Ray3Plane3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point or IntersectionTypes.Ray (a ray lies in a plane) if intersection occured otherwise IntersectionTypes.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Intersection point (in case of IntersectionTypes.Point)
			/// </summary>
			public Vector3D Point;

			/// <summary>
			/// Ray evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
			/// </summary>
			public double RayParameter;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a ray intersects a plane. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay3Plane3(ref Ray3 ray, ref Plane3 plane, out IntersectionTypes intersectionType)
			{
				Ray3Plane3Intr info;
				bool find = FindRay3Plane3(ref ray, ref plane, out info);
				intersectionType = info.IntersectionType;
				return find;
			}

			/// <summary>
			/// Tests if a ray intersects a plane. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay3Plane3(ref Ray3 ray, ref Plane3 plane)
			{
				Ray3Plane3Intr info;
				return FindRay3Plane3(ref ray, ref plane, out info);
			}

			/// <summary>
			/// Tests if a ray intersects a plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay3Plane3(ref Ray3 ray, ref Plane3 plane, out Ray3Plane3Intr info)
			{
				double DdN = ray.Direction.Dot(plane.Normal);
				double signedDistance = plane.SignedDistanceTo(ref ray.Center);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					double parameter = -signedDistance / DdN;
					if (parameter >= -_intervalThreshold)
					{
						// The line is not parallel to the plane, so they must intersect.
						info.RayParameter = parameter;
						info.IntersectionType = IntersectionTypes.Point;
						info.Point = ray.Center + parameter * ray.Direction;
						return true;
					}
					else
					{
						info.IntersectionType = IntersectionTypes.Empty;
						info.RayParameter = 0f;
						info.Point = Vector3D.zero;
						return false;
					}
				}

				// The Line and plane are parallel.  Determine if they are numerically
				// close enough to be coincident.
				if (System.Math.Abs(signedDistance) <= _distanceThreshold)
				{
					// The line is coincident with the plane, so choose t = 0 for the parameter.
					info.RayParameter = 0f;
					info.IntersectionType = IntersectionTypes.Ray;
					info.Point = Vector3D.zero;
					return true;
				}

				info.IntersectionType = IntersectionTypes.Empty;
				info.RayParameter = 0f;
				info.Point = Vector3D.zero;
				return false;
			}
		}
	}
}
