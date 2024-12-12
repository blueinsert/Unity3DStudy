
using System;

namespace Dest
{
	namespace Math
	{
		public class EigenData
		{
			private int      _size;
			private double[]  _diagonal;
			private double[,] _matrix;

			/// <summary>
			/// Eigen system size
			/// </summary>
			public int Size { get { return _size; } }

			internal EigenData(double[] diagonal, double[,] matrix)
			{
				_size     = diagonal.Length;
				_diagonal = diagonal;
				_matrix   = matrix;
			}

			/// <summary>
			/// Gets eigenvalue. Index must be 0&lt;=index&lt;Size
			/// </summary>
			public double GetEigenvalue(int index)
			{
				return _diagonal[index];
			}

			/// <summary>
			/// Gets eigenvector. Use this only if eigen system was of 2x2 size. Index must be 0&lt;=index&lt;Size
			/// </summary>
			public Vector2D GetEigenvector2(int index)
			{
				if (_size == 2)
				{
					Vector2D eigenvector = new Vector2D();
					for (int row = 0; row < _size; ++row)
					{
						eigenvector[row] = _matrix[row, index];
					}
					return eigenvector;
				}
				return Vector2Dex.Zero;
			}

			/// <summary>
			/// Gets eigenvector. Use this only if eigen system was of 3x3 size. Index must be 0&lt;=index&lt;Size
			/// </summary>
			public Vector3D GetEigenvector3(int index)
			{
				if (_size == 3)
				{
					Vector3D eigenvector = new Vector3D();
					for (int row = 0; row < _size; ++row)
					{
						eigenvector[row] = _matrix[row, index];
					}
					return eigenvector;
				}
				return Vector3Dex.Zero;
			}

			/// <summary>
			/// Gets eigenvector. Size of the resulting array is equal to eigen system size. Index must be 0&lt;=index&lt;Size
			/// </summary>
			public double[] GetEigenvector(int index)
			{
				double[] eigenvector = new double[_size];
				for (int row = 0; row < _size; ++row)
				{
					eigenvector[row] = _matrix[row, index];
				}
				return eigenvector;
			}

			/// <summary>
			/// Gets eigenvector. Size of the array must match eigen system size. Method will fill in components of eigenvector into the array.
			/// Index must be 0&lt;=index&lt;Size
			/// </summary>
			public void GetEigenvector(int index, double[] out_eigenvector)
			{
				for (int row = 0; row < _size; ++row)
				{
					out_eigenvector[row] = _matrix[row, index];
				}
			}
		}

		public static class EigenDecomposition
		{
			// Householder reduction to tridiagonal form.

			private static void Tridiagonal2(double[] diagonal, double[] subdiagonal, double[,] matrix, out bool isRotation)
			{
				// The matrix is already tridiagonal.
				diagonal[0] = matrix[0, 0];
				diagonal[1] = matrix[1, 1];

				subdiagonal[0] = matrix[0, 1];
				subdiagonal[1] = 0f;

				matrix[0, 0] = 1f;
				matrix[0, 1] = 0f;
				matrix[1, 0] = 0f;
				matrix[1, 1] = 1f;

				isRotation = true;
			}

			private static void Tridiagonal3(double[] diagonal, double[] subdiagonal, double[,] matrix, out bool isRotation)
			{
				double m00 = matrix[0, 0];
				double m01 = matrix[0, 1];
				double m02 = matrix[0, 2];
				double m11 = matrix[1, 1];
				double m12 = matrix[1, 2];
				double m22 = matrix[2, 2];

				diagonal[0] = m00;
				subdiagonal[2] = 0f;

				if (System.Math.Abs(m02) > Mathex.ZeroTolerance)
				{
					double length = System.Math.Sqrt(m01 * m01 + m02 * m02);
					double invLength = 1f / length;
					m01 *= invLength;
					m02 *= invLength;
					double q = 2f * m01 * m12 + m02 * (m22 - m11);

					diagonal[1] = m11 + m02 * q;
					diagonal[2] = m22 - m02 * q;

					subdiagonal[0] = length;
					subdiagonal[1] = m12 - m01 * q;

					matrix[0, 0] = 1f;
					matrix[0, 1] = 0f;
					matrix[0, 2] = 0f;
					matrix[1, 0] = 0f;
					matrix[1, 1] = m01;
					matrix[1, 2] = m02;
					matrix[2, 0] = 0f;
					matrix[2, 1] = m02;
					matrix[2, 2] = -m01;

					isRotation = false;
				}
				else
				{
					diagonal[1] = m11;
					diagonal[2] = m22;

					subdiagonal[0] = m01;
					subdiagonal[1] = m12;

					matrix[0, 0] = 1f;
					matrix[0, 1] = 0f;
					matrix[0, 2] = 0f;
					matrix[1, 0] = 0f;
					matrix[1, 1] = 1f;
					matrix[1, 2] = 0f;
					matrix[2, 0] = 0f;
					matrix[2, 1] = 0f;
					matrix[2, 2] = 1f;

					isRotation = true;
				}
			}

