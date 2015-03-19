/*
 * 
 * 
 */


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
    public static IntPtr rt;
    public static IntPtr cx;
    public static IntPtr glob;
    public static bool useReflection = false;

//     [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
//     static bool printInt(IntPtr cx, UInt32 argc, IntPtr vp)
//     {
//         int value = JSApi.JSh_ArgvInt(cx, vp, 0);
//         Debug.Log(value);
//         return true;
//     }
// 
//     [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
//     static bool printString(IntPtr cx, UInt32 argc, IntPtr vp)
//     {
//         string value = JSApi.JSh_ArgvStringS(cx, vp, 0);
//         Debug.Log(value);
//         return true;
//     }
//     [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
//     static bool printDouble(IntPtr cx, UInt32 argc, IntPtr vp)
//     {
//         double value = JSApi.JSh_ArgvDouble(cx, vp, 0);
//         Debug.Log(value);
//         return true;
//     }

    static IntPtr jsErrorReporter = IntPtr.Zero;
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSErrorReporter))]
    static int errorReporter(IntPtr cx, string message, IntPtr report)
    {
        string fileName = JSApi.JSh_GetErroReportFileNameS(report);
        int lineno = JSApi.JSh_GetErroReportLineNo(report);
        string str = fileName + "(" + lineno.ToString() + "): " + message;
        //if (jsErrorReporter == IntPtr.Zero)
            Debug.LogWarning(str);
        //else
        //    JSMgr.vCall.CallJSFunction(IntPtr.Zero, jsErrorReporter, str);
        return 1;
    }

    // js function for error reporter must stay alive
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool SetErrorReporter(IntPtr cx, uint argc, IntPtr vp)
    {
        if (argc != 1)
            return true;

        jsErrorReporter = JSApi.JSh_ArgvFunction(cx, vp, 0);
        return true;
    }

    // load generated js files
    public delegate void OnInitJSEngine(bool bSuccess);
    public static OnInitJSEngine onInitJSEngine;
    static int loadedGeneratedJS = 0;
    static string[] jsGeneratedFileNames;
    public static void OnLoadGeneratedJS(string shortName, byte[] bytes, string fullName)
    {
        if (bytes == null) {
            //onInitJSEngine(false);
            return;
        }

        IntPtr ptrScript = JSMgr.CompileScriptContentByte(shortName, bytes, JSMgr.glob, fullName);
        if (ptrScript == IntPtr.Zero)
        {
            return;
        }
        JSMgr.ExecuteScript(ptrScript, JSMgr.glob);

        loadedGeneratedJS++;
        if (loadedGeneratedJS >= jsGeneratedFileNames.Length) {
            onInitJSEngine(true);
        }
    }

    // load js files
    public delegate void OnLoadJS(IntPtr ptrScript);
    public static void LoadJS(string shortName, OnLoadJS onLoadJS)
    {
        IntPtr ptrScript = JSMgr.GetCompiledScript(shortName);
        if (ptrScript != IntPtr.Zero)
        {
            onLoadJS(ptrScript);
            return;
        }
        else
        {
            JSFileLoader.OnLoadJS action = (shName, bytes, fullName) =>
            {
                // find again
                ptrScript = JSMgr.GetCompiledScript(shortName);
                if (ptrScript != IntPtr.Zero)
                {
                    onLoadJS(ptrScript);
                    return;
                }
                ptrScript = JSMgr.CompileScriptContentByte(shName, bytes, JSMgr.glob, fullName);
                onLoadJS(ptrScript);
            };
            jsLoader.LoadJSSync(shortName, false, action);
        }
    }

    static object RefCallStaticMethod(string className, string methodName)
    {
        Type t = Type.GetType(className);
        if (t == null) 
            return null;
        MethodInfo method = t.GetMethod(methodName);
        if (method == null)
            return null;
        return method.Invoke(null, null);
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
    /* test! */
    public static void GenericTest(Type type, string s)
    {
        if (type.IsGenericParameter)
            s += "IsGenericParameter: true\n";
        else
            s += "IsGenericParameter: false\n";
        if (type.IsGenericType)
            s += "IsGenericType: true\n";
        else
            s += "IsGenericType: false\n";
        if (type.IsGenericTypeDefinition)
            s += "IsGenericTypeDefinition: true\n";
        else
            s += "IsGenericTypeDefinition: false\n";

        if (type.ContainsGenericParameters)
            s += "ContainsGenericParameters: true\n";
        else
            s += "ContainsGenericParameters: false\n";

        Type[] ts = type.GetGenericArguments();
        s += "GetGenericArguments: ";
        for (int i = 0; i < ts.Length; i++)
        {
            s += "\n" + ts[i].ToString() + "\n";
        }

        Debug.Log(s);
    }


    static IntPtr oldCompartment = IntPtr.Zero;
    static JSFileLoader jsLoader;
    public static bool InitJSEngine(JSFileLoader jsLoader, OnInitJSEngine onInitJSEngine)
    {
        //GenericTest(typeof(List<Component>), "List<Component> \n");
        if (!JSApi.JSh_Init())
        {
            onInitJSEngine(false);
            return false;
        }

        rt = JSApi.JSh_NewRuntime(8 * 1024 * 1024, 0);
        JSApi.JSh_SetNativeStackQuota(rt, 500000, 0, 0);

        cx = JSApi.JSh_NewContext(rt, 8192);

        // Set error reporter
        JSApi.JSh_SetErrorReporter(cx, new JSApi.JSErrorReporter(errorReporter));

        glob = JSApi.JSh_NewGlobalObject(cx, 1);

        oldCompartment = JSApi.JSh_EnterCompartment(cx, glob);

        if (!JSApi.JSh_InitStandardClasses(cx, glob))
        {
            Debug.LogError("JSh_InitStandardClasses fail. Make sure JSh_SetNativeStackQuota was called.");
            onInitJSEngine(false);
            return false;
        }

        JSApi.JSh_InitReflect(cx, glob);

//         JSApi.JSh_DefineFunction(cx, glob, "printInt", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printInt)), 1, 0/*4164*/);
//         JSApi.JSh_DefineFunction(cx, glob, "printString", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printString)), 1, 0/*4164*/);
//         JSApi.JSh_DefineFunction(cx, glob, "printDouble", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(printDouble)), 1, 0/*4164*/);

        // Resources.Load
        JSMgr.RegisterCS(cx, glob);
        JSValueWrap.Register(CSOBJ, cx);
        //         if (useReflection)
        //         {
        //             for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        //                 JSMgr.AddTypeInfo(JSBindingSettings.classes[i]);
        //         }

        // bool jsRegistered = false;

        jsGeneratedFileNames = (string[])RefGetStaticField("JSGeneratedFileNames", "names");
        //jsGeneratedFileNames = JSGeneratedFileNames.names;

        if (jsGeneratedFileNames == null)
        {
            Debug.LogError("InitJSEngine failed. Click menu [Assets -> JSBinding -> Generate JS and CS Bindings]");
            onInitJSEngine(false);
            return false;
        }

        RefCallStaticMethod("CSharpGenerated", "RegisterAll");
        //CSharpGenerated.RegisterAll(); // register cs function



        JSMgr.jsLoader = jsLoader;
        JSMgr.onInitJSEngine = onInitJSEngine;
        foreach (var shortName in jsGeneratedFileNames)
            jsLoader.LoadJSSync(shortName, true, JSMgr.OnLoadGeneratedJS);

        return true;
    }
    public static JSApi.SC_FINALIZE mjsFinalizer = new JSApi.SC_FINALIZE(JSObjectFinalizer);

    public static bool ShutDown = false;
    public static bool isShutDown { get { return ShutDown; } }
    public static void ShutdownJSEngine()
    {
        JSApi.JSh_LeaveCompartment(cx, oldCompartment);


        JSMgr.ClearJSCSRelation();
        JSMgr.ClearRootedObject();
        JSMgr.ClearCompiledScript();

        JSApi.JSh_DestroyContext(cx);
        JSApi.JSh_DestroyRuntime(rt);
        JSApi.JSh_ShutDown();
        ShutDown = true;
    }


//     public static void EvaluateFile(string fullName, IntPtr obj)
//     {
//         jsval val = new jsval();
//         StreamReader r = new StreamReader(fullName, Encoding.UTF8);
//         string s = r.ReadToEnd();
//         JSApi.JSh_EvaluateScript(cx, obj, s, (uint)s.Length, fullName, 1, ref val);
//         r.Close();
//     }
//     public static void EvaluateString(string name, string s, IntPtr obj)
//     {
//         jsval val = new jsval();
//         JSApi.JSh_EvaluateScript(cx, obj, s, (uint)s.Length, name, 1, ref val);
//     }
//     static string calcFullJSFileName(string shortName, bool bGenerated)
//     {
//         string baseDir = bGenerated ? JSBindingSettings.jsGeneratedDir : JSBindingSettings.jsDir;
// 
// #if UNITY_ANDROID && !UNITY_EDITOR_WIN
//         string fullName = baseDir + "/" + shortName + JSBindingSettings.jsExtension;
// #else
//         string fullName = baseDir + "/" + shortName + JSBindingSettings.jsExtension;
// #endif
//         return fullName;
//     }
//     public static string ReadFileString(string fullName)
//     {
//         //Debug.Log("-----calcFullJSFileName: " + fullName);
// 
// #if UNITY_ANDROID && !UNITY_EDITOR_WIN
//         WWW w = new WWW(fullName);
//         while (true)
//         {
//             if (w.error != null && w.error.Length > 0)
//             {
//                 Debug.Log("ERROR: /// " + w.error);
//                 return "";
//             }
// 
//             if (w.isDone)
//                 break;
// 
//             //SleepTimeout()
//         }
//         //Debug.Log("======= WWW OK");
//         string content = w.text;
//         return content;
// #else
//         StreamReader r = new StreamReader(fullName, Encoding.UTF8);
//         string content = r.ReadToEnd();
//         return content;
// #endif
//     }
//     public static bool EvaluateGeneratedScripts(string[] shortNames)
//     {
//         for (int i = 0; i < shortNames.Length; i++)
//         {
//             string content = ReadFileString(calcFullJSFileName(shortNames[i], true));
//             if (content.Length == 0)
//                 return false;
//             EvaluateString(shortNames[i], content, glob);
//         }
//         return true;
//     }
    //     public static IntPtr CompileScript(string shortName, IntPtr obj)
    //     {
    //         string fullName = calcFullJSFileName(shortName);
    // 
    //         StreamReader r = new StreamReader(fullName, Encoding.UTF8);
    //         string s = r.ReadToEnd();
    // 
    //         IntPtr ptr = JSApi.JSh_CompileScript(cx, obj, s, (uint)s.Length, shortName, 1);
    //         r.Close();
    //         return ptr;
    //     }
    class IntPtrClass
    {
        public IntPtrClass(IntPtr ptr) 
        {
            this.pptr = JSApi.JSh_NewPPointer(ptr); 
            this.ptr = ptr; 
        }
        // ptr is a pointer
        // and *pptr == ptr
        public IntPtr pptr;
        public IntPtr ptr;
    }

    /*
     * Get JS file full path by short name, and weather it's generated or not
     * js files are in JSBindingSettings.jsDir/
     * and in JSBindingSettings.jsGeneratedDir/ if is generated
     */
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

    public static IntPtr CompileScriptContentByte(string shortName, byte[] content, IntPtr obj, string fullName)
    {
        if (content.Length == 0)
        {
            Debug.Log(shortName + " content length = 0");
            return IntPtr.Zero;
        }

        IntPtr ptr = JSApi.JSh_CompileScript(cx, obj, content, (uint)content.Length, fullName, 1);
        IntPtrClass ptrClass = new IntPtrClass(ptr);
        bool b = JSApi.JSh_AddNamedScriptRoot(JSMgr.cx, ptrClass.pptr, shortName);
        if (!b) Debug.LogWarning("JSh_AddNamedScriptRoot fail!!");
        compiledScript.Add(shortName, ptrClass);
        return ptr;
    }
    // execute script even if the same script has been executed before
    public static bool ExecuteScript(IntPtr ptrScript, IntPtr obj)
    {
        jsval val = new jsval();
        return JSApi.JSh_ExecuteScript(cx, obj, ptrScript, ref val);
    }
    // execute script only if the script not executed with the same obj
    // but executing script for different objects is allowed
    // ONLY for 'CS.require' use
    public static bool ExecuteScriptGlobal(IntPtr ptrScript, IntPtr obj)
    {
        IntPtr ptrObj;
        if (executedScript.TryGetValue(ptrScript, out ptrObj))
        {
            if (ptrObj == obj)
                return true;
        }

        jsval val = new jsval();
		executedScript.Add(ptrScript, obj);
        bool b = JSApi.JSh_ExecuteScript(cx, obj, ptrScript, ref val);
        if (!b) 
		{
			executedScript.Remove(ptrScript);
			return false;
		}
        return true;
    }
    /*
     * Get Compiled Script ptr by short name
     */
    static IntPtr GetCompiledScript(string shortName)
    {
        IntPtrClass ptrClass = null;
        if (compiledScript.TryGetValue(shortName, out ptrClass))
            return ptrClass.ptr;
        return IntPtr.Zero;
    }

    static void ClearCompiledScript()
    {
        foreach (var sc in compiledScript)
        {
            JSApi.JSh_RemoveScriptRoot(cx, sc.Value.pptr);
            JSApi.JSh_DelPPointer(sc.Value.pptr);
        }
        compiledScript.Clear();
    }
    static Dictionary<string, IntPtrClass> compiledScript = new Dictionary<string, IntPtrClass>();
    static Dictionary<IntPtr, IntPtr> executedScript = new Dictionary<IntPtr, IntPtr>();

    public static void AddRootedObject(IntPtr obj)
    {
        if (!rootedObject.ContainsKey(obj.ToInt64()))
        {
            IntPtrClass cls = new IntPtrClass(obj);
            JSApi.JSh_AddObjectRoot(JSMgr.cx, cls.pptr);
            rootedObject.Add(obj.ToInt64(), cls);
        }
    }
    public static void RemoveRootedObject(IntPtr obj)
    {
        IntPtrClass cls;
        if (rootedObject.TryGetValue(obj.ToInt64(), out cls))
        {
            JSApi.JSh_RemoveObjectRoot(JSMgr.cx, cls.pptr);
            JSApi.JSh_DelPPointer(cls.pptr);
            rootedObject.Remove(obj.ToInt64());
        }
    }
    static void ClearRootedObject()
    {
        foreach (var sc in rootedObject)
        {
            JSApi.JSh_RemoveObjectRoot(JSMgr.cx, sc.Value.pptr);
            JSApi.JSh_DelPPointer(sc.Value.pptr);
        }
        rootedObject.Clear();
    }
    static Dictionary<long, IntPtrClass> rootedObject = new Dictionary<long, IntPtrClass>();

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// callback function list
    /// </summary>
    /// 
    public delegate void CSCallbackField(JSVCall vc);
    public delegate void CSCallbackProperty(JSVCall vc);
    public delegate bool CSCallbackMethod(JSVCall vc, int start, int count);

    public class MethodCallBackInfo
    {
        public MethodCallBackInfo(CSCallbackMethod f, string n, JSVCall.CSParam[] a) { fun = f; methodName = n; arrCSParam = a; }
        public CSCallbackMethod fun;
        public string methodName;//这个用于判断是否重载函数
        public JSVCall.CSParam[] arrCSParam;
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

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// type info list
    /// </summary>

    // usage
    // 1 used for generating js code
    // 2 used for generating cs code
    // 3 used for calling cs from js, by reflection
    public class ATypeInfo
    {
        public FieldInfo[] fields;
        public PropertyInfo[] properties;
        public ConstructorInfo[] constructors;
        public MethodInfo[] methods;
        public int[] methodsOLInfo;//0 not overloaded >0 overloaded index
        public int howmanyConstructors;//how many constructors actually, (before filtering).
    }
    public static List<ATypeInfo> allTypeInfo = new List<ATypeInfo>();

    public static void ClearTypeInfo()
    {
        //         CallbackInfo cbi = new CallbackInfo();
        //         cbi.fields = new List<CSCallbackField>();
        //         cbi.fields.Add(Vector3Generated.Vector3_x);

        allTypeInfo.Clear();
    }
    public static int AddTypeInfo(Type type)
    {
        ATypeInfo tiOut = new ATypeInfo();
        return AddTypeInfo(type, out tiOut);
    }
    public static int AddTypeInfo(Type type, out ATypeInfo tiOut)
    {
        ATypeInfo ti = new ATypeInfo();
        ti.fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        ti.properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        ti.methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        ti.constructors = type.GetConstructors();
        ti.howmanyConstructors = ti.constructors.Length;

        FilterTypeInfo(type, ti);

        int slot = allTypeInfo.Count;
        allTypeInfo.Add(ti);
        tiOut = ti;
        return slot;
    }
    public static bool IsMemberObsolete(MemberInfo mi)
    {
        object[] attrs = mi.GetCustomAttributes(true);
        for (int j = 0; j < attrs.Length; j++)
        {
            if (attrs[j].GetType() == typeof(System.ObsoleteAttribute))
            {
                return true;
            }
        }
        return false;
    }
    public static int MethodInfoComparison(MethodInfo m1, MethodInfo m2)
    {
        if (!m1.IsStatic && m2.IsStatic)
            return -1;
        if (m1.IsStatic && !m2.IsStatic)
            return 1;
        if (m1.Name != m2.Name)
            return string.Compare(m1.Name, m2.Name);
        int max1 = 0;
        {
            ParameterInfo[] ps = m1.GetParameters();
            if (ps.Length > 0) max1 = ps.Length;
            for (int i = ps.Length - 1; i >= 0; i--)
            {
                if (!ps[i].IsOptional)
                    break;
                max1--;
            }
        }
        int max2 = 0;
        {
            ParameterInfo[] ps = m2.GetParameters();
            if (ps.Length > 0) max2 = ps.Length;
            for (int i = ps.Length - 1; i >= 0; i--)
            {
                if (!ps[i].IsOptional)
                    break;
                max2--;
            }
        }
        if (max1 > max2) return -1;
        if (max2 > max1) return 1;
        return 0;
    }
    public static void FilterTypeInfo(Type type, ATypeInfo ti)
    {
        bool bIsStaticClass = (type.IsClass && type.IsAbstract && type.IsSealed);

        List<ConstructorInfo> lstCons = new List<ConstructorInfo>();
        List<FieldInfo> lstField = new List<FieldInfo>();
        List<PropertyInfo> lstPro = new List<PropertyInfo>();
        Dictionary<string, int> proAccessors = new Dictionary<string, int>();
        List<MethodInfo> lstMethod = new List<MethodInfo>();

        for (int i = 0; i < ti.constructors.Length; i++)
        {
            // don't generate MonoBehaviour constructor
            if (type == typeof(UnityEngine.MonoBehaviour)) { 
                continue;  
            }

            if (!IsMemberObsolete(ti.constructors[i]))
                lstCons.Add(ti.constructors[i]);
        }

        for (int i = 0; i < ti.fields.Length; i++)
        {
            if (typeof(System.Delegate).IsAssignableFrom(ti.fields[i].FieldType.BaseType))
            {
                //Debug.Log("[field]" + type.ToString() + "." + ti.fields[i].Name + "is delegate!");
            }

            if (!IsMemberObsolete(ti.fields[i]) && !JSBindingSettings.IsDiscard(type, ti.fields[i]))
                lstField.Add(ti.fields[i]);
        }


        for (int i = 0; i < ti.properties.Length; i++)
        {
            PropertyInfo pro = ti.properties[i];

            if (typeof(System.Delegate).IsAssignableFrom(pro.PropertyType.BaseType))
            {
                // Debug.Log("[property]" + type.ToString() + "." + pro.Name + "is delegate!");
            }

            MethodInfo[] accessors = pro.GetAccessors();
            foreach (var v in accessors)
            {
                if (!proAccessors.ContainsKey(v.Name))
                    proAccessors.Add(v.Name, 0);
            }


//            if (pro.GetIndexParameters().Length > 0)
//                continue;
//            if (pro.Name == "Item") //[] not support
//                continue;

            // Skip Obsolete
            if (IsMemberObsolete(pro))
                continue;

            if (JSBindingSettings.IsDiscard(type, pro))
                continue;

            lstPro.Add(pro);
        }



        for (int i = 0; i < ti.methods.Length; i++)
        {
            MethodInfo method = ti.methods[i];

            // skip non-static method in static class
            if (bIsStaticClass && !method.IsStatic)
            {
                // NGUITools
                //Debug.Log("........."+type.Name+"."+method.Name);
                continue;
            }

            // skip property accessor
            if (method.IsSpecialName &&
                proAccessors.ContainsKey(method.Name))
                continue;

            if (method.IsSpecialName)
            {
                if (method.Name == "op_Addition" ||
                    method.Name == "op_Subtraction" ||
                    method.Name == "op_UnaryNegation" ||
                    method.Name == "op_Multiply" ||
                    method.Name == "op_Division" ||
                    method.Name == "op_Equality" ||
                    method.Name == "op_Inequality")
                {
                    if (!method.IsStatic)
                    {
                        // Debug.LogWarning("IGNORE not-static special-name function: " + type.Name + "." + method.Name);
                        continue;
                    }
                }
                else
                {
                     Debug.LogWarning("IGNORE special-name function:" + type.Name + "." + method.Name);
                    continue;
                }
            }

            // Skip Obsolete
            if (IsMemberObsolete(method))
                continue;

            if (method.IsGenericMethodDefinition /* || method.IsGenericMethod*/)
            {
//                string s = "GENERIC METHOD DEFINITION " + type.Name + "." + method.Name;
//                foreach (var v in method.GetGenericArguments())
//                {
//                    s += " " + v.Name;
//                }
//                Debug.Log(s);
//                continue;

                bool bDiscard = false;
                var ps = method.GetParameters();
                for (int k = 0; k < ps.Length; k++)
                {
                    if (ps[k].ParameterType.ContainsGenericParameters) {
                        bDiscard = true;
                        break;
                    }
                }
                if (bDiscard)
                    continue;
            }


            if (JSBindingSettings.IsDiscard(type, method))
                continue;

            lstMethod.Add(method);
        }

        if (lstMethod.Count == 0)
            ti.methodsOLInfo = null;
        else
        {
            // sort methods
            lstMethod.Sort(MethodInfoComparison);
            ti.methodsOLInfo = new int[lstMethod.Count];
        }

        int overloadedIndex = 1;
        bool bOL = false;
        for (int i = 0; i < lstMethod.Count; i++)
        {
            ti.methodsOLInfo[i] = 0;
            if (bOL)
            {
                ti.methodsOLInfo[i] = overloadedIndex;
            }

            if (i < lstMethod.Count - 1 && lstMethod[i].Name == lstMethod[i + 1].Name &&
                ((lstMethod[i].IsStatic && lstMethod[i + 1].IsStatic) || (!lstMethod[i].IsStatic && !lstMethod[i + 1].IsStatic)))
            {
                if (!bOL)
                {
                    overloadedIndex = 1;
                    bOL = true;
                    ti.methodsOLInfo[i] = overloadedIndex;
                }
                overloadedIndex++;
            }
            else
            {
                bOL = false;
                overloadedIndex = 1;
            }
        }

        ti.constructors = lstCons.ToArray();
        ti.fields = lstField.ToArray();
        ti.properties = lstPro.ToArray();
        ti.methods = lstMethod.ToArray();
    }

    public static IntPtr CSOBJ = IntPtr.Zero;
    public static JSVCall vCall = new JSVCall();

    /*
     * 'Call'
     * This is the entry for calling cs from js
     * 99% js calls comes here
     */
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool Call(IntPtr cx, uint argc, IntPtr vp)
    {
        //         if (useReflection)
        //             return vCall.CallReflection(cx, argc, vp);
        //         else

        if (JSMgr.isShutDown) return false;

        try
        {
            vCall.CallCallback(cx, argc, vp);
        }
        catch (System.Exception ex)
        {
            /* 
             * if exception occurs, catch it, pass the error to js, and return false
             * js then print the error string and js call stack
             * note: the error contains cs call stack, so now we have both cs and js call stack
             */
            JSApi.JSh_ReportError(cx, ex.ToString());
            return false;
        }
        
        return true;
    }

    /*
     * 'AddJSComponent'
     * Add a js script as GameObject's component
     * Similar to GameObject.AddComponent
     */
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool AddJSComponent(IntPtr cx, uint argc, IntPtr vp)
    {
        if (argc != 2)
        {
            Debug.LogError("AddJSComponent() need 2 params!");
            return true;
        }

        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, 0);
        object csObj = JSMgr.getCSObj(jsObj);
        if (csObj == null || !(csObj is GameObject))
        {
            Debug.LogWarning("AddJSComponent() 1st param must be GameObject");
            return true;
        }

        GameObject go = (GameObject)csObj;

        if (!JSApi.JSh_ArgvIsString(cx, vp, 1))
        {
            Debug.LogWarning("AddJSComponent() 2nd param must be string");
            return true;
        }

        string jsScriptName = JSApi.JSh_ArgvStringS(cx, vp, 1);

        JSComponent jsComp = go.AddComponent<JSComponent>();
        jsComp.jsScriptName = jsScriptName;
        jsComp.Awake();
        JSApi.JSh_SetRvalObject(cx, vp, jsComp.go);
        return true;
    }
    /*
     * 'RemoveJSComponent'
     * Remove a js script component from GameObject
     */
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool RemoveJSComponent(IntPtr cx, uint argc, IntPtr vp)
    {
        if (argc != 1 && argc != 2)
            return false;

        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, 0);
        object csObj = JSMgr.getCSObj(jsObj);
        if (csObj == null || !(csObj is GameObject))
            return false;

        GameObject go = (GameObject)csObj;

        if (argc == 1)
        {
            JSComponent jsComp = go.GetComponent<JSComponent>();
            if (jsComp != null)
                UnityEngine.Object.Destroy(jsComp);
            return true;
        }
        else
        {
            if (!JSApi.JSh_ArgvIsString(cx, vp, 1))
                return false;

            string jsScriptName = JSApi.JSh_ArgvStringS(cx, vp, 1);
            JSComponent[] jsComps = go.GetComponents<JSComponent>();
            foreach (JSComponent v in jsComps)
            {
                if (v.jsScriptName == jsScriptName)
                {
                    UnityEngine.Object.Destroy(v);
                    return true;
                }
            }
            return true;
        }
    }
    /*
     * 'GetJSComponent'
     * Get a js script component from GameObject
     * Similar to GameObject.GetComponent
     */
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool GetJSComponent(IntPtr cx, uint argc, IntPtr vp)
    {
        if (argc != 1 && argc != 2)
            return false;

        IntPtr jsObj = JSApi.JSh_ArgvObject(cx, vp, 0);
        object csObj = JSMgr.getCSObj(jsObj);
        if (csObj == null || !(csObj is GameObject))
            return false;

        GameObject go = (GameObject)csObj;

        if (argc == 1)
        {
            JSComponent jsComp = go.GetComponent<JSComponent>();
            if (jsComp == null)
                JSApi.JSh_SetRvalUndefined(cx, vp);
            else
                JSApi.JSh_SetRvalObject(cx, vp, jsComp.go);
            return true;
        }
        else
        {
            if (!JSApi.JSh_ArgvIsString(cx, vp, 1))
                return false;

            string jsScriptName = JSApi.JSh_ArgvStringS(cx, vp, 1);
            JSComponent[] jsComps = go.GetComponents<JSComponent>();
            foreach (var v in jsComps)
            {
                if (v.jsScriptName == jsScriptName)
                {
                    JSApi.JSh_SetRvalObject(cx, vp, v.go);
                    return true;
                }
            }
            JSApi.JSh_SetRvalUndefined(cx, vp);
            return true;
        }
    }

    /*
     * 'require'
     * Execute a js script in js code
     * can't require a script for different obj
     */
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool require(IntPtr cx, uint argc, IntPtr vp)
    {
        if (argc != 1 && argc != 2)
            return true;
        if (!JSApi.JSh_ArgvIsString(cx, vp, 0))
            return true;

        string jsScriptName = JSApi.JSh_ArgvStringS(cx, vp, 0);

        IntPtr obj = JSMgr.glob;
        if (argc == 2)
        {
            obj = JSApi.JSh_ArgvObject(cx, vp, 1);
            if (obj == IntPtr.Zero)
            {
                // obj is not valid c# object
                Debug.LogError("ERROR CS.require(" + jsScriptName + ", undefined)");
                return true;
            }
        }

        IntPtr ptrScript = GetCompiledScript(jsScriptName);
        if (ptrScript == IntPtr.Zero)
        {
            // require must load js SYNC
            string fullName = JSMgr.getJSFullName(jsScriptName, false);
            byte[] bytes = jsLoader.LoadJSSync(fullName);
            ptrScript = JSMgr.CompileScriptContentByte(jsScriptName, bytes, obj, fullName);
            if (ptrScript == IntPtr.Zero)
            {
                JSApi.JSh_SetRvalBool(cx, vp, false);
                return true;
            }
        }

        bool b = JSMgr.ExecuteScriptGlobal(ptrScript, obj);
        JSApi.JSh_SetRvalBool(cx, vp, b);
        return true;
    }

    // same as require
    public static bool ExecuteFile(string shortName)
    {
        IntPtr ptrScript = GetCompiledScript(shortName);
        if (ptrScript == IntPtr.Zero)
        {
            // require must load js SYNC
            string fullName = JSMgr.getJSFullName(shortName, false);
            byte[] bytes = jsLoader.LoadJSSync(fullName);
            ptrScript = JSMgr.CompileScriptContentByte(shortName, bytes, JSMgr.glob, fullName);
            if (ptrScript == IntPtr.Zero)
            {
                return false;
            }
        }
        return JSMgr.ExecuteScriptGlobal(ptrScript, JSMgr.glob);
    }
    

    /*
     * Create the 'CS' global object
     */
    public static void RegisterCS(IntPtr cx, IntPtr glob)
    {
        IntPtr jsClass = JSApi.JSh_NewClass("CS", 0, null);
        IntPtr obj = JSApi.JSh_InitClass(cx, glob, jsClass);

        JSApi.JSh_DefineFunction(cx, obj, "Call", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(Call)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, obj, "AddJSComponent", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(AddJSComponent)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, obj, "GetJSComponent", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(GetJSComponent)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, obj, "RemoveJSComponent", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(RemoveJSComponent)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, obj, "require", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(require)), 0/* narg */, 0);
        JSApi.JSh_DefineFunction(cx, obj, "SetErrorReporter", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(SetErrorReporter)), 0/* narg */, 0);
        
        CSOBJ = obj;
    }

    public static void JS_GC()
    {
        JSApi.JSh_GC(rt);
    }

    /*
     * record js/cs relation
     */
    class JS_CS_Relation
    {
        public IntPtr jsObj;
        public object csObj;
        public int csHashCode;
        public string name;
        public JS_CS_Relation(IntPtr a, object b)
        {
            jsObj = a;
            csObj = b;
            csHashCode = csObj.GetHashCode();
            name = csObj.ToString();
        }
    }

    public static void addJSCSRelation(IntPtr jsObj, object csObj)
    {
        //         jsval val = new jsval();
        //         JSApi.JSh_SetJsvalInt(ref val, index);
        //         JSApi.JS_SetProperty(cx, jsObj, "__resourceID", ref val);
        if (mDict1.ContainsKey(jsObj.ToInt64()))
            Debug.LogError("mDict1 already contains key for: " + jsObj.ToString());

        mDict1.Add(jsObj.ToInt64(), new JS_CS_Relation(jsObj, csObj));

        //         if (!csObj.GetType().IsValueType)
        //         {
        //             mDict2.Add(csObj.GetHashCode(), new JS_CS_Relation(jsObj, csObj));
        //         }
        //Debug.Log("+jsObj " + jsObj.ToString() +" "+ (mDict1.Count).ToString() + " " + csObj.GetType().Name + "/" + (typeof(UnityEngine.Object).IsAssignableFrom(csObj.GetType()) ? ((UnityEngine.Object)csObj).name : ""));
    }
    public static object getCSObj(IntPtr jsObj)
    {
        JS_CS_Relation obj;
        if (mDict1.TryGetValue(jsObj.ToInt64(), out obj))
            return obj.csObj;
        return null;
    }
    //     public static IntPtr getJSObj(object csObj)
    //     {
    //         if (csObj.GetType().IsValueType)
    //             return IntPtr.Zero;
    // 
    //         JS_CS_Relation obj;
    //         if (mDict2.TryGetValue(csObj.GetHashCode(), out obj))
    //             return obj.jsObj;
    //         return IntPtr.Zero;
    //     }
    public static void changeJSObj(IntPtr jsObj, object csObjNew)
    {
        mDict1.Remove(jsObj.ToInt64());
        addJSCSRelation(jsObj, csObjNew);
    }
    //     public static void changeCSObj(object csObj, object csObjNew)
    //     {
    //         IntPtr jsObj = getJSObj(csObj);
    //         if (jsObj == IntPtr.Zero)
    //             return;
    // 
    //         mDict1.Remove(jsObj.ToInt64());
    //         mDict2.Remove(csObj.GetHashCode());
    //         addJSCSRelation(jsObj, csObjNew);
    //     }
    public static void ClearJSCSRelation() {
        mDict1.Clear();
    }
    static Dictionary<long, JS_CS_Relation> mDict1 = new Dictionary<long, JS_CS_Relation>(); // key = jsObj.hashCode()

    // dict2 stores hashCode as key, may cause problems (2 object may share same hashCode)
    // but if use object as key, after calling 'UnityObject.Destroy(this)' in js, 
    // can't remove element from mDict2 due to csObj is null (JSObjectFinalizer)
    //static Dictionary<int, JS_CS_Relation> mDict2 = new Dictionary<int, JS_CS_Relation>(); // key = nativeObj.hashCode()

    [MonoPInvokeCallbackAttribute(typeof(JSApi.SC_FINALIZE))]
    static void JSObjectFinalizer(IntPtr freeOp, IntPtr jsObj)
    {
        JS_CS_Relation obj;
        if (mDict1.TryGetValue(jsObj.ToInt64(), out obj))
        {
            mDict1.Remove(jsObj.ToInt64());
            //mDict2.Remove(obj.csHashCode);
        }
        else
        {
            // Debug.LogError("Finalizer: csObj not found: " + jsObj.ToInt64().ToString());
        }
//         if (obj != null)
//             Debug.Log("-jsObj " + (mDict1.Count).ToString() + " " + obj.name);

        //if (mDict1.Count != mDict2.Count)
        //    Debug.LogError("JSObjectFinalizer / mDict1.Count != mDict2.Count");
    }

    /*
     * record registered types
     */
    public class GlobalType
    {
        public IntPtr jsClass; // JSClass*
        public IntPtr proto;   // JSObject* returned by JS_InitClass
        public IntPtr parentProto; // JSObject* parent
    }
    public static void addGlobalType(Type type, IntPtr jsClass, IntPtr proto, IntPtr parentProto)
    {
        int hash = type.GetHashCode();
        GlobalType gt = new GlobalType();
        gt.jsClass = jsClass;
        gt.proto = proto;
        gt.parentProto = parentProto;
        mGlobalType.Add(hash, gt);
    }
    public static GlobalType getGlobalType(Type type)
    {
        GlobalType gt;
        if (mGlobalType.TryGetValue(type.GetHashCode(), out gt))
        {
            return gt;
        }
        return null;
    }
    static Dictionary<int, GlobalType> mGlobalType = new Dictionary<int, GlobalType>();
}
