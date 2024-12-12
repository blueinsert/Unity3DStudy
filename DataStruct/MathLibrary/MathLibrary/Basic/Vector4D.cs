using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dest
{
    namespace Math
    {
        [Serializable]
        public struct Vector4D
        {
            public const double kEpsilon = 1E-05f;
            public double x;
            public double y;
            public double z;
            public double w;

            public double this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return this.x;
                        case 1:
                            return this.y;
                        case 2:
                            return this.z;
                        case 3:
                            return this.w;
                        default:
                            throw new IndexOutOfRangeException("Invalid Vector4D index!");
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            this.x = value;
                            break;
                        case 1:
                            this.y = value;
                            break;
                        case 2:
                            this.z = value;
                            break;
                        case 3:
                            this.w = value;
                            break;
                        default:
                            throw new IndexOutOfRangeException("Invalid Vector4D index!");
                    }
                }
            }

            public Vector4D normalized
            {
                get
                {
                    return Vector4D.Normalize(this);
                }
            }

            public double magnitude
            {
                get
                {
                    return System.Math.Sqrt(Vector4D.Dot(this, this));
                }
            }

            public double sqrMagnitude
            {
                get
                {
                    return Vector4D.Dot(this, this);
                }
            }

            public static Vector4D zero
            {
                get
                {
                    return new Vector4D(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }

            public static Vector4D one
            {
                get
                {
                    return new Vector4D(1f, 1f, 1f, 1f);
                }
            }

            public Vector4D(double x, double y, double z, double w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }

            public Vector4D(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = 0.0f;
            }

            public Vector4D(double x, double y)
            {
                this.x = x;
                this.y = y;
                this.z = 0.0f;
                this.w = 0.0f;
            }

            public static implicit operator Vector4D(Vector3D v)
            {
                return new Vector4D(v.x, v.y, v.z, 0.0f);
            }

            public static implicit operator Vector3D(Vector4D v)
            {
                return new Vector3D(v.x, v.y, v.z);
            }

            public static implicit operator Vector4D(Vector2D v)
            {
                return new Vector4D(v.x, v.y, 0.0f, 0.0f);
            }

            public static implicit operator Vector2D(Vector4D v)
            {
                return new Vector2D(v.x, v.y);
            }

            public static Vector4D operator +(Vector4D a, Vector4D b)
            {
                return new Vector4D(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
            }

            public static Vector4D operator -(Vector4D a, Vector4D b)
            {
                return new Vector4D(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
            }

            public static Vector4D operator -(Vector4D a)
            {
                return new Vector4D(-a.x, -a.y, -a.z, -a.w);
            }

            public static Vector4D operator *(Vector4D a, double d)
            {
                return new Vector4D(a.x * d, a.y * d, a.z * d, a.w * d);
            }

            public static Vector4D operator *(double d, Vector4D a)
            {
                return new Vector4D(a.x * d, a.y * d, a.z * d, a.w * d);
            }

            public static Vector4D operator /(Vector4D a, double d)
            {
                return new Vector4D(a.x / d, a.y / d, a.z / d, a.w / d);
            }

            public static bool operator ==(Vector4D lhs, Vector4D rhs)
            {
                return Vector4D.SqrMagnitude(lhs - rhs) < kEpsilon;
            }

            public static bool operator !=(Vector4D lhs, Vector4D rhs)
            {
                return Vector4D.SqrMagnitude(lhs - rhs) >= kEpsilon;
            }

            public void Set(double new_x, double new_y, double new_z, double new_w)
            {
                this.x = new_x;
                this.y = new_y;
                this.z = new_z;
                this.w = new_w;
            }

            public static Vector4D Lerp(Vector4D from, Vector4D to, double t)
            {
                if (t < 0.0) t = 0.0; else if (t > 1.0) t = 1.0;
                return new Vector4D(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t, from.w + (to.w - from.w) * t);
            }

            public static Vector4D MoveTowards(Vector4D current, Vector4D target, double maxDistanceDelta)
            {
                Vector4D vector4 = target - current;
                double magnitude = vector4.magnitude;
                if (magnitude <= maxDistanceDelta || magnitude == 0.0)
                    return target;
                else
                    return current + vector4 / magnitude * maxDistanceDelta;
            }

            public static Vector4D Scale(Vector4D a, Vector4D b)
            {
                return new Vector4D(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
            }

            public void Scale(Vector4D scale)
            {
                this.x *= scale.x;
                this.y *= scale.y;
                this.z *= scale.z;
                this.w *= scale.w;
            }

            public override int GetHashCode()
            {
                return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
            }

            public override bool Equals(object other)
            {
                if (!(other is Vector4D))
                    return false;
                Vector4D vector4 = (Vector4D)other;
                if (this.x.Equals(vector4.x) && this.y.Equals(vector4.y) && this.z.Equals(vector4.z))
                    return this.w.Equals(vector4.w);
                else
                    return false;
            }

            public static Vector4D Normalize(Vector4D a)
            {
                double num = Vector4D.Magnitude(a);
                if (num > 9.99999974737875E-06)
                    return a / num;
                else
                    return Vector4D.zero;
            }

            public void Normalize()
            {
                double num = Vector4D.Magnitude(this);
                if (num > 9.99999974737875E-06)
                    this = this / num;
                else
                    this = Vector4D.zero;
            }

            public override string ToString()
            {
                string fmt = "F1";
                return ToString(fmt);
            }

            public string ToString(string format)
            {
                string fmt = "({0}, {1}, {2}, {3})";
                object[] objArray = new object[4];
                int index1 = 0;
                string str1 = this.x.ToString(format);
                objArray[index1] = (object)str1;
                int index2 = 1;
                string str2 = this.y.ToString(format);
                objArray[index2] = (object)str2;
                int index3 = 2;
                string str3 = this.z.ToString(format);
                objArray[index3] = (object)str3;
                int index4 = 3;
                string str4 = this.w.ToString(format);
                objArray[index4] = (object)str4;
                return String.Format(fmt, objArray);
            }

            public static double Dot(Vector4D a, Vector4D b)
            {
                return (a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w);
            }

            public static Vector4D Project(Vector4D a, Vector4D b)
            {
                return b * Vector4D.Dot(a, b) / Vector4D.Dot(b, b);
            }

            public static double Distance(Vector4D a, Vector4D b)
            {
                return Vector4D.Magnitude(a - b);
            }

            public static double Magnitude(Vector4D a)
            {
                return System.Math.Sqrt(Vector4D.Dot(a, a));
            }

            public static double SqrMagnitude(Vector4D a)
            {
                return Vector4D.Dot(a, a);
            }

            public double SqrMagnitude()
            {
                return Vector4D.Dot(this, this);
            }

            public static Vector4D Min(Vector4D lhs, Vector4D rhs)
            {
                return new Vector4D(System.Math.Min(lhs.x, rhs.x), System.Math.Min(lhs.y, rhs.y), System.Math.Min(lhs.z, rhs.z), System.Math.Min(lhs.w, rhs.w));
            }

            public static Vector4D Max(Vector4D lhs, Vector4D rhs)
            {
                return new Vector4D(System.Math.Max(lhs.x, rhs.x), System.Math.Max(lhs.y, rhs.y), System.Math.Max(lhs.z, rhs.z), System.Math.Max(lhs.w, rhs.w));
            }
        }
    }
}
