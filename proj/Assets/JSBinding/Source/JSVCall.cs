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
            // TODO memory leak
            this.datax.setWhatever((int)JSApi.SetType.SaveAndTempTrace, args[i]);
            JSApi.moveSaveID2Arr(i);
        }

        return JSApi.callFunctionValue(jsObjID, funID, argsLen);
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
    public int jsObjID = 0;
    public object csObj;
    // TODO delete
    public Oper op;
    
    //
    // argc 还有几个参数
    //
    //
    public bool CallCallback(int iOP, int slot, int index, int isStatic, int argc)
    {
        this.Reset();
        this.jsObjID = 0;
        this.csObj = null;

        Oper op = (Oper)iOP;

        if (slot < 0 || slot >= JSMgr.allCallbackInfo.Count)
        {
            throw (new Exception("Bad slot: " + slot));
            return false;
        }
        JSMgr.CallbackInfo aInfo = JSMgr.allCallbackInfo[slot];
        if (isStatic == 0)
        {
            this.jsObjID = JSApi.getObject((int)JSApi.GetType.Arg);
            if (this.jsObjID == 0)
            {
                throw (new Exception("Invalid this jsObjID"));
                return false;
            }

            // for manual js code, this.csObj will be null
            this.csObj = JSMgr.getCSObj(jsObjID);
            //if (this.csObj == null) {
            //	throw(new Exception("Invalid this csObj"));
            //    return JSApi.JS_FALSE;
            //}

            --argc;
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
}