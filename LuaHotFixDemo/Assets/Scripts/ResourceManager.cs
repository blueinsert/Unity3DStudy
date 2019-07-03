using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{

    private ResourceManager() { }
    public static ResourceManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ResourceManager();
            }
            return m_instance;
        }
    }
    public static ResourceManager m_instance;

    public T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }

}
