

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray2 and Box2
		/// </summary>
		public struct Ray2Box2Intr
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
			public Vector2D Point0;

			/// <summary>
			/// Second intersection point
			/// </summary>
			public Vector2D Point1;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests whether ray and box intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay2Box2(ref Ray2 ray, ref Box2 box)
			{
				Vector2D diff = ray.Center - box.Center;

				double WdU0  = ray.Direction.Dot(box.Axis0);
				double DdU0  = diff.Dot(box.Axis0);
				double ADdU0 = System.Math.Abs(DdU0);
				if (ADdU0 > box.Extents.x && DdU0 * WdU0 >= 0f)
				{
					return false;
				}

				double WdU1  = ray.Direction.Dot(box.Axis1);
				double DdU1  = diff.Dot(box.Axis1);
				double ADdU1 = System.Math.Abs(DdU1);
				if (ADdU1 > box.Extents.y && DdU1 * WdU1 >= 0f)
				{
					return false;
				}

				Vector2D perp = ray.Direction.Perp();

				double LHS   = System.Math.Abs(perp.Dot(diff));
				double part0 = System.Math.Abs(perp.Dot(box.Axis0));
				double part1 = System.Math.Abs(perp.Dot(box.Axis1));
				double RHS   = box.Extents.x * part0 + box.Extents.y * part1;

				return LHS <= RHS;
			}

			/// <summary>
			/// Tests whether ray and box intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay2Box2(ref Ray2 ray, ref Box2 box, out Ray2Box2Intr info)
			{
				return DoClipping(
					0.0f, double.PositiveInfinity,
					ref ray.Center, ref ray.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
