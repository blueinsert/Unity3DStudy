using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContinuousType
{
    C0,
    C1,
    C2,
}

public class AnchorPointController : CurvePointController
{
    public ContinuousType m_cType = ContinuousType.C1;
    public ControlPointController m_controlPoint1;
    public ControlPointController m_controlPoint2;

    private LineRenderer m_lineRenderer;

    private BizerCurve m_curve = null;

    private void Awake()
    {
        m_curve = GetComponentInParent<BizerCurve>();
    }

    public Vector3 GetControlPointOffset(int index)
    {
        ControlPointController p = null;
        if (index == 0)
            p = m_controlPoint1;
        else if (index == 1)
            p = m_controlPoint2;
        if(p != null)
        {
            return p.GetPos() - this.GetPos();
        }
        Debug.LogError("AnchorPointController:GetControlPointOffset p==null");
        return Vector3.zero;
    }

    public ControlPointController CreateControlPoint(GameObject prefab, Vector3 pos, int index)
    {
        var pointPrefab = prefab;
        if (pointPrefab == null)
        {
            Debug.LogError("The Prefab is Null!");
            return null;
        }

        GameObject pointClone = Instantiate(pointPrefab);
        pointClone.name = pointClone.name.Replace("(Clone)", "");
        pointClone.transform.SetParent(this.transform);
        pointClone.transform.position = pos;

        var ctrl = pointClone.GetComponent<ControlPointController>();
        if (ctrl == null)
        {
            ctrl = pointClone.AddComponent<ControlPointController>();
        }
        if (index == 0)
            m_controlPoint1 = ctrl;
        if (index == 1)
            m_controlPoint2 = ctrl;
        return ctrl;
    }

    public LineRenderer LineRenderer
    {
        get
        {
            if (gameObject.tag.Equals("AnchorPoint") && !m_lineRenderer)
            {
                m_lineRenderer = gameObject.GetComponent<LineRenderer>();
                if (m_lineRenderer == null)
                {
                    m_lineRenderer = gameObject.AddComponent<LineRenderer>();
                    m_lineRenderer.sortingOrder = 1;
                    m_lineRenderer.material =  Resources.Load("Materials/LineMaterial") as Material;

                    m_lineRenderer.startColor = m_lineRenderer.endColor = Color.yellow;
                    m_lineRenderer.widthMultiplier = 0.03f;
                    m_lineRenderer.positionCount = 0;
                }
                if(m_lineRenderer.material == null)
                {
                    m_lineRenderer.material = Resources.Load("Materials/LineMaterial") as Material;
                }
            }
            return m_lineRenderer;
        }
    }

    private void DrawControlLine()
    {
        if (!gameObject.tag.Equals("AnchorPoint") || (!m_controlPoint1 && !m_controlPoint2)) return;
        if (LineRenderer)
        {
            LineRenderer.positionCount = (m_controlPoint1 && m_controlPoint2) ? 3 : 2;
            if (m_controlPoint1 && !m_controlPoint2)
            {
                LineRenderer.SetPosition(0, m_controlPoint1.GetPos());
                LineRenderer.SetPosition(1, transform.position);
            }
            if (m_controlPoint2 && !m_controlPoint1)
            {
                LineRenderer.SetPosition(0, transform.position);
                LineRenderer.SetPosition(1, m_controlPoint2.GetPos());
            }
            if (m_controlPoint1 && m_controlPoint2)
            {
                LineRenderer.SetPosition(0, m_controlPoint1.GetPos());
                LineRenderer.SetPosition(1, this.GetPos());
                LineRenderer.SetPosition(2, m_controlPoint2.GetPos());
            }
        }
    }

    void Update()
    {
        DrawControlLine();
    }

    protected override void OnMouseDrag()
    {
        base.OnMouseDrag();
        if (OwnerCurve)
            OwnerCurve.UpdateLine(gameObject);
    }

    public void OnControlPointPosChanged(ControlPointController p)
    {
        ControlPointController another = null;
        if (p == m_controlPoint1)
            another = m_controlPoint2;
        if (p == m_controlPoint2)
            another = m_controlPoint1;
        if (another == null)
            return;
        if(m_cType == ContinuousType.C1)
        {
            var dir = p.GetPos() - this.GetPos();
            var dir2 = another.GetPos() - this.GetPos();
            var len = dir2.magnitude;
            var pos = this.GetPos() - dir.normalized * len;
            another.transform.position = pos;
        }else if(m_cType == ContinuousType.C2)
        {
            var dir = p.GetPos() - this.GetPos();
            var len = dir.magnitude;
            var pos = this.GetPos() - dir.normalized * len;
            another.transform.position = pos;
        }
    }
}
