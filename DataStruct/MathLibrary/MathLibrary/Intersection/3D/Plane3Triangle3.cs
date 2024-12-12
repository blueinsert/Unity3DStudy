

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Plane3 and Triangle3
		/// </summary>
		public struct Plane3Triangle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point (a triangle is touching a plane by a vertex) or
			/// IntersectionTypes.Segment (a triangle is touching a plane by an edge or intersecting the plane) or
			/// IntersectionTypes.Polygon (a triangle is lying in a plane), otherwise IntersectionTypes.Empty (no intersection).
			/// </summary>
			public IntersectionTypes IntersectionType;
			
			/// <summary>
			/// Number of intersection points.
			/// 0 - IntersectionTypes.Empty;
			/// 1 - IntersectionTypes.Point;
			/// 2 - IntersectionTypes.Segment;
			/// 3 - IntersectionTypes.Polygon;
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
			/// Third intersection point
			/// </summary>
			public Vector3D Point2;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a plane intersects a triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestPlane3Triangle3(ref Plane3 plane, ref Triangle3 triangle)
			{
				// Compute the signed distances from the vertices to the plane.
				double zero = 0f;

				double SD0 = plane.SignedDistanceTo(ref triangle.V0);
				if (System.Math.Abs(SD0) <= _distanceThreshold)
				{
					SD0 = zero;
				}

				double SD1 = plane.SignedDistanceTo(ref triangle.V1);
				if (System.Math.Abs(SD1) <= _distanceThreshold)
				{
					SD1 = zero;
				}

				double SD2 = plane.SignedDistanceTo(ref triangle.V2);
				if (System.Math.Abs(SD2) <= _distanceThreshold)
				{
					SD2 = zero;
				}

				// The triangle intersects the plane if not all vertices are on the
				// positive side of the plane and not all vertices are on the negative
				// side of the plane.
				return !(SD0 > zero && SD1 > zero && SD2 > zero) &&
					   !(SD0 < zero && SD1 < zero && SD2 < zero);
			}

			/// <summary>
			/// Tests if a plane intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindPlane3Triangle3(ref Plane3 plane, ref Triangle3 triangle, out Plane3Triangle3Intr info)
			{
				// Compute the signed distances from the vertices to the plane.
				double zero = 0f;

				double SD0 = plane.SignedDistanceTo(ref triangle.V0);
				if (System.Math.Abs(SD0) <= _distanceThreshold)
				{
					SD0 = zero;
				}

				double SD1 = plane.SignedDistanceTo(ref triangle.V1);
				if (System.Math.Abs(SD1) <= _distanceThreshold)
				{
					SD1 = zero;
				}

				double SD2 = plane.SignedDistanceTo(ref triangle.V2);
				if (System.Math.Abs(SD2) <= _distanceThreshold)
				{
					SD2 = zero;
				}

				Vector3D V0 = triangle.V0;
				Vector3D V1 = triangle.V1;
				Vector3D V2 = triangle.V2;

				info.Point0 = info.Point1 = info.Point2 = Vector3D.zero;

				if (SD0 > zero)
				{
					if (SD1 > zero)
					{
						if (SD2 > zero)
						{
							// ppp
							info.IntersectionType = IntersectionTypes.Empty;
							info.Quantity = 0;
						}
						else if (SD2 < zero)
						{
							// ppm
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD2)) * (V2 - V0);
							info.Point1 = V1 + (SD1 / (SD1 - SD2)) * (V2 - V1);
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else
						{
							// ppz
							info.Quantity = 1;
							info.Point0 = V2;
							info.IntersectionType = IntersectionTypes.Point;
						}
					}
					else if (SD1 < zero)
					{
						if (SD2 > zero)
						{
							// pmp
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD1)) * (V1 - V0);
							info.Point1 = V1 + (SD1 / (SD1 - SD2)) * (V2 - V1);
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else if (SD2 < zero)
						{
							// pmm
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD1)) * (V1 - V0);
							info.Point1 = V0 + (SD0 / (SD0 - SD2)) * (V2 - V0);
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else
						{
							// pmz
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD1)) * (V1 - V0);
							info.Point1 = V2;
							info.IntersectionType = IntersectionTypes.Segment;
						}
					}
					else
					{
						if (SD2 > zero)
						{
							// pzp
							info.Quantity = 1;
							info.Point0 = V1;
							info.IntersectionType = IntersectionTypes.Point;
						}
						else if (SD2 < zero)
						{
							// pzm
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD2)) * (V2 - V0);
							info.Point1 = V1;
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else
						{
							// pzz
							info.Quantity = 2;
							info.Point0 = V1;
							info.Point1 = V2;
							info.IntersectionType = IntersectionTypes.Segment;
						}
					}
				}
				else if (SD0 < zero)
				{
					if (SD1 > zero)
					{
						if (SD2 > zero)
						{
							// mpp
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD1)) * (V1 - V0);
							info.Point1 = V0 + (SD0 / (SD0 - SD2)) * (V2 - V0);
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else if (SD2 < zero)
						{
							// mpm
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD1)) * (V1 - V0);
							info.Point1 = V1 + (SD1 / (SD1 - SD2)) * (V2 - V1);
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else
						{
							// mpz
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD1)) * (V1 - V0);
							info.Point1 = V2;
							info.IntersectionType = IntersectionTypes.Segment;
						}
					}
					else if (SD1 < zero)
					{
						if (SD2 > zero)
						{
							// mmp
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD2)) * (V2 - V0);
							info.Point1 = V1 + (SD1 / (SD1 - SD2)) * (V2 - V1);
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else if (SD2 < zero)
						{
							// mmm
							info.Quantity = 0;
							info.IntersectionType = IntersectionTypes.Empty;
						}
						else
						{
							// mmz
							info.Quantity = 1;
							info.Point0 = V2;
							info.IntersectionType = IntersectionTypes.Point;
						}
					}
					else
					{
						if (SD2 > zero)
						{
							// mzp
							info.Quantity = 2;
							info.Point0 = V0 + (SD0 / (SD0 - SD2)) * (V2 - V0);
							info.Point1 = V1;
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else if (SD2 < zero)
						{
							// mzm
							info.Quantity = 1;
							info.Point0 = V1;
							info.IntersectionType = IntersectionTypes.Point;
						}
						else
						{
							// mzz
							info.Quantity = 2;
							info.Point0 = V1;
							info.Point1 = V2;
							info.IntersectionType = IntersectionTypes.Segment;
						}
					}
				}
				else
				{
					if (SD1 > zero)
					{
						if (SD2 > zero)
						{
							// zpp
							info.Quantity = 1;
							info.Point0 = V0;
							info.IntersectionType = IntersectionTypes.Point;
						}
						else if (SD2 < zero)
						{
							// zpm
							info.Quantity = 2;
							info.Point0 = V1 + (SD1 / (SD1 - SD2)) * (V2 - V1);
							info.Point1 = V0;
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else
						{
							// zpz
							info.Quantity = 2;
							info.Point0 = V0;
							info.Point1 = V2;
							info.IntersectionType = IntersectionTypes.Segment;
						}
					}
					else if (SD1 < zero)
					{
						if (SD2 > zero)
						{
							// zmp
							info.Quantity = 2;
							info.Point0 = V1 + (SD1 / (SD1 - SD2)) * (V2 - V1);
							info.Point1 = V0;
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else if (SD2 < zero)
						{
							// zmm
							info.Quantity = 1;
							info.Point0 = V0;
							info.IntersectionType = IntersectionTypes.Point;
						}
						else
						{
							// zmz
							info.Quantity = 2;
							info.Point0 = V0;
							info.Point1 = V2;
							info.IntersectionType = IntersectionTypes.Segment;
						}
					}
					else
					{
						if (SD2 > zero)
						{
							// zzp
							info.Quantity = 2;
							info.Point0 = V0;
							info.Point1 = V1;
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else if (SD2 < zero)
						{
							// zzm
							info.Quantity = 2;
							info.Point0 = V0;
							info.Point1 = V1;
							info.IntersectionType = IntersectionTypes.Segment;
						}
						else
						{
							// zzz
							info.Quantity = 3;
							info.Point0 = V0;
							info.Point1 = V1;
							info.Point2 = V2;
							info.IntersectionType = IntersectionTypes.Polygon;
						}
					}
				}

				return info.IntersectionType != IntersectionTypes.Empty;
			}
		}
	}
}
