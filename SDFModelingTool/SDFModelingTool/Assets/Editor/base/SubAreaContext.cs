using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    /// <summary>
    /// 窗口子区域的运行时数据
    /// </summary>
    public class SubAreaContext
    {
        /// <summary>
        /// 定义数据
        /// </summary>
        public SubAreaDef m_areaDef;

        public Vector2 m_curMouseNormalizePosition = Vector2.zero;

        private IWindow m_owner;

        public int Width { get { return (int)(m_owner.Width * m_areaDef.m_size.x); } }
        public int Height { get { return (int)(m_owner.Height * m_areaDef.m_size.y); } }
        /// <summary>
        /// 区域大小,窗口坐标系
        /// </summary>
        public Vector2 Size { get { return new Vector2(Width, Height); } }
        /// <summary>
        /// 在窗口坐标系中的左上角位置
        /// </summary>
        public Vector2 Position { get { return new Vector2(m_owner.Width * m_areaDef.m_position.x, m_owner.Height * m_areaDef.m_position.y); } }

        /// <summary>
        /// 区域范围Rect,窗口坐标系
        /// </summary>
        public Rect Rect { get { return new Rect(Position, Size); } }

        public SubAreaContext(IWindow owner, SubAreaDef areaDef)
        {
            m_owner = owner;
            m_areaDef = areaDef;
        }

    }
}
