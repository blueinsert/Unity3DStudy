

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray3 and Polygon3 (polygon considered to be solid)
		/// </summary>
		public struct Ray3Polygon3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a ray lies in the plane of a polygon)
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
			public static bool TestRay3Polygon3(ref Ray3 ray, Polygon3 polygon)
			{
				Ray3Polygon3Intr info;
				return FindRay3Polygon3(ref ray, polygon, out info);
			}

			/// <summary>
			/// Tests if a ray intersects a solid polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay3Polygon3(ref Ray3 ray, Polygon3 polygon, out Ray3Polygon3Intr info)
			{
				Plane3 plane = polygon.Plane;
				double DdN = ray.Direction.Dot(plane.Normal);
				double signedDistance = plane.SignedDistanceTo(ref ray.Center);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					double parameter = -signedDistance / DdN;
					if (parameter >= -_intervalThreshold)
					{
						// The line is not parallel to the plane, so they must intersect.
						Vector3D point = ray.Center + parameter * ray.Direction;
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
