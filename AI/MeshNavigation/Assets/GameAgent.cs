using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text;

public class GameAgent : MonoBehaviour
{
    // 顶一个网格导航器，然后在面板里面直接将Capsule直接拖过来
    public NavMeshAgent agent;
    NavMeshPath m_curPath = null;
    public List<Vector3> m_pathList = new List<Vector3>();
    // Start is called before the first frame update  
    void Start()
    {
        m_curPath = new NavMeshPath();
    }

    public Vector3 GetAvailableLastPointOnNavmesh(Vector3 srcpoint)
    {
        if (NavMesh.SamplePosition(srcpoint, out var hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return srcpoint;
    }

    // Update is called once per frame  
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var mousePosition = GameMainManager.Instance.mousePosition;
            var targetPos = new Vector3(mousePosition.x, 0, mousePosition.y);
            targetPos = GetAvailableLastPointOnNavmesh(targetPos);
            this.agent.SetDestination(targetPos);
            m_pathList.Clear();
            if(NavMesh.CalculatePath(this.gameObject.transform.position, targetPos, NavMesh.AllAreas, m_curPath))
            {
                var corners = m_curPath.corners;
                for (int i = 0; i < corners.Length; i++)
                {
                    m_pathList.Add(corners[i]);
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
        Gizmos.color = Color.red;
        for (int i = 0; i < m_pathList.Count; i++)
        {
            Gizmos.DrawSphere(m_pathList[i], 0.2f);
        }
    }

}
