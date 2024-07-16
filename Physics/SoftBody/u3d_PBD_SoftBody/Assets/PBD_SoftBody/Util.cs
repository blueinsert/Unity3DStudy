using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static void CopyArray(Vector3[] from, Vector3[] to)
    {
        for (int i = 0; i < from.Length; i++)
        {
            to[i] = from[i];
        }
    }
}
