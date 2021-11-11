using System;
using UnityEngine;

namespace RVO
{
    public static class RVOMath
    {
        public static float Sqr(float a)
        {
            return a * a;
        }

        public static float Det(Vector2 p, Vector2 q)
        {
            return p.x * q.y - p.y * q.x;
        }

        public static float AbsSq(Vector2 q)
        {
            return q.x * q.x + q.y * q.y;
        }
    }
}
