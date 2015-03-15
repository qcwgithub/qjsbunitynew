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
public class JSComponent_SharpKit : MonoBehaviour
{
    public string jsScriptName = string.Empty;

    [HideInInspector]
    [NonSerialized]
    public IntPtr jsObj = IntPtr.Zero;

    jsval valAwake = new jsval();
    jsval valStart = new jsval();
    jsval valUpdate = new jsval();
    jsval valDestroy = new jsval();
    jsval valOnGUI = new jsval();

    bool inited = false;

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
        JSMgr.addJSCSRelation(__nativeObj, this);

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
            Debug.LogError("New MonoBehaviour Fail, name: " + this.jsScriptName);
            return false;
        }

        JSMgr.AddRootedObject(jsObj);

        valAwake.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, jsObj, "Awake", ref valAwake);
        valStart.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, jsObj, "Start", ref valStart);
        valUpdate.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, jsObj, "Update", ref valUpdate);
        valDestroy.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, jsObj, "Destroy", ref valDestroy);
        valOnGUI.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, jsObj, "OnGUI", ref valOnGUI);

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

        if (inited && valAwake.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(jsObj, ref valAwake);
    }

    void Start()
    {
        if (inited && valStart.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(jsObj, ref valStart);
    }

    void Update()
    {
        if (valUpdate.asBits > 0)
        {
            JSMgr.vCall.CallJSFunctionValue(jsObj, ref valUpdate);
        }
    }

    void OnDestroy()
    {
        if (JSMgr.isShutDown) return;

        if (inited && valDestroy.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(jsObj, ref valDestroy);

        if (inited)
        {
            JSMgr.RemoveRootedObject(jsObj);
        }
    }
}
