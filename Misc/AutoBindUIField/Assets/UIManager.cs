using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public void Awake()
    {
        Instance = this;
    }
    
    public void Create(UIIntent uiIntent)
    {
        var prefab = Resources.Load<GameObject>(uiIntent.prefabName);
        var go = GameObject.Instantiate(prefab,this.transform);
        var typeUICtl = Type.GetType(uiIntent.uiControllerName);
        var uictl = go.AddComponent(typeUICtl);
        BindUIFields(uictl, typeUICtl, go.transform);
    }

    private void BindUIFields(System.Object obj, Type type, Transform root)
    {
         foreach(var fieldInfo in type.GetFields())
         {
            string path = "";
            bool isAutoBind = false;
            var custonAttributes = fieldInfo.GetCustomAttributes(false);
            foreach(var attribute in custonAttributes) {
                if(attribute.GetType().FullName == typeof(AutoBindAttribute).FullName)
                {
                    isAutoBind = true;
                    path = (attribute as AutoBindAttribute).path;
                    break;
                }
            }
            if (!isAutoBind)
                continue;
            fieldInfo.SetValue(obj, root.Find(path).GetComponent(fieldInfo.FieldType));
         }
    }
}
