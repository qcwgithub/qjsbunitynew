/*
 * JSVCall
 * 
 * It's the STACK used when calling cs from js
 * 
 */


using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using jsval = JSApi.jsval;


public class JSVCall
{
    public enum Consts
    {
        MaxParams = 16,
    }
    public class JSParam
    {
        public int index; // param index
        public object csObj; // coresponding cs object, for primitive, string, enum, it's null
        public bool isWrap { get { return csObj != null && csObj is JSValueWrap.Wrap; } }
        public object wrappedObj { get { return ((JSValueWrap.Wrap)csObj).obj; } set { ((JSValueWrap.Wrap)csObj).obj = value; } }
        public bool isArray;
        public bool isNull;
    }
    // cs function information
    public class CSParam
    {
        public bool isRef;
        public bool isOptional;
        public bool isArray;
        public Type type;
        public object defaultValue;
        public CSParam() { }
        public CSParam(bool r, bool o, bool i, Type t, object d)
        {
            isRef = r;
            isOptional = o;
            isArray = i;
            type = t;
            defaultValue = d;
        }
    }

    public JSParam[] arrJSParam = null;
    public int arrJSParamsLength = 0;

    public CSParam[] arrCSParam = null;
    public int arrCSParamsLength = 0;

    public object[] callParams = null;
    public int callParamsLength = 0;

    public MethodBase m_Method;
    public ParameterInfo[] m_ParamInfo;

    public IntPtr cx;
    public IntPtr vp;
    public int currIndex = 0;
    public jsval valReturn = new jsval();

