
using System;

namespace Dest
{
	namespace Math
	{
		public static class LinearSystem
		{
			/// <summary>
			/// Solves linear system A*X=B with two equations and two unknowns.
			/// </summary>
			/// <param name="A">double[2,2] array containing equations coefficients</param>
			/// <param name="B">double[2] array containing constants</param>
			/// <param name="X">Out double[2] array contaning the solution or null if system has no solution</param>
			/// <param name="zeroTolerance">Small positive number</param>
			/// <returns>True if solution is found, false otherwise</returns>
			public static bool Solve2(double[,] A, double[] B, out double[] X, double zeroTolerance = Mathex.ZeroTolerance)
			{
				double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
				if (System.Math.Abs(det) < zeroTolerance)
				{
					X = null;
					return false;
				}

				double invDet = 1f / det;
				X = new double[2];
				X[0] = (A[1, 1] * B[0] - A[0, 1] * B[1]) * invDet;
				X[1] = (A[0, 0] * B[1] - A[1, 0] * B[0]) * invDet;
				return true;
			}

			/// <summary>
			/// Solves linear system A*X=B with two equations and two unknowns.
			/// </summary>
			/// <param name="A">double[2,2] array containing equations coefficients</param>
			/// <param name="B">double[2] array containing constants</param>
			/// <param name="X">Out vector contaning the solution or zero vector if system has no solution</param>
			/// <param name="zeroTolerance">Small positive number</param>
			/// <returns>True if solution is found, false otherwise</returns>
			public static bool Solve2(double[,] A, double[] B, out Vector2D X, double zeroTolerance = Mathex.ZeroTolerance)
			{
				double[] temp;
				bool result = Solve2(A, B, out temp, zeroTolerance);
				if (result)
				{
					X.x = temp[0];
					X.y = temp[1];
				}
				else
				{
					X = Vector2Dex.Zero;
				}
				return result;
			}

			/// <summary>
			/// Solves linear system A*X=B with three equations and three unknowns.
			/// </summary>
			/// <param name="A">double[3,3] array containing equations coefficients</param>
			/// <param name="B">double[3] array containing constants</param>
			/// <param name="X">Out double[3] array contaning the solution or null if system has no solution</param>
			/// <param name="zeroTolerance">Small positive number</param>
			/// <returns>True if solution is found, false otherwise</returns>
			public static bool Solve3(double[,] A, double[] B, out double[] X, double zeroTolerance = Mathex.ZeroTolerance)
			{
				double[,] invA = new double[,]
				{
					{ A[1,1] * A[2,2] - A[1,2] * A[2,1], A[0,2] * A[2,1] - A[0,1] * A[2,2], A[0,1] * A[1,2] - A[0,2] * A[1,1] },
					{ A[1,2] * A[2,0] - A[1,0] * A[2,2], A[0,0] * A[2,2] - A[0,2] * A[2,0], A[0,2] * A[1,0] - A[0,0] * A[1,2] },
					{ A[1,0] * A[2,1] - A[1,1] * A[2,0], A[0,1] * A[2,0] - A[0,0] * A[2,1], A[0,0] * A[1,1] - A[0,1] * A[1,0] }
				};

				double det = A[0, 0] * invA[0, 0] + A[0, 1] * invA[1, 0] + A[0, 2] * invA[2, 0];

				if (System.Math.Abs(det) < zeroTolerance)
				{
					X = null;
					return false;
				}

				double invDet = 1f / det;
				for (int row = 0; row < 3; ++row)
				{
					for (int col = 0; col < 3; ++col)
					{
						invA[row, col] *= invDet;
					}
				}

				X = new double[3];
				X[0] = invA[0, 0] * B[0] + invA[0, 1] * B[1] + invA[0, 2] * B[2];
				X[1] = invA[1, 0] * B[0] + invA[1, 1] * B[1] + invA[1, 2] * B[2];
				X[2] = invA[2, 0] * B[0] + invA[2, 1] * B[1] + invA[2, 2] * B[2];

				return true;
			}

