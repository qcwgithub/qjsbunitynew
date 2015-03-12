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
public class JSComponent : MonoBehaviour
{
    public string jsScriptName = string.Empty;

    [HideInInspector]
    [NonSerialized]
    public IntPtr go = IntPtr.Zero;

    jsval valAwake = new jsval();
    jsval valStart = new jsval();
    jsval valUpdate = new jsval();
    jsval valDestroy = new jsval();
    jsval valOnGUI = new jsval();

    bool jsLoaded = false;
    bool inited = false;

    public void Awake()
    {
        if (!JSEngine.inited)
            return;

        if (!inited)
        {
            if (!jsLoaded && jsScriptName.Length > 0)
            {
                jsLoaded = true;
                JSMgr.LoadJS(jsScriptName, this.OnLoadJS);
            }
        }

        if (inited && valAwake.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(go, ref valAwake);
    }

    void Start()
    {
        if (inited && valStart.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(go, ref valStart);
    }

    public void OnLoadJS(IntPtr ptrScript)
    {
        if (ptrScript == IntPtr.Zero)
        {
            Debug.Log("ptrScript is null)");
            enabled = false;
            return;
        }

        go = JSApi.JSh_NewObjectAsClass(JSMgr.cx, JSMgr.glob, "GameObject", JSMgr.mjsFinalizer);
        if (go == IntPtr.Zero) {
			Debug.LogWarning ("JSComponent: create JS GameObject object failed!");
            return;
		}

        // protect from being GC
        //JSApi.JSh_AddObjectRoot(JSMgr.cx, ref go);
        JSMgr.AddRootedObject(go);

        JSMgr.addJSCSRelation(go, gameObject);

        if (!JSMgr.ExecuteScript(ptrScript, go))
        {
            Debug.Log("---------- ExecuteScript fail");
            enabled = false;
            return;
        }

        valAwake.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, go, "Awake", ref valAwake);
        valStart.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, go, "Start", ref valStart);
        valUpdate.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, go, "Update", ref valUpdate);
        valDestroy.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, go, "Destroy", ref valDestroy);
        valOnGUI.asBits = 0; JSApi.JSh_GetFunctionValue(JSMgr.cx, go, "OnGUI", ref valOnGUI);

//         if (valAwake > 0) {
//             JSMgr.vCall.CallJSFunctionValue(go, ref valAwake, null);
//         }

        inited = true;
    }
    void Update()
    {
        if (!JSEngine.inited)
            return;

        if (!inited) {
            if (!jsLoaded && jsScriptName.Length > 0) {
                jsLoaded = true;
                JSMgr.LoadJS(jsScriptName, this.OnLoadJS);
            }
        }
        else
        {
            if (valUpdate.asBits > 0)
            {
                JSMgr.vCall.CallJSFunctionValue(go, ref valUpdate);
            }
        }
    }

    void OnDestroy()
    {
        if (JSMgr.isShutDown) return;

        if (inited && valDestroy.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(go, ref valDestroy);

        if (inited)
        {
            //JSApi.JSh_RemoveObjectRoot(JSMgr.cx, ref go);
            JSMgr.RemoveRootedObject(go);
        }
    }

    void OnGUI()
    {
        if (inited && valOnGUI.asBits > 0)
            JSMgr.vCall.CallJSFunctionValue(go, ref valOnGUI);
    }
}
