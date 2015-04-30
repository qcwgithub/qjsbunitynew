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
    public bool forceProtectJSFunction = false;

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

    /*
     * Formular:
     * All files - DirectoriesNotExport + DirectoriesExport = Files to export to js
     * see JSAnalyzer.MakeJsTypeAttributeInSrc for more information
     */
    public List<string> DirectoriesNotToExport;
    public List<string> DirectoriesToExport;


    public List<string> DirectoriesNotToReplace;

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

    public class Apple<T>
    {
        public void Print(T t)
        {
            Debug.Log(t.ToString());
        }
    }

    void Awake()
    {
// 		string tname = typeof(List<string>).GetGenericTypeDefinition().FullName;
// 		Type type = typeof(List<string>).Assembly.GetType (tname);
        //T//ype type = typeof(List<T>);
        //Apple<int> a = new Apple<int>();
        //MethodInfo method = typeof(Apple<string>).GetGenericTypeDefinition().GetMethod("Print");
        //Debug.Log("haha"+method.ContainsGenericParameters);
        //method.MakeGenericMethod(typeof(int)).Invoke(a, new object[] { 6677 });
        //method.Invoke(a, new object[]{6677});

        /*
         * Don't destroy this GameObject on load
         */
        DontDestroyOnLoad(gameObject);
        inst = this;

        // Can only be false
        JSMgr.useReflection = false;
        JSMgr.InitJSEngine(jsLoader, OnInitJSEngine);
    }

    void Update()
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

    // OUTPUT object mappings
	public bool showStatistics = true;
    void OnGUI()
    {
		if (!showStatistics)
			return;
        int countDict1, countDict2;
        JSMgr.GetDictCount(out countDict1, out countDict2);
        GUI.TextArea(new Rect(0, 10, 400, 20), "C#<->JS Object Total: " + countDict1.ToString() + ", Class: " + countDict2.ToString());

        Dictionary<long, JSMgr.JS_CS_Relation> dict1 = JSMgr.GetDict1();
        Dictionary<string, int> tj = new Dictionary<string, int>();
        foreach (var v in dict1)
        {
            var jscs = v.Value;
            if (tj.ContainsKey(jscs.name))
            {
                tj[jscs.name]++;
            }
            else
            {
                tj[jscs.name] = 1;
            }
        }
        float y = 40;
        foreach (var v in tj)
        {
            GUI.TextArea(new Rect(0, y, 400, 20), v.Key + ": " + v.Value);
            y += 20;
        }
    }
}