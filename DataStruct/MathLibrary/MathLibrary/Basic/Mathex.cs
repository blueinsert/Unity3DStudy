using System;
using System.Collections.Generic;

namespace Dest
{
    namespace Math
    {
        public struct Mathex
        {
            /// <summary>
            /// Rad2Deg
            /// </summary>
            public const double Rad2Deg = 57.29578f;
            /// <summary>
            /// Deg2Rad
            /// </summary>
            public const double Deg2Rad = 0.0174533f;

            /// <summary>
            /// 1e-5f
            /// </summary>
            public const double ZeroTolerance = 1e-5;

            /// <summary>
            /// -1e-5f
            /// </summary>
            public const double NegativeZeroTolerance = -ZeroTolerance;

            /// <summary>
            /// (1e-5f)^2
            /// </summary>
            public const double ZeroToleranceSqr = ZeroTolerance * ZeroTolerance;

            /// <summary>
            /// π
            /// </summary>
            public const double Pi = System.Math.PI;

            /// <summary>
            /// π/2
            /// </summary>
            public const double HalfPi = 0.5 * Pi;

            /// <summary>
            /// 2*π
            /// </summary>
            public const double TwoPi = 2 * Pi;

            /// <summary>
            /// Ln9999
            /// </summary>
            public const double Ln9999 = 9.21024036697585;

            /// <summary>
            /// Evaluates x^2
            /// </summary>
            public static double EvalSquared(double x)
            {
                return x * x;
            }

            /// <summary>
            /// Evaluates x^(1/2)
            /// </summary>
            public static double EvalInvSquared(double x)
            {
                return System.Math.Sqrt(x);
            }

            /// <summary>
            /// Evaluates x^3
            /// </summary>
            public static double EvalCubic(double x)
            {
                return x * x * x;
            }

            /// <summary>
            /// Evaluates x^(1/3)
            /// </summary>
            public static double EvalInvCubic(double x)
            {
                return System.Math.Pow(x, 1f / 3f);
            }

            /// <summary>
            /// Evaluates quadratic equation a*x^2 + b*x + c
            /// </summary>
            public static double EvalQuadratic(double x, double a, double b, double c)
            {
                return a * x * x + b * x + c;
            }

            /// <summary>
            /// Evaluates sigmoid function (used for smoothing values).
            /// Formula: x^2 * (3 - 2*x)
            /// </summary>
            public static double EvalSigmoid(double x)
            {
                return x * x * (3f - 2f * x);
            }

            /// <summary>
            /// Evaluates overlapped step function. Useful for animating several objects
            /// (stepIndex parameter is number of the objects), where animations follow one after
            /// another with some overlapping in time (overlap parameter).
            /// </summary>
            /// <param name="x">Evaluation parameter, makes sence in [0,1] range</param>
            /// <param name="overlap">Overlapping between animations (must be greater or equal to zero),
            /// where 0 means that animations do not overlap and follow one after another.</param>
            /// <param name="objectIndex">Index of object beeing animated</param>
            /// <param name="objectCount">Number of objects beeing animated</param>
            public static double EvalOverlappedStep(double x, double overlap, int objectIndex, int objectCount)
            {
                double result = (x - (1f - overlap) * objectIndex / (objectCount - 1f)) / overlap;
                if (result < 0f)
                {
                    result = 0f;
                }
                else if (result > 1f)
                {
                    result = 1f;
                }
                return result;
            }

            /// <summary>
            /// Evaluates overlapped step function and applies sigmoid to smooth the result. Useful for animating several objects
            /// (stepIndex parameter is number of the objects), where animations follow one after
            /// another with some overlapping in time (overlap parameter).
            /// </summary>
            /// <param name="x">Evaluation parameter, makes sence in [0,1] range</param>
            /// <param name="overlap">Overlapping between animations (must be greater or equal to zero),
            /// where 0 means that animations do not overlap and follow one after another.</param>
            /// <param name="objectIndex">Index of object beeing animated</param>
            /// <param name="objectCount">Number of objects beeing animated</param>
            public static double EvalSmoothOverlappedStep(double x, double overlap, int objectIndex, int objectCount)
            {
                double result = (x - (1f - overlap) * objectIndex / (objectCount - 1f)) / overlap;
                if (result < 0f)
                {
                    result = 0f;
                }
                else if (result > 1f)
                {
                    result = 1f;
                }
                return result * result * (3f - 2f * result);
            }

