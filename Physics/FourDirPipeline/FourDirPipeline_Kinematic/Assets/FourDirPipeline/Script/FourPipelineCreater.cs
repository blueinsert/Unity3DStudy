using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourPipelineCreater : MonoBehaviour
{
    public GameObject m_jointPrefab;
    public int m_jointCount;
    public Vector3 m_jointAxisDir = new Vector3(0, 1, 0);
    public float m_jointOffset;
    public float m_jointRotateDegreeMax = 15f;
    //public bool m_useGravity;
    //public float m_driveStifness = 9999f;
    //public float m_driveDamp = 10000f;

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
        FourPipelineJointType jointType = FourPipelineJointType.Horizonal;
        var parent = this.transform;
        var rootPosition = this.transform.position;
        for(int i=0;i<m_jointCount;i++) {
            var go = Instantiate(m_jointPrefab, parent);
            parent = go.transform;
            go.transform.localPosition = m_jointAxisDir * m_jointOffset;
            FourPipelineJointController ctrl = go.GetComponent<FourPipelineJointController>();
            ctrl.SetIndex(i);
            ctrl.SetLinkType(jointType);
            ctrl.SetRotateLimit(Mathf.Abs(m_jointRotateDegreeMax));
            if (jointType == FourPipelineJointType.Vertical)
                jointType = FourPipelineJointType.Horizonal;
            else if (jointType == FourPipelineJointType.Horizonal)
                jointType = FourPipelineJointType.Vertical;

            if(i == m_jointCount - 1)
            {
                ctrl.HideLinks();
            }
        }

        var fourPipelineCtrl = this.gameObject.GetComponent<FourPipelineController>();
        if (fourPipelineCtrl == null)
        {
            fourPipelineCtrl = this.gameObject.AddComponent<FourPipelineController>();
        }
        fourPipelineCtrl.Init();
        fourPipelineCtrl.StartSimulation();
    }
}
