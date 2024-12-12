

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line2 and convex ccw ordered Polygon2
		/// </summary>
		public struct Line2ConvexPolygon2Intr
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
			public Vector2D Point0;

			/// <summary>
			/// Second intersection point
			/// </summary>
			public Vector2D Point1;

			/// <summary>
			/// Line evaluation parameter of the first intersection point
			/// </summary>
			public double Parameter0;

			/// <summary>
			/// Line evaluation parameter of the second intersection point
			/// </summary>
			public double Parameter1;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a line intersects a convex polygon. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine2ConvexPolygon2(ref Line2 line, Polygon2 convexPolygon)
			{
				Edge2[] edges = convexPolygon.Edges;
				int edgeCount = edges.Length;
				
				for (int i = 0; i < edgeCount; ++i)
				{
					Segment2 edgeSegment = new Segment2(ref edges[i].Point0, ref edges[i].Point1);
					
					if (Intersection.TestLine2Segment2(ref line, ref edgeSegment))
					{
						// Return as soon as we have intersection with one of the edges
						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Tests if a line intersects a convex ccw ordered polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine2ConvexPolygon2(ref Line2 line, Polygon2 convexPolygon, out Line2ConvexPolygon2Intr info)
			{
				// http://geomalgorithms.com/a13-_intersect-4.html

				Edge2[] edges = convexPolygon.Edges;
				int edgeCount = edges.Length;
				double tE = double.NegativeInfinity;		// the maximum entering segment parameter
				double tL = double.PositiveInfinity;		// the minimum leaving segment parameter
				double t, N, D;							// intersect parameter t = N / D
				Vector2D dS = line.Direction;			// the  segment direction vector
				Vector2D e;								// edge vector
				Vector2D ne;								// edge outward normal

				for (int i = 0; i < edgeCount; ++i)
				{
					e = edges[i].Point1 - edges[i].Point0;
					ne = new Vector2D(e.y, -e.x);
					N = ne.Dot(edges[i].Point0 - line.Center);	// dot(ne, V[i] - S.P0)
					D = ne.Dot(dS);								// dot(ne, dS)

					if (System.Math.Abs(D) < Mathex.ZeroTolerance)
					{
						// S is nearly parallel to this edge

						if (N < 0)
						{
							// P0 is outside this edge, so S is outside the polygon
							info = new Line2ConvexPolygon2Intr();
							return false;
						}
						else
						{
							// S cannot cross this edge, so ignore this edge
							continue;
						}
					}

					t = N / D;
					if (D < 0)
					{
						// segment S is entering across this edge

						if (t > tE)
						{
							// new max tE
							tE = t;
							if (tE > tL)
							{
								// S enters after leaving polygon
								info = new Line2ConvexPolygon2Intr();
								return false;
							}
						}
					}
					else
					{
						// segment S is leaving across this edge

						if (t < tL)
						{
							// new min tL
							tL = t;
							if (tL < tE)
							{
								// S leaves before entering polygon
								info = new Line2ConvexPolygon2Intr();
								return false;
							}
						}
					}
				}

				// tE <= tL implies that there is a valid intersection subsegment
				// P(tE) = point where S enters polygon
				// P(tL) = point where S leaves polygon
				
				if (tL - tE > Mathex.ZeroTolerance)
				{
					info.IntersectionType = IntersectionTypes.Segment;
					info.Quantity = 2;
					info.Point0 = line.Center + tE * dS;
					info.Point1 = line.Center + tL * dS;
					info.Parameter0 = tE;
					info.Parameter1 = tL;
				}
				else
				{
					info.IntersectionType = IntersectionTypes.Point;
					info.Quantity = 1;
					info.Point0 = line.Center + tE * dS;
					info.Point1 = Vector2Dex.Zero;
					info.Parameter0 = tE;
					info.Parameter1 = 0f;
				}

				return true;
			}
		}
	}
}
