

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment2 and Circle2
		/// </summary>
		public struct Segment2Circle2Intr
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
			/// Tests whether segment and circle intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment2Circle2(ref Segment2 segment, ref Circle2 circle)
			{
				Vector2D delta = segment.Center - circle.Center;
				double a0 = delta.sqrMagnitude - circle.Radius * circle.Radius;

				if (a0 <= Mathex.ZeroTolerance)
				{
					// P is inside or on the sphere.
					return true;
				}
				// Else: P is outside the sphere.

				double a1 = Vector2D.Dot(segment.Direction, delta);
				double discr = a1 * a1 - a0;
				if (discr < -Mathex.ZeroTolerance)
				{
					// two complex-valued roots, no intersections
					return false;
				}

				double absA1 = System.Math.Abs(a1);
				double qval = segment.Extent * (segment.Extent - 2 * absA1) + a0;
				return qval <= Mathex.ZeroTolerance || absA1 <= segment.Extent;
			}

			/// <summary>
			/// Tests whether segment and circle intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment2Circle2(ref Segment2 segment, ref Circle2 circle, out Segment2Circle2Intr info)
			{
				double t0, t1;
				int quantity;
				bool intersects = Find(ref segment.Center, ref segment.Direction, ref circle.Center, circle.Radius, out quantity, out t0, out t1);

				info.Point0 = info.Point1 = Vector2D.zero;

				if (intersects)
				{
					// Reduce root count if line-circle intersections are not on segment.
					if (quantity == 1)
					{
						if (System.Math.Abs(t0) > (segment.Extent + Mathex.ZeroTolerance))
						{
							info.IntersectionType = IntersectionTypes.Empty;
						}
						else
						{
							info.IntersectionType = IntersectionTypes.Point;
							info.Point0 = segment.Center + t0 * segment.Direction;
						}
					}
					else
					{
						double tolerance = segment.Extent + Mathex.ZeroTolerance;
						if (t1 < -tolerance || t0 > tolerance)
						{
							info.IntersectionType = IntersectionTypes.Empty;
						}
						else
						{
							if (t1 <= tolerance)
							{
								if (t0 < -tolerance)
								{
									quantity = 1;
									t0 = t1;
								}
							}
							else
							{
								quantity = t0 >= -tolerance ? 1 : 0;
							}

							switch (quantity)
							{
								default:
								case 0:
									info.IntersectionType = IntersectionTypes.Empty;
									break;

								case 1:
									info.IntersectionType = IntersectionTypes.Point;
									info.Point0 = segment.Center + t0 * segment.Direction;
									break;

								case 2:
									info.IntersectionType = IntersectionTypes.Segment;
									info.Point0  = segment.Center + t0 * segment.Direction;
									info.Point1 = segment.Center + t1 * segment.Direction;
									break;
							}
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
