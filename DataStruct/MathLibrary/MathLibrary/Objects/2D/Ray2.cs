

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The ray is represented as P+t*D, where P is the ray origin, D is a
		/// unit-length direction vector, and t &gt;= 0.  The user must ensure that D
		/// is indeed unit length.
		/// </summary>
		public struct Ray2
		{
			/// <summary>
			/// Ray origin
			/// </summary>
			public Vector2D Center;

			/// <summary>
			/// Ray direction. Must be unit length!
			/// </summary>
			public Vector2D Direction;


			/// <summary>
			/// Creates the ray
			/// </summary>
			/// <param name="center">Ray origin</param>
			/// <param name="direction">Ray direction. Must be unit length!</param>
			public Ray2(ref Vector2D center, ref Vector2D direction)
			{
				Center = center;
				Direction = direction;
			}

			/// <summary>
			/// Creates the ray
			/// </summary>
			/// <param name="center">Ray origin</param>
			/// <param name="direction">Ray direction. Must be unit length!</param>
			public Ray2(Vector2D center, Vector2D direction)
			{
				Center = center;
				Direction = direction;
			}
			
			
			/// <summary>
			/// Evaluates ray using P+t*D formula, where P is the ray origin, D is a
			/// unit-length direction vector, t is parameter.
			/// </summary>
			/// <param name="t">Evaluation parameter</param>
			public Vector2D Eval(double t)
			{
				return Center + Direction * t;
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector2D point)
			{
				return Distance.Point2Ray2(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector2D Project(Vector2D point)
			{
				Vector2D result;
				Distance.SqrPoint2Ray2(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[Origin: {0} Direction: {1}]", Center.ToStringEx(), Direction.ToStringEx());
			}
		}
	}
}
