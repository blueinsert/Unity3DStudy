

namespace Dest
{
	namespace Math
	{
		public static partial class Intersection
		{
			/// <summary>
			/// Tests if a box intersects another box. Returns true if intersection occurs false otherwise.
			/// </summary>
			public static bool TestBox2Box2(ref Box2 box0, ref Box2 box1)
			{
				Vector2D A0 = box0.Axis0;
				Vector2D A1 = box0.Axis1;
				Vector2D B0 = box1.Axis0;
				Vector2D B1 = box1.Axis1;

				double EA0 = box0.Extents.x;
				double EA1 = box0.Extents.y;
				double EB0 = box1.Extents.x;
				double EB1 = box1.Extents.y;

				// Compute difference of box centers, D = C1-C0.
				Vector2D D = box1.Center - box0.Center;

				double AbsAdB00, AbsAdB01, AbsAdB10, AbsAdB11;
				double AbsAdD, RSum;
    
				
				// axis C0+t*A0
				AbsAdB00 = System.Math.Abs(A0.Dot(B0));
				AbsAdB01 = System.Math.Abs(A0.Dot(B1));
				AbsAdD   = System.Math.Abs(A0.Dot(D));
				RSum     = EA0 + EB0 * AbsAdB00 + EB1 * AbsAdB01;
				if (AbsAdD > RSum)
				{
					return false;
				}

				// axis C0+t*A1
				AbsAdB10 = System.Math.Abs(A1.Dot(B0));
				AbsAdB11 = System.Math.Abs(A1.Dot(B1));
				AbsAdD   = System.Math.Abs(A1.Dot(D));
				RSum     = EA1 + EB0 * AbsAdB10 + EB1 * AbsAdB11;
				if (AbsAdD > RSum)
				{
					return false;
				}


				// axis C0+t*B0
				AbsAdD = System.Math.Abs(B0.Dot(D));
				RSum   = EB0 + EA0 * AbsAdB00 + EA1 * AbsAdB10;
				if (AbsAdD > RSum)
				{
					return false;
				}

				// axis C0+t*B1
				AbsAdD = System.Math.Abs(B1.Dot(D));
				RSum   = EB1 + EA0 * AbsAdB01 + EA1 * AbsAdB11;
				if (AbsAdD > RSum)
				{
					return false;
				}

				return true;
			}
		}
	}
}
