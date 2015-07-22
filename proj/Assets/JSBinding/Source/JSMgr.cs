using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using jsval = JSApi.jsval;

#pragma warning disable 414
public class MonoPInvokeCallbackAttribute : System.Attribute
{
    private Type type;
    public MonoPInvokeCallbackAttribute(Type t) { type = t; }
}
#pragma warning restore 414

public static class JSMgr
{
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSErrorReporter))]
    static int errorReporter(IntPtr cx, string message, IntPtr report)
    {
        string fileName = JSApi.getErroReportFileNameS(report);
        int lineno = JSApi.getErroReportLineNo(report);
        string str = fileName + "(" + lineno.ToString() + "): " + message;
        Debug.LogError(str);
        return 1;
    }

    // load generated js files
    public delegate void OnInitJSEngine(bool bSuccess);
    public static OnInitJSEngine onInitJSEngine;

    static bool RefCallStaticMethod(string className, string methodName)
    {
        Type t = Type.GetType(className);
        if (t == null) 
            return false;
        MethodInfo method = t.GetMethod(methodName);
        if (method == null)
            return false;
        method.Invoke(null, null);
        return true;
    }
    static object RefGetStaticField(string className, string fieldName)
    {
        Type t = Type.GetType(className);
        if (t == null)
            return null;
        FieldInfo field = t.GetField(fieldName);
        if (field == null)
            return null;
        return field.GetValue(null);
    }

    /// <summary>
    /// The js engine round
    /// jSEngineRound++ whenever ShutDownJSEngine
    /// start from 1
    /// </summary>
    public static int jsEngineRound = 1;
    static JSFileLoader jsLoader;
    public static bool InitJSEngine(JSFileLoader jsLoader, OnInitJSEngine onInitJSEngine)
    {
        ShutDown = false;

        int initResult = JSApi.InitJSEngine(
            new JSApi.JSErrorReporter(errorReporter), 
            new JSApi.CSEntry(JSMgr.CSEntry),
            new JSApi.JSNative(require),
            new JSApi.OnObjCollected(onObjCollected),
            new JSApi.JSNative(print));

        if (initResult != 0)
        {
            Debug.LogError("InitJSEngine fail. error = " + initResult);
            onInitJSEngine(false);
            return false;
        }

        if (!RefCallStaticMethod("CSharpGenerated", "RegisterAll"))
        {
            Debug.LogError("Call CSharpGenerated.RegisterAll() failed. Did you forget to click menu [JSB/Generate JS and CS Bindings]?");
            onInitJSEngine(false);
            return false;
        }

        JSMgr.jsLoader = jsLoader;

        onInitJSEngine(true);
        return true;
    }

    public static bool ShutDown = false;
    public static bool isShutDown { get { return ShutDown; } }
    public static void ShutdownJSEngine()
    {
        // There is a JS_GC called inside JSApi.ShutdownJSEngine
#if UNITY_EDITOR
        // DO NOT really cleanup everything, because we wanna start again
        JSApi.ShutdownJSEngine(0);
#else
        JSApi.ShutdownJSEngine(1);
#endif

        // Here:
        // mDictionary1 and mDictionary2 should only left JSComponent object
        // because their 'OnDestroy' may not be called yet
        // It's OK not to call 'JSMgr.ClearJSCSRel()' here
        //
        ShutDown = true;
        allCallbackInfo.Clear();
        JSMgr.ClearJSCSRel();
        evaluatedScript.Clear();
        jsEngineRound++;
    }
    

    /// <summary>
    /// Gets the full name of the javascript file.
    /// </summary>
    /// <param name="shortName">The short name.</param>
    /// <param name="bGenerated">if set to <c>true</c> [b generated].</param>
    /// <returns></returns>
    static public string getJSFullName(string shortName, bool bGenerated)
    {
        string baseDir = bGenerated ? JSBindingSettings.jsGeneratedDir : JSBindingSettings.jsDir;
        string fullName = baseDir + "/" + shortName;// + JSBindingSettings.jsExtension;
        // don't append, if extension already exist
        if (shortName.IndexOf('.') < 0)
        {
            fullName += JSBindingSettings.jsExtension;
        }
        return fullName;
    }

    static Dictionary<string, bool> evaluatedScript = new Dictionary<string, bool>();

    //static Dictionary<long, IntPtrClass> rootedObject = new Dictionary<long, IntPtrClass>();

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// callback function list
    /// </summary>
    /// 
    public delegate void CSCallbackField(JSVCall vc);
    public delegate void CSCallbackProperty(JSVCall vc);
    public delegate bool CSCallbackMethod(JSVCall vc, int argc);

    public class MethodCallBackInfo
    {
        public MethodCallBackInfo(CSCallbackMethod f, string n) { fun = f; methodName = n; }
        public CSCallbackMethod fun;
        public string methodName; // this is originally used to distinguish overloaded methods
    }

    // usage
    // 1 use for calling cs from js, by directly-call
    public class CallbackInfo
    {
        public Type type;
        public CSCallbackField[] fields;
        public CSCallbackProperty[] properties;

        public MethodCallBackInfo[] constructors;
        public MethodCallBackInfo[] methods;
    }
    public static List<CallbackInfo> allCallbackInfo = new List<CallbackInfo>();

    public static BindingFlags BindingFlagsMethod = 
        BindingFlags.Public 
        | BindingFlags.Instance 
        | BindingFlags.Static 
        | BindingFlags.DeclaredOnly;

    // used to judge it's overloaded function or not
    // used in JSGenerator
    public static BindingFlags BindingFlagsMethod2 =
        BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Instance
        | BindingFlags.Static;

    public static BindingFlags BindingFlagsProperty = 
        BindingFlags.Public 
        | BindingFlags.GetProperty 
        | BindingFlags.SetProperty 
        | BindingFlags.Instance 
        | BindingFlags.Static 
        | BindingFlags.DeclaredOnly;

    public static BindingFlags BindingFlagsField = 
        BindingFlags.Public 
        | BindingFlags.GetField 
        | BindingFlags.SetField 
        | BindingFlags.Instance 
        | BindingFlags.Static 
        | BindingFlags.DeclaredOnly;

    public static JSVCall vCall = new JSVCall();
    public static JSDataExchangeMgr datax = new JSDataExchangeMgr();


    /// <summary>
    /// CSEntry: entry for javascript CS.Call
    /// </summary>
    /// <param name="iOP">The i op.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="index">The index.</param>
    /// <param name="isStatic">The is static.</param>
    /// <param name="argc">The argc.</param>
    /// <returns></returns>
    [MonoPInvokeCallbackAttribute(typeof(JSApi.CSEntry))]
    static int CSEntry(int iOP, int slot, int index, int isStatic, int argc)
    {
        if (JSMgr.isShutDown) return 0; 
        try
        {
            vCall.CallCallback(iOP, slot, index, isStatic, argc);
        }
        catch (System.Exception ex)
        {
            /* 
             * if exception occurs, catch it, pass the error to js, and return false
             * js then print the error string and js call stack
             * note: the error contains cs call stack, so now we have both cs and js call stack
             */
            //JSApi.JSh_ReportError(cx, ex.ToString());
            JSApi.reportError(ex.ToString());
            return 0;
        }

        return 1;
    }

    public static bool evaluate(string jsScriptName)
    {
        if (evaluatedScript.ContainsKey(jsScriptName))
        {
            return true;
        }
        // add even failed
        evaluatedScript.Add(jsScriptName, true);

        string fullName = JSMgr.getJSFullName(jsScriptName, false);
        byte[] bytes = jsLoader.LoadJSSync(fullName);

        if (bytes == null)
        {
            Debug.LogError(jsScriptName + "file content is null");
            return false;
        }
        else if (bytes.Length == 0)
        {
            Debug.LogError(jsScriptName + "file content length = 0");
            return false;
        }

        bool ret = (1 == JSApi.evaluate(bytes, (uint)bytes.Length, fullName));
        return ret;
    }

    /// <summary>
    /// execute a JavaScript script
    /// can only require a script once.
    /// </summary>
    /// <param name="cx">The cx.</param>
    /// <param name="argc">The argc.</param>
    /// <param name="vp">The vp.</param>
    /// <returns></returns>
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool require(IntPtr cx, uint argc, IntPtr vp)
    {
        string jsScriptName = JSApi.getArgStringS(vp, 0);
        bool ret = evaluate(jsScriptName);
        JSApi.setRvalBoolS(vp, ret);
        return true;
    }

    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool print(IntPtr cx, uint argc, IntPtr vp)
    {
        string str = JSApi.getArgStringS(vp, 0);
        UnityEngine.Debug.Log(str);
        return true;
    }

    #region JS_CS_REL

    // TODO check
    public class JS_CS_Rel
    {
        public int jsObjID;
        public object csObj;
        public string name;
        public int hash;
        public JS_CS_Rel(int jsObjID, object csObj, int h)
        {
            this.jsObjID = jsObjID;
            this.csObj = csObj;
            this.name = csObj.GetType().Name;// csObj.ToString();
            this.hash = h;
        }

    }
    public static void addJSCSRel(int jsObjID, object csObj, bool weakReference = false)
    {
        if (weakReference)
        {
            int hash = csObj.GetHashCode();
            WeakReference wrObj = new WeakReference(csObj);
            mDictionary1.Add(jsObjID, new JS_CS_Rel(jsObjID, wrObj, hash));
            mDictionary2.Add(hash, new JS_CS_Rel(jsObjID, wrObj, hash));
        }
        else
        {
            int hash = csObj.GetHashCode();
            if (mDictionary1.ContainsKey(jsObjID))
            {
                if (csObj.GetType().IsValueType)
                {
                    mDictionary1.Remove(jsObjID);
                }
            }

            mDictionary1.Add(jsObjID, new JS_CS_Rel(jsObjID, csObj, hash));

            if (csObj.GetType().IsClass)
            {
                mDictionary2.Add(hash, new JS_CS_Rel(jsObjID, csObj, hash));
            }
        }
    }

    public static void removeJSCSRel(int id, int round = 0)
    {
        // don't remove an ID belonging to previous round
        if (round == 0 || round == JSMgr.jsEngineRound)
        {
            JS_CS_Rel Rel;
            if (mDictionary1.TryGetValue(id, out Rel))
            {
                mDictionary1.Remove(id);
                mDictionary2.Remove(Rel.hash);
            }
            else if (!JSMgr.isShutDown)
            {
                Debug.LogError("JSMgr.removeJSCSRel: " + id + " not found.");
            }
        }
        else if (round > 0)
        {
            // 
            // Debug.Log(new StringBuilder().AppendFormat("didn't remove id {0} because it belongs to old round {1}", id, round));
        }
    }

    public static object getCSObj(int jsObjID)
    {
        JS_CS_Rel obj;
        if (mDictionary1.TryGetValue(jsObjID, out obj))
        {
            object ret = obj.csObj;
            if (ret is WeakReference)
            {
                object tar = ((WeakReference)ret).Target;
                if (tar == null)
                {
                    Debug.LogError("ERROR: JSMgr.getCSObj WeakReference.Target == null");
                }
                return tar;
            }
            return ret;
        }
        return null;
    }
    public static int getJSObj(object csObj)
    {
        if (csObj.GetType().IsValueType)
        {
            return 0;
        }

        JS_CS_Rel Rel;
        int hash = (csObj is WeakReference) ? ((WeakReference)csObj).Target.GetHashCode() : csObj.GetHashCode();
        if (mDictionary2.TryGetValue(hash, out Rel))
        {
            return Rel.jsObjID;
        }
        return 0;
    }
    public static void changeJSObj(int jsObjID, object csObjNew)
    {
        if (!csObjNew.GetType().IsValueType)
        {
            Debug.LogError("class can not call changeJSObj2");
            return;
        }
        JS_CS_Rel Rel;
        if (mDictionary1.TryGetValue(jsObjID, out Rel))
        {
            mDictionary1.Remove(jsObjID);
            mDictionary1.Add(jsObjID, new JS_CS_Rel(jsObjID, csObjNew, csObjNew.GetHashCode()));
        }
    }
    public static void ClearJSCSRel()
    {
        mDictionary1.Clear();
        mDictionary2.Clear();
    }

    [MonoPInvokeCallbackAttribute(typeof(JSApi.OnObjCollected))]
    static void onObjCollected(int id)
    {
        removeJSCSRel(id);
    }
    static Dictionary<int, JS_CS_Rel> mDictionary1 = new Dictionary<int, JS_CS_Rel>(); // key = OBJID
    /// <summary>
    /// NOTICE
    /// two C# object may have same hash code?
    /// if Destroy(go) was called, obj becomes null, ... 
    /// TODO
    /// </summary>
    static Dictionary<int, JS_CS_Rel> mDictionary2 = new Dictionary<int, JS_CS_Rel>(); // key = object.GetHashCode()

    public static void GetDictCount(out int countDict1, out int countDict2)
    {
        countDict1 = mDictionary1.Count;
        countDict2 = mDictionary2.Count;
    }
    public static Dictionary<int, JS_CS_Rel> GetDict1() { return mDictionary1;  }

    #endregion

    #region JS<->CS fun<->Delegate relationship

    class JS_CS_FunRel
    {
        public WeakReference wr;
        public int hashCode;
    }
    static Dictionary<int, JS_CS_FunRel> mDictJSFun1 = new Dictionary<int, JS_CS_FunRel>(); // key = FUNCTION ID, Value = JS_CS_FunRel(Delegate, Delegate.GetHashCode())
    static Dictionary<int, int> mDictJSFun2 = new Dictionary<int,int>(); // key = Delegate.GetHashCode(), Value = FUNCTIONID
    public static void addJSFunCSDelegateRel(int funID, Delegate del)
    {
        if (!mDictJSFun1.ContainsKey(funID))
        {
            JS_CS_FunRel rel = new JS_CS_FunRel();
            {
                rel.wr = new WeakReference(del);
                rel.hashCode = del.GetHashCode();
            }

            mDictJSFun1.Add(funID, rel);
            mDictJSFun2.Add(rel.hashCode, funID);
        }
    }
    public static void removeJSFunCSDelegateRel(int funID)
    {
        JS_CS_FunRel rel;
        if (mDictJSFun1.TryGetValue(funID, out rel))
        {
            mDictJSFun1.Remove(funID);
            mDictJSFun2.Remove(rel.hashCode);
        }
    }
    public static int getFunIDByDelegate(Delegate del)
    {
        int hash = del.GetHashCode();

        int funID;
        if (mDictJSFun2.TryGetValue(hash, out funID))
        {
            return funID;
        }
        return 0;
    }
    public static string getJSFunCSDelegateCount()
    {
        var c1 = mDictJSFun1.Count;
        var c2 = mDictJSFun2.Count;
        if (c1 == c2)
        {
            return c1.ToString();
        }
        return "" + c1 + "/" + c2;
    } 
    #endregion
}
