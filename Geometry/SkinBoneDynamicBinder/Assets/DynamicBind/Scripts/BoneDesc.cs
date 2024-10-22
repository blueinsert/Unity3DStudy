using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneDesc : MonoBehaviour
{
    [Tooltip("The maximum distance a cluster can be from a vertex before it will not influence it any more.")]
    [Header("�������Ӱ�����")]
    public float m_skinningMaxDistance = 0.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawWireSphere(this.transform.position, m_skinningMaxDistance);
    }
}
