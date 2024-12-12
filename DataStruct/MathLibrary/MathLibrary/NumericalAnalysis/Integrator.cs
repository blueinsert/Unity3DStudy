using System;

namespace Dest
{
	namespace Math
	{
		public static class Integrator
		{
			// Roots of the Legendre polynomial of specified degree.

			private const int _degree = 5;
			
			private static double[] root =
			{
				-0.9061798459f,
				-0.5384693101f,
				+0.0f,
				+0.5384693101f,
				+0.9061798459f
			};

			private static double[] coeff =
			{
				0.2369268850f,
				0.4786286705f,
				0.5688888889f,
				0.4786286705f,
				0.2369268850f
			};

			/// <summary>
			/// Evaluates integral ∫f(x)dx on [a,b] interval using trapezoidal rule.
			/// sampleCount must be greater or equal to 2.
			/// </summary>
			public static double TrapezoidRule(Func<double, double> function, double a, double b, int sampleCount)
			{
				if (sampleCount < 2) return double.NaN;

				double h = (b - a) / (double)(sampleCount - 1);
				double result = 0.5f * (function(a) + function(b));

				for (int i = 1; i <= sampleCount - 2; ++i)
				{
					result += function(a + i * h);
				}
				result *= h;

				return result;
			}

			/// <summary>
			/// Evaluates integral ∫f(x)dx on [a,b] interval using Romberg's method.
			/// Integration order must be positive (order > 0).
			/// </summary>
			public static double RombergIntegral(Func<double, double> function, double a, double b, int order)
			{
				if (order <= 0) return double.NaN;

				double[,] rom = new double[2, order];
				double h = b - a;

				rom[0, 0] = 0.5f * h * (function(a) + function(b));
				for (int i0 = 2, p0 = 1; i0 <= order; ++i0, p0 *= 2, h *= 0.5f)
				{
					// Approximations via the trapezoid rule.
					double sum = 0f;
					int i1;
					for (i1 = 1; i1 <= p0; ++i1)
					{
						sum += function(a + h * (i1 - 0.5f));
					}

					// Richardson extrapolation.
					rom[1, 0] = 0.5f * (rom[0, 0] + h * sum);
					for (int i2 = 1, p2 = 4; i2 < i0; ++i2, p2 *= 4)
					{
						rom[1, i2] = (p2 * rom[1, i2 - 1] - rom[0, i2 - 1]) / (p2 - 1);
					}

					for (i1 = 0; i1 < i0; ++i1)
					{
						rom[0, i1] = rom[1, i1];
					}
				}

				double result = rom[0, order - 1];
				return result;
			}

			/// <summary>
			/// Evaluates integral ∫f(x)dx on [a,b] interval using Gaussian quadrature rule (five Legendre polynomials).
			/// </summary>
			public static double GaussianQuadrature(Func<double, double> function, double a, double b)
			{
				// Legendre polynomials:
				// P_0(x) = 1
				// P_1(x) = x
				// P_2(x) = (3x^2-1)/2
				// P_3(x) = x(5x^2-3)/2
				// P_4(x) = (35x^4-30x^2+3)/8
				// P_5(x) = x(63x^4-70x^2+15)/8

				// Generation of polynomials:
				//   d/dx[ (1-x^2) dP_n(x)/dx ] + n(n+1) P_n(x) = 0
				//   P_n(x) = sum_{k=0}^{floor(n/2)} c_k x^{n-2k}
				//     c_k = (-1)^k (2n-2k)! / [ 2^n k! (n-k)! (n-2k)! ]
				//   P_n(x) = ((-1)^n/(2^n n!)) d^n/dx^n[ (1-x^2)^n ]
				//   (n+1)P_{n+1}(x) = (2n+1) x P_n(x) - n P_{n-1}(x)
				//   (1-x^2) dP_n(x)/dx = -n x P_n(x) + n P_{n-1}(x)

				// Need to transform domain [a,b] to [-1,1].  If a <= x <= b and
				// -1 <= t <= 1, then x = ((b-a)*t+(b+a))/2.
				double radius = 0.5f * (b - a);
				double center = 0.5f * (b + a);

				double result = 0f;
				for (int i = 0; i < _degree; ++i)
				{
					result += coeff[i] * function(radius * root[i] + center);
				}
				result *= radius;

				return result;
			}
		}
	}
}
