

namespace Dest
{
	namespace Math
	{
		public struct QuadraticRoots
		{
			public double X0;
			public double X1;
			public int   RootCount;
			public double this[int rootIndex]
			{
				get
				{
					switch (rootIndex)
					{
						case 0: return X0;
						case 1: return X1;
					}
					return double.NaN;
				}
			}
		}

		public struct CubicRoots
		{
			public double X0;
			public double X1;
			public double X2;
			public int   RootCount;
			public double this[int rootIndex]
			{
				get
				{
					switch (rootIndex)
					{
						case 0: return X0;
						case 1: return X1;
						case 2: return X2;
					}
					return double.NaN;
				}
			}
		}

		public struct QuarticRoots
		{
			public double X0;
			public double X1;
			public double X2;
			public double X3;
			public int   RootCount;
			public double this[int rootIndex]
			{
				get
				{
					switch (rootIndex)
					{
						case 0: return X0;
						case 1: return X1;
						case 2: return X2;
						case 3: return X3;
					}
					return double.NaN;
				}
			}
		}

		public static partial class RootFinder
		{
			private class PolyRootFinder
			{
				private int     _count;
				private int     _maxRoot;
				private double[] _roots;
				private double   _epsilon;

				public double[] Roots { get { return _roots; } }

				public PolyRootFinder(double epsilon)
				{
					_count   = 0;
					_maxRoot = 4;  // default support for degree <= 4
					_roots   = new double[_maxRoot];
					_epsilon = epsilon;
				}

				public bool Bisection(Polynomial poly, double xMin, double xMax, int digits, out double root)
				{
					double p0 = poly.Eval(xMin);
					if (System.Math.Abs(p0) <= Mathex.ZeroTolerance)
					{
						root = xMin;
						return true;
					}
					double p1 = poly.Eval(xMax);
					if (System.Math.Abs(p1) <= Mathex.ZeroTolerance)
					{
						root = xMax;
						return true;
					}

					root = double.NaN;

					if (p0 * p1 > (double)0)
					{
						return false;
					}

					// Determine number of iterations to get 'digits' accuracy..
					double tmp0 = System.Math.Log(xMax - xMin);
					double tmp1 = ((double)digits) * System.Math.Log((double)10);
					double arg = (tmp0 + tmp1) / System.Math.Log((double)2);
					int maxIter = (int)(arg + (double)0.5);

					for (int i = 0; i < maxIter; ++i)
					{
						root = ((double)0.5) * (xMin + xMax);
						double p = poly.Eval(root);
						double product = p * p0;
						if (product < (double)0)
						{
							xMax = root;
							p1 = p;
						}
						else if (product > (double)0)
						{
							xMin = root;
							p0 = p;
						}
						else
						{
							break;
						}
					}

					return true;
				}

				public bool Find(Polynomial poly, double xMin, double xMax, int digits)
				{
					// Reallocate root array if necessary.
					if (poly.Degree > _maxRoot)
					{
						_maxRoot = poly.Degree;
						_roots = new double[_maxRoot];
					}

					double root;
					if (poly.Degree == 1)
					{
						if (Bisection(poly, xMin, xMax, digits, out root) && root != double.NaN)
						{
							_count = 1;
							_roots[0] = root;
							return true;
						}
						_count = 0;
						return false;
					}

					// Get roots of derivative polynomial.
					Polynomial deriv = poly.CalcDerivative();
					Find(deriv, xMin, xMax, digits);

					int i, newCount = 0;
					double[] newRoot = new double[_count + 1];

					if (_count > 0)
					{
						// Find root on [xmin,root[0]].
						if (Bisection(poly, xMin, _roots[0], digits, out root))
						{
							newRoot[newCount++] = root;
						}

						// Find root on [root[i],root[i+1]] for 0 <= i <= count-2.
						for (i = 0; i <= _count - 2; ++i)
						{
							if (Bisection(poly, _roots[i], _roots[i + 1], digits, out root))
							{
								newRoot[newCount++] = root;
							}
						}

						// Find root on [root[count-1],xmax].
						if (Bisection(poly, _roots[_count - 1], xMax, digits, out root))
						{
							newRoot[newCount++] = root;
						}
					}
					else
					{
						// Polynomial is monotone on [xmin,xmax], has at most one root.
						if (Bisection(poly, xMin, xMax, digits, out root))
						{
							newRoot[newCount++] = root;
						}
					}

					// Copy to old buffer.
					if (newCount > 0)
					{
						_count = 1;
						_roots[0] = newRoot[0];
						for (i = 1; i < newCount; ++i)
						{
							double rootDiff = newRoot[i] - newRoot[i - 1];
							if (System.Math.Abs(rootDiff) > _epsilon)
							{
								_roots[_count++] = newRoot[i];
							}
						}
					}
					else
					{
						_count = 0;
					}

					return _count > 0;
				}
			}

