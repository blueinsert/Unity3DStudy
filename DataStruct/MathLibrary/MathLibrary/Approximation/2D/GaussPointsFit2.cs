
using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		public static partial class Approximation
		{
			/// <summary>
			/// Fits points with a Gaussian distribution. Produces box as the result.
			/// Box center is average of a point set. Box axes are eigenvectors of the
			/// covariance matrix, box extents are eigenvalues.
			/// A set must contain at least one point!
			/// </summary>
			public static Box2 GaussPointsFit2(IList<Vector2D> points)
			{
				Box2 box = new Box2(Vector2Dex.Zero, Vector2Dex.UnitX, Vector2Dex.UnitY, Vector2Dex.One);
				int numPoints = points.Count;
				
				// Compute the mean of the points.
				box.Center = points[0];
				for (int i = 1; i < numPoints; ++i)
				{
					box.Center += points[i];
				}
				double invNumPoints = 1f / numPoints;
				box.Center *= invNumPoints;

				// Compute the covariance matrix of the points.
				double sumXX = 0f;
				double sumXY = 0f;
				double sumYY = 0f;

				for (int i = 0; i < numPoints; ++i)
				{
					Vector2D diff = points[i] - box.Center;
					sumXX += diff.x * diff.x;
					sumXY += diff.x * diff.y;
					sumYY += diff.y * diff.y;
				}

				sumXX *= invNumPoints;
				sumXY *= invNumPoints;
				sumYY *= invNumPoints;

				// Setup the eigensolver.
				double[,] matrix =
				{
					{ sumXX, sumXY },
					{ sumXY, sumYY }
				};
				EigenData eigenData = EigenDecomposition.Solve(matrix, true);

				box.Extents.x = eigenData.GetEigenvalue(0);
				box.Extents.y = eigenData.GetEigenvalue(1);

				box.Axis0 = eigenData.GetEigenvector2(0);
				box.Axis1 = eigenData.GetEigenvector2(1);

				return box;
			}
		}
	}
}
