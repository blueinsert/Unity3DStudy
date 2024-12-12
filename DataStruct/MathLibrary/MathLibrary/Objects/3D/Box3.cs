
using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// A box has center C, axis directions U[0], U[1], and U[2] (mutually
		/// perpendicular unit-length vectors), and extents e[0], e[1], and e[2]
		/// (all nonnegative numbers).  A point X = C+y[0]*U[0]+y[1]*U[1]+y[2]*U[2]
		/// is inside or on the box whenever |y[i]| &lt;= e[i] for all i.
		/// </summary>
		public struct Box3
		{
			/// <summary>
			/// Box center
			/// </summary>
			public Vector3D Center;

			/// <summary>
			/// First box axis. Must be unit length!
			/// </summary>
			public Vector3D Axis0;

			/// <summary>
			/// Second box axis. Must be unit length!
			/// </summary>
			public Vector3D Axis1;

			/// <summary>
			/// Third box axis. Must be unit length!
			/// </summary>
			public Vector3D Axis2;

			/// <summary>
			/// Extents (half sizes) along Axis0, Axis1 and Axis2. Must be non-negative!
			/// </summary>
			public Vector3D Extents;


			/// <summary>
			/// Creates new Box3 instance.
			/// </summary>
			/// <param name="center">Box center</param>
			/// <param name="axis0">First box axis. Must be unit length!</param>
			/// <param name="axis1">Second box axis. Must be unit length!</param>
			/// <param name="axis2">Third box axis. Must be unit length!</param>
			/// <param name="extents">Extents (half sizes) along Axis0, Axis1 and Axis2. Must be non-negative!</param>
			public Box3(ref Vector3D center, ref Vector3D axis0, ref Vector3D axis1, ref Vector3D axis2, ref Vector3D extents)
			{
				Center  = center;
				Axis0   = axis0;
				Axis1   = axis1;
				Axis2   = axis2;
				Extents = extents;
			}

			/// <summary>
			/// Creates new Box3 instance.
			/// </summary>
			/// <param name="center">Box center</param>
			/// <param name="axis0">First box axis. Must be unit length!</param>
			/// <param name="axis1">Second box axis. Must be unit length!</param>
			/// <param name="axis2">Third box axis. Must be unit length!</param>
			/// <param name="extents">Extents (half sizes) along Axis0, Axis1 and Axis2. Must be non-negative!</param>
			public Box3(Vector3D center, Vector3D axis0, Vector3D axis1, Vector3D axis2, Vector3D extents)
			{
				Center  = center;
				Axis0   = axis0;
				Axis1   = axis1;
				Axis2   = axis2;
				Extents = extents;
			}

			/// <summary>
			/// Create Box3 from AxisAlignedBox3
			/// </summary>
			public Box3(ref AAB3 box)
			{
				box.CalcCenterExtents(out Center, out Extents);
				Axis0 = Vector3Dex.UnitX;
				Axis1 = Vector3Dex.UnitY;
				Axis2 = Vector3Dex.UnitZ;
			}

			/// <summary>
			/// Create Box3 from AxisAlignedBox3
			/// </summary>
			public Box3(AAB3 box)
			{
				box.CalcCenterExtents(out Center, out Extents);
				Axis0 = Vector3Dex.UnitX;
				Axis1 = Vector3Dex.UnitY;
				Axis2 = Vector3Dex.UnitZ;
			}

			/// <summary>
			/// Computes oriented bounding box from a set of points.
			/// If a set is empty returns new Box3().
			/// </summary>
			public static Box3 CreateFromPoints(IList<Vector3D> points)
			{
				int numPoints = points.Count;
				if (numPoints == 0)
				{
					return new Box3();
				}

				Box3 box = Approximation.GaussPointsFit3(points);

				// Let C be the box center and let U0, U1, and U2 be the box axes.  Each
				// input point is of the form X = C + y0*U0 + y1*U1 + y2*U2.  The
				// following code computes min(y0), max(y0), min(y1), max(y1), min(y2),
				// and max(y2).  The box center is then adjusted to be
				//   C' = C + 0.5*(min(y0)+max(y0))*U0 + 0.5*(min(y1)+max(y1))*U1 +
				//        0.5*(min(y2)+max(y2))*U2

				Vector3D diff = points[0] - box.Center;
				Vector3D pmin = new Vector3D(diff.Dot(box.Axis0), diff.Dot(box.Axis1), diff.Dot(box.Axis2));
				Vector3D pmax = pmin;

				for (int i = 1; i < numPoints; ++i)
				{
					diff = points[i] - box.Center;
					for (int j = 0; j < 3; ++j)
					{
						double dot = diff.Dot(box.GetAxis(j));
						if (dot < pmin[j])
						{
							pmin[j] = dot;
						}
						else if (dot > pmax[j])
						{
							pmax[j] = dot;
						}
					}
				}

				box.Center +=
					(0.5f * (pmin[0] + pmax[0])) * box.Axis0 +
					(0.5f * (pmin[1] + pmax[1])) * box.Axis1 +
					(0.5f * (pmin[2] + pmax[2])) * box.Axis2;

				box.Extents.x = 0.5f * (pmax[0] - pmin[0]);
				box.Extents.y = 0.5f * (pmax[1] - pmin[1]);
				box.Extents.z = 0.5f * (pmax[2] - pmin[2]);

				return box;
			}


			/// <summary>
			/// Returns axis by index (0, 1, 2)
			/// </summary>
			public Vector3D GetAxis(int index)
			{
				if (index == 0) return Axis0;
				if (index == 1) return Axis1;
				if (index == 2) return Axis2;
				return Vector3Dex.Zero;
			}

			/// <summary>
			/// Calculates 8 box corners. extAxis[i] is Axis[i]*Extent[i], i=0,1,2
			/// </summary>
			/// <param name="vertex0">Center - extAxis0 - extAxis1 - extAxis2</param>
			/// <param name="vertex1">Center + extAxis0 - extAxis1 - extAxis2</param>
			/// <param name="vertex2">Center + extAxis0 + extAxis1 - extAxis2</param>
			/// <param name="vertex3">Center - extAxis0 + extAxis1 - extAxis2</param>
			/// <param name="vertex4">Center - extAxis0 - extAxis1 + extAxis2</param>
			/// <param name="vertex5">Center + extAxis0 - extAxis1 + extAxis2</param>
			/// <param name="vertex6">Center + extAxis0 + extAxis1 + extAxis2</param>
			/// <param name="vertex7">Center - extAxis0 + extAxis1 + extAxis2</param>
			public void CalcVertices(
				out Vector3D vertex0, out Vector3D vertex1, out Vector3D vertex2, out Vector3D vertex3,
				out Vector3D vertex4, out Vector3D vertex5, out Vector3D vertex6, out Vector3D vertex7)
			{
				Vector3D extAxis0 = Extents.x * Axis0;
				Vector3D extAxis1 = Extents.y * Axis1;
				Vector3D extAxis2 = Extents.z * Axis2;

				vertex0 = Center - extAxis0 - extAxis1 - extAxis2;
				vertex1 = Center + extAxis0 - extAxis1 - extAxis2;
				vertex2 = Center + extAxis0 + extAxis1 - extAxis2;
				vertex3 = Center - extAxis0 + extAxis1 - extAxis2;
				vertex4 = Center - extAxis0 - extAxis1 + extAxis2;
				vertex5 = Center + extAxis0 - extAxis1 + extAxis2;
				vertex6 = Center + extAxis0 + extAxis1 + extAxis2;
				vertex7 = Center - extAxis0 + extAxis1 + extAxis2;
			}

			/// <summary>
			/// Calculates 8 box corners and returns them in an allocated array.
			/// See array-less overload for the description.
			/// </summary>
			public Vector3D[] CalcVertices()
			{
				Vector3D extAxis0 = Extents.x * Axis0;
				Vector3D extAxis1 = Extents.y * Axis1;
				Vector3D extAxis2 = Extents.z * Axis2;

				Vector3D[] result = 
				{
					Center - extAxis0 - extAxis1 - extAxis2,
					Center + extAxis0 - extAxis1 - extAxis2,
					Center + extAxis0 + extAxis1 - extAxis2,
					Center - extAxis0 + extAxis1 - extAxis2,
					Center - extAxis0 - extAxis1 + extAxis2,
					Center + extAxis0 - extAxis1 + extAxis2,
					Center + extAxis0 + extAxis1 + extAxis2,
					Center - extAxis0 + extAxis1 + extAxis2,
				};
				return result;
			}

			/// <summary>
			/// Calculates 8 box corners and fills the input array with them (array length must be 8).
			/// See array-less overload for the description.
			/// </summary>
			public void CalcVertices(Vector3D[] array)
			{
				Vector3D extAxis0 = Extents.x * Axis0;
				Vector3D extAxis1 = Extents.y * Axis1;
				Vector3D extAxis2 = Extents.z * Axis2;

				array[0] = Center - extAxis0 - extAxis1 - extAxis2;
				array[1] = Center + extAxis0 - extAxis1 - extAxis2;
				array[2] = Center + extAxis0 + extAxis1 - extAxis2;
				array[3] = Center - extAxis0 + extAxis1 - extAxis2;
				array[4] = Center - extAxis0 - extAxis1 + extAxis2;
				array[5] = Center + extAxis0 - extAxis1 + extAxis2;
				array[6] = Center + extAxis0 + extAxis1 + extAxis2;
				array[7] = Center - extAxis0 + extAxis1 + extAxis2;
			}

			/// <summary>
			/// Returns volume of the box as Extents.x * Extents.y * Extents.z * 8
			/// </summary>
			public double CalcVolume()
			{
				return 8f * Extents.x * Extents.y * Extents.z;
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector3D point)
			{
				return Distance.Point3Box3(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector3D Project(Vector3D point)
			{
				Vector3D result;
				Distance.SqrPoint3Box3(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Tests whether a point is contained by the box
			/// </summary>
			public bool Contains(ref Vector3D point)
			{
				Vector3D diff;
				diff.x = point.x - Center.x;
				diff.y = point.y - Center.y;
				diff.z = point.z - Center.z;
				double proj;
				proj = diff.Dot(Axis0);
				if (proj < -Extents.x) return false;
				if (proj > Extents.x) return false;
				proj = diff.Dot(Axis1);
				if (proj < -Extents.y) return false;
				if (proj > Extents.y) return false;
				proj = diff.Dot(Axis2);
				if (proj < -Extents.z) return false;
				if (proj > Extents.z) return false;
				return true;
			}

			/// <summary>
			/// Tests whether a point is contained by the box
			/// </summary>
			public bool Contains(Vector3D point)
			{
				return Contains(ref point);
			}

			/// <summary>
			/// Enlarges the box so it includes another box.
			/// </summary>
			public void Include(ref Box3 box)
			{
				// Construct a box that contains the input boxes.
				Box3 result = new Box3();

				// The first guess at the box center.  This value will be updated later
				// after the input box vertices are projected onto axes determined by an
				// average of box axes.
				result.Center = 0.5f * (this.Center + box.Center);

				// A box's axes, when viewed as the columns of a matrix, form a rotation
				// matrix.  The input box axes are converted to quaternions.  The average
				// quaternion is computed, then normalized to unit length.  The result is
				// the slerp of the two input quaternions with t-value of 1/2.  The result
				// is converted back to a rotation matrix and its columns are selected as
				// the merged box axes.
				Matrix4X4 m0;
				Matrix4X4ex.CreateRotationFromColumns(ref this.Axis0, ref this.Axis1, ref this.Axis2, out m0);
				Quaternion q0;
				Matrix4X4ex.RotationMatrixToQuaternion(ref m0, out q0);

				Matrix4X4 m1;
				Matrix4X4ex.CreateRotationFromColumns(ref box.Axis0, ref box.Axis1, ref box.Axis2, out m1);
				Quaternion q1;
				Matrix4X4ex.RotationMatrixToQuaternion(ref m1, out q1);

				if (Quaternion.Dot(q0, q1) < 0f)
				{
					q1.x = -q1.x;
					q1.y = -q1.y;
					q1.z = -q1.z;
					q1.w = -q1.w;
				}

				Quaternion q;
				q.x = q0.x + q1.x;
				q.y = q0.x + q1.y;
				q.z = q0.x + q1.z;
				q.w = q0.x + q1.w;

				double invLength = Mathex.InvSqrt(Quaternion.Dot(q, q));
				q.x *= invLength;
				q.y *= invLength;
				q.z *= invLength;
				q.w *= invLength;

				Matrix4X4 m;
				Matrix4X4ex.QuaternionToRotationMatrix(ref q, out m);
				result.Axis0 = m.GetColumn(0);
				result.Axis1 = m.GetColumn(1);
				result.Axis2 = m.GetColumn(2);

				// Project the input box vertices onto the merged-box axes.  Each axis
				// D[i] containing the current center C has a minimum projected value
				// min[i] and a maximum projected value max[i].  The corresponding end
				// points on the axes are C+min[i]*D[i] and C+max[i]*D[i].  The point C
				// is not necessarily the midpoint for any of the intervals.  The actual
				// box center will be adjusted from C to a point C' that is the midpoint
				// of each interval,
				//   C' = C + sum_{i=0}^2 0.5*(min[i]+max[i])*D[i]
				// The box extents are
				//   e[i] = 0.5*(max[i]-min[i])

				int i, j;
				double dot;
				Vector3D diff;
				Vector3D pmin = Vector3Dex.Zero;
				Vector3D pmax = Vector3Dex.Zero;
				Vector3D[] vertex = this.CalcVertices();

				for (i = 0; i < 8; ++i)
				{
					diff = vertex[i] - result.Center;
					for (j = 0; j < 3; ++j)
					{
						dot = diff.Dot(result.GetAxis(j));
						if (dot > pmax[j])
						{
							pmax[j] = dot;
						}
						else if (dot < pmin[j])
						{
							pmin[j] = dot;
						}
					}
				}

				box.CalcVertices(
					out vertex[0], out vertex[1], out vertex[2], out vertex[3],
					out vertex[4], out vertex[5], out vertex[6], out vertex[7]);
				for (i = 0; i < 8; ++i)
				{
					diff = vertex[i] - result.Center;
					for (j = 0; j < 3; ++j)
					{
						dot = diff.Dot(result.GetAxis(j));
						if (dot > pmax[j])
						{
							pmax[j] = dot;
						}
						else if (dot < pmin[j])
						{
							pmin[j] = dot;
						}
					}
				}

				// [min,max] is the axis-aligned box in the coordinate system of the
				// merged box axes.  Update the current box center to be the center of
				// the new box.  Compute the extents based on the new center.
				for (j = 0; j < 3; ++j)
				{
					result.Center += (0.5f * (pmax[j] + pmin[j])) * result.GetAxis(j);
					result.Extents[j] = 0.5f * (pmax[j] - pmin[j]);
				}

				this = result;
			}

			/// <summary>
			/// Enlarges the box so it includes another box.
			/// </summary>
			public void Include(Box3 box)
			{
				Include(ref box);
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[Center: {0} Axis0: {1} Axis1: {2} Axis2: {3} Extents: {4}]",
					Center.ToStringEx(), Axis0.ToStringEx(), Axis1.ToStringEx(), Axis2.ToStringEx(), Extents.ToStringEx());
			}
		}
	}
}
