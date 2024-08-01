using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace bluebean
{
    public interface IVertexPropertyHolder
    {
        public Vector4 GetVertexProperty(int index);
        public void SetVertexProperty(int index, Vector4 value);

        public void Clear();
    }
}
