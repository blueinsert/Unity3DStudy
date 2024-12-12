

namespace Dest
{
	namespace Math
	{
		internal enum QueryTypes
		{
			Int64,
			Integer,
			Rational,
			Real,
			Filtered
		};

		internal class Query
		{
			protected Query()
			{
			}

			// Support for ordering a set of unique indices into the vertex pool.  On
			// output it is guaranteed that:  v0 < v1 < v2.  This is used to guarantee
			// consistent queries when the vertex ordering of a primitive is permuted,
			// a necessity when using floating-point arithmetic that suffers from
			// numerical round-off errors.  The input indices are considered the
			// positive ordering.  The output indices are either positively ordered
			// (an even number of transpositions occurs during sorting) or negatively
			// ordered (an odd number of transpositions occurs during sorting).  The
			// functions return 'true' for a positive ordering and 'false' for a
			// negative ordering.

			public static bool Sort(ref int v0, ref int v1)
			{
				if (v0 < v1)
				{
					return true;
				}
				
				int temp = v0;
				v0 = v1;
				v1 = temp;
				return false;
			}

			public static bool Sort(ref int v0, ref int v1, ref int v2)
			{
				int j0, j1, j2;
				bool positive;

				if (v0 < v1)
				{
					if (v2 < v0)
					{
						j0 = v2; j1 = v0; j2 = v1; positive = true;
					}
					else if (v2 < v1)
					{
						j0 = v0; j1 = v2; j2 = v1; positive = false;
					}
					else
					{
						j0 = v0; j1 = v1; j2 = v2; positive = true;
					}
				}
				else
				{
					if (v2 < v1)
					{
						j0 = v2; j1 = v1; j2 = v0; positive = false;
					}
					else if (v2 < v0)
					{
						j0 = v1; j1 = v2; j2 = v0; positive = true;
					}
					else
					{
						j0 = v1; j1 = v0; j2 = v2; positive = false;
					}
				}

				v0 = j0;
				v1 = j1;
				v2 = j2;
				return positive;
			}

			public static bool Sort(ref int v0, ref int v1, ref int v2, ref int v3)
			{
				int j0, j1, j2, j3;
				bool positive;

				if (v0 < v1)
				{
					if (v2 < v3)
					{
						if (v1 < v2)
						{
							j0 = v0; j1 = v1; j2 = v2; j3 = v3; positive = true;
						}
						else if (v3 < v0)
						{
							j0 = v2; j1 = v3; j2 = v0; j3 = v1; positive = true;
						}
						else if (v2 < v0)
						{
							if (v3 < v1)
							{
								j0 = v2; j1 = v0; j2 = v3; j3 = v1; positive = false;
							}
							else
							{
								j0 = v2; j1 = v0; j2 = v1; j3 = v3; positive = true;
							}
						}
						else
						{
							if (v3 < v1)
							{
								j0 = v0; j1 = v2; j2 = v3; j3 = v1; positive = true;
							}
							else
							{
								j0 = v0; j1 = v2; j2 = v1; j3 = v3; positive = false;
							}
						}
					}
					else
					{
						if (v1 < v3)
						{
							j0 = v0; j1 = v1; j2 = v3; j3 = v2; positive = false;
						}
						else if (v2 < v0)
						{
							j0 = v3; j1 = v2; j2 = v0; j3 = v1; positive = false;
						}
						else if (v3 < v0)
						{
							if (v2 < v1)
							{
								j0 = v3; j1 = v0; j2 = v2; j3 = v1; positive = true;
							}
							else
							{
								j0 = v3; j1 = v0; j2 = v1; j3 = v2; positive = false;
							}
						}
						else
						{
							if (v2 < v1)
							{
								j0 = v0; j1 = v3; j2 = v2; j3 = v1; positive = false;
							}
							else
							{
								j0 = v0; j1 = v3; j2 = v1; j3 = v2; positive = true;
							}
						}
					}
				}
				else
				{
					if (v2 < v3)
					{
						if (v0 < v2)
						{
							j0 = v1; j1 = v0; j2 = v2; j3 = v3; positive = false;
						}
						else if (v3 < v1)
						{
							j0 = v2; j1 = v3; j2 = v1; j3 = v0; positive = false;
						}
						else if (v2 < v1)
						{
							if (v3 < v0)
							{
								j0 = v2; j1 = v1; j2 = v3; j3 = v0; positive = true;
							}
							else
							{
								j0 = v2; j1 = v1; j2 = v0; j3 = v3; positive = false;
							}
						}
						else
						{
							if (v3 < v0)
							{
								j0 = v1; j1 = v2; j2 = v3; j3 = v0; positive = false;
							}
							else
							{
								j0 = v1; j1 = v2; j2 = v0; j3 = v3; positive = true;
							}
						}
					}
					else
					{
						if (v0 < v3)
						{
							j0 = v1; j1 = v0; j2 = v3; j3 = v2; positive = true;
						}
						else if (v2 < v1)
						{
							j0 = v3; j1 = v2; j2 = v1; j3 = v0; positive = true;
						}
						else if (v3 < v1)
						{
							if (v2 < v0)
							{
								j0 = v3; j1 = v1; j2 = v2; j3 = v0; positive = false;
							}
							else
							{
								j0 = v3; j1 = v1; j2 = v0; j3 = v2; positive = true;
							}
						}
						else
						{
							if (v2 < v0)
							{
								j0 = v1; j1 = v3; j2 = v2; j3 = v0; positive = true;
							}
							else
							{
								j0 = v1; j1 = v3; j2 = v0; j3 = v2; positive = false;
							}
						}
					}
				}

				v0 = j0;
				v1 = j1;
				v2 = j2;
				v3 = j3;
				return positive;
			}
		}
	}
}
