

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray3 and AxisAlignedBox3
		/// </summary>
		public struct Ray3AAB3Intr
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
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a ray intersects an axis aligned box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay3AAB3(ref Ray3 ray, ref AAB3 box)
			{
				Vector3D boxCenter;
				Vector3D boxExtents;
				box.CalcCenterExtents(out boxCenter, out boxExtents);

				double RHS;
				Vector3D diff = ray.Center - boxCenter;

				double WdU0 = ray.Direction.x;
				double AWdU0 = System.Math.Abs(WdU0);
				double DdU0 = diff.x;
				double ADdU0 = System.Math.Abs(DdU0);
				if (ADdU0 > boxExtents.x && DdU0 * WdU0 >= 0.0f)
				{
					return false;
				}

				double WdU1 = ray.Direction.y;
				double AWdU1 = System.Math.Abs(WdU1);
				double DdU1 = diff.y;
				double ADdU1 = System.Math.Abs(DdU1);
				if (ADdU1 > boxExtents.y && DdU1 * WdU1 >= 0.0f)
				{
					return false;
				}

				double WdU2 = ray.Direction.z;
				double AWdU2 = System.Math.Abs(WdU2);
				double DdU2 = diff.z;
				double ADdU2 = System.Math.Abs(DdU2);
				if (ADdU2 > boxExtents.z && DdU2 * WdU2 >= 0.0f)
				{
					return false;
				}

				Vector3D WxD = ray.Direction.Cross(diff);

				double AWxDdU0 = System.Math.Abs(WxD.x);
				RHS = boxExtents.y * AWdU2 + boxExtents.z * AWdU1;
				if (AWxDdU0 > RHS)
				{
					return false;
				}

				double AWxDdU1 = System.Math.Abs(WxD.y);
				RHS = boxExtents.x * AWdU2 + boxExtents.z * AWdU0;
				if (AWxDdU1 > RHS)
				{
					return false;
				}

				double AWxDdU2 = System.Math.Abs(WxD.z);
				RHS = boxExtents.x * AWdU1 + boxExtents.y * AWdU0;
				if (AWxDdU2 > RHS)
				{
					return false;
				}

				return true;
			}

			/// <summary>
			/// Tests if a ray intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay3AAB3(ref Ray3 ray, ref AAB3 box, out Ray3AAB3Intr info)
			{
				return DoClipping(
					0.0f, double.PositiveInfinity,
					ref ray.Center, ref ray.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
