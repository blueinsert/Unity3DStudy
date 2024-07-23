using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshProceduralAnim : MonoBehaviour
{
    public Vector3[] X;

    public float m_size = 6;

    public float m_normalizedTime = 0;

    public bool m_isDirty = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isDirty)
        {
            AnimSample(m_normalizedTime);
            m_isDirty = false;
        }
    }

    private float Normalized(float value)
    {
        var norm = value / m_size;
        //norm = Mathf.Clamp(norm, -1f, 1.0f);
        return norm;
    }

    public void GenerateMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        //Resize the mesh.
        int n = 21;
        X = new Vector3[n * n];
        Vector2[] UV = new Vector2[n * n];
        int[] T = new int[(n - 1) * (n - 1) * 6];
        for (int j = 0; j < n; j++)
            for (int i = 0; i < n; i++)
            {
                X[j * n + i] = new Vector3(5 - 10.0f * i / (n - 1), 0, 5 - 10.0f * j / (n - 1));
                UV[j * n + i] = new Vector3(i / (n - 1.0f), j / (n - 1.0f));
            }
        int t = 0;
        for (int j = 0; j < n - 1; j++)
            for (int i = 0; i < n - 1; i++)
            {
                T[t * 6 + 0] = j * n + i;
                T[t * 6 + 1] = (j + 1) * n + i + 1;
                T[t * 6 + 2] = j * n + i + 1; 
                T[t * 6 + 3] = j * n + i;
                T[t * 6 + 4] = (j + 1) * n + i;
                T[t * 6 + 5] = (j + 1) * n + i + 1;
                t++;
            }
        mesh.vertices = X;
        mesh.triangles = T;
        mesh.uv = UV;
        mesh.RecalculateNormals();
    }

    private void SyncMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = X;
        mesh.RecalculateNormals();
    }

    public virtual float ProceduralSample(Vector2 coordinate,float normalizedTime)
    {
        var temp = 2.0f * normalizedTime - coordinate.x * coordinate.x - coordinate.y * coordinate.y;
        if (temp < 0)
            return 0;
        return Mathf.Sqrt(temp);
    }

    public void AnimSample(float normalizeTime)
    {
        for (int i = 0; i < X.Length; i++)
        {
            var p = X[i];
            Vector2 coordinate = new Vector2(Normalized(p.x), Normalized(p.z));
            var value = ProceduralSample(coordinate, normalizeTime);
            X[i].y = value;
        }
        SyncMesh();
    }
}