			private static void TridiagonalN(double[] diagonal, double[] subdiagonal, double[,] matrix, out bool isRotation)
			{
				int i0, i1, i2, i3;
				int size = diagonal.Length;

				for (i0 = size - 1, i3 = size - 2; i0 >= 1; --i0, --i3)
				{
					double value0 = 0f;
					double scale  = 0f;

					if (i3 > 0)
					{
						for (i2 = 0; i2 <= i3; ++i2)
						{
							scale += System.Math.Abs(matrix[i0, i2]);
						}
						if (scale == 0f)
						{
							subdiagonal[i0] = matrix[i0, i3];
						}
						else
						{
							double invScale = 1f / scale;
							for (i2 = 0; i2 <= i3; ++i2)
							{
								matrix[i0, i2] *= invScale;
								value0 += matrix[i0, i2] * matrix[i0, i2];
							}

							double value1 = matrix[i0, i3];
							double value2 = System.Math.Sqrt(value0);
							if (value1 > 0f)
							{
								value2 = -value2;
							}

							subdiagonal[i0] = scale * value2;
							value0 -= value1 * value2;
							matrix[i0, i3] = value1 - value2;
							value1 = 0f;
							double invValue0 = 1f / value0;
							for (i1 = 0; i1 <= i3; ++i1)
							{
								matrix[i1, i0] = matrix[i0, i1] * invValue0;
								value2 = 0f;
								for (i2 = 0; i2 <= i1; ++i2)
								{
									value2 += matrix[i1, i2] * matrix[i0, i2];
								}
								for (i2 = i1 + 1; i2 <= i3; ++i2)
								{
									value2 += matrix[i2, i1] * matrix[i0, i2];
								}
								subdiagonal[i1] = value2 * invValue0;
								value1 += subdiagonal[i1] * matrix[i0, i1];
							}

							double value3 = 0.5f * value1 * invValue0;
							for (i1 = 0; i1 <= i3; ++i1)
							{
								value1 = matrix[i0, i1];
								value2 = subdiagonal[i1] - value3 * value1;
								subdiagonal[i1] = value2;
								for (i2 = 0; i2 <= i1; i2++)
								{
									matrix[i1, i2] -= value1 * subdiagonal[i2] + value2 * matrix[i0, i2];
								}
							}
						}
					}
					else
					{
						subdiagonal[i0] = matrix[i0, i3];
					}

					diagonal[i0] = value0;
				}

				diagonal[0] = 0f;
				subdiagonal[0] = 0f;
				for (i0 = 0, i3 = -1; i0 <= size - 1; ++i0, ++i3)
				{
					if (diagonal[i0] != 0f)
					{
						for (i1 = 0; i1 <= i3; ++i1)
						{
							double sum = 0f;
							for (i2 = 0; i2 <= i3; ++i2)
							{
								sum += matrix[i0, i2] * matrix[i2, i1];
							}
							for (i2 = 0; i2 <= i3; ++i2)
							{
								matrix[i2, i1] -= sum * matrix[i2, i0];
							}
						}
					}
					diagonal[i0] = matrix[i0, i0];
					matrix[i0, i0] = 1f;
					for (i1 = 0; i1 <= i3; ++i1)
					{
						matrix[i1, i0] = 0f;
						matrix[i0, i1] = 0f;
					}
				}

				// Reordering needed by EigenDecomposition::QLAlgorithm.
				for (i0 = 1, i3 = 0; i0 < size; ++i0, ++i3)
				{
					subdiagonal[i3] = subdiagonal[i0];
				}
				subdiagonal[size - 1] = 0f;

				isRotation = (size % 2) == 0;
			}

