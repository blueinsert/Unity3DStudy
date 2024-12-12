

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of two Circle2.
		/// The quantity Q is 0, 1, or 2. When Q > 0, the interpretation depends
		/// on the intersection type.
		///   IntersectionTypes.Point:  Q distinct points of intersection
		///   IntersectionTypes.Other:  The circles are the same
		/// </summary>
		public struct Circle2Circle2Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if there is intersection,
			/// IntersectionTypes.Other if circles are the same and IntersectionTypes.Empty
			/// if circles do not intersect
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Number of intersection points
			/// </summary>
			public int Quantity;
			
			/// <summary>
			/// First intersection point
			/// </summary>
			public Vector2D Point0;
			
			/// <summary>
			/// Second intersection point
			/// </summary>
			public Vector2D Point1;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a circle intersects another circle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestCircle2Circle2(ref Circle2 circle0, ref Circle2 circle1)
			{
				Vector2D diff = circle0.Center - circle1.Center;
				double rSum = circle0.Radius + circle1.Radius;
				return diff.sqrMagnitude <= rSum * rSum;
			}

			/// <summary>
			/// Tests if a circle intersects another circle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindCircle2Circle2(ref Circle2 circle0, ref Circle2 circle1, out Circle2Circle2Intr info)
			{
				// The two circles are |X-C0| = R0 and |X-C1| = R1.  Define U = C1 - C0
				// and V = Perp(U) where Perp(x,y) = (y,-x).  Note that Dot(U,V) = 0 and
				// |V|^2 = |U|^2.  The intersection points X can be written in the form
				// X = C0+s*U+t*V and X = C1+(s-1)*U+t*V.  Squaring the circle equations
				// and substituting these formulas into them yields
				//   R0^2 = (s^2 + t^2)*|U|^2
				//   R1^2 = ((s-1)^2 + t^2)*|U|^2.
				// Subtracting and solving for s yields
				//   s = ((R0^2-R1^2)/|U|^2 + 1)/2
				// Then replace in the first equation and solve for t^2
				//   t^2 = (R0^2/|U|^2) - s^2.
				// In order for there to be solutions, the right-hand side must be
				// nonnegative.  Some algebra leads to the condition for existence of
				// solutions,
				//   (|U|^2 - (R0+R1)^2)*(|U|^2 - (R0-R1)^2) <= 0.
				// This reduces to
				//   |R0-R1| <= |U| <= |R0+R1|.
				// If |U| = |R0-R1|, then the circles are side-by-side and just tangent.
				// If |U| = |R0+R1|, then the circles are nested and just tangent.
				// If |R0-R1| < |U| < |R0+R1|, then the two circles to intersect in two
				// points.

				info.Point0 = info.Point1 = Vector2D.zero;

				Vector2D U = circle1.Center - circle0.Center;
				double USqrLen = U.sqrMagnitude;
				double R0 = circle0.Radius, R1 = circle1.Radius;
				double R0mR1 = R0 - R1;

				if (USqrLen < Mathex.ZeroToleranceSqr &&
					System.Math.Abs(R0mR1) < Mathex.ZeroTolerance)
				{
					// Circles are essentially the same.
					info.IntersectionType = IntersectionTypes.Other;
					info.Quantity = 0;
					return true;
				}

				double R0mR1Sqr = R0mR1 * R0mR1;
				if (USqrLen < R0mR1Sqr)
				{
					info.IntersectionType = IntersectionTypes.Empty;
					info.Quantity = 0;
					return false;
				}

				double R0pR1 = R0 + R1;
				double R0pR1Sqr = R0pR1 * R0pR1;
				if (USqrLen > R0pR1Sqr)
				{
					info.IntersectionType = IntersectionTypes.Empty;
					info.Quantity = 0;
					return false;
				}

				if (USqrLen < R0pR1Sqr)
				{
					if (R0mR1Sqr < USqrLen)
					{
						double invUSqrLen = 1f / USqrLen;
						double s = 0.5f * ((R0 * R0 - R1 * R1) * invUSqrLen + 1f);
						Vector2D tmp = circle0.Center + s * U;

						// In theory, discr is nonnegative.  However, numerical round-off
						// errors can make it slightly negative.  Clamp it to zero.
						double discr = R0 * R0 * invUSqrLen - s * s;
						if (discr < 0f)
						{
							discr = 0f;
						}
						double t = System.Math.Sqrt(discr);
						Vector2D V = new Vector2D(U.y, -U.x);
						info.Quantity = 2;
						info.Point0 = tmp - t * V;
						info.Point1 = tmp + t * V;
					}
					else
					{
						// |U| = |R0-R1|, circles are tangent.
						info.Quantity = 1;
						info.Point0 = circle0.Center + (R0 / R0mR1) * U;
					}
				}
				else
				{
					// |U| = |R0+R1|, circles are tangent.
					info.Quantity = 1;
					info.Point0 = circle0.Center + (R0 / R0pR1) * U;
				}

				info.IntersectionType = IntersectionTypes.Point;
				return true;
			}
		}
	}
}
