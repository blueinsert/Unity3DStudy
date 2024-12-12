

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line3 and Rectangle3 (rectangle considered to be solid)
		/// </summary>
		public struct Line3Rectangle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a line lies in the plane of a rectangle)
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Intersection point
			/// </summary>
			public Vector3D Point;
		}

		public static partial class Intersection
		{
			private static bool Point3InsideRectangle3(ref Vector3D point, ref Rectangle3 rectangle)
			{
				Vector3D diff = point - rectangle.Center;
				double s0 = diff.Dot(rectangle.Axis0);
				double s1 = diff.Dot(rectangle.Axis1);
				double extent;

				extent = rectangle.Extents.x;
				if (s0 < -extent)
				{
					return false;
				}
				else if (s0 > extent)
				{
					return false;
				}

				extent = rectangle.Extents.y;
				if (s1 < -extent)
				{
					return false;
				}
				else if (s1 > extent)
				{
					return false;
				}

				return true;
			}

			/// <summary>
			/// Tests if a line intersects a solid rectangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Rectangle3(ref Line3 line, ref Rectangle3 rectangle)
			{
				Line3Rectangle3Intr info;
				return FindLine3Rectangle3(ref line, ref rectangle, out info);
			}

			/// <summary>
			/// Tests if a line intersects a solid rectangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Rectangle3(ref Line3 line, ref Rectangle3 rectangle, out Line3Rectangle3Intr info)
			{
				double DdN = line.Direction.Dot(rectangle.Normal);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					// The line is not parallel to the plane, so they must intersect.
					double signedDistance = rectangle.Normal.Dot(line.Center - rectangle.Center);
					double lineParameter = -signedDistance / DdN;
					Vector3D point = line.Eval(lineParameter);

					bool inside = Point3InsideRectangle3(ref point, ref rectangle);
					if (inside)
					{
						info.IntersectionType = IntersectionTypes.Point;
						info.Point = point;
						return true;
					}

					// Point is outside of the rectangle, no intersection
				}

				// The line and plane are parallel.
				info.IntersectionType = IntersectionTypes.Empty;
				info.Point = Vector3D.zero;
				return false;
			}
		}
	}
}