            /// <summary>
            /// Evaluates scalar gaussian function. The formula is:
            /// a * e^(-(x-b)^2 / 2*c^2)
            /// </summary>
            /// <param name="x">Function parameter</param>
            public static double EvalGaussian(double x, double a, double b, double c)
            {
                double x_min_b = x - b;
                return a * System.Math.Exp(x_min_b * x_min_b / (-2f * c * c));
            }

            /// <summary>
            /// Evaluates 2-dimensional gaussian function. The formula is:
            /// A * e^(-(a*(x - x0)^2 + 2*b*(x - x0)*(y - y0) + c*(y - y0)^2))
            /// </summary>
            /// <param name="x">First function parameter</param>
            /// <param name="y">Second function parameter</param>
            public static double EvalGaussian2D(double x, double y, double x0, double y0, double A, double a, double b, double c)
            {
                double x_min_x0 = x - x0;
                double y_min_y0 = y - y0;
                return A * System.Math.Exp(-(a * x_min_x0 * x_min_x0 + 2f * b * x_min_x0 * y_min_y0 + c * y_min_y0 * y_min_y0));
            }


            /// <summary>
            /// Linearly interpolates between 'value0' and 'value1'.
            /// </summary>
            /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
            public static double Lerp(double value0, double value1, double factor)
            {
                if (factor < 0f) factor = 0f; else if (factor > 1f) factor = 1f;
                return value0 + (value1 - value0) * factor;
            }

            /// <summary>
            /// Linearly interpolates between 'value0' and 'value1'.
            /// </summary>
            /// <param name="factor">Interpolation factor in range [0..1] (will NOT be clamped, i.e. interpolation can overshoot)</param>
            public static double LerpUnclamped(double value0, double value1, double factor)
            {
                return value0 + (value1 - value0) * factor;
            }

            /// <summary>
            /// Interpolates between 'value0' and 'value1' using sigmoid as interpolation function.
            /// </summary>
            /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
            public static double SigmoidInterp(double value0, double value1, double factor)
            {
                if (factor < 0f) factor = 0f; else if (factor > 1f) factor = 1f;
                if (factor < 0.00001)
                {
                    return value0;
                }
                else if (factor > 0.99999)
                {
                    return value1;
                }
                else
                {
                    // 将factor从[0, 1]映射到[-ln99999, ln99999]
                    factor = 2 * Ln99999 * factor - Ln99999;

                    var sigmoidValue = 1.0 / (1.0 + System.Math.Pow(System.Math.E, -factor));
                    return value0 + (value1 - value0) * sigmoidValue;
                }
            }
            private const double Ln99999 = 11.512915465;

            /// <summary>
            /// Interpolates between 'value0' and 'value1' using sine function easing at the end.
            /// </summary>
            /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
            public static double SinInterp(double value0, double value1, double factor)
            {
                if (factor < 0f) factor = 0f; else if (factor > 1f) factor = 1f;
                factor = System.Math.Sin(factor * Mathex.HalfPi);
                return value0 + (value1 - value0) * factor;
            }

            /// <summary>
            /// Interpolates between 'value0' and 'value1' using cosine function easing in the start.
            /// </summary>
            /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
            public static double CosInterp(double value0, double value1, double factor)
            {
                if (factor < 0f) factor = 0f; else if (factor > 1f) factor = 1f;
                factor = 1.0f - System.Math.Cos(factor * Mathex.HalfPi);
                return value0 + (value1 - value0) * factor;
            }

            /// <summary>
            /// Interpolates between 'value0' and 'value1' in using special function which overshoots first, then waves back and forth gradually declining towards the end.
            /// </summary>
            /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
            public static double WobbleInterp(double value0, double value1, double factor)
            {
                if (factor < 0f) factor = 0f; else if (factor > 1f) factor = 1f;
                factor = (System.Math.Sin(factor * System.Math.PI * (0.2f + 2.5f * factor * factor * factor)) * System.Math.Pow(1f - factor, 2.2f) + factor) * (1f + (1.2f * (1f - factor)));
                return value0 + (value1 - value0) * factor;
            }

