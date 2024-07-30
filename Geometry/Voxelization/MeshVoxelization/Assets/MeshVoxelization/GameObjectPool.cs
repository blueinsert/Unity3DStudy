using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bluebean
{
    /// <summary>
    /// Deactive的GameObject可以被重复使用
    /// </summary>
    public class GameObjectPool<T> where T : MonoBehaviour
    {
        public void Setup(GameObject prefab, GameObject parent)
        {
            m_prefab = prefab;
            m_parent = parent;
        }

        public void SetupParent(GameObject parent)
        {
            m_parent = parent;
        }

        public T Allocate()
        {
            bool isNew;
            return Allocate(out isNew);
        }

        public T Allocate(Action<T> OnNewItem)
        {
            bool isNew;

            T t = Allocate(out isNew);
            if (isNew)
                OnNewItem(t);

            return t;
        }

        public T Allocate(out bool isNew)
        {
            isNew = false;
            if (m_prefab == null)
            {
                Debug.LogError("GameObjectPool.Allocate m_prefab is null.");
                return null;
            }

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
                T c = go.GetComponent<T>();
                if (c == null)
                {
                    {
                        c = go.AddComponent<T>();
                    }
                }

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

        public void Deactive(bool resetParent)
        {
            foreach (var c in m_list)
            {
                if (resetParent)
                    c.transform.SetParent(m_parent.transform, false);
                c.gameObject.SetActive(false);
            }
        }

        public void ReturnObjectToPool(T item, bool resetParent = false)
        {
            if (resetParent)
                item.transform.SetParent(m_parent.transform, false);
            item.gameObject.SetActive(false);
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

        public GameObject Parent { get { return m_parent; } }

        List<T> m_list = new List<T>();
        GameObject m_prefab;
        GameObject m_parent;
    }
}