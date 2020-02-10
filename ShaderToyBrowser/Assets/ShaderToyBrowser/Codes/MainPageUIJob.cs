using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace bluebean.ShaderToyBrowser
{
    public class MainPageUIJob : UIJob
    {
        private const string m_prefab = "Assets/ShaderToyBrowser/Prefabs/MainPageUIPrefab.prefab";
        protected override string prefab { get { return m_prefab; } set { } }

        private Button m_buttonNew;
        private GameObject m_demoItemPrefab;
        private GameObject m_content;
        private GameObjectPool<DemoItemUIController> m_gameObjectPool;

        private string m_forceUpdateShader;

        protected override void InitOnCreateInstance()
        {
            base.InitOnCreateInstance();
            m_buttonNew = instance.transform.Find("ButtonNew").GetComponent<Button>();
            m_demoItemPrefab = instance.transform.Find("Prefabs/PrefabDemoItem").gameObject;
            m_content = instance.transform.Find("Scroll/Viewport/Content").gameObject;
            m_gameObjectPool = new GameObjectPool<DemoItemUIController>();
            m_gameObjectPool.Setup(m_demoItemPrefab, m_content);

            m_buttonNew.onClick.AddListener(OnButtonNewClick);
        }

        public override void Init(CustomParams customParams)
        {
            Debug.Log("MainPage Init");
            m_forceUpdateShader = "";
            base.Init(customParams);
            var from = customParams.GetParam("from");
            if (from != null) {
                if (from as string == "DemoPage") {
                    m_forceUpdateShader = customParams.GetParam("shaderName") as string;
                }
            }
        }

        public override void Execute()
        {
            Debug.Log("MainPage Execute");
            m_gameObjectPool.Deactive();
            var userData = ConfigLoader.Load<UserData>("/ShaderToyBrowser/Config/shaders.json");
            foreach (var config in userData.shaders)
            {
                bool isNew = false;
                var ctrl = m_gameObjectPool.Allocate(out isNew);
                if (isNew)
                {
                    ctrl.EventOnClick += OnDemoItemClick;
                }
                if(string.IsNullOrEmpty(m_forceUpdateShader))
                    ctrl.SetInfo(config);
                else
                {
                    if(config.info.name == m_forceUpdateShader)
                        ctrl.SetInfo(config);
                }
            }
        }

        private void OnDemoItemClick(ShaderData config)
        {
            Debug.Log("OnDemoItemClick");
            CustomParams customParams = new CustomParams();
            customParams.AddParam("config", config);
            customParams.AddParam("isNew", false);
            UIManager.Instance.PushUIJob<DemoPageUIJob>(customParams);
        }

        public void OnButtonNewClick() {
            Debug.Log("OnDemoItemClick");
            CustomParams customParams = new CustomParams();
            customParams.AddParam("config", new ShaderData());
            customParams.AddParam("isNew", true);
            UIManager.Instance.PushUIJob<DemoPageUIJob>(customParams);
        }
    }

    public class DemoItemUIController : MonoBehaviour
    {
        public RawImage m_showImg;
        public Text m_nameText;
        public Button m_button;

        public ShaderData m_config;
        private Shader m_shader;
        private Material m_material;
        RenderTexture m_renderTexture;

        public event Action<ShaderData> EventOnClick;

        private void Awake()
        {
            m_button = transform.GetComponent<Button>();
            m_showImg = transform.Find("ShowImage").GetComponent<RawImage>();
            m_nameText = transform.Find("TextName").GetComponent<Text>();
            m_button.onClick.AddListener(OnButtonClick);
        }

        public void SetInfo(ShaderData config)
        {
            m_config = config;
            m_nameText.text = config.info.name;

            m_shader = ShaderHelper.LoadShader(m_config);
            if (m_shader != null)
            {
                if (m_renderTexture == null)
                    m_renderTexture = RenderTexture.GetTemporary(320, 240);
                m_material = new Material(m_shader);
                m_material.hideFlags = HideFlags.HideAndDontSave;
                var rect = m_showImg.GetComponent<RectTransform>().rect;
                m_material.SetVector("iResolution", rect.size);
                var m_texture2D = new Texture2D(320, 240);
                UnityEngine.Graphics.Blit(m_texture2D, m_renderTexture, m_material);
                m_showImg.texture = m_renderTexture;
            }

        }

        private void OnButtonClick()
        {
            if (EventOnClick != null)
            {
                EventOnClick(m_config);
            }
        }
    }
}
