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
    [HideInInspector]
    [NonSerialized]
    public int jsObjID = 0;

    void initMemberFunction()
    {
        idAwake = JSApi.getObjFunction(jsObjID, "Awake");
        idStart = JSApi.getObjFunction(jsObjID, "Start"); ;
        idFixedUpdate = JSApi.getObjFunction(jsObjID, "FixedUpdate"); ;
        idUpdate = JSApi.getObjFunction(jsObjID, "Update"); ;
        idOnDestroy = JSApi.getObjFunction(jsObjID, "OnDestroy"); ;
        idOnGUI = JSApi.getObjFunction(jsObjID, "OnGUI"); ;
        idOnEnable = JSApi.getObjFunction(jsObjID, "OnEnable"); ;
        idOnTriggerEnter2D = JSApi.getObjFunction(jsObjID, "OnTriggerEnter2D"); ;
        idOnTriggerStay = JSApi.getObjFunction(jsObjID, "OnTriggerStay"); ;
        idOnTriggerExit = JSApi.getObjFunction(jsObjID, "OnTriggerExit"); ;
        idOnAnimatorMove = JSApi.getObjFunction(jsObjID, "OnAnimatorMove"); ;
        idOnAnimatorIK = JSApi.getObjFunction(jsObjID, "OnAnimatorIK"); ;
        idDestroyChildGameObject = JSApi.getObjFunction(jsObjID, "DestroyChildGameObject"); ;
        idDisableChildGameObject = JSApi.getObjFunction(jsObjID, "DisableChildGameObject"); ;
        idDestroyGameObject = JSApi.getObjFunction(jsObjID, "DestroyGameObject"); ;
    }

    int idAwake = 0;
    int idStart = 0;
    int idFixedUpdate = 0;
    int idUpdate = 0;
    int idOnDestroy = 0;
    int idOnGUI = 0;
    int idOnEnable = 0;
    int idOnTriggerEnter2D = 0;
    int idOnTriggerStay = 0;
    int idOnTriggerExit = 0;
    int idOnAnimatorMove = 0;
    int idOnAnimatorIK = 0;
    int idDestroyChildGameObject = 0;
    int idDisableChildGameObject = 0;
    int idDestroyGameObject = 0;

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
    void callIfExist(int funID, params object[] args)
    {
        if (funID > 0)
        {
            JSMgr.vCall.CallJSFunctionValue(jsObjID, funID, args);
        }
    }

    public void initJS()
    {
        if (initFail || initSuccess) return;

        if (string.IsNullOrEmpty(jsScriptName))
        {
            initFail = true;
            return;
        }

        // ATTENSION
        // cannot use createJSClassObject here
        // because we have to call ctor, to run initialization code
        // this object will not have finalizeOp
        jsObjID = JSApi.newJSClassObject(this.jsScriptName);
        JSApi.setTrace(jsObjID, true);
        if (jsObjID == 0)
        {
            Debug.LogError("New MonoBehaviour \"" + this.jsScriptName + "\" failed. Did you forget to export that class?");
            initFail = true;
            return;
        } 
        JSMgr.AddJSCSRel(jsObjID, this);
        initMemberFunction();
        // TODO add root
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
            callIfExist(idAwake);
        }
        callIfExist(idStart);
    }

    void FixedUpdate()
    {
        callIfExist(idFixedUpdate);
    }
    void Update()
    {
        callIfExist(idUpdate);
    }

    void OnDestroy()
    {
        if (JSMgr.isShutDown)
        {
            return;
        }

        callIfExist(idOnDestroy);

        if (initSuccess)
        {
            // JSMgr.RemoveRootedObject(jsObj);
            JSApi.setTrace(jsObjID, false);
            JSMgr.removeJSCSRel(jsObjID);
        }
    }
    void OnEnable()
    {
        callIfExist(idOnEnable);
    }
    void OnGUI()
    {
        callIfExist(idOnGUI);
    }

    void OnTriggerEnter2D (Collider2D other)
    {
//        if (other == null)
//            Debug.Log("OnTriggerEnter2D(null)");
//        else
//            Debug.Log("OnTriggerEnter2D(" + other.GetType().Name + ")");
        callIfExist(idOnTriggerEnter2D, other);
    }
    void OnTriggerStay(Collider other)
    {
        callIfExist(idOnTriggerStay, other);
    }
    void OnTriggerExit(Collider other)
    {
        callIfExist(idOnTriggerExit, other);
    }
    void OnAnimatorMove()
    {
        callIfExist(idOnAnimatorMove);
    }
    void OnAnimatorIK(int layerIndex)
    {
        callIfExist(idOnAnimatorIK);
    }

    void DestroyChildGameObject()
    {
        callIfExist(idDestroyChildGameObject);
    }

    void DisableChildGameObject()
    {
        callIfExist(idDisableChildGameObject);
    }

    void DestroyGameObject()
    {
        callIfExist(idDestroyGameObject);
    }
}
