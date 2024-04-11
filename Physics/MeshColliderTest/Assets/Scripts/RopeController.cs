using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public static RopeController Instance;

    public List<RopeJointController> m_joints = new List<RopeJointController>();
    public float m_horizontalValue = 0;
    public float m_verticalValue = 0;

    public void Init()
    {
       foreach(var jointCtrl in GetComponentsInChildren<RopeJointController>())
       {
            m_joints.Add(jointCtrl);
       }
        Instance = this;
    }

    public  void StartSimulation()
    {
        RopeJointController prev = null;
        int i = 0;
        var all = GetComponentsInChildren<RopeJointController>();
        foreach (var jointCtrl in all)
        {
            jointCtrl.InitPhysicsComps(prev, i, all.Count());
            prev = jointCtrl;
            i++;
        }
    }

    private void Update()
    {

    }

    public void OnGUI()
    {
       
    }
}
