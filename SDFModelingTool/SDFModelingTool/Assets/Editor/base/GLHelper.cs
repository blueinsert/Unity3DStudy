using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class GLHelper
    {
        public static void DrawCircleSolid(Vector2 pos, float radius, int segment, Color color)
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

        public static void DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.End();
        }

        private static float ArrowLen = 2f;

        public static void DrawSingleArrowLine(Vector2 start, Vector2 end, Color color)
        {
            DrawLine(start, end, color);
            var middle = (start + end) / 2f;
            var n = (start - end).normalized;
            var dir1 = n.Rotate(30);
            var dir2 = n.Rotate(-39);
            var p1 = middle - dir1 * ArrowLen;
            var p2 = middle - dir2 * ArrowLen;
            DrawLine(p1, middle, color);
            DrawLine(p2, middle, color);
        }

        public static void DrawDoubleArrowLine(Vector2 start, Vector2 end, Color color)
        {
            DrawLine(start, end, color);
            var n = (start - end).normalized;
            var dir1 = n.Rotate(30);
            var dir2 = n.Rotate(-39);
            var base1 = start + (end - start) / 3;
            var base2 = end - (end - start) / 3;
            DrawLine(base1, base1 - dir1 * ArrowLen, color);
            DrawLine(base1, base1 - dir2 * ArrowLen, color);
            DrawLine(base2, base2 + dir1 * ArrowLen, color);
            DrawLine(base2, base2 + dir2 * ArrowLen, color);
        }

        public static void DrawGridLines(Rect rect, int gridSize, Color gridColor)
        {
            for (int x = (int)rect.xMin; x < (int)rect.xMax; x++)
            {
                if (x % gridSize == 0)
                {
                    DrawLine(new Vector2(x, rect.yMin), new Vector2(x, rect.yMax), gridColor);
                }
            }
            for (int y = (int)rect.yMin; y < (int)rect.yMax; y++)
            {
                if (y % gridSize == 0)
                {
                    DrawLine(new Vector2(rect.xMin, y), new Vector2(rect.xMax, y), gridColor);
                }
            }
        }

        public static void DrawMainAxis(Rect rect, float width, Color color)
        {
            if (rect.xMin < 0 && rect.xMax > 0)
            { 
                //y axis
                DrawRectSolid(new Vector2(0, rect.center.y), width, rect.height, color);
            }
            if (rect.yMin < 0 && rect.yMax > 0)
            {
                //x axis
                DrawRectSolid(new Vector2(rect.center.x, 0), rect.width, width, color);
            }
        }

        public static void DrawGrid(int gridSize, Rect rect, Color color)
        {
            DrawGridLines(rect, gridSize, new Color(0f, 0f, 0f, 0.28f));
            DrawGridLines(rect, gridSize / 10, new Color(0f, 0f, 0f, 0.18f));
            DrawMainAxis(rect, gridSize*0.05f, new Color(0f, 0f, 0f, 0.28f));
        }

        public static void DrawSolidTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(color);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.Vertex(p3);
            GL.End();
        }

        public static void DrawRectSolid(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            GL.Begin(GL.QUADS);
            GL.Color(color);
            GL.TexCoord2(1, 1); GL.Vertex(p1);
            GL.TexCoord2(0, 1); GL.Vertex(p2);
            GL.TexCoord2(0, 0); GL.Vertex(p3);
            GL.TexCoord2(1, 0); GL.Vertex(p4); 
            GL.End();
        }

        public static void DrawRectSolid(Vector2 center, float width, float height, Color color)
        {
            DrawRectSolid(center + new Vector2(width / 2, height / 2), center + new Vector2(-width / 2, height / 2),
                center + new Vector2(-width / 2, -height / 2), center + new Vector2(width / 2, -height / 2), color);
        }

        public static void DrawRectSolid(Vector2 point1, Vector2 point2, Color color)
        {
            var center = (point1 + point2) / 2;
            var width = Mathf.Abs(point1.x - point2.x);
            var height = Mathf.Abs(point1.y - point2.y);
            DrawRectSolid(center, width, height, color);
        }

        public static void DrawRect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Color color)
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            GL.Vertex(p1);
            GL.Vertex(p2);
            GL.Vertex(p3);
            GL.Vertex(p4);
            GL.Vertex(p1);
            GL.End();
        }

        public static void DrawRect(Vector2 center, float width, float height, Color color)
        {
            DrawRect(center + new Vector2(width / 2, height / 2), center + new Vector2(-width / 2, height / 2),
                center + new Vector2(-width / 2, -height / 2), center + new Vector2(width / 2, -height / 2), color);
        }

        public static void DrawRect(Vector2 point1, Vector2 point2, Color color)
        {
            var center = (point1 + point2) / 2;
            var width = Mathf.Abs(point1.x - point2.x);
            var height = Mathf.Abs(point1.y - point2.y);
            DrawRect(center, width, height, color);
        }

        /// <summary>
        /// 在rect中绘制字体，居中对齐
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="characterInfo"></param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        public static void DrawTextQuads(Rect rect, CharacterInfo[] characterInfo, float fontSize, Color color) {
            float maxFontWidth=0;//字体最大宽度
            float maxFontHeight=0;//字体最大高度
            float length = 0;//字体总长度
            for (int i = 0; i < characterInfo.Length; i++)
            {
                CharacterInfo ci = characterInfo[i];
                length += Mathf.Abs(ci.glyphWidth) * fontSize;
                if (Mathf.Abs(ci.glyphWidth) * fontSize > maxFontWidth) {
                    maxFontWidth = Mathf.Abs(ci.glyphWidth) * fontSize;
                }
                if (Mathf.Abs(ci.glyphHeight) * fontSize > maxFontHeight)
                {
                    maxFontHeight = Mathf.Abs(ci.glyphHeight) * fontSize;
                }
            }
            Vector2 currentPos = Vector2.zero;
            currentPos.x = rect.position.x + rect.size.x / 2 - length / 2;
            currentPos.y = rect.position.y + rect.size.y / 2 - maxFontHeight / 2;
            for (int i = 0; i < characterInfo.Length; i++) {
                CharacterInfo ci = characterInfo[i];
                Vector2 leftDown =  ci.uvTopLeft;
                Vector2 rightDown = ci.uvTopRight;
                Vector2 rightUp =  ci.uvBottomRight;
                Vector2 leftUp = ci.uvBottomLeft;
                Vector2 charSize = new Vector2(Mathf.Abs(ci.glyphWidth), Mathf.Abs(ci.glyphHeight))*fontSize;
                GL.Begin(GL.QUADS);
                GL.Color(color);
                GL.TexCoord2(leftDown.x, leftDown.y); GL.Vertex(currentPos);
                GL.TexCoord2(rightDown.x, rightDown.y); GL.Vertex(currentPos + new Vector2(charSize.x,0));
                GL.TexCoord2(rightUp.x, rightUp.y); GL.Vertex(currentPos + charSize);
                GL.TexCoord2(leftUp.x, leftUp.y); GL.Vertex(currentPos + new Vector2(0,charSize.y));
                GL.End();
                currentPos += new Vector2(charSize.x, 0);
            }
        }
    }
}
