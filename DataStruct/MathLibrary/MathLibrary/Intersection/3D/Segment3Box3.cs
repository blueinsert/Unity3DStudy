

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment3 and Box3
		/// </summary>
		public struct Segment3Box3Intr
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
			/// Tests if a segment intersects a box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment3Box3(ref Segment3 segment, ref Box3 box)
			{
				double RHS;
				Vector3D diff = segment.Center - box.Center;

				double AWdU0 = System.Math.Abs(segment.Direction.Dot(box.Axis0));
				double ADdU0 = System.Math.Abs(diff.Dot(box.Axis0));
				RHS = box.Extents.x + segment.Extent * AWdU0;
				if (ADdU0 > RHS)
				{
					return false;
				}

				double AWdU1 = System.Math.Abs(segment.Direction.Dot(box.Axis1));
				double ADdU1 = System.Math.Abs(diff.Dot(box.Axis1));
				RHS = box.Extents.y + segment.Extent * AWdU1;
				if (ADdU1 > RHS)
				{
					return false;
				}

				double AWdU2 = System.Math.Abs(segment.Direction.Dot(box.Axis2));
				double ADdU2 = System.Math.Abs(diff.Dot(box.Axis2));
				RHS = box.Extents.z + segment.Extent * AWdU2;
				if (ADdU2 > RHS)
				{
					return false;
				}

				Vector3D WxD = segment.Direction.Cross(diff);

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
			/// Tests if a segment intersects a box and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment3Box3(ref Segment3 segment, ref Box3 box, out Segment3Box3Intr info)
			{
				return DoClipping(
					-segment.Extent, segment.Extent,
					ref segment.Center, ref segment.Direction, ref box, true,
					out info.Quantity, out info.Point0, out info.Point1, out info.IntersectionType);
			}
		}
	}
}
