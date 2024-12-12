

namespace Dest.Math
{
	/// <summary>
	/// Contains information about intersection of Ray2 and general Polygon2
	/// </summary>
	public struct Ray2Polygon2Intr
	{
		/// <summary>
		/// Equals to IntersectionTypes.Point or IntersectionTypes.Segment (ray and some polygon segment are collinear and overlap in more than one point)
		/// if intersection occured otherwise IntersectionTypes.Empty
		/// </summary>
		public IntersectionTypes IntersectionType;

		/// <summary>
		/// In case of IntersectionTypes.Point constains single point of intersection.
		/// In case of IntersectionTypes.Segment contains first point of intersection.
		/// Otherwise Vector2D.zero.
		/// </summary>
		public Vector2D Point0;

		/// <summary>
		/// In case of IntersectionTypes.Segment contains second point of intersection.
		/// Otherwise Vector2D.zero.
		/// </summary>
		public Vector2D Point1;

		/// <summary>
		/// In case of IntersectionTypes.Point contains evaluation parameter of single
		/// intersection point according to ray.
		/// In case of IntersectionTypes.Segment contains evaluation parameter of the
		/// first intersection point according to ray.
		/// Otherwise 0.
		/// </summary>
		public double Parameter0;

		/// <summary>
		/// In case of IntersectionTypes.Segment contains evaluation parameter of the
		/// second intersection point according to ray.
		/// Otherwise 0.
		/// </summary>
		public double Parameter1;
	}

	public static partial class Intersection
	{
		/// <summary>
		/// Tests if a ray intersects a polygon. Returns true if intersection occurs false otherwise.
		/// </summary>
		public static bool TestRay2Polygon2(ref Ray2 ray, Polygon2 polygon)
		{
			Edge2[] edges = polygon.Edges;
			int edgeCount = edges.Length;

			for (int i = 0; i < edgeCount; ++i)
			{
				Segment2 edgeSegment = new Segment2(ref edges[i].Point0, ref edges[i].Point1);

				if (Intersection.TestRay2Segment2(ref ray, ref edgeSegment))
				{
					// Return as soon as we have intersection with one of the edges
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Tests if a ray intersects a segment array. Returns true if intersection occurs false otherwise.
		/// Using this method allows to pass non-closed polyline instead of a polygon. Also if you
		/// have static polygon which is queried often, it is better to convert polygon to Segment2 array
		/// once and then call this method. Overload which accepts a polygon will convert edges to Segment2
		/// every time, while this overload simply accepts Segment2 array and avoids this overhead.
		/// </summary>
		public static bool TestRay2Polygon2(ref Ray2 ray, Segment2[] segments)
		{
			int edgeCount = segments.Length;

			for (int i = 0; i < edgeCount; ++i)
			{
				if (Intersection.TestRay2Segment2(ref ray, ref segments[i]))
				{
					// Return as soon as we have intersection with one of the edges
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Tests if a ray intersects a polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
		/// </summary>
		public static bool FindRay2Polygon2(ref Ray2 ray, Polygon2 polygon, out Ray2Polygon2Intr info)
		{
			Edge2[] edges = polygon.Edges;
			int edgeCount = edges.Length;
			Ray2Segment2Intr tempInfo;
			Ray2Segment2Intr closestIntersection = new Ray2Segment2Intr();
			double minParam = double.PositiveInfinity;

			for (int i = 0; i < edgeCount; ++i)
			{
				Segment2 edgeSegment = new Segment2(edges[i].Point0, edges[i].Point1);
				if (Intersection.FindRay2Segment2(ref ray, ref edgeSegment, out tempInfo))
				{
					if (tempInfo.Parameter0 < minParam)
					{
						if (tempInfo.IntersectionType == IntersectionTypes.Segment)
						{
							minParam = tempInfo.Parameter0;
							closestIntersection = tempInfo;
						}
						else
						{
							if (minParam - tempInfo.Parameter0 > Mathex.ZeroTolerance)
							{
								minParam = tempInfo.Parameter0;
								closestIntersection = tempInfo;
							}
						}
					}
				}
			}

			if (minParam != double.PositiveInfinity)
			{
				info.IntersectionType = closestIntersection.IntersectionType;
				info.Point0           = closestIntersection.Point0;
				info.Point1           = closestIntersection.Point1;
				info.Parameter0       = closestIntersection.Parameter0;
				info.Parameter1       = closestIntersection.Parameter1;
				return true;
			}

			info = new Ray2Polygon2Intr();
			return false;
		}

		/// <summary>
		/// Tests if a ray intersects a polygon and finds intersection parameters. Returns true if intersection occurs false otherwise.
		/// Using this method allows to pass non-closed polyline instead of a polygon. Also if you
		/// have static polygon which is queried often, it is better to convert polygon to Segment2 array
		/// once and then call this method. Overload which accepts a polygon will convert edges to Segment2
		/// every time, while this overload simply accepts Segment2 array and avoids this overhead.
		/// </summary>
		public static bool FindRay2Polygon2(ref Ray2 ray, Segment2[] segments, out Ray2Polygon2Intr info)
		{
			int edgeCount = segments.Length;
			Ray2Segment2Intr tempInfo;
			Ray2Segment2Intr closestIntersection = new Ray2Segment2Intr();
			double minParam = double.PositiveInfinity;

			for (int i = 0; i < edgeCount; ++i)
			{
				if (Intersection.FindRay2Segment2(ref ray, ref segments[i], out tempInfo))
				{
					if (tempInfo.Parameter0 < minParam)
					{
						if (tempInfo.IntersectionType == IntersectionTypes.Segment)
						{
							minParam = tempInfo.Parameter0;
							closestIntersection = tempInfo;
						}
						else
						{
							if (minParam - tempInfo.Parameter0 > Mathex.ZeroTolerance)
							{
								minParam = tempInfo.Parameter0;
								closestIntersection = tempInfo;
							}
						}
					}
				}
			}

			if (minParam != double.PositiveInfinity)
			{
				info.IntersectionType = closestIntersection.IntersectionType;
				info.Point0 = closestIntersection.Point0;
				info.Point1 = closestIntersection.Point1;
				info.Parameter0 = closestIntersection.Parameter0;
				info.Parameter1 = closestIntersection.Parameter1;
				return true;
			}

			info = new Ray2Polygon2Intr();
			return false;
		}
	}
}
