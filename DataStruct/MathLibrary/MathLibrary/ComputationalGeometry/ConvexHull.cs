using Dest.Math;
using System.Collections.Generic;
using Real = System.Double;

namespace Dest
{
	namespace Math
	{
		internal class ConvexHull1
		{
			private class SortedVertex
			{
				public Real Value;
				public int  Index;
			}

			public static void Create(Real[] vertices, Real epsilon, out int dimension, out int[] indices)
			{
				int numVertices = vertices.Length;
				SortedVertex[] sortedArray = new SortedVertex[numVertices];
				for (int i = 0; i < numVertices; ++i)
				{
					sortedArray[i] = new SortedVertex() { Value = vertices[i], Index = i };
				}
				System.Array.Sort<SortedVertex>(sortedArray, (e1, e2) => Comparer<double>.Default.Compare(e1.Value, e2.Value));

				Real range = sortedArray[numVertices - 1].Value - sortedArray[0].Value;
				if (range >= epsilon)
				{
					dimension = 1;
					indices = new int[] { sortedArray[0].Index, sortedArray[numVertices - 1].Index };
				}
				else
				{
					dimension = 0;
					indices = new int[] { 0 };
				}
			}
		}

		internal class ConvexHull2
		{
			private class Edge
			{
				public int  V0;
				public int  V1;
				public Edge E0;
				public Edge E1;
				public int  Sign;
				public int  Time;

				public Edge(int v0, int v1)
				{
					V0 = v0;
					V1 = v1;
					Time = -1;
				}

				public int GetSign(int i, Query2 query)
				{
					if (i != Time)
					{
						Time = i;
						Sign = query.ToLine(i, V0, V1);
					}
					return Sign;
				}

				public void Insert(Edge adj0, Edge adj1)
				{
					adj0.E1 = this;
					adj1.E0 = this;
					E0 = adj0;
					E1 = adj1;
				}

				public void DeleteSelf()
				{
					if (E0 != null)
					{
						E0.E1 = null;
					}

					if (E1 != null)
					{
						E1.E0 = null;
					}
				}

				public void GetIndices(out int[] indices)
				{
					// Count the number of edge vertices and allocate the index array.
					int numIndices = 0;

					Edge current = this;
					do
					{
						++numIndices;
						current = current.E1;
					}
					while (current != this);

					indices = new int[numIndices];

					// Fill the index array.
					numIndices = 0;
					current = this;
					do
					{
						indices[numIndices] = current.V0;
						++numIndices;
						current = current.E1;
					}
					while (current != this);
				}
			}

			public static bool Create(IList<Vector2D> vertices, Real epsilon, out int dimension, out int[] indices)
			{
				Vector2Dex.Information info = Vector2Dex.GetInformation(vertices, epsilon);
				if (info == null)
				{
					dimension = -1;
					indices = null;
					return false;
				}
				int numVertices = vertices.Count;
				
				if (info.Dimension == 0)
				{
					dimension = 0;
					indices = new int[] { 0 };
					return true;
				}

				if (info.Dimension == 1)
				{
					Real[] projection = new Real[numVertices];
					Vector2D origin = info.Origin;
					Vector2D direction = info.Direction[0];
					for (int i = 0; i < numVertices; ++i)
					{
						Vector2D diff = vertices[i] - origin;
						projection[i] = direction.Dot(diff);
					}
					ConvexHull1.Create(projection, epsilon, out dimension, out indices);
					return true;
				}

				dimension = 2;
				Vector2D[] verticesCopy = new Vector2D[numVertices];
								
				// Transform the vertices to the square [0,1]^2.
				Vector2D minValue = info.Min;
				Real scale = (Real)1 / info.MaxRange;
				for (int i = 0; i < numVertices; ++i)
				{
					verticesCopy[i] = (vertices[i] - minValue) * scale;
				}

				Query2 query = new Query2(verticesCopy);

				// Initial edges
				int i0 = info.Extreme[0];
				int i1 = info.Extreme[1];
				int i2 = info.Extreme[2];
				Edge edge0;
				Edge edge1;
				Edge edge2;
				if (info.ExtremeCCW)
				{
					edge0 = new Edge(i0, i1);
					edge1 = new Edge(i1, i2);
					edge2 = new Edge(i2, i0);
				}
				else
				{
					edge0 = new Edge(i0, i2);
					edge1 = new Edge(i2, i1);
					edge2 = new Edge(i1, i0);
				}
				edge0.Insert(edge2, edge1);
				edge1.Insert(edge0, edge2);
				edge2.Insert(edge1, edge0);

				// Create hull
				Edge hull = edge0;
				for (int i = 0; i < numVertices; ++i)
				{
					if (!Update(ref hull, i, query))
					{
						dimension = -1;
						indices = null;
						return false;
					}
				}

				hull.GetIndices(out indices);
				return true;
			}

