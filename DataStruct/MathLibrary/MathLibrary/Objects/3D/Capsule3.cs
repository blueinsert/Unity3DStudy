

namespace Dest.Math
{
	/// <summary>
	/// Capsule is defined by the volume around the segment with certain radius.
	/// </summary>
	public struct Capsule3
	{
		/// <summary>
		/// Capsule base segment
		/// </summary>
		public Segment3 Segment;
		
		/// <summary>
		/// Capsule radius
		/// </summary>
		public double Radius;


		/// <summary>
		/// Creates new Capsule3 instance.
		/// </summary>
		public Capsule3(ref Segment3 segment, double radius)
		{
			Segment = segment;
			Radius = radius;
		}

		/// <summary>
		/// Creates new Capsule3 instance.
		/// </summary>
		public Capsule3(Segment3 segment, double radius)
		{
			Segment = segment;
			Radius = radius;
		}
	}
}
