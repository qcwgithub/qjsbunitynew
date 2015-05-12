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
    public JSDataExchangeMgr datax;
    public JSVCall()
    {
        datax = new JSDataExchangeMgr(this);
    }

    public enum Consts
    {
        MaxParams = 16,
    }
    public class JSParam
    {
        public int index; // param index
        public object csObj; // coresponding cs object, for primitive, string, enum, it's null
        //public bool isWrap { get { return csObj != null && csObj is JSValueWrap.Wrap; } }
        //public object wrappedObj { get { return ((JSValueWrap.Wrap)csObj).obj; } set { ((JSValueWrap.Wrap)csObj).obj = value; } }
        public bool isArray;
        public bool isNull;
        public jsval valRO; // if valRO.asBits > 0, means this JS param is {Value: XXX}
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

    // TODO delete
    ////////////////////////////////
    public IntPtr cx;
    public IntPtr vp;
    public int currIndex = 0;
    public jsval valReturn = new jsval();
    public jsval valTemp = new jsval();
    ////////////////////////////////


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

        // TODO
        currIndex = 0;
    }
    //
    //
    // get parameters
    //
    //    public Boolean getBool() { return JSApi.JSh_ArgvBool(cx, vp, currIndex++); }
    //    public String getString() { return JSApi.JSh_ArgvStringS(cx, vp, currIndex++); }
    //    public Char getChar() { return (Char)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public Byte getByte() { return (Byte)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public SByte getSByte() { return (SByte)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public UInt16 getUInt16() { return (UInt16)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public Int16 getInt16() { return (Int16)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public UInt32 getUInt32() { return (UInt32)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public Int32 getInt32() { return (Int32)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public UInt64 getUInt64() { return (UInt64)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public Int64 getInt64() { return (Int64)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public Int32 getEnum() { return (Int32)JSApi.JSh_ArgvInt(cx, vp, currIndex++); }
    //    public Single getFloat()
    //    {
    //        // js has only int32 and double, so...
    //        int i = currIndex++;
    //        if (JSApi.JSh_ArgvIsDouble(cx, vp, i))
    //            return (Single)JSApi.JSh_ArgvDouble(cx, vp, i);
    //        else
    //            return (Single)JSApi.JSh_ArgvInt(cx, vp, i);
    //    }
    //   public Double getDouble()
    //   {
    //       // js has only int32 and double, so...
    //       int i = currIndex++;
    //       if (JSApi.JSh_ArgvIsDouble(cx, vp, i))
    //           return (Double)JSApi.JSh_ArgvDouble(cx, vp, i);
    //       else
    //           return (Double)JSApi.JSh_ArgvInt(cx, vp, i);
    //   }
    ////     public JSValueWrap.Wrap getWrap()
    ////     {
    ////         IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
    ////         object csObj = JSMgr.getCSObj(jsObj);
    ////         return (JSValueWrap.Wrap)csObj;
    ////     }
    //   
    ////     public object getObject(Type typeParam = null)
    ////     {
    ////         IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
    ////         if (jsObj == IntPtr.Zero)
    ////             return null;
// 
//         object csObj = JSMgr.getCSObj(jsObj);
//         if (csObj is JSValueWrap.Wrap)
//             return ((JSValueWrap.Wrap)csObj).obj;
//         else if (csObj != null)
//             return csObj;
// 
//         if (!JSApi.JSh_IsArrayObject(cx, jsObj))
//             return null;
// 
// 
//         // array params don't work.
//         // code must be generated, cann't be dynamically run.
//         // because type is unknown during run-time.
//         Type typeElement = typeParam.GetElementType();
//         jsval valElement = new jsval();
// 
//         int length = JSApi.JSh_GetArrayLength(cx, jsObj);
//         object[] arr = new object[length];
//         for (int i = 0; i < length; i++)
//         {
//             JSApi.JSh_GetElement(cx, jsObj, (uint)i, ref valElement);
//             object csObjElement = JSValue_2_CSObject(typeElement, ref valElement);
//             arr[i] = csObjElement;
//         }
//         return arr;
//     }

    //
    // get returnvalue from js
    //
