using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class CanvasContext : SubAreaContext
    {
        public Vector2 m_offset = Vector2.zero;
        public float m_scale = 1;
        public float scale {
            get { return m_scale; }
            set
            {
                m_scale = Mathf.Clamp(value, 0.1f, 3);
            }
        }
        /// <summary>
        /// 是否在绘制选区
        /// </summary>
        public bool m_isDrawSelection = false;
        public bool m_isDragElement = false;
        public Vector2 m_dragStartPosition = Vector2.zero;
        public Vector2 m_curLocalPosition = Vector2.zero;

        public List<CanvasElementBase> m_elements = new List<CanvasElementBase>();
        public List<CanvasElementBase> m_selectedElements = new List<CanvasElementBase>();
        public CanvasElementBase m_curSelected
        {
            get {
                return m_selectedElements.Count == 0 ? null : m_selectedElements[0];
            }
            set {
                if (value != null)
                {
                    if (m_selectedElements.Count == 0)
                    {
                        m_selectedElements.Add(value);
                    }
                    else
                    {
                        m_selectedElements[0] = value;
                    }
                } 
            }
        }

        public CanvasContext(SubAreaDef areaDef) : base(areaDef)
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

        public CanvasElementBase TestPoint(Vector2 position)
        {
            foreach (var element in m_elements)
            {
                if (element.TestPoint(position))
                {
                    return element;
                }
            }
            return null;
        }

        public List<CanvasElementBase> TestRect(Rect rect)
        {
            List<CanvasElementBase> results = new List<CanvasElementBase>();
            foreach (var element in m_elements)
            {
                if (element.TestRect(rect))
                {
                    results.Add(element);
                }
            }
            return results;
        }

        public List<CanvasElementBase> TestRect(Vector2 dragStart, Vector2 dragEnd)
        {
            Vector2 center = (dragStart + dragEnd) / 2;
            Vector2 size = new Vector2(Mathf.Abs(dragEnd.x - dragStart.x), Mathf.Abs(dragEnd.y - dragStart.y));
            Rect rect = new Rect(center - size / 2, size);
            return TestRect(rect);
        }

        public void AddElement(CanvasElementBase element)
        {
            m_elements.Add(element);
        }

        public void RemoveElement(CanvasElementBase element)
        {
            m_elements.Remove(element);
        }

        public void ClearAllElement()
        {
            m_elements.Clear();
            m_selectedElements.Clear();
        }

        public bool IsSelected(CanvasElementBase element)
        {
            return m_selectedElements.Contains(element);
        }

        public void AddSelected(CanvasElementBase element)
        {
            m_selectedElements.Add(element);
        }

        public void AddSelected(List<CanvasElementBase> elements)
        {
            m_selectedElements.AddRange(elements);
        }

        public void RemoveSelected(CanvasElementBase element)
        {
            m_selectedElements.Remove(element);
        }

        public void RemoveSelected(List<CanvasElementBase> elements)
        {
            foreach (var element in elements)
            {
                RemoveSelected(element);
            }
        }

        public void ClearAllSelected()
        {
            m_selectedElements.Clear();
        }

    }
}
