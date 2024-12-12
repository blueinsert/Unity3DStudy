
using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Circle is described by the formula |X - C|^2 = r^2,
		/// where C - circle center, r - circle radius
		/// </summary>
		public struct Circle2
		{
			/// <summary>
			/// Circle center
			/// </summary>
			public Vector2D Center;

			/// <summary>
			/// Circle radius
			/// </summary>
			public double Radius;


			/// <summary>
			/// Creates circle from center and radius
			/// </summary>
			public Circle2(ref Vector2D center, double radius)
			{
				Center = center;
				Radius = radius;
			}

			/// <summary>
			/// Creates circle from center and radius
			/// </summary>
			public Circle2(Vector2D center, double radius)
			{
				Center = center;
				Radius = radius;
			}

			/// <summary>
			/// Computes bounding circle from a set of points.
			/// First compute the axis-aligned bounding box of the points, then compute the circle containing the box.
			/// If a set is empty returns new Circle2().
			/// </summary>
			public static Circle2 CreateFromPointsAAB(IEnumerable<Vector2D> points)
			{
				IEnumerator<Vector2D> enumerator = points.GetEnumerator();
				enumerator.Reset();
				if (!enumerator.MoveNext())
				{
					return new Circle2();
				}

				AAB2 aab = AAB2.CreateFromPoints(points);
				Vector2D center, extents;
				aab.CalcCenterExtents(out center, out extents);

				Circle2 result;
				result.Center = center;
				result.Radius = extents.magnitude;
				return result;
			}

			/// <summary>
			/// Computes bounding circle from a set of points.
			/// First compute the axis-aligned bounding box of the points, then compute the circle containing the box.
			/// If a set is empty returns new Circle2().
			/// </summary>
			public static Circle2 CreateFromPointsAAB(IList<Vector2D> points)
			{
				if (points.Count == 0)
				{
					return new Circle2();
				}

				AAB2 aab = AAB2.CreateFromPoints(points);
				Vector2D center, extents;
				aab.CalcCenterExtents(out center, out extents);
				
				Circle2 result;
				result.Center = center;
				result.Radius = extents.magnitude;
				return result;
			}

			/// <summary>
			/// Computes bounding circle from a set of points.
			/// Compute the smallest circle whose center is the average of a point set.
			/// If a set is empty returns new Circle2().
			/// </summary>
			public static Circle2 CreateFromPointsAverage(IEnumerable<Vector2D> points)
			{
				IEnumerator<Vector2D> enumerator = points.GetEnumerator();
				enumerator.Reset();
				if (!enumerator.MoveNext())
				{
					return new Circle2();
				}

				Vector2D center = enumerator.Current;
				int pointsCount = 1;
				while (enumerator.MoveNext())
				{
					center += enumerator.Current;
					++pointsCount;
				}
				center /= (double)pointsCount;

				double maxRadiusSqr = 0f;
				foreach (Vector2D point in points)
				{
					Vector2D diff = point - center;
					double radiusSqr = diff.sqrMagnitude;
					if (radiusSqr > maxRadiusSqr)
					{
						maxRadiusSqr = radiusSqr;
					}
				}

				Circle2 result;
				result.Center = center;
				result.Radius = System.Math.Sqrt(maxRadiusSqr);
				return result;
			}

			/// <summary>
			/// Computes bounding circle from a set of points.
			/// Compute the smallest circle whose center is the average of a point set.
			/// If a set is empty returns new Circle2().
			/// </summary>
			public static Circle2 CreateFromPointsAverage(IList<Vector2D> points)
			{
				int pointsCount = points.Count;
				if (pointsCount == 0)
				{
					return new Circle2();
				}

				Vector2D center = points[0];
				for (int i = 1; i < pointsCount; ++i)
				{
					center += points[i];
				}
				center /= (double)pointsCount;

				double maxRadiusSqr = 0f;
				for (int i = 0; i < pointsCount; ++i)
				{
					Vector2D diff = points[i] - center;
					double radiusSqr = diff.sqrMagnitude;
					if (radiusSqr > maxRadiusSqr)
					{
						maxRadiusSqr = radiusSqr;
					}
				}

				Circle2 result;
				result.Center = center;
				result.Radius = System.Math.Sqrt(maxRadiusSqr);
				return result;
			}

			/// <summary>
			/// Creates circle which is circumscribed around triangle.
			/// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
			/// </summary>
			public static bool CreateCircumscribed(Vector2D v0, Vector2D v1, Vector2D v2, out Circle2 circle)
			{
				Vector2D e10 = v1 - v0;
				Vector2D e20 = v2 - v0;

				double[,] A =
				{
					{ e10.x, e10.y },
					{ e20.x, e20.y }
				};

				double[] B =
				{
					0.5f * e10.sqrMagnitude,
					0.5f * e20.sqrMagnitude
				};

				Vector2D solution;
				if (LinearSystem.Solve2(A, B, out solution))
				{
					circle.Center = v0 + solution;
					circle.Radius = solution.magnitude;
					return true;
				}

				circle = new Circle2();
				return false;
			}

			/// <summary>
			/// Creates circle which is insribed into triangle.
			/// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
			/// </summary>
			public static bool CreateInscribed(Vector2D v0, Vector2D v1, Vector2D v2, out Circle2 circle)
			{
				Vector2D d10 = v1 - v0;
				Vector2D d20 = v2 - v0;
				Vector2D d21 = v2 - v1;

				double len10 = d10.magnitude;
				double len20 = d20.magnitude;
				double len21 = d21.magnitude;
				double perimeter = len10 + len20 + len21;

				if (perimeter > Mathex.ZeroTolerance)
				{
					double inv = 1f / perimeter;
					len10 *= inv;
					len20 *= inv;
					len21 *= inv;
					circle.Center = len21 * v0 + len20 * v1 + len10 * v2;
					circle.Radius = inv * System.Math.Abs(d10.DotPerp(d20));

					if (circle.Radius > Mathex.ZeroTolerance)
					{
						return true;
					}
				}

				circle = new Circle2();
				return false;
			}

			
			/// <summary>
			/// Returns circle perimeter
			/// </summary>
			public double CalcPerimeter()
			{
				return Mathex.TwoPi * Radius;
			}

			/// <summary>
			/// Returns circle area
			/// </summary>
			public double CalcArea()
			{
				return System.Math.PI * Radius * Radius;
			}

			/// <summary>
			/// Evaluates circle using formula X = C + R*[cos(t), sin(t)]
			/// where t is an angle in [0,2*pi).
			/// </summary>
			/// <param name="t">Evaluation parameter</param>
			public Vector2D Eval(double t)
			{
				return new Vector2D(Center.x + Radius * System.Math.Cos(t), Center.y + Radius * System.Math.Sin(t));
			}

			/// <summary>
			/// Evaluates disk using formula X = C + radius*[cos(t), sin(t)]
			/// where t is an angle in [0,2*pi).
			/// </summary>
			/// <param name="t">Evaluation parameter</param>
			/// <param name="radius">Evaluation radius</param>
			public Vector2D Eval(double t, double radius)
			{
				return new Vector2D(Center.x + radius * System.Math.Cos(t), Center.y + radius * System.Math.Sin(t));
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector2D point)
			{
				return Distance.Point2Circle2(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector2D Project(Vector2D point)
			{
				Vector2D result;
				Distance.SqrPoint2Circle2(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Tests whether a point is contained by the circle
			/// </summary>
			public bool Contains(ref Vector2D point)
			{
				Vector2D diff = point - Center;
				return diff.sqrMagnitude <= Radius * Radius;
			}

			/// <summary>
			/// Tests whether a point is contained by the circle
			/// </summary>
			public bool Contains(Vector2D point)
			{
				Vector2D diff = point - Center;
				return diff.sqrMagnitude <= Radius * Radius;
			}

			/// <summary>
			/// Enlarges the circle so it includes another circle.
			/// </summary>
			public void Include(ref Circle2 circle)
			{
				Vector2D cenDiff = circle.Center - this.Center;
				double lenSqr = cenDiff.sqrMagnitude;
				double rDiff = circle.Radius - this.Radius;
				double rDiffSqr = rDiff * rDiff;

				if (rDiffSqr >= lenSqr)
				{
					if (rDiff >= 0f)
					{
						this = circle;
					}
					return;
				}

				double length = System.Math.Sqrt(lenSqr);
				if (length > Mathex.ZeroTolerance)
				{
					double coeff = (length + rDiff) / (2f * length);
					this.Center += coeff * cenDiff;
				}
				this.Radius = 0.5f * (length + this.Radius + circle.Radius);
			}

			/// <summary>
			/// Enlarges the circle so it includes another circle.
			/// </summary>
			public void Include(Circle2 circle)
			{
				Include(ref circle);
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[Center: {0} Radius: {1}]", Center.ToStringEx(), Radius.ToString());
			}
		}
	}
}
