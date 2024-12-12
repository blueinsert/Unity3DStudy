

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line3 and Plane3
		/// </summary>
		public struct Line3Plane3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point or IntersectionTypes.Line (a line lies in a plane) if intersection occured otherwise IntersectionTypes.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Intersection point (in case of IntersectionTypes.Point)
			/// </summary>
			public Vector3D Point;

			/// <summary>
			/// Line evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
			/// </summary>
			public double LineParameter;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a line intersects a plane. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Plane3(ref Line3 line, ref Plane3 plane, out IntersectionTypes intersectionType)
			{
				double DdN = line.Direction.Dot(plane.Normal);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					// The line is not parallel to the plane, so they must intersect.
					// The line parameter is *not* set, since this is a test-intersection
					// query.
					intersectionType = IntersectionTypes.Point;
					return true;
				}

				// The line and plane are parallel.  Determine if they are numerically
				// close enough to be coincident.
				double signedDistance = plane.SignedDistanceTo(ref line.Center);
				if (System.Math.Abs(signedDistance) <= _distanceThreshold)
				{
					intersectionType = IntersectionTypes.Line;
					return true;
				}

				intersectionType = IntersectionTypes.Empty;
				return false;
			}

			/// <summary>
			/// Tests if a line intersects a plane. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Plane3(ref Line3 line, ref Plane3 plane)
			{
				IntersectionTypes intersectionType;
				return TestLine3Plane3(ref line, ref plane, out intersectionType);
			}

			/// <summary>
			/// Tests if a line intersects a plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Plane3(ref Line3 line, ref Plane3 plane, out Line3Plane3Intr info)
			{
				double DdN = line.Direction.Dot(plane.Normal);
				double signedDistance = plane.SignedDistanceTo(ref line.Center);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					// The line is not parallel to the plane, so they must intersect.
					info.LineParameter = -signedDistance / DdN;
					info.IntersectionType = IntersectionTypes.Point;
					info.Point = line.Eval(info.LineParameter);
					return true;
				}

				// The Line and plane are parallel.  Determine if they are numerically
				// close enough to be coincident.
				if (System.Math.Abs(signedDistance) <= _distanceThreshold)
				{
					// The line is coincident with the plane, so choose t = 0 for the parameter.
					info.LineParameter = 0f;
					info.IntersectionType = IntersectionTypes.Line;
					info.Point = Vector3D.zero;
					return true;
				}

				info.IntersectionType = IntersectionTypes.Empty;
				info.LineParameter = 0f;
				info.Point = Vector3D.zero;

				return false;
			}
		}
	}
}
