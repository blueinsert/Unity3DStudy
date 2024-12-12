
using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The sphere is represented as |X-C| = R where C is the center and R is
		/// the radius.
		/// </summary>
		public struct Sphere3
		{
			private const double _4div3mulPi = 4f / 3f * System.Math.PI;

			/// <summary>
			/// Circle center
			/// </summary>
			public Vector3D Center;

			/// <summary>
			/// Circle radius
			/// </summary>
			public double Radius;


			/// <summary>
			/// Creates Sphere3 from center and radius
			/// </summary>
			public Sphere3(ref Vector3D center, double radius)
			{
				Center = center;
				Radius = radius;
			}

			/// <summary>
			/// Creates Sphere3 from center and radius
			/// </summary>
			public Sphere3(Vector3D center, double radius)
			{
				Center = center;
				Radius = radius;
			}

			/// <summary>
			/// Computes bounding sphere from a set of points.
			/// First compute the axis-aligned bounding box of the points, then compute the sphere containing the box.
			/// If a set is empty returns new Sphere3().
			/// </summary>
			public static Sphere3 CreateFromPointsAAB(IEnumerable<Vector3D> points)
			{
				IEnumerator<Vector3D> enumerator = points.GetEnumerator();
				enumerator.Reset();
				if (!enumerator.MoveNext())
				{
					return new Sphere3();
				}

				AAB3 aab = AAB3.CreateFromPoints(points);
				Vector3D center, extents;
				aab.CalcCenterExtents(out center, out extents);

				Sphere3 result;
				result.Center = center;
				result.Radius = extents.magnitude;
				return result;
			}

			/// <summary>
			/// Computes bounding sphere from a set of points.
			/// First compute the axis-aligned bounding box of the points, then compute the sphere containing the box.
			/// If a set is empty returns new Sphere3().
			/// </summary>
			public static Sphere3 CreateFromPointsAAB(IList<Vector3D> points)
			{
				if (points.Count == 0)
				{
					return new Sphere3();
				}

				AAB3 aab = AAB3.CreateFromPoints(points);
				Vector3D center, extents;
				aab.CalcCenterExtents(out center, out extents);

				Sphere3 result;
				result.Center = center;
				result.Radius = extents.magnitude;
				return result;
			}

			/// <summary>
			/// Computes bounding sphere from a set of points.
			/// Compute the smallest sphere whose center is the average of a point set.
			/// If a set is empty returns new Sphere3().
			/// </summary>
			public static Sphere3 CreateFromPointsAverage(IEnumerable<Vector3D> points)
			{
				IEnumerator<Vector3D> enumerator = points.GetEnumerator();
				enumerator.Reset();
				if (!enumerator.MoveNext())
				{
					return new Sphere3();
				}

				Vector3D center = enumerator.Current;
				int pointsCount = 1;
				while (enumerator.MoveNext())
				{
					center += enumerator.Current;
					++pointsCount;
				}
				center /= (double)pointsCount;

				double maxRadiusSqr = 0f;
				foreach (Vector3D point in points)
				{
					Vector3D diff = point - center;
					double radiusSqr = diff.sqrMagnitude;
					if (radiusSqr > maxRadiusSqr)
					{
						maxRadiusSqr = radiusSqr;
					}
				}

				Sphere3 result;
				result.Center = center;
				result.Radius = System.Math.Sqrt(maxRadiusSqr);
				return result;
			}

			/// <summary>
			/// Computes bounding sphere from a set of points.
			/// Compute the smallest sphere whose center is the average of a point set.
			/// If a set is empty returns new Sphere3().
			/// </summary>
			public static Sphere3 CreateFromPointsAverage(IList<Vector3D> points)
			{
				int pointsCount = points.Count;
				if (pointsCount == 0)
				{
					return new Sphere3();
				}

				Vector3D center = points[0];
				for (int i = 1; i < pointsCount; ++i)
				{
					center += points[i];
				}
				center /= (double)pointsCount;

				double maxRadiusSqr = 0f;
				for (int i = 0; i < pointsCount; ++i)
				{
					Vector3D diff = points[i] - center;
					double radiusSqr = diff.sqrMagnitude;
					if (radiusSqr > maxRadiusSqr)
					{
						maxRadiusSqr = radiusSqr;
					}
				}

				Sphere3 result;
				result.Center = center;
				result.Radius = System.Math.Sqrt(maxRadiusSqr);
				return result;
			}

			/// <summary>
			/// Creates sphere which is circumscribed around tetrahedron.
			/// Returns 'true' if sphere has been constructed, 'false' otherwise (input points are linearly dependent).
			/// </summary>
			public static bool CreateCircumscribed(Vector3D v0, Vector3D v1, Vector3D v2, Vector3D v3, out Sphere3 sphere)
			{
				Vector3D E10 = v1 - v0;
				Vector3D E20 = v2 - v0;
				Vector3D E30 = v3 - v0;

				double[,] A =
				{
					{ E10.x, E10.y, E10.z },
					{ E20.x, E20.y, E20.z },
					{ E30.x, E30.y, E30.z }
				};

				double[] B =
				{
					0.5f * E10.sqrMagnitude,
					0.5f * E20.sqrMagnitude,
					0.5f * E30.sqrMagnitude
				};

				Vector3D solution;
				if (LinearSystem.Solve3(A, B, out solution))
				{
					sphere.Center = v0 + solution;
					sphere.Radius = solution.magnitude;
					return true;
				}

				sphere = new Sphere3();
				return false;
			}

			/// <summary>
			/// Creates sphere which is insribed into tetrahedron.
			/// Returns 'true' if sphere has been constructed, 'false' otherwise (input points are linearly dependent).
			/// </summary>
			public static bool CreateInscribed(Vector3D v0, Vector3D v1, Vector3D v2, Vector3D v3, out Sphere3 sphere)
			{
				// Edges.
				Vector3D E10 = v1 - v0;
				Vector3D E20 = v2 - v0;
				Vector3D E30 = v3 - v0;
				Vector3D E21 = v2 - v1;
				Vector3D E31 = v3 - v1;

				// Normals.
				Vector3D N0 = E31.Cross(E21);
				Vector3D N1 = E20.Cross(E30);
				Vector3D N2 = E30.Cross(E10);
				Vector3D N3 = E10.Cross(E20);

				// Normalize the normals.
				if (System.Math.Abs(Vector3Dex.Normalize(ref N0)) < Mathex.ZeroTolerance)
				{
					sphere = new Sphere3();
					return false;
				}
				if (System.Math.Abs(Vector3Dex.Normalize(ref N1)) < Mathex.ZeroTolerance)
				{
					sphere = new Sphere3();
					return false;
				}
				if (System.Math.Abs(Vector3Dex.Normalize(ref N2)) < Mathex.ZeroTolerance)
				{
					sphere = new Sphere3();
					return false;
				}
				if (System.Math.Abs(Vector3Dex.Normalize(ref N3)) < Mathex.ZeroTolerance)
				{
					sphere = new Sphere3();
					return false;
				}

				double[,] A =
				{
					{ N1.x - N0.x, N1.y - N0.y, N1.z - N0.z },
					{ N2.x - N0.x, N2.y - N0.y, N2.z - N0.z },
					{ N3.x - N0.x, N3.y - N0.y, N3.z - N0.z }
				};

				double[] B =
				{
					0f,
					0f,
					-N3.Dot(E30)
				};

				Vector3D solution;
				if (LinearSystem.Solve3(A, B, out solution))
				{
					sphere.Center = v3 + solution;
					sphere.Radius = System.Math.Abs(N0.Dot(solution));
					return true;
				}

				sphere = new Sphere3();
				return false;
			}

			
			/// <summary>
			/// Returns sphere area
			/// </summary>
			public double CalcArea()
			{
				return 4f * System.Math.PI * Radius * Radius;
			}

			/// <summary>
			/// Returns sphere volume
			/// </summary>
			public double CalcVolume()
			{
				return _4div3mulPi * Radius * Radius * Radius;
			}

			/// <summary>
			/// Evaluates sphere using formula X = C + R*[cos(theta)*sin(phi) , sin(theta)*sin(phi) , cos(phi)],
			/// where 0 &lt;= theta,phi &lt; 2*pi.
			/// </summary>
			public Vector3D Eval(double theta, double phi)
			{
				double sinPhi = System.Math.Sin(phi);
				return new Vector3D(
					Center.x + Radius * System.Math.Cos(theta) * sinPhi,
					Center.y + Radius * System.Math.Sin(theta) * sinPhi,
					Center.z + Radius * System.Math.Cos(phi));
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector3D point)
			{
				return Distance.Point3Sphere3(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector3D Project(Vector3D point)
			{
				Vector3D result;
				Distance.SqrPoint3Sphere3(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Tests whether a point is contained by the sphere
			/// </summary>
			public bool Contains(ref Vector3D point)
			{
				Vector3D diff = point - Center;
				return diff.sqrMagnitude <= Radius * Radius;
			}

			/// <summary>
			/// Tests whether a point is contained by the sphere
			/// </summary>
			public bool Contains(Vector3D point)
			{
				Vector3D diff = point - Center;
				return diff.sqrMagnitude <= Radius * Radius;
			}

			/// <summary>
			/// Enlarges the sphere so it includes another sphere.
			/// </summary>
			public void Include(ref Sphere3 sphere)
			{
				Vector3D cenDiff = sphere.Center - this.Center;
				double lenSqr = cenDiff.sqrMagnitude;
				double rDiff = sphere.Radius - this.Radius;
				double rDiffSqr = rDiff * rDiff;

				if (rDiffSqr >= lenSqr)
				{
					if (rDiff >= 0f)
					{
						this = sphere;
					}
					return;
				}

				double length = System.Math.Sqrt(lenSqr);
				if (length > Mathex.ZeroTolerance)
				{
					double coeff = (length + rDiff) / (2f * length);
					this.Center += coeff * cenDiff;
				}
				this.Radius = 0.5f * (length + this.Radius + sphere.Radius);
			}

			/// <summary>
			/// Enlarges the sphere so it includes another sphere.
			/// </summary>
			public void Include(Sphere3 sphere)
			{
				Include(ref sphere);
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