//    public Boolean getRtBool() { return JSApi.JSh_GetJsvalBool(ref rvalCallJS); }
//    public String getRtString() { return JSApi.JSh_GetJsvalStringS(cx, ref rvalCallJS); }
//    public Char getRtChar() { return (Char)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public Byte getRtByte() { return (Byte)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public SByte getRtSByte() { return (SByte)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public UInt16 getRtUInt16() { return (UInt16)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public Int16 getRtInt16() { return (Int16)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public UInt32 getRtUInt32() { return (UInt32)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public Int32 getRtInt32() { return (Int32)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public UInt64 getRtUInt64() { return (UInt64)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public Int64 getRtInt64() { return (Int64)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public Int32 getRtEnum() { return (Int32)JSApi.JSh_GetJsvalInt(ref rvalCallJS); }
//    public Single getRtFloat()
//    {
//        if (JSApi.JSh_JsvalIsDouble(ref rvalCallJS))
//            return (Single)JSApi.JSh_GetJsvalDouble(ref rvalCallJS);
//        else
//            return (Single)JSApi.JSh_GetJsvalInt(ref rvalCallJS);
//    }
//    public Double getRtDouble()
//    {
//        // js has only int32 and double, so...
//        if (JSApi.JSh_JsvalIsDouble(ref rvalCallJS))
//            return (Double)JSApi.JSh_GetJsvalDouble(ref rvalCallJS);
//        else
//            return (Double)JSApi.JSh_GetJsvalInt(ref rvalCallJS);
//    }/*
//    public JSValueWrap.Wrap getWrap()
//    {
    //        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
    //        object csObj = JSMgr.getCSObj(jsObj);
    //        return (JSValueWrap.Wrap)csObj;
    //    }*/
    ////     public object getRtObject(Type typeParam = null)
    ////     {
    ////         IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref rvalCallJS);
    ////         if (jsObj == IntPtr.Zero)
