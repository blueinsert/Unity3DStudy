using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.ShaderToyBrowser
{
    public partial class GameObjectPool<T> where T : MonoBehaviour
    {
        public void Setup(GameObject prefab, GameObject parent)
        {
            m_prefab = prefab;
            m_parent = parent;
        }

        public T Allocate()
        {
            bool isNew;
            return Allocate(out isNew);
        }

        public T Allocate(out bool isNew)
        {
            isNew = false;
            if (m_prefab == null)
                return null;

            foreach (var c in m_list)
            {
                if (!c.gameObject.activeSelf)
                {
                    c.gameObject.SetActive(true);
                    return c;
                }
            }

            GameObject go = GameObject.Instantiate(m_prefab);
            if (go != null)
            {
                go.SetActive(true);
                T c = null;
                c = go.AddComponent<T>();

                if (c != null)
                {
                    if (m_parent != null)
                    {
                        go.transform.SetParent(m_parent.transform, false);
                    }
                    m_list.Add(c);
                    isNew = true;
                    return c;
                }
            }
            return null;
        }

        public void Deactive()
        {
            foreach (var c in m_list)
            {
                c.gameObject.SetActive(false);
            }
        }

        public void Destroy()
        {
            foreach (var c in m_list)
            {
                UnityEngine.Object.Destroy(c.gameObject);
            }
            m_list.Clear();
        }

        public List<T> GetList()
        {
            return m_list;
        }

        List<T> m_list = new List<T>();
        GameObject m_prefab;
        GameObject m_parent;
    }
}
