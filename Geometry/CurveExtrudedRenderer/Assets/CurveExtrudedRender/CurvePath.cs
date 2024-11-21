using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    [ExecuteInEditMode]
    public class CurvePath : CurvePathProvider
    {
        [SerializeField]
        public List<Transform> m_points = new List<Transform>();
        [SerializeField]
        public List<CurvePathPoint> m_curPointList = new List<CurvePathPoint>();
        [SerializeField]
        public float m_restLen;

        public bool m_isDirty = true;

        void OnEnable()
        {
            m_isDirty = true;
            ReconstructCurve();
        }

        private void OnValidate()
        {
            m_isDirty = true;
            ReconstructCurve();
        }

        void OnDisable()
        {

        }

        void ReconstructCurve()
        {
            if (!m_isDirty)
                return;
            m_curPointList.Clear();//ÓÐ´ýÓÅ»¯
            m_restLen = 0;
            if (m_points.Count >= 2)
            {
                for (int i = 0; i < m_points.Count; i++)
                {
                    var curvePoint = new CurvePathPoint()
                    {
                        thickness = 0.2f,
                        color = Color.white,
                        position = m_points[i].position,
                    };
                    Vector3 tan = Vector3.one;
                    if (i != m_points.Count - 1)
                    {
                        tan = (m_points[i + 1].position - m_points[i].position).normalized;
                        m_restLen += (m_points[i + 1].position - m_points[i].position).magnitude;
                    }
                    else
                    {
                        tan = (m_points[i].position - m_points[i-1].position).normalized;
                    }
                    Vector3 normal = Vector3.up;
                    Vector3 binormal = Vector3.Cross(tan, normal);
                    normal = Vector3.Cross(binormal, tan);

                    curvePoint.normal= normal;
                    curvePoint.binormal = binormal;
                    curvePoint.tangent = tan;

                    m_curPointList.Add(curvePoint);
                }
            }
           
            m_isDirty = false;
        }

        public void Update()
        {
            m_isDirty = true;
            ReconstructCurve();
        }

        public override List<CurvePathPoint> GetPath()
        {
            return m_curPointList;
        }

        public override float RestLength => base.RestLength;
    }
}
