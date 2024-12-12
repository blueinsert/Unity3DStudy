

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a ray
			/// </summary>
			public static double Point2Ray2(ref Vector2D point, ref Ray2 ray)
			{
				return System.Math.Sqrt(SqrPoint2Ray2(ref point, ref ray));
			}

			/// <summary>
			/// Returns distance between a point and a ray
			/// </summary>
			/// <param name="closestPoint">Point projected on a ray and clamped by ray origin</param>
			public static double Point2Ray2(ref Vector2D point, ref Ray2 ray, out Vector2D closestPoint)
			{
				return System.Math.Sqrt(SqrPoint2Ray2(ref point, ref ray, out closestPoint));
			}


			/// <summary>
			/// Returns squared distance between a point and a ray
			/// </summary>
			public static double SqrPoint2Ray2(ref Vector2D point, ref Ray2 ray)
			{
				Vector2D diff = point - ray.Center;
				double param = ray.Direction.Dot(diff);
				Vector2D closestPoint;
				if (param > 0.0f)
				{
					closestPoint = ray.Center + param * ray.Direction;
				}
				else
				{
					closestPoint = ray.Center;
				}
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}

			/// <summary>
			/// Returns squared distance between a point and a ray
			/// </summary>
			/// <param name="closestPoint">Point projected on a ray and clamped by ray origin</param>
			public static double SqrPoint2Ray2(ref Vector2D point, ref Ray2 ray, out Vector2D closestPoint)
			{
				Vector2D diff = point - ray.Center;
				double param = ray.Direction.Dot(diff);
				if (param > 0.0f)
				{
					closestPoint = ray.Center + param * ray.Direction;
				}
				else
				{
					closestPoint = ray.Center;
				}
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}
		}
	}
}
