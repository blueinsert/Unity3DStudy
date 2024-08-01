using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace bluebean
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class VertexPaintTool : MonoBehaviour, IVertexPropertyHolder
    {
        public SkinnedMeshRenderer m_skinMeshRender = null;

        public Vector4[] m_properties = null;

        public Vector4[] VertexProperties
        {
            get
            {
                if(m_properties == null || m_properties.Length == 0)
                {
                    var count = m_skinMeshRender.sharedMesh.vertices.Length;
                    m_properties = new Vector4[count];
                }
                return m_properties;
            }
        }

        public Vector4 GetVertexProperty(int index)
        {
            return VertexProperties[index];
        }

        public void SetVertexProperty(int index, Vector4 value)
        {
            VertexProperties[index] = value;
        }

        public IVertexPropertyHolder GetVertexPropertyHolder()
        {
            return this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Clear()
        {
           for(int i=0;i< m_properties.Length; i++)
            {
                m_properties[i] = Vector4.zero;
            }
        }
    }
}
