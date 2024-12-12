

namespace Dest
{
	namespace Math
	{
		public static class Matrix4X4ex
		{
			public static readonly Matrix4X4 Identity = new Matrix4X4() { m00 = 1, m11 = 1, m22 = 1, m33 = 1 };

			/// <summary>
			/// Converts rotation matrix to quaternion
			/// </summary>
			public static void RotationMatrixToQuaternion(ref Matrix4X4 matrix, out Quaternion quaternion)
			{
				quaternion = Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
			}

			/// <summary>
			/// Converts quaternion to rotation matrix
			/// </summary>
			public static void QuaternionToRotationMatrix(Quaternion quaternion, out Matrix4X4 matrix)
			{
				double qx = quaternion.x;
				double qy = quaternion.y;
				double qz = quaternion.z;
				double qw = quaternion.w;

				matrix.m00 = 1.0f - 2.0f * qy * qy - 2.0f * qz * qz;
				matrix.m01 = 2.0f * qx * qy - 2.0f * qz * qw;
				matrix.m02 = 2.0f * qx * qz + 2.0f * qy * qw;
				matrix.m03 = 0.0f;

				matrix.m10 = 2.0f * qx * qy + 2.0f * qz * qw;
				matrix.m11 = 1.0f - 2.0f * qx * qx - 2.0f * qz * qz;
				matrix.m12 = 2.0f * qy * qz - 2.0f * qx * qw;
				matrix.m13 = 0.0f;

				matrix.m20 = 2.0f * qx * qz - 2.0f * qy * qw;
				matrix.m21 = 2.0f * qy * qz + 2.0f * qx * qw;
				matrix.m22 = 1.0f - 2.0f * qx * qx - 2.0f * qy * qy;
				matrix.m23 = 0.0f;

				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Converts quaternion to rotation matrix
			/// </summary>
			public static void QuaternionToRotationMatrix(ref Quaternion quaternion, out Matrix4X4 matrix)
			{
				double qx = quaternion.x;
				double qy = quaternion.y;
				double qz = quaternion.z;
				double qw = quaternion.w;

				matrix.m00 = 1.0f - 2.0f * qy * qy - 2.0f * qz * qz;
				matrix.m01 = 2.0f * qx * qy - 2.0f * qz * qw;
				matrix.m02 = 2.0f * qx * qz + 2.0f * qy * qw;
				matrix.m03 = 0.0f;

				matrix.m10 = 2.0f * qx * qy + 2.0f * qz * qw;
				matrix.m11 = 1.0f - 2.0f * qx * qx - 2.0f * qz * qz;
				matrix.m12 = 2.0f * qy * qz - 2.0f * qx * qw;
				matrix.m13 = 0.0f;

				matrix.m20 = 2.0f * qx * qz - 2.0f * qy * qw;
				matrix.m21 = 2.0f * qy * qz + 2.0f * qx * qw;
				matrix.m22 = 1.0f - 2.0f * qx * qx - 2.0f * qy * qy;
				matrix.m23 = 0.0f;

				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates translation matrix
			/// </summary>
			public static void CreateTranslation(Vector3D position, out Matrix4X4 matrix)
			{
				matrix.m01 = 0.0f;
				matrix.m02 = 0.0f;
				matrix.m10 = 0.0f;
				matrix.m12 = 0.0f;
				matrix.m20 = 0.0f;
				matrix.m21 = 0.0f;
				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;

				matrix.m03 = position.x;
				matrix.m13 = position.y;
				matrix.m23 = position.z;

				matrix.m00 = 0.0f;
				matrix.m11 = 0.0f;
				matrix.m22 = 0.0f;
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates translation matrix
			/// </summary>
			public static void CreateTranslation(ref Vector3D position, out Matrix4X4 matrix)
			{
				matrix.m01 = 0.0f;
				matrix.m02 = 0.0f;
				matrix.m10 = 0.0f;
				matrix.m12 = 0.0f;
				matrix.m20 = 0.0f;
				matrix.m21 = 0.0f;
				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;

				matrix.m03 = position.x;
				matrix.m13 = position.y;
				matrix.m23 = position.z;

				matrix.m00 = 0.0f;
				matrix.m11 = 0.0f;
				matrix.m22 = 0.0f;
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates non-uniform scale matrix
			/// </summary>
			public static void CreateScale(Vector3D scale, out Matrix4X4 matrix)
			{
				matrix.m01 = 0.0f;
				matrix.m02 = 0.0f;
				matrix.m03 = 0.0f;
				matrix.m10 = 0.0f;
				matrix.m12 = 0.0f;
				matrix.m13 = 0.0f;
				matrix.m20 = 0.0f;
				matrix.m21 = 0.0f;
				matrix.m23 = 0.0f;
				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f; 
				matrix.m32 = 0.0f;

				matrix.m00 = scale.x;
				matrix.m11 = scale.y;
				matrix.m22 = scale.z;

				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates non-uniform scale matrix
			/// </summary>
			public static void CreateScale(ref Vector3D scale, out Matrix4X4 matrix)
			{
				matrix.m01 = 0.0f;
				matrix.m02 = 0.0f;
				matrix.m03 = 0.0f;
				matrix.m10 = 0.0f;
				matrix.m12 = 0.0f;
				matrix.m13 = 0.0f;
				matrix.m20 = 0.0f;
				matrix.m21 = 0.0f;
				matrix.m23 = 0.0f;
				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;

				matrix.m00 = scale.x;
				matrix.m11 = scale.y;
				matrix.m22 = scale.z;

				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates uniform scale matrix
			/// </summary>
			public static void CreateScale(double scale, out Matrix4X4 matrix)
			{
				matrix.m01 = 0.0f;
				matrix.m02 = 0.0f;
				matrix.m03 = 0.0f;
				matrix.m10 = 0.0f;
				matrix.m12 = 0.0f;
				matrix.m13 = 0.0f;
				matrix.m20 = 0.0f;
				matrix.m21 = 0.0f;
				matrix.m23 = 0.0f;
				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;

				matrix.m00 = scale;
				matrix.m11 = scale;
				matrix.m22 = scale;

				matrix.m33 = 1.0f;
			}
		
			/// <summary>
			/// Creates rotaion matrix using euler angles (order is the same as in Quaternion.Euler() method)
			/// </summary>
			public static void CreateRotationEuler(double eulerX, double eulerY, double eulerZ, out Matrix4X4 matrix)
			{
				Quaternion quaternion = Quaternion.Euler(eulerX, eulerY, eulerZ);
				QuaternionToRotationMatrix(ref quaternion, out matrix);
			}

			/// <summary>
			/// Creates rotaion matrix using euler angles (order is the same as in Quaternion.Euler() method)
			/// </summary>
			public static void CreateRotationEuler(Vector3D eulerAngles, out Matrix4X4 matrix)
			{
				Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
				QuaternionToRotationMatrix(ref quaternion, out matrix);
			}

			/// <summary>
			/// Creates rotaion matrix using euler angles (order is the same as in Quaternion.Euler() method)
			/// </summary>
			public static void CreateRotationEuler(ref Vector3D eulerAngles, out Matrix4X4 matrix)
			{
				Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
				QuaternionToRotationMatrix(ref quaternion, out matrix);
			}

			/// <summary>
			/// Creates a matrix that rotates around x-axis
			/// </summary>
			public static void CreateRotationX(double angleInDegrees, out Matrix4X4 matrix)
			{
				double angle = angleInDegrees * Mathex.Deg2Rad;
				double sin = System.Math.Sin(angle);
				double cos = System.Math.Cos(angle);

				matrix.m00 = 1.0f;
				matrix.m01 = 0.0f;
				matrix.m02 = 0.0f;
				matrix.m03 = 0.0f;

				matrix.m10 = 0.0f;
				matrix.m11 = cos;
				matrix.m12 = -sin;
				matrix.m13 = 0.0f;

				matrix.m20 = 0.0f;
				matrix.m21 = sin;
				matrix.m22 = cos;
				matrix.m23 = 0.0f;

				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates a matrix that rotates around y-axis
			/// </summary>
			public static void CreateRotationY(double angleInDegrees, out Matrix4X4 matrix)
			{
				double angle = angleInDegrees * Mathex.Deg2Rad;
				double sin = System.Math.Sin(angle);
				double cos = System.Math.Cos(angle);

				matrix.m00 = cos;
				matrix.m01 = 0.0f;
				matrix.m02 = sin;
				matrix.m03 = 0.0f;

				matrix.m10 = 0.0f;
				matrix.m11 = 1.0f;
				matrix.m12 = 0.0f;
				matrix.m13 = 0.0f;

				matrix.m20 = -sin;
				matrix.m21 = 0.0f;
				matrix.m22 = cos;
				matrix.m23 = 0.0f;

				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates a matrix that rotates around z-axis
			/// </summary>
			public static void CreateRotationZ(double angleInDegrees, out Matrix4X4 matrix)
			{
				double angle = angleInDegrees * Mathex.Deg2Rad;
				double sin = System.Math.Sin(angle);
				double cos = System.Math.Cos(angle);

				matrix.m00 = cos;
				matrix.m01 = -sin;
				matrix.m02 = 0.0f;
				matrix.m03 = 0.0f;

				matrix.m10 = sin;
				matrix.m11 = cos;
				matrix.m12 = 0.0f;
				matrix.m13 = 0.0f;

				matrix.m20 = 0.0f;
				matrix.m21 = 0.0f;
				matrix.m22 = 1.0f;
				matrix.m23 = 0.0f;

				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;
				matrix.m33 = 1.0f;
			}
			
			/// <summary>
			/// Creates a matrix that rotates around an arbirary axis (function will normalize axis)
			/// </summary>
			public static void CreateRotationAngleAxis(double angleInDegrees, Vector3D rotationAxis, out Matrix4X4 matrix)
			{
				Vector3D axis = rotationAxis.normalized;
				double angle = angleInDegrees * Mathex.Deg2Rad;

				double cos = System.Math.Cos(angle);
				double sin = System.Math.Sin(angle);
				double oneMinusCos = 1f - cos;
				double x2 = axis.x * axis.x;
				double y2 = axis.y * axis.y;
				double z2 = axis.z * axis.z;
				double xym = axis.x * axis.y * oneMinusCos;
				double xzm = axis.x * axis.z * oneMinusCos;
				double yzm = axis.y * axis.z * oneMinusCos;
				double xSin = axis.x * sin;
				double ySin = axis.y * sin;
				double zSin = axis.z * sin;

				matrix.m00 = x2 * oneMinusCos + cos;
				matrix.m01 = xym - zSin;
				matrix.m02 = xzm + ySin;

				matrix.m10 = xym + zSin;
				matrix.m11 = y2 * oneMinusCos + cos;
				matrix.m12 = yzm - xSin;

				matrix.m20 = xzm - ySin;
				matrix.m21 = yzm + xSin;
				matrix.m22 = z2 * oneMinusCos + cos;

				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;
				matrix.m03 = 0.0f;
				matrix.m13 = 0.0f; 
				matrix.m23 = 0.0f;
				
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates a matrix that rotates around an arbirary axis (caller must provide unit-length axis)
			/// </summary>
			public static void CreateRotationAngleUnitAxis(double angleInDegrees, Vector3D normalizedAxis, out Matrix4X4 matrix)
			{
				double angle = angleInDegrees * Mathex.Deg2Rad;

				double cos = System.Math.Cos(angle);
				double sin = System.Math.Sin(angle);
				double oneMinusCos = 1f - cos;
				double x2 = normalizedAxis.x * normalizedAxis.x;
				double y2 = normalizedAxis.y * normalizedAxis.y;
				double z2 = normalizedAxis.z * normalizedAxis.z;
				double xym = normalizedAxis.x * normalizedAxis.y * oneMinusCos;
				double xzm = normalizedAxis.x * normalizedAxis.z * oneMinusCos;
				double yzm = normalizedAxis.y * normalizedAxis.z * oneMinusCos;
				double xSin = normalizedAxis.x * sin;
				double ySin = normalizedAxis.y * sin;
				double zSin = normalizedAxis.z * sin;

				matrix.m00 = x2 * oneMinusCos + cos;
				matrix.m01 = xym - zSin;
				matrix.m02 = xzm + ySin;

				matrix.m10 = xym + zSin;
				matrix.m11 = y2 * oneMinusCos + cos;
				matrix.m12 = yzm - xSin;

				matrix.m20 = xzm - ySin;
				matrix.m21 = yzm + xSin;
				matrix.m22 = z2 * oneMinusCos + cos;

				matrix.m30 = 0.0f;
				matrix.m31 = 0.0f;
				matrix.m32 = 0.0f;
				matrix.m03 = 0.0f;
				matrix.m13 = 0.0f;
				matrix.m23 = 0.0f;
				
				matrix.m33 = 1.0f;
			}

			/// <summary>
			/// Creates a matrix that rotates around specified point
			public static void CreateRotation(Vector3D rotationOrigin, Quaternion rotation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x);
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y);
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z);
			}

			/// <summary>
			/// Creates a matrix that rotates around specified point
			public static void CreateRotation(ref Vector3D rotationOrigin, ref Quaternion rotation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x);
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y);
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z);
			}

			/// <summary>
			/// Transposes given matrix
			/// </summary>
			/// <param name="matrix">Matrix to transpose (will be overriden to contain output)</param>
			public static void Transpose(ref Matrix4X4 matrix)
			{
				double temp;

				temp = matrix.m01;
				matrix.m01 = matrix.m10;
				matrix.m10 = temp;

				temp = matrix.m02;
				matrix.m02 = matrix.m20;
				matrix.m20 = temp;

				temp = matrix.m03;
				matrix.m03 = matrix.m30;
				matrix.m30 = temp;

				temp = matrix.m12;
				matrix.m12 = matrix.m21;
				matrix.m21 = temp;

				temp = matrix.m13;
				matrix.m13 = matrix.m31;
				matrix.m31 = temp;

				temp = matrix.m23;
				matrix.m23 = matrix.m32;
				matrix.m32 = temp;
			}

			/// <summary>
			/// Transposes given matrix
			/// </summary>
			/// <param name="matrix">Matrix to transpose</param>
			/// <param name="transpose">Output containing transposed matrix</param>
			public static void Transpose(ref Matrix4X4 matrix, out Matrix4X4 transpose)
			{
				transpose.m00 = matrix.m00;
				transpose.m01 = matrix.m10;
				transpose.m02 = matrix.m20;
				transpose.m03 = matrix.m30;
				
				transpose.m10 = matrix.m01;
				transpose.m11 = matrix.m11;
				transpose.m12 = matrix.m21;
				transpose.m13 = matrix.m31;
				
				transpose.m20 = matrix.m02;
				transpose.m21 = matrix.m12;
				transpose.m22 = matrix.m22;
				transpose.m23 = matrix.m32;
				
				transpose.m30 = matrix.m03;
				transpose.m31 = matrix.m13;
				transpose.m32 = matrix.m23;
				transpose.m33 = matrix.m33;
			}

			/// <summary>
			/// Returns matrix determinant
			/// </summary>
			public static double CalcDeterminant(ref Matrix4X4 matrix)
			{
				double a0 = matrix.m00 * matrix.m11 - matrix.m10 * matrix.m01;
				double a1 = matrix.m00 * matrix.m21 - matrix.m20 * matrix.m01;
				double a2 = matrix.m00 * matrix.m31 - matrix.m30 * matrix.m01;
				double a3 = matrix.m10 * matrix.m21 - matrix.m20 * matrix.m11;
				double a4 = matrix.m10 * matrix.m31 - matrix.m30 * matrix.m11;
				double a5 = matrix.m20 * matrix.m31 - matrix.m30 * matrix.m21;
				double b0 = matrix.m02 * matrix.m13 - matrix.m12 * matrix.m03;
				double b1 = matrix.m02 * matrix.m23 - matrix.m22 * matrix.m03;
				double b2 = matrix.m02 * matrix.m33 - matrix.m32 * matrix.m03;
				double b3 = matrix.m12 * matrix.m23 - matrix.m22 * matrix.m13;
				double b4 = matrix.m12 * matrix.m33 - matrix.m32 * matrix.m13;
				double b5 = matrix.m22 * matrix.m33 - matrix.m32 * matrix.m23;

				return a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;
			}

			/// <summary>
			/// Inverses given matrix
			/// </summary>
			/// <param name="matrix">Matrix to inverse (will be overriden to contain output)</param>
			/// <param name="epsilon">Small positive number used to compare determinant with zero</param>
			public static void Inverse(ref Matrix4X4 matrix, double epsilon = Mathex.ZeroTolerance)
			{
				double a0 = matrix.m00 * matrix.m11 - matrix.m10 * matrix.m01;
				double a1 = matrix.m00 * matrix.m21 - matrix.m20 * matrix.m01;
				double a2 = matrix.m00 * matrix.m31 - matrix.m30 * matrix.m01;
				double a3 = matrix.m10 * matrix.m21 - matrix.m20 * matrix.m11;
				double a4 = matrix.m10 * matrix.m31 - matrix.m30 * matrix.m11;
				double a5 = matrix.m20 * matrix.m31 - matrix.m30 * matrix.m21;
				double b0 = matrix.m02 * matrix.m13 - matrix.m12 * matrix.m03;
				double b1 = matrix.m02 * matrix.m23 - matrix.m22 * matrix.m03;
				double b2 = matrix.m02 * matrix.m33 - matrix.m32 * matrix.m03;
				double b3 = matrix.m12 * matrix.m23 - matrix.m22 * matrix.m13;
				double b4 = matrix.m12 * matrix.m33 - matrix.m32 * matrix.m13;
				double b5 = matrix.m22 * matrix.m33 - matrix.m32 * matrix.m23;

				double det = a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;
				if (System.Math.Abs(det) > epsilon)
				{
					Matrix4X4 inverse;

					inverse.m00 = +matrix.m11 * b5 - matrix.m21 * b4 + matrix.m31 * b3;
					inverse.m01 = -matrix.m01 * b5 + matrix.m21 * b2 - matrix.m31 * b1;
					inverse.m02 = +matrix.m01 * b4 - matrix.m11 * b2 + matrix.m31 * b0;
					inverse.m03 = -matrix.m01 * b3 + matrix.m11 * b1 - matrix.m21 * b0;
					inverse.m10 = -matrix.m10 * b5 + matrix.m20 * b4 - matrix.m30 * b3;
					inverse.m11 = +matrix.m00 * b5 - matrix.m20 * b2 + matrix.m30 * b1;
					inverse.m12 = -matrix.m00 * b4 + matrix.m10 * b2 - matrix.m30 * b0;
					inverse.m13 = +matrix.m00 * b3 - matrix.m10 * b1 + matrix.m20 * b0;
					inverse.m20 = +matrix.m13 * a5 - matrix.m23 * a4 + matrix.m33 * a3;
					inverse.m21 = -matrix.m03 * a5 + matrix.m23 * a2 - matrix.m33 * a1;
					inverse.m22 = +matrix.m03 * a4 - matrix.m13 * a2 + matrix.m33 * a0;
					inverse.m23 = -matrix.m03 * a3 + matrix.m13 * a1 - matrix.m23 * a0;
					inverse.m30 = -matrix.m12 * a5 + matrix.m22 * a4 - matrix.m32 * a3;
					inverse.m31 = +matrix.m02 * a5 - matrix.m22 * a2 + matrix.m32 * a1;
					inverse.m32 = -matrix.m02 * a4 + matrix.m12 * a2 - matrix.m32 * a0;
					inverse.m33 = +matrix.m02 * a3 - matrix.m12 * a1 + matrix.m22 * a0;

					double invDet = 1f / det;
					inverse.m00 *= invDet;
					inverse.m01 *= invDet;
					inverse.m02 *= invDet;
					inverse.m03 *= invDet;
					inverse.m10 *= invDet;
					inverse.m11 *= invDet;
					inverse.m12 *= invDet;
					inverse.m13 *= invDet;
					inverse.m20 *= invDet;
					inverse.m21 *= invDet;
					inverse.m22 *= invDet;
					inverse.m23 *= invDet;
					inverse.m30 *= invDet;
					inverse.m31 *= invDet;
					inverse.m32 *= invDet;
					inverse.m33 *= invDet;

					matrix = inverse;
				}
				else
				{
					matrix = Matrix4X4.zero;
				}
			}

			/// <summary>
			/// Inverses given matrix. IMPORTANT: 'matrix' and 'inverse' parameters must be different!
			/// If you want matrix to contain inverse of itself, use another overload.
			/// </summary>
			/// <param name="matrix">Matrix to inverse</param>
			/// <param name="inverse">Output containing inverse matrix (Must not be the same variable as 'matrix'!)</param>
			/// <param name="epsilon">Small positive number used to compare determinant with zero</param>
			public static void Inverse(ref Matrix4X4 matrix, out Matrix4X4 inverse, double epsilon = Mathex.ZeroTolerance)
			{
				double a0 = matrix.m00 * matrix.m11 - matrix.m10 * matrix.m01;
				double a1 = matrix.m00 * matrix.m21 - matrix.m20 * matrix.m01;
				double a2 = matrix.m00 * matrix.m31 - matrix.m30 * matrix.m01;
				double a3 = matrix.m10 * matrix.m21 - matrix.m20 * matrix.m11;
				double a4 = matrix.m10 * matrix.m31 - matrix.m30 * matrix.m11;
				double a5 = matrix.m20 * matrix.m31 - matrix.m30 * matrix.m21;
				double b0 = matrix.m02 * matrix.m13 - matrix.m12 * matrix.m03;
				double b1 = matrix.m02 * matrix.m23 - matrix.m22 * matrix.m03;
				double b2 = matrix.m02 * matrix.m33 - matrix.m32 * matrix.m03;
				double b3 = matrix.m12 * matrix.m23 - matrix.m22 * matrix.m13;
				double b4 = matrix.m12 * matrix.m33 - matrix.m32 * matrix.m13;
				double b5 = matrix.m22 * matrix.m33 - matrix.m32 * matrix.m23;

				double det = a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;
				if (System.Math.Abs(det) > epsilon)
				{					
					inverse.m00 = + matrix.m11 * b5 - matrix.m21 * b4 + matrix.m31 * b3;
					inverse.m01 = - matrix.m01 * b5 + matrix.m21 * b2 - matrix.m31 * b1;
					inverse.m02 = + matrix.m01 * b4 - matrix.m11 * b2 + matrix.m31 * b0;
					inverse.m03 = - matrix.m01 * b3 + matrix.m11 * b1 - matrix.m21 * b0;
					inverse.m10 = - matrix.m10 * b5 + matrix.m20 * b4 - matrix.m30 * b3;
					inverse.m11 = + matrix.m00 * b5 - matrix.m20 * b2 + matrix.m30 * b1;
					inverse.m12 = - matrix.m00 * b4 + matrix.m10 * b2 - matrix.m30 * b0;
					inverse.m13 = + matrix.m00 * b3 - matrix.m10 * b1 + matrix.m20 * b0;
					inverse.m20 = + matrix.m13 * a5 - matrix.m23 * a4 + matrix.m33 * a3;
					inverse.m21 = - matrix.m03 * a5 + matrix.m23 * a2 - matrix.m33 * a1;
					inverse.m22 = + matrix.m03 * a4 - matrix.m13 * a2 + matrix.m33 * a0;
					inverse.m23 = - matrix.m03 * a3 + matrix.m13 * a1 - matrix.m23 * a0;
					inverse.m30 = - matrix.m12 * a5 + matrix.m22 * a4 - matrix.m32 * a3;
					inverse.m31 = + matrix.m02 * a5 - matrix.m22 * a2 + matrix.m32 * a1;
					inverse.m32 = - matrix.m02 * a4 + matrix.m12 * a2 - matrix.m32 * a0;
					inverse.m33 = + matrix.m02 * a3 - matrix.m12 * a1 + matrix.m22 * a0;

					double invDet = 1f / det;
					inverse.m00 *= invDet;
					inverse.m01 *= invDet;
					inverse.m02 *= invDet;
					inverse.m03 *= invDet;
					inverse.m10 *= invDet;
					inverse.m11 *= invDet;
					inverse.m12 *= invDet;
					inverse.m13 *= invDet;
					inverse.m20 *= invDet;
					inverse.m21 *= invDet;
					inverse.m22 *= invDet;
					inverse.m23 *= invDet;
					inverse.m30 *= invDet;
					inverse.m31 *= invDet;
					inverse.m32 *= invDet;
					inverse.m33 *= invDet;

					return;
				}

				inverse = Matrix4X4.zero;
			}

			/// <summary>
			/// Copies source matrix into destination matrix
			/// </summary>
			public static void CopyMatrix(ref Matrix4X4 source, out Matrix4X4 destination)
			{
				destination.m00 = source.m00;
				destination.m01 = source.m01;
				destination.m02 = source.m02;
				destination.m03 = source.m03;
				destination.m10 = source.m10;
				destination.m11 = source.m11;
				destination.m12 = source.m12;
				destination.m13 = source.m13;
				destination.m20 = source.m20;
				destination.m21 = source.m21;
				destination.m22 = source.m22;
				destination.m23 = source.m23;
				destination.m30 = source.m30;
				destination.m31 = source.m31;
				destination.m32 = source.m32;
				destination.m33 = source.m33;
			}

			/// <summary>
			/// Multiplies two matrices. Result matrix is matrix0*matrix1. IMPORTANT: 'result' parameter must not
			/// be the same variable as either 'matrix0' or 'matrix1', if you want to store the result in one of
			/// the input matrices, use MultiplyLeft or MultiplyRight methods.
			/// </summary>
			public static void Multiply(ref Matrix4X4 matrix0, ref Matrix4X4 matrix1, out Matrix4X4 result)
			{
				result.m00 = matrix0.m00 * matrix1.m00 + matrix0.m01 * matrix1.m10 + matrix0.m02 * matrix1.m20 + matrix0.m03 * matrix1.m30;
				result.m01 = matrix0.m00 * matrix1.m01 + matrix0.m01 * matrix1.m11 + matrix0.m02 * matrix1.m21 + matrix0.m03 * matrix1.m31;
				result.m02 = matrix0.m00 * matrix1.m02 + matrix0.m01 * matrix1.m12 + matrix0.m02 * matrix1.m22 + matrix0.m03 * matrix1.m32;
				result.m03 = matrix0.m00 * matrix1.m03 + matrix0.m01 * matrix1.m13 + matrix0.m02 * matrix1.m23 + matrix0.m03 * matrix1.m33;
				result.m10 = matrix0.m10 * matrix1.m00 + matrix0.m11 * matrix1.m10 + matrix0.m12 * matrix1.m20 + matrix0.m13 * matrix1.m30;
				result.m11 = matrix0.m10 * matrix1.m01 + matrix0.m11 * matrix1.m11 + matrix0.m12 * matrix1.m21 + matrix0.m13 * matrix1.m31;
				result.m12 = matrix0.m10 * matrix1.m02 + matrix0.m11 * matrix1.m12 + matrix0.m12 * matrix1.m22 + matrix0.m13 * matrix1.m32;
				result.m13 = matrix0.m10 * matrix1.m03 + matrix0.m11 * matrix1.m13 + matrix0.m12 * matrix1.m23 + matrix0.m13 * matrix1.m33;
				result.m20 = matrix0.m20 * matrix1.m00 + matrix0.m21 * matrix1.m10 + matrix0.m22 * matrix1.m20 + matrix0.m23 * matrix1.m30;
				result.m21 = matrix0.m20 * matrix1.m01 + matrix0.m21 * matrix1.m11 + matrix0.m22 * matrix1.m21 + matrix0.m23 * matrix1.m31;
				result.m22 = matrix0.m20 * matrix1.m02 + matrix0.m21 * matrix1.m12 + matrix0.m22 * matrix1.m22 + matrix0.m23 * matrix1.m32;
				result.m23 = matrix0.m20 * matrix1.m03 + matrix0.m21 * matrix1.m13 + matrix0.m22 * matrix1.m23 + matrix0.m23 * matrix1.m33;
				result.m30 = matrix0.m30 * matrix1.m00 + matrix0.m31 * matrix1.m10 + matrix0.m32 * matrix1.m20 + matrix0.m33 * matrix1.m30;
				result.m31 = matrix0.m30 * matrix1.m01 + matrix0.m31 * matrix1.m11 + matrix0.m32 * matrix1.m21 + matrix0.m33 * matrix1.m31;
				result.m32 = matrix0.m30 * matrix1.m02 + matrix0.m31 * matrix1.m12 + matrix0.m32 * matrix1.m22 + matrix0.m33 * matrix1.m32;
				result.m33 = matrix0.m30 * matrix1.m03 + matrix0.m31 * matrix1.m13 + matrix0.m32 * matrix1.m23 + matrix0.m33 * matrix1.m33;
			}

			/// <summary>
			/// Multiplies matrix0 by matrix1 on the right, i.e. the result is matrix0*matrix1.
			/// Output is written into matrix0 parameter.
			/// </summary>
			public static void MultiplyRight(ref Matrix4X4 matrix0, ref Matrix4X4 matrix1)
			{
				Matrix4X4 result;
				result.m00 = matrix0.m00 * matrix1.m00 + matrix0.m01 * matrix1.m10 + matrix0.m02 * matrix1.m20 + matrix0.m03 * matrix1.m30;
				result.m01 = matrix0.m00 * matrix1.m01 + matrix0.m01 * matrix1.m11 + matrix0.m02 * matrix1.m21 + matrix0.m03 * matrix1.m31;
				result.m02 = matrix0.m00 * matrix1.m02 + matrix0.m01 * matrix1.m12 + matrix0.m02 * matrix1.m22 + matrix0.m03 * matrix1.m32;
				result.m03 = matrix0.m00 * matrix1.m03 + matrix0.m01 * matrix1.m13 + matrix0.m02 * matrix1.m23 + matrix0.m03 * matrix1.m33;
				result.m10 = matrix0.m10 * matrix1.m00 + matrix0.m11 * matrix1.m10 + matrix0.m12 * matrix1.m20 + matrix0.m13 * matrix1.m30;
				result.m11 = matrix0.m10 * matrix1.m01 + matrix0.m11 * matrix1.m11 + matrix0.m12 * matrix1.m21 + matrix0.m13 * matrix1.m31;
				result.m12 = matrix0.m10 * matrix1.m02 + matrix0.m11 * matrix1.m12 + matrix0.m12 * matrix1.m22 + matrix0.m13 * matrix1.m32;
				result.m13 = matrix0.m10 * matrix1.m03 + matrix0.m11 * matrix1.m13 + matrix0.m12 * matrix1.m23 + matrix0.m13 * matrix1.m33;
				result.m20 = matrix0.m20 * matrix1.m00 + matrix0.m21 * matrix1.m10 + matrix0.m22 * matrix1.m20 + matrix0.m23 * matrix1.m30;
				result.m21 = matrix0.m20 * matrix1.m01 + matrix0.m21 * matrix1.m11 + matrix0.m22 * matrix1.m21 + matrix0.m23 * matrix1.m31;
				result.m22 = matrix0.m20 * matrix1.m02 + matrix0.m21 * matrix1.m12 + matrix0.m22 * matrix1.m22 + matrix0.m23 * matrix1.m32;
				result.m23 = matrix0.m20 * matrix1.m03 + matrix0.m21 * matrix1.m13 + matrix0.m22 * matrix1.m23 + matrix0.m23 * matrix1.m33;
				result.m30 = matrix0.m30 * matrix1.m00 + matrix0.m31 * matrix1.m10 + matrix0.m32 * matrix1.m20 + matrix0.m33 * matrix1.m30;
				result.m31 = matrix0.m30 * matrix1.m01 + matrix0.m31 * matrix1.m11 + matrix0.m32 * matrix1.m21 + matrix0.m33 * matrix1.m31;
				result.m32 = matrix0.m30 * matrix1.m02 + matrix0.m31 * matrix1.m12 + matrix0.m32 * matrix1.m22 + matrix0.m33 * matrix1.m32;
				result.m33 = matrix0.m30 * matrix1.m03 + matrix0.m31 * matrix1.m13 + matrix0.m32 * matrix1.m23 + matrix0.m33 * matrix1.m33;
				CopyMatrix(ref result, out matrix0);
			}

			/// <summary>
			/// Multiplies matrix1 by matrix0 on the left, i.e. the result is matrix0*matrix1.
			/// Output is written into matrix1 parameter.
			/// </summary>
			public static void MultiplyLeft(ref Matrix4X4 matrix1, ref Matrix4X4 matrix0)
			{
				Matrix4X4 result;
				result.m00 = matrix0.m00 * matrix1.m00 + matrix0.m01 * matrix1.m10 + matrix0.m02 * matrix1.m20 + matrix0.m03 * matrix1.m30;
				result.m01 = matrix0.m00 * matrix1.m01 + matrix0.m01 * matrix1.m11 + matrix0.m02 * matrix1.m21 + matrix0.m03 * matrix1.m31;
				result.m02 = matrix0.m00 * matrix1.m02 + matrix0.m01 * matrix1.m12 + matrix0.m02 * matrix1.m22 + matrix0.m03 * matrix1.m32;
				result.m03 = matrix0.m00 * matrix1.m03 + matrix0.m01 * matrix1.m13 + matrix0.m02 * matrix1.m23 + matrix0.m03 * matrix1.m33;
				result.m10 = matrix0.m10 * matrix1.m00 + matrix0.m11 * matrix1.m10 + matrix0.m12 * matrix1.m20 + matrix0.m13 * matrix1.m30;
				result.m11 = matrix0.m10 * matrix1.m01 + matrix0.m11 * matrix1.m11 + matrix0.m12 * matrix1.m21 + matrix0.m13 * matrix1.m31;
				result.m12 = matrix0.m10 * matrix1.m02 + matrix0.m11 * matrix1.m12 + matrix0.m12 * matrix1.m22 + matrix0.m13 * matrix1.m32;
				result.m13 = matrix0.m10 * matrix1.m03 + matrix0.m11 * matrix1.m13 + matrix0.m12 * matrix1.m23 + matrix0.m13 * matrix1.m33;
				result.m20 = matrix0.m20 * matrix1.m00 + matrix0.m21 * matrix1.m10 + matrix0.m22 * matrix1.m20 + matrix0.m23 * matrix1.m30;
				result.m21 = matrix0.m20 * matrix1.m01 + matrix0.m21 * matrix1.m11 + matrix0.m22 * matrix1.m21 + matrix0.m23 * matrix1.m31;
				result.m22 = matrix0.m20 * matrix1.m02 + matrix0.m21 * matrix1.m12 + matrix0.m22 * matrix1.m22 + matrix0.m23 * matrix1.m32;
				result.m23 = matrix0.m20 * matrix1.m03 + matrix0.m21 * matrix1.m13 + matrix0.m22 * matrix1.m23 + matrix0.m23 * matrix1.m33;
				result.m30 = matrix0.m30 * matrix1.m00 + matrix0.m31 * matrix1.m10 + matrix0.m32 * matrix1.m20 + matrix0.m33 * matrix1.m30;
				result.m31 = matrix0.m30 * matrix1.m01 + matrix0.m31 * matrix1.m11 + matrix0.m32 * matrix1.m21 + matrix0.m33 * matrix1.m31;
				result.m32 = matrix0.m30 * matrix1.m02 + matrix0.m31 * matrix1.m12 + matrix0.m32 * matrix1.m22 + matrix0.m33 * matrix1.m32;
				result.m33 = matrix0.m30 * matrix1.m03 + matrix0.m31 * matrix1.m13 + matrix0.m32 * matrix1.m23 + matrix0.m33 * matrix1.m33;
				CopyMatrix(ref result, out matrix1);
			}

			/// <summary>
			/// Multiplies matrix by a scalar.
			/// </summary>
			/// <param name="matrix">Matrix to multiply (will be overriden to contain output)</param>
			/// <param name="scalar">Scalar to multiply</param>
			public static void Multiply(ref Matrix4X4 matrix, double scalar)
			{
				matrix.m00 *= scalar;
				matrix.m01 *= scalar;
				matrix.m02 *= scalar;
				matrix.m03 *= scalar;
				matrix.m10 *= scalar;
				matrix.m11 *= scalar;
				matrix.m12 *= scalar;
				matrix.m13 *= scalar;
				matrix.m20 *= scalar;
				matrix.m21 *= scalar;
				matrix.m22 *= scalar;
				matrix.m23 *= scalar;
				matrix.m30 *= scalar;
				matrix.m31 *= scalar;
				matrix.m32 *= scalar;
				matrix.m33 *= scalar;
			}

			/// <summary>
			/// Multiplies matrix by a scalar.
			/// </summary>
			/// <param name="matrix">Matrix to multiply</param>
			/// <param name="scalar">Scalar to multiply</param>
			/// <param name="result">Output containing multiplied matrix</param>
			public static void Multiply(ref Matrix4X4 matrix, double scalar, out Matrix4X4 result)
			{
				result.m00 = matrix.m00 * scalar;
				result.m01 = matrix.m01 * scalar;
				result.m02 = matrix.m02 * scalar;
				result.m03 = matrix.m03 * scalar;
				result.m10 = matrix.m10 * scalar;
				result.m11 = matrix.m11 * scalar;
				result.m12 = matrix.m12 * scalar;
				result.m13 = matrix.m13 * scalar;
				result.m20 = matrix.m20 * scalar;
				result.m21 = matrix.m21 * scalar;
				result.m22 = matrix.m22 * scalar;
				result.m23 = matrix.m23 * scalar;
				result.m30 = matrix.m30 * scalar;
				result.m31 = matrix.m31 * scalar;
				result.m32 = matrix.m32 * scalar;
				result.m33 = matrix.m33 * scalar;
			}

			/// <summary>
			/// Multiplies matrix by vector. Reulst vector is matrix*vector.
			/// </summary>
			/// <param name="matrix">Matrix to multiply</param>
			/// <param name="vector">Vector to multiply</param>
			/// <returns>Result of multiplication</returns>
			public static Vector4D Multiply(ref Matrix4X4 matrix, Vector4D vector)
			{
				Vector4D result;
				result.x = matrix.m00 * vector.x + matrix.m01 * vector.y + matrix.m02 * vector.z + matrix.m03 * vector.w;
				result.y = matrix.m10 * vector.x + matrix.m11 * vector.y + matrix.m12 * vector.z + matrix.m13 * vector.w;
				result.z = matrix.m20 * vector.x + matrix.m21 * vector.y + matrix.m22 * vector.z + matrix.m23 * vector.w;
				result.w = matrix.m30 * vector.x + matrix.m31 * vector.y + matrix.m32 * vector.z + matrix.m33 * vector.w;
				return result;
			}

			/// <summary>
			/// Multiplies matrix by vector. Reulst vector is matrix*vector.
			/// </summary>
			/// <param name="matrix">Matrix to multiply</param>
			/// <param name="vector">Vector to multiply</param>
			/// <returns>Result of multiplication</returns>
			public static Vector4D Multiply(ref Matrix4X4 matrix, ref Vector4D vector)
			{
				Vector4D result;
				result.x = matrix.m00 * vector.x + matrix.m01 * vector.y + matrix.m02 * vector.z + matrix.m03 * vector.w;
				result.y = matrix.m10 * vector.x + matrix.m11 * vector.y + matrix.m12 * vector.z + matrix.m13 * vector.w;
				result.z = matrix.m20 * vector.x + matrix.m21 * vector.y + matrix.m22 * vector.z + matrix.m23 * vector.w;
				result.w = matrix.m30 * vector.x + matrix.m31 * vector.y + matrix.m32 * vector.z + matrix.m33 * vector.w;
				return result;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: scaling, rotation, translation.
			/// </summary>
			public static void CreateSRT(Vector3D scaling, Quaternion rotation, Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m00 *= scaling.x;
				result.m10 *= scaling.x;
				result.m20 *= scaling.x;

				result.m01 *= scaling.y;
				result.m11 *= scaling.y;
				result.m21 *= scaling.y;

				result.m02 *= scaling.z;
				result.m12 *= scaling.z;
				result.m22 *= scaling.z;

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: scaling, rotation, translation.
			/// </summary>
			public static void CreateSRT(ref Vector3D scaling, ref Quaternion rotation, ref Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m00 *= scaling.x;
				result.m10 *= scaling.x;
				result.m20 *= scaling.x;

				result.m01 *= scaling.y;
				result.m11 *= scaling.y;
				result.m21 *= scaling.y;

				result.m02 *= scaling.z;
				result.m12 *= scaling.z;
				result.m22 *= scaling.z;

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: uniform scaling, rotation, translation.
			/// </summary>
			public static void CreateSRT(double scaling, Quaternion rotation, Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m00 *= scaling;
				result.m10 *= scaling;
				result.m20 *= scaling;

				result.m01 *= scaling;
				result.m11 *= scaling;
				result.m21 *= scaling;

				result.m02 *= scaling;
				result.m12 *= scaling;
				result.m22 *= scaling;

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;
			}

			/// <summary>
			/// Creates transformation matrix. Transformation order is: uniform scaling, rotation, translation.
			/// </summary>
			public static void CreateSRT(double scaling, ref Quaternion rotation, ref Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m00 *= scaling;
				result.m10 *= scaling;
				result.m20 *= scaling;

				result.m01 *= scaling;
				result.m11 *= scaling;
				result.m21 *= scaling;

				result.m02 *= scaling;
				result.m12 *= scaling;
				result.m22 *= scaling;

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: scaling, moving to rotation origin, rotation, moving to translation point.
			/// </summary>
			public static void CreateSRT(Vector3D scaling, Vector3D rotationOrigin, Quaternion rotation, Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x) + translation.x;
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y) + translation.y;
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z) + translation.z;

				result.m00 *= scaling.x;
				result.m10 *= scaling.x;
				result.m20 *= scaling.x;

				result.m01 *= scaling.y;
				result.m11 *= scaling.y;
				result.m21 *= scaling.y;

				result.m02 *= scaling.z;
				result.m12 *= scaling.z;
				result.m22 *= scaling.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: scaling, moving to rotation origin, rotation, moving to translation point.
			/// </summary>
			public static void CreateSRT(ref Vector3D scaling, ref Vector3D rotationOrigin, ref Quaternion rotation, ref Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x) + translation.x;
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y) + translation.y;
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z) + translation.z;

				result.m00 *= scaling.x;
				result.m10 *= scaling.x;
				result.m20 *= scaling.x;

				result.m01 *= scaling.y;
				result.m11 *= scaling.y;
				result.m21 *= scaling.y;

				result.m02 *= scaling.z;
				result.m12 *= scaling.z;
				result.m22 *= scaling.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: uniform scaling, moving to rotation origin, rotation, moving to translation point.
			/// </summary>
			public static void CreateSRT(double scaling, Vector3D rotationOrigin, Quaternion rotation, Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x) + translation.x;
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y) + translation.y;
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z) + translation.z;

				result.m00 *= scaling;
				result.m10 *= scaling;
				result.m20 *= scaling;

				result.m01 *= scaling;
				result.m11 *= scaling;
				result.m21 *= scaling;

				result.m02 *= scaling;
				result.m12 *= scaling;
				result.m22 *= scaling;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: uniform scaling, moving to rotation origin, rotation, moving to translation point.
			/// </summary>
			public static void CreateSRT(double scaling, ref Vector3D rotationOrigin, ref Quaternion rotation, ref Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x) + translation.x;
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y) + translation.y;
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z) + translation.z;

