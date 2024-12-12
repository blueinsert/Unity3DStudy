

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a line
			/// </summary>
			public static double Point3Line3(ref Vector3D point, ref Line3 line)
			{
				return System.Math.Sqrt(SqrPoint3Line3(ref point, ref line));
			}

			/// <summary>
			/// Returns distance between a point and a line
			/// </summary>
			/// <param name="closestPoint">Point projected on a line</param>
			public static double Point3Line3(ref Vector3D point, ref Line3 line, out Vector3D closestPoint)
			{
				return System.Math.Sqrt(SqrPoint3Line3(ref point, ref line, out closestPoint));
			}


			/// <summary>
			/// Returns squared distance between a point and a line
			/// </summary>
			public static double SqrPoint3Line3(ref Vector3D point, ref Line3 line)
			{
				Vector3D diff = point - line.Center;
				double param = line.Direction.Dot(diff);
				Vector3D closestPoint = line.Center + param * line.Direction;
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}

			/// <summary>
			/// Returns squared distance between a point and a line
			/// </summary>
			/// <param name="closestPoint">Point projected on a line</param>
			public static double SqrPoint3Line3(ref Vector3D point, ref Line3 line, out Vector3D closestPoint)
			{
				Vector3D diff = point - line.Center;
				double param = line.Direction.Dot(diff);
				closestPoint = line.Center + param * line.Direction;
				diff = closestPoint - point;
				return diff.sqrMagnitude;
			}
		}
	}
}