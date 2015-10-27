using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using jsval = JSApi.jsval;
/// <summary>
/// JSEngine
/// Represents a JavaScript Engine object
/// In order to run JavaScript, there must be one and only one JSEngine object in the scene
/// You can find JSEngine prefab at path 'JSBinding/Prefabs/_JSEngine.prefab'
/// 
/// JSEngine must have a lower execution order than JSComponent.
/// You can set script execution order by click menu Edit | Project Settings | Script Execution Order
/// for example, set JSEngine to 400, set JSComponent to 500
/// </summary>
public class JSEngine : MonoBehaviour
{
    public static JSEngine inst;
    public static int initState = 0;
    public static bool initSuccess { get { return initState == 1; } set { if (value) initState = 1; } }
    public static bool initFail { get { return initState == 2; } set { if (value) initState = 2; else initState = 0; } }

    /*
     * Debug settings, if port is not available, try another one
     */
    public bool debug = true;
    public int port = 5086;
    bool mDebug = true;

    /*
     * Garbage Collection setting
     * if GCInterval < 0, will not call GC (default value, SpiderMonkey will automatically call GC)
     * if GCInterval >= 0, will call GC every GCInterval seconds
     */
    public float GCInterval = -1f;
    public JSFileLoader jsLoader;

    /*
     * 
     */
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
                    // JSMgr.ExecuteFile(InitLoadScripts[i]);
                    JSMgr.evaluate(InitLoadScripts[i]);
                }
            }

            if (JSApi.initErrorHandler() == 1)
                Debug.Log("JS: print error stack: YES");
            else
                Debug.Log("JS: print error stack: NO");

            initSuccess = true;
            Debug.Log("JS: Init JSEngine OK");
            if (mDebug)
            {
                Debug.Log("JS: Enable Debugger");
                JSApi.enableDebugger(new string[2] { JSBindingSettings.jsDir, JSBindingSettings.jsGeneratedDir }, 2, port);
            }
        }
        else
        {
            initFail = true;
            Debug.Log("JS: Init JSEngine FAIL");
        }
    }

    // FirstInit may be called from JSComponent!
    public static void FirstInit(JSEngine jse = null)
    {
        if (!initSuccess && !initFail)
        {
            if (jse == null)
            {
                GameObject jseGO = GameObject.Find("_JSEngine");
                if (jseGO == null)
                {
                    initFail = true;
                    Debug.LogError("_JSEngine gameObject not found. Drag a \"JSBinding/Prefabs/_JSEngine.prefab\" to the scene.");
                }
                else
                {
                    jse = jseGO.GetComponent<JSEngine>();
                }
            }

            if (jse != null)
            {
                /*
                * Don't destroy this GameObject on load
                */
                DontDestroyOnLoad(jse.gameObject);
                inst = jse;

                JSMgr.InitJSEngine(jse.jsLoader, jse.OnInitJSEngine);
            }
        }
    }

    void Awake()
    {
        if (JSEngine.inst != null && JSEngine.inst != this)
        {
            // destroy self if there is already a JSEngine gameObject
            Destroy(gameObject);
            return;
        }

        JSEngine.FirstInit(this);
    }

    int jsCallCountPerFrame = 0;
    void Update()
    {
        if (this != JSEngine.inst)
            return;

        jsCallCountPerFrame = JSMgr.vCall.jsCallCount;
        JSMgr.vCall.jsCallCount = 0;

        UpdateThreadSafeActions();

        if (initSuccess)
        {
            if (mDebug)
                JSApi.updateDebugger();
        }
    }

    List<Action> lstThreadSafeActions = new List<Action>();
    bool hasThreadSafeActions = false;
    object @lock = new object();

    public void DoThreadSafeAction(Action action)
    {
        lock (@lock)
        {
            hasThreadSafeActions = true;
            lstThreadSafeActions.Add(action);
        }
    }
    void UpdateThreadSafeActions()
    {
        if (hasThreadSafeActions)
        {
            lock (@lock)
            {
                foreach (Action action in lstThreadSafeActions)
                {
                    action();
                }
                lstThreadSafeActions.Clear();
                hasThreadSafeActions = false;
            }
        }
    }

    float accum = 0f;
    void LateUpdate()
    {
        if (this != JSEngine.inst)
            return;

        if (initSuccess && GCInterval >= 0f)
        {
            accum += Time.deltaTime;
            if (accum > GCInterval)
            {
                accum = 0f;
                //Debug.Log("_GC_Begin");
                JSApi.gc();
                //Debug.Log("_GC_End");
            }
        }
    }

//     void OnApplicationQuit()
//     {
//         Debug.Log("OnApplicationQuit");
//     }
    void OnDestroy()
    {
        if (this == JSEngine.inst)
        {
            if (mDebug && initSuccess)
            {
                JSApi.cleanupDebugger();
            }
            JSMgr.ShutdownJSEngine();
            JSEngine.inst = null;
            JSEngine.initState = 0;
            Debug.Log("JS: JSEngine Destroy");
        }
    }

    // Utilities
    JSComponentUtil util = null;
    public bool IsInheritanceRel(string baseClassName, string subClassName)
    {
        if (util == null)
        {
            util = GetComponent<JSComponentUtil>();
        }
        if (util != null)
        {
            return util.IsInheritanceRel(baseClassName, subClassName);
        }
        return false;
    }

	public bool showStatistics = true;
    public int guiX = 0;



    /// <summary>
    /// OnGUI: Output some statistics
    /// </summary>
    void OnGUI()
    {
        if (this != JSEngine.inst)
            return;
		if (!showStatistics)
			return;
        int countDict1, countDict2;

        JSMgr.GetDictCount(out countDict1, out countDict2);

        GUI.TextArea(new Rect(guiX, 10, 500, 20), "JS->CS Count " + this.jsCallCountPerFrame + " Round " + JSMgr.jsEngineRound + " Objs(Total " + countDict1.ToString() + ", Class " + countDict2.ToString() + ") CSR(Obj " + CSRepresentedObject.s_objCount + " Fun " + CSRepresentedObject.s_funCount + ") Del " + JSMgr.getJSFunCSDelegateCount());

        int clsCount = 0;
        Dictionary<int, JSMgr.JS_CS_Rel> dict1 = JSMgr.GetDict1();
        List<int> Keys = new List<int>(dict1.Keys);

        Dictionary<string, int> class2Count = new Dictionary<string, int>();
        foreach (var K in Keys)
        {
            if (!dict1.ContainsKey(K))
                continue;

            var Rel = dict1[K];
            if (class2Count.ContainsKey(Rel.name))
            {
                class2Count[Rel.name]++;
            }
            else
            {
                class2Count[Rel.name] = 1;
            }
            if (Rel.csObj != null && Rel.csObj.GetType().IsClass)
            {
                clsCount++;
            }
        }
        float y = 40;

        GUI.TextArea(new Rect(guiX, y, 400, 20), "class count: " + clsCount);
        y += 20;

        GUI.TextArea(new Rect(guiX, y, 400, 20), "valueMapSize: " + JSApi.getValueMapSize());
        y += 20;

        GUI.TextArea(new Rect(guiX, y, 400, 20), "valueMapIndex: " + JSApi.getValueMapIndex());
        y += 20;

        foreach (var v in class2Count)
        {
            GUI.TextArea(new Rect(guiX, y, 400, 20), v.Key + ": " + v.Value);
            y += 20;
        }
    }
}