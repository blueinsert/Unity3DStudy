

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line3 and Polygon3 (polygon considered to be solid)
		/// </summary>
		public struct Line3Polygon3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a line lies in the plane of a polygon)
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
			/// Tests if a line intersects a solid polygon. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Polygon3(ref Line3 line, Polygon3 polygon)
			{
				Line3Polygon3Intr info;
				return FindLine3Polygon3(ref line, polygon, out info);
			}

			/// <summary>
			/// Tests if a line intersects a solid polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Polygon3(ref Line3 line, Polygon3 polygon, out Line3Polygon3Intr info)
			{
				Plane3 plane = polygon.Plane;
				double DdN = line.Direction.Dot(plane.Normal);
				double signedDistance = plane.SignedDistanceTo(ref line.Center);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					// The line is not parallel to the plane, so they must intersect.
					double lineParameter = -signedDistance / DdN;
					Vector3D point = line.Eval(lineParameter);
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

				// The line and plane are parallel.
				info.IntersectionType = IntersectionTypes.Empty;
				info.Point = Vector3D.zero;
				return false;
			}
		}
	}
}
