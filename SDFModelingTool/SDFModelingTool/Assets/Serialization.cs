using System;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> target;

        public List<T> ToList() { return target; }

        public Serialization(List<T> target)
        {
            this.target = target;
        }

    }
}