using UnityEngine;
using System.Collections;

public class TwoPointFiveDemensionAABox : MonoBehaviour
{
    public Vector3 m_min = Vector3.zero;
    public Vector3 m_max = Vector3.one;
    public TwoPointFiveDimensionCoordianteSystem m_localCoord;

    public float Left { get { return m_min.x; } }
    public float Right { get { return m_max.x; } }
    public float Top { get { return m_max.z; } }
    public float Bottom { get { return m_min.z; } }
    public float Front { get { return m_min.y; } }
    public float Back { get { return m_max.y; } }

    private Vector3 Pos3DTo2D(Vector3 pos3d)
    {
        return m_localCoord.Pos3DTo2D(pos3d);
    }

    private void DrawDotLine(Vector3 start,Vector3 end,int segment = 16)
    {
        var len = (end - start).magnitude;
        var segLen = len / (segment * 2);
        var n = (end - start).normalized;
        for(int i = 0; i < segment * 2; i++)
        {
            if (i % 2 == 0)
                Gizmos.DrawLine(start + segLen * n * i, start+ segLen * n * (i + 1));
        }
    }

    private void DrawAABox()
    {
        //正面
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left,Front,Bottom)), Pos3DTo2D(new Vector3(Right, Front, Bottom)));
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, Front, Bottom)), Pos3DTo2D(new Vector3(Right, Front, Top)));
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, Front, Top)), Pos3DTo2D(new Vector3(Left, Front, Top)));
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left, Front, Top)), Pos3DTo2D(new Vector3(Left, Front, Bottom)));
        //右面
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, Front, Bottom)), Pos3DTo2D(new Vector3(Right, Back, Bottom)));
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, Back, Bottom)), Pos3DTo2D(new Vector3(Right, Back, Top)));
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, Back, Top)), Pos3DTo2D(new Vector3(Right, Front, Top)));
        //上面
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left, Front, Top)), Pos3DTo2D(new Vector3(Left, Back, Top)));
        Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left, Back, Top)), Pos3DTo2D(new Vector3(Right, Back, Top)));
        //内侧三条线
        DrawDotLine(Pos3DTo2D(new Vector3(Left, Front, Bottom)), Pos3DTo2D(new Vector3(Left, Back, Bottom)));
        DrawDotLine(Pos3DTo2D(new Vector3(Left, Back, Bottom)), Pos3DTo2D(new Vector3(Right, Back, Bottom)));
        DrawDotLine(Pos3DTo2D(new Vector3(Left, Back, Bottom)), Pos3DTo2D(new Vector3(Left, Back, Top)));
    }

    public void OnDrawGizmos()
    {
        if (m_localCoord == null)
            return;
        Gizmos.color = Color.blue;
        Gizmos.matrix = m_localCoord.transform.localToWorldMatrix * Matrix4x4.Translate(new Vector3(m_localCoord.m_origin.x, m_localCoord.m_origin.y, 0));
        DrawAABox();
    }

    public void OnDrawGizmosSelected()
    {
        if (m_localCoord == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.matrix = m_localCoord.transform.localToWorldMatrix * Matrix4x4.Translate(new Vector3(m_localCoord.m_origin.x, m_localCoord.m_origin.y, 0));
        DrawAABox();
        m_localCoord.DrawCoordianteSystem();
    }
}
