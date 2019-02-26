using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class CanvasContex
    {
        public Vector2 m_offset = Vector2.zero;
        public float m_scale = 1;
        public bool m_isDrawSelection = false;
        public Vector2 m_dragStartPosition = Vector2.zero;
        public CanvasElementBase m_curSelected;

        public List<CanvasElementBase> m_elements = new List<CanvasElementBase>();
        public List<CanvasElementBase> m_selectedElements = new List<CanvasElementBase>();

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
        }

        public CanvasContex()
        {

        }
    }
}
