

namespace Dest
{
	namespace Math
	{
		public static partial class Distance
		{
			/// <summary>
			/// Returns distance between a point and a circle
			/// </summary>
			/// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
			public static double Point3Circle3(ref Vector3D point, ref Circle3 circle, bool solid = true)
			{
				Vector3D closestPoint;
				return System.Math.Sqrt(SqrPoint3Circle3(ref point, ref circle, out closestPoint, solid));
			}

			/// <summary>
			/// Returns distance between a point and a circle
			/// </summary>
			/// <param name="closestPoint">Point projected on a circle</param>
			/// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
			public static double Point3Circle3(ref Vector3D point, ref Circle3 circle, out Vector3D closestPoint, bool solid = true)
			{
				return System.Math.Sqrt(SqrPoint3Circle3(ref point, ref circle, out closestPoint, solid));
			}


			/// <summary>
			/// Returns squared distance between a point and a circle
			/// </summary>
			/// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
			public static double SqrPoint3Circle3(ref Vector3D point, ref Circle3 circle, bool solid = true)
			{
				Vector3D closestPoint;
				return SqrPoint3Circle3(ref point, ref circle, out closestPoint, solid);
			}

			/// <summary>
			/// Returns squared distance between a point and a circle
			/// </summary>
			/// <param name="closestPoint">Point projected on a circle</param>
			/// <param name="solid">When true circle considered to be solid disk otherwise hollow circle.</param>
			public static double SqrPoint3Circle3(ref Vector3D point, ref Circle3 circle, out Vector3D closestPoint, bool solid = true)
			{
				if (solid)
				{
					// Signed distance from point to plane of circle.
					Vector3D diff0 = point - circle.Center;
					double dist = diff0.Dot(circle.Normal);

					// Projection of P-C onto plane is Q-C = P-C - (fDist)*N.
					Vector3D diff1 = diff0 - dist * circle.Normal;
					double sqrLen = diff1.sqrMagnitude;
					double sqrDistance;

					if (sqrLen > circle.Radius)
					{
						closestPoint = circle.Center + (circle.Radius / System.Math.Sqrt(sqrLen)) * diff1;
						Vector3D diff2 = point - closestPoint;
						sqrDistance = diff2.sqrMagnitude;
					}
					else
					{
						// Projection is inside the disk, closest point is projection itself
						closestPoint = circle.Center + diff1;
						sqrDistance = dist * dist;
					}

					return sqrDistance;
				}
				else
				{
					// Signed distance from point to plane of circle.
					Vector3D diff0 = point - circle.Center;
					double dist = diff0.Dot(circle.Normal);

					// Projection of P-C onto plane is Q-C = P-C - (fDist)*N.
					Vector3D diff1 = diff0 - dist * circle.Normal;
					double sqrLen = diff1.sqrMagnitude;
					double sqrDistance;

					if (sqrLen >= Mathex.ZeroTolerance)
					{
						closestPoint = circle.Center + (circle.Radius / System.Math.Sqrt(sqrLen)) * diff1;
						Vector3D diff2 = point - closestPoint;
						sqrDistance = diff2.sqrMagnitude;
					}
					else
					{
						// All circle points are equidistant to the input point, pick any circle point as closest
						closestPoint = circle.Eval(0);
						sqrDistance = circle.Radius * circle.Radius + dist * dist;
					}

					return sqrDistance;
				}
			}
		}
	}
}
