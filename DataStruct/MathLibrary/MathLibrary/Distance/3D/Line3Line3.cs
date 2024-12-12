

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between two lines.
			/// </summary>
			public static double Line3Line3(ref Line3 line0, ref Line3 line1)
			{
				Vector3D closestPoint0, closestPoint1;
				return System.Math.Sqrt(SqrLine3Line3(ref line0, ref line1, out closestPoint0, out closestPoint1));
			}

			/// <summary>
			/// Returns distance between two lines.
			/// </summary>
			/// <param name="closestPoint0">Point on line0 closest to line1</param>
			/// <param name="closestPoint1">Point on line1 closest to line0</param>
			public static double Line3Line3(ref Line3 line0, ref Line3 line1, out Vector3D closestPoint0, out Vector3D closestPoint1)
			{
				return System.Math.Sqrt(SqrLine3Line3(ref line0, ref line1, out closestPoint0, out closestPoint1));
			}


			/// <summary>
			/// Returns squared distance between two lines.
			/// </summary>
			public static double SqrLine3Line3(ref Line3 line0, ref Line3 line1)
			{
				Vector3D closestPoint0, closestPoint1;
				return SqrLine3Line3(ref line0, ref line1, out closestPoint0, out closestPoint1);
			}

			/// <summary>
			/// Returns squared distance between two lines.
			/// </summary>
			/// <param name="closestPoint0">Point on line0 closest to line1</param>
			/// <param name="closestPoint1">Point on line1 closest to line0</param>
			public static double SqrLine3Line3(ref Line3 line0, ref Line3 line1, out Vector3D closestPoint0, out Vector3D closestPoint1)
			{
				Vector3D diff = line0.Center - line1.Center;
				double a01 = -line0.Direction.Dot(line1.Direction);
				double b0 = diff.Dot(line0.Direction);
				double c = diff.sqrMagnitude;
				double det = System.Math.Abs((double)1 - a01 * a01);
				double b1, s0, s1, sqrDist;

				if (det >= Mathex.ZeroTolerance)
				{
					// Lines are not parallel.
					b1 = -diff.Dot(line1.Direction);
					double invDet = ((double)1) / det;
					s0 = (a01 * b1 - b0) * invDet;
					s1 = (a01 * b0 - b1) * invDet;
					sqrDist = s0 * (s0 + a01 * s1 + ((double)2) * b0) +
						s1 * (a01 * s0 + s1 + ((double)2) * b1) + c;
				}
				else
				{
					// Lines are parallel, select any closest pair of points.
					s0 = -b0;
					s1 = (double)0;
					sqrDist = b0 * s0 + c;
				}

				closestPoint0 = line0.Center + s0 * line0.Direction;
				closestPoint1 = line1.Center + s1 * line1.Direction;

				// Account for numerical round-off errors.
				if (sqrDist < (double)0)
				{
					sqrDist = (double)0;
				}
				return sqrDist;
			}
		}
	}
}