
using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		public static partial class Approximation
		{
			/// <summary>
			/// Fitting to a line using least-squares method and using distance
			/// measurements in the y-direction. The result is a line represented by
			/// y = A*x + B. If a line cannot be constructed method returns false and
			/// A and B are returned as double.MaxValue.
			/// </summary>
			internal static bool HeightLineFit2(IList<Vector2D> points, out double a, out double b)
			{
				// You need at least two points to determine the line.  Even so, if
				// the points are on a vertical line, there is no least-squares fit in
				// the 'height' sense.  This will be trapped by the determinant of the
				// coefficient matrix being (nearly) zero.

				// Compute sums for linear system.
				double sumX = (double)0, sumY = (double)0;
				double sumXX = (double)0, sumXY = (double)0;
				int numPoints = points.Count;
				int i;

				for (i = 0; i < numPoints; ++i)
				{
					sumX += points[i].x;
					sumY += points[i].y;
					sumXX += points[i].x * points[i].x;
					sumXY += points[i].x * points[i].y;
				}

				double[,] A =
				{
					{sumXX, sumX},
					{sumX, (double)numPoints}
				};

				double[] B =
				{
					sumXY,
					sumY
				};

				double[] X;
				bool nonsingular = LinearSystem.Solve2(A, B, out X);

				if (nonsingular)
				{
					a = X[0];
					b = X[1];
				}
				else
				{
					a = double.MaxValue;
					b = double.MaxValue;
				}

				return nonsingular;
			}

			/// <summary>
			/// Producing a line using least-squares fitting. A set must contain at least one point!
			/// </summary>
			public static Line2 LeastSquaresLineFit2(IList<Vector2D> points)
			{
				Line2 line = new Line2();
				int numPoints = points.Count;

				// Compute the mean of the points.
				line.Center = points[0];
				int i;
				for (i = 1; i < numPoints; ++i)
				{
					line.Center += points[i];
				}
				double invNumPoints = ((double)1) / numPoints;
				line.Center *= invNumPoints;

				// Compute the covariance matrix of the points.
				double sumXX = (double)0, sumXY = (double)0, sumYY = (double)0;
				for (i = 0; i < numPoints; ++i)
				{
					Vector2D diff = points[i] - line.Center;
					sumXX += diff.x * diff.x;
					sumXY += diff.x * diff.y;
					sumYY += diff.y * diff.y;
				}

				sumXX *= invNumPoints;
				sumXY *= invNumPoints;
				sumYY *= invNumPoints;

				// Set up the eigensolver.
				double[,] matrix =
				{
					{ sumYY, -sumXY },
					{ sumXY,  sumXX }
				};
				// Compute eigenstuff, smallest eigenvalue is in last position.
				EigenData eigenData = EigenDecomposition.Solve(matrix, false);

				// Unit-length direction for best-fit line.
				line.Direction = eigenData.GetEigenvector2(1);

				return line;
			}
		}
	}
}
