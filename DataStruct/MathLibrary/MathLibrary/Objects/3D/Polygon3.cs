

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Polygon3 edge
		/// </summary>
		public struct Edge3
		{
			/// <summary>
			/// Edge start vertex
			/// </summary>
			public Vector3D Point0;

			/// <summary>
			/// Edge end vertex
			/// </summary>
			public Vector3D Point1;

			/// <summary>
			/// Unit length direction vector
			/// </summary>
			public Vector3D Direction;

			/// <summary>
			/// Unit length normal vector
			/// </summary>
			public Vector3D Normal;

			/// <summary>
			/// Edge length
			/// </summary>
			public double Length;
		}

		/// <summary>
		/// Represents 3d planar polygon (vertex count must be >= 3).
		/// </summary>
		public class Polygon3
		{
			private Vector3D[] _vertices;
			private Edge3[]   _edges;
			private Plane3    _plane;

			/// <summary>
			/// Gets vertices array (do not change the data, use only for traversal)
			/// </summary>
			public Vector3D[] Vertices { get { return _vertices; } }

			/// <summary>
			/// Gets edges array (do not change the data, use only for traversal)
			/// </summary>
			public Edge3[] Edges { get { return _edges; } }

			/// <summary>
			/// Polygon vertex count
			/// </summary>
			public int VertexCount { get { return _vertices.Length; } }

			/// <summary>
			/// Gets or sets polygon vertex. The caller is responsible for supplying the points which lie in the polygon's plane.
			/// </summary>
			public Vector3D this[int vertexIndex] { get { return _vertices[vertexIndex]; } set { _vertices[vertexIndex] = value; } }

			/// <summary>
			/// Gets or sets polygon plane. After plane change reset all vertices manually or call ProjectVertices() to project all vertices automatically.
			/// </summary>
			public Plane3 Plane { get { return _plane; } set { _plane = value; } }


			private Polygon3()
			{
			}

			/// <summary>
			/// Creates polygon from an array of vertices (array is copied).
			/// The caller is responsible for supplying the points which lie in the polygon's plane.
			/// </summary>
			public Polygon3(Vector3D[] vertices, Plane3 plane)
			{
				_vertices = new Vector3D[vertices.Length];
				_edges = new Edge3[vertices.Length];
				System.Array.Copy(vertices, _vertices, vertices.Length);
				_plane = plane;
				UpdateEdges();
			}

			/// <summary>
			/// Creates polygon setting number of vertices. Vertices then
			/// can be filled using indexer.
			/// </summary>
			public Polygon3(int vertexCount, Plane3 plane)
			{
				_vertices = new Vector3D[vertexCount];
				_edges = new Edge3[vertexCount];
				_plane = plane;
			}


			/// <summary>
			/// Sets polygon vertex and ensures that it will lie in the plane by projecting it.
			/// </summary>
			public void SetVertexProjected(int vertexIndex, Vector3D vertex)
			{
				double signedDistance = _plane.Normal.Dot(vertex) - _plane.Constant;
				_vertices[vertexIndex] = vertex - signedDistance * _plane.Normal;
			}

			/// <summary>
			/// Projects polygon vertices onto polygon plane.
			/// </summary>
			public void ProjectVertices()
			{
				for (int i = 0, len = _vertices.Length; i < len; ++i)
				{
					double signedDistance = _plane.Normal.Dot(_vertices[i]) - _plane.Constant;
					_vertices[i] -= signedDistance * _plane.Normal;
				}
			}

			/// <summary>
			/// Returns polygon edge
			/// </summary>
			public Edge3 GetEdge(int edgeIndex)
			{
				return _edges[edgeIndex];
			}

			/// <summary>
			/// Updates all polygon edges. Use after vertex change.
			/// </summary>
			public void UpdateEdges()
			{
				int vertexCount = _vertices.Length;
				for (int i0 = vertexCount - 1, i1 = 0; i1 < vertexCount; i0 = i1, ++i1)
				{
					Vector3D direction =
						(_edges[i0].Point1 = _vertices[i1]) -
						(_edges[i0].Point0 = _vertices[i0]);
					_edges[i0].Length = Vector3Dex.Normalize(ref direction);
					_edges[i0].Direction = direction;
					_edges[i0].Normal = _plane.Normal.Cross(direction);
				}
			}

			/// <summary>
			/// Updates certain polygon edge. Use after vertex change.
			/// </summary>
			public void UpdateEdge(int edgeIndex)
			{
				Vector3D direction =
					(_edges[edgeIndex].Point1 = _vertices[(edgeIndex + 1) % _vertices.Length]) -
					(_edges[edgeIndex].Point0 = _vertices[edgeIndex]);
				_edges[edgeIndex].Length = Vector3Dex.Normalize(ref direction);
				_edges[edgeIndex].Direction = direction;
				_edges[edgeIndex].Normal = _plane.Normal.Cross(direction);
			}

			/// <summary>
			/// Returns polygon center
			/// </summary>
			public Vector3D CalcCenter()
			{
				Vector3D average = _vertices[0];
				int vertexCount = _vertices.Length;
				for (int i = 1; i < vertexCount; ++i)
				{
					average += _vertices[i];
				}
				average /= (double)vertexCount;
				return average;
			}

			/// <summary>
			/// Returns polygon perimeter length
			/// </summary>
			public double CalcPerimeter()
			{
				double perimeterLength = 0f;
				for (int i = 0, len = _edges.Length; i < len; ++i)
				{
					perimeterLength += _edges[i].Length;
				}
				return perimeterLength;
			}

			/// <summary>
			/// Returns true if polygon contains some edges which have zero angle between them.
			/// </summary>
			public bool HasZeroCorners(double threshold = Mathex.ZeroTolerance)
			{
				int edgesLength = _edges.Length;
				Vector3D edge0, edge1;
				double thr = 1f - threshold;

				for (int i0 = edgesLength - 1, i1 = 0; i1 < edgesLength; i0 = i1, ++i1)
				{
					edge0 = -_edges[i0].Direction;
					edge1 = _edges[i1].Direction;

					double dot = Vector3D.Dot(edge0, edge1);
					if (dot >= thr)
					{
						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Reverses polygon vertex order
			/// </summary>
			public void ReverseVertices()
			{
				int vertexCount = _vertices.Length;
				int limit = vertexCount / 2;
				--vertexCount;
				int index;
				Vector3D temp;
				for (int i = 0; i < limit; ++i)
				{
					temp = _vertices[i];
					index = vertexCount - i;
					_vertices[i] = _vertices[index];
					_vertices[index] = temp;
				}
				UpdateEdges();
			}

			/// <summary>
			/// Converts the polygon to segment array
			/// </summary>
			public Segment3[] ToSegmentArray()
			{
				Segment3[] result = new Segment3[_edges.Length];
				for (int i = 0, len = result.Length; i < len; ++i)
				{
					result[i] = new Segment3(_edges[i].Point0, _edges[i].Point1);
				}
				return result;
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("[VertexCount: " + _vertices.Length.ToString());
				for (int i = 0, len = _vertices.Length; i < len; ++i)
				{
					sb.Append(string.Format(" V{0}: {1}", i.ToString(), _vertices[i].ToStringEx()));
				}
				sb.Append("]");
				return sb.ToString();
			}
		}
	}
}
