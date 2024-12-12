﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dest
{
    namespace Math
    {
        public struct Vector2D
        {
            public const double kEpsilon = 1E-05;
            public double x;
            public double y;

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
                        default:
                            throw new IndexOutOfRangeException("Invalid Vector2D index!");
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
                        default:
                            throw new IndexOutOfRangeException("Invalid Vector2D index!");
                    }
                }
            }

            public Vector2D normalized
            {
                get
                {
                    Vector2D Vector2D = new Vector2D(this.x, this.y);
                    Vector2D.Normalize();
                    return Vector2D;
                }
            }

            public double magnitude
            {
                get
                {
                    return System.Math.Sqrt((this.x * this.x + this.y * this.y));
                }
            }

            public double sqrMagnitude
            {
                get
                {
                    return (this.x * this.x + this.y * this.y);
                }
            }

            public static Vector2D zero
            {
                get
                {
                    return new Vector2D(0.0, 0.0);
                }
            }

            public static Vector2D one
            {
                get
                {
                    return new Vector2D(1, 1);
                }
            }

            public static Vector2D up
            {
                get
                {
                    return new Vector2D(0.0, 1);
                }
            }

            public static Vector2D right
            {
                get
                {
                    return new Vector2D(1, 0.0);
                }
            }

            public Vector2D(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public static implicit operator Vector2D(Vector3D v)
            {
                return new Vector2D(v.x, v.y);
            }

            public static implicit operator Vector3D(Vector2D v)
            {
                return new Vector3D(v.x, v.y, 0.0);
            }

            public static Vector2D operator +(Vector2D a, Vector2D b)
            {
                return new Vector2D(a.x + b.x, a.y + b.y);
            }

            public static Vector2D operator -(Vector2D a, Vector2D b)
            {
                return new Vector2D(a.x - b.x, a.y - b.y);
            }

            public static Vector2D operator -(Vector2D a)
            {
                return new Vector2D(-a.x, -a.y);
            }

            public static Vector2D operator *(Vector2D a, double d)
            {
                return new Vector2D(a.x * d, a.y * d);
            }

            public static Vector2D operator *(double d, Vector2D a)
            {
                return new Vector2D(a.x * d, a.y * d);
            }

            public static Vector2D operator /(Vector2D a, double d)
            {
                return new Vector2D(a.x / d, a.y / d);
            }

            public static bool operator ==(Vector2D lhs, Vector2D rhs)
            {
                return Vector2D.SqrMagnitude(lhs - rhs) < kEpsilon;
            }

            public static bool operator !=(Vector2D lhs, Vector2D rhs)
            {
                return Vector2D.SqrMagnitude(lhs - rhs) >= kEpsilon;
            }

            public void Set(double new_x, double new_y)
            {
                this.x = new_x;
                this.y = new_y;
            }

            public static Vector2D Lerp(Vector2D from, Vector2D to, double t)
            {
                if (t < 0.0) t = 0.0; else if (t > 1.0) t = 1.0;
                return new Vector2D(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t);
            }

            public static Vector2D MoveTowards(Vector2D current, Vector2D target, double maxDistanceDelta)
            {
                Vector2D Vector2D = target - current;
                double magnitude = Vector2D.magnitude;
                if (magnitude <= maxDistanceDelta || magnitude == 0.0)
                    return target;
                else
                    return current + Vector2D / magnitude * maxDistanceDelta;
            }

            public static Vector2D Scale(Vector2D a, Vector2D b)
            {
                return new Vector2D(a.x * b.x, a.y * b.y);
            }

            public void Scale(Vector2D scale)
            {
                this.x *= scale.x;
                this.y *= scale.y;
            }

            public void Normalize()
            {
                double magnitude = this.magnitude;
                if (magnitude > 9.99999974737875E-06)
                    this = this / magnitude;
                else
                    this = Vector2D.zero;
            }

            public override string ToString()
            {
                string fmt = "F1";
                return ToString(fmt);
            }

            public string ToString(string format)
            {
                string fmt = "({0}, {1})";
                object[] objArray = new object[2];
                int index1 = 0;
                string str1 = this.x.ToString(format);
                objArray[index1] = (object)str1;
                int index2 = 1;
                string str2 = this.y.ToString(format);
                objArray[index2] = (object)str2;
                return String.Format(fmt, objArray);
            }

            public override int GetHashCode()
            {
                return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
            }

            public override bool Equals(object other)
            {
                if (!(other is Vector2D))
                    return false;
                Vector2D Vector2D = (Vector2D)other;
                if (this.x.Equals(Vector2D.x))
                    return this.y.Equals(Vector2D.y);
                else
                    return false;
            }

            public static double Dot(Vector2D lhs, Vector2D rhs)
            {
                return (lhs.x * rhs.x + lhs.y * rhs.y);
            }

            public static double Angle(Vector2D from, Vector2D to)
            {
                var value = Vector2D.Dot(from.normalized, to.normalized);
                if (value < -1.0) value = -1.0; else if (value > 1.0) value = 1.0;

                return System.Math.Acos(value) * 57.29578;
            }

            public static double Distance(Vector2D a, Vector2D b)
            {
                return (a - b).magnitude;
            }

            public static double DistancePow(Vector2D a, Vector2D b)
            {
                return (a - b).SqrMagnitude();
            }

            public static Vector2D ClampMagnitude(Vector2D vector, double maxLength)
            {
                if (vector.sqrMagnitude > maxLength * maxLength)
                    return vector.normalized * maxLength;
                else
                    return vector;
            }

            public static double SqrMagnitude(Vector2D a)
            {
                return (a.x * a.x + a.y * a.y);
            }

            public double SqrMagnitude()
            {
                return (this.x * this.x + this.y * this.y);
            }

            public static Vector2D Min(Vector2D lhs, Vector2D rhs)
            {
                return new Vector2D(System.Math.Min(lhs.x, rhs.x), System.Math.Min(lhs.y, rhs.y));
            }

            public static Vector2D Max(Vector2D lhs, Vector2D rhs)
            {
                return new Vector2D(System.Math.Max(lhs.x, rhs.x), System.Math.Max(lhs.y, rhs.y));
            }
        }
    }
}
