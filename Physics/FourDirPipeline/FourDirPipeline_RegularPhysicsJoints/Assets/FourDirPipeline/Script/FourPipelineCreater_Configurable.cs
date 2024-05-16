using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourPipelineCreater_Configurable : MonoBehaviour
{
    public GameObject m_jointPrefab;
    public int m_jointCount;
    public Vector3 m_jointAxisDir = new Vector3(0, 1, 0);
    public float m_jointOffset;
    public float m_jointRotateDegreeMax = 15f;

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
        for(int i=0;i < m_jointCount; i++) {
            var go = Instantiate(m_jointPrefab, parent);
            go.transform.position = rootPosition + m_jointAxisDir * m_jointOffset * i;// new Vector3(0, m_jointOffset, 0);
            FourPipelineJointController_Configurable ctrl = go.GetComponent<FourPipelineJointController_Configurable>();
            ctrl.SetLinkType(jointType);
            ctrl.SetRotateLimit(Mathf.Abs(m_jointRotateDegreeMax));
            if (jointType == FourPipelineJointType.Vertical)
                jointType = FourPipelineJointType.Horizonal;
            else if (jointType == FourPipelineJointType.Horizonal)
                jointType = FourPipelineJointType.Vertical;
            ctrl.GetComponent<Rigidbody>().mass = (m_jointCount - i) * 1f;
            if(i == m_jointCount - 1)
            {
                ctrl.HideLinks();
            }
        }

        var fourPipelineCtrl = this.gameObject.GetComponent<FourPipelineController_Configurable>();
        if (fourPipelineCtrl == null)
        {
            fourPipelineCtrl = this.gameObject.AddComponent<FourPipelineController_Configurable>();
        }
        fourPipelineCtrl.Init();
        fourPipelineCtrl.StartSimulation();
    }
}
