using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dest
{
    namespace Math
    {
        public static class Vector2Dex
        {
            internal class Information
            {
                // The intrinsic dimension of the input set.  The parameter 'epsilon'
                // to the GetInformation function is used to provide a tolerance when
                // determining the dimension.
                public int Dimension;

                // Axis-aligned bounding box of the input set.  The maximum range is
                // the larger of max[0]-min[0] and max[1]-min[1].
                public Vector2D Min;
                public Vector2D Max;
                public double MaxRange;

                // Coordinate system.  The origin is valid for any dimension d.  The
                // unit-length direction vector is valid only for 0 <= i < d.  The
                // extreme index is relative to the array of input points, and is also
                // valid only for 0 <= i < d.  If d = 0, all points are effectively
                // the same, but the use of an epsilon may lead to an extreme index
                // that is not zero.  If d = 1, all points effectively lie on a line
                // segment.  If d = 2, the points are not collinear.
                public Vector2D Origin;
                public Vector2D[] Direction = new Vector2D[2];

                // The indices that define the maximum dimensional extents.  The
                // values mExtreme[0] and mExtreme[1] are the indices for the points
                // that define the largest extent in one of the coordinate axis
                // directions.  If the dimension is 2, then mExtreme[2] is the index
                // for the point that generates the largest extent in the direction
                // perpendicular to the line through the points corresponding to
                // mExtreme[0] and mExtreme[1].  The triangle formed by the points
                // V[extreme0], V[extreme1], and V[extreme2] is clockwise or
                // counterclockwise, the condition stored in mExtremeCCW.
                public int[] Extreme = new int[3];
                public bool ExtremeCCW;
            }

            public static readonly Vector2D Zero = new Vector2D(0.0f, 0.0f);
            public static readonly Vector2D One = new Vector2D(1.0f, 1.0f);
            public static readonly Vector2D UnitX = new Vector2D(1.0f, 0.0f);
            public static readonly Vector2D UnitY = new Vector2D(0.0f, 1.0f);
            public static readonly Vector2D PositiveInfinity = new Vector2D(double.PositiveInfinity, double.PositiveInfinity);
            public static readonly Vector2D NegativeInfinity = new Vector2D(double.NegativeInfinity, double.NegativeInfinity);

            internal static Information GetInformation(IList<Vector2D> points, double epsilon)
            {
                if (points == null) return null;
                int numPoints = points.Count;
                if (numPoints == 0 || epsilon < 0f) return null;

                Information info = new Information();
                info.ExtremeCCW = false;

                double temp;

                // Compute the axis-aligned bounding box for the input points.  Keep track
                // of the indices into 'points' for the current min and max.

                double minX = temp = points[0].x;
                double maxX = temp;
                int minXIndex = 0;
                int maxXIndex = 0;

                double minY = temp = points[0].y;
                double maxY = temp;
                int minYIndex = 0;
                int maxYIndex = 0;

                for (int i = 1; i < numPoints; ++i)
                {
                    temp = points[i].x;
                    if (temp < minX)
                    {
                        minX = temp;
                        minXIndex = i;
                    }
                    else if (temp > maxX)
                    {
                        maxX = temp;
                        maxXIndex = i;
                    }

                    temp = points[i].y;
                    if (temp < minY)
                    {
                        minY = temp;
                        minYIndex = i;
                    }
                    else if (temp > maxY)
                    {
                        maxY = temp;
                        maxYIndex = i;
                    }
                }

                info.Min.x = minX;
                info.Min.y = minY;
                info.Max.x = maxX;
                info.Max.y = maxY;

                // Determine the maximum range for the bounding box.
                info.MaxRange = maxX - minX;
                info.Extreme[0] = minXIndex;
                info.Extreme[1] = maxXIndex;
                double range = maxY - minY;
                if (range > info.MaxRange)
                {
                    info.MaxRange = range;
                    info.Extreme[0] = minYIndex;
                    info.Extreme[1] = maxYIndex;
                }

                // The origin is either the point of minimum x-value or point of minimum y-value.
                info.Origin = points[info.Extreme[0]];

                // Test whether the point set is (nearly) a point.
                if (info.MaxRange < epsilon)
                {
                    info.Dimension = 0;
                    info.Extreme[1] = info.Extreme[0];
                    info.Extreme[2] = info.Extreme[0];
                    info.Direction[0] = Vector2Dex.Zero;
                    info.Direction[1] = Vector2Dex.Zero;
                    return info;
                }

                // Test whether the point set is (nearly) a line segment.
                info.Direction[0] = points[info.Extreme[1]] - info.Origin;
                info.Direction[0].Normalize();
                info.Direction[1] = -info.Direction[0].Perp();
                double maxDistance = 0f;
                double maxSign = 0f;
                info.Extreme[2] = info.Extreme[0];
                for (int i = 0; i < numPoints; ++i)
                {
                    Vector2D diff = points[i] - info.Origin;
                    double distance = info.Direction[1].Dot(diff);
                    double sign = System.Math.Sign(distance);
                    distance = System.Math.Abs(distance);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        maxSign = sign;
                        info.Extreme[2] = i;
                    }
                }

                if (maxDistance < epsilon * info.MaxRange)
                {
                    info.Dimension = 1;
                    info.Extreme[2] = info.Extreme[1];
                    return info;
                }

                info.Dimension = 2;
                info.ExtremeCCW = maxSign > 0f;

                return info;
            }

            /// <summary>
            /// Returns vector length
            /// </summary>
            public static double Length(this Vector2D vector)
            {
                return System.Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
            }

            /// <summary>
            /// Returns vector squared length
            /// </summary>
            public static double LengthSqr(this Vector2D vector)
            {
                return vector.x * vector.x + vector.y * vector.y;
            }

            /// <summary>
            /// Returns x0*y1 - y0*x1
            /// </summary>
            public static double DotPerp(this Vector2D vector, Vector2D value)
            {
                return vector.x * value.y - vector.y * value.x;
            }

            /// <summary>
            /// Returns x0*y1 - y0*x1
            /// </summary>
            public static double DotPerp(this Vector2D vector, ref Vector2D value)
            {
                return vector.x * value.y - vector.y * value.x;
            }

            /// <summary>
            /// Returns x0*y1 - y0*x1
            /// </summary>
            public static double DotPerp(ref Vector2D vector, ref Vector2D value)
            {
                return vector.x * value.y - vector.y * value.x;
            }

            /// <summary>
            /// Vector dot product
            /// </summary>
            public static double Dot(this Vector2D vector, Vector2D value)
            {
                return vector.x * value.x + vector.y * value.y;
            }

            /// <summary>
            /// Vector dot product
            /// </summary>
            public static double Dot(this Vector2D vector, ref Vector2D value)
            {
                return vector.x * value.x + vector.y * value.y;
            }

            /// <summary>
            /// Vector dot product
            /// </summary>
            public static double Dot(ref Vector2D vector, ref Vector2D value)
            {
                return vector.x * value.x + vector.y * value.y;
            }

            /// <summary>
            /// Returns (y,-x)
            /// </summary>
            public static Vector2D Perp(this Vector2D vector)
            {
                return new Vector2D(vector.y, -vector.x);
            }

            /// <summary>
            /// Returns angle in degrees between this vector and the target vector. Method normalizes input vectors. Result lies in [0..180] range.
            /// </summary>
            public static double AngleDeg(this Vector2D vector, Vector2D target)
            {
                Normalize(ref vector);
                Normalize(ref target);
                double dot = vector.x * target.x + vector.y * target.y;
                if (dot > 1f) dot = 1f; else if (dot < -1f) dot = -1f;
                return System.Math.Acos(dot) * Mathex.Rad2Deg;
            }

            /// <summary>
            /// Returns angle in radians between this vector and the target vector. Method normalizes input vectors. Result lies in [0..PI] range.
            /// </summary>
            public static double AngleRad(this Vector2D vector, Vector2D target)
            {
                Normalize(ref vector);
                Normalize(ref target);
                double dot = vector.x * target.x + vector.y * target.y;
                if (dot > 1f) dot = 1f; else if (dot < -1f) dot = -1f;
                return System.Math.Acos(dot);
            }

            /// <summary>
            /// Normalizes given vector and returns it's length before normalization.
            /// </summary>
            public static double Normalize(ref Vector2D vector, double epsilon = Mathex.ZeroTolerance)
            {
                double length = System.Math.Sqrt(vector.x * vector.x + vector.y * vector.y);

                if (length >= epsilon)
                {
                    double invLength = 1f / length;
                    vector.x *= invLength;
                    vector.y *= invLength;
                    return length;
                }

                vector.x = 0f;
                vector.y = 0f;
                return 0f;
            }

            /// <summary>
            /// Sets vector length to the given value. Returns new vector length or 0 if vector's initial length is less than epsilon.
            /// </summary>
            public static double SetLength(ref Vector2D vector, double lengthValue, double epsilon = Mathex.ZeroTolerance)
            {
                double length = System.Math.Sqrt(vector.x * vector.x + vector.y * vector.y);

                if (length >= epsilon)
                {
                    double invLength = lengthValue / length;
                    vector.x *= invLength;
                    vector.y *= invLength;
                    return lengthValue;
                }

                vector.x = 0f;
                vector.y = 0f;
                return 0f;
            }

            /// <summary>
            /// Changes vector length adding given value. Returns new vector length or 0 if vector's initial length is less than epsilon.
            /// </summary>
            public static double GrowLength(ref Vector2D vector, double lengthDelta, double epsilon = Mathex.ZeroTolerance)
            {
                double length = System.Math.Sqrt(vector.x * vector.x + vector.y * vector.y);

                if (length >= epsilon)
                {
                    double newLength = length + lengthDelta;
                    double invLength = newLength / length;
                    vector.x *= invLength;
                    vector.y *= invLength;
                    return newLength;
                }

                vector.x = 0f;
                vector.y = 0f;
                return 0f;
            }

            /// <summary>
            /// Creates a vector with all components equal to value
            /// </summary>
            public static Vector2D Replicate(double value)
            {
                return new Vector2D(value, value);
            }

            /// <summary>
            /// Converts Vector2D to Vector3D, copying x and y components of Vector2D to x and y components of Vector3D respectively.
            /// </summary>
            public static Vector3D ToVector3XY(this Vector2D vector)
            {
                return new Vector3D(vector.x, vector.y, 0.0f);
            }

            /// <summary>
            /// Converts Vector2D to Vector3D, copying x and y components of Vector2D to x and z components of Vector3D respectively.
            /// </summary>
            public static Vector3D ToVector3XZ(this Vector2D vector)
            {
                return new Vector3D(vector.x, 0.0f, vector.y);
            }

            /// <summary>
            /// Converts Vector2D to Vector3D, copying x and y components of Vector2D to y and z components of Vector3D respectively.
            /// </summary>
            public static Vector3D ToVector3YZ(this Vector2D vector)
            {
                return new Vector3D(0.0f, vector.x, vector.y);
            }

            /// <summary>
            /// Returns string representation (does not round components as standard Vector2D.ToString() does)
            /// </summary>
            public static string ToStringEx(this Vector2D vector)
            {
                return string.Format("({0}, {1})", vector.x.ToString(), vector.y.ToString());
            }
        }
    }
}
