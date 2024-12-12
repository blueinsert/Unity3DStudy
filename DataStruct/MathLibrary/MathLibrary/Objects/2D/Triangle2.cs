

namespace Dest
{
	namespace Math
	{
		public struct Triangle2
		{
			/// <summary>
			/// First triangle vertex
			/// </summary>
			public Vector2D V0;

			/// <summary>
			/// Second triangle vertex
			/// </summary>
			public Vector2D V1;

			/// <summary>
			/// Third triangle vertex
			/// </summary>
			public Vector2D V2;

			/// <summary>
			/// Gets or sets triangle vertex by index: 0, 1 or 2
			/// </summary>
			public Vector2D this[int index]
			{
				get { switch (index) { case 0: return V0; case 1: return V1; case 2: return V2; default: return Vector2D.zero; } }
				set { switch (index) { case 0: V0 = value; return; case 1: V1 = value; return; case 2: V2 = value; return; } }
			}


			/// <summary>
			/// Creates Triangle2 from 3 vertices
			/// </summary>
			public Triangle2(ref Vector2D v0, ref Vector2D v1, ref Vector2D v2)
			{
				V0 = v0;
				V1 = v1;
				V2 = v2;
			}

			/// <summary>
			/// Creates Triangle2 from 3 vertices
			/// </summary>
			public Triangle2(Vector2D v0, Vector2D v1, Vector2D v2)
			{
				V0 = v0;
				V1 = v1;
				V2 = v2;
			}
			
			
			/// <summary>
			/// Returns triangle edge by index 0, 1 or 2
			/// Edge[i] = V[i+1]-V[i]
			/// </summary>
			public Vector2D CalcEdge(int edgeIndex)
			{
				switch (edgeIndex)
				{
					case 0: return V1 - V0;
					case 1: return V2 - V1;
					case 2: return V0 - V2;
				}
				return Vector2D.zero;
			}

			/// <summary>
			/// Calculates cross product of triangle edges: (V1-V0)x(V2-V0).
			/// If the result is positive then triangle is ordered counter clockwise,
			/// if the result is negative then triangle is ordered clockwise,
			/// if the result is zero then triangle is degenerate.
			/// </summary>
			public double CalcDeterminant()
			{
				return V1.x * V2.y + V0.x * V1.y + V2.x * V0.y - V1.x * V0.y - V2.x * V1.y - V0.x * V2.y;
			}

			/// <summary>
			/// Calculates triangle orientation. See CalcDeterminant() for the description.
			/// </summary>
			public Orientations CalcOrientation(double threshold = Mathex.ZeroTolerance)
			{
				double det = CalcDeterminant();
				if (det > threshold) return Orientations.CCW;
				if (det < -threshold) return Orientations.CW;
				return Orientations.None;
			}

			/// <summary>
			/// Calculates area of the triangle. It's equal to Abs(Determinant())/2
			/// </summary>
			/// <returns></returns>
			public double CalcArea()
			{
				return 0.5f * System.Math.Abs(CalcDeterminant());
			}

			/// <summary>
			/// Calculates area of the triangle defined by 3 points.
			/// </summary>
			public static double CalcArea(ref Vector2D v0, ref Vector2D v1, ref Vector2D v2)
			{
				return 0.5f * System.Math.Abs(v1.x * v2.y + v0.x * v1.y + v2.x * v0.y - v1.x * v0.y - v2.x * v1.y - v0.x * v2.y);
			}

			/// <summary>
			/// Calculates area of the triangle defined by 3 points.
			/// </summary>
			public static double CalcArea(Vector2D v0, Vector2D v1, Vector2D v2)
			{
				return 0.5f * System.Math.Abs(v1.x * v2.y + v0.x * v1.y + v2.x * v0.y - v1.x * v0.y - v2.x * v1.y - v0.x * v2.y);
			}

			/// <summary>
			/// Calculates angles of the triangle in degrees.
			/// Angles are returned in the instance of Vector3D following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
			/// </summary>
			public Vector3D CalcAnglesDeg()
			{
				double sideX = V2.x - V1.x;
				double sideY = V2.y - V1.y;
				double aSqr = sideX * sideX + sideY * sideY;

				sideX = V2.x - V0.x;
				sideY = V2.y - V0.y;
				double bSqr = sideX * sideX + sideY * sideY;

				sideX = V1.x - V0.x;
				sideY = V1.y - V0.y;
				double cSqr = sideX * sideX + sideY * sideY;
				double two_c = 2f * System.Math.Sqrt(cSqr);

				Vector3D result;
				result.x = System.Math.Acos((bSqr + cSqr - aSqr) / (System.Math.Sqrt(bSqr) * two_c)) * Mathex.Rad2Deg;
				result.y = System.Math.Acos((aSqr + cSqr - bSqr) / (System.Math.Sqrt(aSqr) * two_c)) * Mathex.Rad2Deg;
				result.z = 180f - result.x - result.y;

				return result;
			}

