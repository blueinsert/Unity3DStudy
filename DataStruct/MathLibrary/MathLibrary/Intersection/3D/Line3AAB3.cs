

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line3 and AxisAlignedBox3
		/// </summary>
		public struct Line3AAB3Intr
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
			private static bool DoClipping(
				double t0, double t1,
				ref Vector3D origin, ref Vector3D direction, ref AAB3 box, bool solid,
				out int quantity, out Vector3D point0, out Vector3D point1, out IntersectionTypes intrType)
			{
				Vector3D boxCenter;
				Vector3D boxExtents;
				box.CalcCenterExtents(out boxCenter, out boxExtents);

				// Convert linear component to box coordinates.
				Vector3D BOrigin = new Vector3D(origin.x - boxCenter.x, origin.y - boxCenter.y, origin.z - boxCenter.z);

				double saveT0 = t0, saveT1 = t1;
				bool notAllClipped =
					Clip(+direction.x, -BOrigin.x - boxExtents.x, ref t0, ref t1) &&
					Clip(-direction.x, +BOrigin.x - boxExtents.x, ref t0, ref t1) &&
					Clip(+direction.y, -BOrigin.y - boxExtents.y, ref t0, ref t1) &&
					Clip(-direction.y, +BOrigin.y - boxExtents.y, ref t0, ref t1) &&
					Clip(+direction.z, -BOrigin.z - boxExtents.z, ref t0, ref t1) &&
					Clip(-direction.z, +BOrigin.z - boxExtents.z, ref t0, ref t1);

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
						point1   = Vector3Dex.Zero;
					}
				}
				else
				{
					intrType = IntersectionTypes.Empty;
					quantity = 0;
					point0   = Vector3Dex.Zero;
					point1   = Vector3Dex.Zero;
				}

				return intrType != IntersectionTypes.Empty;
			}

			/// <summary>
			/// Tests if a line intersects an axis aligned box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3AAB3(ref Line3 line, ref AAB3 box)
			{
				Vector3D boxCenter;
				Vector3D boxExtents;
				box.CalcCenterExtents(out boxCenter, out boxExtents);

				double RHS;
				Vector3D diff = line.Center - boxCenter;
				Vector3D WxD = line.Direction.Cross(diff);

				double AWdU1 = System.Math.Abs(line.Direction.y);
				double AWdU2 = System.Math.Abs(line.Direction.z);
				double AWxDdU0 = System.Math.Abs(WxD.x);
				RHS = boxExtents.y * AWdU2 + boxExtents.z * AWdU1;
				if (AWxDdU0 > RHS)
				{
					return false;
				}

				double AWdU0 = System.Math.Abs(line.Direction.x);
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
			/// Tests if a line intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3AAB3(ref Line3 line, ref AAB3 box, out Line3AAB3Intr info)
			{
				return DoClipping(
					double.NegativeInfinity, double.PositiveInfinity,
					ref line.Center, ref line.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
