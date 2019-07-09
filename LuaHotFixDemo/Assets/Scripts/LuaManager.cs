using SLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CustomLuaClass]
public class LuaManager
{
    private LuaManager() { }

    [DoNotToLua]
    public static LuaManager CreateLuaManager()
    {
        if (m_instance == null)
        {
            m_instance = new LuaManager();
        }
        return m_instance;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    [DoNotToLua]
    public bool Initlize(string luaRootPath, string luaInitModule = "LuaManagerInit")
    {
        m_luaRootPath = luaRootPath;
        m_luaInitModuleName = luaInitModule;
        return true;
    }

    /// <summary>
    /// 释放
    /// </summary>
    [DoNotToLua]
    public void Uninitlize()
    {
    }

    /// <summary>
    /// 启动lua 环境
    /// </summary>
    [DoNotToLua]
    public void StartLuaSvr(Action<bool> onComplete)
    {
        string initModulePath = GetModulePath(m_luaInitModuleName);
        TextAsset initModuleAsset = null;
        initModuleAsset = ResourceManager.Instance.LoadAsset<TextAsset>(initModulePath);

        if (initModuleAsset == null)
        {
            Debug.LogError("StartLuaSvr fail load initModuleAsset fail");
            onComplete(false);
        }

        // 构造lua state 环境
        m_luaSvr = new LuaSvr();

        // 设置lua模块加载器
        LuaSvr.mainState.loaderDelegate = LuaLoader;

        // 初始化lua环境
        m_luaSvr.init(null, () =>
        {
            // 加载初始化模块
            if (!string.IsNullOrEmpty(m_luaInitModuleName))
            {
                m_luaSvr.start(m_luaInitModuleName);
            }
        });
        onComplete(true);
    }

    /// <summary>
    /// 获取lua环境
    /// </summary>
    /// <returns></returns>
    [DoNotToLua]
    public LuaState GetLuaState()
    {
        return LuaSvr.mainState;
    }

    /// <summary>
    /// 获取用来hotfix的module
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public LuaTable GetHotFixLuaModuleByTypeFullName(Type type)
    {
        if (type == null)
            return null;
        LuaTable hotfixModule;
        if (!m_hotfixDict.TryGetValue(type.FullName, out hotfixModule))
        {
            return null;
        }
        return hotfixModule;
    }

    /// <summary>
    /// 注册用来hotfix的module
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <param name="module"></param>
    /// <returns></returns>
    public void RegHotFix(string typeFullName, LuaTable module)
    {
        if (module == null)
        {
            Debug.LogWarning(string.Format("LuaManager RegHotFix failed, can't find module {0}", typeFullName));
            return;
        }
        Debug.Log(string.Format("LuaManager RegHotFix succeed for:  {0}", typeFullName));
        m_hotfixDict[typeFullName] = module;
    }

    /// <summary>
    /// 尝试初始化hotfix
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="luaModuleName"></param>
    /// <param name="objType"></param>
    [DoNotToLua]
    public static bool TryInitHotfixForType(Type objType, string luaModuleName = null)
    {
        if (m_instance == null)
        {
            return false;
        }

        var type = objType;
        
        // 查找hotfix所需要的lua module
        var module = string.IsNullOrEmpty(luaModuleName)
            ? Instance.GetHotFixLuaModuleByTypeFullName(type) // 如果没有提供moduleName则应该是在LuaManagerInit中按类型名注册过的module
            : Instance.RequireModule(luaModuleName);    // 否则require一个module提供给obj作为hotfix

        if (module == null)
        {
            //Debug.LogWarning(string.Format("LuaManager.TryInitHotfixForObj Failed to find lua module by typename:{0}", type.FullName));
            return false;
        }

        var initFun = type.GetMethod("InitHotFix", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static);

        if (initFun == null)
        {
            Debug.LogWarning(string.Format("LuaManager.TryInitHotfixForObj Can't find InitHotFix method in :{0}", luaModuleName));
            return false;
        }

        return (bool)initFun.Invoke(null, new object[] { module });
    }


    /// <summary>
    /// 获取模块
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public LuaTable RequireModule(string moduleName)
    {
        return LuaSvr.mainState.doString(string.Format("return require(\"{0}\")", moduleName)) as LuaTable;
    }

    /// <summary>
    /// lua文件的加载器
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    private byte[] LuaLoader(string moduleName, ref string absoluteModulePath)
    {
        string path = GetModulePath(moduleName);
        TextAsset asset = null;
        asset = ResourceManager.Instance.LoadAsset<TextAsset>(path);
        if (asset == null)
        {
            Debug.LogError(string.Format("load lua module fail {0}", moduleName));
            return null;
        }
        return asset.bytes;
    }

    /// <summary>
    /// 测试lua文件是否存在
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    public bool IsLuaModuleExist(string moduleName)
    {
        string path = GetModulePath(moduleName);
        TextAsset asset = null;
        asset = Resources.Load<TextAsset>(path);
        if (asset == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 通过模块名称获取资源路径
    /// </summary>
    /// <param name="moduleName"></param>
    /// <returns></returns>
    private string GetModulePath(string moduleName)
    {
        return string.Format("{0}/{1}", m_luaRootPath, moduleName.Replace('.', '/'));
    }

    /// <summary>
    /// 单例访问器
    /// </summary>
    public static LuaManager Instance { get { return m_instance; } }
    private static LuaManager m_instance;

    /// <summary>
    /// 资源根路径
    /// </summary>
    private string m_luaRootPath;

    /// <summary>
    /// lua的初始化module,这个module的main函数会在启动的过程中调用执行必要的初始化
    /// </summary>
    private string m_luaInitModuleName;

    private LuaSvr m_luaSvr;

    /// <summary>
    /// hotfix的注册字典
    /// </summary>
    private Dictionary<string, LuaTable> m_hotfixDict = new Dictionary<string, LuaTable>();

    /// <summary>
    /// lua 辅助对象的 成员名称
    /// </summary>
    public const string LuaObjHelperMemberName = "m_luaObjHelper";
}
