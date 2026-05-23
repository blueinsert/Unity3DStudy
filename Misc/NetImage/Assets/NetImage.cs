using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Image))]
public class NetworkImage : MonoBehaviour
{
    // 在 Inspector 中拖入你的 Image 组件
    private Image targetImage;
    public string m_url;
    public bool m_setOnAwake = true;
    private Coroutine m_curCoroutine = null;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
        if (m_setOnAwake)
        {
            if (!string.IsNullOrEmpty(m_url))
            {
                m_curCoroutine = StartCoroutine(LoadImageFromURL(m_url));
            }
        }
    }

    void Start()
    {
        // 直接调用加载方法，传入图片URL
    }


    public void SetImage(string url)
    {
        if (m_curCoroutine != null)
        {
           StopCoroutine(m_curCoroutine);
            m_curCoroutine = null;
        }
        m_url = url;
        if (!string.IsNullOrEmpty(m_url))
        {
            m_curCoroutine = StartCoroutine(LoadImageFromURL(m_url));
        }
    }

    IEnumerator LoadImageFromURL(string url)
    {
        // 1. 使用 UnityWebRequest 发起网络请求，下载图片纹理
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        // 2. 检查是否下载成功
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("图片加载失败: " + request.error);
        }
        else
        {
            // 3. 下载成功，获取 Texture2D 对象
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // 4. 将 Texture2D 转换为 Sprite
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height), // 使用整张图片
                new Vector2(0.5f, 0.5f) // 精灵的锚点，设为图片中心
            );

            // 5. 最后一步！将生成的 Sprite 赋给 Image 组件的 sprite 属性，完成显示
            targetImage.sprite = sprite;
        }

        m_curCoroutine = null;
    }
}