

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
			/// Tests if an axis aligned box intersects a circle. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestAAB2Circle2(ref AAB2 box, ref Circle2 circle)
			{
				// Initial distance is 0
				double distSquared = 0f;
				double centerCoord;
				double delta;

				// Compute distance in each direction, summing as we go.

				centerCoord = circle.Center.x;
				if (centerCoord < box.Min.x)
				{
					delta = centerCoord - box.Min.x;
					distSquared += delta * delta;
				}
				else if (centerCoord > box.Max.x)
				{
					delta = centerCoord - box.Max.x;
					distSquared += delta * delta;
				}

				centerCoord = circle.Center.y;
				if (centerCoord < box.Min.y)
				{
					delta = centerCoord - box.Min.y;
					distSquared += delta * delta;
				}
				else if (centerCoord > box.Max.y)
				{
					delta = centerCoord - box.Max.y;
					distSquared += delta * delta;
				}

				// Compare distance to radius squared
				return distSquared <= circle.Radius * circle.Radius;
			}
		}
	}
}
