using UnityEngine;
using System.Collections;
using UnityEditor;

public class TwoPointFiveDimensionCoordianteSystem : MonoBehaviour
{

    public Vector2 m_origin = Vector2.zero;
    public float m_angle = 0;
    public const float K = 0.7f; 

    public Vector3 Pos3DTo2D(Vector3 pos3d)
    {
        var sin = Mathf.Sin(m_angle * Mathf.Deg2Rad);
        var cos = Mathf.Cos(m_angle * Mathf.Deg2Rad);
        float x = pos3d.x + pos3d.y * K * cos;
        float y = pos3d.z + pos3d.y * K * sin;
        return new Vector3(x, y, 0);
    }

    public static Vector2 Rotate(Vector2 u, float angle)
    {
        float x, y;
        x = u.x * Mathf.Cos(Mathf.Deg2Rad * angle) - u.y * Mathf.Sin(Mathf.Deg2Rad * angle);
        y = u.x * Mathf.Sin(Mathf.Deg2Rad * angle) + u.y * Mathf.Cos(Mathf.Deg2Rad * angle);
        return new Vector3(x, y, 0);
    }

    public static void DrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(p1, p2); 
    }

    public static void DrawSingleArrowLine(Vector2 start, Vector2 end, float arrowLength, Color color)
    {
        DrawLine(start, end, color);
        var n = (start - end).normalized;
        var dir1 = Rotate(n,30);
        var dir2 = Rotate(n,-30);
        var p1 = end + dir1 * arrowLength;
        var p2 = end + dir2 * arrowLength;
        DrawLine(p1, end, color);
        DrawLine(p2, end, color);
    }

    public void DrawCoordianteSystem()
    {
        float scale = HandleUtility.GetHandleSize(this.transform.position);
        float length = scale * 0.8f;
        DrawSingleArrowLine(Vector2.zero, new Vector2(length, 0),length*0.3f, Color.red);
        var sin = Mathf.Sin(m_angle * Mathf.Deg2Rad);
        var cos = Mathf.Cos(m_angle * Mathf.Deg2Rad);
        DrawSingleArrowLine(Vector2.zero, new Vector2(length*cos, length*sin), length * 0.3f, Color.green);
        DrawSingleArrowLine(Vector2.zero, new Vector2(0, length), length * 0.3f, Color.blue);
    }

    public void OnDrawGizmos()
    {
        //Gizmos.matrix = this.transform.localToWorldMatrix * Matrix4x4.Translate(new Vector3(m_origin.x, m_origin.y, 0));
        //DrawCoordianteSystem();
    }

}
