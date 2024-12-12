

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Plane3 and Plane3
		/// </summary>
		public struct Plane3Plane3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Line or IntersectionTypes.Plane (planes are the same) if intersection occured otherwise IntersectionTypes.Empty.
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Intersection line (in case of IntersectionTypes.Line)
			/// </summary>
			public Line3 Line;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a plane intersects another plane. Returns true if intersection occurs false otherwise (also returns false when planes are the same)
			/// </summary>
			public static bool TestPlane3Plane3(ref Plane3 plane0, ref Plane3 plane1)
			{
				// If Cross(N0,N1) is zero, then either planes are parallel and separated
				// or the same plane.  In both cases, 'false' is returned.  Otherwise, the
				// planes intersect.  To avoid subtle differences in reporting between
				// Test() and Find(), the same parallel test is used.  Mathematically,
				//   |Cross(N0,N1)|^2 = Dot(N0,N0)*Dot(N1,N1)-Dot(N0,N1)^2
				//                    = 1 - Dot(N0,N1)^2
				// The last equality is true since planes are required to have unit-length
				// normal vectors.  The test |Cross(N0,N1)| = 0 is the same as
				// |Dot(N0,N1)| = 1.  I test the latter condition in Test() and Find().

				double dot = plane0.Normal.Dot(plane1.Normal);
				return System.Math.Abs(dot) < 1f - Mathex.ZeroTolerance;
			}

			/// <summary>
			/// Tests if a plane intersects another plane and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindPlane3Plane3(ref Plane3 plane0, ref Plane3 plane1, out Plane3Plane3Intr info)
			{
				// If N0 and N1 are parallel, either the planes are parallel and separated
				// or the same plane.  In both cases, 'false' is returned.  Otherwise,
				// the intersection line is
				//   L(t) = t*Cross(N0,N1)/|Cross(N0,N1)| + c0*N0 + c1*N1
				// for some coefficients c0 and c1 and for t any real number (the line
				// parameter).  Taking dot products with the normals,
				//   d0 = Dot(N0,L) = c0*Dot(N0,N0) + c1*Dot(N0,N1) = c0 + c1*d
				//   d1 = Dot(N1,L) = c0*Dot(N0,N1) + c1*Dot(N1,N1) = c0*d + c1
				// where d = Dot(N0,N1).  These are two equations in two unknowns.  The
				// solution is
				//   c0 = (d0 - d*d1)/det
				//   c1 = (d1 - d*d0)/det
				// where det = 1 - d^2.

				double dot = plane0.Normal.Dot(plane1.Normal);
				if (System.Math.Abs(dot) >= 1f - Mathex.ZeroTolerance)
				{
					// The planes are parallel.  Check if they are coplanar.
					double cDiff;
					if (dot >= 0f)
					{
						// Normals are in same direction, need to look at c0-c1.
						cDiff = plane0.Constant - plane1.Constant;
					}
					else
					{
						// Normals are in opposite directions, need to look at c0+c1.
						cDiff = plane0.Constant + plane1.Constant;
					}

					if (System.Math.Abs(cDiff) < Mathex.ZeroTolerance)
					{
						// Planes are coplanar.
						info.IntersectionType = IntersectionTypes.Plane;
						info.Line = new Line3();
						return true;
					}

					// Planes are parallel, but distinct.
					info.IntersectionType = IntersectionTypes.Empty;
					info.Line = new Line3();
					return false;
				}

				double invDet = 1f / (1f - dot * dot);
				double c0 = (plane0.Constant - dot * plane1.Constant) * invDet;
				double c1 = (plane1.Constant - dot * plane0.Constant) * invDet;

				info.IntersectionType = IntersectionTypes.Line;
				info.Line.Center      = c0 * plane0.Normal + c1 * plane1.Normal;
				info.Line.Direction   = plane0.Normal.UnitCross(plane1.Normal);

				return true;
			}
		}
	}
}
