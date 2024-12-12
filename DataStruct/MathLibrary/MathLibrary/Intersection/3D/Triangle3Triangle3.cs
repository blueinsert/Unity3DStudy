

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Triangle3 and Triangle3
		/// </summary>
		public struct Triangle3Triangle3Intr
		{
			/// <summary>
			/// Gets intersection point by index (0 to 5). Points could be also accessed individually using Point0,...,Point5 fields.
			/// </summary>
			public Vector3D this[int i]
			{
				get
				{
					switch (i)
					{
						case 0: return Point0;
						case 1: return Point1;
						case 2: return Point2;
						case 3: return Point3;
						case 4: return Point4;
						case 5: return Point5;
						default: return Vector3D.zero;
					}
				}
				internal set
				{
					switch (i)
					{
						case 0: Point0 = value; break;
						case 1: Point1 = value; break;
						case 2: Point2 = value; break;
						case 3: Point3 = value; break;
						case 4: Point4 = value; break;
						case 5: Point5 = value; break;
					}
				}
			}

			/// <summary>
			/// Equals to:
			/// IntersectionTypes.Empty if no intersection occurs;
			/// IntersectionTypes.Point if non-coplanar triangles touch in a point, see Touching member for the description;
			/// IntersectionTypes.Segment if non-coplanar triangles intersect or are touch in a segment, see Touching member for the description;
			/// IntersectionTypes.Plane if reportCoplanarIntersections is specified to true when calling Find method and triangles are coplanar
			/// and intersect, if reportCoplanarIntersections is specified to false, coplanar triangles are reported as not intersecting.
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// If triangles are non-coplanar equals to IntersectionType.Empty. If triangles are coplanar, equals to the following options:
			/// IntersectionTypes.Empty if coplanar triangles do not intersect;
			/// IntersectionTypes.Point is coplanar triangles touch in a point;
			/// IntersectionTypes.Segment if coplanar triangles touch in a segment;
			/// IntersectionTypes.Polygon if coplanar triangles intersect.
			/// </summary>
			public IntersectionTypes CoplanarIntersectionType;

			/// <summary>
			/// Equals to true if triangles are non-coplanar and touching in a point 
			/// (IntersectionTypes.Point; touch variants are: a vertex lies in the plane of a triangle and contained by a triangle (including border), two non-collinear edges touch) or
			/// if triangles are not coplanar and touching in a segment
			/// (IntersectionTypes.Segment; an edge lies in the plane of a triangle and intersects triangle in more than one point).
			/// Generally speaking, touching is true when non-coplanar triangles touch each other by some parts of their borders.
			/// Otherwise false.
			/// </summary>
			public bool Touching;			

			/// <summary>
			/// Number of intersection points.
			/// IntersectionTypes.Empty: 0;
			/// IntersectionTypes.Point: 1;
			/// IntersectionTypes.Segment: 2;
			/// IntersectionTypes.Polygon: 1 to 6.
			/// </summary>
			public int Quantity;

			public Vector3D Point0;
			public Vector3D Point1;
			public Vector3D Point2;
			public Vector3D Point3;
			public Vector3D Point4;
			public Vector3D Point5;
		}

		public static partial class Intersection
		{
			private static void ProjectOntoAxis(ref Triangle3 triangle, ref Vector3D axis, out double fmin, out double fmax)
			{
				double dot0 = axis.Dot(triangle.V0);
				double dot1 = axis.Dot(triangle.V1);
				double dot2 = axis.Dot(triangle.V2);

				fmin = dot0;
				fmax = fmin;

				if (dot1 < fmin)
				{
					fmin = dot1;
				}
				else if (dot1 > fmax)
				{
					fmax = dot1;
				}

				if (dot2 < fmin)
				{
					fmin = dot2;
				}
				else if (dot2 > fmax)
				{
					fmax = dot2;
				}
			}

			private static void TrianglePlaneRelations(ref Triangle3 triangle, ref Plane3 plane,
				out double dist0   , out double dist1   , out double dist2,
				out int   sign0   , out int   sign1   , out int   sign2,
				out int   positive, out int   negative, out int   zero)
			{
				// Compute the signed distances of triangle vertices to the plane.
				// Use an epsilon-thick plane test.
				
				positive = 0;
				negative = 0;
				zero     = 0;

				dist0 = plane.SignedDistanceTo(ref triangle.V0);
				if (dist0 > Mathex.ZeroTolerance)
				{
					sign0 = 1;
					++positive;
				}
				else if (dist0 < -Mathex.ZeroTolerance)
				{
					sign0 = -1;
					++negative;
				}
				else
				{
					dist0 = 0.0f;
					sign0 = 0;
					++zero;
				}

				dist1 = plane.SignedDistanceTo(ref triangle.V1);
				if (dist1 > Mathex.ZeroTolerance)
				{
					sign1 = 1;
					++positive;
				}
				else if (dist1 < -Mathex.ZeroTolerance)
				{
					sign1 = -1;
					++negative;
				}
				else
				{
					dist1 = 0.0f;
					sign1 = 0;
					++zero;
				}

				dist2 = plane.SignedDistanceTo(ref triangle.V2);
				if (dist2 > Mathex.ZeroTolerance)
				{
					sign2 = 1;
					++positive;
				}
				else if (dist2 < -Mathex.ZeroTolerance)
				{
					sign2 = -1;
					++negative;
				}
				else
				{
					dist2 = 0.0f;
					sign2 = 0;
					++zero;
				}
			}

			private static bool TrianglePlaneRelationsQuick(ref Triangle3 triangle, ref Plane3 plane)
			{
				// Returns true if triangle vertices strictly lies on one side of the plane

				int sign;
				double dist;
					
				dist = plane.SignedDistanceTo(ref triangle.V0);
				if (dist > Mathex.ZeroTolerance)
				{
					sign = 1;
				}
				else if (dist < -Mathex.ZeroTolerance)
				{
					sign = -1;
				}
				else
				{
					return false;
				}

				dist = plane.SignedDistanceTo(ref triangle.V1);
				if (dist > Mathex.ZeroTolerance)
				{
					if (sign == -1) return false;
				}
				else if (dist < -Mathex.ZeroTolerance)
				{
					if (sign == 1) return false;
				}
				else
				{
					return false;
				}

				dist = plane.SignedDistanceTo(ref triangle.V2);
				if (dist > Mathex.ZeroTolerance)
				{
					if (sign == -1) return false;
				}
				else if (dist < -Mathex.ZeroTolerance)
				{
					if (sign == 1) return false;
				}
				else
				{
					return false;
				}

				return true;
			}

			private static bool IntersectsSegment(ref Plane3 plane, ref Triangle3 triangle, ref Vector3D end0, ref Vector3D end1, bool grazing,
				out Triangle3Triangle3Intr info)
			{
				// Compute the 2D representations of the triangle vertices and the
				// segment endpoints relative to the plane of the triangle.  Then
				// compute the intersection in the 2D space.

				// Project the triangle and segment onto the coordinate plane most
				// aligned with the plane normal.
				int maxNormal = 0;
				double fmax   = System.Math.Abs(plane.Normal.x);
				double absMax = System.Math.Abs(plane.Normal.y);
				if (absMax > fmax)
				{
					maxNormal = 1;
					fmax = absMax;
				}
				absMax = System.Math.Abs(plane.Normal.z);
				if (absMax > fmax)
				{
					maxNormal = 2;
				}

				Triangle2 projTri;
				Vector2D projEnd0, projEnd1;

				if (maxNormal == 0)
				{
					// Project onto yz-plane.
					projTri.V0.x = triangle.V0.y;
					projTri.V0.y = triangle.V0.z;
					projTri.V1.x = triangle.V1.y;
					projTri.V1.y = triangle.V1.z;
					projTri.V2.x = triangle.V2.y;
					projTri.V2.y = triangle.V2.z;
					projEnd0.x = end0.y;
					projEnd0.y = end0.z;
					projEnd1.x = end1.y;
					projEnd1.y = end1.z;
				}
				else if (maxNormal == 1)
				{
					// Project onto xz-plane.
					projTri.V0.x = triangle.V0.x;
					projTri.V0.y = triangle.V0.z;
					projTri.V1.x = triangle.V1.x;
					projTri.V1.y = triangle.V1.z;
					projTri.V2.x = triangle.V2.x;
					projTri.V2.y = triangle.V2.z;
					projEnd0.x = end0.x;
					projEnd0.y = end0.z;
					projEnd1.x = end1.x;
					projEnd1.y = end1.z;
				}
				else
				{
					// Project onto xy-plane.
					projTri.V0.x = triangle.V0.x;
					projTri.V0.y = triangle.V0.y;
					projTri.V1.x = triangle.V1.x;
					projTri.V1.y = triangle.V1.y;
					projTri.V2.x = triangle.V2.x;
					projTri.V2.y = triangle.V2.y;
					projEnd0.x = end0.x;
					projEnd0.y = end0.y;
					projEnd1.x = end1.x;
					projEnd1.y = end1.y;
				}

				Segment2 projSeg = new Segment2(ref projEnd0, ref projEnd1);
				Segment2Triangle2Intr tempInfo;
				if (!Intersection.FindSegment2Triangle2(ref projSeg, ref projTri, out tempInfo))
				{
					info = new Triangle3Triangle3Intr();
					return false;
				}

				Vector2D intr0, intr1;
				if (tempInfo.IntersectionType == IntersectionTypes.Segment)
				{
					info.IntersectionType = IntersectionTypes.Segment;
					info.Touching         = grazing;
					info.Quantity         = 2;

					intr0 = tempInfo.Point0;
					intr1 = tempInfo.Point1;
				}
				else
				{
					info.IntersectionType = IntersectionTypes.Point;
					info.Touching         = true;
					info.Quantity         = 1;

					intr0 = tempInfo.Point0;
					intr1 = Vector2D.zero;
				}

				// Unproject the segment of intersection.
				if (maxNormal == 0)
				{
					double invNX = 1f / plane.Normal.x;
					info.Point0 = new Vector3D(invNX * (plane.Constant - plane.Normal.y * intr0.x - plane.Normal.z * intr0.y), intr0.x, intr0.y);
					info.Point1 = info.Quantity == 2 ? new Vector3D(invNX * (plane.Constant - plane.Normal.y * intr1.x - plane.Normal.z * intr1.y), intr1.x, intr1.y) : Vector3D.zero;
				}
				else if (maxNormal == 1)
				{
					double invNY = 1f / plane.Normal.y;
					info.Point0 = new Vector3D(intr0.x, invNY * (plane.Constant - plane.Normal.x * intr0.x - plane.Normal.z * intr0.y), intr0.y);
					info.Point1 = info.Quantity == 2 ? new Vector3D(intr1.x, invNY * (plane.Constant - plane.Normal.x * intr1.x - plane.Normal.z * intr1.y), intr1.y) : Vector3D.zero;
				}
				else
				{
					double invNZ = 1f / plane.Normal.z;
					info.Point0 = new Vector3D(intr0.x, intr0.y, invNZ * (plane.Constant - plane.Normal.x * intr0.x - plane.Normal.y * intr0.y));
					info.Point1 = info.Quantity == 2 ? new Vector3D(intr1.x, intr1.y, invNZ * (plane.Constant - plane.Normal.x * intr1.x - plane.Normal.y * intr1.y)) : Vector3D.zero;
				}

				info.CoplanarIntersectionType = IntersectionTypes.Empty;
				info.Point2 = Vector3D.zero;
				info.Point3 = Vector3D.zero;
				info.Point4 = Vector3D.zero;
				info.Point5 = Vector3D.zero;

				return true;
			}

			private static int QueryToLine(ref Vector2D test, ref Vector2D vec0, ref Vector2D vec1)
			{
				double x0 = test.x - vec0.x;
				double y0 = test.y - vec0.y;
				double x1 = vec1.x - vec0.x;
				double y1 = vec1.y - vec0.y;

				double det = x0 * y1 - x1 * y0;
				return (det > Mathex.ZeroTolerance ? +1 : (det < -Mathex.ZeroTolerance ? -1 : 0));
			}

			private static int QueryToTriangle(ref Vector2D test, ref Vector2D v0, ref Vector2D v1, ref Vector2D v2)
			{
				int sign0 = QueryToLine(ref test, ref v1, ref v2);
				if (sign0 > 0)
				{
					return +1;
				}

				int sign1 = QueryToLine(ref test, ref v0, ref v2);
				if (sign1 < 0)
				{
					return +1;
				}

				int sign2 = QueryToLine(ref test, ref v0, ref v1);
				if (sign2 > 0)
				{
					return +1;
				}

				return ((sign0 != 0 && sign1 != 0 && sign2 != 0) ? -1 : 0);
			}

			private static bool ContainsPoint(ref Triangle3 triangle, ref Plane3 plane, ref Vector3D point,
				out Triangle3Triangle3Intr info)
			{
				// Generate a coordinate system for the plane.  The incoming triangle has
				// vertices <V0,V1,V2>.  The incoming plane has unit-length normal N.
				// The incoming point is P.  V0 is chosen as the origin for the plane. The
				// coordinate axis directions are two unit-length vectors, U0 and U1,
				// constructed so that {U0,U1,N} is an orthonormal set.  Any point Q
				// in the plane may be written as Q = V0 + x0*U0 + x1*U1.  The coordinates
				// are computed as x0 = Dot(U0,Q-V0) and x1 = Dot(U1,Q-V0).
				Vector3D U0, U1;
				Vector3Dex.CreateOrthonormalBasis(out U0, out U1, ref plane.Normal);

				// Compute the planar coordinates for the points P, V1, and V2.  To
				// simplify matters, the origin is subtracted from the points, in which
				// case the planar coordinates are for P-V0, V1-V0, and V2-V0.
				Vector3D PmV0  = point       - triangle.V0;
				Vector3D V1mV0 = triangle.V1 - triangle.V0;
				Vector3D V2mV0 = triangle.V2 - triangle.V0;

				// The planar representation of P-V0.
				Vector2D ProjP = new Vector2D(U0.Dot(PmV0), U1.Dot(PmV0));

				// The planar representation of the triangle <V0-V0,V1-V0,V2-V0>.
				Vector2D ProjV0 = Vector2D.zero;
				Vector2D ProjV1 = new Vector2D(U0.Dot(V1mV0), U1.Dot(V1mV0));
				Vector2D ProjV2 = new Vector2D(U0.Dot(V2mV0), U1.Dot(V2mV0));

				// Test whether P-V0 is in the triangle <0,V1-V0,V2-V0>.
				int query = QueryToTriangle(ref ProjP, ref ProjV0, ref ProjV1, ref ProjV2);
				if (query <= 0)
				{
					// Report the point of intersection to the caller.
					info.IntersectionType = IntersectionTypes.Point;
					info.CoplanarIntersectionType = IntersectionTypes.Empty;
					info.Touching         = true;
					info.Quantity         = 1;
					info.Point0           = point;
					info.Point1           = Vector3D.zero;
					info.Point2           = Vector3D.zero;
					info.Point3           = Vector3D.zero;
					info.Point4           = Vector3D.zero;
					info.Point5           = Vector3D.zero;

					return true;
				}

				info = new Triangle3Triangle3Intr();
				return false;
			}

			private static bool GetCoplanarIntersection(ref Plane3 plane, ref Triangle3 tri0, ref Triangle3 tri1, out Triangle3Triangle3Intr info)
			{
				// Project triangles onto coordinate plane most aligned with plane normal.
				int maxNormal = 0;
				double fmax   = System.Math.Abs(plane.Normal.x);
				double absMax = System.Math.Abs(plane.Normal.y);
				if (absMax > fmax)
				{
					maxNormal = 1;
					fmax = absMax;
				}
				absMax = System.Math.Abs(plane.Normal.z);
				if (absMax > fmax)
				{
					maxNormal = 2;
				}

				Triangle2 projTri0, projTri1;

				if (maxNormal == 0)
				{
					// Project onto yz-plane.

					projTri0.V0.x = tri0.V0.y;
					projTri0.V0.y = tri0.V0.z;
					projTri1.V0.x = tri1.V0.y;
					projTri1.V0.y = tri1.V0.z;

					projTri0.V1.x = tri0.V1.y;
					projTri0.V1.y = tri0.V1.z;
					projTri1.V1.x = tri1.V1.y;
					projTri1.V1.y = tri1.V1.z;

					projTri0.V2.x = tri0.V2.y;
					projTri0.V2.y = tri0.V2.z;
					projTri1.V2.x = tri1.V2.y;
					projTri1.V2.y = tri1.V2.z;
				}
				else if (maxNormal == 1)
				{
					// Project onto xz-plane.

					projTri0.V0.x = tri0.V0.x;
					projTri0.V0.y = tri0.V0.z;
					projTri1.V0.x = tri1.V0.x;
					projTri1.V0.y = tri1.V0.z;

					projTri0.V1.x = tri0.V1.x;
					projTri0.V1.y = tri0.V1.z;
					projTri1.V1.x = tri1.V1.x;
					projTri1.V1.y = tri1.V1.z;

					projTri0.V2.x = tri0.V2.x;
					projTri0.V2.y = tri0.V2.z;
					projTri1.V2.x = tri1.V2.x;
					projTri1.V2.y = tri1.V2.z;
				}
				else
				{
					// Project onto xy-plane.

					projTri0.V0.x = tri0.V0.x;
					projTri0.V0.y = tri0.V0.y;
					projTri1.V0.x = tri1.V0.x;
					projTri1.V0.y = tri1.V0.y;
					
					projTri0.V1.x = tri0.V1.x;
					projTri0.V1.y = tri0.V1.y;
					projTri1.V1.x = tri1.V1.x;
					projTri1.V1.y = tri1.V1.y;
					
					projTri0.V2.x = tri0.V2.x;
					projTri0.V2.y = tri0.V2.y;
					projTri1.V2.x = tri1.V2.x;
					projTri1.V2.y = tri1.V2.y;
				}

				// 2D triangle intersection routines require counterclockwise ordering.
				Vector2D save;
				Vector2D edge0 = projTri0.V1 - projTri0.V0;
				Vector2D edge1 = projTri0.V2 - projTri0.V0;
				if (edge0.DotPerp(edge1) < 0.0f)
				{
					// Triangle is clockwise, reorder it.
					save = projTri0.V1;
					projTri0.V1 = projTri0.V2;
					projTri0.V2 = save;
				}

				edge0 = projTri1.V1 - projTri1.V0;
				edge1 = projTri1.V2 - projTri1.V0;
				if (edge0.DotPerp(edge1) < 0.0f)
				{
					// Triangle is clockwise, reorder it.
					save = projTri1.V1;
					projTri1.V1 = projTri1.V2;
					projTri1.V2 = save;
				}

				Triangle2Triangle2Intr intr;
				bool find = Intersection.FindTriangle2Triangle2(ref projTri0, ref projTri1, out intr);

				info = new Triangle3Triangle3Intr();

				if (!find)
				{
					return false;
				}

				// Map 2D intersections back to the 3D triangle space.
				int quantity = intr.Quantity;
				info.Quantity = quantity;

				if (maxNormal == 0)
				{
					double invNX = 1.0f / plane.Normal.x;
					for (int i = 0; i < quantity; ++i)
					{
						info[i] = new Vector3D(invNX * (plane.Constant - plane.Normal.y * intr[i].x - plane.Normal.z * intr[i].y), intr[i].x, intr[i].y);
					}
				}
				else if (maxNormal == 1)
				{
					double invNY = 1.0f / plane.Normal.y;
					for (int i = 0; i < quantity; ++i)
					{
						info[i] = new Vector3D(intr[i].x, invNY * (plane.Constant - plane.Normal.x * intr[i].x - plane.Normal.z * intr[i].y), intr[i].y);
					}
				}
				else
				{
					double invNZ = 1.0f / plane.Normal.z;
					for (int i = 0; i < quantity; ++i)
					{
						info[i] = new Vector3D(intr[i].x, intr[i].y, invNZ * (plane.Constant - plane.Normal.x * intr[i].x - plane.Normal.y * intr[i].y));
					}
				}

				info.IntersectionType = IntersectionTypes.Plane;
				info.CoplanarIntersectionType = intr.IntersectionType;
				
				return true;
			}


			/// <summary>
			/// Tests if a triangle intersects another triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestTriangle3Triangle3(ref Triangle3 triangle0, ref Triangle3 triangle1, out IntersectionTypes intersectionType)
			{
				// Get edge vectors for triangle0.
				Vector3D E00 = triangle0.V1 - triangle0.V0;
				Vector3D E01 = triangle0.V2 - triangle0.V1;
				Vector3D E02 = triangle0.V0 - triangle0.V2;

				// Get normal vector of triangle0.
				Vector3D N0 = E00.UnitCross(E01);

				// Project triangle1 onto normal line of triangle0, test for separation.
				double N0dT0V0 = N0.Dot(triangle0.V0);
				double min1, max1;
				ProjectOntoAxis(ref triangle1, ref N0, out min1, out max1);
				if (N0dT0V0 < min1 || N0dT0V0 > max1)
				{
					intersectionType = IntersectionTypes.Empty;
					return false;
				}

				// Get edge vectors for triangle1.
				Vector3D E10 = triangle1.V1 - triangle1.V0;
				Vector3D E11 = triangle1.V2 - triangle1.V1;
				Vector3D E12 = triangle1.V0 - triangle1.V2;

				// Get normal vector of triangle1.
				Vector3D N1 = E10.UnitCross(E11);

				Vector3D dir;
				double min0, max0;

				intersectionType = IntersectionTypes.Empty;

				Vector3D N0xN1 = N0.UnitCross(N1);
				if (N0xN1.Dot(N0xN1) >= Mathex.ZeroTolerance)
				{
					// Triangles are not parallel.

					// Project triangle0 onto normal line of triangle1, test for
					// separation.
					double N1dT1V0 = N1.Dot(triangle1.V0);
					ProjectOntoAxis(ref triangle0, ref N1, out min0, out max0);
					if (N1dT1V0 < min0 || N1dT1V0 > max0)
					{
						return false;
					}

					// Directions E0[i0]xE1[i1], i0,i1 = 0,1,2

					// i0 = 0

					dir = E00.UnitCross(E10);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = E00.UnitCross(E11);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = E00.UnitCross(E12);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					// i0 = 1

					dir = E01.UnitCross(E10);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = E01.UnitCross(E11);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = E01.UnitCross(E12);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					// i0 = 2

					dir = E02.UnitCross(E10);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = E02.UnitCross(E11);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = E02.UnitCross(E12);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					// The test query does not know the intersection set.
					intersectionType = IntersectionTypes.Other;
				}
				else  // Triangles are parallel (and, in fact, coplanar).
				{
					// Directions N0xE0[i], i = 0,1,2
					
					dir = N0.UnitCross(E00);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;
					
					dir = N0.UnitCross(E01);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;
					
					dir = N0.UnitCross(E02);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					// Directions N1xE1[i], i = 0,1,2

					dir = N1.UnitCross(E10);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = N1.UnitCross(E11);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					dir = N1.UnitCross(E12);
					ProjectOntoAxis(ref triangle0, ref dir, out min0, out max0);
					ProjectOntoAxis(ref triangle1, ref dir, out min1, out max1);
					if (max0 < min1 || max1 < min0) return false;

					// The test query does not know the intersection set.
					intersectionType = IntersectionTypes.Plane;
				}

				return true;
			}

			/// <summary>
			/// Tests if a triangle intersects another triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestTriangle3Triangle3(ref Triangle3 triangle0, ref Triangle3 triangle1)
			{
				IntersectionTypes intersectionType;
				return TestTriangle3Triangle3(ref triangle0, ref triangle1, out intersectionType);
			}

			/// <summary>
			/// Tests if a triangle intersects another triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindTriangle3Triangle3(ref Triangle3 triangle0, ref Triangle3 triangle1, out Triangle3Triangle3Intr info, bool reportCoplanarIntersections = false)
			{
				// Get the plane of triangle0.
				Plane3 plane0 = new Plane3(ref triangle0.V0, ref triangle0.V1, ref triangle0.V2);

				// Compute the signed distances of triangle1 vertices to plane0.  Use
				// an epsilon-thick plane test.
				double dist0   , dist1   , dist2;
				int   sign0   , sign1   , sign2;
				int   positive, negative, zero;
				
				TrianglePlaneRelations(ref triangle1, ref plane0,
					out dist0, out dist1, out dist2,
					out sign0, out sign1, out sign2,
					out positive, out negative, out zero);

				if (positive == 3 || negative == 3)
				{
					// Triangle1 is fully on one side of plane0.
					info = new Triangle3Triangle3Intr();
					return false;
				}

				if (zero == 3)
				{
					// Triangle1 is contained by plane0.
					if (reportCoplanarIntersections)
					{
						return GetCoplanarIntersection(ref plane0, ref triangle0, ref triangle1, out info);
					}
					else
					{
						info = new Triangle3Triangle3Intr();
						return false;
					}
				}

				// Check for grazing contact between triangle1 and plane0.
				if (positive == 0 || negative == 0)
				{
					if (zero == 2)
					{
						// An edge of triangle1 is in plane0.
						if (sign0 != 0)
						{
							return IntersectsSegment(ref plane0, ref triangle0, ref triangle1.V2, ref triangle1.V1, true, out info);
						}
						if (sign1 != 0)
						{
							return IntersectsSegment(ref plane0, ref triangle0, ref triangle1.V0, ref triangle1.V2, true, out info);
						}
						if (sign2 != 0)
						{
							return IntersectsSegment(ref plane0, ref triangle0, ref triangle1.V1, ref triangle1.V0, true, out info);
						}
					}
					else // zero == 1
					{
						// A vertex of triangle1 is in plane0.
						if (sign0 == 0)
						{
							return ContainsPoint(ref triangle0, ref plane0, ref triangle1.V0, out info);
						}
						if (sign1 == 0)
						{
							return ContainsPoint(ref triangle0, ref plane0, ref triangle1.V1, out info);
						}
						if (sign2 == 0)
						{
							return ContainsPoint(ref triangle0, ref plane0, ref triangle1.V2, out info);
						}
					}
				}

				Plane3 plane1 = new Plane3(ref triangle1.V0, ref triangle1.V1, ref triangle1.V2);
				if (TrianglePlaneRelationsQuick(ref triangle0, ref plane1))
				{
					info = new Triangle3Triangle3Intr();
					return false;
				}

				// At this point, triangle1 transversely intersects plane 0.  Compute the
				// line segment of intersection.  Then test for intersection between this
				// segment and triangle 0.
				double t;
				Vector3D intr0, intr1;

				if (zero == 0)
				{
					int iSign = (positive == 1 ? +1 : -1);

					if (sign0 == iSign)
					{
						t     = dist0 / (dist0 - dist2);
						intr0 = triangle1.V0 + t * (triangle1.V2 - triangle1.V0);
						
						t     = dist0 / (dist0 - dist1);
						intr1 = triangle1.V0 + t * (triangle1.V1 - triangle1.V0);

						return IntersectsSegment(ref plane0, ref triangle0, ref intr0, ref intr1, false, out info);
					}
					else if (sign1 == iSign)
					{
						t     = dist1 / (dist1 - dist0);
						intr0 = triangle1.V1 + t * (triangle1.V0 - triangle1.V1);
						
						t     = dist1 / (dist1 - dist2);
						intr1 = triangle1.V1 + t * (triangle1.V2 - triangle1.V1);

						return IntersectsSegment(ref plane0, ref triangle0, ref intr0, ref intr1, false, out info);
					}
					else if (sign2 == iSign)
					{
						t     = dist2 / (dist2 - dist1);
						intr0 = triangle1.V2 + t * (triangle1.V1 - triangle1.V2);
						
						t     = dist2 / (dist2 - dist0);
						intr1 = triangle1.V2 + t * (triangle1.V0 - triangle1.V2);

						return IntersectsSegment(ref plane0, ref triangle0, ref intr0, ref intr1, false, out info);
					}
				}
				else // zero == 1
				{
					if (sign0 == 0)
					{
						t     = dist2 / (dist2 - dist1);
						intr0 = triangle1.V2 + t * (triangle1.V1 - triangle1.V2);
						
						return IntersectsSegment(ref plane0, ref triangle0, ref triangle1.V0, ref intr0, false, out info);
					}
					else if (sign1 == 0)
					{
						t     = dist0 / (dist0 - dist2);
						intr0 = triangle1.V0 + t * (triangle1.V2 - triangle1.V0);
						
						return IntersectsSegment(ref plane0, ref triangle0, ref triangle1.V1, ref intr0, false, out info);
					}
					else if (sign2 == 0)
					{
						t     = dist1 / (dist1 - dist0);
						intr0 = triangle1.V1 + t * (triangle1.V0 - triangle1.V1);
						
						return IntersectsSegment(ref plane0, ref triangle0, ref triangle1.V2, ref intr0, false, out info);
					}
				}

				// Should not be here
				info = new Triangle3Triangle3Intr();
				return false;
			}
		}
	}
}
