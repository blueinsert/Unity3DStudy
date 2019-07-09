using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLua;
using System;

/// <summary>
/// 热修测试对象
/// </summary>
public class HotFixTestScript
{
    #region 这些代码由dll注入,自动化产生，这里写了为了做示范

    private static LuaFunction m_Add_ThisInt32Int32_fix;

    private static ObjectLuaHotFixState m_hotfixState = ObjectLuaHotFixState.Uninit;

    private static bool InitHotFix(LuaTable luaModule)
    {
        bool result = false;
        if (luaModule == null)
        {
            result = false;
        }
        else
        {
            m_Add_ThisInt32Int32_fix = (luaModule.get("Add_thisInt32Int32", rawget: true) as LuaFunction);
            result = true;
        }
        return result;
    }

    private static bool TryInitHotFix(string luaModuleName)
    {
        bool result;
        if (m_hotfixState != ObjectLuaHotFixState.Uninit)
        {
            result = (m_hotfixState == ObjectLuaHotFixState.InitAvialable);
        }
        else
        {
            result = LuaManager.TryInitHotfixForType(typeof(HotFixTestScript), luaModuleName);
            m_hotfixState = (result ? ObjectLuaHotFixState.InitAvialable : ObjectLuaHotFixState.InitUnavialable);
        }
        return result;
    }

    #endregion

    public int Add(int a, int b)
    {
        #region 这些代码由dll注入,自动化产生，这里写了为了做示范
        if (TryInitHotFix("") && m_Add_ThisInt32Int32_fix != null)
        {
            var result = m_Add_ThisInt32Int32_fix.call(new object[]
            {
                this,a,b
            });
            return Convert.ToInt32(result);
        }
        #endregion
        return a * b;
    }

    public void Test1(int a, int b)
    {
        #region 这些代码由dll注入,自动化产生，这里写了为了做示范
        if (TryInitHotFix("") && m_Add_ThisInt32Int32_fix != null)
        {
            var result = m_Add_ThisInt32Int32_fix.call(new object[]
            {
                this,a,b
            });
        }
        #endregion
    }
}
[HotFix]
public class HotFixTestScript2
{
    public int Add(int a, int b)
    {
        return a * b;
    }

    public static int s_Add(int a, int b)
    {
        return a * b;
    }
}