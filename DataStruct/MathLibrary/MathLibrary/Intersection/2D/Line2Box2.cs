

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line2 and Box2
		/// </summary>
		public struct Line2Box2Intr
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
			private static bool Clip(double denom, double numer, ref double t0, ref double t1)
			{
				// Return value is 'true' if line segment intersects the current test
				// plane.  Otherwise 'false' is returned in which case the line segment
				// is entirely clipped.

				if (denom > 0f)
				{
					if (numer > denom * t1)
					{
						return false;
					}
					if (numer > denom * t0)
					{
						t0 = numer / denom;
					}
					return true;
				}
				else if (denom < 0f)
				{
					if (numer > denom * t0)
					{
						return false;
					}
					if (numer > denom * t1)
					{
						t1 = numer / denom;
					}
					return true;
				}
				else
				{
					return numer <= 0f;
				}
			}

			private static bool DoClipping(
				double t0, double t1,
				ref Vector2D origin, ref Vector2D direction, ref Box2 box, bool solid,
				out int quantity, out Vector2D point0, out Vector2D point1, out IntersectionTypes intrType)
			{
				// Convert linear component to box coordinates.
				Vector2D diff = new Vector2D(origin.x - box.Center.x, origin.y - box.Center.y);
				Vector2D BOrigin = new Vector2D(diff.Dot(box.Axis0), diff.Dot(box.Axis1));
				Vector2D BDirection = new Vector2D(direction.Dot(box.Axis0), direction.Dot(box.Axis1));

				double saveT0 = t0, saveT1 = t1;
				bool notAllClipped =
					Clip(+BDirection.x, -BOrigin.x - box.Extents.x, ref t0, ref t1) &&
					Clip(-BDirection.x, +BOrigin.x - box.Extents.x, ref t0, ref t1) &&
					Clip(+BDirection.y, -BOrigin.y - box.Extents.y, ref t0, ref t1) &&
					Clip(-BDirection.y, +BOrigin.y - box.Extents.y, ref t0, ref t1);

				if (notAllClipped && (solid || t0 != saveT0 || t1 != saveT1))
				{
					if (t1 > t0)
					{
						intrType = IntersectionTypes.Segment;
						quantity = 2;
						point0   = origin + t0 * direction;
						point1   = origin + t1 * direction;
					}
					else
					{
						intrType = IntersectionTypes.Point;
						quantity = 1;
						point0   = origin + t0 * direction;
						point1   = Vector2Dex.Zero;
					}
				}
				else
				{
					intrType = IntersectionTypes.Empty;
					quantity = 0;
					point0   = Vector2Dex.Zero;
					point1   = Vector2Dex.Zero;
				}

				return intrType != IntersectionTypes.Empty;
			}
						

			/// <summary>
			/// Tests whether line and box intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine2Box2(ref Line2 line, ref Box2 box)
			{
				Vector2D diff = line.Center - box.Center;
				Vector2D perp = line.Direction.Perp();

				double LHS   = System.Math.Abs(perp.Dot(diff));
				double part0 = System.Math.Abs(perp.Dot(box.Axis0));
				double part1 = System.Math.Abs(perp.Dot(box.Axis1));
				double RHS   = box.Extents.x * part0 + box.Extents.y * part1;

				return LHS <= RHS;
			}

			/// <summary>
			/// Tests whether line and box intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine2Box2(ref Line2 line, ref Box2 box, out Line2Box2Intr info)
			{
				return DoClipping(
					double.NegativeInfinity, double.PositiveInfinity,
					ref line.Center, ref line.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
