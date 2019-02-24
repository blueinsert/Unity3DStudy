using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class GLHelper
    {
        public static void DrawCircle(Vector2 pos, float radius, int segment, Color color)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(color);
            for (int i = 0; i < segment; i++)
            {
                float angle1 = i / (float)segment * 360f;
                float angle2 = ((i + 1) == segment ? 0 : i + 1) / (float)segment * 360f;
                var pos1 = new Vector2(Mathf.Cos(angle1 * Mathf.Deg2Rad), Mathf.Sin(angle1 * Mathf.Deg2Rad)) * radius + pos;
                var pos2 = new Vector2(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad)) * radius + pos;
                GL.Vertex(pos);
                GL.Vertex(pos1);
                GL.Vertex(pos2);
            }
            GL.End();
        }

        public static void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        public static void DrawGridLines(Rect rect, int gridSize, Color gridColor)
        {
            GL.Color(gridColor);

            for (int x = (int)rect.xMin; x < (int)rect.xMax; x++)
            {
                if (x % gridSize == 0)
                {
                    DrawLine(new Vector2(x, rect.yMin), new Vector2(x, rect.yMax));
                }
            }
            for (int y = (int)rect.yMin; y < (int)rect.yMax; y++)
            {
                if (y % gridSize == 0)
                {
                    DrawLine(new Vector2(rect.xMin, y), new Vector2(rect.xMax, y));
                }
            }
        }

        public static void DrawGrid(int gridSize, Rect rect)
        {
            GL.Begin(GL.LINES);

            DrawGridLines(rect, gridSize, new Color(0f, 0f, 0f, 0.28f));
            DrawGridLines(rect, gridSize/10, new Color(0f, 0f, 0f, 0.18f));

            GL.End();
        }

    }
}