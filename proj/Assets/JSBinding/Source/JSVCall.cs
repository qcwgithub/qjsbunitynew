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
    public int currIndex = 0;
    ////////////////////////////////


    public void Reset()
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

        // TODO
        currIndex = 0;
    }

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

    public bool CallJSFunctionValue(int jsObjID, int funID, params object[] args)
    {
        if (JSMgr.isShutDown) return false;

        int argsLen = (args != null ? args.Length : 0);
        if (argsLen == 0)
        {
            return JSApi.callFunctionValue(jsObjID, funID, 0);
        }

        for (int i = 0; i < argsLen; i++)
        {
            this.datax.setWhatever((int)JSApi.SetType.TempVal, args[i]);
            JSApi.moveTempVal2Arr(i);
        }

        return JSApi.callFunctionValue(jsObjID, funID, argsLen);
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
    public Oper op;
    
    //
    // argc 还有几个参数
    //
    //
    public bool CallCallback(int iOP, int slot, int index, bool isStatic, int argc)
    {
        this.Reset();

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
                    JSMgr.MethodCallBackInfo[] arrMethod;
                    if (op == Oper.METHOD)
                        arrMethod = aInfo.methods;
                    else
                        arrMethod = aInfo.constructors;

                    arrMethod[index].fun(this, argc);
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