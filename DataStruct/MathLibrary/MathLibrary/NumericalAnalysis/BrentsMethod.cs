
using System;

namespace Dest
{
	namespace Math
	{
		public struct BrentsRoot
		{
			/// <summary>
			/// Function root
			/// </summary>
			public double X;

			/// <summary>
			/// Number of cycles in the inner loop which were performed to find the root.
			/// </summary>
			public int Iterations;

			/// <summary>
			/// True when inner loop exceeds maxIterations variable (in which case root is assigned current approximation), false otherwise.
			/// </summary>
			public bool ExceededMaxIterations;
		}

		public static partial class RootFinder
		{
			/// <summary>
			/// This is an implementation of Brent's
			/// Method for computing a root of a function on an interval [x0,x1] for
			/// which f(x0)*f(x1) &lt; 0 (i.e. values of the function must have
			/// different signs on interval ends).  The method uses inverse quadratic interpolation
			/// to generate a root estimate but falls back to inverse linear
			/// interpolation (secant method) if necessary.  Moreover, based on
			/// previous iterates, the method will fall back to bisection when it
			/// appears the interpolated estimate is not of sufficient quality.
			/// 
			/// This will compute a root, if any, on the interval [x0,x1].  The function returns
			/// 'true' when the root is found, in which case 'BrentsRoot.X' is the root. The function
			/// returns 'false' when the interval is invalid (x1 &lt;= x0) or when the interval
			/// does not bound a root (f(x0)*f(x1) > 0).
			/// </summary>
			/// <param name="function">The function whose root is desired. The function accepts one real number and returns real number.</param>
			/// <param name="x0">Interval left border</param>
			/// <param name="x1">Interval right border</param>
			/// <param name="root">Out parameter containing root of the function.</param>
			/// <param name="maxIterations">The maximum number of iterations used to locate a root. Should be positive number.</param>
			/// <param name="negativeTolerance">The root estimate x is accepted when the function value f(x)
			/// satisfies negativeTolerance &lt;= f(x) &lt;= positiveTolerance. negativeTolerance must be non-positive.</param>
			/// <param name="positiveTolerance">The root estimate x is accepted when the function value f(x)
			/// satisfies negativeTolerance &lt;= f(x) &lt;= positiveTolerance. positiveTolerance must be non-negative.</param>
			/// <param name="stepTolerance">Brent's Method requires additional tests before an interpolated
			/// x-value is accepted as the next root estimate.  One of these tests
			/// compares the difference of consecutive iterates and requires it
			/// to be larger than a user-specified x-tolerance (to ensure progress
			/// is made).  This parameter is that tolerance.</param>
			/// <param name="segmentTolerance">The root search is allowed to terminate when the current
			/// subinterval [xsub0,xsub1] is sufficiently small, say,
			/// |xsub1 - xsub0| &lt;= tolerance.  This parameter is that tolerance.</param>
			/// <returns>True if root is found, false otherwise.</returns>
			public static bool BrentsMethod(Func<double, double> function,
				double x0, double x1, out BrentsRoot root,
				int maxIterations = 128, 
				double negativeTolerance = Mathex.NegativeZeroTolerance, double positiveTolerance = Mathex.ZeroTolerance,
				double stepTolerance = Mathex.ZeroTolerance, double segmentTolerance = Mathex.ZeroTolerance)
			{
				root.Iterations = 0;
				root.ExceededMaxIterations = false;

				if (x1 <= x0)
				{
					// The interval is invalid.
					root.X = double.NaN;
					return false;
				}

				double f0 = function(x0);
				if (negativeTolerance <= f0 && f0 <= positiveTolerance)
				{
					// This endpoint is an approximate root that satisfies the function tolerance.
					root.X = x0;
					return true;
				}

				double f1 = function(x1);
				if (negativeTolerance <= f1 && f1 <= positiveTolerance)
				{
					// This endpoint is an approximate root that satisfies the function tolerance.
					root.X = x1;
					return true;
				}

				if (f0 * f1 >= 0f)
				{
					// The input interval must bound a root.
					root.X = double.NaN;
					return false;
				}

				if (System.Math.Abs(f0) < System.Math.Abs(f1))
				{
					// Swap x0 and x1 so that |f(x1)| <= |f(x0)|.  The number x1 is
					// considered to be the best estimate of the root.
					double temp = x0;
					x0 = x1;
					x1 = temp;

					temp = f0;
					f0 = f1;
					f1 = temp;
				}

				// Initialize values for the root search.
				double x2 = x0;
				double x3 = x0;
				double f2 = f0;
				bool prevBisected = true;

				// The root search.
				int i;
				for (i = 0; i < maxIterations; ++i)
				{
					double fDiff01 = f0 - f1, fDiff02 = f0 - f2, fDiff12 = f1 - f2;
					double invFDiff01 = 1f / fDiff01;
					double s;
					if (fDiff02 != 0f && fDiff12 != 0f)
					{
						// Use inverse quadratic interpolation.
						double infFDiff02 = 1f / fDiff02;
						double invFDiff12 = 1f / fDiff12;
						s =
							x0 * f1 * f2 * invFDiff01 * infFDiff02 -
							x1 * f0 * f2 * invFDiff01 * invFDiff12 +
							x2 * f0 * f1 * infFDiff02 * invFDiff12;
					}
					else
					{
						// Use inverse linear interpolation (secant method).
						s = (x1 * f0 - x0 * f1) * invFDiff01;
					}

					// Compute values need in the accept-or-reject tests.
					double xDiffSAvr = s - 0.75f * x0 - 0.25f * x1;
					double xDiffS1 = s - x1;
					double absXDiffS1 = System.Math.Abs(xDiffS1);
					double absXDiff12 = System.Math.Abs(x1 - x2);
					double absXDiff23 = System.Math.Abs(x2 - x3);

					bool currBisected = false;
					if (xDiffSAvr * xDiffS1 > 0f)
					{
						// The value s is not between 0.75*x0+0.25*x1 and x1.  NOTE:  The
						// algorithm sometimes has x0 < x1 but sometimes x1 < x0, so the
						// betweenness test does not use simple comparisons.
						currBisected = true;
					}
					else if (prevBisected)
					{
						// The first of Brent's tests to determine whether to accept the
						// interpolated s-value.
						currBisected =
							(absXDiffS1 >= 0.5f * absXDiff12) ||
							(absXDiff12 <= stepTolerance);
					}
					else
					{
						// The second of Brent's tests to determine whether to accept the
						// interpolated s-value.
						currBisected =
							(absXDiffS1 >= 0.5f * absXDiff23) ||
							(absXDiff23 <= stepTolerance);
					}

					if (currBisected)
					{
						// One of the additional tests failed, so reject the interpolated
						// s-value and use bisection instead.
						s = 0.5f * (x0 + x1);
						prevBisected = true;
					}
					else
					{
						prevBisected = false;
					}

					// Evaluate the function at the new estimate and test for convergence.
					double fs = function(s);
					if (negativeTolerance <= fs && fs <= positiveTolerance)
					{
						root.X = s;
						root.Iterations = i;
						return true;
					}

					// Update the subinterval to include the new estimate as an endpoint.
					x3 = x2;
					x2 = x1;
					f2 = f1;
					if (f0 * fs < 0f)
					{
						x1 = s;
						f1 = fs;
					}
					else
					{
						x0 = s;
						f0 = fs;
					}

					// Allow the algorithm to terminate when the subinterval is
					// sufficiently small.
					if (System.Math.Abs(x1 - x0) <= segmentTolerance)
					{
						root.X = x1;
						root.Iterations = i;
						return true;
					}

					// A loop invariant is that x1 is the root estimate, f(x0)*f(x1) < 0,
					// and |f(x1)| <= |f(x0)|.
					if (System.Math.Abs(f0) < System.Math.Abs(f1))
					{
						double temp = x0;
						x0 = x1;
						x1 = temp;

						temp = f0;
						f0 = f1;
						f1 = temp;
					}
				}

				root.X = x1;
				root.Iterations = i;
				root.ExceededMaxIterations = true;
				return true;
			}
		}
	}
}
