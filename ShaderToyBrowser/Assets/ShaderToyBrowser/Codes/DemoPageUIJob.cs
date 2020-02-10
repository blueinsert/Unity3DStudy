using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace bluebean.ShaderToyBrowser
{
    public class DemoPageUIJob : UIJob
    {
        private const string m_prefab = "Assets/ShaderToyBrowser/Prefabs/DemoPageUIPrefab.prefab";
        protected override string prefab { get { return m_prefab; } set { } }

        private RawImage m_rawImage;
        public Button m_buttonBack;
        public Text m_fpsText;
        public Button m_buttonRefresh;
        public Button m_buttonSave;
        public Button m_buttonCompile;
        public InputField m_codeEditor;
        public Scrollbar m_horizontal;
        public Scrollbar m_vertical;
        public InputField m_inputName;

        float m_iTime;

        private ShaderData m_config;
        private Shader m_shader;
        private Material m_material;
        private bool m_isNew;

        protected override void InitOnCreateInstance()
        {
            base.InitOnCreateInstance();
            m_buttonBack = instance.transform.Find("ButtonBack").GetComponent<Button>();
            m_rawImage = instance.transform.Find("Left/RawImage").GetComponent<RawImage>();
            m_inputName = instance.transform.Find("Left/Name/InputField").GetComponent<InputField>();
            m_fpsText = instance.transform.Find("TextFps").GetComponent<Text>();
            m_buttonRefresh = instance.transform.Find("ButtonRefresh").GetComponent<Button>();
            m_buttonSave = instance.transform.Find("ButtonSave").GetComponent<Button>();
            m_buttonCompile = instance.transform.Find("ButtonCompile").GetComponent<Button>();
            m_codeEditor = instance.transform.Find("CodeEditor/Viewport/InputField").GetComponent<InputField>();
            m_horizontal = instance.transform.Find("CodeEditor/Horizontal").GetComponent<Scrollbar>();
            m_vertical = instance.transform.Find("CodeEditor/Vertical").GetComponent<Scrollbar>();

            m_buttonBack.onClick.AddListener(OnBackButtonClick);
            m_buttonRefresh.onClick.AddListener(OnRefreshButtonClick);
            m_buttonCompile.onClick.AddListener(OnCompileButtonClick);
            m_buttonSave.onClick.AddListener(OnSaveButtonClick);
            m_codeEditor.onEndEdit.AddListener(OnEndEdit);
            m_inputName.onEndEdit.AddListener(OnNameEndEdit);
        }

        public override void Init(CustomParams customParams)
        {
            Debug.Log("DemoPage Init");
            base.Init(customParams);
            m_isNew = (bool)customParams.GetParam("isNew");
            m_config = customParams.GetParam("config") as ShaderData;
            m_inputName.text = m_config.info.name;
            m_codeEditor.text = m_config.Renderpass0.Code;
            m_shader = ShaderHelper.LoadShader(m_config);
            if (m_shader != null) {
                m_material = new Material(m_shader);
                m_material.hideFlags = HideFlags.HideAndDontSave;
                var rect = m_rawImage.GetComponent<RectTransform>().rect;
                m_material.SetVector("iResolution", rect.size);
            }
            //创建代码的临时文件
            var stream = File.CreateText(Application.dataPath+ "/ShaderToyBrowser/Shaders/tmp/"+m_config.info.name +".shader.txt");
            stream.Write(m_config.Renderpass0.code);
            stream.Close();
        }

        public override void Execute()
        {
            Debug.Log("DemoPage Execute");
            base.Execute();
            m_rawImage.material = m_material;
        }

        IEnumerator Coro_SetScrollBar() {
            yield return new WaitForSeconds(0.1f);
            m_horizontal.value = 0;
            m_vertical.value = 1;
        }

        private void OnRefreshButtonClick() {
            var file = File.OpenText(Application.dataPath + "/ShaderToyBrowser/Shaders/tmp/" + m_config.info.name + ".shader.txt");
            string source = file.ReadToEnd();
            file.Close();
            m_config.Renderpass0.code = source;
            m_codeEditor.text = source;
        }

        private void OnCompileButtonClick() {
            ShaderHelper.ReplaceShader(m_shader, m_config);
        }

        private void OnBackButtonClick() {
            CustomParams customParams = new CustomParams();
            customParams.AddParam("from", "DemoPage");
            customParams.AddParam("shaderName", m_config.info.name);
            UIManager.Instance.PopUIJob(customParams);
        }

        private void OnEndEdit(string value) {
            Debug.Log("OnEndEdit");
            m_config.Renderpass0.code = value;
        }

        private void OnSaveButtonClick()
        {
            Debug.Log("OnSaveButtonClick");
            m_config.Renderpass0.code = m_codeEditor.text;
            var userData = ConfigLoader.Load<UserData>("/ShaderToyBrowser/Config/shaders.json");
            if (!m_isNew)
            {
                var origin = userData.shaders.FindIndex((e) => { return e.info.name == m_config.info.name; });
                userData.shaders.RemoveAt(origin);
            }
            userData.shaders.Add(m_config);
            userData.shaders.Sort((a, b) => { return string.Compare(a.info.name, b.info.name); });
            ConfigLoader.Save<UserData>("/ShaderToyBrowser/Config/shaders.json",userData);
            m_isNew = false;
        }

        private void OnNameEndEdit(string value) {
            m_config.info.name = value;
        }

        public override void Tick()
        {
            m_fpsText.text ="FPS: " + ShowFPS.Instance.FPS.ToString();
            m_iTime += Time.deltaTime;
            if (m_material != null&&Time.frameCount%2==0) {
                m_material.SetFloat("iTime", m_iTime);
            }
        }
    }
}
