

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and an abb
			/// </summary>
			public static double Point2AAB2(ref Vector2D point, ref AAB2 box)
			{
				// Compute squared distance and closest point on box.
				double distSquared = 0.0f;
				double pointCoord;
				double delta;

				pointCoord = point.x;
				if (pointCoord < box.Min.x)
				{
					delta = box.Min.x - pointCoord;
					distSquared += delta * delta;
				}
				else if (pointCoord > box.Max.x)
				{
					delta = pointCoord - box.Max.x;
					distSquared += delta * delta;
				}

				pointCoord = point.y;
				if (pointCoord < box.Min.y)
				{
					delta = box.Min.y - pointCoord;
					distSquared += delta * delta;
				}
				else if (pointCoord > box.Max.y)
				{
					delta = pointCoord - box.Max.y;
					distSquared += delta * delta;
				}

				return System.Math.Sqrt(distSquared);
			}

			/// <summary>
			/// Returns distance between a point and an abb
			/// </summary>
			/// <param name="closestPoint">Point projected on an aab</param>
			public static double Point2AAB2(ref Vector2D point, ref AAB2 box, out Vector2D closestPoint)
			{
				// Compute squared distance and closest point on box.
				double distSquared = 0.0f;
				double pointCoord;
				double delta;

				closestPoint = point;

				pointCoord = point.x;
				if (pointCoord < box.Min.x)
				{
					delta = box.Min.x - pointCoord;
					distSquared += delta * delta;
					closestPoint.x += delta;
				}
				else if (pointCoord > box.Max.x)
				{
					delta = pointCoord - box.Max.x;
					distSquared += delta * delta;
					closestPoint.x -= delta;
				}

				pointCoord = point.y;
				if (pointCoord < box.Min.y)
				{
					delta = box.Min.y - pointCoord;
					distSquared += delta * delta;
					closestPoint.y += delta;
				}
				else if (pointCoord > box.Max.y)
				{
					delta = pointCoord - box.Max.y;
					distSquared += delta * delta;
					closestPoint.y -= delta;
				}

				return System.Math.Sqrt(distSquared);
			}


			/// <summary>
			/// Returns squared distance between a point and an abb
			/// </summary>
			public static double SqrPoint2AAB2(ref Vector2D point, ref AAB2 box)
			{
				// Compute squared distance and closest point on box.
				double distSquared = 0.0f;
				double pointCoord;
				double delta;

				pointCoord = point.x;
				if (pointCoord < box.Min.x)
				{
					delta = box.Min.x - pointCoord;
					distSquared += delta * delta;
				}
				else if (pointCoord > box.Max.x)
				{
					delta = pointCoord - box.Max.x;
					distSquared += delta * delta;
				}

				pointCoord = point.y;
				if (pointCoord < box.Min.y)
				{
					delta = box.Min.y - pointCoord;
					distSquared += delta * delta;
				}
				else if (pointCoord > box.Max.y)
				{
					delta = pointCoord - box.Max.y;
					distSquared += delta * delta;
				}

				return distSquared;
			}

			/// <summary>
			/// Returns squared distance between a point and an abb
			/// </summary>
			/// <param name="closestPoint">Point projected on an aab</param>
			public static double SqrPoint2AAB2(ref Vector2D point, ref AAB2 box, out Vector2D closestPoint)
			{
				// Compute squared distance and closest point on box.
				double distSquared = 0.0f;
				double pointCoord;
				double delta;

				closestPoint = point;

				pointCoord = point.x;
				if (pointCoord < box.Min.x)
				{
					delta = box.Min.x - pointCoord;
					distSquared += delta * delta;
					closestPoint.x += delta;
				}
				else if (pointCoord > box.Max.x)
				{
					delta = pointCoord - box.Max.x;
					distSquared += delta * delta;
					closestPoint.x -= delta;
				}

				pointCoord = point.y;
				if (pointCoord < box.Min.y)
				{
					delta = box.Min.y - pointCoord;
					distSquared += delta * delta;
					closestPoint.y += delta;
				}
				else if (pointCoord > box.Max.y)
				{
					delta = pointCoord - box.Max.y;
					distSquared += delta * delta;
					closestPoint.y -= delta;
				}

				return distSquared;
			}
		}
	}
}
