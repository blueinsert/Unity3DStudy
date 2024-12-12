

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a box
			/// </summary>
			public static double Point2Box2(ref Vector2D point, ref Box2 box)
			{
				return System.Math.Sqrt(SqrPoint2Box2(ref point, ref box));
			}

			/// <summary>
			/// Returns distance between a point and a box
			/// </summary>
			/// <param name="closestPoint">Point projected on a box</param>
			public static double Point2Box2(ref Vector2D point, ref Box2 box, out Vector2D closestPoint)
			{
				return System.Math.Sqrt(SqrPoint2Box2(ref point, ref box, out closestPoint));
			}


			/// <summary>
			/// Returns squared distance between a point and a box
			/// </summary>
			public static double SqrPoint2Box2(ref Vector2D point, ref Box2 box)
			{
				// Work in the box's coordinate system.
				Vector2D diff = point - box.Center;

				// Compute squared distance and closest point on box.
				double distSquared = 0.0f;
				double delta;
				double extent;

				double closest0 = diff.Dot(box.Axis0);
				extent = box.Extents.x;
				if (closest0 < -extent)
				{
					delta = closest0 + extent;
					distSquared += delta * delta;
				}
				else if (closest0 > extent)
				{
					delta = closest0 - extent;
					distSquared += delta * delta;
				}

				double closest1 = diff.Dot(box.Axis1);
				extent = box.Extents.y;
				if (closest1 < -extent)
				{
					delta = closest1 + extent;
					distSquared += delta * delta;
				}
				else if (closest1 > extent)
				{
					delta = closest1 - extent;
					distSquared += delta * delta;
				}

				return distSquared;
			}

			/// <summary>
			/// Returns squared distance between a point and a box
			/// </summary>
			/// <param name="closestPoint">Point projected on a box</param>
			public static double SqrPoint2Box2(ref Vector2D point, ref Box2 box, out Vector2D closestPoint)
			{
				// Work in the box's coordinate system.
				Vector2D diff = point - box.Center;

				// Compute squared distance and closest point on box.
				double distSquared = 0.0f;
				double delta;
				double extent;

				double closest0 = diff.Dot(box.Axis0);
				extent = box.Extents.x;
				if (closest0 < -extent)
				{
					delta = closest0 + extent;
					distSquared += delta * delta;
					closest0 = -extent;
				}
				else if (closest0 > extent)
				{
					delta = closest0 - extent;
					distSquared += delta * delta;
					closest0 = extent;
				}

				double closest1 = diff.Dot(box.Axis1);
				extent = box.Extents.y;
				if (closest1 < -extent)
				{
					delta = closest1 + extent;
					distSquared += delta * delta;
					closest1 = -extent;
				}
				else if (closest1 > extent)
				{
					delta = closest1 - extent;
					distSquared += delta * delta;
					closest1 = extent;
				}

				closestPoint = box.Center + closest0 * box.Axis0 + closest1 * box.Axis1;

				return distSquared;
			}
		}
	}
}
