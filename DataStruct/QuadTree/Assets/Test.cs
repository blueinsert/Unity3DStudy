using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{

    public int m_count = 100;
    public int m_rangeXMin = -50;
    public int m_rangeXMax = 50;
    public int m_rangeYMin = -50;
    public int m_rangeYMax = 50;
    public float m_searchRadius = 5.0f;
    public const int LevelMax = 8;
    private Plane m_hPlane = new Plane(Vector3.up, Vector3.zero);
    public Vector3 m_mousePosition;
    public Color[] m_levelColor = new Color[LevelMax];
    private List<GameObject> m_points = new List<GameObject>();
    QuadTree m_quadTree = new QuadTree();
    [Header("叶子节点包含的最大数据量")]
    public int LeafSize = 15;

    // Use this for initialization
    void Start()
    {
        m_quadTree.SetBounds(new Rect(m_rangeXMin, m_rangeYMin, (m_rangeXMax - m_rangeXMin), m_rangeYMax - m_rangeYMin));

    }

    void GeneratePoints()
    {
        foreach (var p in m_points)
        {
            Destroy(p);
        }
        m_points.Clear();
        for (int i = 0; i < m_count; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var x = Random.Range(m_rangeXMin, m_rangeXMax);
            var y = Random.Range(m_rangeYMin, m_rangeYMax);
            go.transform.position = new Vector3(x, 0, y);
            m_points.Add(go);
        }

        ReBuildTree();
    }

    void ReBuildTree()
    {
        m_quadTree.Clear();
        m_quadTree.LeafSize = this.LeafSize;
        foreach(var go in m_points)
        {
            m_quadTree.Insert(new Vector2(go.transform.position.x, go.transform.position.z), go);
        }
    }

    private void UpdateMousePosition()
    {
        Vector3 position = Vector3.zero;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDistance;
        if (m_hPlane.Raycast(mouseRay, out rayDistance))
            position = mouseRay.GetPoint(rayDistance);

        m_mousePosition = position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMousePosition();
        if (Input.mouseScrollDelta.y < 0)
        {
            m_searchRadius *= 1.2f;
        }
        else if (Input.mouseScrollDelta.y > 0)
        {
            m_searchRadius /= 1.2f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GeneratePoints();
        }
    }

    void WildSearch(Vector2 p, float radius, out List<GameObject> nearest)
    {
        nearest = new List<GameObject>();
        for (int i = 0; i < m_points.Count; i++)
        {
            var go = m_points[i];
            var d = (p - new Vector2(go.transform.position.x, go.transform.position.z)).magnitude;
            if (d < radius)
            {
                nearest.Add(go);
            }
        }
    }

    void DrawCircle(Vector3 pos, float radius, float fragment = 36f)
    {
        for (int i = 0; i < fragment; i++)
        {
            var start = new Vector3(pos.x + radius * Mathf.Cos((i + 1) / fragment * (2 * Mathf.PI)), 0, pos.z + radius * Mathf.Sin((i + 1) / fragment * 2 * Mathf.PI));
            var end = new Vector3(pos.x + radius * Mathf.Cos((i + 2) / fragment * 2 * Mathf.PI), 0, pos.z + radius * Mathf.Sin((i + 2) / fragment * 2 * Mathf.PI));
            Gizmos.DrawLine(start, end);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        DrawCircle(m_mousePosition, m_searchRadius);
       
        Gizmos.color = new Color(0, 0, 0, 1f);
        Gizmos.DrawLine(new Vector3(m_rangeXMin, 0, m_rangeYMin), new Vector3(m_rangeXMin, 0, m_rangeYMax));
        Gizmos.DrawLine(new Vector3(m_rangeXMin, 0, m_rangeYMax), new Vector3(m_rangeXMax, 0, m_rangeYMax));
        Gizmos.DrawLine(new Vector3(m_rangeXMax, 0, m_rangeYMax), new Vector3(m_rangeXMax, 0, m_rangeYMin));
        Gizmos.DrawLine(new Vector3(m_rangeXMax, 0, m_rangeYMin), new Vector3(m_rangeXMin, 0, m_rangeYMin));


        if (m_quadTree != null)
        {
            m_quadTree.DebugDraw();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<GameObject> nearestGos = null;
            WildSearch(new Vector2(m_mousePosition.x, m_mousePosition.z), m_searchRadius, out nearestGos);
            UnityEngine.Debug.Log(string.Format("frame:{0} WildSearch consume {1}ms {2}tick", Time.frameCount, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks));

            stopwatch.Reset();
            stopwatch.Start();
            List<Object> nearests = new List<Object>();
            m_quadTree.Query(new Vector2(m_mousePosition.x, m_mousePosition.z), m_searchRadius, m_quadTree.GetBounds(), out nearests);
            UnityEngine.Debug.Log(string.Format("frame:{0} SearchNearest consume {1}ms {2}ticks", Time.frameCount, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks));
            foreach (var p in nearests)
            {
                var go = p as GameObject;
                Gizmos.DrawSphere(new Vector3(go.transform.position.x, 0, go.transform.position.z), 1);
            }
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "GeneraeSamples")) {
            GeneratePoints();
        }
        if (GUI.Button(new Rect(0, 100, 200, 100), "ReBuildTree"))
        {
            ReBuildTree();
        }
    }
}
