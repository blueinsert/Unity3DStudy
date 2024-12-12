

namespace Dest
{
	namespace Math
	{
		public struct Triangle3
		{
			/// <summary>
			/// First triangle vertex
			/// </summary>
			public Vector3D V0;

			/// <summary>
			/// Second triangle vertex
			/// </summary>
			public Vector3D V1;

			/// <summary>
			/// Third triangle vertex
			/// </summary>
			public Vector3D V2;

			/// <summary>
			/// Gets or sets triangle vertex by index: 0, 1 or 2
			/// </summary>
			public Vector3D this[int index]
			{
				get { switch (index) { case 0: return V0; case 1: return V1; case 2: return V2; default: return Vector3D.zero; } }
				set { switch (index) { case 0: V0 = value; return; case 1: V1 = value; return; case 2: V2 = value; return; } }
			}


			/// <summary>
			/// Creates Triangle3 from 3 vertices
			/// </summary>
			public Triangle3(ref Vector3D v0, ref Vector3D v1, ref Vector3D v2)
			{
				V0 = v0;
				V1 = v1;
				V2 = v2;
			}

			/// <summary>
			/// Creates Triangle3 from 3 vertices
			/// </summary>
			public Triangle3(Vector3D v0, Vector3D v1, Vector3D v2)
			{
				V0 = v0;
				V1 = v1;
				V2 = v2;
			}			

			
			/// <summary>
			/// Returns triangle edge by index 0, 1 or 2
			/// Edge[i] = V[i+1]-V[i]
			/// </summary>
			public Vector3D CalcEdge(int edgeIndex)
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
			/// Returns triangle normal as (V1-V0)x(V2-V0)
			/// </summary>
			public Vector3D CalcNormal()
			{
				return Vector3D.Cross(V1 - V0, V2 - V0);
			}

			/// <summary>
			/// Returns triangle area as 0.5*Abs(Length((V1-V0)x(V2-V0)))
			/// </summary>
			public double CalcArea()
			{
				return 0.5f * Vector3D.Cross(V1 - V0, V2 - V0).magnitude;
			}

			/// <summary>
			/// Returns triangle area defined by 3 points.
			/// </summary>
			public static double CalcArea(ref Vector3D v0, ref Vector3D v1, ref Vector3D v2)
			{
				return 0.5f * Vector3D.Cross(v1 - v0, v2 - v0).magnitude;
			}

			/// <summary>
			/// Returns triangle area defined by 3 points.
			/// </summary>
			public static double CalcArea(Vector3D v0, Vector3D v1, Vector3D v2)
			{
				return 0.5f * Vector3D.Cross(v1 - v0, v2 - v0).magnitude;
			}

