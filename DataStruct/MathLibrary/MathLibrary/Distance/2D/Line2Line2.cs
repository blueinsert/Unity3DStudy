

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between two lines.
			/// </summary>
			public static double Line2Line2(ref Line2 line0, ref Line2 line1)
			{
				return System.Math.Sqrt(SqrLine2Line2(ref line0, ref line1));
			}

			/// <summary>
			/// Returns distance between two lines.
			/// </summary>
			/// <param name="closestPoint0">Point on line0 closest to line1</param>
			/// <param name="closestPoint1">Point on line1 closest to line0</param>
			public static double Line2Line2(ref Line2 line0, ref Line2 line1, out Vector2D closestPoint0, out Vector2D closestPoint1)
			{
				return System.Math.Sqrt(SqrLine2Line2(ref line0, ref line1, out closestPoint0, out closestPoint1));
			}


			/// <summary>
			/// Returns squared distance between two lines.
			/// </summary>
			public static double SqrLine2Line2(ref Line2 line0, ref Line2 line1)
			{
				Vector2D diff = line0.Center - line1.Center;
				double a01 = -line0.Direction.Dot(line1.Direction);
				double b0 = diff.Dot(line0.Direction);
				double c = diff.sqrMagnitude;
				double det = System.Math.Abs(1f - a01 * a01);
				double s0, sqrDist;

				if (det >= Mathex.ZeroTolerance)
				{
					// Lines are not parallel.
					sqrDist = 0f;
				}
				else
				{
					// Lines are parallel, select any closest pair of points.
					s0 = -b0;
					sqrDist = b0 * s0 + c;

					// Account for numerical round-off errors.
					if (sqrDist < 0f)
					{
						sqrDist = 0f;
					}
				}

				return sqrDist;
			}

			/// <summary>
			/// Returns squared distance between two lines.
			/// </summary>
			/// <param name="closestPoint0">Point on line0 closest to line1</param>
			/// <param name="closestPoint1">Point on line1 closest to line0</param>
			public static double SqrLine2Line2(ref Line2 line0, ref Line2 line1, out Vector2D closestPoint0, out Vector2D closestPoint1)
			{
				Vector2D diff = line0.Center - line1.Center;
				double a01 = -line0.Direction.Dot(line1.Direction);
				double b0 = diff.Dot(line0.Direction);
				double c = diff.sqrMagnitude;
				double det = System.Math.Abs(1f - a01 * a01);
				double b1, s0, s1, sqrDist;

				if (det >= Mathex.ZeroTolerance)
				{
					// Lines are not parallel.
					b1 = -diff.Dot(line1.Direction);
					double invDet = 1f / det;
					s0 = (a01 * b1 - b0) * invDet;
					s1 = (a01 * b0 - b1) * invDet;
					sqrDist = 0f;
				}
				else
				{
					// Lines are parallel, select any closest pair of points.
					s0 = -b0;
					s1 = 0f;
					sqrDist = b0 * s0 + c;

					// Account for numerical round-off errors.
					if (sqrDist < 0f)
					{
						sqrDist = 0f;
					}
				}

				closestPoint0 = line0.Center + s0 * line0.Direction;
				closestPoint1 = line1.Center + s1 * line1.Direction;
				return sqrDist;
			}
		}
	}
}