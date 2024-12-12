

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line3 and Circle3 (circle considered to be solid)
		/// </summary>
		public struct Line3Circle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a line lies in the plane of a circle)
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Intersection point
			/// </summary>
			public Vector3D Point;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a line intersects a solid circle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Circle3(ref Line3 line, ref Circle3 circle)
			{
				Line3Circle3Intr info;
				return FindLine3Circle3(ref line, ref circle, out info);
			}

			/// <summary>
			/// Tests if a line intersects a solid circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Circle3(ref Line3 line, ref Circle3 circle, out Line3Circle3Intr info)
			{
				double DdN = line.Direction.Dot(circle.Normal);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					// The line is not parallel to the plane, so they must intersect.
					double signedDistance = circle.Normal.Dot(line.Center - circle.Center);
					double lineParameter = -signedDistance / DdN;
					Vector3D point = line.Eval(lineParameter);

					Vector3D diff = point - circle.Center;
					if (diff.sqrMagnitude <= circle.Radius * circle.Radius)
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