			/// <summary>
			/// Calculates angles of the triangle defined by 3 points in degrees.
			/// Angles are returned in the instance of Vector3D following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
			/// </summary>
			public static Vector3D CalcAnglesDeg(ref Vector2D v0, ref Vector2D v1, ref Vector2D v2)
			{
				double sideX = v2.x - v1.x;
				double sideY = v2.y - v1.y;
				double aSqr = sideX * sideX + sideY * sideY;

				sideX = v2.x - v0.x;
				sideY = v2.y - v0.y;
				double bSqr = sideX * sideX + sideY * sideY;

				sideX = v1.x - v0.x;
				sideY = v1.y - v0.y;
				double cSqr = sideX * sideX + sideY * sideY;
				double two_c = 2f * System.Math.Sqrt(cSqr);

				Vector3D result;
				result.x = System.Math.Acos((bSqr + cSqr - aSqr) / (System.Math.Sqrt(bSqr) * two_c)) * Mathex.Rad2Deg;
				result.y = System.Math.Acos((aSqr + cSqr - bSqr) / (System.Math.Sqrt(aSqr) * two_c)) * Mathex.Rad2Deg;
				result.z = 180f - result.x - result.y;

				return result;
			}

			/// <summary>
			/// Calculates angles of the triangle defined by 3 points in degrees.
			/// Angles are returned in the instance of Vector3D following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
			/// </summary>
			public static Vector3D CalcAnglesDeg(Vector2D v0, Vector2D v1, Vector2D v2)
			{
				return CalcAnglesDeg(ref v0, ref v1, ref v2);
			}

			/// <summary>
			/// Calculates angles of the triangle in radians.
			/// Angles are returned in the instance of Vector3D following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
			/// </summary>
			public Vector3D CalcAnglesRad()
			{
				double sideX = V2.x - V1.x;
				double sideY = V2.y - V1.y;
				double aSqr = sideX * sideX + sideY * sideY;

				sideX = V2.x - V0.x;
				sideY = V2.y - V0.y;
				double bSqr = sideX * sideX + sideY * sideY;

				sideX = V1.x - V0.x;
				sideY = V1.y - V0.y;
				double cSqr = sideX * sideX + sideY * sideY;
				double two_c = 2f * System.Math.Sqrt(cSqr);

				Vector3D result;
				result.x = System.Math.Acos((bSqr + cSqr - aSqr) / (System.Math.Sqrt(bSqr) * two_c));
				result.y = System.Math.Acos((aSqr + cSqr - bSqr) / (System.Math.Sqrt(aSqr) * two_c));
				result.z = Mathex.Pi - result.x - result.y;

				return result;
			}

			/// <summary>
			/// Calculates angles of the triangle defined by 3 points in radians.
			/// Angles are returned in the instance of Vector3D following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
			/// </summary>
			public static Vector3D CalcAnglesRad(ref Vector2D v0, ref Vector2D v1, ref Vector2D v2)
			{
				double sideX = v2.x - v1.x;
				double sideY = v2.y - v1.y;
				double aSqr = sideX * sideX + sideY * sideY;

				sideX = v2.x - v0.x;
				sideY = v2.y - v0.y;
				double bSqr = sideX * sideX + sideY * sideY;

				sideX = v1.x - v0.x;
				sideY = v1.y - v0.y;
				double cSqr = sideX * sideX + sideY * sideY;
				double two_c = 2f * System.Math.Sqrt(cSqr);

				Vector3D result;
				result.x = System.Math.Acos((bSqr + cSqr - aSqr) / (System.Math.Sqrt(bSqr) * two_c));
				result.y = System.Math.Acos((aSqr + cSqr - bSqr) / (System.Math.Sqrt(aSqr) * two_c));
				result.z = Mathex.Pi - result.x - result.y;

				return result;
			}

