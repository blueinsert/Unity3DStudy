using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extends
{
   public static Vector3 Rotate(this Vector3 u, float angle)
    {
        float x, y;
        x = u.x * Mathf.Cos(Mathf.Deg2Rad * angle) - u.y * Mathf.Sin(Mathf.Deg2Rad * angle);
        y = u.x * Mathf.Sin(Mathf.Deg2Rad * angle) + u.y * Mathf.Cos(Mathf.Deg2Rad * angle);
        return new Vector3(x, y, 0);
    }
}
