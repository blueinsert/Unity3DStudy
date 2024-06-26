﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public struct CollideConstrain
{
    public int m_index;//顶点索引
    public Vector3 m_normal;
    public Vector3 m_entryPosition;
    public Collider m_collider;

    public bool m_isDynamic;
}

public struct AttachConstrain
{
    public GameObject m_owner;
    public Vector3 m_offset;
    public int m_index;
}

public class XPBD_Cloth_Solver : MonoBehaviour
{
    public float m_mass = 1.0f;
    float t = 0.0333f;
    float damping = 0.99f;
    float damping_subStep = 0.99f;
    [Range(0f,1f)]
    public float m_edgeCompliance = 0.0f;
    [Range(0f, 1f)]
    public float m_collideCompliance = 0.0f;
    [Range(0f, 1f)]
    public float m_attachCompliance = 0.0f;
    float[] InvMass;
    int[] E;
    float[] L;
    Vector3[] V;
    public float m_planeY;

    Mesh m_mesh;

    float DtSubstep
    {
        get
        {
            return t / SubStep;
        }
    }

    Vector3[] X_last = new Vector3[21 * 21];
    Vector3[] X = new Vector3[21 * 21];

    public CollideConstrain[] m_collideConstrain = new CollideConstrain[CollideConstrainCountMax];
    public int m_collideConstrainCount = 0;
    const int CollideConstrainCountMax = 40 * 40;

    public AttachConstrain[] m_attachConstrain = new AttachConstrain[AttachConstrainCountMax];
    public int m_attachConstrainCount = 0;
    const int AttachConstrainCountMax = 50;

    public List<SimpleMove> m_simpleMoveList = new List<SimpleMove>();

    [Range(7, 55)]
    public int SubStep = 22;

    // Use this for initialization
    void Start()
    {
        InitMesh();
        InitAttachConstain();

        var simpleMoves = GetComponentsInChildren<SimpleMove>();
        m_simpleMoveList.AddRange(simpleMoves);
        damping_subStep = Mathf.Pow(damping, 1.0f / SubStep);
    }

    void InitMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        m_mesh = mesh;
        //Resize the mesh.
        int n = 21;
        InvMass = new float[n * n];
        Vector3[] X = new Vector3[n * n];
        Vector2[] UV = new Vector2[n * n];
        int[] T = new int[(n - 1) * (n - 1) * 6];
        for (int j = 0; j < n; j++)
            for (int i = 0; i < n; i++)
            {
                InvMass[j * n + i] = 1.0f/(m_mass/(n * n));
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

        this.X = X;
        CopyX2Y(this.X, this.X_last);

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

    void InitAttachConstain()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        m_attachConstrainCount = 0;
        var attachDescs = GetComponentsInChildren<AttachConstrainDesc>();
        foreach (var attachDesc in attachDescs)
        {
            var c = attachDesc.gameObject.transform.position;
            var r = attachDesc.m_collider.radius;

            for (int i = 0; i < vertices.Length; i++)
            {
                var p = vertices[i];
                var x = p - c;
                var d = x.magnitude;
                if (d < r && m_attachConstrainCount < AttachConstrainCountMax - 1)
                {
                    m_attachConstrain[m_attachConstrainCount].m_index = i;
                    m_attachConstrain[m_attachConstrainCount].m_owner = attachDesc.gameObject;
                    m_attachConstrain[m_attachConstrainCount].m_offset = p - attachDesc.gameObject.transform.position;
                    m_attachConstrainCount++;
                }
            }
        }
    }

    void Strain_Limiting()
    {
        var vertices = this.X;
        //Apply PBD here.
        //jacobi approach
        Vector3[] x_new = new Vector3[vertices.Length];
        int[] n = new int[vertices.Length];

        float alpha = m_edgeCompliance / (DtSubstep * DtSubstep);
        for (int e = 0; e < E.Length / 2; e++)
        {
            var l_e = L[e];
            int i = E[e * 2 + 0];
            int j = E[e * 2 + 1];
            var x_i = vertices[i];
            var x_j = vertices[j];
            var x_ij = x_j - x_i;
            var dir = x_ij.normalized;
            var grads = dir;
            var len = x_ij.magnitude;
            {//xpdb
                var inv_mass_i = InvMass[i];
                var inv_mass_j = InvMass[j];
                float C = len - l_e;
                float w = inv_mass_i + inv_mass_j;
                var s = -C / (w + alpha);
                vertices[i] -= grads * s * inv_mass_i;
                vertices[j] += grads * s * inv_mass_j;
            }
            if (false)//pdb
            {
                var k1 = 0.5f;
                var k2 = 0.5f;
                x_new[i] += x_i - k1 * (len - l_e) * (-dir);
                x_new[j] += x_j + k2 * (len - l_e) * (-dir);
                n[i] += 1;
                n[j] += 1;
            }
        }
        if (false)//
        {
            for (int i = 0; i < x_new.Length; i++)
            {
                if (n[i] != 0)
                {
                    var x = (x_new[i] + 0.2f * vertices[i]) / (n[i] + 0.2f);
                    vertices[i] = x;
                }
            }
        }
    }