//             return null;
// 
//         object csObj = JSMgr.getCSObj(jsObj);
//         if (csObj is JSValueWrap.Wrap)
//             return ((JSValueWrap.Wrap)csObj).obj;
//         else if (csObj != null)
//             return csObj;
// 
//         if (!JSApi.JSh_IsArrayObject(cx, jsObj))
//             return null;
// 
//         if (typeParam == null)
//             return null;
// 
// 
//         // Create array of typeElement
//         Type typeElement = typeParam.GetElementType();
//         jsval valElement = new jsval();
// 
//         int length = JSApi.JSh_GetArrayLength(cx, jsObj);        
//         Array arr = Array.CreateInstance(typeElement, length);
//         for (int i = 0; i < length; i++)
//         {
//             JSApi.JSh_GetElement(cx, jsObj, (uint)i, ref valElement);
//             object csObjElement = JSValue_2_CSObject(typeElement, ref valElement);
//             arr.SetValue(csObjElement, i);
//         }
//         return arr;
//     }

    //    public struct stJSCS
    //    {
    //        public IntPtr jsObj;
    //        public object csObj;
    //        public stJSCS(IntPtr j, object c) { jsObj = j; csObj = c; }
    //    }
    //    // only for parameters
    //    //     public stJSCS getValueTypeObject()
    //     {
    //         IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
    //         object csObj = JSMgr.getCSObj(jsObj);
    //         return new stJSCS(jsObj, csObj);
    //     }
    //    public stJSCS getJSCSObject()
    //    {
    //        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, currIndex++);
    //        object csObj = JSMgr.getCSObj(jsObj);
    //        return new stJSCS(jsObj, csObj);
    //    }
    //    public IntPtr getJSFunction()
    //    {
    //        return JSApi.JSh_ArgvFunction(cx, vp, currIndex++);
    //    }


    // TODO
    // 改
    public int getJSFunctionValue()
    {
        int funID = JSApi.getFunction((int)JSApi.GetType.Arg);
        if (JSEngine.inst != null && JSEngine.inst.forceProtectJSFunction)
        {
            JSApi.addValueRoot(funID);
        }
        return funID;
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
//     public bool ExtractJSParams(int start, int count)
//     {
//         arrJSParamsLength = 0;
//         for (int i = 0; i < count; i++)
//         {
//             int index = i + start;
//             
//             var tag = JSApi.JSh_ArgvTag(cx, vp, index);
//             bool bUndefined = JSApi.jsval.isUndefined(tag);
//             if (bUndefined)
//                 return true;
// 
//             JSParam jsParam = arrJSParam[arrJSParamsLength++]; //new JSParam();
//             jsParam.index = index;
//             jsParam.isNull = JSApi.jsval.isNullOrUndefined(JSApi.JSh_ArgvTag(cx, vp, index));
//             jsParam.isArray = false;
//             jsParam.csObj = null;
//             jsParam.valRO.asBits = 0;
// 
//             IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, index);
//             if (jsObj == IntPtr.Zero)
//             {
//                 jsParam.csObj = null;
//             }
//             //             else if (false/*JSApi.JSh_IsArrayObject(cx, jsObj)*/)
//             //             {
//             //                 jsParam.isArray = true;
//             //                 Debug.LogError("parse js array to cs is not supported");
//             //             }
//             else
//             {
//                 object csObj = JSMgr.getCSObj(jsObj);
//                 if (csObj == null)
//                 {
//                     jsval valo = new jsval();
//                     JSApi.JSh_SetJsvalUndefined(ref valo);
//                     JSApi.GetProperty(cx, jsObj, "Value", -1, ref valo);
//                     if (!jsval.isNullOrUndefined(valo.tag))
//                     {
//                         jsParam.valRO = valo;
//                        jsObj = JSApi.JSh_GetJsvalObject(ref valo);
//                         csObj = JSMgr.getCSObj(jsObj);
//                    }
//                     else{
//                         Debug.Log("ExtractJSParams: CSObject is not found");
//                         return false;
//                     }
//                 }
//                 jsParam.csObj = csObj;
//             }
//             //lstJSParam.Add(jsParam);
//         }
//         return true;
//     }

    // index means 
    // lstJSParam[index]
    // lstCSParam[index]
    // ps[index]
    // for calling method
//     public object JSValue_2_CSObject(int index)
//     {
//         JSParam jsParam = arrJSParam[index];
//         int paramIndex = jsParam.index;
//         CSParam csParam = arrCSParam[index];
//         //ParameterInfo p = m_ParamInfo[index];
// 
//         Type t = csParam.type;
// 
//         if (csParam.isRef)
//             t = t.GetElementType();
// 
//         if (jsParam.isNull) return null;
//         else if (jsParam.isWrap) return jsParam.wrappedObj;
//         else if (jsParam.csObj != null) return jsParam.csObj;
// 
//         //         if (typeof(UnityEngine.Object).IsAssignableFrom(t))
//         //         {
//         //             if (jsParam.isNull)
//         //                 return null;
//         // 
//         //             if (jsParam.isWrap)
//         //                 return jsParam.wrappedObj;
//         // 
//         //             return jsParam.csObj;
//         //         }
// 
//         return JSValue_2_CSObject(csParam.type, paramIndex);
//     }

    /*
     * BuildMethodArgs
     * 
     * RETURN
     * null -- fail
     * not null -- success
     */
//     public bool BuildMethodArgs(bool addDefaultValue)
//     {
//         //ArrayList args = new ArrayList();
//         callParamsLength = 0;
//         for (int i = 0; i < this.arrCSParamsLength; i++)
//         {
//             callParamsLength++;
// 
//             if (i < this.arrJSParamsLength)
//             {
//                 JSParam jsParam = arrJSParam[i];
//                 if (jsParam.isWrap)
//                 {
//                     //args.Add(jsParam.wrappedObj);
//                     callParams[i] = jsParam.wrappedObj;
//                 }
//                 else if (jsParam.isArray)
//                 {
//                     // todo
//                     // 
//                     Debug.Log("array parameter not supported");
//                     callParams[i] = null;
//                 }
//                 else if (jsParam.isNull)
//                 {
//                     //args.Add(null);
//                     callParams[i] = null;
//                 }
//                 else
//                 {
//                     //args.Add(JSValue_2_CSObject(i));
//                     callParams[i] = JSValue_2_CSObject(i);
//                 }
//             }
//             else
//             {
//                 if (arrCSParam[i].isOptional)
//                 {
//                     if (addDefaultValue)//args.Add(arrCSParam[i].defaultValue);
//                         callParams[i] = arrCSParam[i].defaultValue;
//                     else
//                         break;
//                 }
//                 else
//                 {
//                     Debug.LogError("Not enough arguments calling function '" + m_Method.Name + "'");
//                     return false;
//                 }
//             }
//         }
//         //return args.ToArray();
//         return true;
//     }


//     public void PushResult(object csObj)
//     {
//         if (/*this.op == Oper.METHOD && */ arrCSParam != null)
//         {
//             // handle ref/out parameters
//             for (int i = 0; i < arrCSParamsLength; i++)
//             {
//                 if (arrCSParam[i].isRef)
//                 {
//                     arrJSParam[i].wrappedObj = callParams[i];
//                 }
//             }
//         }
// 
//         jsval val = CSObject_2_JSValue(csObj);
//         JSApi.JSh_SetRvalJSVAL(cx, vp, ref val);
//     }

    //public jsval rvalCallJS = new jsval();
    // better not use this function, use CallJSFunctionValue instead
//     public bool CallJSFunction(IntPtr jsThis, IntPtr/* JSFunction* */ jsFunction, params object[] args)
//     {
//         if (args == null || args.Length == 0)
//         {
//             return JSApi.JSh_CallFunction(JSMgr.cx, jsThis, jsFunction, 0, null/*IntPtr.Zero*/, ref rvalCallJS);
//         }
// 
//         jsval[] vals = new jsval[args.Length];
//         for (int i = 0; i < args.Length; i++)
//         {
//             vals[i] = CSObject_2_JSValue(args[i]);
//         }
// 
//         return JSApi.JSh_CallFunction(JSMgr.cx, jsThis, jsFunction, (UInt32)args.Length, vals, ref rvalCallJS);
//     }
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

    // typeName doesn't include ctor
    public int CallJSClassCtorByName(string typeName)
    {
        int jsObjID = JSApi.NewJSClassObject(typeName);
        return jsObjID;
//         JSApi.jsval valParam = new JSApi.jsval(); 
//         valParam.asBits = 0;
//         JSApi.JSh_SetJsvalString(JSMgr.cx, ref valParam, typeName);
//         JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.glob, "jsb_CallObjectCtor", 1, new JSApi.jsval[] { valParam }, ref JSMgr.vCall.rvalCallJS);
//         IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref JSMgr.vCall.rvalCallJS);
//         return jsObj;
    }

    public bool CallJSFunctionName(int jsObjID, string functionName, params object[] args)
    {
        if (JSMgr.isShutDown) return false;

        int argsLen = (args != null ? args.Length : 0);

        // TODO 把这个加回来
        if (JSEngine.inst.OutputFullCallingStackOnError)
        {
            JSApi.setObject((int)JSApi.SetType.TempVal, jsObjID);
            JSApi.moveTempVal2Arr(0);

            JSApi.setString((int)JSApi.SetType.TempVal, functionName);
            JSApi.moveTempVal2Arr(1);

            if (argsLen > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    this.datax.setWhatever(JSApi.SetType.TempVal, args[i]);
                    JSApi.moveTempVal2Arr(i + 2);
                }
            }
            // TODO 修改 ErrorHandler.javascript
            return JSApi.callFunctionName(jsObjID, functionName, argsLen);
            //return JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.CSOBJ, "jsFunctionEntry", (UInt32)(argsLen + 2), vals, ref rvalCallJS);
        }
        else
        {
            if (argsLen == 0)
            {
                return JSApi.callFunctionName(jsObjID, functionName, 0);
            }

            for (int i = 0; i < argsLen; i++)
            {
                this.datax.setWhatever(JSApi.SetType.TempVal, args[i]);
                JSApi.moveTempVal2Arr(i);
            }

            return JSApi.callFunctionName(jsObjID, functionName, argsLen);
        }
    }

    // TODO: delete
