using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

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

    public void OnInitJSEngine(bool bSuccess)
    {
        /* 
         * Debugging is only available in desktop platform
         * */
        mDebug = debug;
        if (bSuccess)
        {
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
