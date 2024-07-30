using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace bluebean
{
    public static class PhysicsUtil
    {
        public const float epsilon = 0.0000001f;
        public const float sqrt3 = 1.73205080f;
        public const float sqrt2 = 1.41421356f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PureSign(this float val)
        {
            return ((0 <= val) ? 1 : 0) - ((val < 0) ? 1 : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static Bounds Transform(this Bounds b, Matrix4x4 m)
        {
            var xa = m.GetColumn(0) * b.min.x;
            var xb = m.GetColumn(0) * b.max.x;

            var ya = m.GetColumn(1) * b.min.y;
            var yb = m.GetColumn(1) * b.max.y;

            var za = m.GetColumn(2) * b.min.z;
            var zb = m.GetColumn(2) * b.max.z;

            Bounds result = new Bounds();
            Vector3 pos = m.GetColumn(3);
            result.SetMinMax(Vector3.Min(xa, xb) + Vector3.Min(ya, yb) + Vector3.Min(za, zb) + pos,
                             Vector3.Max(xa, xb) + Vector3.Max(ya, yb) + Vector3.Max(za, zb) + pos);


            return result;
        }

        public static void NearestPointOnTri(in Vector3 p1,
                                               in Vector3 p2,
                                               in Vector3 p3,
                                               in Vector3 p,
                                               out Vector3 result)
        {
            float e0x = p2.x - p1.x;
            float e0y = p2.y - p1.y;
            float e0z = p2.z - p1.z;

            float e1x = p3.x - p1.x;
            float e1y = p3.y - p1.y;
            float e1z = p3.z - p1.z;

            float v0x = p1.x - p.x;
            float v0y = p1.y - p.y;
            float v0z = p1.z - p.z;

            float a00 = e0x * e0x + e0y * e0y + e0z * e0z;
            float a01 = e0x * e1x + e0y * e1y + e0z * e1z;
            float a11 = e1x * e1x + e1y * e1y + e1z * e1z;
            float b0 = e0x * v0x + e0y * v0y + e0z * v0z;
            float b1 = e1x * v0x + e1y * v0y + e1z * v0z;

            const float zero = 0;
            const float one = 1;

            float det = a00 * a11 - a01 * a01;
            float t0 = a01 * b1 - a11 * b0;
            float t1 = a01 * b0 - a00 * b1;

            if (t0 + t1 <= det)
            {
                if (t0 < zero)
                {
                    if (t1 < zero)  // region 4
                    {
                        if (b0 < zero)
                        {
                            t1 = zero;
                            if (-b0 >= a00)  // V0
                            {
                                t0 = one;
                            }
                            else  // E01
                            {
                                t0 = -b0 / a00;
                            }
                        }
                        else
                        {
                            t0 = zero;
                            if (b1 >= zero)  // V0
                            {
                                t1 = zero;
                            }
                            else if (-b1 >= a11)  // V2
                            {
                                t1 = one;
                            }
                            else  // E20
                            {
                                t1 = -b1 / a11;
                            }
                        }
                    }
                    else  // region 3
                    {
                        t0 = zero;
                        if (b1 >= zero)  // V0
                        {
                            t1 = zero;
                        }
                        else if (-b1 >= a11)  // V2
                        {
                            t1 = one;
                        }
                        else  // E20
                        {
                            t1 = -b1 / a11;
                        }
                    }
                }
                else if (t1 < zero)  // region 5
                {
                    t1 = zero;
                    if (b0 >= zero)  // V0
                    {
                        t0 = zero;
                    }
                    else if (-b0 >= a00)  // V1
                    {
                        t0 = one;
                    }
                    else  // E01
                    {
                        t0 = -b0 / a00;
                    }
                }
                else  // region 0, interior
                {
                    float invDet = one / det;
                    t0 *= invDet;
                    t1 *= invDet;
                }
            }
            else
            {
                float tmp0, tmp1, numer, denom;

                if (t0 < zero)  // region 2
                {
                    tmp0 = a01 + b0;
                    tmp1 = a11 + b1;
                    if (tmp1 > tmp0)
                    {
                        numer = tmp1 - tmp0;
                        denom = a00 - 2 * a01 + a11;
                        if (numer >= denom)  // V1
                        {
                            t0 = one;
                            t1 = zero;
                        }
                        else  // E12
                        {
                            t0 = numer / denom;
                            t1 = one - t0;
                        }
                    }
                    else
                    {
                        t0 = zero;
                        if (tmp1 <= zero)  // V2
                        {
                            t1 = one;
                        }
                        else if (b1 >= zero)  // V0
                        {
                            t1 = zero;
                        }
                        else  // E20
                        {
                            t1 = -b1 / a11;
                        }
                    }
                }
                else if (t1 < zero)  // region 6
                {
                    tmp0 = a01 + b1;
                    tmp1 = a00 + b0;
                    if (tmp1 > tmp0)
                    {
                        numer = tmp1 - tmp0;
                        denom = a00 - 2 * a01 + a11;
                        if (numer >= denom)  // V2
                        {
                            t1 = one;
                            t0 = zero;
                        }
                        else  // E12
                        {
                            t1 = numer / denom;
                            t0 = one - t1;
                        }
                    }
                    else
                    {
                        t1 = zero;
                        if (tmp1 <= zero)  // V1
                        {
                            t0 = one;
                        }
                        else if (b0 >= zero)  // V0
                        {
                            t0 = zero;
                        }
                        else  // E01
                        {
                            t0 = -b0 / a00;
                        }
                    }
                }
                else  // region 1
                {
                    numer = a11 + b1 - a01 - b0;
                    if (numer <= zero)  // V2
                    {
                        t0 = zero;
                        t1 = one;
                    }
                    else
                    {
                        denom = a00 - 2 * a01 + a11;
                        if (numer >= denom)  // V1
                        {
                            t0 = one;
                            t1 = zero;
                        }
                        else  // 12
                        {
                            t0 = numer / denom;
                            t1 = one - t0;
                        }
                    }
                }
            }

            result.x = p1.x + t0 * e0x + t1 * e1x;
            result.y = p1.y + t0 * e0y + t1 * e1y;
            result.z = p1.z + t0 * e0z + t1 * e1z;
        }

        public static void GetPointCloudAnisotropy(List<Vector3> points, float max_anisotropy, float radius, in Vector3 hint_normal, ref Vector3 centroid, ref Quaternion orientation, ref Vector3 principal_radii)
        {
            int count = points.Count;
            if (count < 2 || radius <= 0 || max_anisotropy <= 0)
            {
                principal_radii = Vector3.one * radius;
                orientation = Quaternion.identity;
                return;
            }

            centroid = GetPointCloudCentroid(points);

            // three columns of a 3x3 anisotropy matrix: 
            Vector4 c0 = Vector4.zero,
            c1 = Vector4.zero,
            c2 = Vector4.zero;

            Matrix4x4 anisotropy = Matrix4x4.zero;

            // multiply offset by offset transposed, and add to matrix:
            for (int i = 0; i < count; i++)
            {
                Vector4 offset = points[i] - centroid;
                c0 += offset * offset[0];
                c1 += offset * offset[1];
                c2 += offset * offset[2];
            }

            // calculate maximum absolute value:
            float max0 = Mathf.Max(Mathf.Max(Mathf.Abs(c0.x), Mathf.Abs(c0.y)), Mathf.Abs(c0.z));
            float max1 = Mathf.Max(Mathf.Max(Mathf.Abs(c1.x), Mathf.Abs(c1.y)), Mathf.Abs(c1.z));
            float max2 = Mathf.Max(Mathf.Max(Mathf.Abs(c2.x), Mathf.Abs(c2.y)), Mathf.Abs(c2.z));
            float max = Mathf.Max(Mathf.Max(max0, max1), max2);

            // normalize matrix:
            if (max > epsilon)
            {
                c0 /= max;
                c1 /= max;
                c2 /= max;
            }

            anisotropy.SetColumn(0, c0);
            anisotropy.SetColumn(1, c1);
            anisotropy.SetColumn(2, c2);

            Matrix4x4 orientMat;
            EigenSolve(anisotropy, out principal_radii, out orientMat);

            // flip orientation if it is not in the same side as the hint normal:
            if (Vector3.Dot(orientMat.GetColumn(2), hint_normal) < 0)
            {
                orientMat.SetColumn(2, orientMat.GetColumn(2) * -1);
                orientMat.SetColumn(1, orientMat.GetColumn(1) * -1);
            }

            max = principal_radii[0];
            principal_radii = Vector3.Max(principal_radii, Vector3.one * max / max_anisotropy) / max * radius;
            orientation = orientMat.rotation;
        }

        public static Vector3 GetPointCloudCentroid(List<Vector3> points)
        {
            Vector3 centroid = Vector3.zero;
            for (int i = 0; i < points.Count; ++i)
                centroid += points[i];
            return centroid / points.Count;
        }

        public static void EigenSolve(Matrix4x4 D, out Vector3 S, out Matrix4x4 V)
        {
            // D is symmetric
            // S is a vector whose elements are eigenvalues
            // V is a matrix whose columns are eigenvectors
            S = EigenValues(D);
            Vector3 V0, V1, V2;

            if (S[0] - S[1] > S[1] - S[2])
            {
                V0 = EigenVector(D, S[0]);
                if (S[1] - S[2] < epsilon)
                {
                    V2 = V0.unitOrthogonal();
                }
                else
                {
                    V2 = EigenVector(D, S[2]); V2 -= V0 * Vector3.Dot(V0, V2); V2 = Vector3.Normalize(V2);
                }
                V1 = Vector3.Cross(V2, V0);
            }
            else
            {
                V2 = EigenVector(D, S[2]);
                if (S[0] - S[1] < epsilon)
                {
                    V1 = V2.unitOrthogonal();
                }
                else
                {
                    V1 = EigenVector(D, S[1]); V1 -= V2 * Vector3.Dot(V2, V1); V1 = Vector3.Normalize(V1);
                }
                V0 = Vector3.Cross(V1, V2);
            }

            V = Matrix4x4.identity;
            V.SetColumn(0, V0);
            V.SetColumn(1, V1);
            V.SetColumn(2, V2);
        }

        // D is symmetric, S is an eigen value
        static Vector3 EigenVector(Matrix4x4 D, float S)
        {
            // Compute a cofactor matrix of D - sI.
            Vector4 c0 = D.GetColumn(0); c0[0] -= S;
            Vector4 c1 = D.GetColumn(1); c1[1] -= S;
            Vector4 c2 = D.GetColumn(2); c2[2] -= S;

            // Use an upper triangle
            Vector3 c0p = new Vector3(c1[1] * c2[2] - c2[1] * c2[1], 0, 0);
            Vector3 c1p = new Vector3(c2[1] * c2[0] - c1[0] * c2[2], c0[0] * c2[2] - c2[0] * c2[0], 0);
            Vector3 c2p = new Vector3(c1[0] * c2[1] - c1[1] * c2[0], c1[0] * c2[0] - c0[0] * c2[1], c0[0] * c1[1] - c1[0] * c1[0]);

            // Get a column vector with a largest norm (non-zero).
            float C01s = c1p[0] * c1p[0];
            float C02s = c2p[0] * c2p[0];
            float C12s = c2p[1] * c2p[1];
            Vector3 norm = new Vector3(c0p[0] * c0p[0] + C01s + C02s,
                                       C01s + c1p[1] * c1p[1] + C12s,
                                       C02s + C12s + c2p[2] * c2p[2]);

            // index of largest:
            int index = 0;
            if (norm[0] > norm[1] && norm[0] > norm[2])
                index = 0;
            else if (norm[1] > norm[0] && norm[1] > norm[2])
                index = 1;
            else
                index = 2;

            Vector3 V = Vector3.zero;

            // special case
            if (norm[index] < epsilon)
            {
                V[0] = 1; return V;
            }
            else if (index == 0)
            {
                V[0] = c0p[0]; V[1] = c1p[0]; V[2] = c2p[0];
            }
            else if (index == 1)
            {
                V[0] = c1p[0]; V[1] = c1p[1]; V[2] = c2p[1];
            }
            else
            {
                V = c2p;
            }
            return Vector3.Normalize(V);
        }

        static Vector3 unitOrthogonal(this Vector3 input)
        {
            // Find a vector to cross() the input with.
            if (!(input.x < input.z * epsilon)
             || !(input.y < input.z * epsilon))
            {
                float invnm = 1 / Vector3.Magnitude(new Vector2(input.x, input.y));
                return new Vector3(-input.y * invnm, input.x * invnm, 0);
            }
            else
            {
                float invnm = 1 / Vector3.Magnitude(new Vector2(input.y, input.z));
                return new Vector3(0, -input.z * invnm, input.y * invnm);
            }
        }

        static Vector3 EigenValues(Matrix4x4 D)
        {
            float one_third = 1 / 3.0f;
            float one_sixth = 1 / 6.0f;
            float three_sqrt = Mathf.Sqrt(3.0f);

            Vector3 c0 = D.GetColumn(0);
            Vector3 c1 = D.GetColumn(1);
            Vector3 c2 = D.GetColumn(2);

            float m = one_third * (c0[0] + c1[1] + c2[2]);

            // K is D - I*diag(S)
            float K00 = c0[0] - m;
            float K11 = c1[1] - m;
            float K22 = c2[2] - m;

            float K01s = c1[0] * c1[0];
            float K02s = c2[0] * c2[0];
            float K12s = c2[1] * c2[1];

            float q = 0.5f * (K00 * (K11 * K22 - K12s) - K22 * K01s - K11 * K02s) + c1[0] * c2[1] * c0[2];
            float p = one_sixth * (K00 * K00 + K11 * K11 + K22 * K22 + 2 * (K01s + K02s + K12s));

            float p_sqrt = Mathf.Sqrt(p);

            float tmp = p * p * p - q * q;
            float phi = one_third * Mathf.Atan2(Mathf.Sqrt(Mathf.Max(0, tmp)), q);
            float phi_c = Mathf.Cos(phi);
            float phi_s = Mathf.Sin(phi);
            float sqrt_p_c_phi = p_sqrt * phi_c;
            float sqrt_p_3_s_phi = p_sqrt * three_sqrt * phi_s;

            float e0 = m + 2 * sqrt_p_c_phi;
            float e1 = m - sqrt_p_c_phi - sqrt_p_3_s_phi;
            float e2 = m - sqrt_p_c_phi + sqrt_p_3_s_phi;

            float aux;
            if (e0 > e1)
            {
                aux = e0;
                e0 = e1;
                e1 = aux;
            }
            if (e0 > e2)
            {
                aux = e0;
                e0 = e2;
                e2 = aux;
            }
            if (e1 > e2)
            {
                aux = e1;
                e1 = e2;
                e2 = aux;
            }

            return new Vector3(e2, e1, e0);
        }
    }
}
