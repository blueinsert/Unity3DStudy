using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public enum ConnectPointType {
        In,
        Out
    }

    public class ConnectPoint
    {
        public Vector2 Position { get { return m_owner.Position + m_offset; } }
        public TreeNode Owner { get { return m_owner; } }
        public ConnectPointType Type { get { return m_type; } }

        private TreeNode m_owner;
        private Vector2 m_offset;
        private ConnectPointType m_type;
        private float m_radius;

        public  ConnectPoint(TreeNode owner, Vector2 offset, float radius, ConnectPointType type) {
            m_owner = owner;
            m_offset = offset;
            m_type = type;
            m_radius = radius;
        }

        public bool TestPoint(Vector2 p) {
            return (p - Position).magnitude - m_radius < 0;
        }

    }
}
