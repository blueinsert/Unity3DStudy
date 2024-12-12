using Real = System.Double;

namespace Dest
{
	namespace Math
	{
		internal class Query3 : Query
		{
			private static Real Zero = (Real)0;

			private Vector3D[] _vertices;

			public Query3(Vector3D[] vertices)
			{
				_vertices = vertices;
			}


			// Returns:
			//   +1, on positive side of plane
			//   -1, on negative side of line
			//    0, on the plane

			public int ToPlane(int i, int v0, int v1, int v2)
			{
				return ToPlane(ref _vertices[i], v0, v1, v2);
			}

			public int ToPlane(ref Vector3D test, int v0, int v1, int v2)
			{
				bool positive = Sort(ref v0, ref v1, ref v2);

				Vector3D vec0 = _vertices[v0];
				Vector3D vec1 = _vertices[v1];
				Vector3D vec2 = _vertices[v2];

				Real x0 = test.x - vec0.x;
				Real y0 = test.y - vec0.y;
				Real z0 = test.z - vec0.z;
				Real x1 = vec1.x - vec0.x;
				Real y1 = vec1.y - vec0.y;
				Real z1 = vec1.z - vec0.z;
				Real x2 = vec2.x - vec0.x;
				Real y2 = vec2.y - vec0.y;
				Real z2 = vec2.z - vec0.z;

				Real det = Det3(x0, y0, z0, x1, y1, z1, x2, y2, z2);
				if (!positive)
				{
					det = -det;
				}

				return (det > Zero ? +1 : (det < Zero ? -1 : 0));
			}


			// Returns:
			//   +1, outside tetrahedron
			//   -1, inside tetrahedron
			//    0, on tetrahedron

			public int ToTetrahedron(int i, int v0, int v1, int v2, int v3)
			{
				return ToTetrahedron(ref _vertices[i], v0, v1, v2, v3);
			}

			public int ToTetrahedron(ref Vector3D test, int v0, int v1, int v2, int v3)
			{
				int sign0 = ToPlane(ref test, v1, v2, v3);
				if (sign0 > 0)
				{
					return +1;
				}

				int sign1 = ToPlane(ref test, v0, v2, v3);
				if (sign1 < 0)
				{
					return +1;
				}

				int sign2 = ToPlane(ref test, v0, v1, v3);
				if (sign2 > 0)
				{
					return +1;
				}

				int sign3 = ToPlane(ref test, v0, v1, v2);
				if (sign3 < 0)
				{
					return +1;
				}

				return ((sign0 != 0 && sign1 != 0 && sign2 != 0 && sign3 != 0) ? -1 : 0);
			}


			// Returns:
			//   +1, outside circumsphere of tetrahedron
			//   -1, inside circumsphere of tetrahedron
			//    0, on circumsphere of tetrahedron

			public int ToCircumsphere(int i, int v0, int v1, int v2, int v3)
			{
				return ToCircumsphere(ref _vertices[i], v0, v1, v2, v3);
			}

			public int ToCircumsphere(ref Vector3D test, int v0, int v1, int v2, int v3)
			{
				bool positive = Sort(ref v0, ref v1, ref v2, ref v3);

				Vector3D vec0 = _vertices[v0];
				Vector3D vec1 = _vertices[v1];
				Vector3D vec2 = _vertices[v2];
				Vector3D vec3 = _vertices[v3];

				Real s0x = vec0.x + test.x;
				Real d0x = vec0.x - test.x;
				Real s0y = vec0.y + test.y;
				Real d0y = vec0.y - test.y;
				Real s0z = vec0.z + test.z;
				Real d0z = vec0.z - test.z;
				Real s1x = vec1.x + test.x;
				Real d1x = vec1.x - test.x;
				Real s1y = vec1.y + test.y;
				Real d1y = vec1.y - test.y;
				Real s1z = vec1.z + test.z;
				Real d1z = vec1.z - test.z;
				Real s2x = vec2.x + test.x;
				Real d2x = vec2.x - test.x;
				Real s2y = vec2.y + test.y;
				Real d2y = vec2.y - test.y;
				Real s2z = vec2.z + test.z;
				Real d2z = vec2.z - test.z;
				Real s3x = vec3.x + test.x;
				Real d3x = vec3.x - test.x;
				Real s3y = vec3.y + test.y;
				Real d3y = vec3.y - test.y;
				Real s3z = vec3.z + test.z;
				Real d3z = vec3.z - test.z;
				Real w0 = s0x * d0x + s0y * d0y + s0z * d0z;
				Real w1 = s1x * d1x + s1y * d1y + s1z * d1z;
				Real w2 = s2x * d2x + s2y * d2y + s2z * d2z;
				Real w3 = s3x * d3x + s3y * d3y + s3z * d3z;

				Real det = Det4(d0x, d0y, d0z, w0, d1x, d1y, d1z, w1, d2x, d2y, d2z, w2, d3x, d3y, d3z, w3);
				if (!positive)
				{
					det = -det;
				}

				return (det > Zero ? 1 : (det < Zero ? -1 : 0));
			}


			// Helper functions.

			public Real Dot(Real x0, Real y0, Real z0, Real x1, Real y1, Real z1)
			{
				return x0 * x1 + y0 * y1 + z0 * z1;
			}

			public Real Det3(Real x0, Real y0, Real z0, Real x1, Real y1, Real z1, Real x2, Real y2, Real z2)
			{
				Real c00 = y1 * z2 - y2 * z1;
				Real c01 = y2 * z0 - y0 * z2;
				Real c02 = y0 * z1 - y1 * z0;
				return x0 * c00 + x1 * c01 + x2 * c02;
			}

			public Real Det4(Real x0, Real y0, Real z0, Real w0, Real x1, Real y1, Real z1, Real w1,
				Real x2, Real y2, Real z2, Real w2, Real x3, Real y3, Real z3, Real w3)
			{
				Real a0 = x0 * y1 - x1 * y0;
				Real a1 = x0 * y2 - x2 * y0;
				Real a2 = x0 * y3 - x3 * y0;
				Real a3 = x1 * y2 - x2 * y1;
				Real a4 = x1 * y3 - x3 * y1;
				Real a5 = x2 * y3 - x3 * y2;
				Real b0 = z0 * w1 - z1 * w0;
				Real b1 = z0 * w2 - z2 * w0;
				Real b2 = z0 * w3 - z3 * w0;
				Real b3 = z1 * w2 - z2 * w1;
				Real b4 = z1 * w3 - z3 * w1;
				Real b5 = z2 * w3 - z3 * w2;
				return a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;
			}
		}
	}
}
