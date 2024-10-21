using System;

namespace bluebean
{
    [Serializable]
    public class NativeByteList : NativeList<byte>
    {

        public NativeByteList() { }
        public NativeByteList(int capacity = 8, int alignment = 16) : base(capacity, alignment)
        {
            for (int i = 0; i < capacity; ++i)
                this[i] = 0;
        }

    }
}

