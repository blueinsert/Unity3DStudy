

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray3 and Rectangle3 (rectangle considered to be solid)
		/// </summary>
		public struct Ray3Rectangle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a ray lies in the plane of a rectangle)
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
			/// Tests if a ray intersects a solid rectangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay3Rectangle3(ref Ray3 ray, ref Rectangle3 rectangle)
			{
				Ray3Rectangle3Intr info;
				return FindRay3Rectangle3(ref ray, ref rectangle, out info);
			}

			/// <summary>
			/// Tests if a ray intersects a solid rectangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay3Rectangle3(ref Ray3 ray, ref Rectangle3 rectangle, out Ray3Rectangle3Intr info)
			{
				double DdN = ray.Direction.Dot(rectangle.Normal);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					double signedDistance = rectangle.Normal.Dot(ray.Center - rectangle.Center);
					double parameter = -signedDistance / DdN;
					if (parameter >= -_intervalThreshold)
					{
						// The line is not parallel to the plane, so they must intersect.
						Vector3D point = ray.Center + parameter * ray.Direction;

						bool inside = Point3InsideRectangle3(ref point, ref rectangle);
						if (inside)
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point = point;
							return true;
						}

						// Point is outside of the rectangle, no intersection
					}

					// Ray does not intersect the plane
				}

				info.IntersectionType = IntersectionTypes.Empty;
				info.Point = Vector3D.zero;
				return false;
			}
		}
	}
}
