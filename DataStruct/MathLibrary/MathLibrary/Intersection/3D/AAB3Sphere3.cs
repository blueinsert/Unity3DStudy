

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
			/// Tests if an axis aligned box intersects a sphere. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestAAB3Sphere3(ref AAB3 box, ref Sphere3 sphere)
			{
				// Initial distance is 0
				double distSquared = 0f;
				double centerCoord;
				double delta;

				// Compute distance in each direction, summing as we go.

				centerCoord = sphere.Center.x;
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

				centerCoord = sphere.Center.y;
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

				centerCoord = sphere.Center.z;
				if (centerCoord < box.Min.z)
				{
					delta = centerCoord - box.Min.z;
					distSquared += delta * delta;
				}
				else if (centerCoord > box.Max.z)
				{
					delta = centerCoord - box.Max.z;
					distSquared += delta * delta;
				}

				// Compare distance to radius squared
				return distSquared <= sphere.Radius * sphere.Radius;
			}
		}
	}
}
