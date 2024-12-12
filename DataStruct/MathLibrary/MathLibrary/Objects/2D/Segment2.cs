

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The segment is represented as (1-s)*P0+s*P1, where P0 and P1 are the
		/// endpoints of the segment and 0 &lt;= s &lt;= 1.
		///
		/// Some algorithms involving segments might prefer a centered
		/// representation similar to how oriented bounding boxes are defined.
		/// This representation is C+t*D, where C = (P0+P1)/2 is the center of
		/// the segment, D = (P1-P0)/Length(P1-P0) is a unit-length direction
		/// vector for the segment, and |t| &lt;= e.  The value e = Length(P1-P0)/2
		/// is the 'extent' (or radius or half-length) of the segment.
		/// </summary>
		public struct Segment2
		{
			/// <summary>
			/// Start point
			/// </summary>
			public Vector2D P0;
			
			/// <summary>
			/// End point
			/// </summary>
			public Vector2D P1;

			/// <summary>
			/// Segment center
			/// </summary>
			public Vector2D Center;

			/// <summary>
			/// Segment direction. Must be unit length!
			/// </summary>
			public Vector2D Direction;

			/// <summary>
			/// Segment half-length
			/// </summary>
			public double Extent;


			/// <summary>
			/// The constructor computes Center, Dircetion, and Extent from P0 and P1.
			/// </summary>
			/// <param name="p0">Segment start point</param>
			/// <param name="p1">Segment end point</param>
			public Segment2(ref Vector2D p0, ref Vector2D p1)
			{
				P0 = p0;
				P1 = p1;
				Center = Direction = Vector2D.zero;
				Extent = 0f;
				CalcCenterDirectionExtent();
			}

			/// <summary>
			/// The constructor computes Center, Dircetion, and Extent from P0 and P1.
			/// </summary>
			/// <param name="p0">Segment start point</param>
			/// <param name="p1">Segment end point</param>
			public Segment2(Vector2D p0, Vector2D p1)
			{
				P0 = p0;
				P1 = p1;
				Center = Direction = Vector2D.zero;
				Extent = 0f;
				CalcCenterDirectionExtent();
			}

			/// <summary>
			/// The constructor computes P0 and P1 from Center, Direction, and Extent.
			/// </summary>
			/// <param name="center">Center of the segment</param>
			/// <param name="direction">Direction of the segment. Must be unit length!</param>
			/// <param name="extent">Half-length of the segment</param>
			public Segment2(ref Vector2D center, ref Vector2D direction, double extent)
			{
				Center = center;
				Direction = direction;
				Extent = extent;
				P0 = P1 = Vector2D.zero;
				CalcEndPoints();
			}

			/// <summary>
			/// The constructor computes P0 and P1 from Center, Direction, and Extent.
			/// </summary>
			/// <param name="center">Center of the segment</param>
			/// <param name="direction">Direction of the segment. Must be unit length!</param>
			/// <param name="extent">Half-length of the segment</param>
			public Segment2(Vector2D center, Vector2D direction, double extent)
			{
				Center = center;
				Direction = direction;
				Extent = extent;
				P0 = P1 = Vector2D.zero;
				CalcEndPoints();
			}			

			
			/// <summary>
			/// Initializes segments from endpoints.
			/// </summary>
			public void SetEndpoints(Vector2D p0, Vector2D p1)
			{
				P0 = p0;
				P1 = p1;
				CalcCenterDirectionExtent();
			}

			/// <summary>
			/// Initializes segment from center, direction and extent.
			/// </summary>
			public void SetCenterDirectionExtent(Vector2D center, Vector2D direction, double extent)
			{
				Center = center;
				Direction = direction;
				Extent = extent;
				CalcEndPoints();
			}

			/// <summary>
			/// Call this function when you change P0 or P1.
			/// </summary>
			public void CalcCenterDirectionExtent()
			{
				Center = 0.5f * (P0 + P1);
				Direction = P1 - P0;
				double directionLength = Direction.magnitude;
				double invDirectionLength = 1f / directionLength;
				Direction *= invDirectionLength;
				Extent = 0.5f * directionLength;
			}

			/// <summary>
			/// Call this function when you change Center, Direction, or Extent.
			/// </summary>
			public void CalcEndPoints()
			{
				P0 = Center - Extent * Direction;
				P1 = Center + Extent * Direction;
			}

			/// <summary>
			/// Evaluates segment using (1-s)*P0+s*P1 formula, where P0 and P1
			/// are endpoints, s is parameter.
			/// </summary>
			/// <param name="s">Evaluation parameter</param>
			public Vector2D Eval(double s)
			{
				return (1f - s) * P0 + s * P1;
			}

			/// <summary>
			/// Returns distance to a point, distance is >= 0f.
			/// </summary>
			public double DistanceTo(Vector2D point)
			{
				return Distance.Point2Segment2(ref point, ref this);
			}

			/// <summary>
			/// Returns projected point
			/// </summary>
			public Vector2D Project(Vector2D point)
			{
				Vector2D result;
				Distance.SqrPoint2Segment2(ref point, ref this, out result);
				return result;
			}

			/// <summary>
			/// Returns string representation.
			/// </summary>
			public override string ToString()
			{
				return string.Format("[P0: {0} P1: {1} Center: {2} Direction: {3} Extent: {4}]", P0.ToStringEx(), P1.ToStringEx(), Center.ToStringEx(), Direction.ToStringEx(), Extent.ToString());
			}
		}
	}
}