			private static bool Update(ref Edge hull, int i, Query2 query)
			{
				// Locate an edge visible to the input point (if possible).
				Edge visible = null;
				Edge current = hull;
				do
				{
					if (current.GetSign(i, query) > 0)
					{
						visible = current;
						break;
					}
					current = current.E1;
				}
				while (current != hull);

				if (visible == null)
				{
					// The point is inside the current hull; nothing to do.
					return true;
				}

				// Remove the visible edges.
				Edge adj0 = visible.E0;
				if (adj0 == null)
				{
					return false;
				}

				Edge adj1 = visible.E1;
				if (adj1 == null)
				{
					return false;
				}

				visible.DeleteSelf();

				while (adj0.GetSign(i, query) > 0)
				{
					hull = adj0;
					adj0 = adj0.E0;
					
					if (adj0 == null)
					{
						return false;
					}

					adj0.E1.DeleteSelf();
				}

				while (adj1.GetSign(i, query) > 0)
				{
					hull = adj1;
					adj1 = adj1.E1;
					
					if (adj1 == null)
					{
						return false;
					}

					adj1.E0.DeleteSelf();
				}

				// Insert the new edges formed by the input point and the end points of the polyline of invisible edges.
				Edge edge0 = new Edge(adj0.V1, i);
				Edge edge1 = new Edge(i, adj1.V0);
				edge0.Insert(adj0, edge1);
				edge1.Insert(edge0, adj1);
				hull = edge0;

				return true;
			}
		}

		internal class ConvexHull3
		{
			private class Triangle
			{
				public int      V0;
				public int      V1;
				public int      V2;
				public Triangle Adj0;
				public Triangle Adj1;
				public Triangle Adj2;
				public int      Sign;
				public int      Time;
				public bool     OnStack;

				public Triangle(int v0, int v1, int v2)
				{
					V0 = v0;
					V1 = v1;
					V2 = v2;
					Time = -1;
				}

				public Triangle GetAdj(int index)
				{
					if (index == 0) return Adj0;
					else if (index == 1) return Adj1;
					return Adj2;
				}

				public void SetAdj(int index, Triangle value)
				{
					if (index == 0) Adj0 = value;
					else if (index == 1) Adj1 = value;
					else Adj2 = value;
				}

				public int GetV(int index)
				{
					if (index == 0) return V0;
					else if (index == 1) return V1;
					return V2;
				}

				public int GetSign(int i, Query3 query)
				{
					if (i != Time)
					{
						Time = i;
						Sign = query.ToPlane(i, V0, V1, V2);
					}
					return Sign;
				}

				public void AttachTo(Triangle adj0, Triangle adj1, Triangle adj2)
				{
					// assert:  The input adjacent triangles are correctly ordered.

					Adj0 = adj0;
					Adj1 = adj1;
					Adj2 = adj2;
				}

				public int DetachFrom(int adjIndex, Triangle adj)
				{
					//assertion(0 <= adjIndex && adjIndex < 3 && Adj[adjIndex] == adj, "Invalid inputs\n");

					if (adjIndex == 0)
					{
						Adj0 = null;
					}
					else if (adjIndex == 1)
					{
						Adj1 = null;
					}
					else
					{
						Adj2 = null;
					}

					if (adj.Adj0 == this)
					{
						adj.Adj0 = null;
						return 0;
					}

					if (adj.Adj1 == this)
					{
						adj.Adj1 = null;
						return 1;
					}

					if (adj.Adj2 == this)
					{
						adj.Adj2 = null;
						return 2;
					}

					return -1;
				}
			}

			private class TerminatorData
			{
				public int      V0;
				public int      V1;
				public int      NullIndex;
				public Triangle T;

				public TerminatorData(int v0 = -1, int v1 = -1, int nullIndex = -1, Triangle tri = null)
				{
					NullIndex = nullIndex;
					T = tri;
					V0 = v0;
					V1 = v1;
				}	
			}

