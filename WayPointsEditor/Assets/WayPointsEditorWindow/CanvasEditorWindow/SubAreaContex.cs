using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  bluebean
{
    public class SubAreaContex
    {
        public SubAreaDef m_areaDef;

        public Vector2 m_curNormalizeMousePosition = Vector2.zero;

        public SubAreaContex(SubAreaDef areaDef)
        {
            m_areaDef = areaDef;
        }

        public Rect GetRealRect(float width, float height, float toolBarHeight)
        {
            var pos = new Vector2(m_areaDef.m_position.x * width, (height - toolBarHeight) * m_areaDef.m_position.y + toolBarHeight);
            var size = new Vector2(m_areaDef.m_size.x * width, (height - toolBarHeight) * m_areaDef.m_size.y);
            var rect = new Rect(pos, size);
            return rect;
        }
    }

}

