using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCreater : MonoBehaviour
{
    public GameObject m_jointPrefab;
    public int m_jointCount;
    public Vector3 m_jointAxisDir = new Vector3(0, 1, 0);
    public float m_jointOffset;

    // Start is called before the first frame update
    void Start()
    {
        CreateRope();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateRope()
    {
        var parent = this.transform;
        for (int i = 0; i < m_jointCount; i++)
        {
            var go = Instantiate(m_jointPrefab, parent);
            go.transform.localPosition = m_jointAxisDir * m_jointOffset;// new Vector3(0, m_jointOffset, 0);
            parent = go.transform;
        }
    }
}
