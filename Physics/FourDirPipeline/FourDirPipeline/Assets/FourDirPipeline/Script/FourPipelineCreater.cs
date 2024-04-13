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
    public bool m_useGravity;
    public float m_driveStifness = 9999f;
    public float m_driveDamp = 10000f;

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
        for(int i=0;i<m_jointCount;i++) {
            var go = Instantiate(m_jointPrefab, parent);
            go.transform.localPosition = m_jointAxisDir * m_jointOffset;// new Vector3(0, m_jointOffset, 0);
            parent = go.transform;
            FourPipelineJointController ctrl = go.GetComponent<FourPipelineJointController>();
            ctrl.SetLinkType(jointType);
            ctrl.SetRotateLimit(Mathf.Abs(m_jointRotateDegreeMax));
            float k = m_jointCount - i;
            ctrl.SetDriveProperty(m_driveStifness*k, m_driveDamp*k);
            //ctrl.m_articulationBody.useGravity = m_useGravity;
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
