

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line2 and AxisAlignedBox2
		/// </summary>
		public struct Line2AAB2Intr
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
			private static bool DoClipping(
				double t0, double t1,
				ref Vector2D origin, ref Vector2D direction, ref AAB2 box, bool solid,
				out int quantity, out Vector2D point0, out Vector2D point1, out IntersectionTypes intrType)
			{
				Vector2D boxCenter;
				Vector2D boxExtents;
				box.CalcCenterExtents(out boxCenter, out boxExtents);

				// Convert linear component to box coordinates.
				Vector2D BOrigin = new Vector2D(origin.x - boxCenter.x, origin.y - boxCenter.y);

				double saveT0 = t0, saveT1 = t1;
				bool notAllClipped =
					Clip(+direction.x, -BOrigin.x - boxExtents.x, ref t0, ref t1) &&
					Clip(-direction.x, +BOrigin.x - boxExtents.x, ref t0, ref t1) &&
					Clip(+direction.y, -BOrigin.y - boxExtents.y, ref t0, ref t1) &&
					Clip(-direction.y, +BOrigin.y - boxExtents.y, ref t0, ref t1);

				if (notAllClipped && (solid || t0 != saveT0 || t1 != saveT1))
				{
					if (t1 > t0)
					{
						intrType = IntersectionTypes.Segment;
						quantity = 2;
						point0 = origin + t0 * direction;
						point1 = origin + t1 * direction;
					}
					else
					{
						intrType = IntersectionTypes.Point;
						quantity = 1;
						point0 = origin + t0 * direction;
						point1 = Vector2Dex.Zero;
					}
				}
				else
				{
					intrType = IntersectionTypes.Empty;
					quantity = 0;
					point0 = Vector2Dex.Zero;
					point1 = Vector2Dex.Zero;
				}

				return intrType != IntersectionTypes.Empty;
			}


			/// <summary>
			/// Tests if a line intersects an axis aligned box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine2AAB2(ref Line2 line, ref AAB2 box)
			{
				Vector2D normal = line.Direction.Perp();

				Vector2D dMin, dMax;

				if (normal.x >= 0.0f)
				{
					dMin.x = box.Min.x;
					dMax.x = box.Max.x;
				}
				else
				{
					dMin.x = box.Max.x;
					dMax.x = box.Min.x;
				}

				if (normal.y >= 0.0f)
				{
					dMin.y = box.Min.y;
					dMax.y = box.Max.y;
				}
				else
				{
					dMin.y = box.Max.y;
					dMax.y = box.Min.y;
				}

				// Check if minimal point on diagonal is on positive side of line
				double distMin = normal.Dot(dMin - line.Center);
				if (distMin >= 0.0f)
				{
					return false;
				}
				else
				{
					// If minimal point is on negative side, then intersection occurs only if maximal point is on positive side
					double distMax = normal.Dot(dMax - line.Center);
					return distMax > 0.0f;
				}
			}

			/// <summary>
			/// Tests if a line intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine2AAB2(ref Line2 line, ref AAB2 box, out Line2AAB2Intr info)
			{
				return DoClipping(
					double.NegativeInfinity, double.PositiveInfinity,
					ref line.Center, ref line.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
