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
        CreateJoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateJoints()
    {
        var parent = this.transform;
        var rootPosition = this.transform.position;
        for(int i=0;i<m_jointCount;i++) {
            var go = Instantiate(m_jointPrefab, parent);
            go.transform.localPosition = m_jointAxisDir * m_jointOffset * i;// new Vector3(0, m_jointOffset, 0);
            RopeJointController ctrl = go.GetComponent<RopeJointController>();
            if(ctrl == null)
            {
                ctrl = go.AddComponent<RopeJointController>();
            }
        }

        var ropeCtrl = this.gameObject.GetComponent<RopeController>();
        if (ropeCtrl == null)
        {
            ropeCtrl = this.gameObject.AddComponent<RopeController>();
        }
        ropeCtrl.Init();
        ropeCtrl.StartSimulation();
        FindObjectOfType<ForceArea>().m_isEnable = true;
    }
}
