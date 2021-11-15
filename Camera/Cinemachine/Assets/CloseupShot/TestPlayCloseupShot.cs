using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestPlayCloseupShot : MonoBehaviour
{
    public GameObject m_player = null;
    public Cinemachine.CinemachineBrain m_brain;
    public Dictionary<string, GameObject> m_closeupShotDic = new Dictionary<string, GameObject>();

    private Dictionary<string, string> m_resourceDesc = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        m_resourceDesc.Add("Test", "TestCloseupShot");
    }

    public void PlayCloseupShot(GameObject root)
    {
        var desc = root.GetComponent<CloseupShotDesc>();
        desc.m_playerTransformTime0.transform.position = m_player.transform.position;
        desc.m_playerTransformTime0.transform.rotation = m_player.transform.rotation;
        var director = desc.m_director;
        director.gameObject.SetActive(true);
        foreach (var binding in director.playableAsset.outputs)
        {
            if (binding.streamName == "Cinemachine Track")
            {
                director.SetGenericBinding(binding.sourceObject, m_brain);
            }
        }
        director.Stop();
        director.Play();
    }

    public void PlayCloseupShot(string key)
    {
        if(m_closeupShotDic.ContainsKey(key))
        {
            PlayCloseupShot(m_closeupShotDic[key]);
            return;
        }
        var path = m_resourceDesc[key];
        var prefab = Resources.Load<GameObject>(path);
        if (prefab.GetComponent<CloseupShotDesc>() == null)
            return;
        var go = Instantiate(prefab, this.transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        m_closeupShotDic.Add(key, go);
        PlayCloseupShot(go);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayCloseupShot("Test");
        }
    }
}
