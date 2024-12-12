

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a box intersects another box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestBox3Box3(ref Box3 box0, ref Box3 box1)
			{
				// Cutoff for cosine of angles between box axes.  This is used to catch
				// the cases when at least one pair of axes are parallel.  If this
				// happens, there is no need to test for separation along the
				// Cross(A[i],B[j]) directions.
				double cutoff = 1.0f - Mathex.ZeroTolerance;
				bool existsParallelPair = false;

				// Convenience variables.
				Vector3D A0 = box0.Axis0;
				Vector3D A1 = box0.Axis1;
				Vector3D A2 = box0.Axis2;
				Vector3D B0 = box1.Axis0;
				Vector3D B1 = box1.Axis1;
				Vector3D B2 = box1.Axis2;
				
				double EA0 = box0.Extents.x;
				double EA1 = box0.Extents.y;
				double EA2 = box0.Extents.z;
				double EB0 = box1.Extents.x;
				double EB1 = box1.Extents.y;
				double EB2 = box1.Extents.z;

				// Compute difference of box centers, D = C1-C0.
				Vector3D D = box1.Center - box0.Center;

				double C00, C01, C02, C10, C11, C12, C20, C21, C22;								// matrix C = A^T B, c_{ij} = Dot(A_i,B_j)
				double AbsC00, AbsC01, AbsC02, AbsC10, AbsC11, AbsC12, AbsC20, AbsC21, AbsC22;	// |c_{ij}|
				double AD0, AD1, AD2;	// Dot(A_i,D)
				double r0, r1, r;		// interval radii and distance between centers
				double r01;				// = R0 + R1


				// Axes of box0

				// axis C0+t*A0
				C00 = A0.Dot(B0);
				AbsC00 = System.Math.Abs(C00);
				if (AbsC00 > cutoff) existsParallelPair = true;
				C01 = A0.Dot(B1);
				AbsC01 = System.Math.Abs(C01);
				if (AbsC01 > cutoff) existsParallelPair = true;
				C02 = A0.Dot(B2);
				AbsC02 = System.Math.Abs(C02);
				if (AbsC02 > cutoff) existsParallelPair = true;
				AD0 = A0.Dot(D);
				r = System.Math.Abs(AD0);
				r1 = EB0 * AbsC00 + EB1 * AbsC01 + EB2 * AbsC02;
				r01 = EA0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A1
				C10 = A1.Dot(B0);
				AbsC10 = System.Math.Abs(C10);
				if (AbsC10 > cutoff) existsParallelPair = true;
				C11 = A1.Dot(B1);
				AbsC11 = System.Math.Abs(C11);
				if (AbsC11 > cutoff) existsParallelPair = true;
				C12 = A1.Dot(B2);
				AbsC12 = System.Math.Abs(C12);
				if (AbsC12 > cutoff) existsParallelPair = true;
				AD1 = A1.Dot(D);
				r = System.Math.Abs(AD1);
				r1 = EB0 * AbsC10 + EB1 * AbsC11 + EB2 * AbsC12;
				r01 = EA1 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A2
				C20 = A2.Dot(B0);
				AbsC20 = System.Math.Abs(C20);
				if (AbsC20 > cutoff) existsParallelPair = true;
				C21 = A2.Dot(B1);
				AbsC21 = System.Math.Abs(C21);
				if (AbsC21 > cutoff) existsParallelPair = true;
				C22 = A2.Dot(B2);
				AbsC22 = System.Math.Abs(C22);
				if (AbsC22 > cutoff) existsParallelPair = true;
				AD2 = A2.Dot(D);
				r = System.Math.Abs(AD2);
				r1 = EB0 * AbsC20 + EB1 * AbsC21 + EB2 * AbsC22;
				r01 = EA2 + r1;
				if (r > r01)
				{
					return false;
				}


				// Axes of box1

				// axis C0+t*B0
				r = System.Math.Abs(B0.Dot(D));
				r0 = EA0 * AbsC00 + EA1 * AbsC10 + EA2 * AbsC20;
				r01 = r0 + EB0;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*B1
				r = System.Math.Abs(B1.Dot(D));
				r0 = EA0 * AbsC01 + EA1 * AbsC11 + EA2 * AbsC21;
				r01 = r0 + EB1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*B2
				r = System.Math.Abs(B2.Dot(D));
				r0 = EA0 * AbsC02 + EA1 * AbsC12 + EA2 * AbsC22;
				r01 = r0 + EB2;
				if (r > r01)
				{
					return false;
				}


				// Combined axes

				// At least one pair of box axes was parallel, so the separation is
				// effectively in 2D where checking the "edge" normals is sufficient for
				// the separation of the boxes.
				if (existsParallelPair)
				{
					return true;
				}

				// axis C0+t*A0xB0
				r = System.Math.Abs(AD2 * C10 - AD1 * C20);
				r0 = EA1 * AbsC20 + EA2 * AbsC10;
				r1 = EB1 * AbsC02 + EB2 * AbsC01;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A0xB1
				r = System.Math.Abs(AD2 * C11 - AD1 * C21);
				r0 = EA1 * AbsC21 + EA2 * AbsC11;
				r1 = EB0 * AbsC02 + EB2 * AbsC00;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A0xB2
				r = System.Math.Abs(AD2 * C12 - AD1 * C22);
				r0 = EA1 * AbsC22 + EA2 * AbsC12;
				r1 = EB0 * AbsC01 + EB1 * AbsC00;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A1xB0
				r = System.Math.Abs(AD0 * C20 - AD2 * C00);
				r0 = EA0 * AbsC20 + EA2 * AbsC00;
				r1 = EB1 * AbsC12 + EB2 * AbsC11;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A1xB1
				r = System.Math.Abs(AD0 * C21 - AD2 * C01);
				r0 = EA0 * AbsC21 + EA2 * AbsC01;
				r1 = EB0 * AbsC12 + EB2 * AbsC10;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A1xB2
				r = System.Math.Abs(AD0 * C22 - AD2 * C02);
				r0 = EA0 * AbsC22 + EA2 * AbsC02;
				r1 = EB0 * AbsC11 + EB1 * AbsC10;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A2xB0
				r = System.Math.Abs(AD1 * C00 - AD0 * C10);
				r0 = EA0 * AbsC10 + EA1 * AbsC00;
				r1 = EB1 * AbsC22 + EB2 * AbsC21;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A2xB1
				r = System.Math.Abs(AD1 * C01 - AD0 * C11);
				r0 = EA0 * AbsC11 + EA1 * AbsC01;
				r1 = EB0 * AbsC22 + EB2 * AbsC20;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				// axis C0+t*A2xB2
				r = System.Math.Abs(AD1 * C02 - AD0 * C12);
				r0 = EA0 * AbsC12 + EA1 * AbsC02;
				r1 = EB0 * AbsC21 + EB1 * AbsC20;
				r01 = r0 + r1;
				if (r > r01)
				{
					return false;
				}

				return true;
			}
		}
	}
}
