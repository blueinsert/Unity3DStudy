

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment3 and AxisAlignedBox3
		/// </summary>
		public struct Segment3AAB3Intr
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
			/// Tests if a segment intersects an axis aligned box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment3AAB3(ref Segment3 segment, ref AAB3 box)
			{
				Vector3D boxCenter;
				Vector3D boxExtents;
				box.CalcCenterExtents(out boxCenter, out boxExtents);

				double RHS;
				Vector3D diff = segment.Center - boxCenter;

				double AWdU0 = System.Math.Abs(segment.Direction.x);
				double ADdU0 = System.Math.Abs(diff.x);
				RHS = boxExtents.x + segment.Extent * AWdU0;
				if (ADdU0 > RHS)
				{
					return false;
				}

				double AWdU1 = System.Math.Abs(segment.Direction.y);
				double ADdU1 = System.Math.Abs(diff.y);
				RHS = boxExtents.y + segment.Extent * AWdU1;
				if (ADdU1 > RHS)
				{
					return false;
				}

				double AWdU2 = System.Math.Abs(segment.Direction.z);
				double ADdU2 = System.Math.Abs(diff.z);
				RHS = boxExtents.z + segment.Extent * AWdU2;
				if (ADdU2 > RHS)
				{
					return false;
				}

				Vector3D WxD = segment.Direction.Cross(diff);

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
			/// Tests if a segment intersects an axis aligned box and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment3AAB3(ref Segment3 segment, ref AAB3 box, out Segment3AAB3Intr info)
			{
				return DoClipping(
					-segment.Extent, segment.Extent,
					ref segment.Center, ref segment.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
