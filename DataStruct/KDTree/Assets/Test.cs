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
    KDTree.TNode m_kdTree = null;
    KDTree KDTree = new KDTree();
    // Use this for initialization
    void Start()
    {

    }

    void GeneratePoints()
    {
        foreach (var p in m_points)
        {
            Destroy(p);
        }
        m_points.Clear();
        List<KDTree.Data> datas = new List<KDTree.Data>();
        for (int i = 0; i < m_count; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var x = Random.Range(m_rangeXMin, m_rangeXMax);
            var y = Random.Range(m_rangeYMin, m_rangeYMax);
            go.transform.position = new Vector3(x, 0, y);
            m_points.Add(go);
            datas.Add(new KDTree.Data() { x = x, y = y });
        }
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        m_kdTree = KDTree.BuildTree(datas);
        var consume = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log(string.Format("frame:{0} BuildKDTree consume {1}ms", Time.frameCount, consume));
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
        }else if (Input.mouseScrollDelta.y > 0)
        {
            m_searchRadius /= 1.2f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GeneratePoints();
        }
    }

    void DrawKDTree(KDTree.TNode tree, float xMin, float xMax, float yMin, float yMax, int level)
    {
        float xSplit = 0;
        float ySplit = 0;
        if (level > LevelMax)
        {
            level = LevelMax;
        }

        Gizmos.color = m_levelColor[level - 1];
        if (tree.m_split == 0)
        {
            var x = tree.m_data.x;
            var start = new Vector3(x, 0, yMin);
            var end = new Vector3(x, 0, yMax);
            Gizmos.DrawLine(start, end);
            xSplit = x;
        }
        else if (tree.m_split == 1)
        {
            var y = tree.m_data.y;
            var start = new Vector3(xMin, 0, y);
            var end = new Vector3(xMax, 0, y);
            Gizmos.DrawLine(start, end);
            ySplit = y;
        }
        if (tree.m_left != null)
        {
            if (tree.m_split == 0)
            {
                DrawKDTree(tree.m_left, xMin, xSplit, yMin, yMax, level + 1);
            }
            else if (tree.m_split == 1)
            {
                DrawKDTree(tree.m_left, xMin, xMax, yMin, ySplit, level + 1);
            }

        }
        if (tree.m_right != null)
        {
            if (tree.m_split == 0)
            {
                DrawKDTree(tree.m_right, xSplit, xMax, yMin, yMax, level + 1);
            }
            else if (tree.m_split == 1)
            {
                DrawKDTree(tree.m_right, xMin, xMax, ySplit, yMax, level + 1);
            }
        }
    }

    void DrawCircle(Vector3 pos, float radius,float fragment = 36f)
    {
        for(int i = 0; i < fragment; i++)
        {
            var start = new Vector3(pos.x + radius * Mathf.Cos((i + 1) / fragment * (2*Mathf.PI)), 0, pos.z + radius * Mathf.Sin((i + 1) / fragment * 2 * Mathf.PI));
            var end = new Vector3(pos.x + radius * Mathf.Cos((i + 2) / fragment * 2 * Mathf.PI), 0, pos.z + radius * Mathf.Sin((i + 2) / fragment * 2 * Mathf.PI));
            Gizmos.DrawLine(start, end);
        }
    }

    void WildSearch(Vector2 p, float radius, out List<GameObject> nearest)
    {
        nearest = new List<GameObject>();
        for(int i = 0; i < m_points.Count; i++)
        {
            var go = m_points[i];
            var d = (p - new Vector2(go.transform.position.x, go.transform.position.z)).magnitude;
            if (d < radius)
            {
                nearest.Add(go);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (m_kdTree != null)
        {
            DrawKDTree(m_kdTree, m_rangeXMin, m_rangeXMax, m_rangeYMin, m_rangeYMax, 1);
            Gizmos.color = new Color(0, 1, 1, 0.5f);
            // Gizmos.DrawSphere(m_mousePosition, m_searchRadius);
            DrawCircle(m_mousePosition, m_searchRadius);
            List<KDTree.Data> nearests = null;
            List<GameObject> nearestGos = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            WildSearch(new Vector2(m_mousePosition.x, m_mousePosition.z), m_searchRadius, out nearestGos);
            UnityEngine.Debug.Log(string.Format("frame:{0} WildSearch consume {1}ms {2}tick", Time.frameCount, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks));

            stopwatch.Reset();
            stopwatch.Start();
            KDTree.SearchNearest(m_kdTree, new KDTree.Data() { x = m_mousePosition.x, y = m_mousePosition.z }, m_searchRadius, out nearests);
            UnityEngine.Debug.Log(string.Format("frame:{0} SearchNearest consume {1}ms {2}tick", Time.frameCount, stopwatch.ElapsedMilliseconds, stopwatch.ElapsedTicks));
            Gizmos.color = new Color(1, 0, 0, 1f);
            foreach (var p in nearests)
            {
                Gizmos.DrawSphere(new Vector3(p.x, 0, p.y), 1);
            }
        }
        Gizmos.color = new Color(0, 0, 0, 1f);
        Gizmos.DrawLine(new Vector3(m_rangeXMin, 0, m_rangeYMin), new Vector3(m_rangeXMin, 0, m_rangeYMax));
        Gizmos.DrawLine(new Vector3(m_rangeXMin, 0, m_rangeYMax), new Vector3(m_rangeXMax, 0, m_rangeYMax));
        Gizmos.DrawLine(new Vector3(m_rangeXMax, 0, m_rangeYMax), new Vector3(m_rangeXMax, 0, m_rangeYMin));
        Gizmos.DrawLine(new Vector3(m_rangeXMax, 0, m_rangeYMin), new Vector3(m_rangeXMin, 0, m_rangeYMin));
    }
}