			private const  double third         = 1f / 3f;
			private const  double twentySeventh = 1f / 27f;
			private static double sqrt3         = System.Math.Sqrt(3f);

			/// <summary>
			/// Linear equations: c1*x+c0 = 0
			/// </summary>
			public static bool Linear(double c0, double c1, out double root, double epsilon = Mathex.ZeroTolerance)
			{
				if (System.Math.Abs(c1) >= epsilon)
				{
					root = -c0 / c1;
					return true;
				}

				root = double.NaN;
				return false;
			}

			/// <summary>
			/// Quadratic equations: c2*x^2+c1*x+c0 = 0
			/// </summary>
			public static bool Quadratic(double c0, double c1, double c2, out QuadraticRoots roots, double epsilon = Mathex.ZeroTolerance)
			{
				if (System.Math.Abs(c2) <= epsilon)
				{
					// Polynomial is linear.
					double root;
					bool result = Linear(c0, c1, out root, epsilon);
					if (result)
					{
						roots.X0 = root;
						roots.X1 = double.NaN;
						roots.RootCount = 1;
					}
					else
					{
						roots.X0 = double.NaN;
						roots.X1 = double.NaN;
						roots.RootCount = 0;
					}
					return result;
				}

				double discr = c1 * c1 - 4f * c0 * c2;
				if (System.Math.Abs(discr) <= epsilon)
				{
					discr = 0f;
				}

				if (discr < 0f)
				{
					roots.X0 = double.NaN;
					roots.X1 = double.NaN;
					roots.RootCount = 0;
					return false;
				}

				double tmp = 0.5f / c2;

				if (discr > 0f)
				{
					discr = System.Math.Sqrt(discr);
					roots.X0 = tmp * (-c1 - discr);
					roots.X1 = tmp * (-c1 + discr);
					roots.RootCount = 2;
				}
				else
				{
					roots.X0 = -tmp * c1;
					roots.X1 = double.NaN;
					roots.RootCount = 1;
				}

				return true;
			}

			/// <summary>
			/// Cubic equations: c3*x^3+c2*x^2+c1*x+c0 = 0
			/// </summary>
			public static bool Cubic(double c0, double c1, double c2, double c3, out CubicRoots roots, double epsilon = Mathex.ZeroTolerance)
			{
				if (System.Math.Abs(c3) <= epsilon)
				{
					// Polynomial is quadratic.
					QuadraticRoots tempRoots;
					bool result = Quadratic(c0, c1, c2, out tempRoots, epsilon);
					if (result)
					{	
						roots.X0 = tempRoots.X0;
						roots.X1 = tempRoots.X1;
						roots.X2 = double.NaN;
						roots.RootCount = tempRoots.RootCount;
					}
					else
					{
						roots.X0 = double.NaN;
						roots.X1 = double.NaN;
						roots.X2 = double.NaN;
						roots.RootCount = 0;
					}
					return result;
				}

				// Make polynomial monic, x^3+c2*x^2+c1*x+c0.
				double invC3 = 1f / c3;
				c2 *= invC3;
				c1 *= invC3;
				c0 *= invC3;

				// Convert to y^3+a*y+b = 0 by x = y-c2/3.
				double offset = third * c2;
				double a = c1 - c2 * offset;
				double b = c0 + c2 * (2f * c2 * c2 - 9f * c1) * twentySeventh;
				double halfB = 0.5f * b;

				double discr = halfB * halfB + a * a * a * twentySeventh;
				if (System.Math.Abs(discr) <= epsilon)
				{
					discr = 0f;
				}

				if (discr > 0f)  // 1 real, 2 complex roots
				{
					discr = System.Math.Sqrt(discr);
					double temp = -halfB + discr;
					if (temp >= 0f)
					{
						roots.X0 = System.Math.Pow(temp, third);
					}
					else
					{
						roots.X0 = -System.Math.Pow(-temp, third);
					}
					temp = -halfB - discr;
					if (temp >= 0f)
					{
						roots.X0 += System.Math.Pow(temp, third);
					}
					else
					{
						roots.X0 -= System.Math.Pow(-temp, third);
					}

					roots.X0 -= offset;
					roots.X1 = double.NaN;
					roots.X2 = double.NaN;
					roots.RootCount = 1;
				}
				else if (discr < 0f)
				{
					double dist = System.Math.Sqrt(-third * a);
					double angle = third * System.Math.Atan2(System.Math.Sqrt(-discr), -halfB);
					double cs = System.Math.Cos(angle);
					double sn = System.Math.Sin(angle);

					roots.X0 = 2f * dist * cs - offset;
					roots.X1 = -dist * (cs + sqrt3 * sn) - offset;
					roots.X2 = -dist * (cs - sqrt3 * sn) - offset;
					roots.RootCount = 3;
				}
				else
				{
					double temp;
					if (halfB >= 0f)
					{
						temp = -System.Math.Pow(halfB, third);
					}
					else
					{
						temp = System.Math.Pow(-halfB, third);
					}

					roots.X0 = 2f * temp - offset;
					roots.X1 = -temp - offset;
					roots.X2 = roots.X1;
					roots.RootCount = 3;
				}

				return true;
			}

