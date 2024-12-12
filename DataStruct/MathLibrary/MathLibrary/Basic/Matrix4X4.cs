using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dest
{
    namespace Math
    {
        public struct Matrix4X4
        {
            public double m00;
            public double m10;
            public double m20;
            public double m30;
            public double m01;
            public double m11;
            public double m21;
            public double m31;
            public double m02;
            public double m12;
            public double m22;
            public double m32;
            public double m03;
            public double m13;
            public double m23;
            public double m33;

            public double this[int row, int column]
            {
                get
                {
                    return this[row + column * 4];
                }
                set
                {
                    this[row + column * 4] = value;
                }
            }

            public double this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return this.m00;
                        case 1:
                            return this.m10;
                        case 2:
                            return this.m20;
                        case 3:
                            return this.m30;
                        case 4:
                            return this.m01;
                        case 5:
                            return this.m11;
                        case 6:
                            return this.m21;
                        case 7:
                            return this.m31;
                        case 8:
                            return this.m02;
                        case 9:
                            return this.m12;
                        case 10:
                            return this.m22;
                        case 11:
                            return this.m32;
                        case 12:
                            return this.m03;
                        case 13:
                            return this.m13;
                        case 14:
                            return this.m23;
                        case 15:
                            return this.m33;
                        default:
                            throw new IndexOutOfRangeException("Invalid matrix index!");
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            this.m00 = value;
                            break;
                        case 1:
                            this.m10 = value;
                            break;
                        case 2:
                            this.m20 = value;
                            break;
                        case 3:
                            this.m30 = value;
                            break;
                        case 4:
                            this.m01 = value;
                            break;
                        case 5:
                            this.m11 = value;
                            break;
                        case 6:
                            this.m21 = value;
                            break;
                        case 7:
                            this.m31 = value;
                            break;
                        case 8:
                            this.m02 = value;
                            break;
                        case 9:
                            this.m12 = value;
                            break;
                        case 10:
                            this.m22 = value;
                            break;
                        case 11:
                            this.m32 = value;
                            break;
                        case 12:
                            this.m03 = value;
                            break;
                        case 13:
                            this.m13 = value;
                            break;
                        case 14:
                            this.m23 = value;
                            break;
                        case 15:
                            this.m33 = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException("Invalid matrix index!");
                    }
                }
            }

            public Matrix4X4 inverse
            {
                get
                {
                    return Matrix4X4.Inverse(this);
                }
            }

            public Matrix4X4 transpose
            {
                get
                {
                    return Matrix4X4.Transpose(this);
                }
            }

            public bool isIdentity 
            { 
                get
                {                
                    return  m00 == 1 &&
                            m01 == 0 &&
                            m02 == 0 &&
                            m03 == 0 &&
                            m10 == 0 &&
                            m11 == 1 &&
                            m12 == 0 &&
                            m13 == 0 &&
                            m20 == 0 &&
                            m21 == 0 &&
                            m22 == 1 &&
                            m23 == 0 &&
                            m30 == 0 &&
                            m31 == 0 &&
                            m32 == 0 &&
                            m33 == 1;
                }
            }

            public static Matrix4X4 zero
            {
                get
                {
                    return new Matrix4X4()
                    {
                        m00 = 0.0f,
                        m01 = 0.0f,
                        m02 = 0.0f,
                        m03 = 0.0f,
                        m10 = 0.0f,
                        m11 = 0.0f,
                        m12 = 0.0f,
                        m13 = 0.0f,
                        m20 = 0.0f,
                        m21 = 0.0f,
                        m22 = 0.0f,
                        m23 = 0.0f,
                        m30 = 0.0f,
                        m31 = 0.0f,
                        m32 = 0.0f,
                        m33 = 0.0f
                    };
                }
            }

            public static Matrix4X4 identity
            {
                get
                {
                    return new Matrix4X4()
                    {
                        m00 = 1f,
                        m01 = 0.0f,
                        m02 = 0.0f,
                        m03 = 0.0f,
                        m10 = 0.0f,
                        m11 = 1f,
                        m12 = 0.0f,
                        m13 = 0.0f,
                        m20 = 0.0f,
                        m21 = 0.0f,
                        m22 = 1f,
                        m23 = 0.0f,
                        m30 = 0.0f,
                        m31 = 0.0f,
                        m32 = 0.0f,
                        m33 = 1f
                    };
                }
            }

            public static Matrix4X4 operator *(Matrix4X4 lhs, Matrix4X4 rhs)
            {
                return new Matrix4X4()
                {
                    m00 = (lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30),
                    m01 = (lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31),
                    m02 = (lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32),
                    m03 = (lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33),
                    m10 = (lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30),
                    m11 = (lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31),
                    m12 = (lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32),
                    m13 = (lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33),
                    m20 = (lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30),
                    m21 = (lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31),
                    m22 = (lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32),
                    m23 = (lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33),
                    m30 = (lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30),
                    m31 = (lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31),
                    m32 = (lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32),
                    m33 = (lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33)
                };
            }

            public static Vector4D operator *(Matrix4X4 lhs, Vector4D v)
            {
                Vector4D vector4;
                vector4.x = (lhs.m00 * v.x + lhs.m01 * v.y + lhs.m02 * v.z + lhs.m03 * v.w);
                vector4.y = (lhs.m10 * v.x + lhs.m11 * v.y + lhs.m12 * v.z + lhs.m13 * v.w);
                vector4.z = (lhs.m20 * v.x + lhs.m21 * v.y + lhs.m22 * v.z + lhs.m23 * v.w);
                vector4.w = (lhs.m30 * v.x + lhs.m31 * v.y + lhs.m32 * v.z + lhs.m33 * v.w);
                return vector4;
            }

            public static bool operator ==(Matrix4X4 lhs, Matrix4X4 rhs)
            {
                if (lhs.GetColumn(0) == rhs.GetColumn(0) && lhs.GetColumn(1) == rhs.GetColumn(1) && lhs.GetColumn(2) == rhs.GetColumn(2))
                    return lhs.GetColumn(3) == rhs.GetColumn(3);
                else
                    return false;
            }

            public static bool operator !=(Matrix4X4 lhs, Matrix4X4 rhs)
            {
                return !(lhs == rhs);
            }

            public override int GetHashCode()
            {
                return this.GetColumn(0).GetHashCode() ^ this.GetColumn(1).GetHashCode() << 2 ^ this.GetColumn(2).GetHashCode() >> 2 ^ this.GetColumn(3).GetHashCode() >> 1;
            }

            public override bool Equals(object other)
            {
                if (!(other is Matrix4X4))
                    return false;
                Matrix4X4 matrix4x4 = (Matrix4X4)other;
                if (this.GetColumn(0).Equals((object)matrix4x4.GetColumn(0)) && this.GetColumn(1).Equals((object)matrix4x4.GetColumn(1)) && this.GetColumn(2).Equals((object)matrix4x4.GetColumn(2)))
                    return this.GetColumn(3).Equals((object)matrix4x4.GetColumn(3));
                else
                    return false;
            }

            public static Matrix4X4 Inverse(Matrix4X4 m)
            {
                return Matrix4X4.INTERNAL_CALL_Inverse(ref m);
            }

            private static Matrix4X4 INTERNAL_CALL_Inverse(ref Matrix4X4 m)
            {
                Matrix4X4 mi = new Matrix4X4();
                Matrix4X4ex.Inverse(ref m, out mi);
                return mi;
            }

            public static Matrix4X4 Transpose(Matrix4X4 m)
            {
                return Matrix4X4.INTERNAL_CALL_Transpose(ref m);
            }

            private static Matrix4X4 INTERNAL_CALL_Transpose(ref Matrix4X4 m)
            {
                return new Matrix4X4()
                {
                    m00 = m.m00,
                    m01 = m.m10,
                    m02 = m.m20,
                    m03 = m.m30,
                    m10 = m.m01,
                    m11 = m.m11,
                    m12 = m.m21,
                    m13 = m.m31,
                    m20 = m.m02,
                    m21 = m.m12,
                    m22 = m.m22,
                    m23 = m.m32,
                    m30 = m.m03,
                    m31 = m.m13,
                    m32 = m.m23,
                    m33 = m.m33
                };
            }

            public Vector4D GetColumn(int i)
            {
                return new Vector4D(this[0, i], this[1, i], this[2, i], this[3, i]);
            }

            public Vector4D GetRow(int i)
            {
                return new Vector4D(this[i, 0], this[i, 1], this[i, 2], this[i, 3]);
            }

            public void SetColumn(int i, Vector4D v)
            {
                this[0, i] = v.x;
                this[1, i] = v.y;
                this[2, i] = v.z;
                this[3, i] = v.w;
            }

            public void SetRow(int i, Vector4D v)
            {
                this[i, 0] = v.x;
                this[i, 1] = v.y;
                this[i, 2] = v.z;
                this[i, 3] = v.w;
            }

            public Vector3D MultiplyPoint(Vector3D v)
            {
                Vector3D vector3;
                vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z) + this.m03;
                vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z) + this.m13;
                vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z) + this.m23;
                double num = 1f / ((this.m30 * v.x + this.m31 * v.y + this.m32 * v.z) + this.m33);
                vector3.x *= num;
                vector3.y *= num;
                vector3.z *= num;
                return vector3;
            }

            public Vector3D MultiplyPoint3x4(Vector3D v)
            {
                Vector3D vector3;
                vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z) + this.m03;
                vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z) + this.m13;
                vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z) + this.m23;
                return vector3;
            }

            public Vector3D MultiplyVector(Vector3D v)
            {
                Vector3D vector3;
                vector3.x = (this.m00 * v.x + this.m01 * v.y + this.m02 * v.z);
                vector3.y = (this.m10 * v.x + this.m11 * v.y + this.m12 * v.z);
                vector3.z = (this.m20 * v.x + this.m21 * v.y + this.m22 * v.z);
                return vector3;
            }

            public static Matrix4X4 Scale(Vector3D v)
            {
                return new Matrix4X4()
                {
                    m00 = v.x,
                    m01 = 0.0f,
                    m02 = 0.0f,
                    m03 = 0.0f,
                    m10 = 0.0f,
                    m11 = v.y,
                    m12 = 0.0f,
                    m13 = 0.0f,
                    m20 = 0.0f,
                    m21 = 0.0f,
                    m22 = v.z,
                    m23 = 0.0f,
                    m30 = 0.0f,
                    m31 = 0.0f,
                    m32 = 0.0f,
                    m33 = 1f
                };
            }

            public override string ToString()
            {
                string fmt = "F5";
                return ToString(fmt);
            }

            public string ToString(string format)
            {
                string fmt = "{0}\t{1}\t{2}\t{3}\n{4}\t{5}\t{6}\t{7}\n{8}\t{9}\t{10}\t{11}\n{12}\t{13}\t{14}\t{15}\n";
                object[] objArray = new object[16];
                int index1 = 0;
                string str1 = this.m00.ToString(format);
                objArray[index1] = (object)str1;
                int index2 = 1;
                string str2 = this.m01.ToString(format);
                objArray[index2] = (object)str2;
                int index3 = 2;
                string str3 = this.m02.ToString(format);
                objArray[index3] = (object)str3;
                int index4 = 3;
                string str4 = this.m03.ToString(format);
                objArray[index4] = (object)str4;
                int index5 = 4;
                string str5 = this.m10.ToString(format);
                objArray[index5] = (object)str5;
                int index6 = 5;
                string str6 = this.m11.ToString(format);
                objArray[index6] = (object)str6;
                int index7 = 6;
                string str7 = this.m12.ToString(format);
                objArray[index7] = (object)str7;
                int index8 = 7;
                string str8 = this.m13.ToString(format);
                objArray[index8] = (object)str8;
                int index9 = 8;
                string str9 = this.m20.ToString(format);
                objArray[index9] = (object)str9;
                int index10 = 9;
                string str10 = this.m21.ToString(format);
                objArray[index10] = (object)str10;
                int index11 = 10;
                string str11 = this.m22.ToString(format);
                objArray[index11] = (object)str11;
                int index12 = 11;
                string str12 = this.m23.ToString(format);
                objArray[index12] = (object)str12;
                int index13 = 12;
                string str13 = this.m30.ToString(format);
                objArray[index13] = (object)str13;
                int index14 = 13;
                string str14 = this.m31.ToString(format);
                objArray[index14] = (object)str14;
                int index15 = 14;
                string str15 = this.m32.ToString(format);
                objArray[index15] = (object)str15;
                int index16 = 15;
                string str16 = this.m33.ToString(format);
                objArray[index16] = (object)str16;
                return String.Format(fmt, objArray);
            }

        }
    }
}
