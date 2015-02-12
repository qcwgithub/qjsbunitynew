using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using jsval = JSApi.jsval;
public class JSEngine : MonoBehaviour 
{
    public static JSEngine inst;
    public static bool inited = false;

    /*
     * Debug settings, if port is not available, try another one
     */
    public bool debug = true;
    public int port = 5086;
    bool mDebug = true;

    /*
     * Garbage Collection setting
     */
    public float GCInterval = 3f;
    public JSFileLoader jsLoader;

    /*
     * 
     */
    [NonSerialized]
    public bool OutputFullCallingStackOnError = false;
    public string[] InitLoadScripts = new string[0];

    public void OnInitJSEngine(bool bSuccess)
    {
        /* 
         * Debugging is only available in desktop platform
         * */
        mDebug = debug;
        if (bSuccess)
        {
            if (InitLoadScripts != null)
            {
                for (var i = 0; i < InitLoadScripts.Length; i++)
                {
                    JSMgr.ExecuteFile(InitLoadScripts[i]);
                }
            }

            //
            // Check to see ErrorHandler exists?
            //
            jsval valJSFunEntry = new jsval();
            valJSFunEntry.asBits = 0;
            if (JSApi.JSh_GetFunctionValue(JSMgr.cx, JSMgr.CSOBJ, "jsFunctionEntry", ref valJSFunEntry) && valJSFunEntry.asBits > 0)
            {
                Debug.Log("JS: Enable printing calling stack on error.");
                OutputFullCallingStackOnError = true;
            }
            else
            {

                Debug.Log("JS: Disable printing calling stack on error.");
            }

            inited = true;
            string[] path = new string[2];
            path[0] = JSBindingSettings.jsDir;
            path[1] = JSBindingSettings.jsGeneratedDir;
            Debug.Log("----------Init JSEngine OK ---");
            if (mDebug)
            {
                Debug.Log("JS: Enable Debugger");
                JSApi.JSh_EnableDebugger(JSMgr.cx, JSMgr.glob, path, 2, port);
            }
        }
        else
            Debug.Log("----------Init JSEngine FAIL ---");

    }

	void Awake () 
    {
        /*
         * Don't destroy this GameObject on load
         */
        DontDestroyOnLoad(gameObject);
        inst = this;

        // Can only be false
        JSMgr.useReflection = false;
        JSMgr.InitJSEngine(jsLoader, OnInitJSEngine);
    }

	void Update () 
    {
        if (inited)
        {
            if (mDebug)
			    JSApi.JSh_UpdateDebugger();
        }
	}

    float accum = 0f;
    void LateUpdate()
    {
        if (inited)
        {
            accum += Time.deltaTime;
            if (accum > GCInterval)
            {
                accum = 0f;
                //Debug.Log("_GC_Begin");
                JSApi.JSh_GC(JSMgr.rt);
                //Debug.Log("_GC_End");
            }
        }
    }

    void OnDestroy()
    {
        JSMgr.ShutdownJSEngine();
        if (mDebug)
            JSApi.JSh_CleanupDebugger();
        Debug.Log("----------JSEngine Destroy ---");
    }
}
