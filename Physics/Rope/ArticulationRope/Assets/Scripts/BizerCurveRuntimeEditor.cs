
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BizerCurveRuntimeEditor : MonoBehaviour
{
#if UNITY_EDITOR
    public GameObject m_player;

    private BizerCurve m_curve = null;

    private void Awake()
    {
        m_curve = FindObjectOfType<BizerCurve>();// this.GetComponent<BizerCurve>();
    }

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonUp(0) && hit.collider.tag.Equals("Terrain"))
                {
                    Vector3 pointPos = new Vector3(hit.point.x, m_player.transform.position.y, hit.point.z);
                    m_curve.AddPoint(pointPos);
                }
                else if (Input.GetMouseButtonUp(1) && hit.collider.tag.Equals("AnchorPoint"))
                {
                    m_curve.DeletePoint(hit.collider.gameObject);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
            m_curve.HiddenLine(false);
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_curve.HiddenLine(true);
        }
    }
#endif
}

