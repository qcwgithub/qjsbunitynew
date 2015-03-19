using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

using jsval = JSApi.jsval;

public class JSDataExchangeMgr
{
    const int VALUE_LEN = -1;

    JSVCall vc;
    public JSDataExchangeMgr(JSVCall vc)
    {
        this.vc = vc;
    }

    public enum eGetType
    {
        GetARGV,
        GetARGVRefOut,
        GetJSFUNRET
    }

    System.Object mTempObj;
    public void setTemp(System.Object obj)
    {
        mTempObj = obj;
    }

    #region Get Operation

    public object getByType(eGetType e)
    {
        Type type = (Type)mTempObj;
        if (type.IsByRef)
            type = type.GetElementType();

        if (type == typeof(string))
            return getString(e);
        else if (type.IsEnum)
            return getEnum(e);
        else if (type.IsPrimitive)
        {
            if (type == typeof(System.Boolean))
                return getBoolean(e);
            else if (type == typeof(System.Char))
                return getChar(e);
            else if (type == typeof(System.Byte))
                return getByte(e);
            else if (type == typeof(System.SByte))
                return getSByte(e);
            else if (type == typeof(System.UInt16))
                return getUInt16(e);
            else if (type == typeof(System.Int16))
                return getInt16(e);
            else if (type == typeof(System.UInt32))
                return getUInt32(e);
            else if (type == typeof(System.Int32))
                return getInt32(e);
            else if (type == typeof(System.UInt64))
                return getUInt64(e);
            else if (type == typeof(System.Int64))
                return getInt64(e);
            else if (type == typeof(System.Single))
                return getSingle(e);
            else if (type == typeof(System.Double))
                return getDouble(e);
            else
                Debug.LogError("Unknown primitive type");
        }
        else
        {
            return getObject(e);
        }
        return null;
    }