			/// <summary>
			/// Solves linear system A*X=B with three equations and three unknowns.
			/// </summary>
			/// <param name="A">double[3,3] array containing equations coefficients</param>
			/// <param name="B">double[3] array containing constants</param>
			/// <param name="X">Out vector contaning the solution or zero vector if system has no solution</param>
			/// <param name="zeroTolerance">Small positive number</param>
			/// <returns>True if solution is found, false otherwise</returns>
			public static bool Solve3(double[,] A, double[] B, out Vector3D X, double zeroTolerance = Mathex.ZeroTolerance)
			{
				double[] temp;
				bool result = Solve3(A, B, out temp, zeroTolerance);
				if (result)
				{
					X.x = temp[0];
					X.y = temp[1];
					X.z = temp[2];
				}
				else
				{
					X = Vector3Dex.Zero;
				}
				return result;
			}


			private static void SwapRows(double[,] matrix, int row0, int row1, int columnCount)
			{
				if (row0 != row1)
				{
					for (int c = 0; c < columnCount; ++c)
					{
						double temp = matrix[row0, c];
						matrix[row0, c] = matrix[row1, c];
						matrix[row1, c] = temp;
					}
				}
			}

			/// <summary>
			/// Solves linear system A*X=B with N equations and N unknowns.
			/// </summary>
			/// <param name="A">double[N,N] array containing equations coefficients</param>
			/// <param name="B">double[N] array containing constants</param>
			/// <param name="X">Out double[N] array contaning the solution or null if system has no solution</param>
			/// <returns>True if solution is found, false otherwise</returns>
			public static bool Solve(double[,] A, double[] B, out double[] X)
			{
				// Matrix must be square
				if (A.GetLength(0) != A.GetLength(1) || A.GetLength(0) != B.Length)
				{
					X = null;
					return false;
				}

				// Computations are performed in-place.
				int size = A.GetLength(1);

				double[,] invA = new double[A.GetLength(0), A.GetLength(1)];
				Buffer.BlockCopy(A, 0, invA, 0, A.Length * sizeof(double));

				X = new double[size];
				Buffer.BlockCopy(B, 0, X, 0, size * sizeof(double));

				int[] colIndex = new int[size];
				int[] rowIndex = new int[size];
				bool[] pivoted = new bool[size];

				int i1, i2, row = 0, col = 0;
				double temp;

				// Elimination by full pivoting.
				for (int i0 = 0; i0 < size; ++i0)
				{
					// Search matrix (excluding pivoted rows) for maximum absolute entry.
					double maxValue = 0f;
					for (i1 = 0; i1 < size; ++i1)
					{
						if (!pivoted[i1])
						{
							for (i2 = 0; i2 < size; ++i2)
							{
								if (!pivoted[i2])
								{
									double absValue = System.Math.Abs(invA[i1, i2]);
									if (absValue > maxValue)
									{
										maxValue = absValue;
										row = i1;
										col = i2;
									}
								}
							}
						}
					}

					if (maxValue == 0f)
					{
						// The matrix is not invertible.
						X = null;
						return false;
					}

					pivoted[col] = true;

					// Swap rows so that A[col][col] contains the pivot entry.
					if (row != col)
					{
						SwapRows(invA, row, col, size);

						temp = X[row];
						X[row] = X[col];
						X[col] = temp;
					}

					// Keep track of the permutations of the rows.
					rowIndex[i0] = row;
					colIndex[i0] = col;

					// Scale the row so that the pivot entry is 1.
					double inv = 1.0f / invA[col, col];
					invA[col, col] = 1f;
					for (i2 = 0; i2 < size; ++i2)
					{
						invA[col, i2] *= inv;
					}
					X[col] *= inv;

					// Zero out the pivot column locations in the other rows.
					for (i1 = 0; i1 < size; ++i1)
					{
						if (i1 != col)
						{
							temp = invA[i1, col];
							invA[i1, col] = 0f;
							for (i2 = 0; i2 < size; i2++)
							{
								invA[i1, i2] -= invA[col, i2] * temp;
							}
							X[i1] -= X[col] * temp;
						}
					}
				}

				return true;
			}

