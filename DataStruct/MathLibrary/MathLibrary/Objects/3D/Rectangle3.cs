

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Points are R(s,t) = C+s0*U0+s1*U1, where C is the center of the
		/// rectangle, U0 and U1 are unit-length and perpendicular axes.  The
		/// parameters s0 and s1 are constrained by |s0| &lt;= e0 and |s1| &lt;= e1,
		/// where e0 &gt; 0 and e1 &gt; 0 are called the extents of the rectangle.
		/// </summary>
		public struct Rectangle3
		{
			/// <summary>
			/// Rectangle center
			/// </summary>
			public Vector3D Center;

			/// <summary>
			/// First rectangle axis. Must be unit length!
			/// </summary>
			public Vector3D Axis0;

			/// <summary>
			/// Second rectangle axis. Must be unit length!
			/// </summary>
			public Vector3D Axis1;

			/// <summary>
			/// Rectangle normal which is Cross(Axis0, Axis1). Must be unit length!
			/// </summary>
			public Vector3D Normal;

			/// <summary>
			/// Extents (half sizes) along Axis0 and Axis1. Must be non-negative!
			/// </summary>
			public Vector2D Extents;


			/// <summary>
			/// Creates new Rectangle3 instance.
			/// </summary>
			/// <param name="center">Rectangle center</param>
			/// <param name="axis0">First box axis. Must be unit length!</param>
			/// <param name="axis1">Second box axis. Must be unit length!</param>
			/// <param name="extents">Extents (half sizes) along Axis0 and Axis1. Must be non-negative!</param>
			public Rectangle3(ref Vector3D center, ref Vector3D axis0, ref Vector3D axis1, ref Vector2D extents)
			{
				Center  = center;
				Axis0   = axis0;
				Axis1   = axis1;
				Normal  = axis0.Cross(axis1);
				Extents = extents;
			}

			/// <summary>
			/// Creates new Rectangle3 instance.
			/// </summary>
			/// <param name="center">Rectangle center</param>
			/// <param name="axis0">First rectangle axis. Must be unit length!</param>
			/// <param name="axis1">Second rectangle axis. Must be unit length!</param>
			/// <param name="extents">Extents (half sizes) along Axis0 and Axis1. Must be non-negative!</param>
			public Rectangle3(Vector3D center, Vector3D axis0, Vector3D axis1, Vector2D extents)
			{
				Center  = center;
				Axis0   = axis0;
				Axis1   = axis1;
				Normal  = axis0.Cross(axis1);
				Extents = extents;
			}			

			/// <summary>
			/// Creates rectangle from 4 counter clockwise ordered ordered points. Center=(p0+p2)/2, Axis0=Normalized(p1-p0), Axis1=Normalized(p2-p1).
			/// The user therefore must ensure that the points are indeed represent rectangle to obtain meaningful result.
			/// </summary>
			public static Rectangle3 CreateFromCCWPoints(Vector3D p0, Vector3D p1, Vector3D p2, Vector3D p3)
			{
				Vector3D axis0 = p1 - p0;
				Vector3D axis1 = p2 - p1;
				
				Rectangle3 result;
				result.Center = (p0 + p2) * .5f;
				result.Extents.x = Vector3Dex.Normalize(ref axis0) * .5f;
				result.Extents.y = Vector3Dex.Normalize(ref axis1) * .5f;
				result.Axis0 = axis0;
				result.Axis1 = axis1;
				result.Normal = axis0.Cross(axis1);

				return result;
			}

			/// <summary>
			/// Creates rectangle from 4 clockwise ordered points. Center=(p0+p2)/2, Axis0=Normalized(p2-p1), Axis1=Normalized(p1-p0).
			/// The user therefore must ensure that the points are indeed represent rectangle to obtain meaningful result.
			/// </summary>
			public static Rectangle3 CreateFromCWPoints(Vector3D p0, Vector3D p1, Vector3D p2, Vector3D p3)
			{
				Vector3D axis0 = p2 - p1;
				Vector3D axis1 = p1 - p0;

				Rectangle3 result;
				result.Center = (p0 + p2) * .5f;
				result.Extents.x = Vector3Dex.Normalize(ref axis0) * .5f;
				result.Extents.y = Vector3Dex.Normalize(ref axis1) * .5f;
				result.Axis0 = axis0;
				result.Axis1 = axis1;
				result.Normal = axis0.Cross(axis1);

				return result;
			}

			
			/// <summary>
			/// Calculates 4 box corners. extAxis[i] is Axis[i]*Extent[i], i=0,1.
			/// </summary>
			/// <param name="vertex0">Center - extAxis0 - extAxis1</param>
			/// <param name="vertex1">Center + extAxis0 - extAxis1</param>
			/// <param name="vertex2">Center + extAxis0 + extAxis1</param>
			/// <param name="vertex3">Center - extAxis0 + extAxis1</param>
			public void CalcVertices(out Vector3D vertex0, out Vector3D vertex1, out Vector3D vertex2, out Vector3D vertex3)
			{
				Vector3D extAxis0 = Axis0 * Extents.x;
				Vector3D extAxis1 = Axis1 * Extents.y;

				vertex0 = Center - extAxis0 - extAxis1;
				vertex1 = Center + extAxis0 - extAxis1;
				vertex2 = Center + extAxis0 + extAxis1;
				vertex3 = Center - extAxis0 + extAxis1;
			}

			/// <summary>
			/// Calculates 4 box corners and returns them in an allocated array.
			/// Look array-less method for the description.
			/// </summary>
			public Vector3D[] CalcVertices()
			{
				Vector3D extAxis0 = Axis0 * Extents.x;
				Vector3D extAxis1 = Axis1 * Extents.y;

				Vector3D[] result =
				{
					Center - extAxis0 - extAxis1,
					Center + extAxis0 - extAxis1,
					Center + extAxis0 + extAxis1,
					Center - extAxis0 + extAxis1,
				};
				return result;
			}

			/// <summary>
			/// Calculates 4 box corners and fills the input array with them (array length must be 4).
			/// Look array-less method for the description.
			/// </summary>
			public void CalcVertices(Vector3D[] array)
			{
				Vector3D extAxis0 = Axis0 * Extents.x;
				Vector3D extAxis1 = Axis1 * Extents.y;

				array[0] = Center - extAxis0 - extAxis1;
				array[1] = Center + extAxis0 - extAxis1;
				array[2] = Center + extAxis0 + extAxis1;
				array[3] = Center - extAxis0 + extAxis1;
			}

			/// <summary>
			/// Returns area of the box as Extent.x*Extent.y*4
			/// </summary>
			public double CalcArea()
			{
				return 4f * Extents.x * Extents.y;
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector3D point)
			{
				return Distance.Point3Rectangle3(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector3D Project(Vector3D point)
			{
				Vector3D result;
				Distance.SqrPoint3Rectangle3(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[Center: {0}  Axis0: {1} Axis1: {2} Normal: {3} Extents: {4}]",
					Center.ToStringEx(), Axis0.ToStringEx(), Axis1.ToStringEx(), Normal.ToStringEx(), Extents.ToStringEx());
			}
		}
	}
}
