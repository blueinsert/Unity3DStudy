using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class BizerCurve : MonoBehaviour
{
    public List<AnchorPointController> m_allPoints;
    private GameObject m_anchorPointPrefab;
    private GameObject m_controlPointPrefab;
    private GameObject m_pointParent;
    private LineRenderer m_lineRenderer;

    private int m_curveCount = 0;
    private int SEGMENT_COUNT = 60;//曲线取点个数（取点越多这个长度越趋向于精确）

    void Awake()
    {
        SetLine();
        if (null == m_anchorPointPrefab)
            m_anchorPointPrefab = Resources.Load("Prefabs/AnchorPoint") as GameObject;
        if (null == m_controlPointPrefab)
            m_controlPointPrefab = Resources.Load("Prefabs/ControlPoint") as GameObject;
    }

    #region 显示相关

    void SetLine()
    {
        if (null == m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.material = Resources.Load("Materials/LineMaterial") as Material;
        m_lineRenderer.startColor = Color.red;
        m_lineRenderer.endColor = Color.green;
        m_lineRenderer.widthMultiplier = 0.2f;
    }


    public List<Vector3> HiddenLine(bool isHidden = false)
    {
        m_pointParent.SetActive(isHidden);
        m_lineRenderer.enabled = isHidden;
        List<Vector3> pathPoints = new List<Vector3>();
        if (!isHidden)
        {
            for (int i = 0; i < m_lineRenderer.positionCount; i++)
            {
                pathPoints.Add(m_lineRenderer.GetPosition(i));
            }
        }
        return pathPoints;
    }

    private void DrawCurve()//画曲线
    {
        if (m_allPoints.Count < 2) return;
        m_curveCount = m_allPoints.Count-1;
        for (int j = 0; j < m_curveCount; j++)
        {
            int nodeIndex = j;
            var cur = m_allPoints[nodeIndex];
            var next = m_allPoints[nodeIndex + 1];
            var p0 = cur.GetPos();
            var p1 = cur.m_controlPoint2.GetPos();
            var p2 = next.m_controlPoint1.GetPos();
            var p3 = next.GetPos();

            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = (float)i / (float)SEGMENT_COUNT;
                Vector3 pixel = CalculateCubicBezierPoint(t, p0,p1,p2,p3);
                m_lineRenderer.positionCount = j * SEGMENT_COUNT + i;
                m_lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
            }
        }
    }

    #endregion

    #region 编辑相关
    private AnchorPointController CreateAnchorPoint(Vector3 pos)
    {
        var pointPrefab = m_anchorPointPrefab;
        if (pointPrefab == null)
        {
            Debug.LogError("The Prefab is Null!");
            return null;
        }
        if (null == m_pointParent)
        {
            m_pointParent = new GameObject("AllPoints");
            m_pointParent.transform.parent = this.transform;
        }

        GameObject pointClone = Instantiate(pointPrefab);
        pointClone.name = pointClone.name.Replace("(Clone)", "");
        pointClone.transform.SetParent(m_pointParent.transform);
        pointClone.transform.position = pos;

        var ctrl = pointClone.GetComponent<AnchorPointController>();
        if(ctrl == null)
        {
            ctrl = pointClone.AddComponent<AnchorPointController>();
        }
        return ctrl;
    }

    public void AddPoint(Vector3 anchorPointPos)
    {
        //初始化时m_allPoints添加了一个player
        if (m_allPoints.Count == 0)
        {
            var firstCtrl = CreateAnchorPoint(anchorPointPos);
            m_allPoints.Add(firstCtrl);
            return;
        }
        var lastPoint = m_allPoints[m_allPoints.Count - 1];
        var newAnchorPoint = CreateAnchorPoint(anchorPointPos);

        var controlPointPos = lastPoint.GetPos() + (lastPoint.m_controlPoint1 != null ? -lastPoint.GetControlPointOffset(0) : new Vector3(0, 0, 1));
        lastPoint.CreateControlPoint(m_controlPointPrefab, controlPointPos, 1);
        var dir = (anchorPointPos - lastPoint.GetPos()).normalized.normalized;
        controlPointPos = newAnchorPoint.GetPos() - dir;
        newAnchorPoint.CreateControlPoint(m_controlPointPrefab, controlPointPos, 0);

        m_allPoints.Add(newAnchorPoint);

        DrawCurve();
    }

    public void DeletePoint(GameObject anchorPoint)
    {
        if (anchorPoint == null) return;
        AnchorPointController curvePoint = anchorPoint.GetComponent<AnchorPointController>();
        if (curvePoint && anchorPoint.tag.Equals("AnchorPoint"))
        {
            if (m_allPoints.IndexOf(curvePoint) == (m_allPoints.Count - 1))
            {
                m_allPoints.Remove(curvePoint);
                //删除新的末尾元素的多的控制点
                var lastPoint = m_allPoints[m_allPoints.Count - 2];
                if (lastPoint.m_controlPoint2)
                {
                 
                    Destroy(lastPoint.m_controlPoint2.gameObject);
                    lastPoint.m_controlPoint2 = null;
                }
            }
            else
            {
                m_allPoints.Remove(curvePoint);
            }
            Destroy(anchorPoint);
            if (m_allPoints.Count == 1)
            {
                m_lineRenderer.positionCount = 0;
            }
        }

        DrawCurve();
    }

    public void UpdateLine(GameObject anchorPoint)
    {
        if (anchorPoint == null) return;
        DrawCurve();
    }

    #endregion

    private void Update()
    {
#if UNITY_EDITOR
        DrawCurve();
#endif
    }

    public int GetCurveSegmentCount()
    {
        if (m_allPoints.Count < 2) return 0;
        m_curveCount = m_allPoints.Count - 1;
        return m_curveCount;
    }

    public float GetCurveLength()
    {
        if (m_allPoints.Count < 2) return 0;
        m_curveCount = m_allPoints.Count - 1;
        float len = 0;
        for (int j = 0; j < m_curveCount; j++)
        {
            var curLen = GetCurveLength(j);
            len += curLen;
        }
        return len;
    }

    public float GetCurveLength(int segmentIndex)
    {
        int nodeIndex = segmentIndex;
        var cur = m_allPoints[nodeIndex];
        var next = m_allPoints[nodeIndex + 1];
        var p0 = cur.GetPos();
        var p1 = cur.m_controlPoint2.GetPos();
        var p2 = next.m_controlPoint1.GetPos();
        var p3 = next.GetPos();
        var curLen = GetCurveLength(p0, p1, p2, p3);
        return curLen;
    }

    private float GetCurveLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var segment = SEGMENT_COUNT*2;
        float len = 0;
        for (int i = 1; i <= segment; i++)
        {
            float t0 = (float)(i-1) / (float)segment;
            float t1 = (float)i / (float)segment;
            
            Vector3 point0 = CalculateCubicBezierPoint(t0, p0, p1, p2, p3);
            Vector3 point1 = CalculateCubicBezierPoint(t1, p0, p1, p2, p3);
            len += (point1 - point0).magnitude;
        }
        return len;
    }

    public Vector3 GetPosition(CurvePosition curvePosition)
    {
        int nodeIndex = curvePosition.m_segmentIndex;
        var cur = m_allPoints[nodeIndex];
        var next = m_allPoints[nodeIndex + 1];
        var p0 = cur.GetPos();
        var p1 = cur.m_controlPoint2.GetPos();
        var p2 = next.m_controlPoint1.GetPos();
        var p3 = next.GetPos();
        var position = CalculateCubicBezierPoint(curvePosition.m_t, p0, p1, p2, p3);
        return position;
    }

    public Vector3 GetTangent(CurvePosition curvePosition)
    {
        var delta = 0.01f;
        var pos = new CurvePosition() { m_segmentIndex = curvePosition.m_segmentIndex,m_t = curvePosition.m_t };
        var p1 = GetPosition(pos);
        pos.m_t += delta;
        var p2 = GetPosition(pos);
        var dir = (p2-p1).normalized;
        return dir;
    }

    //贝塞尔曲线公式：B(t)=P0*(1-t)^3 + 3*P1*t(1-t)^2 + 3*P2*t^2*(1-t) + P3*t^3 ,t属于[0,1].
    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}
