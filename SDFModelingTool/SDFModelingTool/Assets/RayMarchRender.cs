using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace bluebean
{
    public class RayMarchRender
    {
        public const int Width = 512;
        public const int Height = 288;
        public const float RaymarchDrawDistance = 40;
        public const int NodesCapacity = 88;

        private RenderTexture m_resultTexture;

        int m_kernelIndex;
        public ComputeShader m_computeShader;
        ComputeBuffer m_cbNodes;
        ComputeBuffer m_cbTreeNodes;

        public int m_nodeCount;
        public _Node[] m_nodes;
        public int m_treeNodeCount;
        public _TreeNode[] m_treeNodes;
        public int m_treeRoot;
        int m_postOrderFirst;

        public void Initial() {
            m_computeShader = ComputeShaderLib.GetComputeShader("RayMarch");

            m_resultTexture = new RenderTexture(Width, Height, 24);
            m_resultTexture.enableRandomWrite = true;
            m_resultTexture.Create();

            m_nodes = new _Node[NodesCapacity];
            m_treeNodes = new _TreeNode[NodesCapacity];
            m_cbNodes = new ComputeBuffer(NodesCapacity, Marshal.SizeOf(typeof(_Node)));
            m_cbTreeNodes = new ComputeBuffer(NodesCapacity, Marshal.SizeOf(typeof(_TreeNode)));
            m_nodeCount = 0;
            m_treeNodeCount = 0;
        }

        public void ReloadComputeBuff()
        {
            m_cbNodes.SetData(m_nodes);
            m_cbTreeNodes.SetData(m_treeNodes);
        }

        public void Render() {
            //m_computeShader.SetVector("_CameraWS", CurrentCamera.transform.position);
            //m_computeShader.SetMatrix("_FrustumCornersES", GetFrustumCorners(CurrentCamera));
            //m_computeShader.SetMatrix("_CameraInvViewMatrix", CurrentCamera.cameraToWorldMatrix);
            //m_computeShader.SetFloat("_RayMarchDrawDistance", _RaymarchDrawDistance);
            //m_computeShader.SetVector("_LightDir", SunLight ? SunLight.forward : Vector3.down);

            m_kernelIndex = m_computeShader.FindKernel("CSMain");
            m_computeShader.SetTexture(m_kernelIndex, "outputTexture", m_resultTexture);
            m_computeShader.SetBuffer(m_kernelIndex, "_NodesDef", m_cbNodes);
            m_computeShader.SetBuffer(m_kernelIndex, "_TreeNodesDef", m_cbTreeNodes);
            m_computeShader.SetInt("_NodeCount", m_nodeCount);
            m_computeShader.SetInt("_TreeNodeCount", m_treeNodeCount);
            m_computeShader.SetInt("_TreeRoot", m_treeRoot);
            m_computeShader.SetInt("_PostOrderFirst", m_postOrderFirst);

            m_computeShader.Dispatch(m_kernelIndex, Width, Height, 1);
        }

        public RenderTexture GetRenderTexture() {
            return m_resultTexture;
        }

    }
}
