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
            JSMgr.datax.setWhatever((int)JSApi.SetType.SaveAndTempTrace, args[i]);
            JSApi.moveSaveID2Arr(i);
        }

        return JSApi.callFunctionValue(jsObjID, funID, argsLen);
    }


    public bool CallJSFunctionName(int jsObjID, string funName, params object[] args)
    {
        if (JSMgr.isShutDown) return false;
        int funID = JSApi.getObjFunction(jsObjID, funName);
        if (funID <= 0)
            return false;

        return CallJSFunctionValue(jsObjID, funID, args);
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

    public bool bGet = false; // for property
    public int jsObjID = 0;
    public object csObj;
    
    public bool CallCallback(int iOP, int slot, int index, int isStatic, int argc)
    {
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

            // for manual javascript code, this.csObj will be null
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