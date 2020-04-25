using UnityEngine;
using System.Collections;

public enum TwoPointFiveDimensionAAQuadAxis
{
    x,
    y,
    z,
}


public class TwoPointFiveDimensionAAQuad : MonoBehaviour
{
    public TwoPointFiveDimensionAAQuadAxis m_axis;
    public Vector3 m_min;
    public Vector3 m_max;
    public float yz_plane;
    public float xz_plane;
    public float xy_plane;

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

    private void DrawQuad()
    {
        switch (m_axis)
        {
            case TwoPointFiveDimensionAAQuadAxis.x:
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(yz_plane, Front, Bottom)), Pos3DTo2D(new Vector3(yz_plane, Back, Bottom)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(yz_plane, Back, Bottom)), Pos3DTo2D(new Vector3(yz_plane, Back, Top)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(yz_plane, Back, Top)), Pos3DTo2D(new Vector3(yz_plane, Front, Top)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(yz_plane, Front, Top)), Pos3DTo2D(new Vector3(yz_plane, Front, Bottom)));
                break;
            case TwoPointFiveDimensionAAQuadAxis.y:
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left, xz_plane, Bottom)), Pos3DTo2D(new Vector3(Right, xz_plane, Bottom)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, xz_plane, Bottom)), Pos3DTo2D(new Vector3(Right, xz_plane, Top)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, xz_plane, Top)), Pos3DTo2D(new Vector3(Left, xz_plane, Top)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left, xz_plane, Top)), Pos3DTo2D(new Vector3(Left, xz_plane, Bottom)));
                break;
            case TwoPointFiveDimensionAAQuadAxis.z:
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left, Front, xy_plane)), Pos3DTo2D(new Vector3(Right, Front, xy_plane)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, Front, xy_plane)), Pos3DTo2D(new Vector3(Right, Back, xy_plane)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Right, Back, xy_plane)), Pos3DTo2D(new Vector3(Left, Back, xy_plane)));
                Gizmos.DrawLine(Pos3DTo2D(new Vector3(Left, Back, xy_plane)), Pos3DTo2D(new Vector3(Left, Front, xy_plane)));
                break;
        }
    }

    public void OnDrawGizmos()
    {
        if (m_localCoord == null)
            return;
        Gizmos.color = Color.green;
        Gizmos.matrix = m_localCoord.transform.localToWorldMatrix * Matrix4x4.Translate(new Vector3(m_localCoord.m_origin.x, m_localCoord.m_origin.y, 0));
        DrawQuad();
    }

    public void OnDrawGizmosSelected()
    {
        if (m_localCoord == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.matrix = m_localCoord.transform.localToWorldMatrix * Matrix4x4.Translate(new Vector3(m_localCoord.m_origin.x, m_localCoord.m_origin.y, 0));
        DrawQuad();
        m_localCoord.DrawCoordianteSystem();
    }
}
