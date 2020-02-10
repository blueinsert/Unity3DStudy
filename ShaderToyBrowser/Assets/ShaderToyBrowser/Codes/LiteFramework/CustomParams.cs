using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.ShaderToyBrowser
{
    public class CustomParams
    {
        private Dictionary<object, object> dic = new Dictionary<object, object>();

        public void Clear() {
            dic.Clear();
        }

        public void AddParam(object key, object value) {
            dic.Add(key, value);
        }

        public object GetParam(object key) {
            if (dic.ContainsKey(key)) {
                return dic[key];
            }
            return null;
        }
    }
}
