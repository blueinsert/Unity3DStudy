using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavigationComp : MonoBehaviour
{
    // ��һ�����񵼺�����Ȼ�����������ֱ�ӽ�Capsuleֱ���Ϲ���
    public NavMeshAgent agent;
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
            }
        }
    }  

}
