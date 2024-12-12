

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of two Line2
		/// </summary>
		public struct Line2Line2Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point or IntersectionTypes.Line (lines are the same)
			/// if intersection occured otherwise IntersectionTypes.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// In case of IntersectionTypes.Point constains single point of intersection.
			/// Otherwise Vector2D.zero.
			/// </summary>
			public Vector2D Point;

			/// <summary>
			/// In case of IntersectionTypes.Point contains evaluation parameter of single
			/// intersection point according to first line.
			/// Otherwise 0.
			/// </summary>
			public double Parameter;
		}

		public static partial class Intersection
		{
			private static IntersectionTypes Classify(ref Line2 line0, ref Line2 line1, out double s0)
			{
				// The intersection of two lines is a solution to P0+s0*D0 = P1+s1*D1.
				// Rewrite this as s0*D0 - s1*D1 = P1 - P0 = Q.  If D0.Dot(Perp(D1)) = 0,
				// the lines are parallel.  Additionally, if Q.Dot(Perp(D1)) = 0, the
				// lines are the same.  If D0.Dot(Perp(D1)) is not zero, then
				//   s0 = Q.Dot(Perp(D1))/D0.Dot(Perp(D1))
				// produces the point of intersection.  Also,
				//   s1 = Q.Dot(Perp(D0))/D0.Dot(Perp(D1))

				Vector2D originDiff = line1.Center - line0.Center;

				s0 = 0f;

				double D0DotPerpD1 = line0.Direction.DotPerp(line1.Direction);

				if (System.Math.Abs(D0DotPerpD1) > _dotThreshold)
				{
					// Lines intersect in a single point.

					double diffDotPerpD1 = originDiff.DotPerp(line1.Direction);
					s0 = diffDotPerpD1 / D0DotPerpD1;

					return IntersectionTypes.Point;
				}

				// Lines are parallel.
				originDiff.Normalize();

				double diffNDotPerpD1 = originDiff.DotPerp(line1.Direction);
				if (System.Math.Abs(diffNDotPerpD1) <= _dotThreshold)
				{
					// Lines are colinear.
					return IntersectionTypes.Line;
				}

				// Lines are parallel, but distinct.
				return IntersectionTypes.Empty;
			}

			/// <summary>
			/// Tests whether two lines intersect.
			/// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Line),
			/// or false if lines do not intersect (IntersectionTypes.Empty).
			/// </summary>
			public static bool TestLine2Line2(ref Line2 line0, ref Line2 line1, out IntersectionTypes intersectionType)
			{
				double parameter;
				intersectionType = Classify(ref line0, ref line1, out parameter);

				return intersectionType != IntersectionTypes.Empty;
			}

			/// <summary>
			/// Tests whether two lines intersect.
			/// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Line),
			/// or false if lines do not intersect (IntersectionTypes.Empty).
			/// </summary>
			public static bool TestLine2Line2(ref Line2 line0, ref Line2 line1)
			{
				IntersectionTypes intersectionType;
				return TestLine2Line2(ref line0, ref line1, out intersectionType);
			}

			/// <summary>
			/// Tests whether two lines intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs (IntersectionTypes.Point, IntersectionTypes.Line),
			/// or false if lines do not intersect (IntersectionTypes.Empty).
			/// </summary>
			public static bool FindLine2Line2(ref Line2 line0, ref Line2 line1, out Line2Line2Intr info)
			{
				double parameter;
				info.IntersectionType = Classify(ref line0, ref line1, out parameter);

				if (info.IntersectionType == IntersectionTypes.Point)
				{
					info.Point = line0.Center + parameter * line0.Direction;
					info.Parameter = parameter;
				}
				else
				{
					info.Point = Vector2D.zero;
					info.Parameter = 0f;
				}

				return info.IntersectionType != IntersectionTypes.Empty;
			}
		}
	}
}
