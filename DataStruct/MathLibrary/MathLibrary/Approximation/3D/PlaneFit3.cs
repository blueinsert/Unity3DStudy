
using System.Collections.Generic;

namespace Dest
{
	namespace Math
	{
		public static partial class Approximation
		{
			/// <summary>
			/// Least-squares fit of a plane to (x,y,f(x,y)) data by using distance
			/// measurements in the z-direction.  The resulting plane is represented by
			/// z = A*x + B*y + C.  The return value is 'false' if the 3x3 coefficient
			/// matrix in the linear system that defines A, B, and C is (nearly) singular.
			/// In this case, A, B, and C are returned as double.MaxValue.
			/// </summary>
			internal static bool HeightPlaneFit3(IList<Vector3D> points, out double a, out double b, out double c)
			{
				// You need at least three points to determine the plane.  Even so, if
				// the points are on a vertical plane, there is no least-squares fit in
				// the 'height' sense.  This will be trapped by the determinant of the
				// coefficient matrix.

				// Compute sums for linear system.
				double sumX = (double)0, sumY = (double)0, sumZ = (double)0.0;
				double sumXX = (double)0, sumXY = (double)0, sumXZ = (double)0;
				double sumYY = (double)0, sumYZ = (double)0;
				int i;
				int numPoints = points.Count;

				for (i = 0; i < numPoints; ++i)
				{
					sumX += points[i].x;
					sumY += points[i].y;
					sumZ += points[i].z;
					sumXX += points[i].x * points[i].x;
					sumXY += points[i].x * points[i].y;
					sumXZ += points[i].x * points[i].z;
					sumYY += points[i].y * points[i].y;
					sumYZ += points[i].y * points[i].z;
				}

				double[,] A =
				{
					{sumXX, sumXY, sumX},
					{sumXY, sumYY, sumY},
					{sumX,  sumY,  (double)numPoints}
				};

				double[] B =
				{
					sumXZ,
					sumYZ,
					sumZ
				};

				double[] X;
				bool nonsingular = LinearSystem.Solve3(A, B, out X);

				if (nonsingular)
				{
					a = X[0];
					b = X[1];
					c = X[2];
				}
				else
				{
					a = double.MaxValue;
					b = double.MaxValue;
					c = double.MaxValue;
				}

				return nonsingular;
			}

			/// <summary>
			/// Producing a plane using least-squares fitting. A set must contain at least one point!
			/// </summary>
			public static Plane3 LeastSquaresPlaneFit3(IList<Vector3D> points)
			{
				// Compute the mean of the points.
				Vector3D origin = Vector3Dex.Zero;
				int i;
				int numPoints = points.Count;

				for (i = 0; i < numPoints; i++)
				{
					origin += points[i];
				}
				double invNumPoints = ((double)1) / numPoints;
				origin *= invNumPoints;

				// compute sums of products
				double sumXX = (double)0, sumXY = (double)0, sumXZ = (double)0;
				double sumYY = (double)0, sumYZ = (double)0, sumZZ = (double)0;
				for (i = 0; i < numPoints; ++i)
				{
					Vector3D diff = points[i] - origin;
					sumXX += diff.x * diff.x;
					sumXY += diff.x * diff.y;
					sumXZ += diff.x * diff.z;
					sumYY += diff.y * diff.y;
					sumYZ += diff.y * diff.z;
					sumZZ += diff.z * diff.z;
				}

				sumXX *= invNumPoints;
				sumXY *= invNumPoints;
				sumXZ *= invNumPoints;
				sumYY *= invNumPoints;
				sumYZ *= invNumPoints;
				sumZZ *= invNumPoints;

				// Setup the eigensolver.
				double[,] matrix = new double[3, 3];
				matrix[0, 0] = sumXX;
				matrix[0, 1] = sumXY;
				matrix[0, 2] = sumXZ;
				matrix[1, 0] = sumXY;
				matrix[1, 1] = sumYY;
				matrix[1, 2] = sumYZ;
				matrix[2, 0] = sumXZ;
				matrix[2, 1] = sumYZ;
				matrix[2, 2] = sumZZ;

				// Compute eigenstuff, smallest eigenvalue is in last position.
				EigenData eigenData = EigenDecomposition.Solve(matrix, false);

				// Get plane normal.
				Vector3D normal = eigenData.GetEigenvector3(2);

				// The minimum energy.
				return new Plane3(ref normal, ref origin);
			}
		}
	}
}
