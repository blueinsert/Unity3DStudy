

namespace Dest.Math
{
	public static partial class Intersection
	{
		/// <summary>
		/// Tests if a box intersects a capsule. Returns true if intersection occurs false otherwise.
		/// </summary>
		public static bool TestBox3Capsule3(ref Box3 box, ref Capsule3 capsule)
		{
			double dist = Distance.Segment3Box3(ref capsule.Segment, ref box);
			return dist <= capsule.Radius;
		}
	}
}
