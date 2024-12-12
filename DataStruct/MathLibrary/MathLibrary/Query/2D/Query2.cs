using Real = System.Double;

namespace Dest
{
	namespace Math
	{
		internal class Query2 : Query
		{
			private static Real Zero = (Real)0;

			private Vector2D[] _vertices;

			public Query2(Vector2D[] vertices)
			{
				_vertices = vertices;
			}


			// Returns:
			//   +1, on right of line
			//   -1, on left of line
			//    0, on the line

			public int ToLine(int i, int v0, int v1)
			{
				return ToLine(ref _vertices[i], v0, v1);
			}

			public int ToLine(ref Vector2D test, int v0, int v1)
			{
				bool positive = Sort(ref v0, ref v1);

				Vector2D vec0 = _vertices[v0];
				Vector2D vec1 = _vertices[v1];

				Real x0 = test.x - vec0.x;
				Real y0 = test.y - vec0.y;
				Real x1 = vec1.x - vec0.x;
				Real y1 = vec1.y - vec0.y;

				Real det = Det2(x0, y0, x1, y1);
				if (!positive)
				{
					det = -det;
				}

				return (det > Zero ? +1 : (det < Zero ? -1 : 0));
			}


			// Returns:
			//   +1, outside triangle
			//   -1, inside triangle
			//    0, on triangle

			public int ToTriangle(int i, int v0, int v1, int v2)
			{
				return ToTriangle(ref _vertices[i], v0, v1, v2);
			}

			public int ToTriangle(ref Vector2D test, int v0, int v1, int v2)
			{
				int sign0 = ToLine(ref test, v1, v2);
				if (sign0 > 0)
				{
					return +1;
				}

				int sign1 = ToLine(ref test, v0, v2);
				if (sign1 < 0)
				{
					return +1;
				}

				int sign2 = ToLine(ref test, v0, v1);
				if (sign2 > 0)
				{
					return +1;
				}

				return ((sign0 != 0 && sign1 != 0 && sign2 != 0) ? -1 : 0);
			}


			// Returns:
			//   +1, outside circumcircle of triangle
			//   -1, inside circumcircle of triangle
			//    0, on circumcircle of triangle

			public int ToCircumcircle(int i, int v0, int v1, int v2)
			{
				return ToCircumcircle(ref _vertices[i], v0, v1, v2);
			}

			public int ToCircumcircle(ref Vector2D test, int v0, int v1, int v2)
			{
				bool positive = Sort(ref v0, ref v1, ref v2);

				Vector2D vec0 = _vertices[v0];
				Vector2D vec1 = _vertices[v1];
				Vector2D vec2 = _vertices[v2];

				Real s0x = vec0.x + test.x;
				Real d0x = vec0.x - test.x;
				Real s0y = vec0.y + test.y;
				Real d0y = vec0.y - test.y;
				Real s1x = vec1.x + test.x;
				Real d1x = vec1.x - test.x;
				Real s1y = vec1.y + test.y;
				Real d1y = vec1.y - test.y;
				Real s2x = vec2.x + test.x;
				Real d2x = vec2.x - test.x;
				Real s2y = vec2.y + test.y;
				Real d2y = vec2.y - test.y;
				Real z0 = s0x * d0x + s0y * d0y;
				Real z1 = s1x * d1x + s1y * d1y;
				Real z2 = s2x * d2x + s2y * d2y;

				Real det = Det3(d0x, d0y, z0, d1x, d1y, z1, d2x, d2y, z2);
				if (!positive)
				{
					det = -det;
				}

				return (det < Zero ? 1 : (det > Zero ? -1 : 0));
			}


			// Helper functions.

			public Real Dot(Real x0, Real y0, Real x1, Real y1)
			{
				return x0 * x1 + y0 * y1;
			}

			public Real Det2(Real x0, Real y0, Real x1, Real y1)
			{
				return x0 * y1 - x1 * y0;
			}

			public Real Det3(Real x0, Real y0, Real z0, Real x1, Real y1, Real z1, Real x2, Real y2, Real z2)
			{
				Real c00 = y1 * z2 - y2 * z1;
				Real c01 = y2 * z0 - y0 * z2;
				Real c02 = y0 * z1 - y1 * z0;
				return x0 * c00 + x1 * c01 + x2 * c02;
			}
		}
	}
}
