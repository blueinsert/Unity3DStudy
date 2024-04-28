using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhysicUtil
{
    public static bool IsOverlap(Vector3 p,Collider collider,out Vector3 nearestPoint,out Vector3 normal)
    {
        nearestPoint = Vector3.zero;
        normal = Vector3.zero;
        if (collider is SphereCollider)
        {
            var sphereC = collider as SphereCollider;
            var c = collider.transform.position;
            var r = sphereC.radius;
            var x = p - c;
            var d = x.magnitude;
            var dir = x.normalized;
            if (d < r)
            {
                normal = dir;
                nearestPoint = c + dir * r;
                return true;
            }
            return false;
        }
        Debug.LogError(string.Format("the collider {0} is not supported!", collider));
        return false;
    }
}
