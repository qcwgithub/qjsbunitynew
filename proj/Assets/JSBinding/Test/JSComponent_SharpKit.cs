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
public class JSComponent_SharpKit : JSSerializer
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
    jsval valOnTriggerStay = new jsval();
    jsval valOnTriggerExit = new jsval();
    jsval valOnAnimatorMove = new jsval();
    jsval valOnAnimatorIK = new jsval();

    jsval valDestroyChildGameObject = new jsval();
    jsval valDisableChildGameObject = new jsval();
    jsval valDestroyGameObject = new jsval();

    int initState = 0;
    bool initSuccess { get { return initState == 1; } set { if (value) initState = 1; } }
    bool initFail { get { return initState == 2; } set { if (value) initState = 2; } }

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

    public void initJS()
    {
        if (initFail || initSuccess) return;

        if (string.IsNullOrEmpty(jsScriptName))
        {
            initFail = true;
            return;
        }

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
            Debug.LogError("New MonoBehaviour \"" + this.jsScriptName + "\" failed. Did you forget to export that class?");
            initFail = true;
            return;
        }
        JSMgr.addJSCSRelation(jsObj, __nativeObj, this);

        JSMgr.AddRootedObject(jsObj);

        initVal(ref valAwake, "Awake");
        initVal(ref valStart, "Start");
        initVal(ref valFixedUpdate, "FixedUpdate");
        initVal(ref valUpdate, "Update");
        initVal(ref valDestroy, "Destroy");
        initVal(ref valOnGUI, "OnGUI");
        initVal(ref valOnTriggerEnter2D, "OnTriggerEnter2D");
        initVal(ref valOnTriggerStay, "OnTriggerStay");
        initVal(ref valOnTriggerExit, "OnTriggerExit");
        initVal(ref valOnAnimatorMove, "OnAnimatorMove");
        initVal(ref valOnAnimatorIK, "OnAnimatorIK");


        // TODO
        // ??
        initVal(ref valDestroyChildGameObject, "DestroyChildGameObject");
        initVal(ref valDisableChildGameObject, "DisableChildGameObject");
        initVal(ref valDestroyGameObject, "DestroyGameObject");

        initSuccess = true;
    }
    public void Awake()
    {
        if (!JSEngine.inited)
            return;

        initJS();
        if (initSuccess)
        {
            initSerializedData(JSMgr.cx, jsObj);
        }
    }
    /// <summary>
    /// 获取 jsObj
    /// 可能本脚本的 Awake 还未执行就由其他脚本调用了这个函数
    /// 因为其他脚本需要引用到这个脚本的 jsObj
    /// </summary>
    /// <returns></returns>
    public IntPtr GetJSObj()
    {
        if (jsObj == IntPtr.Zero)
        {
            if (!initFail) 
                initJS();
        }
        return jsObj;
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

        if (initSuccess)
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
    void OnTriggerStay(Collider other)
    {
        callIfExist(ref valOnTriggerStay, other);
    }
    void OnTriggerExit(Collider other)
    {
        callIfExist(ref valOnTriggerExit, other);
    }
    void OnAnimatorMove()
    {
        callIfExist(ref valOnAnimatorMove);
    }
    void OnAnimatorIK(int layerIndex)
    {
        callIfExist(ref valOnAnimatorIK);
    }

    void DestroyChildGameObject()
    {
        callIfExist(ref valDestroyChildGameObject);
    }

    void DisableChildGameObject()
    {
        callIfExist(ref valDisableChildGameObject);
    }

    void DestroyGameObject()
    {
        callIfExist(ref valDestroyGameObject);
    }
}
