

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a plane intersects a box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestPlane3Box3(ref Plane3 plane, ref Box3 box)
			{
				double tmp0 = box.Extents.x * (plane.Normal.Dot(box.Axis0));
				double tmp1 = box.Extents.y * (plane.Normal.Dot(box.Axis1));
				double tmp2 = box.Extents.z * (plane.Normal.Dot(box.Axis2));

				double radius = System.Math.Abs(tmp0) + System.Math.Abs(tmp1) + System.Math.Abs(tmp2);

				double signedDistance = plane.SignedDistanceTo(ref box.Center);
				return System.Math.Abs(signedDistance) <= radius;
			}
		}
	}
}
