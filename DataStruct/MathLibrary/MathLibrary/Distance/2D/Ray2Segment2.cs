

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a ray and a segment
			/// </summary>
			public static double Ray2Segment2(ref Ray2 ray, ref Segment2 segment)
			{
				Vector2D closestPoint0, closestPoint1;
				return System.Math.Sqrt(SqrRay2Segment2(ref ray, ref segment, out closestPoint0, out closestPoint1));
			}

			/// <summary>
			/// Returns distance between a ray and a segment
			/// </summary>
			/// <param name="closestPoint0">Point on ray closest to segment</param>
			/// <param name="closestPoint1">Point on segment closest to ray</param>
			public static double Ray2Segment2(ref Ray2 ray, ref Segment2 segment, out Vector2D closestPoint0, out Vector2D closestPoint1)
			{
				return System.Math.Sqrt(SqrRay2Segment2(ref ray, ref segment, out closestPoint0, out closestPoint1));
			}


			/// <summary>
			/// Returns squared distance between a ray and a segment
			/// </summary>
			public static double SqrRay2Segment2(ref Ray2 ray, ref Segment2 segment)
			{
				Vector2D closestPoint0, closestPoint1;
				return SqrRay2Segment2(ref ray, ref segment, out closestPoint0, out closestPoint1);
			}

			/// <summary>
			/// Returns squared distance between a ray and a segment
			/// </summary>
			/// <param name="closestPoint0">Point on ray closest to segment</param>
			/// <param name="closestPoint1">Point on segment closest to ray</param>
			public static double SqrRay2Segment2(ref Ray2 ray, ref Segment2 segment, out Vector2D closestPoint0, out Vector2D closestPoint1)
			{
				Vector2D diff = ray.Center - segment.Center;
				double a01 = -ray.Direction.Dot(segment.Direction);
				double b0 = diff.Dot(ray.Direction);
				double b1 = -diff.Dot(segment.Direction);
				double c = diff.sqrMagnitude;
				double det = System.Math.Abs((double)1 - a01 * a01);
				double s0, s1, sqrDist, extDet;

				if (det >= Mathex.ZeroTolerance)
				{
					// The ray and segment are not parallel.
					s0 = a01 * b1 - b0;
					s1 = a01 * b0 - b1;
					extDet = segment.Extent * det;

					if (s0 >= (double)0)
					{
						if (s1 >= -extDet)
						{
							if (s1 <= extDet)  // region 0
							{
								// Minimum at interior points of ray and segment.
								double invDet = ((double)1) / det;
								s0 *= invDet;
								s1 *= invDet;
								sqrDist = (double)0;
							}
							else  // region 1
							{
								s1 = segment.Extent;
								s0 = -(a01 * s1 + b0);
								if (s0 > (double)0)
								{
									sqrDist = -s0 * s0 + s1 * (s1 + ((double)2) * b1) + c;
								}
								else
								{
									s0 = (double)0;
									sqrDist = s1 * (s1 + ((double)2) * b1) + c;
								}
							}
						}
						else  // region 5
						{
							s1 = -segment.Extent;
							s0 = -(a01 * s1 + b0);
							if (s0 > (double)0)
							{
								sqrDist = -s0 * s0 + s1 * (s1 + ((double)2) * b1) + c;
							}
							else
							{
								s0 = (double)0;
								sqrDist = s1 * (s1 + ((double)2) * b1) + c;
							}
						}
					}
					else
					{
						if (s1 <= -extDet)  // region 4
						{
							s0 = -(-a01 * segment.Extent + b0);
							if (s0 > (double)0)
							{
								s1 = -segment.Extent;
								sqrDist = -s0 * s0 + s1 * (s1 + ((double)2) * b1) + c;
							}
							else
							{
								s0 = (double)0;
								s1 = -b1;
								if (s1 < -segment.Extent)
								{
									s1 = -segment.Extent;
								}
								else if (s1 > segment.Extent)
								{
									s1 = segment.Extent;
								}
								sqrDist = s1 * (s1 + ((double)2) * b1) + c;
							}
						}
						else if (s1 <= extDet)  // region 3
						{
							s0 = (double)0;
							s1 = -b1;
							if (s1 < -segment.Extent)
							{
								s1 = -segment.Extent;
							}
							else if (s1 > segment.Extent)
							{
								s1 = segment.Extent;
							}
							sqrDist = s1 * (s1 + ((double)2) * b1) + c;
						}
						else  // region 2
						{
							s0 = -(a01 * segment.Extent + b0);
							if (s0 > (double)0)
							{
								s1 = segment.Extent;
								sqrDist = -s0 * s0 + s1 * (s1 + ((double)2) * b1) + c;
							}
							else
							{
								s0 = (double)0;
								s1 = -b1;
								if (s1 < -segment.Extent)
								{
									s1 = -segment.Extent;
								}
								else if (s1 > segment.Extent)
								{
									s1 = segment.Extent;
								}
								sqrDist = s1 * (s1 + ((double)2) * b1) + c;
							}
						}
					}
				}
				else
				{
					// Ray and segment are parallel.
					if (a01 > (double)0)
					{
						// Opposite direction vectors.
						s1 = -segment.Extent;
					}
					else
					{
						// Same direction vectors.
						s1 = segment.Extent;
					}

					s0 = -(a01 * s1 + b0);
					if (s0 > (double)0)
					{
						sqrDist = -s0 * s0 + s1 * (s1 + ((double)2) * b1) + c;
					}
					else
					{
						s0 = (double)0;
						sqrDist = s1 * (s1 + ((double)2) * b1) + c;
					}
				}

				closestPoint0 = ray.Center + s0 * ray.Direction;
				closestPoint1 = segment.Center + s1 * segment.Direction;

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