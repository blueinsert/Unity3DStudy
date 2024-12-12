

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The line is represented as P+t*D where P is the line origin, D is a
		/// unit-length direction vector, and t is any real number.  The user must
		/// ensure that D is indeed unit length.
		/// </summary>
		public struct Line3
		{
			/// <summary>
			/// Line origin
			/// </summary>
			public Vector3D Center;

			/// <summary>
			/// Line direction. Must be unit length!
			/// </summary>
			public Vector3D Direction;


			/// <summary>
			/// Creates the line
			/// </summary>
			/// <param name="center">Line origin</param>
			/// <param name="direction">Line direction. Must be unit length!</param>
			public Line3(ref Vector3D center, ref Vector3D direction)
			{
				Center = center;
				Direction = direction;
			}

			/// <summary>
			/// Creates the line
			/// </summary>
			/// <param name="center">Line origin</param>
			/// <param name="direction">Line direction. Must be unit length!</param>
			public Line3(Vector3D center, Vector3D direction)
			{
				Center = center;
				Direction = direction;
			}

			/// <summary>
			/// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
			/// </summary>
			/// <param name="p0">First point</param>
			/// <param name="p1">Second point</param>
			public static Line3 CreateFromTwoPoints(ref Vector3D p0, ref Vector3D p1)
			{
				Line3 result;
				result.Center = p0;
				result.Direction = (p1 - p0).normalized;
				return result;
			}

			/// <summary>
			/// Creates the line. Origin is p0, Direction is Normalized(p1-p0).
			/// </summary>
			/// <param name="p0">First point</param>
			/// <param name="p1">Second point</param>
			public static Line3 CreateFromTwoPoints(Vector3D p0, Vector3D p1)
			{
				Line3 result;
				result.Center = p0;
				result.Direction = (p1 - p0).normalized;
				return result;
			}

			
			/// <summary>
			/// Evaluates line using P+t*D formula, where P is the line origin, D is a
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
				return Distance.Point3Line3(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector3D Project(Vector3D point)
			{
				Vector3D result;
				Distance.SqrPoint3Line3(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Returns angle between this line's direction and another line's direction as: arccos(dot(this.Direction,another.Direction))
			/// If acuteAngleDesired is true, then in resulting angle is > pi/2, then result is transformed to be pi-angle.
			/// </summary>
			public double AngleBetweenTwoLines(Line3 anotherLine, bool acuteAngleDesired = false)
			{
				double angle = System.Math.Acos(this.Direction.Dot(anotherLine.Direction));
				if (acuteAngleDesired &&
					angle > Mathex.HalfPi)
				{
					return System.Math.PI - angle;
				}
				else
				{
					return angle;
				}
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
