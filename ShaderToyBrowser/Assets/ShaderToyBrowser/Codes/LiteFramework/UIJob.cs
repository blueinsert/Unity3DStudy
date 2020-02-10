using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean.ShaderToyBrowser
{
    public class UIJob
    {
        public GameObject instance;
        protected  virtual string prefab { get; set; }

        public UIJob() { }

        public GameObject CreateInstance() {
            if (instance == null) {
                var _prefab = AssetLoader.Load<GameObject>(prefab);
                instance = GameObject.Instantiate(_prefab);
                InitOnCreateInstance();
            }
            return instance;
        }

        protected virtual void InitOnCreateInstance()
        {
        }

        public virtual void Init(CustomParams customParams) {
        }

        public virtual void Execute() {
        }

        public virtual void Tick() { }
    }
}
