using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CopyTransform : MonoBehaviour
{
    public Transform m_target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target != null)
        {
            this.transform.position = m_target.transform.position;
            this.transform.rotation = m_target.transform.rotation;
        }
    }
}
