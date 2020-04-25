using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class FakeDepthUIImage:MonoBehaviour
    {
        [Range(0,1)]
        public float m_depthLeft;
        [Range(0, 1)]
        public float m_depthRight;
        [Range(0, 1)]
        public float m_depthTop;
        [Range(0, 1)]
        public float m_depthBottom;
        [Range(0, 1)]
        public float m_horizonDegree;

        private Image m_targetImage;
        
        private Image Image
        {
            get
            {
                if (m_targetImage == null)
                {
                    m_targetImage = this.GetComponent<Image>();
                }
                return m_targetImage;
            }
        }

        private string GetShaderName()
        {
            return "ProjectL/UI_FakeDepth";
        }

        private void ReplaceMaterial()
        {
            if(Image.material.shader.name!= GetShaderName())
            {
                Image.material = new Material(Shader.Find(GetShaderName()));
            }
        }

        private void UpdateMaterial()
        {
            ReplaceMaterial();
            Image.material.SetFloat("_DepthLeft", m_depthLeft);
            Image.material.SetFloat("_DepthRight", m_depthRight);
            Image.material.SetFloat("_DepthTop", m_depthTop);
            Image.material.SetFloat("_DepthBottom", m_depthBottom);
            Image.material.SetFloat("_HorizonDegree", m_horizonDegree);
        }

        private void Awake()
        {
            UpdateMaterial();
        }

        private void OnEnable()
        {
            UpdateMaterial();
        }

        private void Update()
        {
            if(Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.LinuxEditor
                || Application.platform == RuntimePlatform.OSXEditor)
            {
                UpdateMaterial();
            }
        }
    }
