using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace bluebean
{
    /// <summary>
    /// 画布区域的运行时数据
    /// </summary>
    public class CanvasContext : SubAreaContext
    {
        /// <summary>
        /// 逻辑空间转画布空间矩阵
        /// </summary>
        private Matrix4x4 m_matrix = Matrix4x4.identity;
        /// <summary>
        /// 偏移
        /// </summary>
        public Vector2 m_offset = Vector2.zero;
        /// <summary>
        /// 缩放
        /// </summary>
        public float m_scale = 1;

        private Vector2 m_mousePosition = Vector2.zero;

        private Color m_curDrawColor = Color.black;

        private Font m_curFont = null;

        private Rect m_logicRect;

        public float Scale {
            get { return m_scale; }
            set
            {
                m_scale = Mathf.Clamp(value, 0.1f, 3);
            }
        }

        public Matrix4x4 Matrix
        {
            get
            {
                Vector2 translate = Size / 2 + m_offset;
                m_matrix.SetTRS(translate, Quaternion.identity, new Vector3(1 / m_scale, -1 / m_scale, 1));
                return m_matrix;
            }
        }

        /// <summary>
        /// 鼠标位置，逻辑空间
        /// </summary>
        public Vector2 MousePosition { get { return m_mousePosition; } set { m_mousePosition = value; } }
        
        public Color Color { get { return m_curDrawColor; } set { m_curDrawColor = value; } }

        /// <summary>
        /// 逻辑空间中的范围
        /// </summary>
        public Rect LogicRect {
            get {
                m_logicRect = new Rect(CanvasToLogicPos(new Vector2(0,Height)), Size * m_scale);
                return m_logicRect;
            }
        }

        private Material m_textureMaterial;

        public Material TextureMaterial {
            get
            {
                if (m_textureMaterial == null) {
                    CreateTextureMaterial();
                }
                return m_textureMaterial;
            }
        }

        void CreateTextureMaterial()
        {
            //如果材质球不存在
            if (!m_textureMaterial)
            {
                //用代码的方式实例一个材质球
                Shader shader = Shader.Find("zzx/Image");
                m_textureMaterial = new Material(shader);
                m_textureMaterial.hideFlags = HideFlags.HideAndDontSave; 
            }
        }

        //划线使用的材质球
        private Material m_lineMaterial;
        public Material LineMaterial {
            get
            {
                if (m_lineMaterial == null)
                    CreateLineMaterial();
                return m_lineMaterial;
            }
        }
        /// <summary>
        /// 创建一个材质球
        /// </summary>
        private void CreateLineMaterial()
        {
            //如果材质球不存在
            if (!m_lineMaterial)
            {
                //用代码的方式实例一个材质球
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                m_lineMaterial = new Material(shader);
                m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                m_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m_lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                m_lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        //划线使用的材质球
        private Material m_fontMaterial;
        public Material FontMaterial
        {
            get
            {
                if (m_fontMaterial == null)
                    CreateFontMaterial();
                return m_fontMaterial;
            }
        }

        /// <summary>
        /// 创建一个材质球
        /// </summary>
        private void CreateFontMaterial()
        {
            //如果材质球不存在
            if (!m_fontMaterial)
            {
                //用代码的方式实例一个材质球
                Shader shader = Shader.Find("zzx/Font");
                m_fontMaterial = new Material(shader);
                m_fontMaterial.hideFlags = HideFlags.HideAndDontSave; 
            }
        }

        public CanvasContext(IWindow owner, SubAreaDef areaDef) : base(owner, areaDef)
        {
        }

        public Vector2 LogicToCanvasPos(Vector2 pos)
        {
            var res = Matrix * new Vector4(pos.x,pos.y,0,1);
            return new Vector2(res.x,res.y);
        }

        public Vector2 CanvasToLogicPos(Vector2 pos) {
            var res = Matrix.inverse * new Vector4(pos.x, pos.y, 0, 1);
            return new Vector2(res.x, res.y);
        }

        private void PushMatrix()
        {
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.MultMatrix(Matrix);
        }

        private void PopMatrix() {
            GL.PopMatrix();
        }

        public void DrawBezier(Vector2 start, Vector2 end,float lineSize, Color color) {
            PushMatrix();
            LineMaterial.SetPass(0);

            Vector2 endToStart = (end - start);
            float dirFactor = Mathf.Clamp(endToStart.magnitude, 20f, 80f);

            endToStart.Normalize();
            Vector2 project = Vector3.Project(endToStart, Vector3.right);

            Vector2 startTan = start + project * dirFactor;
            Vector2 endTan = end - project * dirFactor;

            UnityEditor.Handles.DrawBezier(start, end, startTan, endTan, color, null, lineSize);

            PopMatrix();
        }

        public void DrawTexture(Rect position, Texture2D texture) {
            PushMatrix();
            TextureMaterial.SetTexture("_MainTex", texture);
            TextureMaterial.SetPass(0);
            GLHelper.DrawRectSolid(position.position, position.position + position.size, Color);
            PopMatrix();
        }

        public void DrawGrid() {
            PushMatrix();
            LineMaterial.SetPass(0);
            //in canvas coordinate
            Vector2 leftDown = new Vector2(0, Height);
            Vector2 rightUp = new Vector2(Width, 0);
            var p1 = CanvasToLogicPos(leftDown);
            var p2 = CanvasToLogicPos(rightUp);
            GLHelper.DrawGrid(100, new Rect(p1, p2 - p1),Color.black);
            PopMatrix();
        }

        public void DrawSolidTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Color color) {
            PushMatrix();
            LineMaterial.SetPass(0);
            GLHelper.DrawSolidTriangle(p1, p2, p3, color);
            PopMatrix();
        }

        public void DrawRectSolid(Vector2 center, float width, float height, Color color) {
            PushMatrix();
            LineMaterial.SetPass(0);
            GLHelper.DrawRectSolid(center, width, height, color);
            PopMatrix();
        }

        public void DrawCircleSolid(Vector2 pos, float radius, Color color) {
            PushMatrix();
            LineMaterial.SetPass(0);
            GLHelper.DrawCircleSolid(pos, radius, 36, color);
            PopMatrix();
        }

        public void SetFont(string name) {
            m_curFont = FontLib.GetFont(name);
        }

        public void DrawText(Rect rect, string text,float fontSize)
        {
            PushMatrix();
            //m_curFont.material.SetPass(0);
            FontMaterial.SetTexture("_MainTex", m_curFont.material.GetTexture("_MainTex"));
            FontMaterial.SetPass(0);
            List<CharacterInfo> characterInfoList = new List<CharacterInfo>();
            for (int i = 0; i < text.Length; i++) {
                CharacterInfo characterInfo;
                char c = text[i];
                if (!m_curFont.GetCharacterInfo(c, out characterInfo))
                {
                    m_curFont.GetCharacterInfo('*', out characterInfo);
                }
                characterInfoList.Add(characterInfo);
            }
            GLHelper.DrawTextQuads(rect, characterInfoList.ToArray(), fontSize, Color);
            PopMatrix();
        }
    }
}