//     public jsval[] arrJsval0 = new jsval[0];
//     public bool CallJSFunctionValue(IntPtr jsThis, ref jsval valFunction, params object[] args)
//     {
//         if (JSMgr.isShutDown) return false;
// 
//         int argsLen = (args != null ? args.Length : 0);
// 
//         if (JSEngine.inst.OutputFullCallingStackOnError)
//         {
//             jsval[] vals = new jsval[argsLen + 2];
// 
//             JSApi.JSh_SetJsvalObject(ref vals[0], jsThis);
//             vals[1] = valFunction;
// 
//             if (argsLen > 0)
//             {
//                 for (int i = 0; i < args.Length; i++)
//                 {
//                     // vals[i + 2] = CSObject_2_JSValue(args[i]);
//                     this.datax.setWhatever(JSApi.SetType.TempVal, args[i]);
//                     vals[i + 2] = valTemp;
//                 }
//             }
// 
//             return JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.CSOBJ, "jsFunctionEntry", (UInt32)(argsLen + 2), vals, ref rvalCallJS);
//         }
//         else
//         {
//             if (argsLen == 0)
//            {
//                 return JSApi.JSh_CallFunctionValue(JSMgr.cx, jsThis, ref valFunction, 0, arrJsval0, ref rvalCallJS);
//            }
// 
//             jsval[] vals = new jsval[argsLen];
//             for (int i = 0; i < argsLen; i++)
//            {
//                 // vals[i] = CSObject_2_JSValue(args[i]);
//                 this.datax.setWhatever(JSDataExchangeMgr.eSetType.Jsval, args[i]);
//                 vals[i] = valTemp;
//            }
// 
//             return JSApi.JSh_CallFunctionValue(JSMgr.cx, jsThis, ref valFunction, (UInt32)argsLen, vals, ref rvalCallJS);
//         }
//     }

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
    public int jsObjID = 0;
    public object csObj;
    // TODO delete
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

    //
    // argc 还有几个参数
    //
    //
    public bool CallCallback(int iOP, int slot, int index, bool isStatic, int argc)
    {
        this.Reset(cx, vp);

        Oper op = (Oper)iOP;

        if (slot < 0 || slot >= JSMgr.allCallbackInfo.Count)
        {
            throw (new Exception("Bad slot: " + slot));
            return false;
        }
        JSMgr.CallbackInfo aInfo = JSMgr.allCallbackInfo[slot];
        if (!isStatic)
        {
            this.jsObjID = JSApi.getObject((int)JSApi.GetType.Arg);
            if (this.jsObjID == 0)
            {
                throw (new Exception("Invalid this jsObjID"));
                return false;
            }

            // for manual js code, this.csObj will be null
            this.csObj = JSMgr.getCSObj2(jsObjID);
            //if (this.csObj == null) {
            //	throw(new Exception("Invalid this csObj"));
            //    return JSApi.JS_FALSE;
            //}
        }

        switch (op)
        {
            case Oper.GET_FIELD:
            case Oper.SET_FIELD:
                {
                    this.bGet = (op == Oper.GET_FIELD);
                    JSMgr.CSCallbackField fun = aInfo.fields[index];
                    if (fun == null)
                    {
                        throw (new Exception("Field not found"));
                        return false;
                    }
                    fun(this);
                }
                break;
            case Oper.GET_PROPERTY:
            case Oper.SET_PROPERTY:
                {
                    this.bGet = (op == Oper.GET_PROPERTY);
                    JSMgr.CSCallbackProperty fun = aInfo.properties[index];
                    if (fun == null)
                    {
                        throw (new Exception("Property not found"));
                        return false;
                    }
                    fun(this);
                }
                break;
            case Oper.METHOD:
            case Oper.CONSTRUCTOR:
                {
                    // TODO dont need this param
                    bool overloaded = JSApi.getBoolean((int)JSApi.GetType.Arg);
                    overloaded = false; // force to false

                    JSMgr.MethodCallBackInfo[] arrMethod;
                    if (op == Oper.METHOD)
                        arrMethod = aInfo.methods;
                    else
                        arrMethod = aInfo.constructors;

                    currIndex = currentParamCount;
                    // TODO 改成 argc
                    arrMethod[index].fun(this, currentParamCount, argc - 1);
                }
                break;
        }
        return true;
    }

    // TODO delete
