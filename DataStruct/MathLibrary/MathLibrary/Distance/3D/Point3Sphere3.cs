

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a sphere
			/// </summary>
			public static double Point3Sphere3(ref Vector3D point, ref Sphere3 sphere)
			{
				double diff = (point - sphere.Center).magnitude - sphere.Radius;
				return diff > 0f ? diff : 0f;
			}

			/// <summary>
			/// Returns distance between a point and a sphere
			/// </summary>
			/// <param name="closestPoint">Point projected on a sphere</param>
			public static double Point3Sphere3(ref Vector3D point, ref Sphere3 sphere, out Vector3D closestPoint)
			{
				Vector3D diff = point - sphere.Center;
				double diffSqrLen = diff.sqrMagnitude;
				if (diffSqrLen > sphere.Radius * sphere.Radius)
				{
					double diffLen = System.Math.Sqrt(diffSqrLen);
					closestPoint = sphere.Center + diff * (sphere.Radius / diffLen);
					return diffLen - sphere.Radius;
				}
				closestPoint = point;
				return 0f;
			}


			/// <summary>
			/// Returns squared distance between a point and a sphere
			/// </summary>
			public static double SqrPoint3Sphere3(ref Vector3D point, ref Sphere3 sphere)
			{
				double diff = (point - sphere.Center).magnitude - sphere.Radius;
				return diff > 0f ? diff * diff : 0f;
			}

			/// <summary>
			/// Returns squared distance between a point and a sphere
			/// </summary>
			/// <param name="closestPoint">Point projected on a sphere</param>
			public static double SqrPoint3Sphere3(ref Vector3D point, ref Sphere3 sphere, out Vector3D closestPoint)
			{
				Vector3D diff = point - sphere.Center;
				double diffSqrLen = diff.sqrMagnitude;
				if (diffSqrLen > sphere.Radius * sphere.Radius)
				{
					double diffLen = System.Math.Sqrt(diffSqrLen);
					closestPoint = sphere.Center + diff * (sphere.Radius / diffLen);
					double result = diffLen - sphere.Radius;
					return result * result;
				}
				closestPoint = point;
				return 0f;
			}
		}
	}
}