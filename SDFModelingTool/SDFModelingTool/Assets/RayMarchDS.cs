using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{

    public enum NodeType
    {
        None=0,
        Sphere,
        Plane,
        Box,
        RoundBox,
        Torus,
        Cylinder,
        Cone,
        HexPrism,
        TriPrism,
        Capsule,
        Triangle,
        Quad,
        Union,
        Subtraction,
        Intersection,
        Position,
        Rotate,
        Scale,
    }

    public struct _Node
    {
        public int type;
        public float param1;
        public float param2;
        public float param3;
        public float param4;
        public float param5;
        public float param6;
        public float param7;
        public float param8;
        public float param9;
        public float param10;
        public float param11;
        public float param12;
        public Matrix4x4 transform; //应用于p的变换
    }

    public struct _TreeNode
    {
        public int parent;//父节点索引
        public int index;//树节点数组中的索引
        public int data;//在node数组中的索引
        public int lchild;
        public int rchild;
        public int next; //后续遍历指向下一个元素的索引
    }

}
