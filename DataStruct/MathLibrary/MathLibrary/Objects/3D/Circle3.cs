

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The plane containing the circle is Dot(N,X-C) = 0, where X is any point
		/// in the plane.  Vectors U, V, and N form an orthonormal set
		/// (matrix [U V N] is orthonormal and has determinant 1).  The circle
		/// within the plane is parameterized by X = C + R*(cos(t)*U + sin(t)*V),
		/// where t is an angle in [0,2*pi).
		/// </summary>
		public struct Circle3
		{
			/// <summary>
			/// Circle center.
			/// </summary>
			public Vector3D Center;

			/// <summary>
			/// First circle axis. Must be unit length!
			/// </summary>
			public Vector3D Axis0;

			/// <summary>
			/// Second circle axis. Must be unit length!
			/// </summary>
			public Vector3D Axis1;

			/// <summary>
			/// Circle normal which is Cross(Axis0, Axis1). Must be unit length!
			/// </summary>
			public Vector3D Normal;

			/// <summary>
			/// Circle radius.
			/// </summary>
			public double Radius;


			/// <summary>
			/// Creates new circle instance from center, axes and radius. Normal is calculated as cross product of the axes.
			/// </summary>
			/// <param name="axis0">Must be unit length!</param>
			/// <param name="axis1">Must be unit length!</param>
			public Circle3(ref Vector3D center, ref Vector3D axis0, ref Vector3D axis1, double radius)
			{
				Center = center;
				Axis0  = axis0;
				Axis1  = axis1;
				Normal = axis0.Cross(axis1);
				Radius = radius;
			}

			/// <summary>
			/// Creates new circle instance from center, axes and radius. Normal is calculated as cross product of the axes.
			/// </summary>
			public Circle3(Vector3D center, Vector3D axis0, Vector3D axis1, double radius)
			{
				Center = center;
				Axis0  = axis0;
				Axis1  = axis1;
				Normal = axis0.Cross(axis1);
				Radius = radius;
			}

			/// <summary>
			/// Creates new circle instance. Computes axes from specified normal.
			/// </summary>
			/// <param name="normal">Must be unit length!</param>
			public Circle3(ref Vector3D center, ref Vector3D normal, double radius)
			{
				Center = center;
				Normal = normal;
				Vector3Dex.CreateOrthonormalBasis(out Axis0, out Axis1, ref Normal);
				Radius = radius;
			}

			/// <summary>
			/// Creates new circle instance. Computes axes from specified normal.
			/// </summary>
			/// <param name="normal">Must be unit length!</param>
			public Circle3(Vector3D center, Vector3D normal, double radius)
			{
				Center = center;
				Normal = normal;
				Vector3Dex.CreateOrthonormalBasis(out Axis0, out Axis1, ref Normal);
				Radius = radius;
			}

			/// <summary>
			/// Creates circle which is circumscribed around triangle.
			/// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
			/// </summary>
			public static bool CreateCircumscribed(Vector3D v0, Vector3D v1, Vector3D v2, out Circle3 circle)
			{
				Vector3D E02 = v0 - v2;
				Vector3D E12 = v1 - v2;
				double e02e02 = E02.Dot(E02);
				double e02e12 = E02.Dot(E12);
				double e12e12 = E12.Dot(E12);
				double det = e02e02 * e12e12 - e02e12 * e02e12;

				if (System.Math.Abs(det) < Mathex.ZeroTolerance)
				{
					circle = new Circle3();
					return false;
				}

				double halfInvDet = 0.5f / det;
				double u0 = halfInvDet * e12e12 * (e02e02 - e02e12);
				double u1 = halfInvDet * e02e02 * (e12e12 - e02e12);
				Vector3D tmp = u0 * E02 + u1 * E12;

				circle.Center = v2 + tmp;
				circle.Radius = tmp.magnitude;

				circle.Normal = E02.UnitCross(E12);

				if (System.Math.Abs(circle.Normal.x) >= System.Math.Abs(circle.Normal.y) &&
					System.Math.Abs(circle.Normal.x) >= System.Math.Abs(circle.Normal.z))
				{
					circle.Axis0.x = -circle.Normal.y;
					circle.Axis0.y = circle.Normal.x;
					circle.Axis0.z = 0f;
				}
				else
				{
					circle.Axis0.x = 0f;
					circle.Axis0.y = circle.Normal.z;
					circle.Axis0.z = -circle.Normal.y;
				}

				circle.Axis0.Normalize();
				circle.Axis1 = circle.Normal.Cross(circle.Axis0);

				return true;
			}

			/// <summary>
			/// Creates circle which is insribed into triangle.
			/// Returns 'true' if circle has been constructed, 'false' otherwise (input points are linearly dependent).
			/// </summary>
			public static bool CreateInscribed(Vector3D v0, Vector3D v1, Vector3D v2, out Circle3 circle)
			{
				// Edges.
				Vector3D E0 = v1 - v0;
				Vector3D E1 = v2 - v1;
				Vector3D E2 = v0 - v2;

				// Plane normal.
				circle.Normal = E1.Cross(E0);

				// Edge normals within the plane.
				Vector3D N0 = circle.Normal.UnitCross(E0);
				Vector3D N1 = circle.Normal.UnitCross(E1);
				Vector3D N2 = circle.Normal.UnitCross(E2);

				double a0 = N1.Dot(E0);
				if (System.Math.Abs(a0) < Mathex.ZeroTolerance)
				{
					circle = new Circle3();
					return false;
				}

				double a1 = N2.Dot(E1);
				if (System.Math.Abs(a1) < Mathex.ZeroTolerance)
				{
					circle = new Circle3();
					return false;
				}

				double a2 = N0.Dot(E2);
				if (System.Math.Abs(a2) < Mathex.ZeroTolerance)
				{
					circle = new Circle3();
					return false;
				}

				double invA0 = 1f / a0;
				double invA1 = 1f / a1;
				double invA2 = 1f / a2;

				circle.Radius = 1f / (invA0 + invA1 + invA2);
				circle.Center = circle.Radius * (invA0 * v0 + invA1 * v1 + invA2 * v2);

				circle.Normal.Normalize();
				circle.Axis0 = N0;
				circle.Axis1 = circle.Normal.Cross(circle.Axis0);

				return true;
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
			/// Evaluates circle using formula X = C + R*cos(t)*U + R*sin(t)*V
			/// where t is an angle in [0,2*pi).
			/// </summary>
			/// <param name="t">Evaluation parameter</param>
			public Vector3D Eval(double t)
			{
				return Center + Radius * (System.Math.Cos(t) * Axis0 + System.Math.Sin(t) * Axis1);
			}

			/// <summary>
			/// Evaluates disk using formula X = C + radius*cos(t)*U + radius*sin(t)*V
			/// where t is an angle in [0,2*pi).
			/// </summary>
			/// <param name="t">Evaluation parameter</param>
			/// <param name="radius">Evaluation radius</param>
			public Vector3D Eval(double t, double radius)
			{
				return Center + radius * (System.Math.Cos(t) * Axis0 + System.Math.Sin(t) * Axis1);
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector3D point, bool solid = true)
			{
				return Distance.Point3Circle3(ref point, ref this, solid);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector3D Project(Vector3D point, bool solid = true)
			{
				Vector3D result;
				Distance.SqrPoint3Circle3(ref point, ref this, out result, solid);
				return result;
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[Center: {0} Axis0: {1} Axis1: {2} Normal: {3} Radius: {4}]", Center.ToStringEx(), Axis0.ToStringEx(), Axis1.ToStringEx(), Normal.ToStringEx(), Radius.ToString());
			}
		}
	}
}
