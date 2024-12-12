

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray2 and Circle2
		/// </summary>
		public struct Ray2Circle2Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point or IntersectionTypes.Segment
			/// if intersection occured otherwise IntersectionTypes.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// First point of intersection (in case of IntersectionTypes.Point or IntersectionTypes.Segment)
			/// </summary>
			public Vector2D Point0;

			/// <summary>
			/// Second point of intersection (in case of IntersectionTypes.Segment)
			/// </summary>
			public Vector2D Point1;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests whether ray and circle intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay2Circle2(ref Ray2 ray, ref Circle2 circle)
			{
				Vector2D delta = ray.Center - circle.Center;
				double a0 = delta.sqrMagnitude - circle.Radius * circle.Radius;
				
				if (a0 <= Mathex.ZeroTolerance)
				{
					// ray.P is inside or on the sphere.
					return true;
				}
				// Else: ray.P is outside the sphere.

				double a1 = Vector2D.Dot(ray.Direction, delta);
				if (a1 >= 0f)
				{
					// Ray is directed away from the sphere.
					return false;
				}

				double discr = a1 * a1 - a0;
				return discr >= -Mathex.ZeroTolerance;
			}

			/// <summary>
			/// Tests whether ray and circle intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay2Circle2(ref Ray2 ray, ref Circle2 circle, out Ray2Circle2Intr info)
			{
				double t0, t1;
				int quantity;
				bool intersects = Find(ref ray.Center, ref ray.Direction, ref circle.Center, circle.Radius, out quantity, out t0, out t1);

				info.Point0 = info.Point1 = Vector2D.zero;

				if (intersects)
				{
					// Reduce root count if line-circle intersections are not on ray.
					if (quantity == 1)
					{
						if (t0 < 0f)
						{
							info.IntersectionType = IntersectionTypes.Empty;
						}
						else
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point0 = ray.Center + t0 * ray.Direction;
						}
					}
					else
					{
						if (t1 < 0f)
						{
							info.IntersectionType = IntersectionTypes.Empty;
						}
						else if (t0 < 0f)
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point0 = ray.Center + t1 * ray.Direction;
						}
						else
						{
							info.IntersectionType = IntersectionTypes.Segment;
							info.Point0  = ray.Center + t0 * ray.Direction;
							info.Point1 = ray.Center + t1 * ray.Direction;
						}
					}
				}
				else
				{
					info.IntersectionType = IntersectionTypes.Empty;
				}

				return info.IntersectionType != IntersectionTypes.Empty;
			}
		}
	}
}
