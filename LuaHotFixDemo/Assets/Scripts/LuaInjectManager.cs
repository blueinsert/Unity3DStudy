using Mono.Cecil;
using Mono.Cecil.Cil;
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
            Debug.LogError(string.Format("InjectTool Inject Load assembly failed: {0}", AssemblyPath));
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
                var hotfixStateField = InjectHotfixStateField(type);
                //3.注入每个方法对应的LuaFunction声明
                var bridgeLuaFuncs = InjectLuaFunctionDefForType(type);
                //4.注入TryInitHotFix函数
                var tryInitHotFixMethod = InjectTryInitHotFixFunc(type, hotfixStateField);
                //5.注入InitHotFix函数
                InjectInitHotFixFunc(type, hotfixStateField, bridgeLuaFuncs);
                //6.为每个函数注入拦截代码片段
                Type realType = Type.GetType(type.FullName);
                bool isUnityComponent = typeof(UnityEngine.Component).IsAssignableFrom(realType);
                foreach (var methodDef in type.Methods)
                {
                    if (!LuaInjectUtil.IsMethodNeedHotFix(methodDef, isUnityComponent))
                    {
                        continue;
                    }
                    var bridgeLuaFunc = bridgeLuaFuncs.Find((field) =>
                    {
                        return field.Name == LuaInjectUtil.GetHotFixFunctionNameInCS(methodDef);
                    });
                    InjectRedirectCodeForMethod(module, type, methodDef, tryInitHotFixMethod, bridgeLuaFunc);
                }

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
        typeRef_object = module.ImportReference(typeof(object));
        typeRef_bool = module.ImportReference(typeof(bool));
        typeRef_string = module.ImportReference(typeof(string));

        methodRef_Type_GetTypeFromHandle = module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"));
        methodRef_LuaManager_TryInitHotFix = module.ImportReference(typeof(LuaManager).GetMethod("TryInitHotfixForType"));
        methodRef_LuaTable_get = module.ImportReference(typeof(LuaTable).GetMethod("get"));
        methodRef_LuaVar_op_Equality = module.ImportReference(typeof(LuaVar).GetMethod("op_Equality"));
        methodRef_LuaVar_op_Inequality = module.ImportReference(typeof(LuaVar).GetMethod("op_Inequality"));
        methodRef_LuaFunction_call = module.ImportReference(typeof(LuaFunction).GetMethod("call", new Type[] {typeof(object[])}));
    }

    /// <summary>
    /// 注入m_hotfixState变量
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static FieldDefinition InjectHotfixStateField(TypeDefinition typeDefinition)
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
        return hotfixStateField;
    }

    /// <summary>
    /// 注入每个热修方法对应的luaFunction定义
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static List<FieldDefinition> InjectLuaFunctionDefForType(TypeDefinition typeDefinition)
    {
        List<FieldDefinition> bridgeLuaFuncs = new List<FieldDefinition>();
        Type type = Type.GetType(typeDefinition.FullName);
        bool isUnityComponent = type.IsAssignableFrom(typeof(UnityEngine.Component));
        foreach (var methodDef in typeDefinition.Methods)
        {
            if (!LuaInjectUtil.IsMethodNeedHotFix(methodDef, isUnityComponent))
            {
                continue;
            }
            var luaFuncVarName = LuaInjectUtil.GetHotFixFunctionNameInCS(methodDef);
            FieldDefinition fieldDefinition = new FieldDefinition(luaFuncVarName, FieldAttributes.Private | FieldAttributes.Static, typeRef_LuaFunction);
            foreach (var field in typeDefinition.Fields)
            {
                if (field.Name == luaFuncVarName)
                {
                    typeDefinition.Fields.Remove(field);
                    break;
                }
            }
            typeDefinition.Fields.Add(fieldDefinition);
            bridgeLuaFuncs.Add(fieldDefinition);
        }
        return bridgeLuaFuncs;
    }

    /// <summary>
    /// 为类型注入TryInitHotFix函数
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static MethodDefinition InjectTryInitHotFixFunc(TypeDefinition typeDefinition, FieldReference hotfixStateField)
    {
        string funcName = "TryInitHotFix";
        foreach (var method in typeDefinition.Methods)
        {
            if (method.Name == funcName)
            {
                typeDefinition.Methods.Remove(method);
                break;
            }
        }
        MethodDefinition tryInitHotFixMethod = new MethodDefinition(funcName, MethodAttributes.Static | MethodAttributes.Private, typeRef_bool);
        tryInitHotFixMethod.Parameters.Add(new ParameterDefinition("luaModuleName", ParameterAttributes.None, typeRef_string));
        tryInitHotFixMethod.Body = new Mono.Cecil.Cil.MethodBody(tryInitHotFixMethod);
        tryInitHotFixMethod.Body.MaxStackSize = 2;
        tryInitHotFixMethod.Body.Variables.Add(new VariableDefinition(typeRef_bool));
        tryInitHotFixMethod.Body.Variables.Add(new VariableDefinition(typeRef_bool));
        // 开始注入IL代码
        var ilProcessor = tryInitHotFixMethod.Body.GetILProcessor();
        tryInitHotFixMethod.Body.Instructions.Add(ilProcessor.Create(OpCodes.Ret));
        var insertPoint = tryInitHotFixMethod.Body.Instructions[0];

        // 设置一些标签用于语句跳转
        var label1 = ilProcessor.Create(OpCodes.Nop);
        var label2 = ilProcessor.Create(OpCodes.Ldloc_0);
        var label3 = ilProcessor.Create(OpCodes.Ldc_I4_1);
        var label4 = ilProcessor.Create(OpCodes.Stsfld, hotfixStateField);
        var label5 = ilProcessor.Create(OpCodes.Ldloc_1);
        //if (m_hotFixState == 0)
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldsfld, hotfixStateField));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Brfalse, label1));
        //flag = (HotFixTestScript.m_hotfixState == ObjectLuaHotFixState.InitAvialable);
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldsfld, hotfixStateField));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_2));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ceq));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label2));
        //flag = LuaManager.TryInitHotfixForObj(typeof(HotFixTestScript), luaModuleName);
        ilProcessor.InsertBefore(insertPoint, label1);//OpCodes.Nop
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldtoken, typeDefinition));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, methodRef_Type_GetTypeFromHandle));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, methodRef_LuaManager_TryInitHotFix));
        //HotFixTestScript.m_hotfixState = ((!flag) ? ObjectLuaHotFixState.InitUnavialable : ObjectLuaHotFixState.InitAvialable);
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldloc_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Brfalse, label3));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_2));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label4));
        ilProcessor.InsertBefore(insertPoint, label3);//OpCodes.Ldc_I4_1
        ilProcessor.InsertBefore(insertPoint, label4);//OpCodes.Stsfld, hotfixState
        //return flag
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, label2);//OpCodes.Ldloc_0
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label5));
        ilProcessor.InsertBefore(insertPoint, label5);//OpCodes.Ldloc_1

        typeDefinition.Methods.Add(tryInitHotFixMethod);

        return tryInitHotFixMethod;
    }

    /// <summary>
    /// 为类型注入InitHotFix函数
    /// </summary>
    /// <param name="typeDefinition"></param>
    private static void InjectInitHotFixFunc(TypeDefinition typeDefinition, FieldReference hotfixState, List<FieldDefinition> bridgeLuaFuncs)
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
        MethodDefinition initHotFixMethod = new MethodDefinition(funcName, MethodAttributes.Static | MethodAttributes.Private, typeRef_bool);
        initHotFixMethod.Body = new Mono.Cecil.Cil.MethodBody(initHotFixMethod);
        initHotFixMethod.Parameters.Add(new ParameterDefinition("luaModule", ParameterAttributes.None, typeRef_LuaTable));
        initHotFixMethod.Body = new Mono.Cecil.Cil.MethodBody(initHotFixMethod);
        initHotFixMethod.Body.MaxStackSize = 4;
        initHotFixMethod.Body.Variables.Add(new VariableDefinition(typeRef_bool));
        initHotFixMethod.Body.Variables.Add(new VariableDefinition(typeRef_bool));
        // 开始注入IL代码
        var ilProcessor = initHotFixMethod.Body.GetILProcessor();
        initHotFixMethod.Body.Instructions.Add(ilProcessor.Create(OpCodes.Ret));
        var insertPoint = initHotFixMethod.Body.Instructions[0];

        // 设置一些标签用于语句跳转
        var label1 = ilProcessor.Create(OpCodes.Nop);
        var label2 = ilProcessor.Create(OpCodes.Ldloc_0);
        var label3 = ilProcessor.Create(OpCodes.Ldloc_1);
        // if (luaModule == null)
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldnull));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, methodRef_LuaVar_op_Equality));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Brfalse, label1));
        //result = false; 将0压入局部变量索引零处
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label2));

        //luaModule != null
        ilProcessor.InsertBefore(insertPoint, label1);//OpCodes.Nop   
        //初始化方法对应的luaFunction变量
        Type type = Type.GetType(typeDefinition.FullName);
        bool isUnityComponent = type.IsAssignableFrom(typeof(UnityEngine.Component));
        foreach (var methodDef in typeDefinition.Methods)
        {
            //for example: 
            //m_Add_ThisInt32Int32_fix = (luaModule.get("Add_ThisInt32Int32", rawget: true) as LuaFunction);
            if (!LuaInjectUtil.IsMethodNeedHotFix(methodDef, isUnityComponent))
            {
                continue;
            }
            var bridgeLuaFunc = bridgeLuaFuncs.Find((field) =>
            {
                return field.Name == LuaInjectUtil.GetHotFixFunctionNameInCS(methodDef);
            });
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldstr, LuaInjectUtil.GetHotFixFunctionNameInLua(methodDef)));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_0));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_1));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Callvirt, methodRef_LuaTable_get));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Isinst, typeRef_LuaFunction));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stsfld, bridgeLuaFunc));
        }
        ////result = true; 将1压入局部变量索引零处
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc_0));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));

        ilProcessor.InsertBefore(insertPoint, label2);//OpCodes.Ldloc_0
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc_1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Br, label3));
        ilProcessor.InsertBefore(insertPoint, label3);//OpCodes.Ldloc_1

        typeDefinition.Methods.Add(initHotFixMethod);
    }

    /// <summary>
    /// 为每个热修方法注入劫持代码
    /// </summary>
    private static void InjectRedirectCodeForMethod(ModuleDefinition module, TypeDefinition typeDefinition, MethodDefinition hotfixedMethodDefinition, MethodDefinition tryInitHotFixMethod, FieldDefinition bridgeLuaFunc)
    {
        /*
         * example:
         *  if (TryInitHotFix("") && m_Add_ThisInt32Int32_fix != null)
            {
                var result = m_Add_ThisInt32Int32_fix.call(new object[]
                {
                    this,a,b
                });
               return (int)(double)result;
            }
         */
        bool hasReturnValue = hotfixedMethodDefinition.ReturnType.Name != module.TypeSystem.Void.Name;

        VariableDefinition returunLocalVar = null;
        if (hasReturnValue)
        {
            //用于存放返回值
            returunLocalVar = new VariableDefinition(hotfixedMethodDefinition.ReturnType);
            hotfixedMethodDefinition.Body.Variables.Add(returunLocalVar);
        }
        Instruction lastInst = hotfixedMethodDefinition.Body.Instructions[hotfixedMethodDefinition.Body.Instructions.Count - 1];
        if (lastInst.OpCode != OpCodes.Ret)
        {
            Debug.LogError(string.Format("The last opcode is't 'return' in {0}.{1}",typeDefinition.Name, hotfixedMethodDefinition.Name));
            return;
        }
        // 开始注入IL代码
        var ilProcessor = hotfixedMethodDefinition.Body.GetILProcessor();
        Instruction insertPoint;
        if (tryInitHotFixMethod.IsConstructor)
        {
            //如果是构造方法在最后插入
            insertPoint = lastInst;
        }
        else
        {
            insertPoint = hotfixedMethodDefinition.Body.Instructions[0];
        }
        
        // 设置一些标签用于语句跳转
        var label1 = ilProcessor.Create(OpCodes.Nop);

        //if (TryInitHotFix(""))
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldstr, ""));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, tryInitHotFixMethod));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Brfalse, label1));
        //if (m_Add_ThisInt32Int32_fix != null)
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldsfld, bridgeLuaFunc));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldnull));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, methodRef_LuaVar_op_Inequality));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Brfalse, label1));
        //两个条件都满足
        //for example
        //var result = m_Add_ThisInt32Int32_fix.call(new object[]
        //{
        //    this,a,b
        //});
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Nop));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldsfld, bridgeLuaFunc));
        //创建new object[]{}
        int paramCount = hotfixedMethodDefinition.Parameters.Count;
        ilProcessor.InsertBefore(insertPoint,
            ilProcessor.Create(OpCodes.Ldc_I4, hotfixedMethodDefinition.IsStatic ? paramCount : paramCount + 1));
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Newarr, typeRef_object));
        
        
        int arrayIndex = 0;
        //往数组里插入this
        if (!hotfixedMethodDefinition.IsStatic)
        {
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Dup));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4, arrayIndex++));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg_0));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stelem_Ref));
        }
        //插入剩余参数
        for(int i = 0; i < hotfixedMethodDefinition.Parameters.Count; i++)
        {
            var paramType = hotfixedMethodDefinition.Parameters[i];
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Dup));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldc_I4, arrayIndex++));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldarg, i+1));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Box, paramType.ParameterType));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stelem_Ref));
        }
        ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Callvirt, methodRef_LuaFunction_call));
        if (!hasReturnValue)
        {
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Pop));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ret));
        }
        else
        {
            if (hotfixedMethodDefinition.ReturnType.IsPrimitive)
            {
                string methodName = "To" + hotfixedMethodDefinition.ReturnType.Name;
                var convertMethod = module.ImportReference(typeof(Convert).GetMethod(methodName, new Type[] { typeof(object) }));
                ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, convertMethod));
            }
            else if (hotfixedMethodDefinition.ReturnType.Resolve().IsEnum)
            {
                string methodName = "ToInt32";
                var convertMethod = module.ImportReference(typeof(Convert).GetMethod(methodName, new Type[] { typeof(object) }));
                ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Call, convertMethod));
            }
            else {
                if (hotfixedMethodDefinition.ReturnType.IsValueType)
                {
                    ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Unbox_Any));
                }
                else
                {
                    ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Castclass, hotfixedMethodDefinition.ReturnType));
                }
            }
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Stloc, returunLocalVar));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ldloc, returunLocalVar));
            ilProcessor.InsertBefore(insertPoint, ilProcessor.Create(OpCodes.Ret));
        }
        ilProcessor.InsertBefore(insertPoint, label1);//OpCodes.Nop
    }

    private static TypeReference typeRef_LuaManager;
    private static TypeReference typeRef_ObjectLuaHotFixState;
    private static TypeReference typeRef_LuaFunction;
    private static TypeReference typeRef_LuaTable;
    private static TypeReference typeRef_object;
    private static TypeReference typeRef_bool;
    private static TypeReference typeRef_string;

    private static MethodReference methodRef_LuaManager_TryInitHotFix;
    private static MethodReference methodRef_Type_GetTypeFromHandle;
    private static MethodReference methodRef_LuaVar_op_Equality;
    private static MethodReference methodRef_LuaVar_op_Inequality;
    private static MethodReference methodRef_LuaTable_get;
    private static MethodReference methodRef_LuaFunction_call;
    
    private static FieldReference fieldRef_m_hotfixState;
}
