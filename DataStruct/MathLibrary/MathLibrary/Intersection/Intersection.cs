namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Contains various intersection methods.
		/// </summary>
		public static partial class Intersection
		{
			private static double _intervalThreshold;
			private static double _dotThreshold;
			private static double _distanceThreshold;

			/// <summary>
			/// Used in interval comparisons. Default is MathfEx.ZeroTolerance.
			/// </summary>
			public static double IntervalThreshold
			{
				get { return _intervalThreshold; }
				set
				{
					if (value >= 0f)
					{
						_intervalThreshold = value;
						return;
					}
				}
			}

			/// <summary>
			/// Used in dot product comparisons. Default is MathfEx.ZeroTolerance.
			/// </summary>
			public static double DotThreshold
			{
				get { return _dotThreshold; }
				set
				{
					if (value >= 0f)
					{
						_dotThreshold = value;
						return;
					}
				}
			}

			/// <summary>
			/// Used in distance comparisons. Default is MathfEx.ZeroTolerance.
			/// </summary>
			public static double DistanceThreshold
			{
				get { return _distanceThreshold; }
				set
				{
					if (value >= 0f)
					{
						_distanceThreshold = value;
						return;
					}
				}
			}

			static Intersection()
			{
				_intervalThreshold = _dotThreshold = _distanceThreshold = Mathex.ZeroTolerance;
			}

			/// <summary>
			/// Finds intersection of 1d intervals. Endpoints of the intervals must be sorted,
			/// i.e. seg0Start must be &lt;= seg0End, seg1Start must be &lt;= seg1End. Returns 0 if
			/// intersection is empty, 1 - if intervals intersect in one point, 2 - if intervals
			/// intersect in segment. w0 and w1 will contain intersection point in case intersection occurs.
			/// </summary>
			public static int FindSegment1Segment1(double seg0Start, double seg0End, double seg1Start, double seg1End, out double w0, out double w1)
			{
				w0 = w1 = 0f;
				double eps = _distanceThreshold;

				if (seg0End < (seg1Start - eps) || seg0Start > (seg1End + eps)) return 0;

				if (seg0End > seg1Start + eps)
				{
					if (seg0Start < seg1End - eps)
					{
						if (seg0Start < seg1Start)
						{
							w0 = seg1Start;
						}
						else
						{
							w0 = seg0Start;
						}

						if (seg0End > seg1End)
						{
							w1 = seg1End;
						}
						else
						{
							w1 = seg0End;
						}

						if (w1 - w0 <= eps)
						{
							return 1;
						}

						return 2;
					}
					else
					{
						// u0 == v1
						w0 = seg0Start;
						return 1;
					}
				}
				else
				{
					// u1 == v0
					w0 = seg0End;
					return 1;
				}
			}

            /// <summary>
            /// Sphere, Capsule intersection test contact information
            /// </summary>
            public struct ContactInfo
            {
                public Vector3D Point;
                public Vector3D Normal;
                public double Depth;
            }
		}
	}
}
