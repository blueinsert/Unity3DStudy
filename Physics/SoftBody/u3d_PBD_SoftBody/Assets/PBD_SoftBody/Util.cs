using System;
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

public struct Vector4Int
{
    public int x; public int y; public int z; public int w;
    public Vector4Int(int x, int y, int z, int w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public int this[int index]
    {
        get
        {
            // 检查索引是否越界  
            if (index < 0 || index >= 4)
            {
                throw new IndexOutOfRangeException("Index was out of range.");
            }
            switch (index)
            {
                case 0: return x;
                case 1: return y;
                case 2: return z;
                case 3: return w;
            }
            return -1;
        }
        set
        {
            // 检查索引是否越界  
            if (index < 0 || index >= 4)
            {
                throw new IndexOutOfRangeException("Index was out of range.");
            }
            switch (index)
            {
                case 0: x = value; break;
                case 1: y = value; break;
                case 2: z = value; break;
                case 3: w = value; break;
            }
        }
    }
}