    public void getJSValueOfParam(ref jsval val, int pIndex)
    {
        IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, pIndex);
        if (jsObj != IntPtr.Zero) 
        {
            JSApi.GetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
        }
        else
        {
            Debug.LogError("ref/out param must be js object");
        }
    }

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
            case eGetType.GetARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalUndefined(ref val);
                    getJSValueOfParam(ref val, vc.currIndex++);
                    if (JSApi.JSh_JsvalIsNullOrUndefined(ref val))
                        return 0;
                    if (JSApi.JSh_JsvalIsDouble(ref val))
                        return JSApi.JSh_GetJsvalDouble(ref val);
                    else
                        return JSApi.JSh_GetJsvalInt(ref val);
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
            default:
                Debug.LogError("Not Supported");
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
            case eGetType.GetARGVRefOut:
                {
                    jsval val = new jsval();
                    getJSValueOfParam(ref val, vc.currIndex++);
                    return JSApi.JSh_GetJsvalBool(ref val);
                }
                break;
            case eGetType.GetJSFUNRET:
                return JSApi.JSh_GetJsvalBool(ref vc.rvalCallJS);
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
        return false;
    }
    public String getString(eGetType e)
    {
        switch (e)
        {
            case eGetType.GetARGV:
		        {
			        string s = JSApi.JSh_ArgvStringS(vc.cx, vc.vp, vc.currIndex++);
			        return s;
		        }
                break;
            case eGetType.GetARGVRefOut:
                {
                    jsval val = new jsval();
                    getJSValueOfParam(ref val, vc.currIndex++);
                    return JSApi.JSh_GetJsvalStringS(vc.cx, ref val);
                }
                break;
            case eGetType.GetJSFUNRET:
                return JSApi.JSh_GetJsvalStringS(vc.cx, ref vc.rvalCallJS);
                break;
            default:
                Debug.LogError("Not Supported");
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
    public jsval getFunction(eGetType e)
    {
        jsval val = new jsval();
        val.asBits = 0;
        switch (e)
        {
            case eGetType.GetARGV:
                JSApi.JSh_ArgvFunctionValue(vc.cx, vc.vp, vc.currIndex++, ref val);
                break;
            case eGetType.GetARGVRefOut:
                {
                    Debug.LogError("getFunction not support eGetType.GetARGVRefOut");
                }
                break;
            case eGetType.GetJSFUNRET:
                {
                    Debug.LogError("getFunction not support eGetType.GetJSFUNRET");
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
        return val;
    }
    public object getObject(eGetType e)
    {
        switch (e)
        {
            case eGetType.GetARGV:
                {
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex++);
                    if (jsObj == IntPtr.Zero)
                        return null;

                    jsval val = new jsval();
                    JSApi.JSh_GetUCProperty(JSMgr.cx, jsObj, "__nativeObj", -1, ref val);
                    IntPtr __nativeObj = JSApi.JSh_GetJsvalObject(ref val);
                    if (__nativeObj == IntPtr.Zero)
                        return null;

                    object csObj = JSMgr.getCSObj(__nativeObj);
                    return csObj;
                }
                break;
            case eGetType.GetARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalUndefined(ref val);
                    getJSValueOfParam(ref val, vc.currIndex++);

                    IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref val);
                    object csObj = JSMgr.getCSObj(jsObj);
                    return csObj;
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
        return null;
    }

    public object getWhatever(eGetType e)
    {
        switch (e)
        {
            case eGetType.GetARGV:
                {
                    int i = vc.currIndex;
                    if (JSApi.JSh_ArgvIsNullOrUndefined(vc.cx, vc.vp, i))
                        return null;
                    else if (JSApi.JSh_ArgvIsBool(vc.cx, vc.vp, i))
                        return getBoolean(e);
                    else if (JSApi.JSh_ArgvIsInt32(vc.cx, vc.vp, i))
                        return getInt32(e);
                    else if (JSApi.JSh_ArgvIsDouble(vc.cx, vc.vp, i))
                        return getDouble(e);
                    else if (JSApi.JSh_ArgvIsString(vc.cx, vc.vp, i))
                        return getString(e);
                    else if (JSApi.JSh_ArgvIsObject(vc.cx, vc.vp, i))
                    {
                        return getObject(e);
                    }
                    return null;
                }
                break;
            default:
                Debug.LogError("getWhatever ////// Not Supported");
                break;
        }
        return null;
    }
    #endregion


    public enum eSetType
    {
        SetRval,
        UpdateARGVRefOut,
        Jsval,
        //GetJSFUNRET
    }

    #region Set Operation

    public void setByType(eSetType e, object obj)
    {
        Type type = (Type)mTempObj;
        if (type.IsByRef)
            type = type.GetElementType();

        if (type == typeof(string))
            setString(e, (string)obj);
        else if (type.IsEnum)
            setEnum(e, (int)obj);
        else if (type.IsPrimitive)
        {
            if (type == typeof(System.Boolean))
                setBoolean(e, (bool)obj);
            else if (type == typeof(System.Char))
                setChar(e, (char)obj);
            else if (type == typeof(System.Byte))
                setByte(e, (Byte)obj);
            else if (type == typeof(System.SByte))
                setSByte(e, (SByte)obj);
            else if (type == typeof(System.UInt16))
                setUInt16(e, (UInt16)obj);
            else if (type == typeof(System.Int16))
                setInt16(e, (Int16)obj);
            else if (type == typeof(System.UInt32))
                setUInt32(e, (UInt32)obj);
            else if (type == typeof(System.Int32))
                setInt32(e, (Int32)obj);
            else if (type == typeof(System.UInt64))
                setUInt64(e, (UInt64)obj);
            else if (type == typeof(System.Int64))
                setInt64(e, (Int64)obj);
            else if (type == typeof(System.Single))
                setSingle(e, (Single)obj);
            else if (type == typeof(System.Double))
                setDouble(e, (Double)obj);
            else
                Debug.LogError("Unknown primitive type");
        }
        else
        {
            setObject(e, obj);
        }
    }


    public void setBoolean(eSetType e, bool v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalBool(vc.cx, vc.vp, v);
                break;
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalBool(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalString(vc.cx, ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalUInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalDouble(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
    }
    public void setUInt64(eSetType e, UInt64 v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalDouble(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
    }
    public void setEnum(eSetType e, int v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalInt(vc.cx, vc.vp, v);
                break;
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
    }
    public void setSingle(eSetType e, float v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalDouble(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
    }
    public void setDouble(eSetType e, Double v)
    {
        switch (e)
        {
            case eSetType.SetRval:
                JSApi.JSh_SetRvalDouble(vc.cx, vc.vp, v);
                break;
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalDouble(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.SetProperty(vc.cx, jsObj, "Value", VALUE_LEN, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
    }
    public void setObject(eSetType e, object csObj)
    {
        switch (e)
        {
            case eSetType.SetRval:
                {
                    JSApi.JSh_SetJsvalUndefined(ref vc.valReturn);
                    if (csObj == null)
                    {
                        JSApi.JSh_SetRvalJSVAL(vc.cx, vc.vp, ref vc.valReturn);
                        return;
                    }

                    //
                    // 返回给JS的对象：需要 prototype
                    // 他包含的__nativeObj：需要 finalizer，需要 csObj 对应
                    //
                    string typeName = JSDataExchangeMgr.GetTypeFullName(csObj.GetType());
                    IntPtr jstypeObj = JSDataExchangeMgr.GetJSObjectByname(typeName);
                    if (jstypeObj != IntPtr.Zero)
                    {
                        IntPtr jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null /*JSMgr.mjsFinalizer*/);

                        // __nativeObj
                        IntPtr __nativeObj = JSApi.JSh_NewMyClass(JSMgr.cx, JSMgr.mjsFinalizer);
                        JSMgr.addJSCSRelation(__nativeObj, csObj);

                        // jsObj.__nativeObj = __nativeObj
                        jsval val = new jsval();
                        JSApi.JSh_SetJsvalObject(ref val, __nativeObj);
                        JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, "__nativeObj", -1, ref val);

                        JSApi.JSh_SetJsvalObject(ref vc.valReturn, jsObj);
                    }
                    JSApi.JSh_SetRvalJSVAL(vc.cx, vc.vp, ref vc.valReturn);
                }
                break;
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval(); val.asBits = 0;
                    IntPtr argvJSObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (argvJSObj != IntPtr.Zero)
                    {
                        bool success = false;

                        // csObj must not be null
                        IntPtr jstypeObj = JSDataExchangeMgr.GetJSObjectByname(JSDataExchangeMgr.GetTypeFullName(csObj.GetType()));
                        if (jstypeObj != IntPtr.Zero)
                        {
                            // 1)
                            // jsObj: prototype  
                            // __nativeObj: csObj + finalizer
                            // 
                            IntPtr jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null /*JSMgr.mjsFinalizer*/);
                            // __nativeObj
                            IntPtr __nativeObj = JSApi.JSh_NewMyClass(JSMgr.cx, JSMgr.mjsFinalizer);
                            JSMgr.addJSCSRelation(__nativeObj, csObj);

                            //
                            // 2)
                            // jsObj.__nativeObj = __nativeObj
                            //
                            JSApi.JSh_SetJsvalObject(ref val, __nativeObj);
                            JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, "__nativeObj", -1, ref val);

                            // 3)
                            // argvObj.Value = jsObj
                            //
                            if (argvJSObj != IntPtr.Zero)
                            {
                                JSApi.JSh_SetJsvalObject(ref val, jsObj);
                                JSApi.JSh_SetUCProperty(vc.cx, argvJSObj, "Value", -1, ref val);
                                success = true;
                            }
                        }

                        if (!success)
                        {
                            JSApi.JSh_SetJsvalUndefined(ref val);
                            JSApi.JSh_SetUCProperty(vc.cx, argvJSObj, "Value", -1, ref val);
                        }
                    }
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
    }
    #endregion

    public static string GetTypeFullName(Type type)
    {
        if (type == null) return "";

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

    static Dictionary<string, Type> typeCache = new Dictionary<string,Type>();
    public static Type GetTypeByName(string typeName)
    {
        Type t = null;
        if (!typeCache.TryGetValue(typeName, out t))
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = a.GetType(typeName);
                if (t != null)
                    break;
            }
            typeCache[typeName] = t; // perhaps null
        }
        return t;
    }

    static Dictionary<Type, JSDataExchange> dict;
    static JSDataExchange enumExchange;
    static JSDataExchange objExchange;
    static JSDataExchange t_Exchange;

    // Editor only
    public static void reset()
    {
        dict = new Dictionary<Type, JSDataExchange>();

        dict.Add(typeof(Boolean), new JSDataExchange_Boolean());
        dict.Add(typeof(Byte), new JSDataExchange_Byte());
        dict.Add(typeof(SByte), new JSDataExchange_SByte());
        dict.Add(typeof(Char), new JSDataExchange_Char());
        dict.Add(typeof(Int16), new JSDataExchange_Int16());
        dict.Add(typeof(UInt16), new JSDataExchange_UInt16());
        dict.Add(typeof(Int32), new JSDataExchange_Int32());
        dict.Add(typeof(UInt32), new JSDataExchange_UInt32());
        dict.Add(typeof(Int64), new JSDataExchange_Int64());
        dict.Add(typeof(UInt64), new JSDataExchange_UInt64());
        dict.Add(typeof(Single), new JSDataExchange_Single());
        dict.Add(typeof(Double), new JSDataExchange_Double());

        dict.Add(typeof(String), new JSDataExchange_String());
        dict.Add(typeof(System.Object), new JSDataExchange_SystemObject());

        enumExchange = new JSDataExchange_Enum();
        objExchange = new JSDataExchange_Obj();
        t_Exchange = new JSDataExchange_T();
    }

    // Editor only
    public struct ParamHandler
    {
        public string argName; // argN
        public string getter;
        public string updater;
    }
    // Editor only
    public static ParamHandler Get_TType(int index)
    {
        ParamHandler ph = new ParamHandler();
        ph.argName = "t" + index.ToString();

        string get_getParam = dict[typeof(string)].Get_GetParam(null);
        ph.getter = "System.Type " + ph.argName + " = JSDataExchangeMgr.GetTypeByName(" + get_getParam + ");";

//         string get_getParam = objExchange.Get_GetParam(typeof(Type));
//         ph.getter = "System.Type " + ph.argName + " = (System.Type)" + get_getParam + ";";
        return ph;
    }
    // Editor only
    public static ParamHandler Get_ParamHandler(Type type, int paramIndex, bool isOutOrRef)
    {
        ParamHandler ph = new ParamHandler();
        ph.argName = "arg" + paramIndex.ToString();

        if (type.IsArray)
        {
            //Debug.LogError("Parameter: Array not supported");
            //return ph;
        }

        if (typeof(System.Delegate).IsAssignableFrom(type))
        {
            //Debug.LogError("Delegate should not get here");
            return ph;
        }

        if (isOutOrRef)
        {
            type = type.GetElementType();
        }

        JSDataExchange xcg = null;
        if (type.IsGenericParameter) 
        {
            xcg = t_Exchange;
        }
        if (xcg == null)
        {
            dict.TryGetValue(type, out xcg);
        }

        if (xcg == null) 
        { 
            if (type.IsPrimitive)
            {
                Debug.LogError("Unknown Primitive Type: " + type.ToString());
                return ph;
            }
            if (type.IsEnum)
            {
                xcg = enumExchange;
            }
            else
            {
                xcg = objExchange;
            }
        }

        string typeFullName = GetTypeFullName(type);
        string get_getParam = string.Empty;
        if (isOutOrRef)
        {
            get_getParam = xcg.Get_GetRefOutParam(type);
            ph.getter = "int r_arg" + paramIndex.ToString() + " = vc.currIndex;\n";
        }
        else
        {
            get_getParam = xcg.Get_GetParam(type);
            ph.getter = string.Empty;
        }
        if (xcg.isGetParamNeedCast)
        {
            ph.getter += typeFullName + " " + ph.argName + " = (" + typeFullName + ")" + get_getParam + ";";
        }
        else
        {
            ph.getter += typeFullName + " " + ph.argName + " = " + get_getParam + ";";
        }

        if (isOutOrRef)
        {
            ph.updater = "vc.currIndex = r_arg" + paramIndex.ToString() + ";\n";
            ph.updater += xcg.Get_ReturnRefOut(ph.argName) + ";";
        }
        return ph;
    }

    // Editor only
    public static ParamHandler Get_ParamHandler(ParameterInfo paramInfo, int paramIndex)
    {
        return Get_ParamHandler(paramInfo.ParameterType, paramIndex, paramInfo.ParameterType.IsByRef || paramInfo.IsOut);
    }
    // Editor only
    public static ParamHandler Get_ParamHandler(FieldInfo fieldInfo)
    {
        return Get_ParamHandler(fieldInfo.FieldType, 0, false);//fieldInfo.FieldType.IsByRef);
    }
    // Editor only
    public static string Get_Return(Type type, string expVar) 
    {
        if (type == typeof(void))
            return expVar + ";";

        JSDataExchange xcg = null;
        dict.TryGetValue(type, out xcg);
        if (xcg == null)
        {
            if (type.IsPrimitive)
            {
                Debug.LogError("Unknown Primitive Type: " + type.ToString());
                return "";
            }
            if (type.IsEnum)
            {
                xcg = enumExchange;
            }
            else
            {
                xcg = objExchange;
            }
        }
        return xcg.Get_Return(expVar) + ";";
    }

    // new obj, and assign prototype
    // set __nativeObj
    // and return
    public static IntPtr NewJSObject(string typeFullName)
    {
        jsval[] valParam = new jsval[1];
        JSApi.JSh_SetJsvalString(JSMgr.cx, ref valParam[0], typeFullName);

        jsval valRet = new jsval();
        valRet.asBits = 0;
        JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.glob, "jsb_NewObject", 1, valParam, ref valRet);
        if (JSApi.JSh_JsvalIsNullOrUndefined(ref valRet))
            return IntPtr.Zero;

        IntPtr jsObj = JSApi.JSh_NewMyClass(JSMgr.cx, JSMgr.mjsFinalizer);
        JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, "__nativeObj", -1, ref valRet);
        return jsObj;
    }

    public static IntPtr GetJSObjectByname(string name)
    {
        string[] arr = name.Split('.');
        IntPtr obj = JSMgr.glob;
        jsval val = new jsval();
        val.asBits = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            JSApi.JSh_GetUCProperty(JSMgr.cx, obj, arr[i], -1, ref val);
            obj = JSApi.JSh_GetJsvalObject(ref val);
            if (obj == IntPtr.Zero)
                return IntPtr.Zero;
            val.asBits = 0;
        }
        return obj;
    }

    public static MethodInfo GetGenericMethodInfo(Type type, string name, int paramCount)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static/* | BindingFlags.DeclaredOnly */;
        MethodInfo[] methods = type.GetMethods(flags);
        if (methods == null || methods.Length == 0) 
            return null;

        MethodInfo method = null;
        for (int i = 0; i < methods.Length; i++)
        {
            if (methods[i].Name == name && 
                methods[i].IsGenericMethodDefinition &&
                methods[i].GetParameters().Length == paramCount)
            {
                if (method == null)
                    method = methods[i];
                else
                {
                    Debug.LogError("More than 1 Generic method found!!! " + GetTypeFullName(type) + "." + name);
                    return null;
                }
            }
        }
        return method;
    }
}

public class JSDataExchange 
{
    // get value from param
    public virtual string Get_GetParam(Type t) { Debug.LogError("X Get_GetParam "); return string.Empty; }
    public virtual bool isGetParamNeedCast { get { return false; } }

    public virtual string Get_Return(string expVar) { Debug.LogError("X Get_Return "); return string.Empty; }

    public virtual string Get_GetRefOutParam(Type t) { Debug.LogError("X Get_GetRefOutParam "); return string.Empty; }
    public virtual string Get_ReturnRefOut(string expVar) { Debug.LogError("X Get_ReturnRefOut "); return string.Empty; }
}

#region Actual Data Exchange (Only for Editor)

public class JSDataExchange_Boolean : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getBoolean(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setBoolean(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getBoolean(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setBoolean(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Byte : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getByte(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setByte(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getByte(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setByte(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_SByte : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getSByte(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setSByte(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getSByte(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setSByte(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Char : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getChar(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setChar(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getChar(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setChar(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Int16 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getInt16(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setInt16(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getInt16(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setInt16(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_UInt16 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getUInt16(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setUInt16(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getUInt16(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setUInt16(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Int32 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getInt32(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setInt32(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getInt32(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setInt32(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_UInt32 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getUInt32(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setUInt32(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getUInt32(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setUInt32(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Int64 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getInt64(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setInt64(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getInt64(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setInt64(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_UInt64 : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getUInt64(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setUInt64(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getUInt64(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setUInt64(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Single : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setSingle(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setSingle(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Double : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getDouble(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setDouble(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getDouble(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setDouble(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}

// System.Object
public class JSDataExchange_SystemObject : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getWhatever(JSDataExchangeMgr.eGetType.GetARGV)"; }
    //public override string Get_Return(string expVar) { return "setWhatever(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    //public override string Get_GetRefOutParam(Type t) { return "getWhatever(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    //public override string Get_ReturnRefOut(string expVar) { return "setWhatever(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
    public override bool isGetParamNeedCast { get { return true; } }
}
public class JSDataExchange_String : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getString(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setString(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getString(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setString(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
}
public class JSDataExchange_Enum : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getEnum(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setEnum(JSDataExchangeMgr.eSetType.SetRval, (int)" + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getEnum(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setEnum(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, (int)" + expVar + ")"; }
    public override bool isGetParamNeedCast { get { return true; } }
}
public class JSDataExchange_Obj : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getObject(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setObject(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getObject(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setObject(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
    public override bool isGetParamNeedCast { get { return true; } }

}
// generic 
public class JSDataExchange_T : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getByType(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setByType(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getByType(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setByType(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
    public override bool isGetParamNeedCast { get { return true; } }

}

#endregion