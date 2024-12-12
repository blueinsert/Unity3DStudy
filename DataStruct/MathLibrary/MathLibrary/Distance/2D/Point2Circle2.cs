

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a circle
			/// </summary>
			public static double Point2Circle2(ref Vector2D point, ref Circle2 circle)
			{
				double diff = (point - circle.Center).magnitude - circle.Radius;
				return diff > 0f ? diff : 0f;
			}

			/// <summary>
			/// Returns distance between a point and a circle
			/// </summary>
			/// <param name="closestPoint">Point projected on a circle</param>
			public static double Point2Circle2(ref Vector2D point, ref Circle2 circle, out Vector2D closestPoint)
			{
				Vector2D diff = point - circle.Center;
				double diffSqrLen = diff.sqrMagnitude;
				if (diffSqrLen > circle.Radius * circle.Radius)
				{
					double diffLen = System.Math.Sqrt(diffSqrLen);
					closestPoint = circle.Center + diff * (circle.Radius / diffLen);
					return diffLen - circle.Radius;
				}
				closestPoint = point;
				return 0f;
			}

			
			/// <summary>
			/// Returns squared distance between a point and a circle
			/// </summary>
			public static double SqrPoint2Circle2(ref Vector2D point, ref Circle2 circle)
			{
				double diff = (point - circle.Center).magnitude - circle.Radius;
				return diff > 0f ? diff * diff : 0f;
			}

			/// <summary>
			/// Returns squared distance between a point and a circle
			/// </summary>
			/// <param name="closestPoint">Point projected on a circle</param>
			public static double SqrPoint2Circle2(ref Vector2D point, ref Circle2 circle, out Vector2D closestPoint)
			{
				Vector2D diff = point - circle.Center;
				double diffSqrLen = diff.sqrMagnitude;
				if (diffSqrLen > circle.Radius * circle.Radius)
				{
					double diffLen = System.Math.Sqrt(diffSqrLen);
					closestPoint = circle.Center + diff * (circle.Radius / diffLen);
					double result = diffLen - circle.Radius;
					return result * result;
				}
				closestPoint = point;
				return 0f;
			}
		}
	}
}