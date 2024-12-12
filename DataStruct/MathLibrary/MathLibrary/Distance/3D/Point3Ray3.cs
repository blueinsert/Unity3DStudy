

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a ray
			/// </summary>
			public static double Point3Ray3(ref Vector3D point, ref Ray3 ray)
			{
				return System.Math.Sqrt(SqrPoint3Ray3(ref point, ref ray));
			}

			/// <summary>
			/// Returns distance between a point and a ray
			/// </summary>
			/// <param name="closestPoint">Point projected on a ray and clamped by ray origin</param>
			public static double Point3Ray3(ref Vector3D point, ref Ray3 ray, out Vector3D closestPoint)
			{
				return System.Math.Sqrt(SqrPoint3Ray3(ref point, ref ray, out closestPoint));
			}


			/// <summary>
			/// Returns squared distance between a point and a ray
			/// </summary>
			public static double SqrPoint3Ray3(ref Vector3D point, ref Ray3 ray)
			{
				Vector3D diff = point - ray.Center;
				double param = ray.Direction.Dot(diff);
				Vector3D closestPoint;
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
			public static double SqrPoint3Ray3(ref Vector3D point, ref Ray3 ray, out Vector3D closestPoint)
			{
				Vector3D diff = point - ray.Center;
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