using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dest
{
    namespace Math
    {
        [Serializable]
        public struct Quaternion
        {
            public const double kEpsilon = 1E-06f;
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
                    throw new IndexOutOfRangeException("Invalid Quaternion index!");
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
                    throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
              }
            }

            public static Quaternion identity
            {
              get
              {
                return new Quaternion(0.0f, 0.0f, 0.0f, 1f);
              }
            }

            public Vector3D eulerAngles
            {
              get
              {
                return Quaternion.Internal_ToEulerRad(this) * 57.29578f;
              }
              set
              {
                this = Quaternion.Internal_FromEulerRad(value * (System.Math.PI / 180.0));
              }
            }

            public Quaternion(double x, double y, double z, double w)
            {
              this.x = x;
              this.y = y;
              this.z = z;
              this.w = w;
            }

            public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
            {
              return new Quaternion( ( lhs.w *  rhs.x +  lhs.x *  rhs.w +  lhs.y *  rhs.z -  lhs.z *  rhs.y),  ( lhs.w *  rhs.y +  lhs.y *  rhs.w +  lhs.z *  rhs.x -  lhs.x *  rhs.z),  ( lhs.w *  rhs.z +  lhs.z *  rhs.w +  lhs.x *  rhs.y -  lhs.y *  rhs.x),  ( lhs.w *  rhs.w -  lhs.x *  rhs.x -  lhs.y *  rhs.y -  lhs.z *  rhs.z));
            }

            public static Vector3D operator *(Quaternion rotation, Vector3D point)
            {
              double num1 = rotation.x * 2f;
              double num2 = rotation.y * 2f;
              double num3 = rotation.z * 2f;
              double num4 = rotation.x * num1;
              double num5 = rotation.y * num2;
              double num6 = rotation.z * num3;
              double num7 = rotation.x * num2;
              double num8 = rotation.x * num3;
              double num9 = rotation.y * num3;
              double num10 = rotation.w * num1;
              double num11 = rotation.w * num2;
              double num12 = rotation.w * num3;
              Vector3D vector3;
              vector3.x =  ((1.0 - ( num5 +  num6)) *  point.x + ( num7 -  num12) *  point.y + ( num8 +  num11) *  point.z);
              vector3.y =  (( num7 +  num12) *  point.x + (1.0 - ( num4 +  num6)) *  point.y + ( num9 -  num10) *  point.z);
              vector3.z =  (( num8 -  num11) *  point.x + ( num9 +  num10) *  point.y + (1.0 - ( num4 +  num5)) *  point.z);
              return vector3;
            }

            public static bool operator ==(Quaternion lhs, Quaternion rhs)
            {
              return  Quaternion.Dot(lhs, rhs) > 0.999998986721039;
            }

            public static bool operator !=(Quaternion lhs, Quaternion rhs)
            {
              return  Quaternion.Dot(lhs, rhs) <= 0.999998986721039;
            }

            public static Quaternion operator +(Quaternion q0, Quaternion q1)
            {
                return new Quaternion(q0.x + q1.x, q0.y + q1.y, q0.z + q1.z, q0.w + q1.w);
            }

            public static Quaternion operator -(Quaternion q0, Quaternion q1)
            {
                return new Quaternion(q0.x - q1.x, q0.y - q1.y, q0.z - q1.z, q0.w - q1.w);
            }

            public static Quaternion operator *(Quaternion q, double t)
            {
                return new Quaternion(q.x * t, q.y * t, q.z * t, q.w * t);
            }

            public static Quaternion operator /(Quaternion q, double t)
            {
                return new Quaternion(q.x / t, q.y / t, q.z / t, q.w / t);
            }

            public void Set(double new_x, double new_y, double new_z, double new_w)
            {
              this.x = new_x;
              this.y = new_y;
              this.z = new_z;
              this.w = new_w;
            }

            public static double Dot(Quaternion a, Quaternion b)
            {
              return  ( a.x *  b.x +  a.y *  b.y +  a.z *  b.z +  a.w *  b.w);
            }

            public static Quaternion AngleAxis(double angle, Vector3D axis)
            {
              return Quaternion.INTERNAL_CALL_AngleAxis(angle, ref axis);
            }

            private static Quaternion INTERNAL_CALL_AngleAxis(double angle, ref Vector3D axis)
            {
                axis.Normalize();
                double halfRad = angle * Mathex.Deg2Rad * 0.5;
                double sinHalfRad = System.Math.Sin(halfRad);
                return new Quaternion(
                            axis.x * sinHalfRad,
                            axis.y * sinHalfRad,
                            axis.z * sinHalfRad,
                            System.Math.Cos(halfRad));
            }

            public void ToAngleAxis(out double angle, out Vector3D axis)
            {
              Quaternion.Internal_ToAxisAngleRad(this, out axis, out angle);
              angle = angle * 57.29578f;
            }

            public static Quaternion FromToRotation(Vector3D fromDirection, Vector3D toDirection)
            {
              return Quaternion.INTERNAL_CALL_FromToRotation(ref fromDirection, ref toDirection);
            }

            public static double Length(Quaternion q)
            {
                return System.Math.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            }

            public static Quaternion Normalize(Quaternion q)
            {
                double l = Length(q);
                if (l <= 0)
                    return q;

                double rl = 1 / l;
                q.x *= rl;
                q.y *= rl;
                q.z *= rl;
                q.w *= rl;
                return q;
            }

            // http://lolengine.net/blog/2013/09/18/beautiful-maths-quaternion-from-vectors
            private static Quaternion INTERNAL_CALL_FromToRotation(ref Vector3D fromDirection, ref Vector3D toDirection)
            {
                Vector3D c = Vector3D.Cross(fromDirection, toDirection);
                double d = Vector3D.Dot(fromDirection, toDirection);
                Quaternion q = new Quaternion(c.x, c.y, c.z, d);
                q.w += Length(q);
                return Normalize(q);
            }

            public void SetFromToRotation(Vector3D fromDirection, Vector3D toDirection)
            {
              this = Quaternion.FromToRotation(fromDirection, toDirection);
            }

            public static Quaternion LookRotation(Vector3D forward, Vector3D upwards)
            {
              return Quaternion.INTERNAL_CALL_LookRotation(ref forward, ref upwards);
            }

            public static Quaternion LookRotation(Vector3D forward)
            {
              Vector3D up = Vector3D.up;
              return Quaternion.INTERNAL_CALL_LookRotation(ref forward, ref up);
            }

            // http://www.gamedev.net/topic/613595-quaternion-lookrotationlookat-up/
            // http://www.gamedev.net/topic/648857-how-to-implement-lookrotation/            
            private static Quaternion INTERNAL_CALL_LookRotation(ref Vector3D forward, ref Vector3D upwards)
            {
                Vector3D front = forward.normalized;
                Vector3D right = Vector3D.Cross(upwards, front).normalized;
                Vector3D up = Vector3D.Cross(front, right);

                double m00 = right.x;
                double m10 = right.y;
                double m20 = right.z;
                double m01 = up.x;
                double m11 = up.y;
                double m21 = up.z;
                double m02 = front.x;
                double m12 = front.y;
                double m22 = front.z;
                
                // convert from matrix to quaternion
                // http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm

                Quaternion q = new Quaternion();
                double tr = m00 + m11 + m22;
                if (tr > 0)
                {
                    double S = System.Math.Sqrt(tr + 1.0) * 2; // S=4*qw 
                    q.w = 0.25 * S;
                    q.x = (m21 - m12) / S;
                    q.y = (m02 - m20) / S;
                    q.z = (m10 - m01) / S;
                }
                else if ((m00 > m11) & (m00 > m22))
                {
                    double S = System.Math.Sqrt(1.0 + m00 - m11 - m22) * 2; // S=4*qx 
                    q.w = (m21 - m12) / S;
                    q.x = 0.25 * S;
                    q.y = (m01 + m10) / S;
                    q.z = (m02 + m20) / S;
                }
                else if (m11 > m22)
                {
                    double S = System.Math.Sqrt(1.0 + m11 - m00 - m22) * 2; // S=4*qy
                    q.w = (m02 - m20) / S;
                    q.x = (m01 + m10) / S;
                    q.y = 0.25 * S;
                    q.z = (m12 + m21) / S;
                }
                else
                {
                    double S = System.Math.Sqrt(1.0 + m22 - m00 - m11) * 2; // S=4*qz
                    q.w = (m10 - m01) / S;
                    q.x = (m02 + m20) / S;
                    q.y = (m12 + m21) / S;
                    q.z = 0.25 * S;
                }
                return q;
            }

            public void SetLookRotation(Vector3D view)
            {
              Vector3D up = Vector3D.up;
              this.SetLookRotation(view, up);
            }

            public void SetLookRotation(Vector3D view, Vector3D up)
            {
              this = Quaternion.LookRotation(view, up);
            }

            public static Quaternion Slerp(Quaternion from, Quaternion to, double t)
            {
                if (t < 0)
                    t = 0;
                else if (t > 1)
                    t = 1;

                return SlerpUnclamped(from, to, t);
            }

            //[WrapperlessIcall]
            //[MethodImpl(MethodImplOptions.InternalCall)]
            //private static extern Quaternion INTERNAL_CALL_Slerp(ref Quaternion from, ref Quaternion to, double t);

            public static Quaternion Lerp(Quaternion from, Quaternion to, double t)
            {
                if (t < 0)
                    t = 0;
                else if (t > 1)
                    t = 1;

                if (Quaternion.Dot(from, to) < 0)
                    return Normalize(from - (to + from) * t);
                else
                    return Normalize(from + (to - from) * t);
            }

            //[WrapperlessIcall]
            //[MethodImpl(MethodImplOptions.InternalCall)]
            //private static extern Quaternion INTERNAL_CALL_Lerp(ref Quaternion from, ref Quaternion to, double t);

            public static Quaternion RotateTowards(Quaternion from, Quaternion to, double maxDegreesDelta)
            {
              double num = Quaternion.Angle(from, to);
              if ( num == 0.0)
                return to;
              double t = System.Math.Min(1f, maxDegreesDelta / num);
              return Quaternion.SlerpUnclamped(from, to, t);
            }

            // reference: OgreQuaternion.cpp 
            public static Quaternion SlerpUnclamped(Quaternion from, Quaternion to, double t)
            {
                double cosine = Quaternion.Dot(from, to);
                Quaternion bb;

                // Do we need to invert rotation?
                if (cosine < 0)
                {
                    cosine = -cosine;
                    bb = new Quaternion(-to.x, -to.y, -to.z, -to.w);
                }
                else
                {
                    bb = to;
                }

                if (System.Math.Abs(cosine) < 0.999f)
                {
                    // Standard case (slerp)
                    double sine = System.Math.Sqrt(1 - cosine * cosine);
                    double angle = System.Math.Atan2(sine, cosine);
                    double invSin = 1.0f / sine;
                    double c0 = System.Math.Sin((1.0f - t) * angle) * invSin;
                    double c1 = System.Math.Sin(t * angle) * invSin;
                    return from * c0 + bb * c1;
                }
                else
                {
                    // There are two situations:
                    // 1. "a" and "b" are very close (cosine ~= +1), so we can do a linear
                    //    interpolation safely.
                    // 2. "a" and "b" are almost inverse of each other (cosine ~= -1), there
                    //    are an infinite number of possibilities interpolation. but we haven't
                    //    have method to fix this case, so just use linear interpolation here.
                    Quaternion q = from * (1.0f - t) + bb * t;
                    // taking the complement requires renormalisation
                    return Normalize(q);
                }
            }

            //[WrapperlessIcall]
            //[MethodImpl(MethodImplOptions.InternalCall)]
            //private static extern Quaternion INTERNAL_CALL_UnclampedSlerp(ref Quaternion from, ref Quaternion to, double t);

            public static Quaternion Inverse(Quaternion rotation)
            {
                return new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
            }

            //[WrapperlessIcall]
            //[MethodImpl(MethodImplOptions.InternalCall)]
            //private static extern Quaternion INTERNAL_CALL_Inverse(ref Quaternion rotation);

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
              objArray[index1] = (object) str1;
              int index2 = 1;
              string str2 = this.y.ToString(format);
              objArray[index2] = (object) str2;
              int index3 = 2;
              string str3 = this.z.ToString(format);
              objArray[index3] = (object) str3;
              int index4 = 3;
              string str4 = this.w.ToString(format);
              objArray[index4] = (object) str4;
              return String.Format(fmt, objArray);
            }

            public static double Angle(Quaternion a, Quaternion b)
            {
              return  ( System.Math.Acos(System.Math.Min(System.Math.Abs(Quaternion.Dot(a, b)), 1f)) * 2.0 * 57.2957801818848);
            }

            public static Quaternion Euler(double x, double y, double z)
            {
              return Quaternion.Internal_FromEulerRad(new Vector3D(x, y, z) * (System.Math.PI / 180.0));
            }

            public static Quaternion Euler(Vector3D euler)
            {
              return Quaternion.Internal_FromEulerRad(euler * (System.Math.PI / 180.0));
            }

            private static Vector3D ThreeAxisRot(double r11, double r12, double r21, double r31, double r32)
            {
                // 限制一下r21的值，必须在[-1.1]，防止因为浮点数误差导致计算出错
                r21 = System.Math.Min(1, r21);
                r21 = System.Math.Max(-1, r21);

                return new Vector3D(System.Math.Asin(r21),
                                    System.Math.Atan2(r11, r12),
                                    System.Math.Atan2(r31, r32));
            }

            // http://bediyap.com/programming/convert-quaternion-to-euler-rotations/
            private static Vector3D Internal_ToEulerRad(Quaternion rotation)
            {
                Quaternion q = Normalize(rotation);
                return ThreeAxisRot(2 * (q.x * q.z + q.w * q.y),
                                    q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                                    -2 * (q.y * q.z - q.w * q.x),
                                    2 * (q.x * q.y + q.w * q.z),
                                    q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);
            }

            private static Quaternion AngleAxis(double rad, double ax, double ay, double az)
            {
                double halfRad = rad * 0.5f;
                double sinHalfRad = System.Math.Sin(halfRad);
                return new Quaternion(
                            ax * sinHalfRad,
                            ay * sinHalfRad,
                            az * sinHalfRad,
                            System.Math.Cos(halfRad));
            }

            //[WrapperlessIcall]
            //[MethodImpl(MethodImplOptions.InternalCall)]
            //private static extern Vector3D INTERNAL_CALL_Internal_ToEulerRad(ref Quaternion rotation);

            private static Quaternion Internal_FromEulerRad(Vector3D euler)
            {
                return AngleAxis(euler.y, 0, 1, 0) *
                       AngleAxis(euler.x, 1, 0, 0) *
                       AngleAxis(euler.z, 0, 0, 1);
            }

            //[WrapperlessIcall]
            //[MethodImpl(MethodImplOptions.InternalCall)]
            //private static extern Quaternion INTERNAL_CALL_Internal_FromEulerRad(ref Vector3D euler);

            private static void Internal_ToAxisAngleRad(Quaternion q, out Vector3D axis, out double angle)
            {
                double halfRad = System.Math.Acos(q.w);
                if (halfRad > 0)
                {
                    double sinHalfRad = System.Math.Sin(halfRad);
                    angle = halfRad * 2;
                    axis.x = q.x / sinHalfRad;
                    axis.y = q.y / sinHalfRad;
                    axis.z = q.z / sinHalfRad;
                }
                else
                {
                    angle = 0;
                    axis.x = 1;
                    axis.y = 0;
                    axis.z = 0;
                }
            }

            //[WrapperlessIcall]
            //[MethodImpl(MethodImplOptions.InternalCall)]
            //private static extern void INTERNAL_CALL_Internal_ToAxisAngleRad(ref Quaternion q, out Vector3D axis, out double angle);

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public static Quaternion EulerRotation(double x, double y, double z)
            //{
            //  return Quaternion.Internal_FromEulerRad(new Vector3D(x, y, z));
            //}

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public static Quaternion EulerRotation(Vector3D euler)
            //{
            //  return Quaternion.Internal_FromEulerRad(euler);
            //}

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public void SetEulerRotation(double x, double y, double z)
            //{
            //  this = Quaternion.Internal_FromEulerRad(new Vector3D(x, y, z));
            //}

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public void SetEulerRotation(Vector3D euler)
            //{
            //  this = Quaternion.Internal_FromEulerRad(euler);
            //}

            //[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
            //public Vector3D ToEuler()
            //{
            //  return Quaternion.Internal_ToEulerRad(this);
            //}

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public static Quaternion EulerAngles(double x, double y, double z)
            //{
            //  return Quaternion.Internal_FromEulerRad(new Vector3D(x, y, z));
            //}

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public static Quaternion EulerAngles(Vector3D euler)
            //{
            //  return Quaternion.Internal_FromEulerRad(euler);
            //}

            //[Obsolete("Use Quaternion.ToAngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
            //public void ToAxisAngle(out Vector3D axis, out double angle)
            //{
            //  Quaternion.Internal_ToAxisAngleRad(this, out axis, out angle);
            //}

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public void SetEulerAngles(double x, double y, double z)
            //{
            //  this.SetEulerRotation(new Vector3D(x, y, z));
            //}

            //[Obsolete("Use Quaternion.Euler instead. This function was deprecated because it uses radians instead of degrees")]
            //public void SetEulerAngles(Vector3D euler)
            //{
            //  this = Quaternion.EulerRotation(euler);
            //}

            //[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
            //public static Vector3D ToEulerAngles(Quaternion rotation)
            //{
            //  return Quaternion.Internal_ToEulerRad(rotation);
            //}

            //[Obsolete("Use Quaternion.eulerAngles instead. This function was deprecated because it uses radians instead of degrees")]
            //public Vector3D ToEulerAngles()
            //{
            //  return Quaternion.Internal_ToEulerRad(this);
            //}

            //[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
            //public static Quaternion AxisAngle(Vector3D axis, double angle)
            //{
            //    //return Quaternion.INTERNAL_CALL_AxisAngle(ref axis, angle);
            //    throw new NotImplementedException("INTERNAL_CALL_AxisAngle not implemented");
            //}

            ////[WrapperlessIcall]
            ////[MethodImpl(MethodImplOptions.InternalCall)]
            ////private static extern Quaternion INTERNAL_CALL_AxisAngle(ref Vector3D axis, double angle);

            //[Obsolete("Use Quaternion.AngleAxis instead. This function was deprecated because it uses radians instead of degrees")]
            //public void SetAxisAngle(Vector3D axis, double angle)
            //{
            //  this = Quaternion.AxisAngle(axis, angle);
            //}

            public override int GetHashCode()
            {
              return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2 ^ this.w.GetHashCode() >> 1;
            }

            public override bool Equals(object other)
            {
              if (!(other is Quaternion))
                return false;
              Quaternion quaternion = (Quaternion) other;
              if (this.x.Equals(quaternion.x) && this.y.Equals(quaternion.y) && this.z.Equals(quaternion.z))
                return this.w.Equals(quaternion.w);
              else
                return false;
            }
        }
    }
}