			/// <summary>
			/// Calculates angles of the triangle in degrees.
			/// Angles are returned in the instance of Vector3D following way: (angle of vertex V0, angle of vertex V1, angle of vertex V2)
			/// </summary>
			public Vector3D CalcAnglesDeg()
			{
				double sideX = V2.x - V1.x;
				double sideY = V2.y - V1.y;
				double sideZ = V2.z - V1.z;
				double aSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = V2.x - V0.x;
				sideY = V2.y - V0.y;
				sideZ = V2.z - V0.z;
				double bSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = V1.x - V0.x;
				sideY = V1.y - V0.y;
				sideZ = V1.z - V0.z;
				double cSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;
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
			public static Vector3D CalcAnglesDeg(ref Vector3D v0, ref Vector3D v1, ref Vector3D v2)
			{
				double sideX = v2.x - v1.x;
				double sideY = v2.y - v1.y;
				double sideZ = v2.z - v1.z;
				double aSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = v2.x - v0.x;
				sideY = v2.y - v0.y;
				sideZ = v2.z - v0.z;
				double bSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = v1.x - v0.x;
				sideY = v1.y - v0.y;
				sideZ = v1.z - v0.z;
				double cSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;
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
			public static Vector3D CalcAnglesDeg(Vector3D v0, Vector3D v1, Vector3D v2)
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
				double sideZ = V2.z - V1.z;
				double aSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = V2.x - V0.x;
				sideY = V2.y - V0.y;
				sideZ = V2.z - V0.z;
				double bSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = V1.x - V0.x;
				sideY = V1.y - V0.y;
				sideZ = V1.z - V0.z;
				double cSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;
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
			public static Vector3D CalcAnglesRad(ref Vector3D v0, ref Vector3D v1, ref Vector3D v2)
			{
				double sideX = v2.x - v1.x;
				double sideY = v2.y - v1.y;
				double sideZ = v2.z - v1.z;
				double aSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = v2.x - v0.x;
				sideY = v2.y - v0.y;
				sideZ = v2.z - v0.z;
				double bSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;

				sideX = v1.x - v0.x;
				sideY = v1.y - v0.y;
				sideZ = v1.z - v0.z;
				double cSqr = sideX * sideX + sideY * sideY + sideZ * sideZ;
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
			public static Vector3D CalcAnglesRad(Vector3D v0, Vector3D v1, Vector3D v2)
			{
				return CalcAnglesRad(ref v0, ref v1, ref v2);
			}

			/// <summary>
			/// Gets point on the triangle using barycentric coordinates.
			/// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1, c2 is calculated as 1-c0-c1.
			/// </summary>
			public Vector3D EvalBarycentric(double c0, double c1)
			{
				double c2 = 1f - c0 - c1;
				return c0 * V0 + c1 * V1 + c2 * V2;
			}

			/// <summary>
			/// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
			/// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
			/// </summary>
			public Vector3D EvalBarycentric(ref Vector3D baryCoords)
			{
				return baryCoords.x * V0 + baryCoords.y * V1 + baryCoords.z * V2;
			}

			/// <summary>
			/// Gets point on the triangle using barycentric coordinates. baryCoords parameter is (c0,c1,c2).
			/// The result is c0*V0 + c1*V1 + c2*V2, 0 &lt;= c0,c1,c2 &lt;= 1, c0+c1+c2=1
			/// </summary>
			public Vector3D EvalBarycentric(Vector3D baryCoords)
			{
				return baryCoords.x * V0 + baryCoords.y * V1 + baryCoords.z * V2;
			}

			/// <summary>
			/// Calculate barycentric coordinates for the input point with regarding to triangle defined by 3 points.
			/// </summary>
			public static void CalcBarycentricCoords(ref Vector3D point, ref Vector3D v0, ref Vector3D v1, ref Vector3D v2, out Vector3D baryCoords)
			{
				// http://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
				
				Vector3D e0 = v1 - v0;
				Vector3D e1 = v2 - v0;
				Vector3D e2 = point - v0;
				double d00 = Vector3Dex.Dot(ref e0, ref e0);
				double d01 = Vector3Dex.Dot(ref e0, ref e1);
				double d11 = Vector3Dex.Dot(ref e1, ref e1);
				double d20 = Vector3Dex.Dot(ref e2, ref e0);
				double d21 = Vector3Dex.Dot(ref e2, ref e1);
				double denomInv = 1f / (d00 * d11 - d01 * d01);

				baryCoords.y = (d11 * d20 - d01 * d21) * denomInv;
				baryCoords.z = (d00 * d21 - d01 * d20) * denomInv;
				baryCoords.x = 1.0f - baryCoords.y - baryCoords.z;
			}

			/// <summary>
			/// Calculate barycentric coordinates for the input point regarding to the triangle.
			/// </summary>
			public Vector3D CalcBarycentricCoords(ref Vector3D point)
			{
				Vector3D result;
				CalcBarycentricCoords(ref point, ref V0, ref V1, ref V2, out result);
				return result;
			}

			/// <summary>
			/// Calculate barycentric coordinates for the input point regarding to the triangle.
			/// </summary>
			public Vector3D CalcBarycentricCoords(Vector3D point)
			{
				Vector3D result;
				CalcBarycentricCoords(ref point, ref V0, ref V1, ref V2, out result);
				return result;
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