            ///// <summary>
            ///// Interpolates between 'value0' and 'value1' using provided animation curve (curve will be sampled in [0..1] range]).
            ///// </summary>
            ///// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
            //public static double CurveInterp(double value0, double value1, double factor, AnimationCurve curve)
            //{
            //    if (factor < 0f) factor = 0f; else if (factor > 1f) factor = 1f;
            //    factor = curve.Evaluate(factor);
            //    return value0 + (value1 - value0) * factor;
            //}

            /// <summary>
            /// Interpolates between 'value0' and 'value1' using provided function (function will be sampled in [0..1] range]).
            /// </summary>
            /// <param name="factor">Interpolation factor in range [0..1] (will be clamped)</param>
            public static double FuncInterp(double value0, double value1, double factor, System.Func<double, double> func)
            {
                if (factor < 0f) factor = 0f; else if (factor > 1f) factor = 1f;
                double t = func(factor);
                return value0 * (1f - t) + value1 * t;
            }


            /// <summary>
            /// Returns 1/Sqrt(value) if value != 0, otherwise returns 0.
            /// </summary>
            public static double InvSqrt(double value)
            {
                if (value != 0.0f)
                {
                    return 1f / System.Math.Sqrt(value);
                }
                else
                {
                    return 0.0f;
                }
            }

            /// <summary>
            /// Returns abs(v0-v1)&lt;eps
            /// </summary>
            public static bool Near(double value0, double value1, double epsilon = Mathex.ZeroTolerance)
            {
                return System.Math.Abs(value0 - value1) < epsilon;
            }

            /// <summary>
            /// Returns abs(v)&lt;eps
            /// </summary>
            public static bool NearZero(double value, double epsilon = Mathex.ZeroTolerance)
            {
                return System.Math.Abs(value) < epsilon;
            }


            /// <summary>
            /// Converts cartesian coordinates to polar coordinates.
            /// Resulting vector contains rho (length) in x coordinate and phi (angle) in y coordinate; rho >= 0, 0 &lt;= phi &lt; 2pi.
            /// If cartesian coordinates are (0,0) resulting coordinates are (0,0).
            /// </summary>
            public static Vector2D CartesianToPolar(Vector2D cartesianCoordinates)
            {
                double x = cartesianCoordinates.x;
                double y = cartesianCoordinates.y;
                Vector2D result;

                result.x = System.Math.Sqrt(x * x + y * y);

                if (x > 0f)
                {
                    if (y >= 0)
                    {
                        result.y = System.Math.Atan(y / x);
                    }
                    else // y < 0
                    {
                        result.y = System.Math.Atan(y / x) + TwoPi;
                    }
                }
                else if (x < 0f)
                {
                    result.y = System.Math.Atan(y / x) + Pi;
                }
                else // x == 0
                {
                    if (y > 0f)
                    {
                        result.y = HalfPi;
                    }
                    else if (y < 0f)
                    {
                        result.y = HalfPi + Pi;
                    }
                    else // y == 0
                    {
                        result.x = 0f;
                        result.y = 0f;
                    }
                }

                return result;
            }

            /// <summary>
            /// Converts polar coordinates to cartesian coordinates.
            /// Input vector contains rho (length) in x coordinate and phi (angle) in y coordinate; rho >= 0, 0 &lt;= phi &lt; 2pi.
            /// </summary>
            public static Vector2D PolarToCartesian(Vector2D polarCoordinates)
            {
                Vector2D result;
                result.x = polarCoordinates.x * System.Math.Cos(polarCoordinates.y);
                result.y = polarCoordinates.x * System.Math.Sin(polarCoordinates.y);
                return result;
            }

