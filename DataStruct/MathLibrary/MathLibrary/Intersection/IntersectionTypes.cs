namespace Dest
{
	namespace Math
	{
		public enum IntersectionTypes
		{
			/// <summary>
			/// Entities do not intersect
			/// </summary>
			Empty,
			
			/// <summary>
			/// Entities intersect in a point
			/// </summary>
			Point,
			
			/// <summary>
			/// Entities intersect in a segment
			/// </summary>
			Segment,
			
			/// <summary>
			/// Entities intersect in a ray
			/// </summary>
			Ray,
			
			/// <summary>
			/// Entities intersect in a line
			/// </summary>
			Line,
			
			/// <summary>
			/// Entities intersect in a polygon
			/// </summary>
			Polygon,
			
			/// <summary>
			/// Entities intersect in a plane
			/// </summary>
			Plane,
			
			/// <summary>
			/// Entities intersect in a polyhedron
			/// </summary>
			Polyhedron,
			
			/// <summary>
			/// Entities intersect somehow
			/// </summary>
			Other
		}
	}
}