			public static bool Create(IList<Vector3D> vertices, Real epsilon, out int dimension, out int[] indices)
			{
				Vector3Dex.Information info = Vector3Dex.GetInformation(vertices, epsilon);
				if (info == null)
				{
					dimension = -1;
					indices = null;
					return false;
				}
				int numVertices = vertices.Count;

				if (info.Dimension == 0)
				{
					dimension = 0;
					indices = new int[] { 0 };
					return true;
				}

				if (info.Dimension == 1)
				{
					Real[] projection = new Real[numVertices];
					Vector3D origin = info.Origin;
					Vector3D direction = info.Direction[0];
					for (int i = 0; i < numVertices; ++i)
					{
						Vector3D diff = vertices[i] - origin;
						projection[i] = direction.Dot(diff);
					}
					ConvexHull1.Create(projection, epsilon, out dimension, out indices);
					return true;
				}

				if (info.Dimension == 2)
				{
					Vector2D[] projection = new Vector2D[numVertices];
					Vector3D origin = info.Origin;
					Vector3D direction0 = info.Direction[0];
					Vector3D direction1 = info.Direction[1];
					for (int i = 0; i < numVertices; ++i)
					{
						Vector3D diff = vertices[i] - origin;
						projection[i] = new Vector2D(direction0.Dot(diff), direction1.Dot(diff));
					}
					return ConvexHull2.Create(projection, epsilon, out dimension, out indices);
				}

				dimension = 3;
				Vector3D[] verticesCopy = new Vector3D[numVertices];

				// Transform the vertices to the cube [0,1]^3.
				Vector3D minValue = info.Min;
				Real scale = ((Real)1) / info.MaxRange;
				for (int i = 0; i < numVertices; ++i)
				{
					verticesCopy[i] = (vertices[i] - minValue) * scale;
				}
				
				Query3 query = new Query3(verticesCopy);
				
				Triangle tri0;
				Triangle tri1;
				Triangle tri2;
				Triangle tri3;
				int i0 = info.Extreme[0];
				int i1 = info.Extreme[1];
				int i2 = info.Extreme[2];
				int i3 = info.Extreme[3];
				if (info.ExtremeCCW)
				{
					tri0 = new Triangle(i0, i1, i3);
					tri1 = new Triangle(i0, i2, i1);
					tri2 = new Triangle(i0, i3, i2);
					tri3 = new Triangle(i1, i2, i3);
					tri0.AttachTo(tri1, tri3, tri2);
					tri1.AttachTo(tri2, tri3, tri0);
					tri2.AttachTo(tri0, tri3, tri1);
					tri3.AttachTo(tri1, tri2, tri0);
				}
				else
				{
					tri0 = new Triangle(i0, i3, i1);
					tri1 = new Triangle(i0, i1, i2);
					tri2 = new Triangle(i0, i2, i3);
					tri3 = new Triangle(i1, i3, i2);
					tri0.AttachTo(tri2, tri3, tri1);
					tri1.AttachTo(tri0, tri3, tri2);
					tri2.AttachTo(tri1, tri3, tri0);
					tri3.AttachTo(tri0, tri2, tri1);
				}

				HashSet<Triangle> hull = new HashSet<Triangle>();
				hull.Add(tri0);
				hull.Add(tri1);
				hull.Add(tri2);
				hull.Add(tri3);

				for (int i = 0; i < numVertices; ++i)
				{
					if (!Update(hull, i, query))
					{
						dimension = -1;
						indices = null;
						return false;
					}
				}				

				ExtractIndices(hull, out indices);

				return true;
			}

			private static bool Update(HashSet<Triangle> hull, int i, Query3 query)
			{
				// Locate a triangle visible to the input point (if possible).
				Triangle visible = null;
				foreach (Triangle item in hull)
				{
					if (item.GetSign(i, query) > 0)
					{
						visible = item;
						break;
					}
				}

				if (visible == null)
				{
					// The point is inside the current hull; nothing to do.
					return true;
				}

				// Locate and remove the visible triangles.
				Stack<Triangle> visibleSet = new Stack<Triangle>();
				visibleSet.Push(visible);
				visible.OnStack = true;

				Dictionary<int, TerminatorData> terminator = new Dictionary<int, TerminatorData>();
				int j, v0, v1;

				Triangle tri;
				while (visibleSet.Count != 0)
				{
					tri = visibleSet.Pop();
					tri.OnStack = false;

					for (j = 0; j < 3; ++j)
					{
						Triangle adj = tri.GetAdj(j);
						if (adj != null)
						{
							// Detach triangle and adjacent triangle from each other.
							int nullIndex = tri.DetachFrom(j, adj);

							if (adj.GetSign(i, query) > 0)
							{
								if (!adj.OnStack)
								{
									// Adjacent triangle is visible.
									visibleSet.Push(adj);
									adj.OnStack = true;
								}
							}
							else
							{
								// Adjacent triangle is invisible.
								v0 = tri.GetV(j);
								v1 = tri.GetV((j + 1) % 3);
								terminator[v0] = new TerminatorData(v0, v1, nullIndex, adj);
							}
						}
					}
					hull.Remove(tri);
				}

				// Insert the new edges formed by the input point and the terminator
				// between visible and invisible triangles.
				int size = terminator.Count;
				if (size < 3)
				{
					return false;
				}

				var enumerator = terminator.GetEnumerator();
				enumerator.MoveNext();
				KeyValuePair<int, TerminatorData> edge = enumerator.Current;
				v0 = edge.Value.V0;
				v1 = edge.Value.V1;
				tri = new Triangle(i, v0, v1);
				hull.Add(tri);

				// Save information for linking first/last inserted new triangles.
				int saveV0 = edge.Value.V0;
				Triangle saveTri = tri;

				// Establish adjacency links across terminator edge.
				tri.Adj1 = edge.Value.T;
				edge.Value.T.SetAdj(edge.Value.NullIndex, tri);
				for (j = 1; j < size; ++j)
				{
					TerminatorData edgeSecond;
					if (!terminator.TryGetValue(v1, out edgeSecond))
					{
						return false;
					}

					v0 = v1;
					v1 = edgeSecond.V1;
					Triangle next = new Triangle(i, v0, v1);
					hull.Add(next);

					// Establish adjacency links across terminator edge.
					next.Adj1 = edgeSecond.T;
					edgeSecond.T.SetAdj(edgeSecond.NullIndex, next);

					// Establish adjacency links with previously inserted triangle.
					next.Adj0 = tri;
					tri.Adj2 = next;

					tri = next;
				}

				if (v1 != saveV0)
				{
					return false;
				}

				// Establish adjacency links between first/last triangles.
				saveTri.Adj0 = tri;
				tri.Adj2 = saveTri;
				return true;
			}

