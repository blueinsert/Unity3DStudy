using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace bluebean {
    [RequireComponent(typeof(Camera))]
    public class RayMarchSceneRender : SceneViewFilter
    {
        public Transform SunLight;

        const int Width = 512;
        const int Height = 288;
        const float _RaymarchDrawDistance = 40;
        public Camera CurrentCamera
        {
            get
            {
                if (!_CurrentCamera)
                    _CurrentCamera = GetComponent<Camera>();
                return _CurrentCamera;
            }
        }
        private Camera _CurrentCamera;

        public ComputeShader m_computeShader;
        private RenderTexture m_tempTexture;
        int m_kernelIndex;

        public int m_nodeCount;
        const int NodesCapacity = 88;
        public _Node[] m_nodes;
        const int TreeNodeCapacity = 88;
        public int m_treeNodeCount;
        public _TreeNode[] m_treeNodes;
        public int m_treeRoot;
        int m_postOrderFirst;

        ComputeBuffer m_cbNodes;
        ComputeBuffer m_cbTreeNodes;

        public static RayMarchSceneRender Instance
        {
            get
            {
                return m_instance;
            }
        }

        private static RayMarchSceneRender m_instance;

        private void Awake()
        {
            m_tempTexture = new RenderTexture(Width, Height, 24);
            m_tempTexture.enableRandomWrite = true;
            m_tempTexture.Create();

            m_nodes = new _Node[NodesCapacity];
            m_treeNodes = new _TreeNode[TreeNodeCapacity];
            m_cbNodes = new ComputeBuffer(NodesCapacity, Marshal.SizeOf(typeof(_Node)));
            m_cbTreeNodes = new ComputeBuffer(TreeNodeCapacity, Marshal.SizeOf(typeof(_TreeNode)));
            //测试 构造树形结构
            m_nodes[0] = new _Node() { type = (int)NodeType.Union };
            m_nodes[1] = new _Node() { type = (int)NodeType.Position, param1 = 2 };
            m_nodes[2] = new _Node() { type = (int)NodeType.Sphere, param4 = 1 };
            m_nodes[3] = new _Node() { type = (int)NodeType.Sphere, param4 = 2};
            m_treeNodes[0] = new _TreeNode() { parent = -1, index = 0, data = 0, lchild = 1, rchild = 3 };
            m_treeNodes[1] = new _TreeNode() { parent = 0, index = 1, data = 1, lchild = 2, rchild = -1 };
            m_treeNodes[2] = new _TreeNode() { parent = 1, index = 2, data = 2, lchild = -1, rchild = -1 };
            m_treeNodes[3] = new _TreeNode() { parent = 0, index = 3, data = 3, lchild = -1, rchild = -1 };
            m_treeRoot = 0;
            InitTree(m_treeNodes, m_treeRoot, out m_postOrderFirst);
            m_nodeCount = 2;
            m_treeNodeCount = 4;
            m_cbNodes.SetData(m_nodes);
            m_cbTreeNodes.SetData(m_treeNodes);

            m_instance = this;
        }

        public void UpdateDS()
        {
            InitTree(m_treeNodes, m_treeRoot, out m_postOrderFirst);
            m_cbNodes.SetData(m_nodes);
            m_cbTreeNodes.SetData(m_treeNodes);
        }

        public void RebuildNodeTransform() {
            _TreeNode[] tree = m_treeNodes;
            int root = m_treeRoot;
            //进行广度遍历，初始化每个节点的变换
            Queue<_TreeNode> queue = new Queue<_TreeNode>();
            queue.Enqueue(tree[root]);
            while (queue.Count > 0)
            {
                _TreeNode node = queue.Dequeue();
                //do something
                {
                    Matrix4x4 parentTransform = Matrix4x4.identity;
                    if (node.parent != -1)
                    {
                        parentTransform = m_nodes[tree[node.parent].data].transform;
                    }

                    _Node opNode = m_nodes[node.data];
                    if (opNode.type == (int)NodeType.Position)
                    {
                        parentTransform *= Matrix4x4.Translate(new Vector3(-opNode.param1, -opNode.param2, -opNode.param3));
                    }
                    else if (opNode.type == (int)NodeType.Rotate)
                    {
                        parentTransform *= Matrix4x4.Rotate(Quaternion.Euler(new Vector3(-opNode.param1, -opNode.param2, -opNode.param3)));
                    }
                    else if (opNode.type == (int)NodeType.Scale)
                    {
                        parentTransform *= Matrix4x4.Scale(new Vector3(1/opNode.param4, 1 / opNode.param4, 1 / opNode.param4));
                    }

                    m_nodes[tree[node.index].data].transform = parentTransform;
                }
                //add children
                if (node.lchild != -1)
                {
                    queue.Enqueue(tree[node.lchild]);
                }
                if (node.rchild != -1)
                {
                    queue.Enqueue(tree[node.rchild]);
                }
            }
        }

        public void ReloadComputeBuff() {
            m_cbNodes.SetData(m_nodes);
            m_cbTreeNodes.SetData(m_treeNodes);
        }

        private void InitTree(_TreeNode[] tree, int root, out int postOrderFirst)
        {
            RebuildNodeTransform();

            postOrderFirst = -1;
            //2.后续遍历，建立后续遍历的次序线索
            Stack<int> stack = new Stack<int>();
            int cur = root, lastVisit = -1;
            //移到最左节点
            while (cur != -1)
            {
                stack.Push(cur);
                cur = tree[cur].lchild;
            }
            while (stack.Count > 0)
            {
                cur = stack.Pop();
                _TreeNode node = tree[cur];
                if (node.rchild == -1 || node.rchild == lastVisit)
                {
                    //do something
                    {
                        if (lastVisit != -1)
                            tree[lastVisit].next = cur;
                        //记录后续遍历的第一个元素
                        if (postOrderFirst == -1)
                        {
                            postOrderFirst = cur;
                        }
                    }
                    lastVisit = cur;
                }
                else
                {
                    stack.Push(cur);
                    cur = tree[cur].rchild;
                    while (cur != -1)
                    {
                        stack.Push(cur);
                        cur = tree[cur].lchild;
                    }
                }
            }
            tree[root].next = -1;
        }

        /// <summary>
        /// 返回在摄像机坐标系中定义的由摄像机位置指向近裁剪面的四个顶点的四个向量组成的矩阵
        /// 第一行：指向左上顶点的向量
        /// 第二行：指向右上顶点的向量
        /// 第三行：指向右下顶点的向量
        /// 第四行：指向左下顶点的向量
        /// </summary>
        private Matrix4x4 GetFrustumCorners(Camera cam)
        {
            float camFov = cam.fieldOfView;
            float camAspect = cam.aspect;

            Matrix4x4 frustumCorners = Matrix4x4.identity;

            float fovWHalf = camFov * 0.5f;

            float tan_fov = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 toRight = Vector3.right * tan_fov * camAspect;
            Vector3 toTop = Vector3.up * tan_fov;

            Vector3 topLeft = (-Vector3.forward - toRight + toTop);
            Vector3 topRight = (-Vector3.forward + toRight + toTop);
            Vector3 bottomRight = (-Vector3.forward + toRight - toTop);
            Vector3 bottomLeft = (-Vector3.forward - toRight - toTop);

            frustumCorners.SetRow(0, topLeft);
            frustumCorners.SetRow(1, topRight);
            frustumCorners.SetRow(2, bottomRight);
            frustumCorners.SetRow(3, bottomLeft);

            return frustumCorners;
        }

        private void OnGUI()
        {
            //if (GUI.Button(new Rect(0, 0, 200, 60), "OpenEditor")) {
            //}
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            //在scene视图中绘制由摄像机指向视椎体四个顶点的向量
            Matrix4x4 corners = GetFrustumCorners(CurrentCamera);
            Vector3 pos = CurrentCamera.transform.position;

            for (int x = 0; x < 4; x++)
            {
                //将向量从摄像机坐标系转到世界坐标系
                corners.SetRow(x, CurrentCamera.cameraToWorldMatrix * corners.GetRow(x));
                Gizmos.DrawLine(pos, pos + (Vector3)(corners.GetRow(x)));
            }
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            m_computeShader.SetVector("_CameraWS", CurrentCamera.transform.position);
            m_computeShader.SetMatrix("_FrustumCornersES", GetFrustumCorners(CurrentCamera));
            m_computeShader.SetMatrix("_CameraInvViewMatrix", CurrentCamera.cameraToWorldMatrix);
            m_computeShader.SetFloat("_RayMarchDrawDistance", _RaymarchDrawDistance);
            m_computeShader.SetVector("_LightDir", SunLight ? SunLight.forward : Vector3.down);

            m_kernelIndex = m_computeShader.FindKernel("CSMain");
            m_computeShader.SetTexture(m_kernelIndex, "outputTexture", m_tempTexture);
            m_computeShader.SetBuffer(m_kernelIndex, "_NodesDef", m_cbNodes);
            m_computeShader.SetBuffer(m_kernelIndex, "_TreeNodesDef", m_cbTreeNodes);
            m_computeShader.SetInt("_NodeCount", m_nodeCount);
            m_computeShader.SetInt("_TreeNodeCount", m_treeNodeCount);
            m_computeShader.SetInt("_TreeRoot", m_treeRoot);
            m_computeShader.SetInt("_PostOrderFirst", m_postOrderFirst);

            m_computeShader.Dispatch(m_kernelIndex, Width, Height, 1);
            Graphics.Blit(m_tempTexture, destination);
            return;
        }
    }
}
