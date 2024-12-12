

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// Represents n-degree polynomial of one variable
		/// </summary>
		public class Polynomial
		{
			private int     _degree;
			private double[] _coeffs;
			
			/// <summary>
			/// Gets or sets polynomial degree (0 - constant, 1 - linear, 2 - quadratic, etc).
			/// When set, recreates coefficient array thus all coefficients become 0.
			/// </summary>
			public int Degree
			{
				get { return _degree; }
				set
				{
					_degree = value;
					_coeffs = new double[_degree + 1];					
				}
			}

			/// <summary>
			/// Gets or sets polynomial coefficient.
			/// </summary>
			/// <param name="index">Valid index is 0&lt;=index&lt;=Degree</param>
			public double this[int index] { get { return _coeffs[index]; } set { _coeffs[index] = value; } }


			/// <summary>
			/// Creates polynomial of specified degree. Use indexer to set coefficients.
			/// Coefficients order is from smallest order to highest order, e.g for quadratic equation it's:
			/// c0 + c1*x + c2*x^2, coefficients array will be [c0,c1,c2].
			/// </summary>
			/// <param name="degree">Must be >= 0!</param>
			public Polynomial(int degree)
			{
				Degree = degree;
			}

			/// <summary>
			/// Copies the polynomial
			/// </summary>
			public Polynomial DeepCopy()
			{
				Polynomial result = new Polynomial(_degree);
				for (int i = 0; i <= _degree; ++i)
				{
					result._coeffs[i] = _coeffs[i];
				}
				return result;
			}


			/// <summary>
			/// Returns derivative of the current polynomial. Formula is:
			/// p (x) = c0 + c1*x + c2*x^2 + ... + cn*x^n
			/// p'(x) = c1 + 2*c2*x + 3*c3*x^2 + ... + n*cn*x^(n-1)
			/// </summary>
			public Polynomial CalcDerivative()
			{
				if (_degree > 0)
				{
					Polynomial result = new Polynomial(_degree - 1);
					for (int i0 = 0, i1 = 1; i0 < _degree; ++i0, ++i1)
					{
						result._coeffs[i0] = i1 * _coeffs[i1];
					}
					return result;
				}
				else
				{
					Polynomial result = new Polynomial(0);
					result._coeffs[0] = 0f;
					return result;
				}
			}

			/// <summary>
			/// Computes inversion of the current polynomial ( invpoly[i] = poly[degree-i] for 0 &lt;= i &lt;= degree ).
			/// </summary>
			public Polynomial CalcInversion()
			{
				Polynomial result = new Polynomial(_degree);
				for (int i = 0; i <= _degree; ++i)
				{
					result._coeffs[i] = _coeffs[_degree - i];
				}
				return result;
			}

			/// <summary>
			/// Reduce the degree by eliminating all (nearly) zero leading coefficients
			/// and by making the leading coefficient one.  The input parameter is
			/// the threshold for specifying that a coefficient is effectively zero.
			/// </summary>
			public void Compress(double epsilon = Mathex.ZeroTolerance)
			{
				int tempDegree = (int)_degree;
				for (int i = tempDegree; i >= 0; --i)
				{
					if (System.Math.Abs(_coeffs[i]) <= epsilon)
					{
						--tempDegree;
					}
					else
					{
						break;
					}
				}

				if (tempDegree >= 0)
				{
					_degree = tempDegree;
					double invLeading = 1f / _coeffs[_degree];
					_coeffs[_degree] = 1f;
					for (int i = 0; i < _degree; ++i)
					{
						_coeffs[i] *= invLeading;
					}
				}
			}

			/// <summary>
			/// Evaluates the polynomial
			/// </summary>
			public double Eval(double t)
			{
				double result = _coeffs[_degree];
				for (int i = _degree - 1; i >= 0; --i)
				{
					result *= t;
					result += _coeffs[i];
				}
				return result;
			}
		}
	}
}
