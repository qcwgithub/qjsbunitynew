using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class JSDataExchangeMgr
{
    JSVCall vc;

    public enum eGetType
    {
        GetARGV,
        GetJSFUNRET
    }

    #region Get Operation

    public double getNumberic(eGetType e)
    {
        switch (e)
        {
            case eGetType.GetARGV:
                {
                    int i = vc.currIndex++;
                    if (JSApi.JSh_ArgvIsDouble(vc.cx, vc.vp, i))
                        return JSApi.JSh_ArgvDouble(vc.cx, vc.vp, i);
                    else
                        return (double)JSApi.JSh_ArgvInt(vc.cx, vc.vp, i);
                }
                break;
            case eGetType.GetJSFUNRET:
                {
                    if (JSApi.JSh_JsvalIsDouble(ref vc.rvalCallJS))
                        return JSApi.JSh_GetJsvalDouble(ref vc.rvalCallJS);
                    else
                        return (double)JSApi.JSh_GetJsvalInt(ref vc.rvalCallJS);
                }
                break;
        }
        return 0;
    }

    public Boolean getBoolean(eGetType e)
    {
        switch (e)
        {
            case eGetType.GetARGV:
                return JSApi.JSh_ArgvBool(vc.cx, vc.vp, vc.currIndex++);
                break;
            case eGetType.GetJSFUNRET:
                return JSApi.JSh_GetJsvalBool(ref vc.rvalCallJS);
                break;
        }
        return false;
    }
    public String getString(eGetType e)
    {
        switch (e)
        {
            case eGetType.GetARGV:
                return JSApi.JSh_ArgvStringS(vc.cx, vc.vp, vc.currIndex++);
                break;
            case eGetType.GetJSFUNRET:
                return JSApi.JSh_GetJsvalStringS(vc.cx, ref vc.rvalCallJS);
                break;
        }
        return string.Empty;
    }
    public Char getChar(eGetType e)
    {
        return (Char)getNumberic(e);
    }
    public SByte getSByte(eGetType e)
    {
        return (SByte)getNumberic(e);
    }
    public Byte getByte(eGetType e)
    {
        return (Byte)getNumberic(e);
    }
    public Int16 getInt16(eGetType e)
    {
        return (Int16)getNumberic(e);
    }
    public UInt16 getUInt16(eGetType e)
    {
        return (UInt16)getNumberic(e);
    }
    public Int32 getInt32(eGetType e)
    {
        return (Int32)getNumberic(e);
    }
    public UInt32 getUInt32(eGetType e)
    {
        return (UInt32)getNumberic(e);
    }
    public Int64 getInt64(eGetType e)
    {
        return (Int64)getNumberic(e);
    }
    public UInt64 getUInt64(eGetType e)
    {
        return (UInt64)getNumberic(e);
    }
    public Int32 getEnum(eGetType e)
    {
        return (Int32)getNumberic(e);
    }
    public Single getSingle(eGetType e)
    {
        return (Single)getNumberic(e);
    }
    public Double getDouble(eGetType e)
    {
        return (Double)getNumberic(e);
    }
    #endregion


    public enum eSetType
    {
        SetRval,
        //GetJSFUNRET
    }

    #region Set Operation

    public void setBoolean(eSetType e, bool v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalBool(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setString(eSetType e, string v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalString(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setChar(eSetType e, char v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setSByte(eSetType e, SByte v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setByte(eSetType e, Byte v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setInt16(eSetType e, Int16 v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setUInt16(eSetType e, UInt16 v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setInt32(eSetType e, Int32 v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setUInt32(eSetType e, UInt32 v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
        }
    }
    public void setInt64(eSetType e, Int64 v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
        }
    }
    public void getUInt64(eSetType e, UInt64 v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
        }
    }
    public void getEnum(eSetType e, int v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
        }
    }
    public void getSingle(eSetType e, float v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
        }
    }
    public void getDouble(eSetType e, Double v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
        }
    }
    #endregion

    public static string GetTypeFullName(Type type)
    {
        if (type.IsByRef)
            type = type.GetElementType();

        if (!type.IsGenericType)
        {
            string rt = type.FullName;
            rt = rt.Replace('+', '.');
            return rt;
        }
        else
        {
            string fatherName = type.Name.Substring(0, type.Name.Length - 2);
            Type[] ts = type.GetGenericArguments();
            fatherName += "<";
            for (int i = 0; i < ts.Length; i++)
            {
                fatherName += ts[i].Name;
                if (i != ts.Length - 1)
                    fatherName += ", ";
            }
            fatherName += ">";
            fatherName.Replace('+', '.');
            return fatherName;
        }
    }

    Dictionary<Type, JSDataExchange> dict;
    public void init()
    {
        dict = new Dictionary<Type, JSDataExchange>();
        dict.Add(typeof(String), new JSDataExchange_String());
        dict.Add(typeof(Boolean), new JSDataExchange_Boolean());
        dict.Add(typeof(Char), new JSDataExchange_Char());
        dict.Add(typeof(Byte), new JSDataExchange_Byte());
        dict.Add(typeof(SByte), new JSDataExchange_SByte());
        dict.Add(typeof(Int16), new JSDataExchange_Int16());
        dict.Add(typeof(UInt16), new JSDataExchange_UInt16());
        dict.Add(typeof(Int32), new JSDataExchange_Int32());
        dict.Add(typeof(UInt32), new JSDataExchange_UInt32());
        dict.Add(typeof(Int64), new JSDataExchange_Int64());
        dict.Add(typeof(UInt64), new JSDataExchange_UInt64());
        dict.Add(typeof(Enum), new JSDataExchange_Enum());
        dict.Add(typeof(Single), new JSDataExchange_Single());
        dict.Add(typeof(Double), new JSDataExchange_Double());
    }


    public string Get_GetParam(Type t)
    {
        JSDataExchange de;
        if (dict.TryGetValue(t, out de))
        {
            return de.Get_GetParam(t);
        }
        else
        {

        }
    }
    public string Get_Return(Type t, string expVar) 
    {
        JSDataExchange de;
        if (dict.TryGetValue(t, out de))
        {
            return de.Get_Return(expVar);
        }
        else
        {

        }
    }
}

public class JSDataExchange 
{
    // get value from param
    public virtual string Get_GetParam(Type t) { return string.Empty; }
    public virtual string Get_Return(string expVar) { return string.Empty; }
}

#region Actual Data Exchange (Only for Editor)

public class JSDataExchange_String : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getString(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setString(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Boolean : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getBoolean(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setBoolean(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Char : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getChar(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setChar(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Byte : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getByte(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setByte(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_SByte : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getSByte(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setSByte(eSetType.SetRval, " + expVar + ")"; }
}
public class JSDataExchange_Int16 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getInt16(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setInt16(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_UInt16 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getUInt16(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setUInt16(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Int32 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getInt32(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setInt32(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_UInt32 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getUInt32(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setUInt32(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Int64 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getInt64(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setInt64(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_UInt64 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getUInt64(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setUInt64(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Enum : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "(" + JSDataExchangeMgr.GetTypeFullName(t) + ")" + "getEnum(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setEnum(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Single : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getSingle(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setSingle(eSetType.SetRval, " + expVar + ")"; }
}

public class JSDataExchange_Double : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "getDouble(eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "setDouble(eSetType.SetRval, " + expVar + ")"; }
}

#endregion