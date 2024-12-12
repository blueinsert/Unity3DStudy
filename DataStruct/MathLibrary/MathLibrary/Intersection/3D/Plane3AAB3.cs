

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a plane intersects a box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestPlane3AAB3(ref Plane3 plane, ref AAB3 box)
			{
				// Find points at end of diagonal nearest plane normal
				Vector3D dMin, dMax;

				if (plane.Normal.x >= 0.0f)
				{
					dMin.x = box.Min.x;
					dMax.x = box.Max.x;
				}
				else
				{
					dMin.x = box.Max.x;
					dMax.x = box.Min.x;
				}

				if (plane.Normal.y >= 0.0f)
				{
					dMin.y = box.Min.y;
					dMax.y = box.Max.y;
				}
				else
				{
					dMin.y = box.Max.y;
					dMax.y = box.Min.y;
				}

				if (plane.Normal.z >= 0.0f)
				{
					dMin.z = box.Min.z;
					dMax.z = box.Max.z;
				}
				else
				{
					dMin.z = box.Max.z;
					dMax.z = box.Min.z;
				}

				// Check if minimal point on diagonal is on positive side of plane
				if (plane.SignedDistanceTo(ref dMin) >= 0.0f)
				{
					return false;
				}
				else
				{
					// If minimal point is on negative side, then intersection occurs only if maximal point is on positive side
					return plane.SignedDistanceTo(ref dMax) > 0.0f;
				}
			}
		}
	}
}
