using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLua;

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
            m_Add_ThisInt32Int32_fix = (luaModule.get("Add_ThisInt32Int32", rawget: true) as LuaFunction);
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
            result = LuaManager.TryInitHotfixForObj(typeof(HotFixTestScript), luaModuleName);
            m_hotfixState = ((!result) ? ObjectLuaHotFixState.InitUnavialable : ObjectLuaHotFixState.InitAvialable);
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
            return (int)(double)result;
        }
        #endregion
        return a * b;
    }

}
[HotFix]
public class HotFixTestScript2
{
    public int Add(int a, int b)
    {
        return a * b;
    }
}