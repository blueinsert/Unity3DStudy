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
    LuaObjHelper m_luaObjHelper = new LuaObjHelper();

    private LuaFunction m_add_fix;

    private ObjectLuaHotFixState m_hotfixState = ObjectLuaHotFixState.Uninit;

    private bool InitHotFix(LuaTable luaTable)
    {
        this.m_luaObjHelper.InitInCS(this, luaTable);
        LuaFunction luaFunction = luaTable.get("HotFixObject", rawget: true) as LuaFunction;
        bool result;
        if (luaFunction == null)
        {
            Debug.LogError("Can't find HotFixObject Func");
            result = false;
        }
        else
        {
            luaFunction.call(new object[]
            {
                this,
                this.m_luaObjHelper
            });
            LuaTable luaObj = this.m_luaObjHelper.GetLuaObj();
            if (luaObj == null)
            {
                result = false;
            }
            else
            {
                this.m_add_fix = (luaObj.get("Add", rawget: true) as LuaFunction);
                result = true;
            }
        }
        return result;
    }

    private bool TryInitHotFix(string luaModuleName)
    {
        bool result;
        if (this.m_hotfixState != ObjectLuaHotFixState.Uninit)
        {
            result = (this.m_hotfixState == ObjectLuaHotFixState.InitAvialable);
        }
        else
        {
            bool flag = LuaManager.TryInitHotfixForObj(this, luaModuleName, typeof(HotFixTestScript));
            this.m_hotfixState = ((!flag) ? ObjectLuaHotFixState.InitUnavialable : ObjectLuaHotFixState.InitAvialable);
            result = flag;
        }
        return result;
    }

    private bool IsLuaObjHelperDisposed()
    {
        return this.m_luaObjHelper != null && this.m_luaObjHelper.IsDisposed();
    }

    #endregion

    public int Add(int a, int b)
    {
        #region 这些代码由dll注入,自动化产生，这里写了为了做示范
        if (this.TryInitHotFix("") && this.m_add_fix != null && !this.IsLuaObjHelperDisposed())
        {
            var result = this.m_add_fix.call(new object[]
            {
                this,a,b
            });
            //Debug.Log(result.GetType().Name);
            return (int)(double)result;
        }
        #endregion

        return a * b;
    }
}