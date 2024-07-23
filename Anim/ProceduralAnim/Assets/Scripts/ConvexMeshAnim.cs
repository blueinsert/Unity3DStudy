using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexMeshAnim : MeshProceduralAnim
{
    public float m_a = 2.0f;

    public override float ProceduralSample(Vector2 coordinate, float normalizedTime)
    {
        var temp = m_a * normalizedTime - coordinate.x * coordinate.x - coordinate.y * coordinate.y;
        if (temp < 0)
            return 0;
        return Mathf.Sqrt(temp);
    }
}
