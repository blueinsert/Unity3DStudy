using Dest.Math;
using System.Collections.Generic;

namespace Dest.Math
{
	internal class ConcaveHull2
	{
		private struct Edge
		{
			public int  V0;
			public int  V1;

			public Edge(int v0, int v1)
			{
				V0 = v0;
				V1 = v1;
			}
		}

		private struct InnerPoint
		{
			public double AverageDistance;
			public double Distance0;
			public double Distance1;
			public int   Index;
		}

		private static void Quicksort(InnerPoint[] x, int first, int last)
		{
			int i, j, pivot;
			InnerPoint temp;

			if (first < last)
			{
				pivot = first;
				i = first;
				j = last;

				while (i < j)
				{
					while (x[i].AverageDistance <= x[pivot].AverageDistance && i < last)
						i++;
					
					while (x[j].AverageDistance > x[pivot].AverageDistance)
						j--;

					if (i < j)
					{
						temp = x[i];
						x[i] = x[j];
						x[j] = temp;
					}
				}

				temp = x[pivot];
				x[pivot] = x[j];
				x[j] = temp;
				Quicksort(x, first, j - 1);
				Quicksort(x, j + 1, last);
			}
		}

		private static double CalcDistanceFromPointToEdge(ref Vector2D pointA, ref Vector2D v0, ref Vector2D v1)
		{
			double tmp0, tmp1;

			tmp0 = v0.x - pointA.x;
			tmp1 = v0.y - pointA.y;
			double a = tmp0 * tmp0 + tmp1 * tmp1;

			tmp0 = v1.x - pointA.x;
			tmp1 = v1.y - pointA.y;
			double b = tmp0 * tmp0 + tmp1 * tmp1;

			tmp0 = v0.x - v1.x;
			tmp1 = v0.y - v1.y;
			double c = tmp0 * tmp0 + tmp1 * tmp1;

			if (a < b)
			{
				double tmp = a;
				a = b;
				b = tmp;
			}

			if (a > b + c || c < Mathex.ZeroTolerance)
			{
				return System.Math.Sqrt(b);
			}
			else
			{
				double t = v0.x * v1.y - v1.x * v0.y + v1.x * pointA.y - pointA.x * v1.y + pointA.x * v0.y - v0.x * pointA.y;
				return System.Math.Abs(t) / System.Math.Sqrt(c);
			}
		}

