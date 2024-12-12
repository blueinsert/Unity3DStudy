

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line2 and Triangle2
		/// </summary>
		public struct Line2Triangle2Intr
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
			private static void TriangleLineRelations(ref Vector2D origin  , ref Vector2D direction, ref Triangle2 triangle,
				out double dist0   , out double dist1   , out double dist2,
				out int   sign0   , out int   sign1   , out int   sign2,
				out int   positive, out int   negative, out int   zero)
			{
				positive = 0;
				negative = 0;
				zero     = 0;
				
				Vector2D diff = triangle.V0 - origin;
				dist0 = diff.DotPerp(direction);
				if (dist0 > Mathex.ZeroTolerance)
				{
					sign0 = 1;
					++positive;
				}
				else if (dist0 < -Mathex.ZeroTolerance)
				{
					sign0 = -1;
					++negative;
				}
				else
				{
					dist0 = 0.0f;
					sign0 = 0;
					++zero;
				}

				diff = triangle.V1 - origin;
				dist1 = diff.DotPerp(direction);
				if (dist1 > Mathex.ZeroTolerance)
				{
					sign1 = 1;
					++positive;
				}
				else if (dist1 < -Mathex.ZeroTolerance)
				{
					sign1 = -1;
					++negative;
				}
				else
				{
					dist1 = 0.0f;
					sign1 = 0;
					++zero;
				}

				diff = triangle.V2 - origin;
				dist2 = diff.DotPerp(direction);
				if (dist2 > Mathex.ZeroTolerance)
				{
					sign2 = 1;
					++positive;
				}
				else if (dist2 < -Mathex.ZeroTolerance)
				{
					sign2 = -1;
					++negative;
				}
				else
				{
					dist2 = 0.0f;
					sign2 = 0;
					++zero;
				}
			}

			private static bool GetInterval(ref Vector2D origin, ref Vector2D direction, ref Triangle2 triangle,
				double     dist0, double dist1, double dist2,
				int       sign0, int   sign1, int   sign2,
				out double param0, out double param1)
			{
				// Project triangle onto line.
				Vector2D diff = triangle.V0 - origin;
				double proj0 = direction.Dot(diff);
				
				diff = triangle.V1 - origin;
				double proj1 = direction.Dot(diff);
				
				diff = triangle.V2 - origin;
				double proj2 = direction.Dot(diff);

				param0 = 0.0f;
				param1 = 0.0f;

				// Compute transverse intersections of triangle edges with line.
				int quantity = 0;
				if (sign2 * sign0 < 0)
				{
					param0 = (dist2 * proj0 - dist0 * proj2) / (dist2 - dist0);
					++quantity;
				}
				if (sign0 * sign1 < 0)
				{
					if (quantity == 0) param0 = (dist0 * proj1 - dist1 * proj0) / (dist0 - dist1);
					else               param1 = (dist0 * proj1 - dist1 * proj0) / (dist0 - dist1);
					++quantity;
				}
				if (sign1 * sign2 < 0)
				{
					if (quantity > 1) return true; // Too many intersections
					if (quantity == 0) param0 = (dist1 * proj2 - dist2 * proj1) / (dist1 - dist2);
					else               param1 = (dist1 * proj2 - dist2 * proj1) / (dist1 - dist2);
					++quantity;
				}
				
				// Check for grazing contact.
				if (quantity < 2)
				{
					if (sign0 == 0)
					{						
						if (quantity > 1) return true;
						if (quantity == 0) param0 = proj0;
						else               param1 = proj0;
						quantity++;
					}
					if (sign1 == 0)
					{
						if (quantity > 1) return true;
						if (quantity == 0) param0 = proj1;
						else               param1 = proj1;
						quantity++;
					}
					if (sign2 == 0)
					{
						if (quantity > 1) return true;
						if (quantity == 0) param0 = proj2;
						else               param1 = proj2;
						quantity++;
					}
				}


				if (quantity < 1) return true; // Need at least one intersection

				// Sort.
				if (quantity == 2)
				{
					if (param0 > param1)
					{
						double temp = param0;
						param0 = param1;
						param1 = temp;
					}
				}
				else // quantity == 1
				{
					param1 = param0;
				}

				return false;
			}


			/// <summary>
			/// Tests whether line and triangle intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine2Triangle2(ref Line2 line, ref Triangle2 triangle, out IntersectionTypes intersectionType)
			{
				double dist0   , dist1   , dist2;
				int   sign0   , sign1   , sign2;
				int   positive, negative, zero;

				TriangleLineRelations(ref line.Center, ref line.Direction, ref triangle,
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
					bool error = GetInterval(ref line.Center, ref line.Direction, ref triangle,
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
						int quantity = Intersection.FindSegment1Segment1(param0, param1, double.NegativeInfinity, double.PositiveInfinity, out w0, out w1);

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
			/// Tests whether line and triangle intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine2Triangle2(ref Line2 line, ref Triangle2 triangle)
			{
				IntersectionTypes intersectionType;
				return TestLine2Triangle2(ref line, ref triangle, out intersectionType);
			}

			/// <summary>
			/// Tests whether line and triangle intersect and finds actual intersection parameters.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine2Triangle2(ref Line2 line, ref Triangle2 triangle, out Line2Triangle2Intr info)
			{
				double dist0   , dist1   , dist2;
				int   sign0   , sign1   , sign2;
				int   positive, negative, zero;

				TriangleLineRelations(ref line.Center, ref line.Direction, ref triangle,
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
					bool error = GetInterval(ref line.Center, ref line.Direction, ref triangle,
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
						info.Quantity = Intersection.FindSegment1Segment1(param0, param1, double.NegativeInfinity, double.PositiveInfinity, out w0, out w1);

						if (info.Quantity == 2)
						{
							// Segment intersection.
							info.IntersectionType = IntersectionTypes.Segment;
							info.Point0           = line.Center + w0 * line.Direction;
							info.Point1           = line.Center + w1 * line.Direction;
						}
						else if (info.Quantity == 1)
						{
							// Point intersection.
							info.IntersectionType = IntersectionTypes.Point;
							info.Point0           = line.Center + w0 * line.Direction;
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
