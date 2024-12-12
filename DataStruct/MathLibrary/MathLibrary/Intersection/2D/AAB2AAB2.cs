

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
			public static bool TestAAB2AAB2(ref AAB2 box0, ref AAB2 box1)
			{
				if (box0.Max.x < box1.Min.x || box0.Min.x > box1.Max.x) return false;
				if (box0.Max.y < box1.Min.y || box0.Min.y > box1.Max.y) return false;
				return true;
			}

			/// <summary>
			/// Tests whether two AAB intersect and finds intersection which is AAB itself.
			/// Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool FindAAB2AAB2(ref AAB2 box0, ref AAB2 box1, out AAB2 intersection)
			{
				if (box0.Max.x < box1.Min.x || box0.Min.x > box1.Max.x)
				{
					intersection = new AAB2();
					return false;
				}

				if (box0.Max.y < box1.Min.y || box0.Min.y > box1.Max.y)
				{
					intersection = new AAB2();
					return false;
				}

				intersection.Max.x = box0.Max.x <= box1.Max.x ? box0.Max.x : box1.Max.x;
				intersection.Min.x = box0.Min.x <= box1.Min.x ? box1.Min.x : box0.Min.x;

				intersection.Max.y = box0.Max.y <= box1.Max.y ? box0.Max.y : box1.Max.y;
				intersection.Min.y = box0.Min.y <= box1.Min.y ? box1.Min.y : box0.Min.y;

				return true;
			}

			/// <summary>
			/// Checks whether two aab has x overlap
			/// </summary>
			public static bool TestAAB2AAB2OverlapX(ref AAB2 box0, ref AAB2 box1)
			{
				return
					box0.Max.x >= box1.Min.x &&
					box0.Min.x <= box1.Max.x;
			}

			/// <summary>
			/// Checks whether two aab has y overlap
			/// </summary>
			public static bool TestAAB2AAB2OverlapY(ref AAB2 box0, ref AAB2 box1)
			{
				return
					box0.Max.y >= box1.Min.y &&
					box0.Min.y <= box1.Max.y;
			}
		}
	}
}
