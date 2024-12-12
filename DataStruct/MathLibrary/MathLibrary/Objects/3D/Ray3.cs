

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The ray is represented as P+t*D, where P is the ray origin, D is a
		/// unit-length direction vector, and t &gt;= 0.  The user must ensure that D
		/// is indeed unit length.
		/// </summary>
		public struct Ray3
		{
			/// <summary>
			/// Ray origin
			/// </summary>
			public Vector3D Center;

			/// <summary>
			/// Ray direction. Must be unit length!
			/// </summary>
			public Vector3D Direction;


			/// <summary>
			/// Creates the ray
			/// </summary>
			/// <param name="center">Ray origin</param>
			/// <param name="direction">Ray direction. Must be unit length!</param>
			public Ray3(ref Vector3D center, ref Vector3D direction)
			{
				Center = center;
				Direction = direction;
			}
			
			/// <summary>
			/// Creates the ray
			/// </summary>
			/// <param name="center">Ray origin</param>
			/// <param name="direction">Ray direction. Must be unit length!</param>
			public Ray3(Vector3D center, Vector3D direction)
			{
				Center = center;
				Direction = direction;
			}

            ///// <summary>
            ///// Converts Ray3 to UnityEngine.Ray
            ///// </summary>
            //public static implicit operator Ray(Ray3 value)
            //{
            //    return new Ray(value.Center, value.Direction);
            //}

            ///// <summary>
            ///// Converts UnityEngine.Ray to Ray3
            ///// </summary>
            //public static implicit operator Ray3(Ray value)
            //{
            //    return new Ray3(value.origin, value.direction);
            //}

			
			/// <summary>
			/// Evaluates ray using P+t*D formula, where P is the ray origin, D is a
			/// unit-length direction vector, t is parameter.
			/// </summary>
			/// <param name="t">Evaluation parameter</param>
			public Vector3D Eval(double t)
			{
				return Center + Direction * t;
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector3D point)
			{
				return Distance.Point3Ray3(ref point, ref this);
			}
			
			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector3D Project(Vector3D point)
			{
				Vector3D result;
				Distance.SqrPoint3Ray3(ref point, ref this, out result);
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
