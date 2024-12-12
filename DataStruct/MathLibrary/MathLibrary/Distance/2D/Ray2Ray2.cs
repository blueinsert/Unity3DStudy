

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between two rays
			/// </summary>
			public static double Ray2Ray2(ref Ray2 ray0, ref Ray2 ray1)
			{
				Vector2D closestPoint0, closestPoint1;
				return System.Math.Sqrt(SqrRay2Ray2(ref ray0, ref ray1, out closestPoint0, out closestPoint1));
			}

			/// <summary>
			/// Returns distance between two rays
			/// </summary>
			/// <param name="closestPoint0">Point on ray0 closest to ray1</param>
			/// <param name="closestPoint1">Point on ray1 closest to ray0</param>
			public static double Ray2Ray2(ref Ray2 ray0, ref Ray2 ray1, out Vector2D closestPoint0, out Vector2D closestPoint1)
			{
				return System.Math.Sqrt(SqrRay2Ray2(ref ray0, ref ray1, out closestPoint0, out closestPoint1));
			}


			/// <summary>
			/// Returns squared distance between two rays
			/// </summary>
			public static double SqrRay2Ray2(ref Ray2 ray0, ref Ray2 ray1)
			{
				Vector2D closestPoint0, closestPoint1;
				return SqrRay2Ray2(ref ray0, ref ray1, out closestPoint0, out closestPoint1);
			}

			/// <summary>
			/// Returns squared distance between two rays
			/// </summary>
			/// <param name="closestPoint0">Point on ray0 closest to ray1</param>
			/// <param name="closestPoint1">Point on ray1 closest to ray0</param>
			public static double SqrRay2Ray2(ref Ray2 ray0, ref Ray2 ray1, out Vector2D closestPoint0, out Vector2D closestPoint1)
			{
				Vector2D diff = ray0.Center - ray1.Center;
				double a01 = -ray0.Direction.Dot(ray1.Direction);
				double b0 = diff.Dot(ray0.Direction);
				double c = diff.sqrMagnitude;
				double det = System.Math.Abs(1f - a01 * a01);
				double b1, s0, s1, sqrDist;

				if (det >= Mathex.ZeroTolerance)
				{
					// Rays are not parallel.
					b1 = -diff.Dot(ray1.Direction);
					s0 = a01 * b1 - b0;
					s1 = a01 * b0 - b1;

					if (s0 >= 0f)
					{
						if (s1 >= 0f)  // region 0 (interior)
						{
							// Minimum at two interior points of rays.
							double invDet = 1f / det;
							s0 *= invDet;
							s1 *= invDet;
							sqrDist = 0f;
						}
						else  // region 3 (side)
						{
							s1 = 0f;
							if (b0 >= 0f)
							{
								s0 = 0f;
								sqrDist = c;
							}
							else
							{
								s0 = -b0;
								sqrDist = b0 * s0 + c;
							}
						}
					}
					else
					{
						if (s1 >= 0f)  // region 1 (side)
						{
							s0 = 0f;
							if (b1 >= 0f)
							{
								s1 = 0f;
								sqrDist = c;
							}
							else
							{
								s1 = -b1;
								sqrDist = b1 * s1 + c;
							}
						}
						else  // region 2 (corner)
						{
							if (b0 < 0f)
							{
								s0 = -b0;
								s1 = 0f;
								sqrDist = b0 * s0 + c;
							}
							else
							{
								s0 = 0f;
								if (b1 >= 0f)
								{
									s1 = 0f;
									sqrDist = c;
								}
								else
								{
									s1 = -b1;
									sqrDist = b1 * s1 + c;
								}
							}
						}
					}
				}
				else
				{
					// Rays are parallel.
					if (a01 > 0.0f)
					{
						// Opposite direction vectors.
						s1 = 0f;
						if (b0 >= 0f)
						{
							s0 = 0f;
							sqrDist = c;
						}
						else
						{
							s0 = -b0;
							sqrDist = b0 * s0 + c;
						}
					}
					else
					{
						// Same direction vectors.
						if (b0 >= 0f)
						{
							b1 = -diff.Dot(ray1.Direction);
							s0 = 0f;
							s1 = -b1;
							sqrDist = b1 * s1 + c;
						}
						else
						{
							s0 = -b0;
							s1 = 0f;
							sqrDist = b0 * s0 + c;
						}
					}
				}

				closestPoint0 = ray0.Center + s0 * ray0.Direction;
				closestPoint1 = ray1.Center + s1 * ray1.Direction;

				// Account for numerical round-off errors.
				if (sqrDist < 0f)
				{
					sqrDist = 0f;
				}
				return sqrDist;
			}
		}
	}
}