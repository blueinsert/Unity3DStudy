

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a rectangle
			/// </summary>
			public static double Point3Rectangle3(ref Vector3D point, ref Rectangle3 rectangle)
			{
				return System.Math.Sqrt(SqrPoint3Rectangle3(ref point, ref rectangle));
			}

			/// <summary>
			/// Returns distance between a point and a rectangle
			/// </summary>
			/// <param name="closestPoint">Point projected on a rectangle</param>
			public static double Point3Rectangle3(ref Vector3D point, ref Rectangle3 rectangle, out Vector3D closestPoint)
			{
				return System.Math.Sqrt(SqrPoint3Rectangle3(ref point, ref rectangle, out closestPoint));
			}


			/// <summary>
			/// Returns squared distance between a point and a rectangle
			/// </summary>
			public static double SqrPoint3Rectangle3(ref Vector3D point, ref Rectangle3 rectangle)
			{
				Vector3D diff = rectangle.Center - point;
				double b0 = diff.Dot(rectangle.Axis0);
				double b1 = diff.Dot(rectangle.Axis1);
				double s0 = -b0, s1 = -b1;
				double sqrDistance = diff.sqrMagnitude;
				double extent;

				extent = rectangle.Extents.x;
				if (s0 < -extent)
				{
					s0 = -extent;
				}
				else if (s0 > extent)
				{
					s0 = extent;
				}
				sqrDistance += s0 * (s0 + 2f * b0);

				extent = rectangle.Extents.y;
				if (s1 < -extent)
				{
					s1 = -extent;
				}
				else if (s1 > extent)
				{
					s1 = extent;
				}
				sqrDistance += s1 * (s1 + 2f * b1);

				// Account for numerical round-off error.
				if (sqrDistance < 0f)
				{
					sqrDistance = 0f;
				}

				return sqrDistance;
			}

			/// <summary>
			/// Returns squared distance between a point and a rectangle
			/// </summary>
			/// <param name="closestPoint">Point projected on a rectangle</param>
			public static double SqrPoint3Rectangle3(ref Vector3D point, ref Rectangle3 rectangle, out Vector3D closestPoint)
			{
				Vector3D diff = rectangle.Center - point;
				double b0 = diff.Dot(rectangle.Axis0);
				double b1 = diff.Dot(rectangle.Axis1);
				double s0 = -b0, s1 = -b1;
				double sqrDistance = diff.sqrMagnitude;
				double extent;

				extent = rectangle.Extents.x;
				if (s0 < -extent)
				{
					s0 = -extent;
				}
				else if (s0 > extent)
				{
					s0 = extent;
				}
				sqrDistance += s0 * (s0 + 2f * b0);

				extent = rectangle.Extents.y;
				if (s1 < -extent)
				{
					s1 = -extent;
				}
				else if (s1 > extent)
				{
					s1 = extent;
				}
				sqrDistance += s1 * (s1 + 2f * b1);

				// Account for numerical round-off error.
				if (sqrDistance < 0f)
				{
					sqrDistance = 0f;
				}

				closestPoint = rectangle.Center + s0 * rectangle.Axis0 + s1 * rectangle.Axis1;

				return sqrDistance;
			}
		}
	}
}
