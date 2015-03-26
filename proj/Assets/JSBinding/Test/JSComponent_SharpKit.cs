using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security;


using jsval = JSApi.jsval;

/*
 * JSComponent
 * A class simply transfer callbacks to js
 * 
 * This usage might cost much cpu times. Especially when there are a lot of GameObjects in the scene
 * One likely solution is call Awake, Start, Update only once per frame
 * 
 */
public class JSComponent_SharpKit : ExtraHelper
{
    //public string jsScriptName = string.Empty;

    [HideInInspector]
    [NonSerialized]
    public IntPtr jsObj = IntPtr.Zero;

    jsval valAwake = new jsval();
    jsval valStart = new jsval();
    jsval valFixedUpdate = new jsval();
    jsval valUpdate = new jsval();
    jsval valDestroy = new jsval();
    jsval valOnGUI = new jsval();
    jsval valOnTriggerEnter2D = new jsval();

    bool inited = false;

    void initVal(ref jsval val, string jsFunName)
    {
        val.asBits = 0;
        JSApi.JSh_GetFunctionValue(JSMgr.cx, jsObj, jsFunName, ref val);
    }
    void callIfExist(ref jsval val, params object[] args)
    {
        if (val.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(jsObj, ref val, args);
    }

    public bool initJS()
    {
        if (string.IsNullOrEmpty(jsScriptName))
            return false;

        jsval[] valParam = new jsval[2];
        jsval valRet = new jsval();

        // 1)
        // __nativeObj: csObj + finalizer
        //
        IntPtr __nativeObj = JSApi.JSh_NewMyClass(JSMgr.cx, JSMgr.mjsFinalizer);

        JSApi.JSh_SetJsvalString(JSMgr.cx, ref valParam[0], this.jsScriptName);
        JSApi.JSh_SetJsvalObject(ref valParam[1], __nativeObj);

        // 2)
        // jsObj: prototype
        // jsObj.__nativeObj = __nativeObj
        //
        valRet.asBits = 0;
        bool ret = JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.glob, "jsb_NewMonoBehaviour", 2, valParam, ref valRet);
        if (ret) jsObj = JSApi.JSh_GetJsvalObject(ref valRet);
        if (!ret || jsObj == IntPtr.Zero)
        {
            jsObj = IntPtr.Zero;
            Debug.LogError("New MonoBehaviour Fail, name: " + this.jsScriptName + ". Did you forget to export that class?");
            return false;
        }
        JSMgr.addJSCSRelation(jsObj, __nativeObj, this);

        // TODO:
        // handle serialization here
        //
        //
        initSerializedData(JSMgr.cx, jsObj);
        //
        //
        //

        JSMgr.AddRootedObject(jsObj);

        initVal(ref valAwake, "Awake");
        initVal(ref valStart, "Start");
        initVal(ref valFixedUpdate, "FixedUpdate");
        initVal(ref valUpdate, "Update");
        initVal(ref valDestroy, "Destroy");
        initVal(ref valOnGUI, "OnGUI");
        initVal(ref valOnTriggerEnter2D, "OnTriggerEnter2D");

        inited = true;
        return true;
    }
    public void Awake()
    {
        if (!JSEngine.inited)
            return;

        if (!initJS())
        {
            return;
        }
    }

    private bool firstStart = true;
    void Start() 
    {
        if (firstStart)
        {
            firstStart = false;
            callIfExist(ref valAwake);
        }
        callIfExist(ref valStart);
    }

    void FixedUpdate()
    {
        callIfExist(ref valFixedUpdate);
    }
    void Update()
    {
        callIfExist(ref valUpdate);
    }

    void OnDestroy()
    {
        if (JSMgr.isShutDown)
        {
            return;
        }

        callIfExist(ref valDestroy);

        if (inited)
        {
            JSMgr.RemoveRootedObject(jsObj);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
//        if (other == null)
//            Debug.Log("OnTriggerEnter2D(null)");
//        else
//            Debug.Log("OnTriggerEnter2D(" + other.GetType().Name + ")");
        callIfExist(ref valOnTriggerEnter2D, other);
    }
}
