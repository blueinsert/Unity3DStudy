

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line3 and Box3
		/// </summary>
		public struct Line3Box3Intr
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
				ref Vector3D origin, ref Vector3D direction, ref Box3 box, bool solid,
				out int quantity, out Vector3D point0, out Vector3D point1, out IntersectionTypes intrType)
			{
				// Convert linear component to box coordinates.
				Vector3D diff = origin - box.Center;
				Vector3D BOrigin = new Vector3D(
					diff.Dot(box.Axis0),
					diff.Dot(box.Axis1),
					diff.Dot(box.Axis2)
				);
				Vector3D BDirection = new Vector3D(
					direction.Dot(box.Axis0),
					direction.Dot(box.Axis1),
					direction.Dot(box.Axis2)
				);

				double saveT0 = t0, saveT1 = t1;
				bool notAllClipped =
					Clip(+BDirection.x, -BOrigin.x - box.Extents.x, ref t0, ref t1) &&
					Clip(-BDirection.x, +BOrigin.x - box.Extents.x, ref t0, ref t1) &&
					Clip(+BDirection.y, -BOrigin.y - box.Extents.y, ref t0, ref t1) &&
					Clip(-BDirection.y, +BOrigin.y - box.Extents.y, ref t0, ref t1) &&
					Clip(+BDirection.z, -BOrigin.z - box.Extents.z, ref t0, ref t1) &&
					Clip(-BDirection.z, +BOrigin.z - box.Extents.z, ref t0, ref t1);

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
			/// Tests if a line intersects a box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Box3(ref Line3 line, ref Box3 box)
			{
				double AWdU0, AWdU1, AWdU2;
				double AWxDdU0, AWxDdU1, AWxDdU2;
				double RHS;

				Vector3D diff = line.Center - box.Center;
				Vector3D WxD = line.Direction.Cross(diff);

				AWdU1   = System.Math.Abs(line.Direction.Dot(box.Axis1));
				AWdU2   = System.Math.Abs(line.Direction.Dot(box.Axis2));
				AWxDdU0 = System.Math.Abs(WxD.Dot(box.Axis0));
				RHS     = box.Extents.y * AWdU2 + box.Extents.z * AWdU1;
				if (AWxDdU0 > RHS)
				{
					return false;
				}

				AWdU0   = System.Math.Abs(line.Direction.Dot(box.Axis0));
				AWxDdU1 = System.Math.Abs(WxD.Dot(box.Axis1));
				RHS     = box.Extents.x * AWdU2 + box.Extents.z * AWdU0;
				if (AWxDdU1 > RHS)
				{
					return false;
				}

				AWxDdU2 = System.Math.Abs(WxD.Dot(box.Axis2));
				RHS     = box.Extents.x * AWdU1 + box.Extents.y * AWdU0;
				if (AWxDdU2 > RHS)
				{
					return false;
				}

				return true;
			}

			/// <summary>
			/// Tests if a line intersects a box and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Box3(ref Line3 line, ref Box3 box, out Line3Box3Intr info)
			{
				return DoClipping(
					double.NegativeInfinity, double.PositiveInfinity,
					ref line.Center, ref line.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
