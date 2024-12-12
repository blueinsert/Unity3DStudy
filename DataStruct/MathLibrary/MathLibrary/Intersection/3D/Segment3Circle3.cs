﻿

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment3 and Circle3 (circle considered to be solid)
		/// </summary>
		public struct Segment3Circle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a segment lies in the plane of a circle)
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
			/// Tests if a segment intersects a solid circle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment3Circle3(ref Segment3 segment, ref Circle3 circle)
			{
				Segment3Circle3Intr info;
				return FindSegment3Circle3(ref segment, ref circle, out info);
			}

			/// <summary>
			/// Tests if a segment intersects a solid circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment3Circle3(ref Segment3 segment, ref Circle3 circle, out Segment3Circle3Intr info)
			{
				double DdN = segment.Direction.Dot(circle.Normal);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					double signedDistance = circle.Normal.Dot(segment.Center - circle.Center);
					double parameter = -signedDistance / DdN;
					if (System.Math.Abs(parameter) <= segment.Extent + _intervalThreshold)
					{
						Vector3D point = segment.Center + parameter * segment.Direction;

						Vector3D diff = point - circle.Center;
						if (diff.sqrMagnitude <= circle.Radius * circle.Radius)
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point = point;
							return true;
						}

						// Point is outside of the circle, no intersection
					}

					// Segment does not intersect the plane
				}

				info.IntersectionType = IntersectionTypes.Empty;
				info.Point = Vector3D.zero;
				return false;
			}
		}
	}
}