			/// <summary>
			/// QL algorithm with implicit shifting.  This function is called for tridiagonal matrices.
			/// </summary>
			private static bool QLAlgorithm(double[] diagonal, double[] subdiagonal, double[,] matrix)
			{
				int maxIter = 32;
				int size = diagonal.Length;

				for (int i0 = 0; i0 < size; ++i0)
				{
					int i1;
					for (i1 = 0; i1 < maxIter; ++i1)
					{
						int i2;
						for (i2 = i0; i2 <= size - 2; ++i2)
						{
							double tmp = System.Math.Abs(diagonal[i2]) + System.Math.Abs(diagonal[i2 + 1]);

							if (System.Math.Abs(subdiagonal[i2]) + tmp == tmp)
							{
								break;
							}
						}
						if (i2 == i0)
						{
							break;
						}

						double value0 = (diagonal[i0 + 1] - diagonal[i0]) / (2f * subdiagonal[i0]);
						double value1 = System.Math.Sqrt(value0 * value0 + 1f);
						if (value0 < 0f)
						{
							value0 = diagonal[i2] - diagonal[i0] + subdiagonal[i0] / (value0 - value1);
						}
						else
						{
							value0 = diagonal[i2] - diagonal[i0] + subdiagonal[i0] / (value0 + value1);
						}

						double sn = (double)1, cs = (double)1, value2 = (double)0;
						for (int i3 = i2 - 1; i3 >= i0; --i3)
						{
							double value3 = sn * subdiagonal[i3];
							double value4 = cs * subdiagonal[i3];
							if (System.Math.Abs(value3) >= System.Math.Abs(value0))
							{
								cs = value0 / value3;
								value1 = System.Math.Sqrt(cs * cs + 1f);
								subdiagonal[i3 + 1] = value3 * value1;
								sn = 1f / value1;
								cs *= sn;
							}
							else
							{
								sn = value3 / value0;
								value1 = System.Math.Sqrt(sn * sn + 1f);
								subdiagonal[i3 + 1] = value0 * value1;
								cs = 1f / value1;
								sn *= cs;
							}

							value0 = diagonal[i3 + 1] - value2;
							value1 = (diagonal[i3] - value0) * sn + 2f * value4 * cs;
							value2 = sn * value1;
							diagonal[i3 + 1] = value0 + value2;
							value0 = cs * value1 - value4;

							for (int i4 = 0; i4 < size; ++i4)
							{
								value3 = matrix[i4, i3 + 1];
								matrix[i4, i3 + 1] = sn * matrix[i4, i3] + cs * value3;
								matrix[i4, i3] = cs * matrix[i4, i3] - sn * value3;
							}
						}
						diagonal[i0] -= value2;
						subdiagonal[i0] = value0;
						subdiagonal[i2] = 0f;
					}

					if (i1 == maxIter)
					{
						return false;
					}
				}

				return true;
			}

			/// <summary>
			/// Sort eigenvalues from smallest to largest.
			/// </summary>
			private static void IncreasingSort(double[] diagonal, double[] subdiagonal, double[,] matrix, ref bool isRotation)
			{
				int size = diagonal.Length;

				// Sort the eigenvalues in increasing order, e[0] <= ... <= e[mSize-1]
				for (int i0 = 0, i1; i0 <= size - 2; ++i0)
				{
					// Locate the minimum eigenvalue.
					i1 = i0;
					double minValue = diagonal[i1];
					int i2;
					for (i2 = i0 + 1; i2 < size; ++i2)
					{
						if (diagonal[i2] < minValue)
						{
							i1 = i2;
							minValue = diagonal[i1];
						}
					}

					if (i1 != i0)
					{
						// Swap the eigenvalues.
						diagonal[i1] = diagonal[i0];
						diagonal[i0] = minValue;

						// Swap the eigenvectors corresponding to the eigenvalues.
						for (i2 = 0; i2 < size; ++i2)
						{
							double tmp = matrix[i2,i0];
							matrix[i2,i0] = matrix[i2,i1];
							matrix[i2,i1] = tmp;
							isRotation = !isRotation;
						}
					}
				}
			}

