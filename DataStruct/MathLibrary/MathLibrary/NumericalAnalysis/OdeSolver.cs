

namespace Dest
{
	namespace Math
	{
		/// <summary>
		/// The system is y'(t) = F(t,y). The dimension of y is passed to the constructor of OdeSolver.
		/// </summary>
		public delegate void OdeFunction(
			double   t,	// t
			double[] y,	// y
			double[] F);	// F(t,y)

		public abstract class OdeSolver
		{
			protected int         _dim;
			protected double       _step;
			protected OdeFunction _function;
			protected double[]     _FValue;

			public virtual double Step { get { return _step; } set { _step = value; } }

			public OdeSolver(int dim, double step, OdeFunction function)
			{
				_dim      = dim;
				_step     = step;
				_function = function;
				_FValue   = new double[_dim];
			}

			public abstract void Update(double tIn, double[] yIn, ref double tOut, double[] yOut);
		}

		public class OdeEuler : OdeSolver
		{
			public OdeEuler(int dim, double step, OdeFunction function)
				: base(dim, step, function)
			{
			}

			public override void Update(double tIn, double[] yIn, ref double tOut, double[] yOut)
			{
				_function(tIn, yIn, _FValue);

				for (int i = 0; i < _dim; ++i)
				{
					yOut[i] = yIn[i] + _step * _FValue[i];
				}

				tOut = tIn + _step;
			}
		}

		public class OdeMidpoint : OdeSolver
		{
			private double   _halfStep;
			private double[] _yTemp;

			public override double Step { get { return base.Step; } set { _step = value; _halfStep = _step * .5f; } }

			public OdeMidpoint(int dim, double step, OdeFunction function)
				: base(dim, step, function)
			{
				_halfStep = _step * 0.5f;
				_yTemp = new double[_dim];
			}

			public override void Update(double tIn, double[] yIn, ref double tOut, double[] yOut)
			{
				// first step
				_function(tIn, yIn, _FValue);
				int i;
				for (i = 0; i < _dim; ++i)
				{
					_yTemp[i] = yIn[i] + _halfStep * _FValue[i];
				}

				// second step
				double halfT = tIn + _halfStep;
				_function(halfT, _yTemp, _FValue);
				for (i = 0; i < _dim; ++i)
				{
					yOut[i] = yIn[i] + _step * _FValue[i];
				}

				tOut = tIn + _step;
			}
		}

		public class OdeRungeKutta4 : OdeSolver
		{
			private double   _halfStep;
			private double   _sixthStep;
			private double[] _temp1;
			private double[] _temp2;
			private double[] _temp3;
			private double[] _temp4;
			private double[] _yTemp;

			public override double Step { get { return base.Step; } set { _step = value; _halfStep = _step * .5f; _sixthStep = _step / 6.0f; } }

			public OdeRungeKutta4(int dim, double step, OdeFunction function)
				: base(dim, step, function)
			{
				_halfStep = 0.5f * step;
				_sixthStep = step / 6.0f;

				_temp1 = new double[_dim];
				_temp2 = new double[_dim];
				_temp3 = new double[_dim];
				_temp4 = new double[_dim];
				_yTemp = new double[_dim];
			}

			public override void Update(double tIn, double[] yIn, ref double tOut, double[] yOut)
			{
				// first step
				_function(tIn, yIn, _temp1);
				int i;
				for (i = 0; i < _dim; ++i)
				{
					_yTemp[i] = yIn[i] + _halfStep * _temp1[i];
				}

				// second step
				double halfT = tIn + _halfStep;
				_function(halfT, _yTemp, _temp2);
				for (i = 0; i < _dim; ++i)
				{
					_yTemp[i] = yIn[i] + _halfStep * _temp2[i];
				}

				// third step
				_function(halfT, _yTemp, _temp3);
				for (i = 0; i < _dim; ++i)
				{
					_yTemp[i] = yIn[i] + _step * _temp3[i];
				}

				// fourth step
				tOut = tIn + _step;
				_function(tOut, _yTemp, _temp4);
				for (i = 0; i < _dim; ++i)
				{
					yOut[i] = yIn[i] + _sixthStep * (_temp1[i] + 2f * (_temp2[i] + _temp3[i]) + _temp4[i]);
				}
			}
		}
	}
}
