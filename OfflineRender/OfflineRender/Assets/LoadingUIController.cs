using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour {
    public GameObject m_uiRootPrefab;
    public GameObject m_someUIPrefab;
    public Image m_image;
    public Text m_text;
    public Image m_loadProcessImage;

    private float m_loadProgress;
    private RenderTexture m_renderTexture;
    private const string m_fileName = "Capture.png";
    
	// Use this for initialization
	void Start () {
        var uiRoot = GameObject.Instantiate(m_uiRootPrefab);
        var camera = uiRoot.GetComponent<Camera>();
        var canvas = uiRoot.GetComponentInChildren<Canvas>();
        var testUI = GameObject.Instantiate(m_someUIPrefab, canvas.transform, false).GetComponent<SomeUIController>();
        testUI.DoSomething();
        m_renderTexture = new RenderTexture(320, 240, 24);
        camera.targetTexture = m_renderTexture;
        StartCoroutine(CaptureFrame(() => {
            GameObject.Destroy(uiRoot);
            m_text.text = "loading complete!";
            StartCoroutine(LoadImageByWWW((sprite) => {
                m_image.sprite = sprite;
            }));
        }));
        
    }

    IEnumerator LoadImageByWWW(Action<Sprite> onSuccess)
    {
        string path = Application.temporaryCachePath + "/" + m_fileName;
        WWW www = new WWW("file://" + path);
        yield return www;
        if(www!=null && string.IsNullOrEmpty(www.error))
        {
            Texture2D texture = www.texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (onSuccess != null)
            {
                onSuccess(sprite);
            }
        }else if(www != null && !string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("LoadImage Error:" + www.error);
        }
    }

    IEnumerator CaptureFrame(Action onEnd)
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture2D = CreateFrom(m_renderTexture);
        string path = Application.temporaryCachePath + "/" + m_fileName;
        Save(path, texture2D);
        float timer = 0;
        while (timer < 5)
        {
            yield return null;
            timer += Time.deltaTime;
            m_loadProgress = timer / 5.0f;
        }
        if (onEnd != null)
            onEnd();
    }

    private void Save(string path, Texture2D texture2D)
    {
        Debug.Log("Save Path:" + path);
        var bytes = texture2D.EncodeToPNG();
        //var bytes = texture2D.EncodeToJPG();
        System.IO.File.WriteAllBytes(path, bytes);
    }

    private Texture2D CreateFrom(RenderTexture renderTexture)
    {
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        var previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        RenderTexture.active = previous;

        texture2D.Apply();

        return texture2D;
    }

    // Update is called once per frame
    void Update () {
        m_loadProcessImage.fillAmount = m_loadProgress;

    }
}
