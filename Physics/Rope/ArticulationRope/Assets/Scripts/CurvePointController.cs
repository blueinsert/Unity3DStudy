using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurvePointController : MonoBehaviour
{
    [Header("锁定X轴")]
    public bool m_isLockX = false;
    [Header("锁定Y轴")]
    public bool m_isLockY = true;
    [Header("锁定Z轴")]
    public bool m_isLockZ = false;

    private BizerCurve m_curve = null;

    protected BizerCurve OwnerCurve
    {
        get
        {
            if (m_curve == null)
            {
                m_curve = this.GetComponentInParent<BizerCurve>();
            }
            return m_curve;
        }
    }

    private void Awake()
    {
        
    }

    void OnMouseDown()
    {
        //Debug.Log("CurvePointControl:OnMouseDown");
        if (!gameObject.tag.Equals("AnchorPoint")) return;
    }

    protected virtual void OnMouseDrag()
    {
        Vector3 pos0 = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos0.z);
        Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 thisPos = mousePosInWorld;
        if (m_isLockX)
            thisPos.x = transform.position.x;
        if (m_isLockY)
            thisPos.y = transform.position.y;
        if (m_isLockZ)
            thisPos.z = transform.position.z;
        transform.position = thisPos;
    }

    public Vector3 GetPos()
    {
        return this.transform.position;
    }
}
