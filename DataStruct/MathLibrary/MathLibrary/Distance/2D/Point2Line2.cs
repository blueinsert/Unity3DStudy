

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a line
			/// </summary>
			public static double Point2Line2(ref Vector2D point, ref Line2 line)
			{
				return System.Math.Sqrt(SqrPoint2Line2(ref point, ref line));
			}

			/// <summary>
			/// Returns distance between a point and a line
			/// </summary>
			/// <param name="closestPoint">Point projected on a line</param>
			public static double Point2Line2(ref Vector2D point, ref Line2 line, out Vector2D closestPoint)
			{
				return System.Math.Sqrt(SqrPoint2Line2(ref point, ref line, out closestPoint));
			}


			/// <summary>
			/// Returns squared distance between a point and a line
			/// </summary>
			public static double SqrPoint2Line2(ref Vector2D point, ref Line2 line)
			{
				Vector2D diff = point - line.Center;
				double param = line.Direction.Dot(diff);
				Vector2D closestPoint = line.Center + param * line.Direction;
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}

			/// <summary>
			/// Returns squared distance between a point and a line
			/// </summary>
			/// <param name="closestPoint">Point projected on a line</param>
			public static double SqrPoint2Line2(ref Vector2D point, ref Line2 line, out Vector2D closestPoint)
			{
				Vector2D diff = point - line.Center;
				double param = line.Direction.Dot(diff);
				closestPoint = line.Center + param * line.Direction;
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}
		}
	}
}
