using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class CanvasContex : SubAreaContex
    {
        public Vector2 m_offset = Vector2.zero;
        public float m_scale = 1;
        public bool m_isDrawSelection = false;
        public Vector2 m_dragStartPosition = Vector2.zero;
        public CanvasElementBase m_curSelected;
        public Vector2 m_curLocalPosition = Vector2.zero;

        public List<CanvasElementBase> m_elements = new List<CanvasElementBase>();
        public List<CanvasElementBase> m_selectedElements = new List<CanvasElementBase>();

        public CanvasContex(SubAreaDef areaDef) : base(areaDef)
        {

        }

        public Matrix4x4 GetTRSMatrix(Vector2 size)
        {
            var trs = Matrix4x4.identity;
            var translate = size / 2 + m_offset;
            trs.SetTRS(translate, Quaternion.identity, new Vector3(1 / m_scale, -1 / m_scale, 1));
            return trs;
        }

        public Vector2 GetLocalPos(Vector2 normalizedPosInCanvas, Vector2 canvasSize)
        {
            var canvasPos = new Vector2(normalizedPosInCanvas.x * canvasSize.x, normalizedPosInCanvas.y * canvasSize.y);
            var res = GetTRSMatrix(canvasSize).inverse * new Vector4(canvasPos.x, canvasPos.y, 0, 1);
            return new Vector2(res.x, res.y);
        }

        public void AddElement(CanvasElementBase element)
        {
            m_elements.Add(element);
        }

        public void RemoveElement(CanvasElementBase element)
        {
            m_elements.Remove(element);
            m_selectedElements.Remove(element);
        }

        public void ClearAllElement()
        {
            m_elements.Clear();
            m_selectedElements.Clear();
        }

        public void AddSelected(CanvasElementBase element)
        {
            m_selectedElements.Add(element);
        }

        public void AddSelected(List<CanvasElementBase> elements)
        {
            m_selectedElements.AddRange(elements);
        }

        public void ClearAllSelected()
        {
            m_selectedElements.Clear();
        }

        public bool IsSelected(CanvasElementBase element)
        {
            return m_selectedElements.Contains(element);
        }

        public List<CanvasElementBase> Overlap(Vector2 point1, Vector2 point2)
        {
            var center = (point1 + point2) / 2;
            var width = Mathf.Abs(point1.x - point2.x);
            var height = Mathf.Abs(point1.y - point2.y);
            Vector2 size = new Vector2(width, height);
            Rect rect = new Rect(center - size / 2, size);
            List<CanvasElementBase> results = new List<CanvasElementBase>();
            foreach (var element in this.m_elements)
            {
                if (element.canSelected && element.Overlap(rect))
                {
                    results.Add(element);
                }
            }
            return results;
        }
    }
}
