

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Triangle2 and Triangle2
		/// </summary>
		public struct Triangle2Triangle2Intr
		{
			/// <summary>
			/// Gets intersection point by index (0 to 5). Points could be also accessed individually using Point0,...,Point5 fields.
			/// </summary>
			public Vector2D this[int i]
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
						default: return Vector2D.zero;
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
			/// IntersectionTypes.Point if triangles are touching in a point;
			/// IntersectionTypes.Segment if triangles are touching in a segment;
			/// IntersectionTypes.Polygon if triangles intersect.
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Number of intersection points.
			/// IntersectionTypes.Empty: 0;
			/// IntersectionTypes.Point: 1;
			/// IntersectionTypes.Segment: 2;
			/// IntersectionTypes.Polygon: 3 to 6.
			/// </summary>
			public int Quantity;

			public Vector2D Point0;
			public Vector2D Point1;
			public Vector2D Point2;
			public Vector2D Point3;
			public Vector2D Point4;
			public Vector2D Point5;
		}

		internal struct Float6
		{
			private double _0;
			private double _1;
			private double _2;
			private double _3;
			private double _4;
			private double _5;

			public double this[int i]
			{
				get
				{
					switch (i)
					{
						case 0: return _0;
						case 1: return _1;
						case 2: return _2;
						case 3: return _3;
						case 4: return _4;
						case 5: return _5;
						default: return 0.0f;
					}
				}
				set
				{
					switch (i)
					{
						case 0: _0 = value; break;
						case 1: _1 = value; break;
						case 2: _2 = value; break;
						case 3: _3 = value; break;
						case 4: _4 = value; break;
						case 5: _5 = value; break;
					}
				}
			}
		}

		public static partial class Intersection
		{
			private static int WhichSide(ref Triangle2 triangle, ref Vector2D P, ref Vector2D D)
			{
				// Vertices are projected to the form P+t*D.  Return value is +1 if all
				// t > 0, -1 if all t < 0, 0 otherwise, in which case the line splits the
				// triangle.

				int positive = 0, negative = 0, zero = 0;
				double dist;

				dist = D.Dot(triangle.V0 - P);
				if (dist > Mathex.ZeroTolerance)
				{
					++positive;
				}
				else if (dist < -Mathex.ZeroTolerance)
				{
					++negative;
				}
				else
				{
					++zero;
				}
				if (positive > 0 && negative > 0) return 0;

				dist = D.Dot(triangle.V1 - P);
				if (dist > Mathex.ZeroTolerance)
				{
					++positive;
				}
				else if (dist < -Mathex.ZeroTolerance)
				{
					++negative;
				}
				else
				{
					++zero;
				}
				if (positive > 0 && negative > 0) return 0;

				dist = D.Dot(triangle.V2 - P);
				if (dist > Mathex.ZeroTolerance)
				{
					++positive;
				}
				else if (dist < -Mathex.ZeroTolerance)
				{
					++negative;
				}
				else
				{
					++zero;
				}
				if (positive > 0 && negative > 0) return 0;

				return zero == 0 ? (positive > 0 ? 1 : -1) : 0;
			}

			private static void ClipConvexPolygonAgainstLine(ref Vector2D edgeStart, ref Vector2D edgeEnd, ref int quantity, ref Triangle2Triangle2Intr info)
			{
				// The input vertices are assumed to be in counterclockwise order.
				// The ordering is an invariant of this function.

				Vector2D edgeNormal = new Vector2D(edgeStart.y - edgeEnd.y, edgeEnd.x - edgeStart.x);
				double c = edgeNormal.Dot(edgeStart);

				// Test on which side of line the vertices are.
				int positive = 0, negative = 0, zero = 0, pIndex = -1;
				Float6 dist = new Float6();
				for (int i = 0; i < quantity; ++i)
				{
					double distEntry = edgeNormal.Dot(info[i]) - c;
					if (distEntry > Mathex.ZeroTolerance)
					{
						++positive;
						if (pIndex < 0) pIndex = i;
					}
					else if (distEntry < -Mathex.ZeroTolerance)
					{
						++negative;
					}
					else
					{
						distEntry = 0.0f;
						++zero;
					}
					dist[i] = distEntry;
				}

				if (positive > 0)
				{
					if (negative > 0)
					{
						// Line transversely intersects polygon.
						Triangle2Triangle2Intr CV = new Triangle2Triangle2Intr();
						int cQuantity = 0, cur, prv;

						if (pIndex > 0)
						{
							// First clip vertex on line.
							cur = pIndex;
							prv = cur - 1;

							double t = dist[cur] / (dist[cur] - dist[prv]);
							CV[cQuantity++] = info[cur] + t * (info[prv] - info[cur]);

							// Vertices on positive side of line.
							while (cur < quantity && dist[cur] > 0f)
							{
								CV[cQuantity++] = info[cur++];
							}

							// Last clip vertex on line.
							if (cur < quantity)
							{
								prv = cur - 1;
							}
							else
							{
								cur = 0;
								prv = quantity - 1;
							}

							t = dist[cur] / (dist[cur] - dist[prv]);
							CV[cQuantity++] = info[cur] + t * (info[prv] - info[cur]);
						}
						else  // pIndex is 0
						{
							// Vertices on positive side of line.
							cur = 0;
							while (cur < quantity && dist[cur] > 0.0f)
							{
								CV[cQuantity++] = info[cur++];
							}

							// Last clip vertex on line.
							prv = cur - 1;

							double t = dist[cur] / (dist[cur] - dist[prv]);
							CV[cQuantity++] = info[cur] + t * (info[prv] - info[cur]);

							// Skip vertices on negative side.
							while (cur < quantity && dist[cur] <= 0.0f)
							{
								++cur;
							}

							// First clip vertex on line.
							if (cur < quantity)
							{
								prv = cur - 1;

								t = dist[cur] / (dist[cur] - dist[prv]);
								CV[cQuantity++] = info[cur] + t * (info[prv] - info[cur]);

								// Vertices on positive side of line.
								while (cur < quantity && dist[cur] > 0.0f)
								{
									CV[cQuantity++] = info[cur++];
								}
							}
							else
							{
								// cur = 0
								prv = quantity - 1;

								t = dist[0] / (dist[0] - dist[prv]);
								CV[cQuantity++] = info[0] + t * (info[prv] - info[0]);
							}
						}

						quantity = cQuantity;
						info = CV;
					}
					// else polygon fully on positive side of line, nothing to do.
				}
				else
				{
					if (zero == 0)
					{
						// Polygon does not intersect positive side of line, clip all.
						quantity = 0;
					}
					else
					{
						int maxNormal = System.Math.Abs(edgeNormal.y) > System.Math.Abs(edgeNormal.x) ? 1 : 0;

						double minValue = double.PositiveInfinity, maxValue = double.NegativeInfinity;
						double edgeMin, edgeMax;

						// Project edge and vertices lying on the edge into 1D
						if (maxNormal == 0)
						{
							for (int k = 0; k < quantity; ++k)
							{
								if (dist[k] == 0)
								{
									double projVal = info[k].y;
									if (projVal > maxValue) maxValue = projVal;
									if (projVal < minValue) minValue = projVal;
								}
							}

							if (edgeStart.y < edgeEnd.y)
							{
								edgeMin = edgeStart.y;
								edgeMax = edgeEnd.y;
							}
							else
							{
								edgeMin = edgeEnd.y;
								edgeMax = edgeStart.y;
							}
						}
						else
						{
							for (int k = 0; k < quantity; ++k)
							{
								if (dist[k] == 0)
								{
									double projVal = info[k].x;
									if (projVal > maxValue) maxValue = projVal;
									if (projVal < minValue) minValue = projVal;
								}
							}

							if (edgeStart.x < edgeEnd.x)
							{
								edgeMin = edgeStart.x;
								edgeMax = edgeEnd.x;
							}
							else
							{
								edgeMin = edgeEnd.x;
								edgeMax = edgeStart.x;
							}
						}

						double w0, w1;
						int q = Intersection.FindSegment1Segment1(minValue, maxValue, edgeMin, edgeMax, out w0, out w1);
						if (q > 0)
						{
							// Unproject values back to the edge
							if (maxNormal == 0)
							{
								info.Point0 = new Vector2D((c - edgeNormal.y * w0) / edgeNormal.x, w0);
								if (q == 2)
								{
									info.Point1 = new Vector2D((c - edgeNormal.y * w1) / edgeNormal.x, w1);
								}
							}
							else
							{
								info.Point0 = new Vector2D(w0, (c - edgeNormal.x * w0) / edgeNormal.y);
								if (q == 2)
								{
									info.Point1 = new Vector2D(w1, (c - edgeNormal.x * w1) / edgeNormal.y);
								}
							}

							info.IntersectionType = q == 1 ? IntersectionTypes.Point : IntersectionTypes.Segment;
							info.Quantity = q;
							quantity = -1; // Signal to the calling routine, that it should stop clipping the polygon
						}
						else
						{
							// No intersection between the edge and projected points
							quantity = 0;
						}
					}
				}
			}

			/// <summary>
			/// Tests if a triangle intersects another triangle (both triangles must be ordered counter clockwise). Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestTriangle2Triangle2(ref Triangle2 triangle0, ref Triangle2 triangle1)
			{
				//NOTE: due to inability to allocate arrays on stack in safe context in C#,
				// triangle vertices are separate vectors and following code has to be
				// unrolled manually (thus look ugly).

				Vector2D dir, v0, v1;

				// Test edges of triangle0 for separation.

				v0 = triangle0.V0;
				v1 = triangle0.V1;
				dir.x = v1.y - v0.y;
				dir.y = v0.x - v1.x;
				if (WhichSide(ref triangle1, ref v0, ref dir) > 0) return false;

				v0 = triangle0.V1;
				v1 = triangle0.V2;
				dir.x = v1.y - v0.y;
				dir.y = v0.x - v1.x;
				if (WhichSide(ref triangle1, ref v0, ref dir) > 0) return false;

				v0 = triangle0.V2;
				v1 = triangle0.V0;
				dir.x = v1.y - v0.y;
				dir.y = v0.x - v1.x;
				if (WhichSide(ref triangle1, ref v0, ref dir) > 0) return false;

				// Test edges of triangle1 for separation.

				v0 = triangle1.V0;
				v1 = triangle1.V1;
				dir.x = v1.y - v0.y;
				dir.y = v0.x - v1.x;
				if (WhichSide(ref triangle0, ref v0, ref dir) > 0) return false;

				v0 = triangle1.V1;
				v1 = triangle1.V2;
				dir.x = v1.y - v0.y;
				dir.y = v0.x - v1.x;
				if (WhichSide(ref triangle0, ref v0, ref dir) > 0) return false;

				v0 = triangle1.V2;
				v1 = triangle1.V0;
				dir.x = v1.y - v0.y;
				dir.y = v0.x - v1.x;
				if (WhichSide(ref triangle0, ref v0, ref dir) > 0) return false;

				return true;
			}

			/// <summary>
			/// Tests if a triangle intersects another triangle and finds intersection parameters (both triangles must be ordered counter clockwise). Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindTriangle2Triangle2(ref Triangle2 triangle0, ref Triangle2 triangle1, out Triangle2Triangle2Intr info)
			{
				// The potential intersection is initialized to triangle1.  The set of
				// vertices is refined based on clipping against each edge of triangle0.

				info = new Triangle2Triangle2Intr();
				info.Point0 = triangle1.V0;
				info.Point1 = triangle1.V1;
				info.Point2 = triangle1.V2;
				int quantity = 3;

				ClipConvexPolygonAgainstLine(ref triangle0.V2, ref triangle0.V0, ref quantity, ref info);
				if (quantity == 0) return false;	// Triangle completely clipped, no intersection occurs.
				if (quantity  < 0) return true;		// Triangle has grazing contact, no need to clip further

				ClipConvexPolygonAgainstLine(ref triangle0.V0, ref triangle0.V1, ref quantity, ref info);
				if (quantity == 0) return false;
				if (quantity  < 0) return true;

				ClipConvexPolygonAgainstLine(ref triangle0.V1, ref triangle0.V2, ref quantity, ref info);
				if (quantity == 0) return false;
				if (quantity  < 0) return true;

				info.IntersectionType = IntersectionTypes.Polygon;
				info.Quantity = quantity;
				return true;
			}
		}
	}
}
