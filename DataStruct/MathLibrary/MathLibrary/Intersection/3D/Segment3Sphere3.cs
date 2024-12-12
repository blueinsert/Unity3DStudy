

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Segment3 and Sphere3
		/// </summary>
		public struct Segment3Sphere3Intr
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
			/// Segment evaluation parameter of the first intersection point
			/// </summary>
			public double SegmentParameter0;

			/// <summary>
			/// Segment evaluation parameter of the second intersection point
			/// </summary>
			public double SegmentParameter1;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a segment intersects a sphere. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestSegment3Sphere3(ref Segment3 segment, ref Sphere3 sphere)
			{
				Vector3D diff = segment.Center - sphere.Center;
				double a0 = diff.Dot(diff) - sphere.Radius * sphere.Radius;
				double a1 = segment.Direction.Dot(diff);
				double discr = a1 * a1 - a0;
				if (discr < 0f)
				{
					return false;
				}

				double tmp0 = segment.Extent * segment.Extent + a0;
				double tmp1 = 2f * a1 * segment.Extent;
				double qm = tmp0 - tmp1;
				double qp = tmp0 + tmp1;
				if (qm * qp <= 0f)
				{
					return true;
				}

				return qm > 0f && System.Math.Abs(a1) < segment.Extent;
			}

			/// <summary>
			/// Tests if a segment intersects a sphere and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindSegment3Sphere3(ref Segment3 segment, ref Sphere3 sphere, out Segment3Sphere3Intr info)
			{
				Vector3D diff = segment.Center - sphere.Center;
				double a0 = diff.Dot(diff) - sphere.Radius * sphere.Radius;
				double a1 = segment.Direction.Dot(diff);
				double discr = a1 * a1 - a0;
				if (discr < 0f)
				{
					info = new Segment3Sphere3Intr();
					return false;
				}

				double tmp0 = segment.Extent * segment.Extent + a0;
				double tmp1 = 2f * a1 * segment.Extent;
				double qm = tmp0 - tmp1;
				double qp = tmp0 + tmp1;
				double root;
				if (qm * qp <= 0f)
				{
					root = System.Math.Sqrt(discr);

					info.SegmentParameter0 = (qm > 0f ? -a1 - root : -a1 + root);
					info.SegmentParameter1 = 0f;
					info.Point0            = segment.Center + info.SegmentParameter0 * segment.Direction;
					info.Point1            = Vector3D.zero;
					info.SegmentParameter0 = (info.SegmentParameter0 + segment.Extent) / (2f * segment.Extent);
					info.Quantity          = 1;
					info.IntersectionType  = IntersectionTypes.Point;
					return true;
				}

				if (qm > 0f && System.Math.Abs(a1) < segment.Extent)
				{
					if (discr >= Mathex.ZeroTolerance)
					{
						root = System.Math.Sqrt(discr);
						
						info.SegmentParameter0 = -a1 - root;
						info.SegmentParameter1 = -a1 + root;
						info.Point0            = segment.Center + info.SegmentParameter0  * segment.Direction;
						info.Point1            = segment.Center + info.SegmentParameter1 * segment.Direction;
						info.SegmentParameter0 = (info.SegmentParameter0  + segment.Extent) / (2f * segment.Extent);
						info.SegmentParameter1 = (info.SegmentParameter1 + segment.Extent) / (2f * segment.Extent);
						info.Quantity          = 2;
						info.IntersectionType  = IntersectionTypes.Segment;
					}
					else
					{
						info.SegmentParameter0 = -a1;
						info.SegmentParameter1 = 0f;
						info.Point0            = segment.Center + info.SegmentParameter0 * segment.Direction;
						info.Point1            = Vector3D.zero;
						info.SegmentParameter0 = (info.SegmentParameter0 + segment.Extent) / (2f * segment.Extent);
						info.Quantity          = 1;
						info.IntersectionType  = IntersectionTypes.Point;
					}
				}
				else
				{
					info = new Segment3Sphere3Intr();
				}

				return info.Quantity > 0;
			}
		}
	}
}
