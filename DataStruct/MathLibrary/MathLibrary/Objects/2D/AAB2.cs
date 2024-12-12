using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Axis aligned bounding box in 2D
		/// </summary>
		public struct AAB2
		{
			/// <summary>
			/// Min point
			/// </summary>
			public Vector2D Min;

			/// <summary>
			/// Max point
			/// </summary>
			public Vector2D Max;

			
			/// <summary>
			/// Creates AAB from min and max points.
			/// </summary>
			public AAB2(ref Vector2D min, ref Vector2D max)
			{
				Min = min;
				Max = max;
			}

			/// <summary>
			/// Creates AAB from min and max points.
			/// </summary>
			public AAB2(Vector2D min, Vector2D max)
			{
				Min = min;
				Max = max;
			}			

			/// <summary>
			/// Creates AAB. The caller must ensure that xmin &lt;= xmax and ymin &lt;= ymax.
			/// </summary>
			public AAB2(double xMin, double xMax, double yMin, double yMax)
			{
				Min.x = xMin;
				Min.y = yMin;
				Max.x = xMax;				
				Max.y = yMax;
			}

            ///// <summary>
            ///// Converts AAB2 to UnityEngine.Rect. Code is Rect.MinMaxRect(value.Min.x, value.Min.y, value.Max.x, value.Max.y)
            ///// </summary>
            //public static implicit operator Rect(AAB2 value)
            //{
            //    return Rect.MinMaxRect(value.Min.x, value.Min.y, value.Max.x, value.Max.y);
            //}

            ///// <summary>
            ///// Converts UnityEngine.Rect to AAB2. Code is AAB2() { Min = new Vector2D(value.xMin, value.yMin), Max = new Vector2D(value.xMax, value.yMax) }
            ///// </summary>
            //public static implicit operator AAB2(Rect value)
            //{
            //    return new AAB2() { Min = new Vector2D(value.xMin, value.yMin), Max = new Vector2D(value.xMax, value.yMax) };
            //}

			/// <summary>
			/// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
			/// </summary>
			public static AAB2 CreateFromPoint(ref Vector2D point)
			{
				AAB2 result;
				result.Min = point;
				result.Max = point;
				return result;
			}

			/// <summary>
			/// Creates AAB from single point. Min and Max are set to point. Use Include() method to grow the resulting AAB.
			/// </summary>
			public static AAB2 CreateFromPoint(Vector2D point)
			{
				AAB2 result;
				result.Min = point;
				result.Max = point;
				return result;
			}

			/// <summary>
			/// Computes AAB from two points extracting min and max values. In case min and max points are known, use constructor instead.
			/// </summary>
			public static AAB2 CreateFromTwoPoints(ref Vector2D point0, ref Vector2D point1)
			{
				AAB2 result;

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

				return result;
			}

			/// <summary>
			/// Computes AAB from two points. In case min and max points are known, use constructor instead.
			/// </summary>
			public static AAB2 CreateFromTwoPoints(Vector2D point0, Vector2D point1)
			{
				return CreateFromTwoPoints(ref point0, ref point1);
			}

			/// <summary>
			/// Computes AAB from the a of points. Method includes points from a set one by one to create the AAB.
			/// If a set is empty, returns new AAB2().
			/// </summary>
			public static AAB2 CreateFromPoints(IEnumerable<Vector2D> points)
			{
				IEnumerator<Vector2D> enumerator = points.GetEnumerator();
				enumerator.Reset();
				if (!enumerator.MoveNext())
				{
					return new AAB2();
				}

				AAB2 result = CreateFromPoint(enumerator.Current);
				while (enumerator.MoveNext())
				{
					result.Include(enumerator.Current);
				}
				return result;
			}

			/// <summary>
			/// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
			/// If a set is empty, returns new AAB2().
			/// </summary>
			public static AAB2 CreateFromPoints(IList<Vector2D> points)
			{
				int count = points.Count;
				if (count > 0)
				{
					AAB2 result = CreateFromPoint(points[0]);
					for (int i = 1; i < count; ++i)
					{
						result.Include(points[i]);
					}
					return result;
				}
				return new AAB2();
			}

			/// <summary>
			/// Computes AAB from a set of points. Method includes points from a set one by one to create the AAB.
			/// If a set is empty, returns new AAB2()
			/// </summary>
			public static AAB2 CreateFromPoints(Vector2D[] points)
			{
				int count = points.Length;
				if (count > 0)
				{
					AAB2 result = CreateFromPoint(ref points[0]);
					for (int i = 1; i < count; ++i)
					{
						result.Include(ref points[i]);
					}
					return result;
				}
				return new AAB2();
			}

			
			/// <summary>
			/// Computes box center and extents (half sizes)
			/// </summary>
			public void CalcCenterExtents(out Vector2D center, out Vector2D extents)
			{
				center.x = 0.5f * (Max.x + Min.x);
				center.y = 0.5f * (Max.y + Min.y);
				extents.x = 0.5f * (Max.x - Min.x);
				extents.y = 0.5f * (Max.y - Min.y);
			}

			/// <summary>
			/// Calculates 4 box corners.
			/// </summary>
			/// <param name="vertex0">Vector2D(Min.x, Min.y)</param>
			/// <param name="vertex1">Vector2D(Max.x, Min.y)</param>
			/// <param name="vertex2">Vector2D(Max.x, Max.y)</param>
			/// <param name="vertex3">Vector2D(Min.x, Max.y)</param>
			public void CalcVertices(out Vector2D vertex0, out Vector2D vertex1, out Vector2D vertex2, out Vector2D vertex3)
			{
				vertex0 = Min;
				vertex1 = new Vector2D(Max.x, Min.y);
				vertex2 = Max;
				vertex3 = new Vector2D(Min.x, Max.y);
			}

			/// <summary>
			/// Calculates 4 box corners and returns them in an allocated array.
			/// See array-less overload for the description.
			/// </summary>
			public Vector2D[] CalcVertices()
			{
				return new Vector2D[]
				{
					Min,
					new Vector2D(Max.x, Min.y),
					Max,
					new Vector2D(Min.x, Max.y),
				};
			}

			/// <summary>
			/// Calculates 4 box corners and fills the input array with them (array length must be 4).
			/// See array-less overload for the description.
			/// </summary>
			public void CalcVertices(Vector2D[] array)
			{
				array[0] = Min;
				array[1] = new Vector2D(Max.x, Min.y);
				array[2] = Max;
				array[3] = new Vector2D(Min.x, Max.y);
			}

			/// <summary>
			/// Returns box area
			/// </summary>
			public double CalcArea()
			{
				return (Max.x - Min.x) * (Max.y - Min.y);
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector2D point)
			{
				return Distance.Point2AAB2(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector2D Project(Vector2D point)
			{
				Vector2D result;
				Distance.SqrPoint2AAB2(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Tests whether a point is contained by the aab
			/// </summary>
			public bool Contains(ref Vector2D point)
			{
				if (point.x < Min.x) return false;
				if (point.x > Max.x) return false;
				if (point.y < Min.y) return false;
				if (point.y > Max.y) return false;
				return true;
			}

			/// <summary>
			/// Tests whether a point is contained by the aab
			/// </summary>
			public bool Contains(Vector2D point)
			{
				if (point.x < Min.x) return false;
				if (point.x > Max.x) return false;
				if (point.y < Min.y) return false;
				if (point.y > Max.y) return false;
				return true;
			}

			/// <summary>
			/// Enlarges the aab to include the point. If the point is inside the AAB does nothing.
			/// </summary>
			public void Include(ref Vector2D point)
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
			}

			/// <summary>
			/// Enlarges the aab to include the point. If the point is inside the AAB does nothing.
			/// </summary>
			public void Include(Vector2D point)
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
			}

			/// <summary>
			/// Enlarges the aab so it includes another aab.
			/// </summary>
			public void Include(ref AAB2 box)
			{
				Include(ref box.Min);
				Include(ref box.Max);
			}

			/// <summary>
			/// Enlarges the aab so it includes another aab.
			/// </summary>
			public void Include(AAB2 box)
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
