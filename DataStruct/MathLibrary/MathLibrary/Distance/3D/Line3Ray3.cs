

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a line and a ray
			/// </summary>
			public static double Line3Ray3(ref Line3 line, ref Ray3 ray)
			{
				Vector3D closestPoint0, closestPoint1;
				return System.Math.Sqrt(SqrLine3Ray3(ref line, ref ray, out closestPoint0, out closestPoint1));
			}

			/// <summary>
			/// Returns distance between a line and a ray
			/// </summary>
			/// <param name="closestPoint0">Point on line closest to ray</param>
			/// <param name="closestPoint1">Point on ray closest to line</param>
			public static double Line3Ray3(ref Line3 line, ref Ray3 ray, out Vector3D closestPoint0, out Vector3D closestPoint1)
			{
				return System.Math.Sqrt(SqrLine3Ray3(ref line, ref ray, out closestPoint0, out closestPoint1));
			}


			/// <summary>
			/// Returns squared distance between a line and a ray
			/// </summary>
			public static double SqrLine3Ray3(ref Line3 line, ref Ray3 ray)
			{
				Vector3D closestPoint0, closestPoint1;
				return SqrLine3Ray3(ref line, ref ray, out closestPoint0, out closestPoint1);
			}

			/// <summary>
			/// Returns squared distance between a line and a ray
			/// </summary>
			/// <param name="closestPoint0">Point on line closest to ray</param>
			/// <param name="closestPoint1">Point on ray closest to line</param>
			public static double SqrLine3Ray3(ref Line3 line, ref Ray3 ray, out Vector3D closestPoint0, out Vector3D closestPoint1)
			{
				Vector3D kDiff = line.Center - ray.Center;
				double a01 = -line.Direction.Dot(ray.Direction);
				double b0 = kDiff.Dot(line.Direction);
				double c = kDiff.sqrMagnitude;
				double det = System.Math.Abs((double)1 - a01 * a01);
				double b1, s0, s1, sqrDist;

				if (det >= Mathex.ZeroTolerance)
				{
					b1 = -kDiff.Dot(ray.Direction);
					s1 = a01 * b0 - b1;

					if (s1 >= (double)0)
					{
						// Two interior points are closest, one on line and one on ray.
						double invDet = ((double)1) / det;
						s0 = (a01 * b1 - b0) * invDet;
						s1 *= invDet;
						sqrDist = s0 * (s0 + a01 * s1 + ((double)2) * b0) +
							s1 * (a01 * s0 + s1 + ((double)2) * b1) + c;
					}
					else
					{
						// Origin of ray and interior point of line are closest.
						s0 = -b0;
						s1 = (double)0;
						sqrDist = b0 * s0 + c;
					}
				}
				else
				{
					// Lines are parallel, closest pair with one point at ray origin.
					s0 = -b0;
					s1 = (double)0;
					sqrDist = b0 * s0 + c;
				}

				closestPoint0 = line.Center + s0 * line.Direction;
				closestPoint1 = ray.Center + s1 * ray.Direction;

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