				result.m00 *= scaling;
				result.m10 *= scaling;
				result.m20 *= scaling;

				result.m01 *= scaling;
				result.m11 *= scaling;
				result.m21 *= scaling;

				result.m02 *= scaling;
				result.m12 *= scaling;
				result.m22 *= scaling;
			}
			
			/// <summary>
			/// Creates a transformation matrix. Transformation order is: rotation, translation.
			/// </summary>
			public static void CreateRT(Quaternion rotation, Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: rotation, translation.
			/// </summary>
			public static void CreateRT(ref Quaternion rotation, ref Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: moving to rotation origin, rotation, moving to translation point.
			/// </summary>
			public static void CreateRT(Vector3D rotationOrigin, Quaternion rotation, Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x) + translation.x;
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y) + translation.y;
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z) + translation.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation order is: moving to rotation origin, rotation, moving to translation point.
			/// </summary>
			public static void CreateRT(ref Vector3D rotationOrigin, ref Quaternion rotation, ref Vector3D translation, out Matrix4X4 result)
			{
				QuaternionToRotationMatrix(ref rotation, out result);

				result.m03 = -(result.m00 * rotationOrigin.x + result.m01 * rotationOrigin.y + result.m02 * rotationOrigin.z - rotationOrigin.x) + translation.x;
				result.m13 = -(result.m10 * rotationOrigin.x + result.m11 * rotationOrigin.y + result.m12 * rotationOrigin.z - rotationOrigin.y) + translation.y;
				result.m23 = -(result.m20 * rotationOrigin.x + result.m21 * rotationOrigin.y + result.m22 * rotationOrigin.z - rotationOrigin.z) + translation.z;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation includes scaling and translation (order is unimportant).
			/// </summary>
			public static void CreateST(Vector3D scaling, Vector3D translation, out Matrix4X4 result)
			{
				result.m00 = scaling.x;
				result.m11 = scaling.y;
				result.m22 = scaling.z;

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;

				result.m01 = 0.0f;
				result.m02 = 0.0f;
				result.m10 = 0.0f;
				result.m12 = 0.0f;
				result.m20 = 0.0f;
				result.m21 = 0.0f;
				result.m30 = 0.0f;
				result.m31 = 0.0f;
				result.m32 = 0.0f;

				result.m33 = 1.0f;
			}

			/// <summary>
			/// Creates a transformation matrix. Transformation includes scaling and translation (order is unimportant).
			/// </summary>
			public static void CreateST(ref Vector3D scaling, ref Vector3D translation, out Matrix4X4 result)
			{
				result.m00 = scaling.x;
				result.m11 = scaling.y;
				result.m22 = scaling.z;

				result.m03 = translation.x;
				result.m13 = translation.y;
				result.m23 = translation.z;

				result.m01 = 0.0f;
				result.m02 = 0.0f;
				result.m10 = 0.0f;
				result.m12 = 0.0f;
				result.m20 = 0.0f;
				result.m21 = 0.0f;
				result.m30 = 0.0f;
				result.m31 = 0.0f;
				result.m32 = 0.0f;

				result.m33 = 1.0f;
			}

			/// <summary>
			/// Creates rotation matrix from 3 vectors (vectors are columns of the matrix)
			/// </summary>
			public static void CreateRotationFromColumns(Vector3D column0, Vector3D column1, Vector3D column2, out Matrix4X4 matrix)
			{
				matrix = Matrix4X4.identity;
				matrix.SetColumn(0, column0);
				matrix.SetColumn(1, column1);
				matrix.SetColumn(2, column2);
			}

			/// <summary>
			/// Creates rotation matrix from 3 vectors (vectors are columns of the matrix)
			/// </summary>
			public static void CreateRotationFromColumns(ref Vector3D column0, ref Vector3D column1, ref Vector3D column2, out Matrix4X4 matrix)
			{
				matrix = Matrix4X4.identity;
				matrix.SetColumn(0, column0);
				matrix.SetColumn(1, column1);
				matrix.SetColumn(2, column2);
			}


			/// <summary>
			/// Creates directional light shadow matrix that flattens geometry into a plane.
			/// </summary>
			/// <param name="shadowPlane">Projection plane</param>
			/// <param name="dirLightOppositeDirection">Light source is a directional light and parameter contains
			/// opposite direction of directional light (e.g. if light direction is L, caller must pass -L as a parameter)</param>
			public static void CreateShadowDirectional(Plane3 shadowPlane, Vector3D dirLightOppositeDirection, out Matrix4X4 result)
			{
				Vector3D N     = shadowPlane.Normal;
				double   d     = shadowPlane.Constant;
				double   NdotL = N.x * dirLightOppositeDirection.x + N.y * dirLightOppositeDirection.y + N.z * dirLightOppositeDirection.z;

				result.m00 = NdotL - dirLightOppositeDirection.x * N.x;
				result.m01 = -dirLightOppositeDirection.x * N.y;
				result.m02 = -dirLightOppositeDirection.x * N.z;
				result.m03 = dirLightOppositeDirection.x * d;

				result.m10 = -dirLightOppositeDirection.y * N.x;
				result.m11 = NdotL - dirLightOppositeDirection.y * N.y;
				result.m12 = -dirLightOppositeDirection.y * N.z;
				result.m13 = dirLightOppositeDirection.y * d;

				result.m20 = -dirLightOppositeDirection.z * N.x;
				result.m21 = -dirLightOppositeDirection.z * N.y;
				result.m22 = NdotL - dirLightOppositeDirection.z * N.z;
				result.m23 = dirLightOppositeDirection.z * d;

				result.m33 = NdotL;

				result.m30 = 0.0f;
				result.m31 = 0.0f;
				result.m32 = 0.0f;
			}

			/// <summary>
			/// Creates directional light shadow matrix that flattens geometry into a plane.
			/// </summary>
			/// <param name="shadowPlane">Projection plane</param>
			/// <param name="dirLightOppositeDirection">Light source is a directional light and parameter contains
			/// opposite direction of directional light (e.g. if light direction is L, caller must pass -L as a parameter)</param>
			public static void CreateShadowDirectional(ref Plane3 shadowPlane, ref Vector3D dirLightOppositeDirection, out Matrix4X4 result)
			{
				Vector3D N = shadowPlane.Normal;
				double d = shadowPlane.Constant;
				double NdotL = N.x * dirLightOppositeDirection.x + N.y * dirLightOppositeDirection.y + N.z * dirLightOppositeDirection.z;

				result.m00 = NdotL - dirLightOppositeDirection.x * N.x;
				result.m01 = -dirLightOppositeDirection.x * N.y;
				result.m02 = -dirLightOppositeDirection.x * N.z;
				result.m03 = dirLightOppositeDirection.x * d;

				result.m10 = -dirLightOppositeDirection.y * N.x;
				result.m11 = NdotL - dirLightOppositeDirection.y * N.y;
				result.m12 = -dirLightOppositeDirection.y * N.z;
				result.m13 = dirLightOppositeDirection.y * d;

				result.m20 = -dirLightOppositeDirection.z * N.x;
				result.m21 = -dirLightOppositeDirection.z * N.y;
				result.m22 = NdotL - dirLightOppositeDirection.z * N.z;
				result.m23 = dirLightOppositeDirection.z * d;

				result.m33 = NdotL;

				result.m30 = 0.0f;
				result.m31 = 0.0f;
				result.m32 = 0.0f;
			}

			/// <summary>
			/// Creates point light shadow matrix that flattens geometry into a plane.
			/// </summary>
			/// <param name="shadowPlane">Projection plane</param>
			/// <param name="pointLightPosition">Light source is a point light and parameter contains
			/// position of a point light</param>
			public static void CreateShadowPoint(Plane3 shadowPlane, Vector3D pointLightPosition, out Matrix4X4 result)
			{
				Vector3D N     = shadowPlane.Normal;
				double   d     = shadowPlane.Constant;
				double   NdotL = N.x * pointLightPosition.x + N.y * pointLightPosition.y + N.z * pointLightPosition.z;

				result.m00 = NdotL + pointLightPosition.x * N.x - d;
				result.m01 = -pointLightPosition.x * N.y;
				result.m02 = -pointLightPosition.x * N.z;
				result.m03 = pointLightPosition.x * d;

				result.m10 = -pointLightPosition.y * N.x;
				result.m11 = NdotL - pointLightPosition.y * N.y - d;
				result.m12 = -pointLightPosition.y * N.z;
				result.m13 = pointLightPosition.y * d;

				result.m20 = -pointLightPosition.z * N.x;
				result.m21 = -pointLightPosition.z * N.y;
				result.m22 = NdotL - pointLightPosition.z * N.z - d;
				result.m23 = pointLightPosition.z * d;

				result.m30 = -N.x;
				result.m31 = -N.y;
				result.m32 = -N.z;

				result.m33 = NdotL;
			}

			/// <summary>
			/// Creates point light shadow matrix that flattens geometry into a plane.
			/// </summary>
			/// <param name="shadowPlane">Projection plane</param>
			/// <param name="pointLightPosition">Light source is a point light and parameter contains
			/// position of a point light</param>
			public static void CreateShadowPoint(ref Plane3 shadowPlane, ref Vector3D pointLightPosition, out Matrix4X4 result)
			{
				Vector3D N = shadowPlane.Normal;
				double d = shadowPlane.Constant;
				double NdotL = N.x * pointLightPosition.x + N.y * pointLightPosition.y + N.z * pointLightPosition.z;

				result.m00 = NdotL + pointLightPosition.x * N.x - d;
				result.m01 = -pointLightPosition.x * N.y;
				result.m02 = -pointLightPosition.x * N.z;
				result.m03 = pointLightPosition.x * d;

				result.m10 = -pointLightPosition.y * N.x;
				result.m11 = NdotL - pointLightPosition.y * N.y - d;
				result.m12 = -pointLightPosition.y * N.z;
				result.m13 = pointLightPosition.y * d;

				result.m20 = -pointLightPosition.z * N.x;
				result.m21 = -pointLightPosition.z * N.y;
				result.m22 = NdotL - pointLightPosition.z * N.z - d;
				result.m23 = pointLightPosition.z * d;

				result.m30 = -N.x;
				result.m31 = -N.y;
				result.m32 = -N.z;

				result.m33 = NdotL;
			}

			/// <summary>
			/// Creates a generic shadow matrix that flattens geometry into a plane.
			/// </summary>
			/// <param name="shadowPlane">Projection plane</param>
			/// <param name="lightData">If w component is 0.0f, then light source is directional light and
			/// x,y,z components contain opposite direction of directional light. If w component is 1.0f
			/// then source is point light and x,y,z components contain position of point light.</param>
			public static void CreateShadow(Plane3 shadowPlane, Vector4D lightData, out Matrix4X4 result)
			{
				Vector3D N     = shadowPlane.Normal;
				double   d     = shadowPlane.Constant;
				double   NdotL = N.x * lightData.x + N.y * lightData.y + N.z * lightData.z;

				result.m00 = NdotL + lightData.x * N.x - d * lightData.w;
				result.m01 = -lightData.x * N.y;
				result.m02 = -lightData.x * N.z;
				result.m03 = lightData.x * d;

				result.m10 = -lightData.y * N.x;
				result.m11 = NdotL - lightData.y * N.y - d * lightData.w;
				result.m12 = -lightData.y * N.z;
				result.m13 = lightData.y * d;

				result.m20 = -lightData.z * N.x;
				result.m21 = -lightData.z * N.y;
				result.m22 = NdotL - lightData.z * N.z - d * lightData.w;
				result.m23 = lightData.z * d;

				result.m30 = -N.x * lightData.w;
				result.m31 = -N.y * lightData.w;
				result.m32 = -N.z * lightData.w;

				result.m33 = NdotL;
			}

			/// <summary>
			/// Creates a generic shadow matrix that flattens geometry into a plane.
			/// </summary>
			/// <param name="shadowPlane">Projection plane</param>
			/// <param name="lightData">If w component is 0.0f, then light source is directional light and
			/// x,y,z components contain opposite direction of directional light. If w component is 1.0f
			/// then source is point light and x,y,z components contain position of point light.</param>
			public static void CreateShadow(ref Plane3 shadowPlane, ref Vector4D lightData, out Matrix4X4 result)
			{
				Vector3D N = shadowPlane.Normal;
				double d = shadowPlane.Constant;
				double NdotL = N.x * lightData.x + N.y * lightData.y + N.z * lightData.z;

				result.m00 = NdotL + lightData.x * N.x - d * lightData.w;
				result.m01 = -lightData.x * N.y;
				result.m02 = -lightData.x * N.z;
				result.m03 = lightData.x * d;

				result.m10 = -lightData.y * N.x;
				result.m11 = NdotL - lightData.y * N.y - d * lightData.w;
				result.m12 = -lightData.y * N.z;
				result.m13 = lightData.y * d;

				result.m20 = -lightData.z * N.x;
				result.m21 = -lightData.z * N.y;
				result.m22 = NdotL - lightData.z * N.z - d * lightData.w;
				result.m23 = lightData.z * d;

				result.m30 = -N.x * lightData.w;
				result.m31 = -N.y * lightData.w;
				result.m32 = -N.z * lightData.w;

				result.m33 = NdotL;
			}
		}
	}
}
