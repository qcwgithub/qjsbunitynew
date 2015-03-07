using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

using jsval = JSApi.jsval;

public class JSDataExchangeMgr
{
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

    #region Get Operation

    public void getJSValueOfParam(ref jsval val, int pIndex)
    {
        IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, pIndex);
        if (jsObj != IntPtr.Zero) 
        {
            JSApi.JSh_GetUCProperty(vc.cx, vc.vp, "Value", 5, ref val);
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
                    int i = vc.currIndex;
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
                    getJSValueOfParam(ref val, vc.currIndex);
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
                return JSApi.JSh_ArgvBool(vc.cx, vc.vp, vc.currIndex);
                break;
            case eGetType.GetARGVRefOut:
                {
                    jsval val = new jsval();
                    getJSValueOfParam(ref val, vc.currIndex);
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
                return JSApi.JSh_ArgvStringS(vc.cx, vc.vp, vc.currIndex);
                break;
            case eGetType.GetARGVRefOut:
                {
                    jsval val = new jsval();
                    getJSValueOfParam(ref val, vc.currIndex);
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
                JSApi.JSh_ArgvFunctionValue(vc.cx, vc.vp, vc.currIndex, ref val);
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
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero)
                        return null;

                    object csObj = JSMgr.getCSObj(jsObj);
                    return csObj;
                }
                break;
            case eGetType.GetARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalUndefined(ref val);
                    getJSValueOfParam(ref val, vc.currIndex);

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
                    if (JSApi.JSh_ArgvIsBool(vc.cx, vc.vp, i))
                        return JSApi.JSh_ArgvBool(vc.cx, vc.vp, i);
                    else if (JSApi.JSh_ArgvIsInt32(vc.cx, vc.vp, i))
                        return JSApi.JSh_ArgvInt(vc.cx, vc.vp, i);
                    else if (JSApi.JSh_ArgvIsDouble(vc.cx, vc.vp, i))
                        return JSApi.JSh_ArgvDouble(vc.cx, vc.vp, i);
                    else if (JSApi.JSh_ArgvIsString(vc.cx, vc.vp, i))
                        return JSApi.JSh_ArgvStringS(vc.cx, vc.vp, i);
                    else if (JSApi.JSh_ArgvIsObject(vc.cx, vc.vp, i))
                    {
                        IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, i);
                        object csObj = JSMgr.getCSObj(jsObj);
                        return csObj;
                    }
                    else if (JSApi.JSh_ArgvIsNullOrUndefined(vc.cx, vc.vp, i))
                        return null;
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalDouble(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalInt(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalDouble(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
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
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalDouble(ref val, v);
                    IntPtr jsObj = JSApi.JSh_ArgvObject(vc.cx, vc.vp, vc.currIndex);
                    if (jsObj == IntPtr.Zero) Debug.LogError("ref/out param must be js obj!");
                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
                }
                break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
    }
    public void setObject(eSetType e, string className, object csObj)
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

                    IntPtr jsObj;
                    //jsObj = JSMgr.getJSObj(csObj);
                    //if (jsObj == IntPtr.Zero)
                    { // always add a new jsObj
                        jsObj = JSApi.JSh_NewObjectAsClass(vc.cx, JSMgr.glob, /*className*/csObj.GetType().Name, JSMgr.mjsFinalizer);
                        if (jsObj == IntPtr.Zero)
                            jsObj = JSApi.JSh_NewObjectAsClass(vc.cx, JSMgr.glob, /*className*/csObj.GetType().BaseType.Name, JSMgr.mjsFinalizer);
                        if (jsObj != IntPtr.Zero)
                            JSMgr.addJSCSRelation(jsObj, csObj);
                    }

                    if (jsObj == IntPtr.Zero)
                        JSApi.JSh_SetJsvalUndefined(ref vc.valReturn);
                    else
                        JSApi.JSh_SetJsvalObject(ref vc.valReturn, jsObj);

                    JSApi.JSh_SetRvalJSVAL(vc.cx, vc.vp, ref vc.valReturn);
                }
                break;
            case eSetType.UpdateARGVRefOut:
                {
                    jsval val = new jsval();
                    JSApi.JSh_SetJsvalUndefined(ref vc.valReturn);
                    // csObj must not be null

                    IntPtr jsObj;
                    //jsObj = JSMgr.getJSObj(csObj);
                    //if (jsObj == IntPtr.Zero)
                    { // always add a new jsObj
                        jsObj = JSApi.JSh_NewObjectAsClass(vc.cx, JSMgr.glob, /*className*/csObj.GetType().Name, JSMgr.mjsFinalizer);
                        if (jsObj == IntPtr.Zero)
                            jsObj = JSApi.JSh_NewObjectAsClass(vc.cx, JSMgr.glob, /*className*/csObj.GetType().BaseType.Name, JSMgr.mjsFinalizer);
                        if (jsObj != IntPtr.Zero)
                            JSMgr.addJSCSRelation(jsObj, csObj);
                    }


                    if (jsObj == IntPtr.Zero)
                        JSApi.JSh_SetJsvalUndefined(ref val);
                    else
                        JSApi.JSh_SetJsvalObject(ref val, jsObj);

                    JSApi.JSh_SetUCProperty(vc.cx, jsObj, "Value", 5, ref val);
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

    static Dictionary<Type, JSDataExchange> dict;
    static JSDataExchange enumExchange;
    static JSDataExchange objExchange;
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
    }

