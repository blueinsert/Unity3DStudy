using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text;

public class GameAgent : MonoBehaviour
{
    // ��һ�����񵼺�����Ȼ�����������ֱ�ӽ�Capsuleֱ���Ϲ���
    public NavMeshAgent agent;
    NavMeshPath m_curPath;
    // Start is called before the first frame update  
    void Start()
    {

    }

    // Update is called once per frame  
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var mousePosition = GameMainManager.Instance.mousePosition;
            this.agent.SetDestination(new Vector3(mousePosition.x, 0, mousePosition.y));
        }
    }

    private void OnDrawGizmos()
    {
        if (m_curPath == null)
        {
            return;
        }
        Gizmos.color = Color.blue;
        var corners = m_curPath.corners;
        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawSphere(corners[i], 0.1f);
        }
    }

}
