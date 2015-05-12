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
public class JSComponent : JSSerializer
{
    //public string jsScriptName = string.Empty;

//     [HideInInspector]
//     [NonSerialized]
//     public IntPtr jsObj = IntPtr.Zero;

    [HideInInspector]
    [NonSerialized]
    public int jsObjID = 0;

    Dictionary<string, bool> existMemberFunctions = new Dictionary<string, bool>();
    void initMemberFun()
    {
        var arr = new string[] 
        {
            "Awake",
            "Start",
            "FixedUpdate",
            "Update",
            "Destroy",
            "OnGUI",
            "OnEnable",

            "OnTriggerEnter2D",
            "OnTriggerStay",
            "OnTriggerExit",
            "OnAnimatorMove",
            "OnAnimatorIK",

            "DestroyChildGameObject",
            "DisableChildGameObject",
            "DestroyGameObject",
        };
        foreach (var name in arr)
        {
            bool bExist = JSApi.IsJSClassObjectFunctionExist(jsObjID, name);
            if (bExist)
            {
                existMemberFunctions.Add(name, true);
            }
        }
    }

//     jsval valAwake = new jsval();
//     jsval valStart = new jsval();
//     jsval valFixedUpdate = new jsval();
//     jsval valUpdate = new jsval();
//     jsval valDestroy = new jsval();
//     jsval valOnGUI = new jsval();
//     jsval valOnEnable = new jsval();
//     jsval valOnTriggerEnter2D = new jsval();
//     jsval valOnTriggerStay = new jsval();
//     jsval valOnTriggerExit = new jsval();
//     jsval valOnAnimatorMove = new jsval();
//     jsval valOnAnimatorIK = new jsval();
// 
//     jsval valDestroyChildGameObject = new jsval();
//     jsval valDisableChildGameObject = new jsval();
//     jsval valDestroyGameObject = new jsval();

    int initState = 0;
    bool initSuccess { get { return initState == 1; } set { if (value) initState = 1; } }
    bool initFail { get { return initState == 2; } set { if (value) initState = 2; } }

//     void initVal(ref jsval val, string jsFunName)
//     {
//         val.asBits = 0;
//         JSApi.JSh_GetFunctionValue(JSMgr.cx, jsObj, jsFunName, ref val);
//     }
//     void callIfExist(ref jsval val, params object[] args)
//     {
//         if (val.asBits > 0)
//             JSMgr.vCall.CallJSFunctionValue(jsObj, ref val, args);
//     }
    void callIfExist(string name, params object[] args)
    {
        JSMgr.vCall.CallJSFunctionName(jsObjID, name, args);
    }

