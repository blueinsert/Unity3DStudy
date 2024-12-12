

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
			/// Tests whether two AAB intersect.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestAAB3AAB3(ref AAB3 box0, ref AAB3 box1)
			{
				if (box0.Max.x < box1.Min.x || box0.Min.x > box1.Max.x) return false;
				if (box0.Max.y < box1.Min.y || box0.Min.y > box1.Max.y) return false;
				if (box0.Max.z < box1.Min.z || box0.Min.z > box1.Max.z) return false;
				return true;
			}

			/// <summary>
			/// Tests whether two AAB intersect and finds intersection which is AAB itself.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindAAB3AAB3(ref AAB3 box0, ref AAB3 box1, out AAB3 intersection)
			{
				if (box0.Max.x < box1.Min.x || box0.Min.x > box1.Max.x)
				{
					intersection = new AAB3();
					return false;
				}

				if (box0.Max.y < box1.Min.y || box0.Min.y > box1.Max.y)
				{
					intersection = new AAB3();
					return false;
				}

				if (box0.Max.z < box1.Min.z || box0.Min.z > box1.Max.z)
				{
					intersection = new AAB3();
					return false;
				}

				intersection.Max.x = box0.Max.x <= box1.Max.x ? box0.Max.x : box1.Max.x;
				intersection.Min.x = box0.Min.x <= box1.Min.x ? box1.Min.x : box0.Min.x;

				intersection.Max.y = box0.Max.y <= box1.Max.y ? box0.Max.y : box1.Max.y;
				intersection.Min.y = box0.Min.y <= box1.Min.y ? box1.Min.y : box0.Min.y;

				intersection.Max.z = box0.Max.z <= box1.Max.z ? box0.Max.z : box1.Max.z;
				intersection.Min.z = box0.Min.z <= box1.Min.z ? box1.Min.z : box0.Min.z;

				return true;
			}

			/// <summary>
			/// Checks whether two aab has x overlap
			/// </summary>
			public static bool TestAAB3AAB3OverlapX(ref AAB3 box0, ref AAB3 box1)
			{
				return
					box0.Max.x >= box1.Min.x &&
					box0.Min.x <= box1.Max.x;
			}

			/// <summary>
			/// Checks whether two aab has y overlap
			/// </summary>
			public static bool TestAAB3AAB3OverlapY(ref AAB3 box0, ref AAB3 box1)
			{
				return
					box0.Max.y >= box1.Min.y &&
					box0.Min.y <= box1.Max.y;
			}

			/// <summary>
			/// Checks whether two aab has z overlap
			/// </summary>
			public static bool TestAAB3AAB3OverlapZ(ref AAB3 box0, ref AAB3 box1)
			{
				return
					box0.Max.z >= box1.Min.z &&
					box0.Min.z <= box1.Max.z;
			}
		}
	}
}