//     public int CallCallback2(IntPtr cx, uint argc, IntPtr vp)
//     {
//         this.Reset(cx, vp);
// 
//         // first 4 params are fixed
//         this.op = (Oper)JSApi.JSh_ArgvInt(cx, vp, 0);
//         int slot = JSApi.JSh_ArgvInt(cx, vp, 1);
//         int index = JSApi.JSh_ArgvInt(cx, vp, 2);
//         bool isStatic = JSApi.JSh_ArgvBool(cx, vp, 3);
// 
//         //StreamWriter sw = File.AppendText(Application.dataPath + "/VCall.txt");
//         //string nnname = string.Empty;
//         //if (op == Oper.GET_FIELD || op == Oper.SET_FIELD) nnname = JSMgr.allCallbackInfo[slot].fields[index].Method.Name;
//         //else if (op == Oper.GET_PROPERTY || op == Oper.SET_PROPERTY) nnname = JSMgr.allCallbackInfo[slot].properties[index].Method.Name;
//         //else if (op == Oper.METHOD) nnname = JSMgr.allCallbackInfo[slot].methods[index].fun.Method.Name;
//         //else if (op == Oper.CONSTRUCTOR) nnname = JSMgr.allCallbackInfo[slot].constructors[index].fun.Method.Name;
//         //sw.WriteLine(new StringBuilder().AppendFormat("{0},{1},{2},{3}  {4}", this.op, slot, index, isStatic, nnname).ToString());
//         //sw.Close();
// 
//         if (slot < 0 || slot >= JSMgr.allCallbackInfo.Count)
//         {
//			throw(new Exception("Bad slot: " + slot));
//             return JSApi.JS_FALSE;
//         }
//         JSMgr.CallbackInfo aInfo = JSMgr.allCallbackInfo[slot];
// 
//         currentParamCount = 4;
//         if (!isStatic)
//         {
//             this.jsObj = JSApi.JSh_ArgvObject(cx, vp, 4);
// 			if (this.jsObj == IntPtr.Zero) {
//				throw(new Exception("Invalid this jsObj"));
//                 return JSApi.JS_FALSE;
//			}
// 
//             this.csObj = JSMgr.getCSObj(jsObj);
//             //
//             // for manual js code
//             // this.csObj will be null
//             //
// 			//if (this.csObj == null) {
			//	throw(new Exception("Invalid this csObj"));
