using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KDTree
{
    public struct Data
    {
        public float x;
        public float y;
    }

    public class TNode
    {
        public Data m_data;
        public int m_split;//拆分使用的坐标轴
        public TNode m_left;
        public TNode m_right;
    }

    public int DataSorter1(Data d1, Data d2)
    {
        return (int)(d1.x - d2.x);
    }

    public int DataSorter2(Data d1, Data d2)
    {
        return (int)(d1.y - d2.y);
    }

    void ChooseSplit(List<Data> datas, out int split, out Data splitData, out List<Data> leftDatas, out List<Data> rightDatas)
    {
        float temp1 = 0, temp2 = 0;
        int size = datas.Count;
        leftDatas = new List<Data>();
        rightDatas = new List<Data>();
        if (size == 1)
        {
            split = -1;
            splitData = datas[0];
            return;
        }
        for (int i = 0; i < size; i++)
        {
            temp1 += 1.0f / size * datas[i].x * datas[i].x;
            temp2 += 1.0f / size * datas[i].x;
        }
        float v1 = temp1 - temp2 * temp2;//x方差
        temp1 = 0; temp2 = 0;
        for (int i = 0; i < size; i++)
        {
            temp1 += 1.0f / size * datas[i].y * datas[i].y;
            temp2 += 1.0f / size * datas[i].y;
        }
        float v2 = temp1 - temp2 * temp2;//y方差
        split = v1 > v2 ? 0 : 1;//将方差较大的轴进行拆分
        if (split == 0)
        {
            datas.Sort(DataSorter1);
        }
        else
        {
            datas.Sort(DataSorter2);
        }
        int index = size / 2;
        splitData.x = datas[index].x;
        splitData.y = datas[index].y;
        for (int i = 0; i < index; i++)
        {
            leftDatas.Add(datas[i]);
        }
        for (int i = index + 1; i < size; i++)
        {
            rightDatas.Add(datas[i]);
        }
    }

    public TNode BuildTree(List<Data> datas)
    {
        if (datas == null || datas.Count == 0)
            return null;
        int split;
        Data splitData;
        List<Data> leftDatas;
        List<Data> rightDatas;
        ChooseSplit(datas, out split, out splitData, out leftDatas, out rightDatas);
        TNode node = new TNode() { m_data = splitData, m_split = split };
        node.m_left = null;
        node.m_right = null;
        if (leftDatas.Count != 0)
        {
            node.m_left = BuildTree(leftDatas);
        }
        if (rightDatas.Count != 0)
        {
            node.m_right = BuildTree(rightDatas);
        }
        return node;
    }

    private float Distance(Data d1, Data d2)
    {
        return Mathf.Sqrt((d1.x - d2.x) * (d1.x - d2.x) + (d2.y - d1.y) * (d2.y - d1.y));
    }

    public void SearchNearest(TNode tree, Data pos, float radius, out List<Data> nearest)
    {
        nearest = new List<Data>();
        Stack<TNode> searchPath = new Stack<TNode>();
        TNode searchNode = tree;
        while (searchNode != null)
        {
            searchPath.Push(searchNode);
            if (searchNode.m_split == 0)
            {
                if (pos.x <= searchNode.m_data.x)
                {
                    searchNode = searchNode.m_left;
                }
                else
                {
                    searchNode = searchNode.m_right;
                }
            }
            else if (searchNode.m_split == 1)
            {
                if (pos.y <= searchNode.m_data.y)
                {
                    searchNode = searchNode.m_left;
                }
                else
                {
                    searchNode = searchNode.m_right;
                }
            }
            else
            {
                searchNode = null;
            }
        }
        List<TNode> originSearchPath = new List<TNode>(searchPath.ToArray());
        TNode backNode = null;
        while (searchPath.Count != 0)
        {
            backNode = searchPath.Pop();
            if (Distance(backNode.m_data, pos) <= radius)
            {
                nearest.Add(backNode.m_data);
            }
            //叶子节点
            if (backNode.m_left == null && backNode.m_right == null)
            {

            }
            else
            {
                if (originSearchPath.Contains(backNode))
                {
                    if (backNode.m_split == 0)
                    {
                        //以目标点为中心的圆与分离轴相近，需要将搜索路径的另一半加入
                        if (Mathf.Abs(pos.x - backNode.m_data.x) < radius)
                        {

                            if (pos.x <= backNode.m_data.x)
                            {
                                searchNode = backNode.m_right;
                            }
                            else
                            {
                                searchNode = backNode.m_left;
                            }
                            if (searchNode != null)
                                searchPath.Push(searchNode);
                        }
                    }
                    else if (backNode.m_split == 1)
                    {
                        //以目标点为中心的圆与分离轴相近，需要将搜索路径的另一半加入
                        if (Mathf.Abs(pos.y - backNode.m_data.y) < radius)
                        {
                            if (pos.y <= backNode.m_data.y)
                            {
                                searchNode = backNode.m_right;
                            }
                            else
                            {
                                searchNode = backNode.m_left;
                            }
                            if (searchNode != null)
                                searchPath.Push(searchNode);
                        }
                    }
                }
                else
                {
                    if (backNode.m_split == 0)
                    {
                        if (pos.x <= backNode.m_data.x)
                        {
                            searchNode = backNode.m_left;
                        }
                        else
                        {
                            searchNode = backNode.m_right;
                        }
                        if (searchNode != null)
                            searchPath.Push(searchNode);
                        //以目标点为中心的圆与分离轴相近，需要将搜索路径的另一半加入
                        if (Mathf.Abs(pos.x - backNode.m_data.x) < radius)
                        {
                            if (pos.x <= backNode.m_data.x)
                            {
                                searchNode = backNode.m_right;
                            }
                            else
                            {
                                searchNode = backNode.m_left;
                            }
                            if (searchNode != null)
                                searchPath.Push(searchNode);
                        }
                    }
                    else if (backNode.m_split == 1)
                    {
                        if (pos.y <= backNode.m_data.y)
                        {
                            searchNode = backNode.m_left;
                        }
                        else
                        {
                            searchNode = backNode.m_right;
                        }
                        if(searchNode != null)
                            searchPath.Push(searchNode);
                        //以目标点为中心的圆与分离轴相近，需要将搜索路径的另一半加入
                        if (Mathf.Abs(pos.y - backNode.m_data.y) < radius)
                        {
                            if (pos.y <= backNode.m_data.y)
                            {
                                searchNode = backNode.m_right;
                            }
                            else
                            {
                                searchNode = backNode.m_left;
                            }
                            if (searchNode != null)
                                searchPath.Push(searchNode);
                        }
                    }      
                }
            }
        }

    }
}