    // Editor only
    public struct ParamHandler
    {
        public string argName; // argN
        public string getter;
        public string updater;
    }
    // Editor only
    public static ParamHandler Get_ParamHandler(Type type, int paramIndex, bool isOutOrRef)
    {
        ParamHandler ph = new ParamHandler();
        ph.argName = "arg" + paramIndex.ToString();

        if (type.IsArray)
        {
            Debug.LogError("Parameter: Array not supported");
            return ph;
        }

        JSDataExchange xcg;
        if (dict.TryGetValue(type, out xcg)) {}
        if (xcg == null && type.IsPrimitive) { 
            Debug.LogError("Unknown Primitive Type: " + type.ToString()); 
        }
        if (typeof(System.Delegate).IsAssignableFrom(type)) { 
            Debug.LogError("Delegate should not get here"); 
        }
        if (type.IsEnum) xcg = enumExchange;
        xcg = objExchange;

        string typeFullName = GetTypeFullName(type);
        string get_getParam = isOutOrRef ? xcg.Get_GetRefOutParam(type) : xcg.Get_GetParam(type);
        if (isOutOrRef) { ph.updater = xcg.Get_ReturnRefOut(ph.argName) + ";"; }
        if (xcg.needCast) { 
            ph.getter = typeFullName + " " + ph.argName + " = (" + typeFullName + ")" + xcg.Get_GetParam(type) + ";"; 
        }
        else { 
            ph.getter = typeFullName + " " + ph.argName + " = " + xcg.Get_GetParam(type) + ";"; 
        }
        
        if (!isOutOrRef)
        {
            ph.getter = typeFullName + " " + ph.argName + " = " + xcg.Get_GetParam(type) + ";";
            ph.updater = string.Empty;
        }
        else
        {
            ph.getter = typeFullName + " " + ph.argName + " = " + xcg.Get_GetRefOutParam(type);
            ph.updater = xcg.Get_ReturnRefOut(ph.argName) + ";";
        }
        return ph;
    }

    // Editor only
    public static ParamHandler Get_ParamHandler(ParameterInfo paramInfo, int paramIndex)
    {
        return Get_ParamHandler(paramInfo.ParameterType, paramIndex, paramInfo.ParameterType.IsByRef || paramInfo.IsOut);
    }
    public static string Get_Return(Type t, string expVar) 
    {
        JSDataExchange de;
        if (dict.TryGetValue(t, out de))
        {
            return de.Get_Return(expVar) + ";";
        }
        return "";
    }
}

public class JSDataExchange 
{
    // get value from param
    public virtual string Get_GetParam(Type t) { Debug.LogError("X Get_GetParam "); return string.Empty; }
    public virtual string Get_Return(string expVar) { Debug.LogError("X Get_Return "); return string.Empty; }

    public virtual string Get_GetRefOutParam(Type t) { Debug.LogError("X Get_GetRefOutParam "); return string.Empty; }
    public virtual string Get_ReturnRefOut(string expVar) { Debug.LogError("X Get_ReturnRefOut "); return string.Empty; }

    public virtual bool needCast { get { return false; } }
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
    public override bool needCast { get { return true; } }
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
    public override string Get_GetParam(Type t) { return "(" + JSDataExchangeMgr.GetTypeFullName(t) + ")" + "vc.datax.getEnum(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setEnum(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getEnum(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setEnum(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
    public override bool needCast { get { return true; } }
}
public class JSDataExchange_Obj : JSDataExchange
{
    public override string Get_GetParam(Type t) { return "vc.datax.getObject(JSDataExchangeMgr.eGetType.GetARGV)"; }
    public override string Get_Return(string expVar) { return "vc.datax.setObject(JSDataExchangeMgr.eSetType.SetRval, " + expVar + ")"; }
    public override string Get_GetRefOutParam(Type t) { return "vc.datax.getObject(JSDataExchangeMgr.eGetType.GetARGVRefOut)"; }
    public override string Get_ReturnRefOut(string expVar) { return "vc.datax.setObject(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, " + expVar + ")"; }
    public override bool needCast { get { return true; } }

}

#endregion