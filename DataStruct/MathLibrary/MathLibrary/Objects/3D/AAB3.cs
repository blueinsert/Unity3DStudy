
using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Axis aligned bounding box in 3D
		/// </summary>
		public struct AAB3
		{
			/// <summary>
			/// Min point
			/// </summary>
			public Vector3D Min;

			/// <summary>
			/// Max point
			/// </summary>
			public Vector3D Max;


			/// <summary>
			/// Creates AAB from min and max points.
			/// </summary>
			public AAB3(ref Vector3D min, ref Vector3D max)
			{
				Min = min;
				Max = max;
			}

			/// <summary>
			/// Creates AAB from min and max points.
			/// </summary>
			public AAB3(Vector3D min, Vector3D max)
			{
				Min = min;
				Max = max;
			}

			/// <summary>
			/// Creates AAB. The caller must ensure that xmin &lt;= xmax, ymin &lt;= ymax and zmin &lt;= zmax.
			/// </summary>
			public AAB3(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
			{
				Min.x = xMin;
				Min.y = yMin;
				Min.z = zMin;
				Max.x = xMax;				
				Max.y = yMax;
				Max.z = zMax;
			}

            ///// <summary>
            ///// Converts AAB3 to UnityEngine.Bounds
            ///// </summary>
            //public static implicit operator Bounds(AAB3 value)
            //{
            //    // Internally Bounds store center and extents anyway, which is silly because it represents aabb...
            //    Vector3D center, extents;
            //    value.CalcCenterExtents(out center, out extents);
            //    return new Bounds() { center = center, extents = extents };
            //}

            ///// <summary>
            ///// Converts UnityEngine.Bounds to AAB3
            ///// </summary>
            //public static implicit operator AAB3(Bounds value)
            //{
            //    return new AAB3() { Min = value.min, Max = value.max };
            //}

			/// <summary>
			/// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
			/// </summary>
			public static AAB3 CreateFromPoint(ref Vector3D point)
			{
				AAB3 result;
				result.Min = point;
				result.Max = point;
				return result;
			}

			/// <summary>
			/// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
			/// </summary>
			public static AAB3 CreateFromPoint(Vector3D point)
			{
				AAB3 result;
				result.Min = point;
				result.Max = point;
				return result;
			}

			/// <summary>
			/// Computes AAB from two points extracting min and max values. In case min and max points are known, use constructor instead.
			/// </summary>
			public static AAB3 CreateFromTwoPoints(ref Vector3D point0, ref Vector3D point1)
			{
				AAB3 result;

				if (point0.x < point1.x)
				{
					result.Min.x = point0.x;
					result.Max.x = point1.x;
				}
				else
				{
					result.Min.x = point1.x;
					result.Max.x = point0.x;
				}

				if (point0.y < point1.y)
				{
					result.Min.y = point0.y;
					result.Max.y = point1.y;
				}
				else
				{
					result.Min.y = point1.y;
					result.Max.y = point0.y;
				}

				if (point0.z < point1.z)
				{
					result.Min.z = point0.z;
					result.Max.z = point1.z;
				}
				else
				{
					result.Min.z = point1.z;
					result.Max.z = point0.z;
				}

				return result;
			}

			/// <summary>
			/// Computes AAB from two points. In case min and max points are known, use constructor instead.
			/// </summary>
			public static AAB3 CreateFromTwoPoints(Vector3D point0, Vector3D point1)
			{
				return CreateFromTwoPoints(ref point0, ref point1);
			}

			/// <summary>
			/// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
			/// If a set is empty, returns new AAB3().
			/// </summary>
			public static AAB3 CreateFromPoints(IEnumerable<Vector3D> points)
			{
				IEnumerator<Vector3D> enumerator = points.GetEnumerator();
				enumerator.Reset();
				if (!enumerator.MoveNext())
				{
					return new AAB3();
				}

				AAB3 result = CreateFromPoint(enumerator.Current);
				while (enumerator.MoveNext())
				{
					result.Include(enumerator.Current);
				}
				return result;
			}

			/// <summary>
			/// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
			/// If a set is empty, returns new AAB3().
			/// </summary>
			public static AAB3 CreateFromPoints(IList<Vector3D> points)
			{
				int count = points.Count;
				if (count > 0)
				{
					AAB3 result = CreateFromPoint(points[0]);
					for (int i = 1; i < count; ++i)
					{
						result.Include(points[i]);
					}
					return result;
				}
				return new AAB3();
			}

			/// <summary>
			/// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
			/// If a set is empty, returns new AAB3().
			/// </summary>
			public static AAB3 CreateFromPoints(Vector3D[] points)
			{
				int count = points.Length;
				if (count > 0)
				{
					AAB3 result = CreateFromPoint(ref points[0]);
					for (int i = 1; i < count; ++i)
					{
						result.Include(ref points[i]);
					}
					return result;
				}
				return new AAB3();
			}

			
			/// <summary>
			/// Computes box center and extents (half sizes)
			/// </summary>
			public void CalcCenterExtents(out Vector3D center, out Vector3D extents)
			{
				center.x = 0.5f * (Max.x + Min.x);
				center.y = 0.5f * (Max.y + Min.y);
				center.z = 0.5f * (Max.z + Min.z);
				extents.x = 0.5f * (Max.x - Min.x);
				extents.y = 0.5f * (Max.y - Min.y);
				extents.z = 0.5f * (Max.z - Min.z);
			}

			/// <summary>
			/// Calculates 8 box corners.
			/// </summary>
			/// <param name="vertex0">Vector3D(Min.x, Min.y, Min.z)</param>
			/// <param name="vertex1">Vector3D(Max.x, Min.y, Min.z)</param>
			/// <param name="vertex2">Vector3D(Max.x, Max.y, Min.z)</param>
			/// <param name="vertex3">Vector3D(Min.x, Max.y, Min.z)</param>
			/// <param name="vertex4">Vector3D(Min.x, Min.y, Max.z)</param>
			/// <param name="vertex5">Vector3D(Max.x, Min.y, Max.z)</param>
			/// <param name="vertex6">Vector3D(Max.x, Max.y, Max.z)</param>
			/// <param name="vertex7">Vector3D(Min.x, Max.y, Max.z)</param>
			public void CalcVertices(
				out Vector3D vertex0, out Vector3D vertex1, out Vector3D vertex2, out Vector3D vertex3,
				out Vector3D vertex4, out Vector3D vertex5, out Vector3D vertex6, out Vector3D vertex7)
			{
				vertex0 = Min;
				vertex1 = new Vector3D(Max.x, Min.y, Min.z);
				vertex2 = new Vector3D(Max.x, Max.y, Min.z);
				vertex3 = new Vector3D(Min.x, Max.y, Min.z);
				vertex4 = new Vector3D(Min.x, Min.y, Max.z);
				vertex5 = new Vector3D(Max.x, Min.y, Max.z);
				vertex6 = Max;
				vertex7 = new Vector3D(Min.x, Max.y, Max.z);
			}

			/// <summary>
			/// Calculates 8 box corners and returns them in an allocated array.
			/// See array-less overload for the description.
			/// </summary>
			public Vector3D[] CalcVertices()
			{
				return new Vector3D[]
				{
					Min,
					new Vector3D(Max.x, Min.y, Min.z),
					new Vector3D(Max.x, Max.y, Min.z),
					new Vector3D(Min.x, Max.y, Min.z),
					new Vector3D(Min.x, Min.y, Max.z),
					new Vector3D(Max.x, Min.y, Max.z),
					Max,
					new Vector3D(Min.x, Max.y, Max.z),
				};
			}

			/// <summary>
			/// Calculates 8 box corners and fills the input array with them (array length must be 8).
			/// See array-less overload for the description.
			/// </summary>
			public void CalcVertices(Vector3D[] array)
			{
				array[0] = Min;
				array[1] = new Vector3D(Max.x, Min.y, Min.z);
				array[2] = new Vector3D(Max.x, Max.y, Min.z);
				array[3] = new Vector3D(Min.x, Max.y, Min.z);
				array[4] = new Vector3D(Min.x, Min.y, Max.z);
				array[5] = new Vector3D(Max.x, Min.y, Max.z);
				array[6] = Max;
				array[7] = new Vector3D(Min.x, Max.y, Max.z);
			}
			
			/// <summary>
			/// Returns box volume
			/// </summary>
			public double CalcVolume()
			{
				return (Max.x - Min.x) * (Max.y - Min.y) * (Max.z - Min.z);
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector3D point)
			{
				return Distance.Point3AAB3(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector3D Project(Vector3D point)
			{
				Vector3D result;
				Distance.SqrPoint3AAB3(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Tests whether a point is contained by the aab
			/// </summary>
			public bool Contains(ref Vector3D point)
			{
				if (point.x < Min.x) return false;
				if (point.x > Max.x) return false;
				if (point.y < Min.y) return false;
				if (point.y > Max.y) return false;
				if (point.z < Min.z) return false;
				if (point.z > Max.z) return false;
				return true;
			}

			/// <summary>
			/// Tests whether a point is contained by the aab
			/// </summary>
			public bool Contains(Vector3D point)
			{
				if (point.x < Min.x) return false;
				if (point.x > Max.x) return false;
				if (point.y < Min.y) return false;
				if (point.y > Max.y) return false;
				if (point.z < Min.z) return false;
				if (point.z > Max.z) return false;
				return true;
			}

			/// <summary>
			/// Enlarging the aab to include the point. If the point is inside the AAB does nothing.
			/// </summary>
			public void Include(ref Vector3D point)
			{
				if (point.x < Min.x)
				{
					Min.x = point.x;
				}
				else if (point.x > Max.x)
				{
					Max.x = point.x;
				}
				if (point.y < Min.y)
				{
					Min.y = point.y;
				}
				else if (point.y > Max.y)
				{
					Max.y = point.y;
				}
				if (point.z < Min.z)
				{
					Min.z = point.z;
				}
				else if (point.z > Max.z)
				{
					Max.z = point.z;
				}
			}

			/// <summary>
			/// Enlarging the aab to include the point. If the point is inside the AAB does nothing.
			/// </summary>
			public void Include(Vector3D point)
			{
				if (point.x < Min.x)
				{
					Min.x = point.x;
				}
				else if (point.x > Max.x)
				{
					Max.x = point.x;
				}
				if (point.y < Min.y)
				{
					Min.y = point.y;
				}
				else if (point.y > Max.y)
				{
					Max.y = point.y;
				}
				if (point.z < Min.z)
				{
					Min.z = point.z;
				}
				else if (point.z > Max.z)
				{
					Max.z = point.z;
				}
			}

			/// <summary>
			/// Enlarges the aab so it includes another aab.
			/// </summary>
			public void Include(ref AAB3 box)
			{
				Include(ref box.Min);
				Include(ref box.Max);
			}

			/// <summary>
			/// Enlarges the aab so it includes another aab.
			/// </summary>
			public void Include(AAB3 box)
			{
				Include(ref box.Min);
				Include(ref box.Max);
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[Min: {0} Max: {1}]", Min.ToStringEx(), Max.ToStringEx());
			}
		}
	}
}
