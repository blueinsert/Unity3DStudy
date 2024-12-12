

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment3 and Rectangle3 (rectangle considered to be solid)
		/// </summary>
		public struct Segment3Rectangle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a segment lies in the plane of a rectangle)
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
			/// Tests if a segment intersects a solid rectangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment3Rectangle3(ref Segment3 segment, ref Rectangle3 rectangle)
			{
				Segment3Rectangle3Intr info;
				return FindSegment3Rectangle3(ref segment, ref rectangle, out info);
			}

			/// <summary>
			/// Tests if a segment intersects a solid rectangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment3Rectangle3(ref Segment3 segment, ref Rectangle3 rectangle, out Segment3Rectangle3Intr info)
			{
				double DdN = segment.Direction.Dot(rectangle.Normal);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					double signedDistance = rectangle.Normal.Dot(segment.Center - rectangle.Center);
					double parameter = -signedDistance / DdN;
					if (System.Math.Abs(parameter) <= segment.Extent + _intervalThreshold)
					{
						Vector3D point = segment.Center + parameter * segment.Direction;

						bool inside = Point3InsideRectangle3(ref point, ref rectangle);
						if (inside)
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point = point;
							return true;
						}

						// Point is outside of the rectangle, no intersection
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
