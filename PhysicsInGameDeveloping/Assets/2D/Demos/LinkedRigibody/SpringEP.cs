using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace bluebean
{
    public class EndPoint
    {
        public int m_index;
        public Vector3 m_ptLocal;
    }

    public class SpringEP
    {
        public int m_id;
        public EndPoint m_end1;
        public EndPoint m_end2;
        public float m_k;
        public float m_d;
        public float m_initialLength;

        public SpringEP()
        {
            m_end1 = new EndPoint();
            m_end2 = new EndPoint();
        }
    }
}
