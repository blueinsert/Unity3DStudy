using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text;

public class NavigationComp : MonoBehaviour
{
    // 顶一个网格导航器，然后在面板里面直接将Capsule直接拖过来
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
            // 这是3D拾取的过程，将屏幕点击的点转化为一个三维空间的坐标
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // 如果有点击到地面的时候，就将Capsule设置到点击的点对应的三维空间的位置
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
