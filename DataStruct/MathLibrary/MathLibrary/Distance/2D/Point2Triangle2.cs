

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a triangle
			/// </summary>
			public static double Point2Triangle2(ref Vector2D point, ref Triangle2 triangle)
			{
				if (triangle.Contains(point))
				{
					return 0f;
				}

				Segment2 segment0 = new Segment2(ref triangle.V0, ref triangle.V1);
				double dist0 = Distance.Point2Segment2(ref point, ref segment0);

				Segment2 segment1 = new Segment2(ref triangle.V1, ref triangle.V2);
				double dist1 = Distance.Point2Segment2(ref point, ref segment1);

				Segment2 segment2 = new Segment2(ref triangle.V2, ref triangle.V0);
				double dist2 = Distance.Point2Segment2(ref point, ref segment2);

				if (dist0 < dist1)
				{
					if (dist0 < dist2)
					{
						return dist0;
					}
					else
					{
						if (dist1 < dist2)
						{
							return dist1;
						}
						else
						{
							return dist2;
						}
					}
				}
				else
				{
					if (dist1 < dist2)
					{
						return dist1;
					}
					else
					{
						if (dist0 < dist2)
						{
							return dist0;
						}
						else
						{
							return dist2;
						}
					}
				}
			}

			/// <summary>
			/// Returns distance between a point and a triangle
			/// </summary>
			/// <param name="closestPoint">Point projected on a triangle</param>
			public static double Point2Triangle2(ref Vector2D point, ref Triangle2 triangle, out Vector2D closestPoint)
			{
				if (triangle.Contains(point))
				{
					closestPoint = point;
					return 0f;
				}

				Segment2 segment0 = new Segment2(ref triangle.V0, ref triangle.V1);
				Vector2D closest0;
				double dist0 = Distance.Point2Segment2(ref point, ref segment0, out closest0);

				Segment2 segment1 = new Segment2(ref triangle.V1, ref triangle.V2);
				Vector2D closest1;
				double dist1 = Distance.Point2Segment2(ref point, ref segment1, out closest1);

				Segment2 segment2 = new Segment2(ref triangle.V2, ref triangle.V0);
				Vector2D closest2;
				double dist2 = Distance.Point2Segment2(ref point, ref segment2, out closest2);

				if (dist0 < dist1)
				{
					if (dist0 < dist2)
					{
						closestPoint = closest0;
						return dist0;
					}
					else
					{
						if (dist1 < dist2)
						{
							closestPoint = closest1;
							return dist1;
						}
						else
						{
							closestPoint = closest2;
							return dist2;
						}
					}
				}
				else
				{
					if (dist1 < dist2)
					{
						closestPoint = closest1;
						return dist1;
					}
					else
					{
						if (dist0 < dist2)
						{
							closestPoint = closest0;
							return dist0;
						}
						else
						{
							closestPoint = closest2;
							return dist2;
						}
					}
				}
			}


			/// <summary>
			/// Returns squared distance between a point and a triangle
			/// </summary>
			public static double SqrPoint2Triangle2(ref Vector2D point, ref Triangle2 triangle)
			{
				if (triangle.Contains(point))
				{
					return 0f;
				}

				Segment2 segment0 = new Segment2(ref triangle.V0, ref triangle.V1);
				double dist0 = Distance.SqrPoint2Segment2(ref point, ref segment0);

				Segment2 segment1 = new Segment2(ref triangle.V1, ref triangle.V2);
				double dist1 = Distance.SqrPoint2Segment2(ref point, ref segment1);

				Segment2 segment2 = new Segment2(ref triangle.V2, ref triangle.V0);
				double dist2 = Distance.SqrPoint2Segment2(ref point, ref segment2);

				if (dist0 < dist1)
				{
					if (dist0 < dist2)
					{
						return dist0;
					}
					else
					{
						if (dist1 < dist2)
						{
							return dist1;
						}
						else
						{
							return dist2;
						}
					}
				}
				else
				{
					if (dist1 < dist2)
					{
						return dist1;
					}
					else
					{
						if (dist0 < dist2)
						{
							return dist0;
						}
						else
						{
							return dist2;
						}
					}
				}
			}

			/// <summary>
			/// Returns squared distance between a point and a triangle
			/// </summary>
			/// <param name="closestPoint">Point projected on a triangle</param>
			public static double SqrPoint2Triangle2(ref Vector2D point, ref Triangle2 triangle, out Vector2D closestPoint)
			{
				if (triangle.Contains(point))
				{
					closestPoint = point;
					return 0f;
				}

				Segment2 segment0 = new Segment2(ref triangle.V0, ref triangle.V1);
				Vector2D closest0;
				double dist0 = Distance.SqrPoint2Segment2(ref point, ref segment0, out closest0);

				Segment2 segment1 = new Segment2(ref triangle.V1, ref triangle.V2);
				Vector2D closest1;
				double dist1 = Distance.SqrPoint2Segment2(ref point, ref segment1, out closest1);

				Segment2 segment2 = new Segment2(ref triangle.V2, ref triangle.V0);
				Vector2D closest2;
				double dist2 = Distance.SqrPoint2Segment2(ref point, ref segment2, out closest2);

				if (dist0 < dist1)
				{
					if (dist0 < dist2)
					{
						closestPoint = closest0;
						return dist0;
					}
					else
					{
						if (dist1 < dist2)
						{
							closestPoint = closest1;
							return dist1;
						}
						else
						{
							closestPoint = closest2;
							return dist2;
						}
					}
				}
				else
				{
					if (dist1 < dist2)
					{
						closestPoint = closest1;
						return dist1;
					}
					else
					{
						if (dist0 < dist2)
						{
							closestPoint = closest0;
							return dist0;
						}
						else
						{
							closestPoint = closest2;
							return dist2;
						}
					}
				}
			}
		}
	}
}
