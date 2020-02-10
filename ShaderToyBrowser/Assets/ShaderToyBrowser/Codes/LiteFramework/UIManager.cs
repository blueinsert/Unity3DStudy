using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace bluebean.ShaderToyBrowser
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public Canvas canvas;
        public GameObject UnusedRoot;

        private Dictionary<Type, UIJob> uiJobDic = new Dictionary<Type, UIJob>();
        private Stack<UIJob> uiJobStack = new Stack<UIJob>();

        private void Awake()
        {
            Instance = this;
        }

        public void PushUIJob<T>(CustomParams customParams) where T : UIJob, new()
        {
            UIJob jobInstance = null;
            if (uiJobDic.ContainsKey(typeof(T)))
            {
                jobInstance = uiJobDic[typeof(T)];
            }
            else {
                jobInstance = new T();
                uiJobDic.Add(typeof(T), jobInstance);
            }
            jobInstance.CreateInstance();
            if (uiJobStack.Count != 0) {
                var top = uiJobStack.Peek();
                top.instance.transform.SetParent(UnusedRoot.transform, false);
            }
            uiJobStack.Push(jobInstance);
            jobInstance.instance.transform.SetParent(canvas.transform, false);
            jobInstance.Init(customParams);
            jobInstance.Execute();
        }

        public void PopUIJob(CustomParams customParams) {
            if (uiJobStack.Count == 0)
                return;
            var uiJobInstance = uiJobStack.Pop();
            uiJobInstance.instance.transform.SetParent(UnusedRoot.transform, false);
            if (uiJobStack.Count != 0) {
                var top = uiJobStack.Peek();
                top.instance.transform.SetParent(canvas.transform, false);
                top.Init(customParams);
                top.Execute();
            }
        }

        public void Update()
        {
            if (uiJobStack.Count != 0) {
                var top = uiJobStack.Peek();
                top.Tick();
            }
        }
    }
}