			/// <summary>
			/// Calculates angles of the triangle defined by 3 points in radians.
			/// Angles are returned in the instance of Vector3D following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
			/// </summary>
			public static Vector3D CalcAnglesRad(Vector2D v0, Vector2D v1, Vector2D v2)
			{
				return CalcAnglesRad(ref v0, ref v1, ref v2);
			}

			/// <summary>
			/// Gets point on the triangle using barycentric coordinates.
			/// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1, c2 is calculated as 1-c0-c1.
			/// </summary>
			public Vector2D EvalBarycentric(double c0, double c1)
			{
				double c2 = 1f - c0 - c1;
				return c0 * V0 + c1 * V1 + c2 * V2;
			}

			/// <summary>
			/// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
			/// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
			/// </summary>
			public Vector2D EvalBarycentric(ref Vector3D baryCoords)
			{
				return baryCoords.x * V0 + baryCoords.y * V1 + baryCoords.z * V2;
			}

			/// <summary>
			/// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
			/// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
			/// </summary>
			public Vector2D EvalBarycentric(Vector3D baryCoords)
			{
				return baryCoords.x * V0 + baryCoords.y * V1 + baryCoords.z * V2;
			}

			/// <summary>
			/// Calculate barycentric coordinates for the input point with regarding to triangle defined by 3 points.
			/// </summary>
			public static void CalcBarycentricCoords(ref Vector2D point, ref Vector2D v0, ref Vector2D v1, ref Vector2D v2, out Vector3D baryCoords)
			{
				// http://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates

				Vector2D e0 = v1 - v0;
				Vector2D e1 = v2 - v0;
				Vector2D e2 = point - v0;
				double d00 = Vector2Dex.Dot(ref e0, ref e0);
				double d01 = Vector2Dex.Dot(ref e0, ref e1);
				double d11 = Vector2Dex.Dot(ref e1, ref e1);
				double d20 = Vector2Dex.Dot(ref e2, ref e0);
				double d21 = Vector2Dex.Dot(ref e2, ref e1);
				double denomInv = 1f / (d00 * d11 - d01 * d01);

				baryCoords.y = (d11 * d20 - d01 * d21) * denomInv;
				baryCoords.z = (d00 * d21 - d01 * d20) * denomInv;
				baryCoords.x = 1.0f - baryCoords.y - baryCoords.z;
			}

			/// <summary>
			/// Calculate barycentric coordinates for the input point regarding to the triangle.
			/// </summary>
			public Vector3D CalcBarycentricCoords(ref Vector2D point)
			{
				Vector3D result;
				CalcBarycentricCoords(ref point, ref V0, ref V1, ref V2, out result);
				return result;
			}

			/// <summary>
			/// Calculate barycentric coordinates for the input point regarding to the triangle.
			/// </summary>
			public Vector3D CalcBarycentricCoords(Vector2D point)
			{
				Vector3D result;
				CalcBarycentricCoords(ref point, ref V0, ref V1, ref V2, out result);
				return result;
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector2D point)
			{
				return Distance.Point2Triangle2(ref point, ref this);
			}

			/// <summary>
			/// Determines on which side of the triangle a point is. Returns +1 if a point
			/// is outside of the triangle, 0 if it's on the triangle border, -1 if it's inside the triangle.
			/// Method must be called for CCW ordered triangles.
			/// </summary>
			public int QuerySideCCW(Vector2D point, double epsilon = Mathex.ZeroTolerance)
			{
				double dist0 = (point.x - V0.x) * (V1.y - V0.y) - (point.y - V0.y) * (V1.x - V0.x);
				if (dist0 > epsilon) return 1;

				double dist1 = (point.x - V1.x) * (V2.y - V1.y) - (point.y - V1.y) * (V2.x - V1.x);
				if (dist1 > epsilon) return 1;

				double dist2 = (point.x - V2.x) * (V0.y - V2.y) - (point.y - V2.y) * (V0.x - V2.x);
				if (dist2 > epsilon) return 1;

				double negEpsilon = -epsilon;
				return (dist0 < negEpsilon && dist1 < negEpsilon && dist2 < negEpsilon) ? -1 : 0;
			}

