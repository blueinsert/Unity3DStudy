using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text;

public class NavigationComp : MonoBehaviour
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
        if (Input.GetMouseButtonDown(0))
        {
            // ����3Dʰȡ�Ĺ��̣�����Ļ����ĵ�ת��Ϊһ����ά�ռ������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // ����е���������ʱ�򣬾ͽ�Capsule���õ�����ĵ��Ӧ����ά�ռ��λ��
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                this.agent.SetDestination(hit.point);
                NavMeshPath path = new NavMeshPath();
                m_curPath = path;
                if (agent.CalculatePath(hit.point, path))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("path:[");
                    var corners = path.corners;
                    for(int i = 0; i < corners.Length; i++)
                    {
                        sb.Append(corners[i].ToString()).Append(",");
                    }
                    sb.Append("]");
                    Debug.Log(sb.ToString());
                }
            }
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
