using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum FourPipelineJointType
{
    None = 0,
    Horizonal,
    Vertical,
}

public class FourPipelineJointController : MonoBehaviour
{
    public GameObject m_linkHNode;
    public GameObject m_linkVNode;
    public ArticulationBody m_articulationBody;
    public Vector2 m_rotateLimit = new Vector2(-15, 15);

    public FourPipelineJointType m_type;

    public float m_targetValue;

    public bool m_useProcessive = true;
    public float m_speed = 50;

    public void SetLinkType(FourPipelineJointType type)
    {
        m_type = type;
        m_linkHNode.SetActive(m_type == FourPipelineJointType.Horizonal);
        m_linkVNode.SetActive(m_type == FourPipelineJointType.Vertical);
        SetArticulationBody();
    }

    public void HideLinks()
    {
        m_linkHNode.SetActive(false);
        m_linkVNode.SetActive(false);
    }

    private void SetArticulationBody()
    {
        var ro = Quaternion.Euler(0, m_type == FourPipelineJointType.Horizonal ? 0 : 90, 0);
        m_articulationBody.anchorRotation = ro;
    }

    public void SetRotateLimit(float max)
    {
        m_rotateLimit = new Vector2(-max, max);
        var temp = m_articulationBody.xDrive;
        temp.lowerLimit = -max;
        temp.upperLimit = max;
        m_articulationBody.xDrive = temp;
    }

    public void SetDriveProperty(float stiffness, float damp)
    {
        var temp = m_articulationBody.xDrive;
        temp.driveType = ArticulationDriveType.Force;
        temp.stiffness = 3000000;
        temp.damping = 30000;
        m_articulationBody.xDrive = temp;
    }

    public void SetValue(float value)
    {
        value = (value + 1.0f) / 2;
        value = Mathf.Clamp01(value);
        m_targetValue = Mathf.Lerp(m_rotateLimit.x, m_rotateLimit.y, value);
        if (!m_useProcessive)
        {
            RotateTo(m_targetValue);
        }
    }


    void FixedUpdate()
    {
        if (m_useProcessive)
        {
            var dist = m_targetValue - CurrentPrimaryAxisRotation();
            if (Mathf.Abs(dist) < 0.01f)
            {
                RotateTo(m_targetValue);
                return;
            }
            var sign = Mathf.Sign(m_targetValue - CurrentPrimaryAxisRotation());
            float rotationChange = sign * m_speed * Time.fixedDeltaTime;
            float rotationGoal = CurrentPrimaryAxisRotation() + rotationChange;
            RotateTo(rotationGoal);
        }   
    }

    float CurrentPrimaryAxisRotation()
    {
        float currentRotationRads = m_articulationBody.jointPosition[0];
        float currentRotation = Mathf.Rad2Deg * currentRotationRads;
        return currentRotation;
    }

    void RotateTo(float primaryAxisRotation)
    {
        var drive = m_articulationBody.xDrive;
        drive.target = primaryAxisRotation;
        m_articulationBody.xDrive = drive;
    }
}
