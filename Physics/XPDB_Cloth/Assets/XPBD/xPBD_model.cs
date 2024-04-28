using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xPBD_Cloth : MonoBehaviour
{

    public int simuCount = 20;

    public float alpha = 0.005f;

    public Vector3 G = new Vector3(0, 0, -9.8f);

    public Mesh clothMesh;

    private Vector3[] prePos;

    private Vector3[] nowPos;

    private Vector3[] postPos;

    private Vector3[] tempV;

    private float[] invmass;

    private float t;

    void Start()
    {
        clothMesh = GetComponent<MeshFilter>().mesh;
        nowPos = clothMesh.vertices.Clone() as Vector3[];
        prePos = nowPos.Clone() as Vector3[];

        tempV = new Vector3[prePos.Length];

        for (int i = 0; i < tempV.Length; i++)
            tempV[i] = Vector3.zero;

        postPos = new Vector3[prePos.Length];
        t = Time.deltaTime / simuCount;
        alpha /= (Time.fixedDeltaTime * Time.fixedDeltaTime);
        List<float> massIndex = new List<float>();
        for (int i = 0; i < nowPos.Length; i++)
        {
            if (i == 0 || i == 10)
                massIndex.Add(0);
            else
                massIndex.Add(1f);
        }

        invmass = massIndex.ToArray();
    }


    void SolverParticle()
    {
        for (int i = 0; i < nowPos.Length; i++)
        {
            for (int s = 0; s < 5; s++)
            {
                int id0 = i;
                int id1 = i;

                switch (s)
                {
                    case 0: id1 = i + 1; break;
                    case 1: id1 = i - 1; break;
                    case 2: id1 = i + 11; break;
                    case 3: id1 = i - 11; break;
                    case 4: id1 = i + 12; break;
                }

                if (id1 < 0 || id1 >= nowPos.Length || id0 < 0 || id0 >= nowPos.Length)
                    continue;

                float w0 = invmass[id0];
                float w1 = invmass[id1];
                float w_T = w0 + w1;

                if (w_T == 0f)
                {
                    continue;
                }

                //两个质点之间进行约束
                // |x0-x1|
                float l = Vector3.Distance(nowPos[id0], nowPos[id1]);
                // x0 - x1
                Vector3 dir = nowPos[id0] - nowPos[id1];

                if (l == 0)
                {
                    continue;
                }

                //gradC = (xo-x1) / |x0-x1| 
                Vector3 gradC = dir / l;
                float l_rest = Vector3.Distance(prePos[id0], prePos[id1]);
                float C = l - l_rest;

                float gradC_2 = Mathf.Pow(gradC.magnitude, 2);

                // 我们采用的是每帧更新 n次约束，因此lambda每次都应该重置为0,也就可以将d_lambda等同于lambda
                // d_lambda = (-C - alpha*lambda) / (gradC * w_T * gradC_T + alpha)
                float lambda = -C / (w_T * gradC_2 + alpha);

                //x = x + deltaX where deltaX = gradC * w_T(i) * lambda
                Vector3 correction = lambda * gradC;

                nowPos[id0] += w0 * correction;
                nowPos[id1] -= w1 * correction;
            }
        }
    }

    void Update()
    {
        nowPos[0] = new Vector3(5, 0, 5);

        nowPos[10] = new Vector3(- 5, 0, 5);

        clothMesh.vertices = nowPos;
        clothMesh.RecalculateBounds();
        clothMesh.RecalculateNormals();
    }

    private void FixedUpdate()
    {
        t = Time.fixedDeltaTime / simuCount;

        for (int n = 0; n < simuCount; n++)
        {
            //pre
            for (int i = 0; i < nowPos.Length; i++)
            {
                tempV[i] += t * G;
                postPos[i] = nowPos[i];
                nowPos[i] += t * tempV[i];
            }

            SolverParticle();

            //post
            for (int i = 0; i < nowPos.Length; i++)
            {
                tempV[i] = (nowPos[i] - postPos[i]) / t;
                if (QCatchPoint.closestIndex > 0)
                {
                    invmass[QCatchPoint.closestIndex] = 10;
                }
            }
        }
    }

}
