

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a segment
			/// </summary>
			public static double Point2Segment2(ref Vector2D point, ref Segment2 segment)
			{
				return System.Math.Sqrt(SqrPoint2Segment2(ref point, ref segment));
			}

			/// <summary>
			/// Returns distance between a point and a segment
			/// </summary>
			/// <param name="closestPoint">Point projected on a segment and clamped by segment endpoints</param>
			public static double Point2Segment2(ref Vector2D point, ref Segment2 segment, out Vector2D closestPoint)
			{
				return System.Math.Sqrt(SqrPoint2Segment2(ref point, ref segment, out closestPoint));
			}


			/// <summary>
			/// Returns squared distance between a point and a segment
			/// </summary>
			public static double SqrPoint2Segment2(ref Vector2D point, ref Segment2 segment)
			{
				Vector2D diff = point - segment.Center;
				double param = segment.Direction.Dot(diff);
				Vector2D closestPoint;
				if (-segment.Extent < param)
				{
					if (param < segment.Extent)
					{
						closestPoint = segment.Center + param * segment.Direction;
					}
					else
					{
						closestPoint = segment.P1;
					}
				}
				else
				{
					closestPoint = segment.P0;
				}
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}

			/// <summary>
			/// Returns squared distance between a point and a segment
			/// </summary>
			/// <param name="closestPoint">Point projected on a segment and clamped by segment endpoints</param>
			public static double SqrPoint2Segment2(ref Vector2D point, ref Segment2 segment, out Vector2D closestPoint)
			{
				Vector2D diff = point - segment.Center;
				double param = segment.Direction.Dot(diff);
				if (-segment.Extent < param)
				{
					if (param < segment.Extent)
					{
						closestPoint = segment.Center + param * segment.Direction;
					}
					else
					{
						closestPoint = segment.P1;
					}
				}
				else
				{
					closestPoint = segment.P0;
				}
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}
		}
	}
}
