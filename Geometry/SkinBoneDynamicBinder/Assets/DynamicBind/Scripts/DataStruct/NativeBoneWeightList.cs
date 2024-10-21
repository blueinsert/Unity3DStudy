using System;
using UnityEngine;

namespace bluebean
{
    [Serializable]
    public class NativeBoneWeightList : NativeList<BoneWeight1>
    {
        public NativeBoneWeightList() { }
        public NativeBoneWeightList(int capacity = 8, int alignment = 16) : base(capacity, alignment)
        {
            for (int i = 0; i < capacity; ++i)
                this[i] = new BoneWeight1();
        }

    }
}