		public static bool Create(Vector2D[] points, out int[] concaveHull, int[] convexHull, double N, double epsilon = Mathex.ZeroTolerance)
		{
			// Source paper:
			// "A New Concave Hull Algorithm and Concaveness Measure for n-dimensional Datasets"
			// JIN-SEO PARK and SE-JONG OH

			//UnityEngine.Profiler.BeginSample("Prepare");
			LinkedList<Edge> hull = new LinkedList<Edge>();
			int convexHullLength = convexHull.Length;
			HashSet<int> availableIndices = new HashSet<int>();
			int i = 0;
			int pointsCount = points.Length;
			for (i = 0; i < pointsCount; ++i)
			{
				availableIndices.Add(i);
			}
			int index;
			for (int i0 = convexHullLength - 1, i1 = 0; i1 < convexHullLength; i0 = i1, ++i1)
			{
				index = convexHull[i1];
				hull.AddLast(new Edge(convexHull[i0], index));
				availableIndices.Remove(index);
			}
			InnerPoint[] innerPoints = new InnerPoint[availableIndices.Count];
			//UnityEngine.Profiler.EndSample();

			//UnityEngine.Profiler.BeginSample("Dig");
			// Do digging
			LinkedListNode<Edge> hullSegment = hull.First;
			int innerPointsLength;
			double tmpX, tmpY, distance0, distance1, averageDistance;
			while (hullSegment != null)
			{
				if (availableIndices.Count == 0) break; // Got no more candidates for digging

				//UnityEngine.Profiler.BeginSample("0");
				int ci0 = hullSegment.Value.V0;
				int ci1 = hullSegment.Value.V1;
				Vector2D v0 = points[ci0];
				Vector2D v1 = points[ci1];

				// Calc average distance from the edge to every available point
				innerPointsLength = 0;
				foreach (int availableIndex in availableIndices)
				{
					Vector2D point = points[availableIndex];

					tmpX = point.x - v0.x;
					tmpY = point.y - v0.y;
					distance0 = System.Math.Sqrt(tmpX * tmpX + tmpY * tmpY);
					tmpX = point.x - v1.x;
					tmpY = point.y - v1.y;
					distance1 = System.Math.Sqrt(tmpX * tmpX + tmpY * tmpY);
					averageDistance = (distance0 + distance1) * .5f;

					InnerPoint ip = new InnerPoint();
					ip.Distance0 = distance0;
					ip.Distance1 = distance1;
					ip.AverageDistance = averageDistance;
					ip.Index = availableIndex;
					innerPoints[innerPointsLength] = ip;
					
					++innerPointsLength;
				}
				Quicksort(innerPoints, 0, innerPointsLength - 1);
				//UnityEngine.Profiler.EndSample();

				//UnityEngine.Profiler.BeginSample("1");
				// As innerPoints is sorted, go from smallest distance to largest
				//Segment2 edgeSegment = new Segment2(ref v0, ref v1);
				InnerPoint nearesPoint = new InnerPoint();
				bool gotPoint = false;
				for (int k = 0, len = innerPointsLength; k < len; ++k)
				{
					InnerPoint innerPoint = innerPoints[k];
					Vector2D point = points[innerPoint.Index];
					int nearestEdgeIndex = innerPoint.Distance0 < innerPoint.Distance1 ? ci0 : ci1;

					// Find adjacent segment
					//TODO rework base data to include adjacency (will also help on sorting stage)
					LinkedListNode<Edge> segment = hull.First;
					LinkedListNode<Edge> adjacent = null;
					while (segment != null)
					{
						if (segment != hullSegment)
						{
							if (segment.Value.V0 == nearestEdgeIndex || segment.Value.V1 == nearestEdgeIndex)
							{
								adjacent = segment;
								break;
							}
						}
						segment = segment.Next;
					}

					// Assert
					//if (adjacent == null)
					//{
					//	Logger.LogError("Can't find adjacent edge to current edge");
					//	concaveHull = null;
					//	return false;
					//}

					double distSqrToEdge = CalcDistanceFromPointToEdge(ref point, ref v0, ref v1);
					double distSqrToNeighbour = CalcDistanceFromPointToEdge(ref point, ref points[adjacent.Value.V0], ref points[adjacent.Value.V1]);

					//Segment2 neigbourSegment = new Segment2(points[adjacent.Value.V0], points[adjacent.Value.V1]);
					//double distSqrToEdge = Distance.SqrPoint2Segment2(ref point, ref edgeSegment);
					//double distSqrToNeighbour = Distance.SqrPoint2Segment2(ref point, ref neigbourSegment);

					if (distSqrToEdge < distSqrToNeighbour)
					{
						nearesPoint = innerPoint;
						gotPoint = true;
						break;
					}
				}
				//UnityEngine.Profiler.EndSample();

				//if (nearesPoint == null)
				if (!gotPoint)
				{
					hullSegment = hullSegment.Next;
					continue;
				}

				//UnityEngine.Profiler.BeginSample("2");
				double minInnerPointToEdgePointDist = nearesPoint.Distance0 < nearesPoint.Distance1 ? nearesPoint.Distance0 : nearesPoint.Distance1;
				double edgeLength = (v0 - v1).magnitude;

				if (minInnerPointToEdgePointDist > 0 &&
					edgeLength / minInnerPointToEdgePointDist > N)
				{
					LinkedListNode<Edge> edgeToDelete = hullSegment;
					hullSegment = hullSegment.Next;
					hull.Remove(edgeToDelete);
					int k = nearesPoint.Index;
					hull.AddLast(new Edge(ci0, k));
					hull.AddLast(new Edge(k, ci1));
					availableIndices.Remove(k);
				}
				else
				{
					hullSegment = hullSegment.Next;
				}
				//UnityEngine.Profiler.EndSample();
			}
			//UnityEngine.Profiler.EndSample();

			//UnityEngine.Profiler.BeginSample("Result");
			// Sort the hull (connect adjacent edges)
			LinkedListNode<Edge> sortedNode = hull.First;
			bool process;
			do
			{
				process = false;
				LinkedListNode<Edge> node = sortedNode.Next;
				while (node != null)
				{
					if (sortedNode.Value.V1 == node.Value.V0)
					{
						// Found adjacent edges, bring them together
						hull.Remove(node);
						hull.AddAfter(sortedNode, node);
						sortedNode = node;
						process = true;
						break;
					}
					node = node.Next;
				}
			}
			while (process);
			
			// Get indices out of edges
			concaveHull = new int[hull.Count];
			i = 0;
			foreach (Edge edge in hull)
			{
				concaveHull[i] = edge.V0;
				++i;
			}
			//UnityEngine.Profiler.EndSample();

			return true;
		}
	}

	public static class ConcaveHull
	{
		public static bool Create2D(Vector2D[] points, out int[] concaveHull, out int[] convexHull, double algorithmThreshold, double epsilon = Mathex.ZeroTolerance)
		{
			if (algorithmThreshold <= 0)
			{
				concaveHull = convexHull = null;
				return false;
			}

			//UnityEngine.Profiler.BeginSample("CreateConvex");
			int dim;
			bool convexResult = ConvexHull.Create2D(points, out convexHull, out dim, epsilon);
			//UnityEngine.Profiler.EndSample();

			if (!convexResult)
			{
				concaveHull = convexHull = null;
				return false;
			}

			if (dim != 2)
			{
				concaveHull = convexHull = null;
				return false;
			}

			//UnityEngine.Profiler.BeginSample("CreateConcave");
			bool result = ConcaveHull2.Create(points, out concaveHull, convexHull, algorithmThreshold, epsilon);
			//UnityEngine.Profiler.EndSample();
			if (!result)
			{
				convexHull = null;
			}
			return result;
		}

		public static bool Create2D(Vector2D[] points, out int[] concaveHull, double algorithmThreshold, double epsilon = Mathex.ZeroTolerance)
		{
			int[] convexHull;
			return Create2D(points, out concaveHull, out convexHull, algorithmThreshold, epsilon);
		}
	}
}