            /// <summary>
            /// Converts cartesian coordinates to spherical coordinates.
            /// Resulting vector contains rho (length) in x coordinate, theta (azimutal angle in XZ plane from X axis) in y coordinate,
            /// phi (zenith angle from positive Y axis) in z coordinate; rho >= 0, 0 &lt;= theta &lt; 2pi, 0 &lt;= phi &lt; pi.
            /// If cartesian coordinates are (0,0,0) resulting coordinates are (0,0,0).
            /// </summary>
            /// <param name="cartesianCoordinates"></param>
            /// <returns></returns>
            public static Vector3D CartesianToSpherical(Vector3D cartesianCoordinates)
            {
                double x = cartesianCoordinates.x;
                double y = cartesianCoordinates.y;
                double z = cartesianCoordinates.z;

                double rho = System.Math.Sqrt(x * x + y * y + z * z);
                double theta;
                double phi;

                if (rho != 0f)
                {
                    phi = System.Math.Acos(y / rho);

                    if (x > 0f)
                    {
                        if (z >= 0)
                        {
                            theta = System.Math.Atan(z / x);
                        }
                        else // z < 0
                        {
                            theta = System.Math.Atan(z / x) + TwoPi;
                        }
                    }
                    else if (x < 0f)
                    {
                        theta = System.Math.Atan(z / x) + Pi;
                    }
                    else // x == 0
                    {
                        if (z > 0f)
                        {
                            theta = HalfPi;
                        }
                        else if (z < 0f)
                        {
                            theta = HalfPi + Pi;
                        }
                        else // z == 0
                        {
                            theta = 0f;
                        }
                    }
                }
                else
                {
                    rho = 0f;
                    theta = 0f;
                    phi = 0f;
                }

                Vector3D result;
                result.x = rho;
                result.y = theta;
                result.z = phi;

                return result;
            }

            /// <summary>
            /// Converts spherical coordinates to cartesian coordinates.
            /// Input vector contains rho (length) in x coordinate, theta (azimutal angle in XZ plane from X axis) in y coordinate,
            /// phi (zenith angle from positive Y axis) in z coordinate; rho >= 0, 0 &lt;= theta &lt; 2pi, 0 &lt;= phi &lt; pi.
            /// </summary>
            /// <param name="sphericalCoordinates"></param>
            /// <returns></returns>
            public static Vector3D SphericalToCartesian(Vector3D sphericalCoordinates)
            {
                double rho = sphericalCoordinates.x;
                double theta = sphericalCoordinates.y;
                double phi = sphericalCoordinates.z;

                double sinPhi = System.Math.Sin(phi);

                Vector3D result;
                result.x = rho * System.Math.Cos(theta) * sinPhi;
                result.y = rho * System.Math.Cos(phi);
                result.z = rho * System.Math.Sin(theta) * sinPhi;

                return result;
            }

            /// <summary>
            /// Converts cartesian coordinates to cylindrical coordinates.
            /// Resulting vector contains rho (length) in x coordinate, phi (polar angle in XZ plane) in y coordinate,
            /// height (height from XZ plane to the point) in z coordinate.
            /// </summary>
            public static Vector3D CartesianToCylindrical(Vector3D cartesianCoordinates)
            {
                double x = cartesianCoordinates.x;
                double z = cartesianCoordinates.z;

                double rho = System.Math.Sqrt(x * x + z * z);
                double phi;

                if (x > 0f)
                {
                    if (z >= 0)
                    {
                        phi = System.Math.Atan(z / x);
                    }
                    else // z < 0
                    {
                        phi = System.Math.Atan(z / x) + TwoPi;
                    }
                }
                else if (x < 0f)
                {
                    phi = System.Math.Atan(z / x) + Pi;
                }
                else // x == 0
                {
                    if (z > 0f)
                    {
                        phi = HalfPi;
                    }
                    else if (z < 0f)
                    {
                        phi = HalfPi + Pi;
                    }
                    else // z == 0
                    {
                        phi = 0;
                    }
                }

                Vector3D result;
                result.x = rho;
                result.y = phi;
                result.z = cartesianCoordinates.y;

                return result;
            }

            /// <summary>
            /// Converts cylindrical coordinates to cartesian coordinates.
            /// Input vector contains rho (length) in x coordinate, phi (polar angle in XZ plane) in y coordinate,
            /// height (height from XZ plane to the point) in z coordinate.
            /// </summary>
            public static Vector3D CylindricalToCartesian(Vector3D cylindricalCoordinates)
            {
                Vector3D result;
                result.x = cylindricalCoordinates.x * System.Math.Cos(cylindricalCoordinates.y);
                result.y = cylindricalCoordinates.z;
                result.z = cylindricalCoordinates.x * System.Math.Sin(cylindricalCoordinates.y);
                return result;
            }
        }
    }
}
