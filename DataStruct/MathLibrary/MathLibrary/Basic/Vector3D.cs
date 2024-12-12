using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dest
{
    namespace Math
    {
        [Serializable]
        public struct Vector3D
        {
            public const double kEpsilon = 1E-05;
            public double x;
            public double y;
            public double z;

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
                        default:
                            throw new IndexOutOfRangeException("Invalid Vector3D index!");
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
                        default:
                            throw new IndexOutOfRangeException("Invalid Vector3D index!");
                    }
                }
            }

            public Vector3D normalized
            {
                get
                {
                    return Vector3D.Normalize(this);
                }
            }

            public double magnitude
            {
                get
                {
                    return System.Math.Sqrt((this.x * this.x + this.y * this.y + this.z * this.z));
                }
            }

            public double sqrMagnitude
            {
                get
                {
                    return (this.x * this.x + this.y * this.y + this.z * this.z);
                }
            }

            public static Vector3D zero
            {
                get
                {
                    return new Vector3D(0.0, 0.0, 0.0);
                }
            }

            public static Vector3D one
            {
                get
                {
                    return new Vector3D(1, 1, 1);
                }
            }

            public static Vector3D forward
            {
                get
                {
                    return new Vector3D(0.0, 0.0, 1);
                }
            }

            public static Vector3D back
            {
                get
                {
                    return new Vector3D(0.0, 0.0, -1);
                }
            }

            public static Vector3D up
            {
                get
                {
                    return new Vector3D(0.0, 1, 0.0);
                }
            }

            public static Vector3D down
            {
                get
                {
                    return new Vector3D(0.0, -1, 0.0);
                }
            }

            public static Vector3D left
            {
                get
                {
                    return new Vector3D(-1, 0.0, 0.0);
                }
            }

            public static Vector3D right
            {
                get
                {
                    return new Vector3D(1, 0.0, 0.0);
                }
            }

            public Vector3D(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public Vector3D(double x, double y)
            {
                this.x = x;
                this.y = y;
                this.z = 0.0;
            }

            public static Vector3D operator +(Vector3D a, Vector3D b)
            {
                return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
            }

            public static Vector3D operator -(Vector3D a, Vector3D b)
            {
                return new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
            }

            public static Vector3D operator -(Vector3D a)
            {
                return new Vector3D(-a.x, -a.y, -a.z);
            }

            public static Vector3D operator *(Vector3D a, double d)
            {
                return new Vector3D(a.x * d, a.y * d, a.z * d);
            }

            public static Vector3D operator *(double d, Vector3D a)
            {
                return new Vector3D(a.x * d, a.y * d, a.z * d);
            }

            public static Vector3D operator /(Vector3D a, double d)
            {
                return new Vector3D(a.x / d, a.y / d, a.z / d);
            }

            public static bool operator ==(Vector3D lhs, Vector3D rhs)
            {
                return Vector3D.SqrMagnitude(lhs - rhs) < kEpsilon;
            }

            public static bool operator !=(Vector3D lhs, Vector3D rhs)
            {
                return Vector3D.SqrMagnitude(lhs - rhs) >= kEpsilon;
            }

            public static Vector3D Lerp(Vector3D from, Vector3D to, double t)
            {
                if (t < 0.0) t = 0.0; else if (t > 1.0) t = 1.0;
                return new Vector3D(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
            }

            public static Vector3D MoveTowards(Vector3D current, Vector3D target, double maxDistanceDelta)
            {
                Vector3D vector3 = target - current;
                double magnitude = vector3.magnitude;
                if (magnitude <= maxDistanceDelta || magnitude == 0.0)
                    return target;
                else
                    return current + vector3 / magnitude * maxDistanceDelta;
            }

            public void Set(double new_x, double new_y, double new_z)
            {
                this.x = new_x;
                this.y = new_y;
                this.z = new_z;
            }

            public static Vector3D Scale(Vector3D a, Vector3D b)
            {
                return new Vector3D(a.x * b.x, a.y * b.y, a.z * b.z);
            }

            public void Scale(Vector3D scale)
            {
                this.x *= scale.x;
                this.y *= scale.y;
                this.z *= scale.z;
            }

            public static Vector3D Cross(Vector3D lhs, Vector3D rhs)
            {
                return new Vector3D((lhs.y * rhs.z - lhs.z * rhs.y), (lhs.z * rhs.x - lhs.x * rhs.z), (lhs.x * rhs.y - lhs.y * rhs.x));
            }

            public override int GetHashCode()
            {
                return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
            }

            public override bool Equals(object other)
            {
                if (!(other is Vector3D))
                    return false;
                Vector3D vector3 = (Vector3D)other;
                if (this.x.Equals(vector3.x) && this.y.Equals(vector3.y))
                    return this.z.Equals(vector3.z);
                else
                    return false;
            }

            public static Vector3D Reflect(Vector3D inDirection, Vector3D inNormal)
            {
                return -2 * Vector3D.Dot(inNormal, inDirection) * inNormal + inDirection;
            }

            public static Vector3D Normalize(Vector3D value)
            {
                double num = Vector3D.Magnitude(value);
                if (num > 9.99999974737875E-06)
                    return value / num;
                else
                    return Vector3D.zero;
            }

            public void Normalize()
            {
                double num = Vector3D.Magnitude(this);
                if (num > 9.99999974737875E-06)
                    this = this / num;
                else
                    this = Vector3D.zero;
            }

            public override string ToString()
            {
                string fmt = "F1";
                return ToString(fmt);
            }

            public string ToString(string format)
            {
                string fmt = "({0}, {1}, {2})";
                object[] objArray = new object[3];
                int index1 = 0;
                string str1 = this.x.ToString(format);
                objArray[index1] = (object)str1;
                int index2 = 1;
                string str2 = this.y.ToString(format);
                objArray[index2] = (object)str2;
                int index3 = 2;
                string str3 = this.z.ToString(format);
                objArray[index3] = (object)str3;
                return String.Format(fmt, objArray);
            }

            public static double Dot(Vector3D lhs, Vector3D rhs)
            {
                return (lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z);
            }

            public static Vector3D Project(Vector3D vector, Vector3D onNormal)
            {
                double num = Vector3D.Dot(onNormal, onNormal);
                if (num < 1.40129846432482E-45)
                    return Vector3D.zero;
                else
                    return onNormal * Vector3D.Dot(vector, onNormal) / num;
            }

            public static Vector3D Exclude(Vector3D excludeThis, Vector3D fromThat)
            {
                return fromThat - Vector3D.Project(fromThat, excludeThis);
            }

            public static double Angle(Vector3D from, Vector3D to)
            {
                var value = Vector3D.Dot(from.normalized, to.normalized);
                if (value < -1.0) value = -1.0; else if (value > 1.0) value = 1.0;

                return System.Math.Acos(value) * 57.29578;
            }

            public static double AngleForRad(Vector3D from, Vector3D to)
            {
                var value = Vector3D.Dot(from.normalized, to.normalized);
                if (value < -1.0) value = -1.0; else if (value > 1.0) value = 1.0;

                return System.Math.Acos(value);
            }

            public static double Distance(Vector3D a, Vector3D b)
            {
                Vector3D vector3 = new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
                return System.Math.Sqrt((vector3.x * vector3.x + vector3.y * vector3.y + vector3.z * vector3.z));
            }

            /// <summary>
            /// 两点间距离的平方，可以用来在某些地方代替Distance(a,b)，来提高运算速度
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double DistancePow(Vector3D a, Vector3D b)
            {
                return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z);
            }

            /// <summary>
            /// 额外的一个计算两点距离的方法，之前的方法可能会溢出
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            public static double DistanceEx(Vector3D a, Vector3D b)
            {
                double deltaX = System.Math.Abs(b.x - a.x);
                double deltaY = System.Math.Abs(b.y - a.y);
                double deltaZ = System.Math.Abs(b.z - a.z);

                if (deltaX == 0 && deltaY == 0 && deltaZ == 0)
                {
                    return 0;
                }
                else if (deltaX == 0 && deltaY == 0)
                {
                    return deltaZ;
                }
                else if (deltaX == 0 && deltaZ == 0)
                {
                    return deltaY;
                }
                else if (deltaY == 0 && deltaZ == 0)
                {
                    return deltaX;
                }
                else if (deltaX == 0)
                {
                    double angleYZ = System.Math.Atan(deltaZ / deltaY);
                    return deltaZ / System.Math.Sin(angleYZ);
                }
                else if (deltaY == 0)
                {
                    double angleXZ = System.Math.Atan(deltaZ / deltaX);
                    return deltaZ / System.Math.Sin(angleXZ);
                }
                else if (deltaZ == 0)
                {
                    double angleXY = System.Math.Atan(deltaY / deltaX);
                    return deltaY / System.Math.Sin(angleXY);
                }
                else
                {
                    double angeXY = System.Math.Atan(deltaY / deltaX);
                    double lengthXY = deltaY / System.Math.Sin(angeXY);

                    double angleXYZ = System.Math.Atan(lengthXY / deltaZ);
                    return lengthXY / System.Math.Sin(angleXYZ);
                }
            }

            public static Vector3D ClampMagnitude(Vector3D vector, double maxLength)
            {
                if (vector.sqrMagnitude > maxLength * maxLength)
                    return vector.normalized * maxLength;
                else
                    return vector;
            }

            public static double Magnitude(Vector3D a)
            {
                return System.Math.Sqrt((a.x * a.x + a.y * a.y + a.z * a.z));
            }

            public static double SqrMagnitude(Vector3D a)
            {
                return (a.x * a.x + a.y * a.y + a.z * a.z);
            }

            public static Vector3D Min(Vector3D lhs, Vector3D rhs)
            {
                return new Vector3D(System.Math.Min(lhs.x, rhs.x), System.Math.Min(lhs.y, rhs.y), System.Math.Min(lhs.z, rhs.z));
            }

            public static Vector3D Max(Vector3D lhs, Vector3D rhs)
            {
                return new Vector3D(System.Math.Max(lhs.x, rhs.x), System.Math.Max(lhs.y, rhs.y), System.Math.Max(lhs.z, rhs.z));
            }

            public static bool IsSameDirection(Vector3D v1, Vector3D v2)
            {
                return v1.normalized == v2.normalized;
            }

            /// <summary>
            /// 近似计算给定点是否在指定点的指定半径内
            /// </summary>
            /// <param name="source"></param>
            /// <param name="center"></param>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static bool IsInRangeSimilar(Vector3D source, Vector3D center, double radius)
            {
                return
                    source.x <= center.x + radius && source.x >= center.x - radius &&
                    source.y <= center.y + radius && source.y >= center.y - radius &&
                    source.z <= center.z + radius && source.z >= center.z - radius;
            }

            /// <summary>
            /// 返回该向量的一个非零垂直单位向量
            /// </summary>
            /// <returns>与该向量垂直的非零向量，如果返回了零向量，说明传入的向量也是零向量</returns>
            public static Vector3D GetNormalizedVerticalVector(Vector3D vec)
            {
                /*
                 * 1. 在第一个向量中找到一个不为0的数a[i]，如果全为0，则这个向量为0向量，任何其它一个向量都和其垂直；
                 * 2. 令b中除了b[i]外全置为1(这个数可以变).b[i]=0；
                 * 3. 求出sum = Dot(a, b); i是a 中的一个不为0的数的下标；
                 * 4. 求出b[i] = -sum / a[i]；
                 */

                if (vec == Vector3D.zero)
                {
                    return Vector3D.zero;
                }

                Vector3D verticalVec = Vector3D.zero;
                double dotValue = 0;

                if (vec.x != 0)
                {
                    dotValue = vec.y + vec.z;

                    verticalVec.x = -dotValue / vec.x;
                    verticalVec.y = verticalVec.z = 1;
                }
                else if (vec.y != 0)
                {
                    dotValue = vec.z;
                    verticalVec.y = -dotValue / vec.y;
                    verticalVec.x = verticalVec.z = 1;
                }
                else if (vec.z != 0)
                {
                    dotValue = 0;
                    verticalVec.z = -dotValue / vec.z;
                    verticalVec.x = verticalVec.y = 1;
                }

                return verticalVec.normalized;
            }
        }
    }
}
