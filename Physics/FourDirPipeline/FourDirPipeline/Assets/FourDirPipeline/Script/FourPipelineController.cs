using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourPipelineController : MonoBehaviour
{
    public static FourPipelineController Instance;

    public List<FourPipelineJointController> m_joints = new List<FourPipelineJointController>();
    public float m_horizontalValue = 0;
    public float m_verticalValue = 0;

    public void Init()
    {
       foreach(var jointCtrl in GetComponentsInChildren<FourPipelineJointController>())
       {
            m_joints.Add(jointCtrl);
            //jointCtrl.m_articulationBody.enabled = true;
       }
        Instance = this;
    }

    public  void StartSimulation()
    {
        this.GetComponent<ArticulationBody>().enabled = true;
        foreach (var jointCtrl in GetComponentsInChildren<FourPipelineJointController>())
        {
            jointCtrl.m_articulationBody.enabled = true;
        }
    }

    public void SetValue(Vector2 value)
    {
        SetHorizontalValue(value.x);
        SetVerticalValue(value.y);
    }

    public void SetHorizontalValue(float value)
    {
        foreach (var jointCtrl in m_joints)
        {
            if (jointCtrl.m_type == FourPipelineJointType.Horizonal)
            {
                jointCtrl.SetValue(value);
            }
        }
    }

    public void SetVerticalValue(float value)
    {
        foreach (var jointCtrl in m_joints)
        {
           if (jointCtrl.m_type == FourPipelineJointType.Vertical)
            {
                jointCtrl.SetValue(value);
            }
        }
    }

    private void Update()
    {

    }

    public void OnGUI()
    {
        if(GUI.Button(new Rect(20, 20, 40, 20), "¿ªÊ¼"))
        {
            StartSimulation();
        }
        
        GUI.Label(new Rect(20, 50, 50, 50), m_horizontalValue.ToString("F2"));
        var last = m_horizontalValue;
        m_horizontalValue = GUI.HorizontalSlider(new Rect(80, 50, 400, 50), m_horizontalValue, -1, 1);
        if (last != m_horizontalValue)
        {
            SetHorizontalValue(m_horizontalValue);
        }
        GUI.Label(new Rect(20, 120, 50, 50), m_verticalValue.ToString("F2"));
        last = m_verticalValue;
        m_verticalValue = GUI.HorizontalSlider(new Rect(80, 120, 400, 50), m_verticalValue, -1, 1);
        if (last != m_verticalValue)
        {
            SetVerticalValue(m_verticalValue);
        }
    }
}
