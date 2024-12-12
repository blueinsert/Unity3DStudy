

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Ray2 and Triangle2
		/// </summary>
		public struct Ray2Triangle2Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point or IntersectionTypes.Segment
			/// if intersection occured otherwise IntersectionTypes.Empty
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Number of intersection points
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
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests whether ray and triangle intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay2Triangle2(ref Ray2 ray, ref Triangle2 triangle, out IntersectionTypes intersectionType)
			{
				double dist0   , dist1   , dist2;
				int   sign0   , sign1   , sign2;
				int   positive, negative, zero;

				TriangleLineRelations(ref ray.Center, ref ray.Direction, ref triangle,
					out dist0   , out dist1   , out dist2,
					out sign0   , out sign1   , out sign2,
					out positive, out negative, out zero);

				if (positive == 3 || negative == 3)
				{
					intersectionType = IntersectionTypes.Empty;
				}
				else
				{
					double param0, param1;
					bool error = GetInterval(ref ray.Center, ref ray.Direction, ref triangle,
						dist0, dist1, dist2,
						sign0, sign1, sign2,
						out param0, out param1);

					if (error)
					{
						intersectionType = IntersectionTypes.Empty;
					}
					else
					{
						double w0, w1;
						int quantity = Intersection.FindSegment1Segment1(param0, param1, 0f, double.PositiveInfinity, out w0, out w1);

						if (quantity == 2)
						{
							intersectionType = IntersectionTypes.Segment;
						}
						else if (quantity == 1)
						{
							intersectionType = IntersectionTypes.Point;
						}
						else
						{
							intersectionType = IntersectionTypes.Empty;
						}
					}
				}

				return intersectionType != IntersectionTypes.Empty;
			}

			/// <summary>
			/// Tests whether ray and triangle intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestRay2Triangle2(ref Ray2 ray, ref Triangle2 triangle)
			{
				IntersectionTypes intersectionType;
				return TestRay2Triangle2(ref ray, ref triangle, out intersectionType);
			}

			/// <summary>
			/// Tests whether ray and triangle intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindRay2Triangle2(ref Ray2 ray, ref Triangle2 triangle, out Ray2Triangle2Intr info)
			{
				double dist0   , dist1   , dist2;
				int   sign0   , sign1   , sign2;
				int   positive, negative, zero;

				TriangleLineRelations(ref ray.Center, ref ray.Direction, ref triangle,
					out dist0   , out dist1   , out dist2,
					out sign0   , out sign1   , out sign2,
					out positive, out negative, out zero);

				if (positive == 3 || negative == 3)
				{
					// No intersections.
					info.IntersectionType = IntersectionTypes.Empty;
					info.Quantity         = 0;
					info.Point0           = Vector2D.zero;
					info.Point1           = Vector2D.zero;
				}
				else
				{
					double param0, param1;
					bool error = GetInterval(ref ray.Center, ref ray.Direction, ref triangle,
						dist0, dist1, dist2,
						sign0, sign1, sign2,
						out param0, out param1);

					if (error)
					{
						info.IntersectionType = IntersectionTypes.Empty;
						info.Quantity         = 0;
						info.Point0           = Vector2D.zero;
						info.Point1           = Vector2D.zero;
					}
					else
					{
						double w0, w1;
						info.Quantity = Intersection.FindSegment1Segment1(param0, param1, 0f, double.PositiveInfinity, out w0, out w1);

						if (info.Quantity == 2)
						{
							// Segment intersection.
							info.IntersectionType = IntersectionTypes.Segment;
							info.Point0           = ray.Center + w0 * ray.Direction;
							info.Point1           = ray.Center + w1 * ray.Direction;
						}
						else if (info.Quantity == 1)
						{
							// Point intersection.
							info.IntersectionType = IntersectionTypes.Point;
							info.Point0           = ray.Center + w0 * ray.Direction;
							info.Point1           = Vector2D.zero;
						}
						else
						{
							// No intersections.
							info.IntersectionType = IntersectionTypes.Empty;
							info.Point0           = Vector2D.zero;
							info.Point1           = Vector2D.zero;	
						}
					}
				}

				return info.IntersectionType != IntersectionTypes.Empty;
			}
		}
	}
}
