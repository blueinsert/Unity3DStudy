

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray3 and Sphere3
		/// </summary>
		public struct Ray3Sphere3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point or IntersectionTypes.Segment if intersection occured otherwise IntersectionTypes.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Number of intersection points.
			/// IntersectionTypes.Empty: 0;
			/// IntersectionTypes.Point: 1;
			/// IntersectionTypes.Segment: 2.
			/// </summary>
			public int Quantity;

			/// <summary>
			/// First intersection point
			/// </summary>
			public Vector3D Point0;

			/// <summary>
			/// Second intersection point
			/// </summary>
			public Vector3D Point1;

			/// <summary>
			/// Ray evaluation parameter of the first intersection point
			/// </summary>
			public double RayParameter0;

			/// <summary>
			/// Ray evaluation parameter of the second intersection point
			/// </summary>
			public double RayParameter1;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a ray intersects a sphere. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay3Sphere3(ref Ray3 ray, ref Sphere3 sphere)
			{
				Vector3D diff = ray.Center - sphere.Center;
				double a0 = diff.Dot(diff) - sphere.Radius * sphere.Radius;

				if (a0 <= 0f)
				{
					// P is inside the sphere
					return true;
				}
				// else: P is outside the sphere

				double a1 = ray.Direction.Dot(diff);
				if (a1 >= 0f)
				{
					return false;
				}

				// Quadratic has a real root if discriminant is nonnegative.
				return a1 * a1 >= a0;
			}

			/// <summary>
			/// Tests if a ray intersects a sphere and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay3Sphere3(ref Ray3 ray, ref Sphere3 sphere, out Ray3Sphere3Intr info)
			{
				Vector3D diff = ray.Center - sphere.Center;
				double a0 = diff.Dot(diff) - sphere.Radius * sphere.Radius;
				double a1, discr, root;

				if (a0 <= 0f)
				{
					// P is inside the sphere
					a1 = ray.Direction.Dot(diff);
					discr = a1 * a1 - a0;
					root = System.Math.Sqrt(discr);

					info.RayParameter0    = -a1 + root;
					info.RayParameter1    = 0f;
					info.Point0           = ray.Center + info.RayParameter0 * ray.Direction;
					info.Point1           = Vector3D.zero;
					info.Quantity         = 1;
					info.IntersectionType = IntersectionTypes.Point;
					return true;
				}
				// else: P is outside the sphere

				a1 = ray.Direction.Dot(diff);
				if (a1 >= 0.0f)
				{
					info = new Ray3Sphere3Intr();
					return false;
				}

				discr = a1 * a1 - a0;
				if (discr < 0.0f)
				{
					info = new Ray3Sphere3Intr();
				}
				else if (discr >= Mathex.ZeroTolerance)
				{
					root = System.Math.Sqrt(discr);

					info.RayParameter0    = -a1 - root;
					info.RayParameter1    = -a1 + root;
					info.Point0           = ray.Center + info.RayParameter0  * ray.Direction;
					info.Point1           = ray.Center + info.RayParameter1 * ray.Direction;
					info.Quantity         = 2;
					info.IntersectionType = IntersectionTypes.Segment;
				}
				else
				{
					info.RayParameter0    = -a1;
					info.RayParameter1    = 0f;
					info.Point0           = ray.Center + info.RayParameter0 * ray.Direction;
					info.Point1           = Vector3D.zero;
					info.Quantity         = 1;
					info.IntersectionType = IntersectionTypes.Point;
				}

				return info.Quantity > 0;
			}
		}
	}
}
