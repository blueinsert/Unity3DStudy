using Dest.Math;
using System.Collections.Generic;

namespace Dest.Math
{
    public class Circle
    {
        /// <summary>
        /// 计算圆上p的切点
        /// </summary>
        // http://stackoverflow.com/questions/1351746/find-a-tangent-point-on-circle
        public static bool FindCircleNearestTangentPoint(Vector2D circleCenter, double circleRadius, Vector2D p, bool clockwise, out Vector2D tp)
        {
            Vector2D d = circleCenter - p;
            double dlen = System.Math.Sqrt(d.x * d.x + d.y * d.y);

            if (dlen < circleRadius)
            {
                tp = Vector2D.zero;
                return false;
            }
			
			if (System.Math.Abs(dlen - circleRadius) < Vector2D.kEpsilon)
			{
				tp = p;
				return true;
			}
			
            double a = System.Math.Asin(circleRadius / dlen);
            double b = System.Math.Atan2(d.y, d.x);

            if (clockwise)
            {
                double t = b + a;
                tp.x = circleCenter.x + circleRadius * -System.Math.Sin(t);
                tp.y = circleCenter.y + circleRadius * System.Math.Cos(t);
            }
            else
            {
                double t = b - a;
                tp.x = circleCenter.x + circleRadius * System.Math.Sin(t);
                tp.y = circleCenter.y + circleRadius * -System.Math.Cos(t);
            }

            return true;
        }

        /// <summary>
        /// 计算圆上离p最近的点
        /// </summary>
        public static Vector2D FindCircleNearestPoint(Vector2D circleCenter, double circleRadius, Vector2D p)
        {
            Vector2D d = circleCenter - p;
            double dlen = System.Math.Sqrt(d.x * d.x + d.y * d.y);
            if (dlen > 0)
                return circleCenter - d * (circleRadius / dlen);
            else
                return circleCenter + new Vector2D(circleRadius, 0);
        }
    }
}