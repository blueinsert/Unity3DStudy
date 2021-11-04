using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GenerateShake : MonoBehaviour
{
    public CinemachineImpulseSource m_impulseSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("GenerateImpulse");
            m_impulseSource.GenerateImpulse();
        }
    }
}
