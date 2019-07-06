using Mono.Cecil;
using SLua;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LuaInjectManager
{
    private const string AssemblyPath = "./Library/ScriptAssemblies/Assembly-CSharp.dll";

    [MenuItem("HotFix/InjectToIL")]
    public static void InjectToIL()
    {
        Debug.Log("InjectTool Inject  Start");

        if (Application.isPlaying || EditorApplication.isCompiling)
        {
            Debug.Log("You need stop play mode or wait compiling finished");
            return;
        }

        // 按路径读取程序集
        var readerParameters = new ReaderParameters { ReadSymbols = true };
        var assembly = AssemblyDefinition.ReadAssembly(AssemblyPath, readerParameters);
        if (assembly == null)
        {
            Debug.LogError(string.Format("InjectTool Inject Load assembly failed: {0}",AssemblyPath));
            return;
        }

        try
        {
            var module = assembly.MainModule;
            //1.收集类型和方法引用
            CollectTypeMethodReference(module);
            foreach (var type in module.Types)
            {
                if (!LuaInjectUtil.IsTypeNeedHotFix(type))
                {
                    continue;
                }
                //2.添加ObjectLuaHotFixState
                InjectHotfixStateField(type);
                //3.注入每个方法对应的LuaFunction声明
                InjectLuaFunctionDefForType(type);
                //4.注入TryInitHotFix函数
                InjectTryInitHotFixFunc(type);
                //5.注入InitHotFix函数
                InjectInitHotFixFunc(type);
                //6.为每个函数注入拦截代码片段
            }
            assembly.Write(AssemblyPath, new WriterParameters { WriteSymbols = true });
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("InjectTool Inject failed: {0}", ex));
            throw;
        }
        finally
        {
            if (assembly.MainModule.SymbolReader != null)
            {
                Debug.Log("InjectTool Inject SymbolReader.Dispose Succeed");
                assembly.MainModule.SymbolReader.Dispose();
            }
        }
        Debug.Log("InjectTool Inject End");
    }

    /// <summary>
    /// 收集类型和方法引用
    /// </summary>
    private static void CollectTypeMethodReference(ModuleDefinition module)
    {
        typeRef_LuaFunction = module.ImportReference(typeof(LuaFunction));
        typeRef_LuaTable = module.ImportReference(typeof(LuaTable));
        typeRef_ObjectLuaHotFixState = module.ImportReference(typeof(ObjectLuaHotFixState));
        typeRef_LuaManager = module.ImportReference(typeof(LuaManager));
        typeRef_bool = module.ImportReference(typeof(bool));
    }

    /// <summary>
    /// 注入m_hotfixState变量
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static void InjectHotfixStateField(TypeDefinition typeDefinition)
    {
        string varName = "m_hotfixState";
        foreach (var field in typeDefinition.Fields)
        {
            if (field.Name == varName)
            {
                typeDefinition.Fields.Remove(field);
                break;
            }
        }
        FieldDefinition hotfixStateField = new FieldDefinition(varName, FieldAttributes.Private | FieldAttributes.Static, typeRef_ObjectLuaHotFixState);
        typeDefinition.Fields.Add(hotfixStateField);
    }

    /// <summary>
    /// 注入每个热修方法对应的luaFunction定义
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static void InjectLuaFunctionDefForType(TypeDefinition typeDefinition)
    {
        Type type = Type.GetType(typeDefinition.FullName);
        bool isUnityComponent = type.IsAssignableFrom(typeof(UnityEngine.Component));
        foreach(var methodDef in typeDefinition.Methods)
        {
            if (!LuaInjectUtil.IsMethodNeedHotFix(methodDef, isUnityComponent))
            {
                continue;
            }
            var luaFuncVarName = LuaInjectUtil.GetHotFixFunctionNameInCS(methodDef);
            FieldDefinition fieldDefinition = new FieldDefinition(luaFuncVarName, FieldAttributes.Private | FieldAttributes.Static, typeRef_LuaFunction);
            foreach(var field in typeDefinition.Fields)
            {
                if(field.Name == luaFuncVarName)
                {
                    typeDefinition.Fields.Remove(field);
                    break;
                }
            }
            typeDefinition.Fields.Add(fieldDefinition);
        }
    }

    /// <summary>
    /// 为类型注入TryInitHotFix函数
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static void InjectTryInitHotFixFunc(TypeDefinition typeDefinition)
    {
        string funcName = "TryInitHotFix";
        foreach(var method in typeDefinition.Methods)
        {
            if(method.Name == funcName)
            {
                typeDefinition.Methods.Remove(method);
                break;
            }
        }
        MethodDefinition tryInitHotFixMethod = new MethodDefinition(funcName, MethodAttributes.Static | MethodAttributes.Private, typeRef_bool);
        tryInitHotFixMethod.Body = new Mono.Cecil.Cil.MethodBody(tryInitHotFixMethod);
        //todo
        typeDefinition.Methods.Add(tryInitHotFixMethod);
    }

    /// <summary>
    /// 为类型注入InitHotFix函数
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static void InjectInitHotFixFunc(TypeDefinition typeDefinition)
    {
        string funcName = "InitHotFix";
        foreach (var method in typeDefinition.Methods)
        {
            if (method.Name == funcName)
            {
                typeDefinition.Methods.Remove(method);
                break;
            }
        }
        MethodDefinition tryInitHotFixMethod = new MethodDefinition(funcName, MethodAttributes.Static | MethodAttributes.Private, typeRef_bool);
        tryInitHotFixMethod.Body = new Mono.Cecil.Cil.MethodBody(tryInitHotFixMethod);
        //todo
        typeDefinition.Methods.Add(tryInitHotFixMethod);
    }

    private static TypeReference typeRef_LuaManager;
    private static TypeReference typeRef_ObjectLuaHotFixState;
    private static TypeReference typeRef_LuaFunction;
    private static TypeReference typeRef_LuaTable;
    private static TypeReference typeRef_bool;

    private static MethodReference methodRef_LuaManager_TryInitHotFix;
}
