

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a plane
			/// </summary>
			public static double Point3Plane3(ref Vector3D point, ref Plane3 plane)
			{
				double signedDistance = plane.Normal.Dot(point) - plane.Constant;
				return System.Math.Abs(signedDistance);
			}

			/// <summary>
			/// Returns distance between a point and a plane
			/// </summary>
			/// <param name="closestPoint">Point projected on a plane</param>
			public static double Point3Plane3(ref Vector3D point, ref Plane3 plane, out Vector3D closestPoint)
			{
				double signedDistance = plane.Normal.Dot(point) - plane.Constant;
				closestPoint = point - signedDistance * plane.Normal;
				return System.Math.Abs(signedDistance);
			}


			/// <summary>
			/// Returns squared distance between a point and a plane
			/// </summary>
			public static double SqrPoint3Plane3(ref Vector3D point, ref Plane3 plane)
			{
				double signedDistance = plane.Normal.Dot(point) - plane.Constant;
				return signedDistance * signedDistance;
			}

			/// <summary>
			/// Returns squared distance between a point and a plane
			/// </summary>
			/// <param name="closestPoint">Point projected on a plane</param>
			public static double SqrPoint3Plane3(ref Vector3D point, ref Plane3 plane, out Vector3D closestPoint)
			{
				double signedDistance = plane.Normal.Dot(point) - plane.Constant;
				closestPoint = point - signedDistance * plane.Normal;
				return signedDistance * signedDistance;
			}
		}
	}
}
