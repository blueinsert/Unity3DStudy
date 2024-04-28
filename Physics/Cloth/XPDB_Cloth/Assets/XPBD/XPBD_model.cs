﻿using UnityEngine;
using System.Collections;

public struct CollideConstrain
{
    public int m_index;//顶点索引
    public Vector3 m_normal;
    public Vector3 m_entryPosition;
}

public class XPBD_model : MonoBehaviour
{

    float t = 0.0333f;
    float damping = 0.99f;
    int[] E;
    float[] L;
    Vector3[] V;

    public GameObject sphere;

    Vector3[] X_last = new Vector3[21*21];

    public CollideConstrain[] m_collideConstrain = new CollideConstrain[100];
    public int m_collideConstrainCount = 0;

    // Use this for initialization
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        //Resize the mesh.
        int n = 21;
        Vector3[] X = new Vector3[n * n];
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
                T[t * 6 + 1] = j * n + i + 1;
                T[t * 6 + 2] = (j + 1) * n + i + 1;
                T[t * 6 + 3] = j * n + i;
                T[t * 6 + 4] = (j + 1) * n + i + 1;
                T[t * 6 + 5] = (j + 1) * n + i;
                t++;
            }
        mesh.vertices = X;
        mesh.triangles = T;
        mesh.uv = UV;
        mesh.RecalculateNormals();

        X_last = X;

        //Construct the original edge list
        int[] _E = new int[T.Length * 2];
        for (int i = 0; i < T.Length; i += 3)
        {
            _E[i * 2 + 0] = T[i + 0];
            _E[i * 2 + 1] = T[i + 1];
            _E[i * 2 + 2] = T[i + 1];
            _E[i * 2 + 3] = T[i + 2];
            _E[i * 2 + 4] = T[i + 2];
            _E[i * 2 + 5] = T[i + 0];
        }
        //Reorder the original edge list
        for (int i = 0; i < _E.Length; i += 2)
            if (_E[i] > _E[i + 1])
                Swap(ref _E[i], ref _E[i + 1]);
        //Sort the original edge list using quicksort
        Quick_Sort(ref _E, 0, _E.Length / 2 - 1);

        int e_number = 0;
        for (int i = 0; i < _E.Length; i += 2)
            if (i == 0 || _E[i + 0] != _E[i - 2] || _E[i + 1] != _E[i - 1])
                e_number++;

        E = new int[e_number * 2];
        for (int i = 0, e = 0; i < _E.Length; i += 2)
            if (i == 0 || _E[i + 0] != _E[i - 2] || _E[i + 1] != _E[i - 1])
            {
                E[e * 2 + 0] = _E[i + 0];
                E[e * 2 + 1] = _E[i + 1];
                e++;
            }

        L = new float[E.Length / 2];
        for (int e = 0; e < E.Length / 2; e++)
        {
            int i = E[e * 2 + 0];
            int j = E[e * 2 + 1];
            L[e] = (X[i] - X[j]).magnitude;
        }

        V = new Vector3[X.Length];
        for (int i = 0; i < X.Length; i++)
            V[i] = new Vector3(0, 0, 0);
    }

    void Quick_Sort(ref int[] a, int l, int r)
    {
        int j;
        if (l < r)
        {
            j = Quick_Sort_Partition(ref a, l, r);
            Quick_Sort(ref a, l, j - 1);
            Quick_Sort(ref a, j + 1, r);
        }
    }

    int Quick_Sort_Partition(ref int[] a, int l, int r)
    {
        int pivot_0, pivot_1, i, j;
        pivot_0 = a[l * 2 + 0];
        pivot_1 = a[l * 2 + 1];
        i = l;
        j = r + 1;
        while (true)
        {
            do ++i; while (i <= r && (a[i * 2] < pivot_0 || a[i * 2] == pivot_0 && a[i * 2 + 1] <= pivot_1));
            do --j; while (a[j * 2] > pivot_0 || a[j * 2] == pivot_0 && a[j * 2 + 1] > pivot_1);
            if (i >= j) break;
            Swap(ref a[i * 2], ref a[j * 2]);
            Swap(ref a[i * 2 + 1], ref a[j * 2 + 1]);
        }
        Swap(ref a[l * 2 + 0], ref a[j * 2 + 0]);
        Swap(ref a[l * 2 + 1], ref a[j * 2 + 1]);
        return j;
    }

    void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }

    void Strain_Limiting()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        //Apply PBD here.
        //jacobi approach
        Vector3[] x_new = new Vector3[vertices.Length];
        int[] n = new int[vertices.Length];

        for (int e = 0; e < E.Length / 2; e++)
        {
            var l_e = L[e];
            int i = E[e * 2 + 0];
            int j = E[e * 2 + 1];
            var x_i = vertices[i];
            var x_j = vertices[j];
            var x_ij = x_j - x_i;
            var dir = x_ij.normalized;
            var len = x_ij.magnitude;
            var k1 = 0.5f;
            var k2 = 0.5f;
            //if (i == 0 || i == 20)
            //{
            //    k1 = 0;
            //    k2 = 1.0f;
            //}
            //else if (j == 0 || j == 20)
            //{
            //    k1 = 1.0f;
            //    k2 = 0;
            //}
            x_new[i] += x_i - k1 * (len - l_e) * (-dir);
            x_new[j] += x_j + k2 * (len - l_e) * (-dir);
            n[i] += 1;
            n[j] += 1;
        }

        for (int i = 0; i < x_new.Length; i++)
        {
            //if (i == 0 || i == 20) continue;

            if (n[i] != 0)
            {
                var x = (x_new[i] + 0.2f * vertices[i]) / (n[i] + 0.2f);
                vertices[i] = x;
            }
        }

        mesh.vertices = vertices;
    }

    void Solve_CollideConstrain()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < m_collideConstrainCount; i++)
        {
            var constrain = m_collideConstrain[i];
            var pos = vertices[constrain.m_index];
            float C = Vector3.Dot((pos - constrain.m_entryPosition), constrain.m_normal);
            if(C < 0)
            {
                Vector3 dp = -constrain.m_normal * C;
                pos += dp;
                vertices[constrain.m_index] = pos;
            }
            
        }

        mesh.vertices = vertices;
    }

    void Solve()
    {
        Strain_Limiting();
        Solve_CollideConstrain();
    }

    void GenerateCollideConstrains()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] X = mesh.vertices;

        m_collideConstrainCount = 0;

        if (sphere != null)
        {
            var c = sphere.transform.position;
            //var sphereCollider = sphere.GetComponent<SphereCollider>();
            var r = 2.6f;
            for (int i = 0; i < X.Length; i++)
            {
                var p = X[i];
                var x = p - c;
                var d = x.magnitude;
                var dir = x.normalized;
                if (d < r && m_collideConstrainCount<100-1)
                {
                    //X[i] = c + dir * r;

                    m_collideConstrain[m_collideConstrainCount].m_index = i;
                    m_collideConstrain[m_collideConstrainCount].m_normal = dir;
                    m_collideConstrain[m_collideConstrainCount].m_entryPosition = c + dir * r;
                    m_collideConstrainCount++;
                }
            }
        }

        for (int i = 0; i < X.Length; i++)
        {
            var p = X[i];
            if (p.y < -10 + 0.1f)
            {
                m_collideConstrain[m_collideConstrainCount].m_index = i;
                m_collideConstrain[m_collideConstrainCount].m_normal = new Vector3(0,1,0);
                m_collideConstrain[m_collideConstrainCount].m_entryPosition = new Vector3(p.x,0,p.z);
                m_collideConstrainCount++;
            }
        }
    }

    void Collision_Handling()
    {
        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        //Vector3[] X = mesh.vertices;
        //for (int i = 0; i < X.Length; i++)
        //{
        //    var p = X[i];
        //    if (p.y < -10 + 0.1f)
        //    {
        //        p.y = -10 + 0.1f;
        //    }
        //    X[i] = p;
        //}
        //mesh.vertices = X;
    }

    void CopyToXLast(Vector3[] X)
    {
        for(int i = 0; i < X.Length; i++)
        {
            X_last[i] = X[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] X = mesh.vertices;

        for (int i = 0; i < X.Length; i++)
        {
            //if (i == 0 || i == 20) continue;
            V[i] += new Vector3(0, -9.8f, 0) * t;
            X[i] += V[i] * t;
        }
        mesh.vertices = X;

        GenerateCollideConstrains();

        for (int l = 0; l < 64; l++)
            Solve();

        Collision_Handling();

        for (int i = 0; i < X.Length; i++)
        {
            V[i] += (mesh.vertices[i] - X[i]) / t;
            V[i] *= damping;
        }

        mesh.RecalculateNormals();

        CopyToXLast(X);
    }


}

