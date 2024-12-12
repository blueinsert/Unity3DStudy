

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains information about intersection of Line3 and Triangle3
		/// </summary>
		public struct Line3Triangle3Intr
		{
			/// <summary>
			/// Equals to IntersectionTypes.Point if intersection occured otherwise IntersectionTypes.Empty (even when a line lies in a triangle plane)
			/// </summary>
			public IntersectionTypes IntersectionType;

			/// <summary>
			/// Intersection point (in case of IntersectionTypes.Point)
			/// </summary>
			public Vector3D Point;

			/// <summary>
			/// Line evaluation parameter of the intersection point (in case of IntersectionTypes.Point)
			/// </summary>
			public double LineParameter;

			/// <summary>
			/// First barycentric coordinate of the intersection point
			/// </summary>
			public double TriBary0;

			/// <summary>
			/// Second barycentric coordinate of the intersection point
			/// </summary>
			public double TriBary1;

			/// <summary>
			/// Third barycentric coordinate of the intersection point
			/// </summary>
			public double TriBary2;
		}

		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Triangle3(ref Line3 line, ref Triangle3 triangle, out IntersectionTypes intersectionType)
			{
				// Compute the offset origin, edges, and normal.
				Vector3D diff   = line.Center - triangle.V0;
				Vector3D edge1  = triangle.V1 - triangle.V0;
				Vector3D edge2  = triangle.V2 - triangle.V0;
				Vector3D normal = edge1.Cross(edge2);

				// Solve Q + t*D = b1*E1 + b2*E2 (Q = diff, D = line direction,
				// E1 = edge1, E2 = edge2, N = Cross(E1,E2)) by
				//   |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
				//   |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
				//   |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
				double DdN = line.Direction.Dot(normal);
				double sign;
				if (DdN > _dotThreshold)
				{
					sign = 1f;
				}
				else if (DdN < -_dotThreshold)
				{
					sign = -1f;
					DdN = -DdN;
				}
				else
				{
					// Line and triangle are parallel, call it a "no intersection"
					// even if the line does intersect.
					intersectionType = IntersectionTypes.Empty;
					return false;
				}

				double DdQxE2 = sign * line.Direction.Dot(diff.Cross(edge2));
				if (DdQxE2 >= -Mathex.ZeroTolerance)
				{
					double DdE1xQ = sign * line.Direction.Dot(edge1.Cross(diff));
					if (DdE1xQ >= -Mathex.ZeroTolerance)
					{
						if (DdQxE2 + DdE1xQ <= DdN + Mathex.ZeroTolerance)
						{
							// Line intersects triangle.
							intersectionType = IntersectionTypes.Point;
							return true;
						}
						// else: b1+b2 > 1, no intersection
					}
					// else: b2 < 0, no intersection
				}
				// else: b1 < 0, no intersection

				intersectionType = IntersectionTypes.Empty;
				return false;
			}

			/// <summary>
			/// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Triangle3(ref Line3 line, ref Vector3D v0, ref Vector3D v1, ref Vector3D v2, out IntersectionTypes intersectionType)
			{
				Triangle3 triangle = new Triangle3() { V0 = v0, V1 = v1, V2 = v2 };
				return TestLine3Triangle3(ref line, ref triangle, out intersectionType);
			}

			/// <summary>
			/// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Triangle3(ref Line3 line, Vector3D v0, Vector3D v1, Vector3D v2, out IntersectionTypes intersectionType)
			{
				Triangle3 triangle = new Triangle3() { V0 = v0, V1 = v1, V2 = v2 };
				return TestLine3Triangle3(ref line, ref triangle, out intersectionType);
			}


			/// <summary>
			/// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Triangle3(ref Line3 line, ref Triangle3 triangle)
			{
				IntersectionTypes intersectionType;
				return TestLine3Triangle3(ref line, ref triangle, out intersectionType);
			}

			/// <summary>
			/// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Triangle3(ref Line3 line, ref Vector3D v0, ref Vector3D v1, ref Vector3D v2)
			{
				IntersectionTypes intersectionType;
				Triangle3 triangle = new Triangle3() { V0 = v0, V1 = v1, V2 = v2 };
				return TestLine3Triangle3(ref line, ref triangle, out intersectionType);
			}

			/// <summary>
			/// Tests if a line intersects a triangle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestLine3Triangle3(ref Line3 line, Vector3D v0, Vector3D v1, Vector3D v2)
			{
				IntersectionTypes intersectionType;
				Triangle3 triangle = new Triangle3() { V0 = v0, V1 = v1, V2 = v2 };
				return TestLine3Triangle3(ref line, ref triangle, out intersectionType);
			}


			/// <summary>
			/// Tests if a line intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Triangle3(ref Line3 line, ref Triangle3 triangle, out Line3Triangle3Intr info)
			{
				// Compute the offset origin, edges, and normal.
				Vector3D diff   = line.Center - triangle.V0;
				Vector3D edge1  = triangle.V1 - triangle.V0;
				Vector3D edge2  = triangle.V2 - triangle.V0;
				Vector3D normal = edge1.Cross(edge2);

				// Solve Q + t*D = b1*E1 + b2*E2 (Q = diff, D = line direction,
				// E1 = edge1, E2 = edge2, N = Cross(E1,E2)) by
				//   |Dot(D,N)|*b1 = sign(Dot(D,N))*Dot(D,Cross(Q,E2))
				//   |Dot(D,N)|*b2 = sign(Dot(D,N))*Dot(D,Cross(E1,Q))
				//   |Dot(D,N)|*t = -sign(Dot(D,N))*Dot(Q,N)
				double DdN = line.Direction.Dot(normal);
				double sign;
				if (DdN > _dotThreshold)
				{
					sign = 1f;
				}
				else if (DdN < -_dotThreshold)
				{
					sign = -1f;
					DdN = -DdN;
				}
				else
				{
					// Line and triangle are parallel, call it a "no intersection"
					// even if the line does intersect.
					info = new Line3Triangle3Intr();
					return false;
				}

				double DdQxE2 = sign * line.Direction.Dot(diff.Cross(edge2));
				if (DdQxE2 >= -Mathex.ZeroTolerance)
				{
					double DdE1xQ = sign * line.Direction.Dot(edge1.Cross(diff));
					if (DdE1xQ >= -Mathex.ZeroTolerance)
					{
						if (DdQxE2 + DdE1xQ <= DdN + Mathex.ZeroTolerance)
						{
							// Line intersects triangle.
							double QdN = -sign * diff.Dot(normal);
							double inv = 1f / DdN;
							
							info.IntersectionType = IntersectionTypes.Point;
							info.LineParameter    = QdN * inv;
							info.Point            = line.Eval(info.LineParameter);
							info.TriBary1         = DdQxE2 * inv;
							info.TriBary2         = DdE1xQ * inv;
							info.TriBary0         = 1f - info.TriBary1 - info.TriBary2;
							
							return true;
						}
						// else: b1+b2 > 1, no intersection
					}
					// else: b2 < 0, no intersection
				}
				// else: b1 < 0, no intersection

				info = new Line3Triangle3Intr();
				return false;
			}

			/// <summary>
			/// Tests if a line intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Triangle3(ref Line3 line, ref Vector3D v0, ref Vector3D v1, ref Vector3D v2, out Line3Triangle3Intr info)
			{
				Triangle3 triangle = new Triangle3() { V0 = v0, V1 = v1, V2 = v2 };
				return FindLine3Triangle3(ref line, ref triangle, out info);
			}

			/// <summary>
			/// Tests if a line intersects a triangle and finds intersection parameters. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindLine3Triangle3(ref Line3 line, Vector3D v0, Vector3D v1, Vector3D v2, out Line3Triangle3Intr info)
			{
				Triangle3 triangle = new Triangle3() { V0 = v0, V1 = v1, V2 = v2 };
				return FindLine3Triangle3(ref line, ref triangle, out info);
			}
		}
	}
}
