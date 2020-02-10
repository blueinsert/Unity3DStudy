using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class SubAreaDef
    {
        public int m_id;
        /// <summary>
        /// 窗口名字
        /// </summary>
        public string m_showName;
        //归一化位置 左上角
        public Vector2 m_position;
        //归一化 大小
        public Vector2 m_size;
        public Color m_bgColor;
        public bool m_isCanvas;
        public int m_renderOrder;
    }
}