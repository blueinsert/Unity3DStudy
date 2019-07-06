using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil;
using System.Text;

public static class LuaInjectUtil
{
    /// <summary>
    /// 类型是否需要热修复
    /// </summary>
    /// <param name="typeReference"></param>
    /// <returns></returns>
    public static bool IsTypeNeedHotFix(TypeDefinition typeDefinition)
    {
        //如果没有添加HotFixAttribute则不需要热修
        bool hasHotFixAttribute = false;
        foreach(var attribute in typeDefinition.CustomAttributes)
        {
            if(attribute.AttributeType.FullName == typeof(HotFixAttribute).FullName)
            {
                hasHotFixAttribute = true;
                break;
            }
        }
        if (hasHotFixAttribute)
            return true;
        //接口不需要热修
        if (typeDefinition.IsInterface)
        {
            return false;
        }
        return false;
    }

    /// <summary>
    /// 方法是否需要热修复
    /// </summary>
    /// <param name="methodDefinition"></param>
    /// <param name="isUnityComponent"></param>
    /// <returns></returns>
    public static bool IsMethodNeedHotFix(MethodDefinition methodDefinition, bool isUnityComponent)
    {
        var isCtor = methodDefinition.IsConstructor;
        //unity组件类的构成方法不需要插入代码片段
        if(isUnityComponent && isCtor)
        {
            return false;
        }
        //没有方法体
        if(!methodDefinition.HasBody || methodDefinition.Body == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获得被热修的方法在lua代码中对应的名字
    /// </summary>
    /// <param name="methodDefinition"></param>
    /// <returns></returns>
    public static string GetHotFixFunctionNameInLua(MethodDefinition methodDefinition)
    {
        StringBuilder sb = new StringBuilder();
        string methodName = methodDefinition.Name;
        if (methodDefinition.IsConstructor)
        {
            methodName = methodName.Substring(1);
        }
        sb.Append(methodName);
        sb.Append("_");
        if (!methodDefinition.IsStatic)
        {
            sb.Append("this");
        }
        foreach(var param in methodDefinition.Parameters)
        {
            sb.Append(param.ParameterType.Name);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 获得被热修的方法在CS代码中对应LuaFunction的名字
    /// </summary>
    /// <param name="methodDefinition"></param>
    /// <returns></returns>
    public static string GetHotFixFunctionNameInCS(MethodDefinition methodDefinition)
    {
        return string.Format("m_{0}_hotfix", GetHotFixFunctionNameInLua(methodDefinition));
    }
}
