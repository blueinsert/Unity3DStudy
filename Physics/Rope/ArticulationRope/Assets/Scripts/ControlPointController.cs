using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPointController : CurvePointController
{
    private AnchorPointController m_anchorPointCtrl = null;

    protected AnchorPointController AnchorPointController
    {
        get
        {
            if (m_anchorPointCtrl == null)
            {
                m_anchorPointCtrl = this.GetComponentInParent<AnchorPointController>();
            }
            return m_anchorPointCtrl;
        }
    }

    protected override void OnMouseDrag()
    {
        base.OnMouseDrag();
        if (AnchorPointController)
        {
            AnchorPointController.OnControlPointPosChanged(this);
        }
        if (OwnerCurve)
            OwnerCurve.UpdateLine(gameObject);
    }
}