//             //    return JSApi.JS_FALSE;
			//}
// 
//             currentParamCount++;
//         }
// 
//         switch (op)
//         {
//             case Oper.GET_FIELD:
//             case Oper.SET_FIELD:
//                 {
//                     currIndex = currentParamCount;
//                     this.bGet = (op == Oper.GET_FIELD);
//                     JSMgr.CSCallbackField fun = aInfo.fields[index];
//                     if (fun == null)
//                     {
//                         throw (new Exception("Field not found"));
//                         return JSApi.JS_FALSE;
//                     }
//                     fun(this);
//                 }
//                 break;
//             case Oper.GET_PROPERTY:
//             case Oper.SET_PROPERTY:
//                 {
//                     currIndex = currentParamCount;
//                     this.bGet = (op == Oper.GET_PROPERTY);
//                     JSMgr.CSCallbackProperty fun = aInfo.properties[index];
//                     if (fun == null)
//                     {
//                         throw (new Exception("Property not found"));
//                         return JSApi.JS_FALSE;
//                     }
//                     fun(this);
//                 }
//                 break;
//             case Oper.METHOD:
//             case Oper.CONSTRUCTOR:
//                 {
//                     bool overloaded = JSApi.JSh_ArgvBool(cx, vp, currentParamCount);
//                     currentParamCount++;
// 
//                     JSMgr.MethodCallBackInfo[] arrMethod;
//                     if (op == Oper.METHOD)
//                         arrMethod = aInfo.methods;
//                     else
//                         arrMethod = aInfo.constructors;
// 
//                     // params count
//                     // for overloaded function, it's caculated by ExtractJSParams
//                     int jsParamCount = (int)argc - currentParamCount;
//                     if (!overloaded)
//                     {
//                         // for not-overloaded function
//                         int i = (int)argc;
//                         while (i > 0 && JSApi.jsval.isUndefined(JSApi.JSh_ArgvTag(cx, vp, --i)))
//                             jsParamCount--;
//                     }
//                     else
//                     {
//                         if (!this.ExtractJSParams(currentParamCount, (int)argc - currentParamCount))
//                             return JSApi.JS_FALSE;
// 
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
//								throw(new Exception("Overloaded function can't find match: " + methodName));
//                                 return JSApi.JS_FALSE;
//                             }
//                         }
// 
//                         jsParamCount = arrJSParamsLength;
//                     }
// 
//                     currIndex = currentParamCount;
//                     arrMethod[index].fun(this, currentParamCount, jsParamCount);
//                 }
//                 break;
//         }
//         return JSApi.JS_TRUE;
//     }

