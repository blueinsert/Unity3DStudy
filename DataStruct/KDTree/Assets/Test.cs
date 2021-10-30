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
    public const int LevelMax = 8;
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
        UnityEngine.Debug.Log(string.Format("BuildKDTree consume {0}ms", consume));
    }

    // Update is called once per frame
    void Update()
    {
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

    void OnDrawGizmos()
    {
        if (m_kdTree != null)
        {
            DrawKDTree(m_kdTree, m_rangeXMin, m_rangeXMax, m_rangeYMin, m_rangeYMax, 1);
        }
    }
}