    void Solve_CollideConstrain()
    {
        var vertices = this.X;
        var alpha = m_collideCompliance / (DtSubstep * DtSubstep);
        for (int i = 0; i < m_collideConstrainCount; i++)
        {
            var constrain = m_collideConstrain[i];
            var pos = vertices[constrain.m_index];
            float C = Vector3.Dot((pos - constrain.m_entryPosition), constrain.m_normal);
            if (C < 0)
            {
                Vector3 grads = constrain.m_normal;
                float invMass = InvMass[constrain.m_index];
                float w = invMass;
                float s = -C / (w + alpha);
                if (!constrain.m_isDynamic)
                {
                    Vector3 dp = invMass * s * grads;
                    vertices[constrain.m_index] += dp;
                }
                else
                {
                    Vector3 dp = invMass * s * grads;
                    vertices[constrain.m_index] += dp;
                }
            }

        }
    }

    void Solve_AttachConstrain()
    {
        var vertices = this.X;
        float alpha = m_attachCompliance / (DtSubstep * DtSubstep);
        for (int i = 0; i < m_attachConstrainCount; i++)
        {
            var constrain = m_attachConstrain[i];
            var p = vertices[constrain.m_index];
            float C = (p - (constrain.m_owner.transform.position + constrain.m_offset)).magnitude;
            Vector3 grads = (p - (constrain.m_owner.transform.position + constrain.m_offset)).normalized;
            float invMass = InvMass[constrain.m_index];
            float w = invMass;
            float s = -C / (w + alpha);
            Vector3 dp = invMass * s * grads;
            vertices[constrain.m_index] += dp;
        }
    }

    void Solve()
    {
        Strain_Limiting();
        Solve_CollideConstrain();
        Solve_AttachConstrain();
    }

    void GenerateCollideConstrains()
    {
        m_collideConstrainCount = 0;

        var collideDescs = GetComponentsInChildren<CollideConstrainDesc>();
        foreach (var collideDesc in collideDescs)
        {
            if (collideDesc.m_collider == null) continue;
            for (int i = 0; i < X.Length; i++)
            {
                var p = X[i];
                Vector3 normal, nearestP;
                if (PhysicUtil.IsOverlap(p, collideDesc.m_collider, out nearestP, out normal))
                {
                    if (m_collideConstrainCount < CollideConstrainCountMax - 1)
                    {
                        m_collideConstrain[m_collideConstrainCount].m_index = i;
                        m_collideConstrain[m_collideConstrainCount].m_normal = normal;
                        m_collideConstrain[m_collideConstrainCount].m_entryPosition = nearestP;
                        m_collideConstrain[m_collideConstrainCount].m_collider = collideDesc.m_collider;
                        m_collideConstrain[m_collideConstrainCount].m_isDynamic = collideDesc.m_isDynamic;
                        m_collideConstrainCount++;
                    }
                }
            }
        }


        for (int i = 0; i < X.Length; i++)
        {
            var p = X[i];
            float planeY = m_planeY;
            if (p.y < planeY && m_collideConstrainCount < CollideConstrainCountMax - 1)
            {
                m_collideConstrain[m_collideConstrainCount].m_index = i;
                m_collideConstrain[m_collideConstrainCount].m_normal = new Vector3(0, 1, 0);
                m_collideConstrain[m_collideConstrainCount].m_entryPosition = new Vector3(p.x, planeY, p.z);
                m_collideConstrainCount++;
            }
        }
    }

    void CopyX2Y(Vector3[] xarray, Vector3[] yarray)
    {
        for (int i = 0; i < xarray.Length; i++)
        {
            yarray[i] = xarray[i];
        }
    }

    void PreUpdate(float dt)
    {
        for (int i = 0; i < X.Length; i++)
        {
            //if (i == 0 || i == 20) continue;
            V[i] += new Vector3(0, -9.8f, 0) * dt;
            X[i] += V[i] * dt;
        }

        foreach (var simpleMove in m_simpleMoveList)
        {
            simpleMove.PreUpdate(dt);
        }
    }

    void Step(float dt)
    {
        PreUpdate(dt);

        CopyX2Y(this.X, this.X_last);

        GenerateCollideConstrains();

        Solve();

        for (int i = 0; i < X.Length; i++)
        {
            V[i] += (X[i] - X_last[i]) / dt;
            V[i] *= damping_subStep;
        }


    }

    // Update is called once per frame
    void Update()
    {
        float dt = t / SubStep;
        for (int i = 0; i < SubStep; i++)
        {
            Step(dt);
        }

        m_mesh.vertices = this.X;
        m_mesh.RecalculateNormals();
        //CopyToXLast(X);
    }


}