    public void initJS()
    {
        if (initFail || initSuccess) return;

        if (string.IsNullOrEmpty(jsScriptName))
        {
            initFail = true;
            return;
        }

        jsObjID = JSApi.NewJSClassObject(this.jsScriptName);
        if (jsObjID == 0)
        {
            Debug.LogError("New MonoBehaviour \"" + this.jsScriptName + "\" failed. Did you forget to export that class?");
            initFail = true;
            return;
        } 
        JSMgr.AddJSCSRel(jsObjID, this);
        initMemberFun();
        // TODO add root
        JSApi.addObjectRoot(jsObjID);
        initSuccess = true;

//         jsval[] valParam = new jsval[2];
//         jsval valRet = new jsval();
// 
//         // 1)
//         // __nativeObj: csObj + finalizer
//         //
//         IntPtr __nativeObj = JSApi.JSh_NewMyClass(JSMgr.cx, JSMgr.mjsFinalizer);
// 
//         JSApi.JSh_SetJsvalString(JSMgr.cx, ref valParam[0], this.jsScriptName);
//         JSApi.JSh_SetJsvalObject(ref valParam[1], __nativeObj);
// 
//         // 2)
//         // jsObj: prototype
//         // jsObj.__nativeObj = __nativeObj
//         //
//         valRet.asBits = 0;
//         bool ret = JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.glob, "jsb_NewMonoBehaviour", 2, valParam, ref valRet);
//         if (ret) jsObj = JSApi.JSh_GetJsvalObject(ref valRet);
//         if (!ret || jsObj == IntPtr.Zero)
//         {
//             jsObj = IntPtr.Zero;
//             Debug.LogError("New MonoBehaviour \"" + this.jsScriptName + "\" failed. Did you forget to export that class?");
//             initFail = true;
//             return;
//         }
//         JSMgr.addJSCSRelation(jsObj, __nativeObj, this);
// 
//         JSMgr.AddRootedObject(jsObj);
// 
//         initVal(ref valAwake, "Awake");
//         initVal(ref valStart, "Start");
//         initVal(ref valFixedUpdate, "FixedUpdate");
//         initVal(ref valUpdate, "Update");
//         initVal(ref valDestroy, "Destroy");
//         initVal(ref valOnGUI, "OnGUI");
//         initVal(ref valOnEnable, "OnEnable");
//         
//         initVal(ref valOnTriggerEnter2D, "OnTriggerEnter2D");
//         initVal(ref valOnTriggerStay, "OnTriggerStay");
//         initVal(ref valOnTriggerExit, "OnTriggerExit");
//         initVal(ref valOnAnimatorMove, "OnAnimatorMove");
//         initVal(ref valOnAnimatorIK, "OnAnimatorIK");
// 
// 
//         // TODO
//         // ??
//         initVal(ref valDestroyChildGameObject, "DestroyChildGameObject");
//         initVal(ref valDisableChildGameObject, "DisableChildGameObject");
//         initVal(ref valDestroyGameObject, "DestroyGameObject");
//         initSuccess = true;
    }
    public void Awake()
    {
        if (!JSEngine.inited)
            return;

        initJS();

        if (initSuccess)
        {
            initSerializedData(jsObjID);
        }
    }
    /// <summary>
    /// 获取 jsObj
    /// 可能本脚本的 Awake 还未执行就由其他脚本调用了这个函数
    /// 因为其他脚本需要引用到这个脚本的 jsObj
    /// </summary>
    /// <returns></returns>
    /// 
    // TODO
    public int GetJSObjID()
    {
        if (jsObjID == 0)
        {
            if (!initFail) 
                initJS();
        }
        return jsObjID;
    }

    private bool firstStart = true;
    void Start() 
    {
        if (firstStart)
        {
            firstStart = false;
            callIfExist("Awake");
        }
        callIfExist("Start");
    }

    void FixedUpdate()
    {
        callIfExist("FixedUpdate");
    }
    void Update()
    {
        callIfExist("Update");
    }

    void OnDestroy()
    {
        if (JSMgr.isShutDown)
        {
            return;
        }

        callIfExist("OnDestroy");

        if (initSuccess)
        {
            // JSMgr.RemoveRootedObject(jsObj);
            JSApi.removeObjectRoot(jsObjID);
        }
    }
    void OnEnable()
    {
        callIfExist("OnEnable");
    }
    void OnGUI()
    {
        callIfExist("OnGUI");
    }

    void OnTriggerEnter2D (Collider2D other)
    {
//        if (other == null)
//            Debug.Log("OnTriggerEnter2D(null)");
//        else
//            Debug.Log("OnTriggerEnter2D(" + other.GetType().Name + ")");
        callIfExist("OnTriggerEnter2D", other);
    }
    void OnTriggerStay(Collider other)
    {
        callIfExist("OnTriggerStay", other);
    }
    void OnTriggerExit(Collider other)
    {
        callIfExist("OnTriggerExit", other);
    }
    void OnAnimatorMove()
    {
        callIfExist("OnAnimatorMove");
    }
    void OnAnimatorIK(int layerIndex)
    {
        callIfExist("OnAnimatorIK");
    }

    void DestroyChildGameObject()
    {
        callIfExist("DestroyChildGameObject");
    }

    void DisableChildGameObject()
    {
        callIfExist("DisableChildGameObject");
    }

    void DestroyGameObject()
    {
        callIfExist("DestroyGameObject");
    }
}