			/// <summary>
			/// Sort eigenvalues from largest to smallest.
			/// </summary>
			private static void DecreasingSort(double[] diagonal, double[] subdiagonal, double[,] matrix, ref bool isRotation)
			{
				int size = diagonal.Length;

				// Sort the eigenvalues in decreasing order, e[0] >= ... >= e[mSize-1]
				for (int i0 = 0, i1; i0 <= size - 2; ++i0)
				{
					// Locate the maximum eigenvalue.
					i1 = i0;
					double maxValue = diagonal[i1];
					int i2;
					for (i2 = i0 + 1; i2 < size; ++i2)
					{
						if (diagonal[i2] > maxValue)
						{
							i1 = i2;
							maxValue = diagonal[i1];
						}
					}

					if (i1 != i0)
					{
						// Swap the eigenvalues.
						diagonal[i1] = diagonal[i0];
						diagonal[i0] = maxValue;

						// Swap the eigenvectors corresponding to the eigenvalues.
						for (i2 = 0; i2 < size; ++i2)
						{
							double tmp = matrix[i2,i0];
							matrix[i2,i0] = matrix[i2,i1];
							matrix[i2,i1] = tmp;
							isRotation = !isRotation;
						}
					}
				}
			}			
			
			private static void GuaranteeRotation(double[,] matrix, bool isRotation)
			{
				if (!isRotation)
				{
					int size = matrix.GetLength(0);

					// Change sign on the first column.
					for (int row = 0; row < size; ++row)
					{
						matrix[row, 0] = -matrix[row, 0];
					}
				}
			}

			
			/// <summary>
			/// Solve the eigensystem. Set increasingSort to true when you want
			/// the eigenvalues to be sorted in increasing order (from smallest to largest);
			/// otherwise, the eigenvalues are sorted in decreasing order (from largest to smallest).
			/// </summary>
			/// <param name="symmetricSquareMatrix">Matrix must be square and symmetric. Matrix size must be >= 2.</param>
			/// <param name="increasingSort">true for increasing sort, false for decreasing sort.</param>
			/// <returns>Data containing eigenvalues and eigenvectors or null if matrix is non-square or size is &lt; 2.</returns>
			public static EigenData Solve(double[,] symmetricSquareMatrix, bool increasingSort)
			{
				int size;
				if ((size = symmetricSquareMatrix.GetLength(0)) != symmetricSquareMatrix.GetLength(1))
				{
					return null;
				}
				if (size < 2)
				{
					return null;
				}

				double[,] matrix = new double[size, size];
				Buffer.BlockCopy(symmetricSquareMatrix, 0, matrix, 0, symmetricSquareMatrix.Length * sizeof(double));
				double[] diagonal    = new double[size];
				double[] subdiagonal = new double[size];

				// For odd size matrices, the Householder reduction involves an odd
				// number of reflections.  The product of these is a reflection.  The
				// QL algorithm uses rotations for further reductions.  The final
				// orthogonal matrix whose columns are the eigenvectors is a reflection,
				// so its determinant is -1.  For even size matrices, the Householder
				// reduction involves an even number of reflections whose product is a
				// rotation.  The final orthogonal matrix has determinant +1.  Many
				// algorithms that need an eigendecomposition want a rotation matrix.
				// We want to guarantee this is the case, so mRotation keeps track of
				// this.  The DecrSort and IncrSort further complicate the issue since
				// they swap columns of the orthogonal matrix, causing the matrix to
				// toggle between rotation and reflection.  The value mRotation must
				// be toggled accordingly.
				bool    isRotation;

				if (size == 2)
				{
					Tridiagonal2(diagonal, subdiagonal, matrix, out isRotation);
				}
				else if (size == 3)
				{
					Tridiagonal3(diagonal, subdiagonal, matrix, out isRotation);
				}
				else
				{
					TridiagonalN(diagonal, subdiagonal, matrix, out isRotation);
				}

				QLAlgorithm(diagonal, subdiagonal, matrix);

				if (increasingSort)
				{
					IncreasingSort(diagonal, subdiagonal, matrix, ref isRotation);
				}
				else
				{
					DecreasingSort(diagonal, subdiagonal, matrix, ref isRotation);
				}

				GuaranteeRotation(matrix, isRotation);				

				return new EigenData(diagonal, matrix);
			}
		}
	}
}
