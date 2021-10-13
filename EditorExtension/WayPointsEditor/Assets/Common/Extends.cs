using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public static class Extends
    {
        public static Vector2 Rotate(this Vector2 u, float angle)
        {
            float x, y;
            x = u.x * Mathf.Cos(Mathf.Deg2Rad * angle) - u.y * Mathf.Sin(Mathf.Deg2Rad * angle);
            y = u.x * Mathf.Sin(Mathf.Deg2Rad * angle) + u.y * Mathf.Cos(Mathf.Deg2Rad * angle);
            return new Vector3(x, y, 0);
        }

        public static Vector2 GetNormal(this Vector2 v)
        {
            var normal = Vector3.Cross(new Vector3(v.x, v.y, 0), new Vector3(0, 0, 1)).normalized;
            return new Vector2(normal.x, normal.y);
        }
    }
}