			/// <summary>
			/// Determines on which side of the triangle a point is. Returns +1 if a point
			/// is outside of the triangle, 0 if it's on the triangle border, -1 if it's inside the triangle.
			/// Method must be called for CW ordered triangles.
			/// </summary>
			public int QuerySideCW(Vector2D point, double epsilon = Mathex.ZeroTolerance)
			{
				double negEpsilon = -epsilon;

				double dist0 = (point.x - V0.x) * (V1.y - V0.y) - (point.y - V0.y) * (V1.x - V0.x);
				if (dist0 < negEpsilon) return 1;

				double dist1 = (point.x - V1.x) * (V2.y - V1.y) - (point.y - V1.y) * (V2.x - V1.x);
				if (dist1 < negEpsilon) return 1;

				double dist2 = (point.x - V2.x) * (V0.y - V2.y) - (point.y - V2.y) * (V0.x - V2.x);
				if (dist2 < negEpsilon) return 1;

				return (dist0 > epsilon && dist1 > epsilon && dist2 > epsilon) ? -1 : 0;
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector2D Project(Vector2D point)
			{
				Vector2D result;
				Distance.SqrPoint2Triangle2(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Tests whether a point is contained by the triangle (CW or CCW ordered).
			/// Note however that if the triangle is CCW then points which are on triangle border considered inside, but
			/// if the triangle is CW then points which are on triangle border considered outside.
			/// For consistent (and faster) test use appropriate overloads for CW and CCW triangles.
			/// </summary>
			public bool Contains(ref Vector2D point)
			{
				//private double Sign(ref Vector2D p0, ref Vector2D p1, ref Vector2D p2)
				//{
				//	return (p0.x - p2.x) * (p1.y - p2.y) - (p0.y - p2.y) * (p1.x - p2.x);
				//}
				// b0 = Sign(ref point, ref V0, ref V1) < 0;
				// b1 = Sign(ref point, ref V1, ref V2) < 0;
				// b2 = Sign(ref point, ref V2, ref V0) < 0;
				
				bool b0 = ((point.x - V1.x) * (V0.y - V1.y) - (point.y - V1.y) * (V0.x - V1.x)) < 0.0f;
				bool b1 = ((point.x - V2.x) * (V1.y - V2.y) - (point.y - V2.y) * (V1.x - V2.x)) < 0.0f;
				if (b0 != b1) return false;
				bool b2 = ((point.x - V0.x) * (V2.y - V0.y) - (point.y - V0.y) * (V2.x - V0.x)) < 0.0f;
				return b1 == b2;
			}

			/// <summary>
			/// Tests whether a point is contained by the triangle (CW or CCW ordered).
			/// Note however that if the triangle is CCW then points which are on triangle border considered inside, but
			/// if the triangle is CW then points which are on triangle border considered outside.
			/// For consistent (and faster) test use appropriate overloads for CW and CCW triangles.
			/// </summary>
			public bool Contains(Vector2D point)
			{
				return Contains(ref point);
			}

			/// <summary>
			/// Tests whether a point is contained by the CCW triangle
			/// </summary>
			public bool ContainsCCW(ref Vector2D point)
			{
				if ((point.x - V0.x) * (V1.y - V0.y) - (point.y - V0.y) * (V1.x - V0.x) > 0.0f) return false;
				if ((point.x - V1.x) * (V2.y - V1.y) - (point.y - V1.y) * (V2.x - V1.x) > 0.0f) return false;
				if ((point.x - V2.x) * (V0.y - V2.y) - (point.y - V2.y) * (V0.x - V2.x) > 0.0f) return false;
				return true;
			}

			/// <summary>
			/// Tests whether a point is contained by the CCW triangle
			/// </summary>
			public bool ContainsCCW(Vector2D point)
			{
				return ContainsCCW(ref point);
			}

			/// <summary>
			/// Tests whether a point is contained by the CW triangle
			/// </summary>
			public bool ContainsCW(ref Vector2D point)
			{
				if ((point.x - V0.x) * (V1.y - V0.y) - (point.y - V0.y) * (V1.x - V0.x) < 0.0f) return false;
				if ((point.x - V1.x) * (V2.y - V1.y) - (point.y - V1.y) * (V2.x - V1.x) < 0.0f) return false;
				if ((point.x - V2.x) * (V0.y - V2.y) - (point.y - V2.y) * (V0.x - V2.x) < 0.0f) return false;
				return true;
			}

			/// <summary>
			/// Tests whether a point is contained by the CW triangle
			/// </summary>
			public bool ContainsCW(Vector2D point)
			{
				return ContainsCW(ref point);
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[V0: {0} V1: {1} V2: {2}]", V0.ToStringEx(), V1.ToStringEx(), V2.ToStringEx());
			}
		}
	}
}
