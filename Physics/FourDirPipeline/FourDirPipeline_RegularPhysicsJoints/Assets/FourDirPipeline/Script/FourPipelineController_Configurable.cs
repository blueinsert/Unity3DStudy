using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FourPipelineController_Configurable : MonoBehaviour
{
    public static FourPipelineController_Configurable Instance;

    public List<FourPipelineJointController_Configurable> m_joints = new List<FourPipelineJointController_Configurable>();
    public float m_horizontalValue = 0;
    public float m_verticalValue = 0;

    public void Init()
    {
       foreach(var jointCtrl in GetComponentsInChildren<FourPipelineJointController_Configurable>())
       {
            m_joints.Add(jointCtrl);
       }
        Instance = this;
    }

    public  void StartSimulation()
    {
        FourPipelineJointController_Configurable prev = null;
        int i = 0;
        var all = GetComponentsInChildren<FourPipelineJointController_Configurable>();
        foreach (var jointCtrl in all)
        {
            jointCtrl.InitPhysicsComps(prev, i, all.Count());
            prev = jointCtrl;
            i++;
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

    public void SetParams()
    {

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
