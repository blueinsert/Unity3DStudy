

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray3 and Box3
		/// </summary>
		public struct Ray3Box3Intr
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
			/// Tests if a ray intersects a box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay3Box3(ref Ray3 ray, ref Box3 box)
			{
				double RHS;
				Vector3D diff = ray.Center - box.Center;

				double WdU0 = ray.Direction.Dot(box.Axis0);
				double AWdU0 = System.Math.Abs(WdU0);
				double DdU0 = diff.Dot(box.Axis0);
				double ADdU0 = System.Math.Abs(DdU0);
				if (ADdU0 > box.Extents.x && DdU0 * WdU0 >= 0.0f)
				{
					return false;
				}

				double WdU1 = ray.Direction.Dot(box.Axis1);
				double AWdU1 = System.Math.Abs(WdU1);
				double DdU1 = diff.Dot(box.Axis1);
				double ADdU1 = System.Math.Abs(DdU1);
				if (ADdU1 > box.Extents.y && DdU1 * WdU1 >= 0.0f)
				{
					return false;
				}

				double WdU2 = ray.Direction.Dot(box.Axis2);
				double AWdU2 = System.Math.Abs(WdU2);
				double DdU2 = diff.Dot(box.Axis2);
				double ADdU2 = System.Math.Abs(DdU2);
				if (ADdU2 > box.Extents.z && DdU2 * WdU2 >= 0.0f)
				{
					return false;
				}

				Vector3D WxD = ray.Direction.Cross(diff);

				double AWxDdU0 = System.Math.Abs(WxD.Dot(box.Axis0));
				RHS = box.Extents.y * AWdU2 + box.Extents.z * AWdU1;
				if (AWxDdU0 > RHS)
				{
					return false;
				}

				double AWxDdU1 = System.Math.Abs(WxD.Dot(box.Axis1));
				RHS = box.Extents.x * AWdU2 + box.Extents.z * AWdU0;
				if (AWxDdU1 > RHS)
				{
					return false;
				}

				double AWxDdU2 = System.Math.Abs(WxD.Dot(box.Axis2));
				RHS = box.Extents.x * AWdU1 + box.Extents.y * AWdU0;
				if (AWxDdU2 > RHS)
				{
					return false;
				}

				return true;
			}

			/// <summary>
			/// Tests if a ray intersects a box and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay3Box3(ref Ray3 ray, ref Box3 box, out Ray3Box3Intr info)
			{
				return DoClipping(
					0.0f, double.PositiveInfinity,
					ref ray.Center, ref ray.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
