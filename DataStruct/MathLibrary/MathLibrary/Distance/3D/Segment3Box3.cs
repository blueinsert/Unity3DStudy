

namespace Dest.Math
{
	public static partial class Distance
	{
		public static double Segment3Box3(ref Segment3 segment, ref Box3 box, out Vector3D closestPoint0, out Vector3D closestPoint1)
		{
			return System.Math.Sqrt(SqrSegment3Box3(ref segment, ref box, out closestPoint0, out closestPoint1));
		}

		public static double Segment3Box3(ref Segment3 segment, ref Box3 box)
		{
			Vector3D closestPoint0, closestPoint1;
			return System.Math.Sqrt(SqrSegment3Box3(ref segment, ref box, out closestPoint0, out closestPoint1));
		}


		public static double SqrSegment3Box3(ref Segment3 segment, ref Box3 box, out Vector3D closestPoint0, out Vector3D closestPoint1)
		{
			Line3 line = new Line3(segment.Center, segment.Direction);
			
			Line3Box3Dist info;
			double sqrDistance = SqrLine3Box3(ref line, ref box, out info);
			double lineParameter = info.LineParameter;

			if (lineParameter >= -segment.Extent)
			{
				if (lineParameter <= segment.Extent)
				{
					closestPoint0 = info.ClosestPoint0;
					closestPoint1 = info.ClosestPoint1;
				}
				else
				{
					closestPoint0 = segment.P1;
					sqrDistance = SqrPoint3Box3(ref segment.P1, ref box, out closestPoint1);
				}
			}
			else
			{
				closestPoint0 = segment.P0;
				sqrDistance = SqrPoint3Box3(ref segment.P0, ref box, out closestPoint1);
			}

			return sqrDistance;
		}

		public static double SqrSegment3Box3(ref Segment3 segment, ref Box3 box)
		{
			Vector3D closestPoint0, closestPoint1;
			return SqrSegment3Box3(ref segment, ref box, out closestPoint0, out closestPoint1);
		}
	}
}