			private static void ExtractIndices(HashSet<Triangle> hull, out int[] indices)
			{
				int numSimplices = hull.Count;
				indices = new int[3 * numSimplices];

				int i = 0;
				foreach (Triangle tri in hull)
				{
					indices[i] = tri.V0;
					++i;
					indices[i] = tri.V1;
					++i;
					indices[i] = tri.V2;
					++i;
				}
				hull.Clear();
			}
		}

		public static class ConvexHull
		{
			/// <summary>
			/// Generates 2D convex hull of the input point set. Resulting convex hull is defined by the indices parameter. Its behavior depends on the dimension parameter.
			/// If dimension is 2, then convex hull is 2D polygon and indices should be accessed as Edge=(points[indices[i]], points[indices[(i+1)%indices.Length]), for i=[0,indices.Length-1].
			/// If dimension is 1, then input point set lie on the line and covex hull is a segment, use (points[indices[0]], points[indices[1]) to access the segment.
			/// If dimension is 0, then all points in the input set are practically the same.
			/// </summary>
			/// <param name="points">Input point set whose convex hull should be calculated.</param>
			/// <param name="indices">Contains indices into point set (null if construction has failed).</param>
			/// <param name="dimension">Resulting dimension of the input set: 2, 1 or 0.</param>
			/// <param name="epsilon">Small positive number used to determine dimension of the input set.</param>
			/// <returns>True if convex hull is created, false otherwise (in case if input point set is null, contains no points or if some error has occured during construction)</returns>
			public static bool Create2D(IList<Vector2D> points, out int[] indices, out int dimension, double epsilon = Mathex.ZeroTolerance)
			{
				if (points == null || points.Count == 0)
				{
					indices = null;
					dimension = -1;
					return false;
				}
				epsilon = epsilon >= 0 ? epsilon : 0f;

				return ConvexHull2.Create(points, epsilon, out dimension, out indices);
			}

			/// <summary>
			/// Generates 3D convex hull of the input point set. Resulting convex hull is defined fo the indices parameter. Its behavior depends on the dimension parameter.
			/// If dimension is 3, then convex hull is 3D polyhedron and indices define triangles, Triangle=(points[indices[i]], points[indices[i+1]], points[indices[i+2]]), for i=[0,indices.Length-1], i+=3.
			/// If dimension is 2, then convex hull is 2D polygon and indices should be accessed as Edge=(points[indices[i]], points[indices[(i+1)%indices.Length]), for i=[0,indices.Length-1].
			/// If dimension is 1, then input point set lie on the line and covex hull is a segment, use (points[indices[0]], points[indices[1]) to access the segment.
			/// If dimension is 0, then all points in the input set are practically the same.
			/// </summary>
			/// <param name="points">Input point set whose convex hull should be calculated.</param>
			/// <param name="indices">Contains indices into point set (null if construction has failed).</param>
			/// <param name="dimension">Resulting dimension of the input set: 3, 2, 1 or 0.</param>
			/// <param name="epsilon">Small positive number used to determine dimension of the input set.</param>
			/// <returns>True if convex hull is created, false otherwise (in case if input point set is null, contains no points or if some error has occured during construction)</returns>
			public static bool Create3D(IList<Vector3D> points, out int[] indices, out int dimension, double epsilon = Mathex.ZeroTolerance)
			{
				if (points == null || points.Count == 0)
				{
					indices = null;
					dimension = -1;
					return false;
				}
				epsilon = epsilon >= 0 ? epsilon : 0f;

				return ConvexHull3.Create(points, epsilon, out dimension, out indices);
			}
		}
	}
}
