

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			private static int WhichSide(Polygon2 V, Vector2D P, ref Vector2D D)
			{
				// Vertices are projected to the form P+t*D.  Return value is +1 if all
				// t > 0, -1 if all t < 0, 0 otherwise, in which case the line splits the polygon.

				int positive = 0, negative = 0, zero = 0;
				for (int i = 0; i < V.VertexCount; ++i)
				{
					double t = D.Dot(V[i] - P);
					if (t > 0f)
					{
						++positive;
					}
					else if (t < 0f)
					{
						++negative;
					}
					else
					{
						++zero;
					}

					if (positive > 0 && negative > 0)
					{
						return 0;
					}
				}

				return (zero == 0 ? (positive > 0 ? 1 : -1) : 0);
			}

			/// <summary>
			/// Tests whether two convex CCW ordered polygons intersect.
			/// Returns true if intersection occurs false otherwise.
			/// Note that caller is responsibile for supplying proper polygons (convex and CCW ordered).
			/// </summary>
			public static bool TestConvexPolygon2ConvexPolygon2(Polygon2 convexPolygon0, Polygon2 convexPolygon1)
			{
				Vector2D D;

				for (int i0 = 0, i1 = convexPolygon0.VertexCount - 1; i0 < convexPolygon0.VertexCount; i1 = i0, i0++)
				{
					D = (convexPolygon0[i0] - convexPolygon0[i1]).Perp();
					if (WhichSide(convexPolygon1, convexPolygon0[i0], ref D) > 0)
					{
						// C1 is entirely on ‘positive’ side of line C0.V(i0) + t * D
						return false;
					}
				}

				for (int i0 = 0, i1 = convexPolygon1.VertexCount - 1; i0 < convexPolygon1.VertexCount; i1 = i0, i0++)
				{
					D = (convexPolygon1[i0] - convexPolygon1[i1]).Perp();
					if (WhichSide(convexPolygon0, convexPolygon1[i0], ref D) > 0)
					{
						// C0 is entirely on ‘positive’ side of line C1.V(i0) + t * D
						return false;
					}
				}

				return true;
			}
		}
	}
}
