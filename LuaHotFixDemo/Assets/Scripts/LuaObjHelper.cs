using System;
using System.Collections;
using SLua;

public enum ObjectLuaHotFixState
{
    Uninit,
    InitUnavialable,
    InitAvialable,
}

public class LuaObjHelper : IDisposable
{
    private object m_csObject;
    private LuaTable m_luaModule;
    private LuaTable m_luaObject;

    public void InitInCS(object csObject, LuaTable lauModule)
    {
        m_csObject = csObject;
    }

    public void SetLuaObj(LuaTable luaObject)
    {
        m_luaObject = luaObject;
    }

    public LuaTable GetLuaObj()
    {
        return m_luaObject;
    }

    public void Dispose()
    {
        m_luaModule.Dispose();
        m_luaObject.Dispose();
    }

    public bool IsDisposed()
    {
        return false;
    }
}