			/// <summary>
			/// Solves linear system A*X=B, where A is tridiagonal matrix.
			/// </summary>
			/// <param name="A">Lower diagonal double[N-1]</param>
			/// <param name="B">Main  diagonal double[N]</param>
			/// <param name="C">Upper diagonal double[N-1]</param>
			/// <param name="R">Right-hand side double[N]</param>
			/// <param name="U">Out double[N] containing the solution or null if system has no solution</param>
			/// <returns>True if solution is found, false otherwise</returns>
			public static bool SolveTridiagonal(double[] A, double[] B, double[] C, double[] R, out double[] U)
			{
				int size = B.Length;

				if (B[0] == 0f)
				{
					U = null;
					return false;
				}

				double[] D = new double[size - 1];
				double E = B[0];
				double invE = 1f / E;

				U = new double[size];
				U[0] = R[0] * invE;

				int i0, i1;
				for (i0 = 0, i1 = 1; i1 < size; ++i0, ++i1)
				{
					D[i0] = C[i0] * invE;
					E = B[i1] - A[i0] * D[i0];
					if (E == 0f)
					{
						U = null;
						return false;
					}
					invE = 1f / E;
					U[i1] = (R[i1] - A[i0] * U[i0]) * invE;
				}

				for (i0 = size - 1, i1 = size - 2; i1 >= 0; --i0, --i1)
				{
					U[i1] -= D[i1] * U[i0];
				}

				return true;
			}

			/// <summary>
			/// Inverses square matrix A. Returns inversed matrix in invA parameter (invA is null if A has no inverse).
			/// </summary>
			public static bool Inverse(double[,] A, out double[,] invA)
			{
				// Computations are performed in-place.
				
				// Matrix must be square
				if (A.GetLength(0) != A.GetLength(1))
				{
					invA = null;
					return false;
				}

				int size = A.GetLength(0);

				invA = new double[size, size];
				Buffer.BlockCopy(A, 0, invA, 0, A.Length * sizeof(double));

				int[] colIndex = new int[size];
				int[] rowIndex = new int[size];
				bool[] pivoted = new bool[size];

				int i1, i2, row = 0, col = 0;
				double save;

				// Elimination by full pivoting.
				for (int i0 = 0; i0 < size; ++i0)
				{
					// Search matrix (excluding pivoted rows) for maximum absolute entry.
					double maxValue = 0f;
					for (i1 = 0; i1 < size; ++i1)
					{
						if (!pivoted[i1])
						{
							for (i2 = 0; i2 < size; ++i2)
							{
								if (!pivoted[i2])
								{
									double absValue = System.Math.Abs(invA[i1, i2]);
									if (absValue > maxValue)
									{
										maxValue = absValue;
										row = i1;
										col = i2;
									}
								}
							}
						}
					}

					if (maxValue == 0f)
					{
						// The matrix is not invertible.
						invA = null;
						return false;
					}

					pivoted[col] = true;

					// Swap rows so that A[col][col] contains the pivot entry.
					if (row != col)
					{
						SwapRows(invA, row, col, size);
					}

					// Keep track of the permutations of the rows.
					rowIndex[i0] = row;
					colIndex[i0] = col;

					// Scale the row so that the pivot entry is 1.
					double inv = 1f / invA[col, col];
					invA[col, col] = 1f;
					for (i2 = 0; i2 < size; i2++)
					{
						invA[col, i2] *= inv;
					}

					// Zero out the pivot column locations in the other rows.
					for (i1 = 0; i1 < size; ++i1)
					{
						if (i1 != col)
						{
							save = invA[i1, col];
							invA[i1, col] = 0f;
							for (i2 = 0; i2 < size; ++i2)
							{
								invA[i1, i2] -= invA[col, i2] * save;
							}
						}
					}
				}

				// Reorder rows so that A[][] stores the inverse of the original matrix.
				for (i1 = size - 1; i1 >= 0; --i1)
				{
					if (rowIndex[i1] != colIndex[i1])
					{
						for (i2 = 0; i2 < size; ++i2)
						{
							save = invA[i2, rowIndex[i1]];
							invA[i2, rowIndex[i1]] = invA[i2, colIndex[i1]];
							invA[i2, colIndex[i1]] = save;
						}
					}
				}

				return true;
			}
		}
	}
}