			/// <summary>
			/// Quartic equations: c4*x^4+c3*x^3+c2*x^2+c1*x+c0 = 0
			/// </summary>
			public static bool Quartic(double c0, double c1, double c2, double c3, double c4, out QuarticRoots roots, double epsilon = Mathex.ZeroTolerance)
			{
				roots.X0 = double.NaN;
				roots.X1 = double.NaN;
				roots.X2 = double.NaN;
				roots.X3 = double.NaN;

				if (System.Math.Abs(c4) <= epsilon)
				{
					// Polynomial is cubic.
					CubicRoots tempRoots;
					bool result = Cubic(c0, c1, c2, c3, out tempRoots, epsilon);
					if (result)
					{
						roots.X0 = tempRoots.X0;
						roots.X1 = tempRoots.X1;
						roots.X2 = tempRoots.X2;
						roots.RootCount = tempRoots.RootCount;
					}
					else
					{
						roots.RootCount = 0;
					}
					return result;
				}

				// Make polynomial monic, x^4+c3*x^3+c2*x^2+c1*x+c0.
				double invC4 = ((double)1) / c4;
				c0 *= invC4;
				c1 *= invC4;
				c2 *= invC4;
				c3 *= invC4;

				// Reduction to resolvent cubic polynomial y^3+r2*y^2+r1*y+r0 = 0.
				double r0 = -c3 * c3 * c0 + ((double)4) * c2 * c0 - c1 * c1;
				double r1 = c3 * c1 - ((double)4) * c0;
				double r2 = -c2;

				CubicRoots cubicRoots;
				Cubic(r0, r1, r2, (double)1, out cubicRoots, epsilon);  // always produces at least one root
				double y = cubicRoots.X0;

				roots.RootCount = 0;
				double discr = ((double)0.25) * c3 * c3 - c2 + y;
				if (System.Math.Abs(discr) <= epsilon)
				{
					discr = (double)0;
				}

				if (discr > (double)0)
				{
					double r = System.Math.Sqrt(discr);
					double t1 = ((double)0.75) * c3 * c3 - r * r - ((double)2) * c2;
					double t2 = (((double)4) * c3 * c2 - ((double)8) * c1 - c3 * c3 * c3) / (((double)4.0) * r);

					double tPlus = t1 + t2;
					double tMinus = t1 - t2;
					if (System.Math.Abs(tPlus) <= epsilon)
					{
						tPlus = (double)0;
					}
					if (System.Math.Abs(tMinus) <= epsilon)
					{
						tMinus = (double)0;
					}

					if (tPlus >= (double)0)
					{
						double d = System.Math.Sqrt(tPlus);
						roots.X0 = -((double)0.25) * c3 + ((double)0.5) * (r + d);
						roots.X1 = -((double)0.25) * c3 + ((double)0.5) * (r - d);
						roots.RootCount += 2;
					}
					if (tMinus >= (double)0)
					{
						double e = System.Math.Sqrt(tMinus);
						if (roots.RootCount == 0)
						{
							roots.X0 = -((double)0.25) * c3 + ((double)0.5) * (e - r);
							roots.X1 = -((double)0.25) * c3 - ((double)0.5) * (e + r);
						}
						else
						{
							roots.X2 = -((double)0.25) * c3 + ((double)0.5) * (e - r);
							roots.X3 = -((double)0.25) * c3 - ((double)0.5) * (e + r);
						}
						roots.RootCount += 2;
					}
				}
				else if (discr < (double)0)
				{
					roots.RootCount = 0;
				}
				else
				{
					double t2 = y * y - ((double)4) * c0;
					if (t2 >= -epsilon)
					{
						if (t2 < (double)0) // round to zero
						{
							t2 = (double)0;
						}
						t2 = ((double)2) * System.Math.Sqrt(t2);
						double t1 = ((double)0.75) * c3 * c3 - ((double)2) * c2;
						double tPlus = t1 + t2;
						if (tPlus >= epsilon)
						{
							double d = System.Math.Sqrt(tPlus);
							roots.X0 = -((double)0.25) * c3 + ((double)0.5) * d;
							roots.X1 = -((double)0.25) * c3 - ((double)0.5) * d;
							roots.RootCount += 2;
						}
						double tMinus = t1 - t2;
						if (tMinus >= epsilon)
						{
							double e = System.Math.Sqrt(tMinus);
							if (roots.RootCount == 0)
							{
								roots.X0 = -((double)0.25) * c3 + ((double)0.5) * e;
								roots.X1 = -((double)0.25) * c3 - ((double)0.5) * e;
							}
							else
							{
								roots.X2 = -((double)0.25) * c3 + ((double)0.5) * e;
								roots.X3 = -((double)0.25) * c3 - ((double)0.5) * e;
							}
							roots.RootCount += 2;
						}
					}
				}

				return roots.RootCount > 0;
			}

