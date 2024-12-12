

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray3 and Circle3 (circle considered to be solid)
		/// </summary>
		public struct Ray3Circle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty
			/// (including the case when a ray lies in the plane of a circle)
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
			/// Tests if a ray intersects a solid circle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay3Circle3(ref Ray3 ray, ref Circle3 circle)
			{
				Ray3Circle3Intr info;
				return FindRay3Circle3(ref ray, ref circle, out info);
			}

			/// <summary>
			/// Tests if a ray intersects a solid circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay3Circle3(ref Ray3 ray, ref Circle3 circle, out Ray3Circle3Intr info)
			{
				double DdN = ray.Direction.Dot(circle.Normal);
				if (System.Math.Abs(DdN) > _dotThreshold)
				{
					double signedDistance = circle.Normal.Dot(ray.Center - circle.Center);
					double parameter = -signedDistance / DdN;
					if (parameter >= -_intervalThreshold)
					{
						// The line is not parallel to the plane, so they must intersect.
						Vector3D point = ray.Center + parameter * ray.Direction;

						Vector3D diff = point - circle.Center;
						if (diff.sqrMagnitude <= circle.Radius * circle.Radius)
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point = point;
							return true;
						}

						// Point is outside of the circle, no intersection
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
