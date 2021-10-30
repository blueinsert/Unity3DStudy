using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KDTree {
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
        return  (int)(d1.x - d2.x);
    }

    public int DataSorter2(Data d1, Data d2)
    {
        return (int)(d1.y - d2.y);
    }

    void ChooseSplit(List<Data> datas, out int split, out Data splitData, out List<Data> leftDatas, out List<Data> rightDatas) {
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
        TNode node = new TNode() { m_data = splitData, m_split = split};
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
}