//     public int CallReflection(IntPtr cx, uint argc, IntPtr vp)
//     {
//         this.Reset(cx, vp);
// 
//         this.op = (Oper)JSApi.JSh_ArgvInt(cx, vp, 0);
//         int slot = JSApi.JSh_ArgvInt(cx, vp, 1);
//         int index = JSApi.JSh_ArgvInt(cx, vp, 2);
//         bool isStatic = JSApi.JSh_ArgvBool(cx, vp, 3);
// 
//         if (slot < 0 || slot >= JSMgr.allTypeInfo.Count)
//         {
//             Debug.LogError("Bad slot: " + slot);
//             return JSApi.JS_FALSE;
//         }
//         JSMgr.ATypeInfo aInfo = JSMgr.allTypeInfo[slot];
// 
//         currentParamCount = 4;
//         object csObj = null;
//         if (!isStatic)
//         {
//             IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, 4);
//             if (jsObj == IntPtr.Zero) {
//				Debug.LogError("Invalid this jsObj");
//                 return JSApi.JS_FALSE;
//			}
// 
//             csObj = JSMgr.getCSObj(jsObj);
// 			if (csObj == null) {
//				Debug.LogError("Invalid this csObj");
//                 return JSApi.JS_FALSE;
//			}
// 
//             currentParamCount++;
//         }
// 
//         //object result = null;
// 
//         switch (op)
//         {
//             case Oper.GET_FIELD:
//                 {
//                     result = aInfo.fields[index].GetValue(csObj);
//                 }
//                 break;
//             case Oper.SET_FIELD:
//                 {
//                     FieldInfo field = aInfo.fields[index];
//                     field.SetValue(csObj, JSValue_2_CSObject(field.FieldType, currentParamCount));
//                 }
//                 break;
//             case Oper.GET_PROPERTY:
//                 {
//                     result = aInfo.properties[index].GetValue(csObj, null);
//                 }
//                 break;
//             case Oper.SET_PROPERTY:
//                 {
//                     PropertyInfo property = aInfo.properties[index];
//                     property.SetValue(csObj, JSValue_2_CSObject(property.PropertyType, currentParamCount), null);
//                 }
//                 break;
//             case Oper.METHOD:
//             case Oper.CONSTRUCTOR:
//                 {
//                     bool overloaded = JSApi.JSh_ArgvBool(cx, vp, currentParamCount);
//                     currentParamCount++;
// 
//                     if (!this.ExtractJSParams(currentParamCount, (int)argc - currentParamCount))
//                         return JSApi.JS_FALSE;
// 
//                     if (overloaded)
//                     {
//                         MethodBase[] methods = aInfo.methods;
//                         if (op == Oper.CONSTRUCTOR)
//                             methods = aInfo.constructors;
// 
//                         if (-1 == MatchOverloadedMethod(methods, index))
//                             return JSApi.JS_FALSE;
//                     }
//                     else
//                     {
//                         m_Method = aInfo.methods[index];
//                         if (op == Oper.CONSTRUCTOR)
//                             m_Method = aInfo.constructors[index];
//                     }
// 
//                     this.ExtractCSParams();
// 
//                     //!!!!!!!!!!!!!!!!!!!!
//                     if (!BuildMethodArgs(true))
//                         return JSApi.JS_FALSE;
// 
//                     object[] cp = new object[callParamsLength];
//                     for (int i = 0; callParamsLength > i; i++)
//                         cp[i] = callParams[i];
// 
//                     result = this.m_Method.Invoke(csObj, cp);
//                 }
//                 break;
//         }
// 
//         this.PushResult(result);
//         return JSApi.JS_TRUE;
//     }
}