    public void Reset(IntPtr cx, IntPtr vp)
    {
        if (arrJSParam == null)
        {
            arrJSParam = new JSParam[(int)Consts.MaxParams];
            arrCSParam = new CSParam[(int)Consts.MaxParams];
            for (int i = 0; (int)Consts.MaxParams > i; i++)
            {
                arrJSParam[i] = new JSParam();
                arrCSParam[i] = new CSParam();
            }
            callParams = new object[(int)Consts.MaxParams];
        }
        arrJSParamsLength = 0;
        arrCSParamsLength = 0;
        callParamsLength = 0;

        m_Method = null;
        m_ParamInfo = null;

        this.cx = cx;
        this.vp = vp;
        currIndex = 0;
    }
    //
    //
    // get parameters
    //
    public Boolean getBool() { return JSApi.JSh_ArgvBool(cx, vp, currIndex++); }
    public String getString() { return JSApi.JSh_ArgvStringS(cx, vp, currIndex++); }
    public Char getChar() { return (Char)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public Byte getByte() { return (Byte)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public SByte getSByte() { return (SByte)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public UInt16 getUInt16() { return (UInt16)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public Int16 getInt16() { return (Int16)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public UInt32 getUInt32() { return (UInt32)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public Int32 getInt32() { return (Int32)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public UInt64 getUInt64() { return (UInt64)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public Int64 getInt64() { return (Int64)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public Int32 getEnum() { return (Int32)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    public Single getFloat()
    {
        // js has only int32 and double, so...
        int i = currIndex++;
        if (JSApi.JSh_ArgvIsDouble(cx, vp, i))
            return (Single)JSApi.JSh_ArgvDouble(cx, vp, i);
        else
            return (Single)JSApi.JSh_ArgvInt(cx, vp, i);
    }
    public Double getDouble()
    {
        // js has only int32 and double, so...
        int i = currIndex++;
        if (JSApi.JSh_ArgvIsDouble(cx, vp, i))
            return (Double)JSApi.JSh_ArgvDouble(cx, vp, i);
        else
            return (Double)JSApi.JSh_ArgvInt(cx, vp, i);
    }
    public JSValueWrap.Wrap getWrap()
    {
        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
        object csObj = JSMgr.getCSObj(jsObj);
        return (JSValueWrap.Wrap)csObj;
    }
    
    public object getObject(Type typeParam = null)
    {
        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
        if (jsObj == IntPtr.Zero)
            return null;

        object csObj = JSMgr.getCSObj(jsObj);
        if (csObj is JSValueWrap.Wrap)
            return ((JSValueWrap.Wrap)csObj).obj;
        else if (csObj != null)
            return csObj;

        if (!JSApi.JSh_IsArrayObject(cx, jsObj))
            return null;


        // array params don't work.
        // code must be generated, cann't be dynamically run.
        // because type is unknown during run-time.
        Type typeElement = typeParam.GetElementType();
        jsval valElement = new jsval();

        int length = JSApi.JSh_GetArrayLength(cx, jsObj);
        object[] arr = new object[length];
        for (int i = 0; i < length; i++)
        {
            JSApi.JSh_GetElement(cx, jsObj, (uint)i, ref valElement);
            object csObjElement = JSValue_2_CSObject(typeElement, ref valElement);
            arr[i] = csObjElement;
        }
        return arr;
    }

    //
    // get returnvalue from js
    //
    public Boolean getRtBool() { return JSApi.JSh_GetJsvalBool(ref rvalCallJS); }
    public String getRtString() { return JSApi.JSh_GetJsvalStringS(cx, ref rvalCallJS); }
    public Char getRtChar() { return (Char)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public Byte getRtByte() { return (Byte)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public SByte getRtSByte() { return (SByte)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public UInt16 getRtUInt16() { return (UInt16)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public Int16 getRtInt16() { return (Int16)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public UInt32 getRtUInt32() { return (UInt32)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public Int32 getRtInt32() { return (Int32)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public UInt64 getRtUInt64() { return (UInt64)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public Int64 getRtInt64() { return (Int64)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public Int32 getRtEnum() { return (Int32)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
    public Single getRtFloat()
    {
        if (JSApi.JSh_JsvalIsDouble(ref rvalCallJS))
            return (Single)JSApi.JSh_GetJsvalDouble(ref rvalCallJS);
        else
            return (Single)JSApi.JSh_GetJsvalInt(ref rvalCallJS);
    }
    public Double getRtDouble()
    {
        // js has only int32 and double, so...
        if (JSApi.JSh_JsvalIsDouble(ref rvalCallJS))
            return (Double)JSApi.JSh_GetJsvalDouble(ref rvalCallJS);
        else
            return (Double)JSApi.JSh_GetJsvalInt(ref rvalCallJS);
    }/*
    public JSValueWrap.Wrap getWrap()
    {
        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
        object csObj = JSMgr.getCSObj(jsObj);
        return (JSValueWrap.Wrap)csObj;
    }*/
    public object getRtObject(Type typeParam = null)
    {
        IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref rvalCallJS);
        if (jsObj == IntPtr.Zero)
            return null;

        object csObj = JSMgr.getCSObj(jsObj);
        if (csObj is JSValueWrap.Wrap)
            return ((JSValueWrap.Wrap)csObj).obj;
        else if (csObj != null)
            return csObj;

        if (!JSApi.JSh_IsArrayObject(cx, jsObj))
            return null;

        if (typeParam == null)
            return null;


        // Create array of typeElement
        Type typeElement = typeParam.GetElementType();
        jsval valElement = new jsval();

        int length = JSApi.JSh_GetArrayLength(cx, jsObj);        
        Array arr = Array.CreateInstance(typeElement, length);
        for (int i = 0; i < length; i++)
        {
            JSApi.JSh_GetElement(cx, jsObj, (uint)i, ref valElement);
            object csObjElement = JSValue_2_CSObject(typeElement, ref valElement);
            arr.SetValue(csObjElement, i);
        }
        return arr;
    }

    public struct stJSCS
    {
        public IntPtr jsObj;
        public object csObj;
        public stJSCS(IntPtr j, object c) { jsObj = j; csObj = c; }
    }
    // only for parameters
    //     public stJSCS getValueTypeObject()
    //     {
    //         IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
    //         object csObj = JSMgr.getCSObj(jsObj);
    //         return new stJSCS(jsObj, csObj);
    //     }
    public stJSCS getJSCSObject()
    {
        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
        object csObj = JSMgr.getCSObj(jsObj);
        return new stJSCS(jsObj, csObj);
    }
    public IntPtr getJSFunction()
    {
        return JSApi.JSh_ArgvFunction(cx, vp, currIndex++);
    }
    public jsval getJSFunctionValue()
    {
        jsval val = new jsval();
        JSApi.JSh_ArgvFunctionValue(cx, vp, currIndex++, ref val);
        return val;
    }
    public object getWhatever()
    {
        int i = currIndex++;
        if (JSApi.JSh_ArgvIsBool(cx, vp, i))
            return JSApi.JSh_ArgvBool(cx, vp, i);
        else if (JSApi.JSh_ArgvIsInt32(cx, vp, i))
            return JSApi.JSh_ArgvInt(cx, vp, i);
        else if (JSApi.JSh_ArgvIsDouble(cx, vp, i))
            return JSApi.JSh_ArgvDouble(cx, vp, i);
        else if (JSApi.JSh_ArgvIsString(cx, vp, i))
            return JSApi.JSh_ArgvStringS(cx, vp, i);
        else if (JSApi.JSh_ArgvIsObject(cx, vp, i))
        {
            IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, i);
            object csObj = JSMgr.getCSObj(jsObj);
            if (csObj is JSValueWrap.Wrap)
                return ((JSValueWrap.Wrap)csObj).obj;
            else
                return csObj;
        }
        else if (JSApi.JSh_ArgvIsNullOrUndefined(cx, vp, i))
            return null;
        return null;
    }
    public object getRefWhatever()
    {
        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
        if (jsObj == IntPtr.Zero) 
            return null;
        jsval val = new jsval(); val.asBits = 0;
        JSApi.JSh_GetUCProperty(cx, jsObj, "Value", 5, ref val);

        if (val.asBits == 0)
            return null;
        
        if (JSApi.JSh_JsvalIsNullOrUndefined(ref val))
            return null;
        else if (JSApi.JSh_JsvalIsBool(ref val))
            return JSApi.JSh_GetJsvalBool(ref val);
        else if (JSApi.JSh_JsvalIsInt32(ref val))
            return JSApi.JSh_GetJsvalInt(ref val);
        else if (JSApi.JSh_JsvalIsDouble(ref val))
            return JSApi.JSh_GetJsvalDouble(ref val);
        else if (JSApi.JSh_JsvalIsString(ref val))
            return JSApi.JSh_GetJsvalStringS(cx, ref val);
        else if (JSApi.JSh_JsvalIsObject(ref val))
        {
            jsObj = JSApi.JSh_GetJsvalObject(ref val);
            object csObj = JSMgr.getCSObj(jsObj);
            if (csObj is JSValueWrap.Wrap)
                return ((JSValueWrap.Wrap)csObj).obj;
            else
                return csObj;
        }
        return null;
    }

    public void returnBool(bool v) { JSApi.JSh_SetRvalBool(cx, vp, v); }
    public void returnString(String v) { JSApi.JSh_SetRvalString(cx, vp, v); }
    public void returnChar(Char v) { JSApi.JSh_SetRvalInt(cx, vp, v); }
    public void returnByte(Byte v) { JSApi.JSh_SetRvalInt(cx, vp, v); }
    public void returnSByte(SByte v) { JSApi.JSh_SetRvalInt(cx, vp, v); }
    public void returnUInt16(UInt16 v) { JSApi.JSh_SetRvalInt(cx, vp, v); }
    public void returnInt16(Int16 v) { JSApi.JSh_SetRvalInt(cx, vp, v); }
    public void returnUInt32(UInt32 v) { JSApi.JSh_SetRvalInt(cx, vp, (Int32)v); }
    public void returnInt32(Int32 v) { JSApi.JSh_SetRvalInt(cx, vp, v); }
    public void returnUInt64(UInt64 v) { JSApi.JSh_SetRvalInt(cx, vp, (Int32)v); }
    public void returnInt64(Int64 v)
    {
        JSApi.JSh_SetRvalInt(cx, vp, (Int32)v);
    }
    public void returnEnum(Int32 v) { JSApi.JSh_SetRvalInt(cx, vp, v); }
    public void returnFloat(Single v) { JSApi.JSh_SetRvalDouble(cx, vp, v); }
    public void returnDouble(Double v) { JSApi.JSh_SetRvalDouble(cx, vp, v); }
    public void returnObject(string className, object csObj)
    {
        JSApi.JSh_SetJsvalUndefined(ref this.valReturn);
        if (csObj == null)
        {
            JSApi.JSh_SetRvalJSVAL(cx, vp, ref this.valReturn);
            return;
        }

        if (!csObj.GetType().IsArray)
        {
            IntPtr jsObj;
            //jsObj = JSMgr.getJSObj(csObj);
            //if (jsObj == IntPtr.Zero)
            { // always add a new jsObj
                jsObj = JSApi.JSh_NewObjectAsClass(cx, JSMgr.glob, /*className*/csObj.GetType().Name, JSMgr.mjsFinalizer);
                if (jsObj == IntPtr.Zero)
                    jsObj = JSApi.JSh_NewObjectAsClass(cx, JSMgr.glob, /*className*/csObj.GetType().BaseType.Name, JSMgr.mjsFinalizer);
                if (jsObj != IntPtr.Zero)
                    JSMgr.addJSCSRelation(jsObj, csObj);
            }

            if (jsObj == IntPtr.Zero)
                JSApi.JSh_SetJsvalUndefined(ref this.valReturn);
            else
                JSApi.JSh_SetJsvalObject(ref this.valReturn, jsObj);
        }
        else
        {
            Array arr = csObj as Array;
            if (arr.Length > 0 && arr.GetValue(0).GetType().IsArray)
            {
                Debug.LogWarning("cs return [][] may cause problems.");
            }

            IntPtr jsArr = JSApi.JSh_NewArrayObjectS(cx, arr.Length);

            for (int i = 0; i < arr.Length; i++)
            {
                jsval subVal = CSObject_2_JSValue(arr.GetValue(i));
                JSApi.JSh_SetElement(cx, jsArr, (uint)i, ref subVal);
            }
            JSApi.JSh_SetJsvalObject(ref this.valReturn, jsArr);
        }

        JSApi.JSh_SetRvalJSVAL(cx, vp, ref this.valReturn);
    }

    /*
     * ExtractCSParams
     *
     * extract some info to use latter
     * write into m_ParamInfo and lstCSParam
     * ONLY for Reflection
     */
    public void ExtractCSParams()
    {
        if (m_ParamInfo == null)
            m_ParamInfo = m_Method.GetParameters();

        arrCSParamsLength = m_ParamInfo.Length;
        for (int i = 0; i < m_ParamInfo.Length; i++)
        {
            ParameterInfo p = m_ParamInfo[i];

            CSParam csParam = arrCSParam[i];
            csParam.isOptional = p.IsOptional;
            csParam.isRef = p.ParameterType.IsByRef;
            csParam.isArray = p.ParameterType.IsArray;
            csParam.type = p.ParameterType;
        }
    }

    /*
     * ExtractJSParams
     * 
     * write into lstJSParam
     * 
     * RETURN
     * false -- fail
     * true  -- success
     * 
     * for primitive, enum, string: not handled
     */
    public bool ExtractJSParams(int start, int count)
    {
        arrJSParamsLength = 0;
        for (int i = 0; i < count; i++)
        {
            int index = i + start;
            bool bUndefined = JSApi.JSh_ArgvIsUndefined(cx, vp, index);
            if (bUndefined)
                return true;

            JSParam jsParam = arrJSParam[arrJSParamsLength++]; //new JSParam();
            jsParam.index = index;
            jsParam.isNull = JSApi.JSh_ArgvIsNull(cx, vp, index);
            jsParam.isArray = false;
            jsParam.csObj = null;

            IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, index);
            if (jsObj == IntPtr.Zero)
            {
                jsParam.csObj = null;
            }
            //             else if (false/*JSApi.JSh_IsArrayObject(cx, jsObj)*/)
            //             {
            //                 jsParam.isArray = true;
            //                 Debug.LogError("parse js array to cs is not supported");
            //             }
            else
            {
                object csObj = JSMgr.getCSObj(jsObj);
                if (csObj == null)
                {
                    Debug.Log("ExtractJSParams: CSObject is not found");
                    return false;
                }
                jsParam.csObj = csObj;
            }
            //lstJSParam.Add(jsParam);
        }
        return true;
    }

    // not to involved with ParamterInfo
    // use lstJSParam
    // to determine wether param match or not
    public bool IsParamMatch(int csParamIndex, bool csIsOptional, Type csType)
    {
        if (csParamIndex < arrJSParamsLength)
        {
            if (csType == typeof(object))
            {
                return true;
            }
            else if (csType.IsArray)
            {
                // todo
                // overloaded functions only matchs wether it's array or not
                //                 if (!arrJSParam[csParamIndex].isArray)
                //                     return false;
                return true;
            }
            else if (arrJSParam[csParamIndex].isWrap)
            {
                Type jsType = arrJSParam[csParamIndex].wrappedObj.GetType();

                if (!csType.IsByRef)
                {
                    if (csType != jsType && !csType.IsAssignableFrom(jsType))
                    {
                        return false;
                    }
                }
                else
                {
                    Type csElementType = csType.GetElementType();
                    if (csElementType != jsType && !csElementType.IsAssignableFrom(jsType))
                    {
                        return false;
                    }
                }
            }
            else// if (!arrJSParam[csParamIndex].isWrap)
            {
                if (arrJSParam[csParamIndex].csObj != null)
                {
                    Type jsType = arrJSParam[csParamIndex].csObj.GetType();
                    if (!csType.IsByRef)
                    {
                        if (csType != jsType && !csType.IsAssignableFrom(jsType))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Type csElementType = csType.GetElementType();
                        if (csElementType != jsType && !csElementType.IsAssignableFrom(jsType))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (csType == typeof(bool))
                        return JSApi.JSh_ArgvIsBool(cx, vp, arrJSParam[csParamIndex].index);
                    else if (csType == typeof(string))
                        return JSApi.JSh_ArgvIsString(cx, vp, arrJSParam[csParamIndex].index);
                    else if (csType.IsEnum || csType.IsPrimitive)
                        return JSApi.JSh_ArgvIsNumber(cx, vp, arrJSParam[csParamIndex].index);
                    else if (csType.IsArray)
                        return true;
                    else
                        return false;
                }
            }
        }
        else
        {
            if (!csIsOptional)
                return false;
        }
        return true;
    }

    /*
     * MatchOverloadedMethod
     * 
     * write into this.method and this.ps
     *
     * ONLY for reflection
     */
    public int MatchOverloadedMethod(MethodBase[] methods, int methodIndex)
    {
        for (int i = methodIndex; i < methods.Length; i++)
        {
            MethodBase method = methods[i];
            if (method.Name != methods[methodIndex].Name)
                return -1;
            ParameterInfo[] ps = method.GetParameters();
            if (arrJSParamsLength > ps.Length)
                continue;

            bool matchSuccess = true;
            for (int j = 0; j < ps.Length; j++)
            {
                ParameterInfo p = ps[j];

                if (!IsParamMatch(j, p.IsOptional, p.ParameterType))
                {
                    matchSuccess = false;
                    break;
                }
            }

            if (matchSuccess)
            {
                // yes, this method is what we want
                this.m_Method = method;
                this.m_ParamInfo = ps;
                return i;
            }
        }
        return -1;
    }

    /*
     * directly-call scheme use this function
     */
    public bool IsMethodMatch(CSParam[] arrCSParam)
    {
        for (int i = 0; i < arrCSParam.Length; i++)
        {
            if (!IsParamMatch(i, arrCSParam[i].isOptional, arrCSParam[i].type))
                return false;
        }
        return true;
    }

    // used for js->cs array
    // 
    public object JSValue_2_CSObject(Type t, ref jsval val)
    {
        if (t.IsArray)
        {
            Debug.LogError("JSValue_2_CSObject: could not pass an array");
            return null;
        }

        if (t.IsByRef)
            t = t.GetElementType();

        if (t == typeof(string))
            return JSApi.JSh_GetJsvalString(cx, ref val);
        else if (t.IsEnum)
            return JSApi.JSh_GetJsvalInt(ref val);
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                return JSApi.JSh_GetJsvalBool(ref val);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                return JSApi.JSh_GetJsvalInt(ref val);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                return JSApi.JSh_GetJsvalDouble(ref val);
            }
            else
            {
                Debug.Log("ConvertJSValue2CSValue: Unknown primitive type: " + t.ToString());
            }
        }
        //         else if (t.IsValueType)
        //         {
        // 
        //         }
        else// if (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsValueType)
        {
            IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref val);
            if (jsObj == IntPtr.Zero)
                return null;

            object csObject = JSMgr.getCSObj(jsObj);
            return csObject;
        }
        //         else
        //         {
        //             Debug.Log("ConvertJSValue2CSValue: Unknown CS type: " + t.ToString());
        //         }
        return null;
    }

    // index means 
    // lstJSParam[index]
    // lstCSParam[index]
    // ps[index]
    // for calling property/field
    public object JSValue_2_CSObject(Type t, int paramIndex)
    {
        if (t.IsArray)
        {
            Debug.LogError("JSValue_2_CSObject: could not pass an array");
            return null;
        }

        if (t.IsByRef)
            t = t.GetElementType();

        if (t == typeof(string))
            return JSApi.JSh_ArgvStringS(cx, vp, paramIndex);
        else if (t.IsEnum)
            return JSApi.JSh_ArgvInt(cx, vp, paramIndex);
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                return JSApi.JSh_ArgvBool(cx, vp, paramIndex);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                return JSApi.JSh_ArgvInt(cx, vp, paramIndex);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                return JSApi.JSh_ArgvDouble(cx, vp, paramIndex);
            }
            else
            {
                Debug.Log("ConvertJSValue2CSValue: Unknown primitive type: " + t.ToString());
            }
        }
        //         else if (t.IsValueType)
        //         {
        // 
        //         }
        else// if (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsValueType)
        {
            if (JSApi.JSh_ArgvIsNull(cx, vp, paramIndex))
                return null;

            IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, paramIndex);
            if (jsObj == IntPtr.Zero)
                return null;

            object csObject = JSMgr.getCSObj(jsObj);
            return csObject;
        }
        //         else
        //         {
        //             Debug.Log("ConvertJSValue2CSValue: Unknown CS type: " + t.ToString());
        //         }
        return null;
    }

    // index means 
    // lstJSParam[index]
    // lstCSParam[index]
    // ps[index]
    // for calling method
    public object JSValue_2_CSObject(int index)
    {
        JSParam jsParam = arrJSParam[index];
        int paramIndex = jsParam.index;
        CSParam csParam = arrCSParam[index];
        //ParameterInfo p = m_ParamInfo[index];

        Type t = csParam.type;

        if (csParam.isRef)
            t = t.GetElementType();

        if (jsParam.isNull) return null;
        else if (jsParam.isWrap) return jsParam.wrappedObj;
        else if (jsParam.csObj != null) return jsParam.csObj;

        //         if (typeof(UnityEngine.Object).IsAssignableFrom(t))
        //         {
        //             if (jsParam.isNull)
        //                 return null;
        // 
        //             if (jsParam.isWrap)
        //                 return jsParam.wrappedObj;
        // 
        //             return jsParam.csObj;
        //         }

        return JSValue_2_CSObject(csParam.type, paramIndex);
    }

    /*
     * BuildMethodArgs
     * 
     * RETURN
     * null -- fail
     * not null -- success
     */
    public bool BuildMethodArgs(bool addDefaultValue)
    {
        //ArrayList args = new ArrayList();
        callParamsLength = 0;
        for (int i = 0; i < this.arrCSParamsLength; i++)
        {
            callParamsLength++;

            if (i < this.arrJSParamsLength)
            {
                JSParam jsParam = arrJSParam[i];
                if (jsParam.isWrap)
                {
                    //args.Add(jsParam.wrappedObj);
                    callParams[i] = jsParam.wrappedObj;
                }
                else if (jsParam.isArray)
                {
                    // todo
                    // 
                    Debug.Log("array parameter not supported");
                    callParams[i] = null;
                }
                else if (jsParam.isNull)
                {
                    //args.Add(null);
                    callParams[i] = null;
                }
                else
                {
                    //args.Add(JSValue_2_CSObject(i));
                    callParams[i] = JSValue_2_CSObject(i);
                }
            }
            else
            {
                if (arrCSParam[i].isOptional)
                {
                    if (addDefaultValue)//args.Add(arrCSParam[i].defaultValue);
                        callParams[i] = arrCSParam[i].defaultValue;
                    else
                        break;
                }
                else
                {
                    Debug.LogError("Not enough arguments calling function '" + m_Method.Name + "'");
                    return false;
                }
            }
        }
        //return args.ToArray();
        return true;
    }

    // CS -> JS
    public jsval CSObject_2_JSValue(object csObj)
    {
        jsval val = new jsval();
        JSApi.JSh_SetJsvalUndefined(ref val);

        if (csObj == null)
        {
            return val;
        }

        Type t = csObj.GetType();
        if (t == typeof(void))
        {
            return val;
        }
        else if (t == typeof(string))
        {
            JSApi.JSh_SetJsvalString(cx, ref val, (string)csObj);
        }
        else if (t.IsEnum)
        {
            JSApi.JSh_SetJsvalInt(ref val, (int)csObj);
        }
        else if (t.IsPrimitive)
        {
            if (t == typeof(System.Boolean))
            {
                JSApi.JSh_SetJsvalBool(ref val, (bool)csObj);
            }
            else if (t == typeof(System.UInt32) || t == typeof(System.Int32))
            {
                int v = (Int32)csObj;
                JSApi.JSh_SetJsvalInt(ref val, v);
            }
            else if (t == typeof(System.Char) ||
                t == typeof(System.Byte) || t == typeof(System.SByte) ||
                t == typeof(System.UInt16) || t == typeof(System.Int16) ||
                //t == typeof(System.UInt32) || t == typeof(System.Int32) ||
                t == typeof(System.UInt64) || t == typeof(System.Int64))
            {
                JSApi.JSh_SetJsvalInt(ref val, (Int32)(Int64)csObj);
            }
            else if (t == typeof(System.Single) || t == typeof(System.Double))
            {
                JSApi.JSh_SetJsvalDouble(ref val, (double)csObj);
            }
            else
            {
                Debug.Log("CS -> JS: Unknown primitive type: " + t.ToString());
            }
        }
        //         else if (t.IsValueType)
        //         {
        // 
        //         }
        else if (t.IsArray)
        {
            Debug.LogError("CSObject_2_JSValue: Cannot handle array here");

            // todo
            // return [][] may cause problems
            //             Array arr = csObj as Array;
            //             if (arr.Length > 0 && arr.GetValue(0).GetType().IsArray)
            //             {
            //                 Debug.LogWarning("cs return [][] may cause problems.");
            //             }
            // 
            //             IntPtr jsArr = JSApi.JSh_NewArrayObjectS(cx, arr.Length);
            //             
            //             for (int i = 0; i < arr.Length; i++)
            //             {
            //                 jsval subVal = CSObject_2_JSValue(arr.GetValue(i));
            //                 JSApi.JSh_SetElement(cx, jsArr, (uint)i, ref subVal);
            //             }
            //             JSApi.JSh_SetJsvalObject(ref val, jsArr);
        }
        else// if (typeof(UnityEngine.Object).IsAssignableFrom(t) || t.IsClass || t.IsValueType)
        {
            IntPtr jsObj;
            //             jsObj = JSMgr.getJSObj(csObj);
            //             if (jsObj == IntPtr.Zero)
            {
                // always add a new jsObj
                jsObj = JSApi.JSh_NewObjectAsClass(cx, JSMgr.glob, t.Name, JSMgr.mjsFinalizer);
                if (jsObj != IntPtr.Zero)
                    JSMgr.addJSCSRelation(jsObj, csObj);
            }
            if (jsObj == IntPtr.Zero)
                JSApi.JSh_SetJsvalUndefined(ref val);
            else
                JSApi.JSh_SetJsvalObject(ref val, jsObj);
        }
        //         else
        //         {
        //             Debug.Log("CS -> JS: Unknown CS type: " + t.ToString());
        //             JSApi.JSh_SetJsvalUndefined(ref val);
        //         }
        return val;
    }

    public void PushResult(object csObj)
    {
        if (/*this.op == Oper.METHOD && */ arrCSParam != null)
        {
            // handle ref/out parameters
            for (int i = 0; i < arrCSParamsLength; i++)
            {
                if (arrCSParam[i].isRef)
                {
                    arrJSParam[i].wrappedObj = callParams[i];
                }
            }
        }

        jsval val = CSObject_2_JSValue(csObj);
        JSApi.JSh_SetRvalJSVAL(cx, vp, ref val);
    }

    public jsval rvalCallJS = new jsval();
    // better not use this function, use CallJSFunctionValue instead
    public bool CallJSFunction(IntPtr jsThis, IntPtr/* JSFunction* */ jsFunction, params object[] args)
    {
        if (args == null || args.Length == 0)
        {
            return JSApi.JSh_CallFunction(JSMgr.cx, jsThis, jsFunction, 0, null/*IntPtr.Zero*/, ref rvalCallJS);
        }

        jsval[] vals = new jsval[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            vals[i] = CSObject_2_JSValue(args[i]);
        }

        return JSApi.JSh_CallFunction(JSMgr.cx, jsThis, jsFunction, (UInt32)args.Length, vals, ref rvalCallJS);
    }
//     public bool CallJSFunctionValue(IntPtr jsThis, ref jsval valFunction, params object[] args)
//     {
//         if (args == null || args.Length == 0)
//         {
//             return JSApi.JSh_CallFunctionValue(JSMgr.cx, jsThis, ref valFunction, 0, null/*IntPtr.Zero*/, ref rvalCallJS);
//         }
// 
//         jsval[] vals = new jsval[args.Length];
//         for (int i = 0; i < args.Length; i++)
//         {
//             vals[i] = CSObject_2_JSValue(args[i]);
//         }
// 
//         return JSApi.JSh_CallFunctionValue(JSMgr.cx, jsThis, ref valFunction, (UInt32)args.Length, vals, ref rvalCallJS);
//     }

    public jsval[] arrJsval0 = new jsval[0];
    public bool CallJSFunctionValue(IntPtr jsThis, ref jsval valFunction, params object[] args)
    {
        if (JSMgr.isShutDown) return false;

        int argsLen = (args != null ? args.Length : 0);

        if (JSEngine.inst.OutputFullCallingStackOnError)
        {
            jsval[] vals = new jsval[argsLen + 2];

            JSApi.JSh_SetJsvalObject(ref vals[0], jsThis);
            vals[1] = valFunction;

            if (argsLen > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    vals[i + 2] = CSObject_2_JSValue(args[i]);
                }
            }

            return JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.CSOBJ, "jsFunctionEntry", (UInt32)(argsLen + 2), vals, ref rvalCallJS);
        }
        else
        {
            if (argsLen == 0)
            {
                return JSApi.JSh_CallFunctionValue(JSMgr.cx, jsThis, ref valFunction, 0, arrJsval0, ref rvalCallJS);
            }

            jsval[] vals = new jsval[argsLen];
            for (int i = 0; i < argsLen; i++)
            {
                vals[i] = CSObject_2_JSValue(args[i]);
            }

            return JSApi.JSh_CallFunctionValue(JSMgr.cx, jsThis, ref valFunction, (UInt32)argsLen, vals, ref rvalCallJS);
        }
    }

    public enum Oper
    {
        GET_FIELD = 0,
        SET_FIELD = 1,
        GET_PROPERTY = 2,
        SET_PROPERTY = 3,
        METHOD = 4,
        CONSTRUCTOR = 5,
    }

    public bool bGet = false, bStatic = false;
    public IntPtr jsObj = IntPtr.Zero;
    public object csObj, result, arg;
    public object[] args;
    public int currentParamCount = 0;
    public Oper op;
    //     public static bool CSCallback()
    //     {
    //         if (JSVCall.bGet)
    //             result = ((GameObject)JSVCall.jsObj).activeSelf;
    //         else
    //         {
    //             object arg = JSValue_2_CSObject(typeof(bool), JSVCall.currentParamCount);
    //             ((GameObject)JSVCall.jsObj).activeSelf = (bool)JSVCall.arg;
    //         }
    //     }

    public int CallCallback(IntPtr cx, uint argc, IntPtr vp)
    {
        this.Reset(cx, vp);

        // first 4 params are fixed
        this.op = (Oper)JSApi.JSh_ArgvInt(cx, vp, 0);
        int slot = JSApi.JSh_ArgvInt(cx, vp, 1);
        int index = JSApi.JSh_ArgvInt(cx, vp, 2);
        bool isStatic = JSApi.JSh_ArgvBool(cx, vp, 3);

        if (slot < 0 || slot >= JSMgr.allCallbackInfo.Count)
        {
            Debug.LogError("Bad slot: " + slot);
            return JSApi.JS_FALSE;
        }
        JSMgr.CallbackInfo aInfo = JSMgr.allCallbackInfo[slot];

        currentParamCount = 4;
        if (!isStatic)
        {
            this.jsObj = JSApi.JSh_ArgvObject(cx, vp, 4);
            if (this.jsObj == IntPtr.Zero)
                return JSApi.JS_FALSE;

            this.csObj = JSMgr.getCSObj(jsObj);
            if (this.csObj == null)
                return JSApi.JS_FALSE;

            currentParamCount++;
        }

        switch (op)
        {
            case Oper.GET_FIELD:
            case Oper.SET_FIELD:
                {
                    currIndex = currentParamCount;
                    this.bGet = (op == Oper.GET_FIELD);
                    JSMgr.CSCallbackField fun = aInfo.fields[index];
                    if (fun == null) return JSApi.JS_FALSE;
                    fun(this);
                }
                break;
            case Oper.GET_PROPERTY:
            case Oper.SET_PROPERTY:
                {
                    currIndex = currentParamCount;
                    this.bGet = (op == Oper.GET_PROPERTY);
                    JSMgr.CSCallbackProperty fun = aInfo.properties[index];
                    if (fun == null) return JSApi.JS_FALSE;
                    fun(this);
                }
                break;
            case Oper.METHOD:
            case Oper.CONSTRUCTOR:
                {
                    bool overloaded = JSApi.JSh_ArgvBool(cx, vp, currentParamCount);
                    currentParamCount++;

                    JSMgr.MethodCallBackInfo[] arrMethod;
                    if (op == Oper.METHOD)
                        arrMethod = aInfo.methods;
                    else
                        arrMethod = aInfo.constructors;

                    // params count
                    // for overloaded function, it's caculated by ExtractJSParams
                    int jsParamCount = (int)argc - currentParamCount;
                    if (!overloaded)
                    {
                        // for not-overloaded function
                        int i = (int)argc;
                        while (i > 0 && JSApi.JSh_ArgvIsUndefined(cx, vp, --i))
                            jsParamCount--;
                    }
                    else
                    {
                        if (!this.ExtractJSParams(currentParamCount, (int)argc - currentParamCount))
                            return JSApi.JS_FALSE;

                        string methodName = arrMethod[index].methodName;

                        int i = index;
                        while (true)
                        {
                            if (IsMethodMatch(arrMethod[i].arrCSParam))
                            {
                                index = i;
                                break;
                            }
                            i++;
                            if (arrMethod[i].methodName != methodName)
                            {
                                Debug.LogError("Overloaded function can't find match: " + methodName);
                                return JSApi.JS_FALSE;
                            }
                        }

                        jsParamCount = arrJSParamsLength;
                    }

                    currIndex = currentParamCount;
                    arrMethod[index].fun(this, currentParamCount, jsParamCount);
                    //Debug.Log(slot.ToString()+"/"+index.ToString()+"Call OK");

                    //                     if (overloaded)
                    //                     {
                    //                         string methodName = arrMethod[index].methodName;
                    // 
                    //                         int i = index;
                    //                         while (true)
                    //                         {
                    //                             if (IsMethodMatch(arrMethod[i].arrCSParam))
                    //                             {
                    //                                 index = i;
                    //                                 break;
                    //                             }
                    //                             i++;
                    //                             if (arrMethod[i].methodName != methodName)
                    //                             {
                    //                                 Debug.LogError("Overloaded function can't find match: " + methodName);
                    //                                 return JSApi.JS_FALSE;
                    //                             }
                    //                         }
                    //                     }
                    // 
                    //                     JSMgr.CSCallbackMethod fun;
                    //                     
                    //                     fun = arrMethod[index].fun;
                    //                     arrCSParam = arrMethod[index].arrCSParam;
                    //                     arrCSParamsLength = arrCSParam.Length;
                    // 
                    //                     if (fun == null || arrCSParam == null) 
                    //                         return JSApi.JS_FALSE;
                    // 
                    //                     if (!BuildMethodArgs(false))
                    //                          return JSApi.JS_FALSE;
                    //                     
                    //                     if (!fun(this, currentParamCount, (int)argc - currentParamCount))
                    //                         return JSApi.JS_FALSE;
                }
                break;
        }

        //         this.PushResult(result);
        //         return JSApi.JS_TRUE;
        return JSApi.JS_TRUE;
    }

    public int CallReflection(IntPtr cx, uint argc, IntPtr vp)
    {
        this.Reset(cx, vp);

        this.op = (Oper)JSApi.JSh_ArgvInt(cx, vp, 0);
        int slot = JSApi.JSh_ArgvInt(cx, vp, 1);
        int index = JSApi.JSh_ArgvInt(cx, vp, 2);
        bool isStatic = JSApi.JSh_ArgvBool(cx, vp, 3);

        if (slot < 0 || slot >= JSMgr.allTypeInfo.Count)
        {
            Debug.LogError("Bad slot: " + slot);
            return JSApi.JS_FALSE;
        }
        JSMgr.ATypeInfo aInfo = JSMgr.allTypeInfo[slot];

        currentParamCount = 4;
        object csObj = null;
        if (!isStatic)
        {
            IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, 4);
            if (jsObj == IntPtr.Zero)
                return JSApi.JS_FALSE;

            csObj = JSMgr.getCSObj(jsObj);
            if (csObj == null)
                return JSApi.JS_FALSE;

            currentParamCount++;
        }

        //object result = null;

        switch (op)
        {
            case Oper.GET_FIELD:
                {
                    result = aInfo.fields[index].GetValue(csObj);
                }
                break;
            case Oper.SET_FIELD:
                {
                    FieldInfo field = aInfo.fields[index];
                    field.SetValue(csObj, JSValue_2_CSObject(field.FieldType, currentParamCount));
                }
                break;
            case Oper.GET_PROPERTY:
                {
                    result = aInfo.properties[index].GetValue(csObj, null);
                }
                break;
            case Oper.SET_PROPERTY:
                {
                    PropertyInfo property = aInfo.properties[index];
                    property.SetValue(csObj, JSValue_2_CSObject(property.PropertyType, currentParamCount), null);
                }
                break;
            case Oper.METHOD:
            case Oper.CONSTRUCTOR:
                {
                    bool overloaded = JSApi.JSh_ArgvBool(cx, vp, currentParamCount);
                    currentParamCount++;

                    if (!this.ExtractJSParams(currentParamCount, (int)argc - currentParamCount))
                        return JSApi.JS_FALSE;

                    if (overloaded)
                    {
                        MethodBase[] methods = aInfo.methods;
                        if (op == Oper.CONSTRUCTOR)
                            methods = aInfo.constructors;

                        if (-1 == MatchOverloadedMethod(methods, index))
                            return JSApi.JS_FALSE;
                    }
                    else
                    {
                        m_Method = aInfo.methods[index];
                        if (op == Oper.CONSTRUCTOR)
                            m_Method = aInfo.constructors[index];
                    }

                    this.ExtractCSParams();

                    //!!!!!!!!!!!!!!!!!!!!
                    if (!BuildMethodArgs(true))
                        return JSApi.JS_FALSE;

                    object[] cp = new object[callParamsLength];
                    for (int i = 0; callParamsLength > i; i++)
                        cp[i] = callParams[i];

                    result = this.m_Method.Invoke(csObj, cp);
                }
                break;
        }

        this.PushResult(result);
        return JSApi.JS_TRUE;
    }
}