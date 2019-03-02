using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class SubAreaContext
    {
        public SubAreaDef m_areaDef;

        public Vector2 m_curMouseNormalizePosition = Vector2.zero;

        public SubAreaContext(SubAreaDef areaDef)
        {
            m_areaDef = areaDef;
        }

        public Vector2 GetRealSize(float width, float height)
        {
            return new Vector2(width * m_areaDef.m_size.x, height * m_areaDef.m_size.y);
        }

        public Vector2 GetRealPos(float width, float height)
        {
            return new Vector2(width * m_areaDef.m_position.x, height * m_areaDef.m_position.y);
        }

        public Rect GetRealRect(float width, float height)
        {
            return new Rect(GetRealPos(width, height), GetRealSize(width, height));
        }
    }
}
