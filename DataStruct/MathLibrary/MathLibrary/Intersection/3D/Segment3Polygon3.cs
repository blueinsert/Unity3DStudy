

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment3 and Polygon3 (polygon considered to be solid)
		/// </summary>
		public struct Segment3Polygon3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a segment lies in the plane of a polygon)
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
			/// Tests if a segment intersects a solid polygon. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment3Polygon3(ref Segment3 segment, Polygon3 polygon)
			{
				Segment3Polygon3Intr info;
				return FindSegment3Polygon3(ref segment, polygon, out info);
			}

			/// <summary>
			/// Tests if a segment intersects a solid polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment3Polygon3(ref Segment3 segment, Polygon3 polygon, out Segment3Polygon3Intr info)
			{
				Plane3 plane = polygon.Plane;
				double DdN = segment.Direction.Dot(plane.Normal);
				double signedDistance = plane.SignedDistanceTo(ref segment.Center);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					double parameter = -signedDistance / DdN;
					if (System.Math.Abs(parameter) <= segment.Extent + _intervalThreshold)
					{
						Vector3D point = segment.Center + parameter * segment.Direction;
						ProjectionPlanes projectionPlane = plane.Normal.GetProjectionPlane();
						Polygon2 projectedPolygon = Polygon2.CreateProjected(polygon, projectionPlane);
						Vector2D projectedPoint = point.ToVector2(projectionPlane);

						bool inside = projectedPolygon.ContainsSimple(projectedPoint);
						if (inside)
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point = point;
							return true;
						}

						// Point is outside of the rectangle, no intersection
					}
					else
					{
						info.IntersectionType = IntersectionTypes.Empty;
						info.Point = Vector3D.zero;
						return false;
					}
				}

				// The Line and plane are parallel.
				info.IntersectionType = IntersectionTypes.Empty;
				info.Point = Vector3D.zero;
				return false;
			}
		}
	}
}