			/// <summary>
			/// Gets roots bound of the given polynomial or -1 if polynomial is constant.
			/// </summary>
			public static double PolynomialBound(Polynomial poly, double epsilon = Mathex.ZeroTolerance)
			{
				Polynomial copyPoly = poly.DeepCopy();
				copyPoly.Compress(epsilon);

				int degree = copyPoly.Degree;
				if (degree < 1)
				{
					// Polynomial is constant, return invalid bound.
					return -1f;
				}

				double invCopyDeg = 1f / copyPoly[degree];
				double maxValue = 0f;
				for (int i = 0; i < degree; ++i)
				{
					double tmp = System.Math.Abs(copyPoly[i]) * invCopyDeg;
					if (tmp > maxValue)
					{
						maxValue = tmp;
					}
				}

				return 1f + maxValue;
			}

			/// <summary>
			/// General polynomial equation: Σ(c_i * x^i), where i=[0..degree]. Finds roots in the interval [xMin..xMax].
			/// </summary>
			/// <param name="poly">Polynomial whose roots to be found</param>
			/// <param name="xMin">Interval left border</param>
			/// <param name="xMax">Interval right border</param>
			/// <param name="roots">Roots of the polynomial</param>
			/// <param name="digits">Accuracy</param>
			/// <param name="epsilon">Small positive number</param>
			public static bool Polynomial(Polynomial poly, double xMin, double xMax, out double[] roots, int digits = 6, double epsilon = Mathex.ZeroTolerance)
			{
				PolyRootFinder finder = new PolyRootFinder(epsilon);
				if (finder.Find(poly, xMin, xMax, digits))
				{
					roots = finder.Roots;
					return true;
				}
				else
				{
					roots = new double[0];
					return false;
				}
			}

			/// <summary>
			/// General polynomial equation: Σ(c_i * x^i), where i=[0..degree].
			/// </summary>
			/// <param name="poly">Polynomial whose roots to be found</param>
			/// <param name="roots">Roots of the polynomial</param>
			/// <param name="digits">Accuracy</param>
			/// <param name="epsilon">Small positive number</param>
			public static bool Polynomial(Polynomial poly, out double[] roots, int digits = 6, double epsilon = Mathex.ZeroTolerance)
			{
				double bound = PolynomialBound(poly);
				if (bound == -1f)
				{
					roots = new double[0];
					return false;
				}
				return Polynomial(poly, -bound, bound, out roots, digits, epsilon);
			}		
		}
	}
}
