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
    static IntPtr jsErrorReporter = IntPtr.Zero;
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSErrorReporter))]
    static int errorReporter(IntPtr cx, string message, IntPtr report)
    {
        string fileName = JSApi.getErroReportFileNameS(report);
        int lineno = JSApi.getErroReportLineNo(report);
        string str = fileName + "(" + lineno.ToString() + "): " + message;
        Debug.LogWarning(str);
        return 1;
    }

    // load generated js files
    public delegate void OnInitJSEngine(bool bSuccess);
    public static OnInitJSEngine onInitJSEngine;
    static int loadedGeneratedJS = 0;
    static string[] jsGeneratedFileNames;
//    public static void OnLoadGeneratedJS(string shortName, byte[] bytes, string fullName)
//    {
//        if (bytes == null) {
//            //onInitJSEngine(false);
//            return;
//        }
//
//        IntPtr ptrScript = JSMgr.CompileScriptContentByte(shortName, bytes, JSMgr.glob, fullName);
//        if (ptrScript == IntPtr.Zero)
//        {
//            return;
//        }
//        JSMgr.ExecuteScript(ptrScript, JSMgr.glob);
//
//        loadedGeneratedJS++;
//        if (loadedGeneratedJS >= jsGeneratedFileNames.Length) {
//            onInitJSEngine(true);
//        }
//    }

    // load js files
    public delegate void OnLoadJS(IntPtr ptrScript);
//    public static void LoadJS(string shortName, OnLoadJS onLoadJS)
//    {
//        IntPtr ptrScript = JSMgr.GetCompiledScript(shortName);
//        if (ptrScript != IntPtr.Zero)
//        {
//            onLoadJS(ptrScript);
//            return;
//        }
//        else
//        {
//            JSFileLoader.OnLoadJS action = (shName, bytes, fullName) =>
//            {
//                // find again
//                ptrScript = JSMgr.GetCompiledScript(shortName);
//                if (ptrScript != IntPtr.Zero)
//                {
//                    onLoadJS(ptrScript);
//                    return;
//                }
//                ptrScript = JSMgr.CompileScriptContentByte(shName, bytes, JSMgr.glob, fullName);
//                onLoadJS(ptrScript);
//            };
//            jsLoader.LoadJSSync(shortName, false, action);
//        }
//    }

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

    static JSFileLoader jsLoader;
    public static bool InitJSEngine(JSFileLoader jsLoader, OnInitJSEngine onInitJSEngine)
    {
        int initResult = JSApi.InitJSEngine(new JSApi.JSErrorReporter(errorReporter), new JSApi.CSEntry(JSMgr.CSEntry), new JSApi.JSNative(require));
        if (initResult != 0)
        {
            Debug.LogError("InitJSEngine fail. error = " + initResult);
            onInitJSEngine(false);
            return false;
        }
        //JSApi.SetCSEntry(new JSApi.CSEntry(JSMgr.CSEntry));

        // 
        // move to C
        //JSMgr.RegisterCS(cx, glob);
        //
        RefCallStaticMethod("CSharpGenerated", "RegisterAll");
        JSMgr.jsLoader = jsLoader;

        onInitJSEngine(true);
        return true;
    }
    // old code
    public static bool InitJSEngine2(JSFileLoader jsLoader, OnInitJSEngine onInitJSEngine)
    {
//         if (!JSApi.JSh_Init())
//         {
//             onInitJSEngine(false);
//             return false;
//         }
// 
//         rt = JSApi.JSh_NewRuntime(8 * 1024 * 1024, 0);
//         JSApi.JSh_SetNativeStackQuota(rt, 500000, 0, 0);
// 
//         cx = JSApi.JSh_NewContext(rt, 8192);
//         JSApi.JSh_SetErrorReporter(cx, new JSApi.JSErrorReporter(errorReporter));
// 
//         glob = JSApi.JSh_NewGlobalObject(cx, 1);
//         JSApi.InitPersistentObject(rt, cx, glob, mjsFinalizer);
// 
//         oldCompartment = JSApi.JSh_EnterCompartment(cx, glob);
// 
//         if (!JSApi.JSh_InitStandardClasses(cx, glob))
//         {
//             Debug.LogError("JSh_InitStandardClasses fail. Make sure JSh_SetNativeStackQuota was called.");
//             onInitJSEngine(false);
//             return false;
//         }
// 
//         JSApi.JSh_InitReflect(cx, glob);
//         JSApi.JSh_SetGCCallback(rt, jsGCCallback, IntPtr.Zero);
// 
//         JSMgr.RegisterCS(cx, glob);
//         RefCallStaticMethod("CSharpGenerated", "RegisterAll");
//         JSMgr.jsLoader = jsLoader;
// 
//         onInitJSEngine(true);

        return true;
    }
    public static JSApi.JSFinalizeOp mjsFinalizer = new JSApi.JSFinalizeOp(JSObjectFinalizer);

    public static bool ShutDown = false;
    public static bool isShutDown { get { return ShutDown; } }
    public static void ShutdownJSEngine()
    {
        // TODO
        JSMgr.ClearJSCSRelation2();
//        JSMgr.ClearRootedObject();
        //JSMgr.ClearCompiledScript();

        JSApi.ShutdownJSEngine();

        ShutDown = true;
    }
    // old code
//     public static void ShutdownJSEngine()
//     {
//         JSApi.JSh_LeaveCompartment(cx, oldCompartment);
// 
// 
//         JSMgr.ClearJSCSRelation();
//         JSMgr.ClearRootedObject();
//         JSMgr.ClearCompiledScript();
// 
//         JSApi.JSh_DestroyContext(cx);
//         JSApi.JSh_DestroyRuntime(rt);
//         JSApi.JSh_ShutDown();
//         ShutDown = true;
//     }


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
    public class IntPtrClass
    {
        public IntPtrClass(IntPtr ptr) 
        {
            // this.pptr = JSApi.JSh_NewPPointer(ptr); 
            this.pptr = IntPtr.Zero; 
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

//    public static IntPtr CompileScriptContentByte(string shortName, byte[] content, IntPtr obj, string fullName)
//    {
//        if (content == null)
//        {
//            Debug.LogError(shortName + "file content is null");
//            return IntPtr.Zero;
//        }
//        else if (content.Length == 0)
//        {
//            Debug.LogError(shortName + "file content length = 0");
//            return IntPtr.Zero;
//        }
//
//        IntPtr ptr = JSApi.JSh_CompileScript(cx, obj, content, (uint)content.Length, fullName, 1);
//        IntPtrClass ptrClass = new IntPtrClass(ptr);
//        bool b = JSApi.JSh_AddNamedScriptRoot(JSMgr.cx, ptrClass.pptr, shortName);
//        if (!b) Debug.LogWarning("JSh_AddNamedScriptRoot fail!!");
//        //Debug.Log("``````" + shortName);
//        compiledScript.Add(shortName, ptrClass);
//        return ptr;
//    }
    // execute script even if the same script has been executed before
//     public static bool ExecuteScript(IntPtr ptrScript, IntPtr obj)
//     {
//         jsval val = new jsval();
//         return JSApi.JSh_ExecuteScript(cx, obj, ptrScript, ref val);
//     }
    // execute script only if the script not executed with the same obj
    // but executing script for different objects is allowed
    // ONLY for 'CS.require' use
//    public static bool ExecuteScriptGlobal(IntPtr ptrScript, IntPtr obj)
//    {
//        IntPtr ptrObj;
//        if (executedScript.TryGetValue(ptrScript, out ptrObj))
//        {
//            if (ptrObj == obj)
//                return true;
//        }
//
//        jsval val = new jsval();
//		executedScript.Add(ptrScript, obj);
//        bool b = JSApi.JSh_ExecuteScript(cx, obj, ptrScript, ref val);
//        if (!b) 
//		{
//			executedScript.Remove(ptrScript);
//			return false;
//		}
//        return true;
//    }
    /*
     * Get Compiled Script ptr by short name
     */
//    static IntPtr GetCompiledScript(string shortName)
//    {
//        IntPtrClass ptrClass = null;
//        if (compiledScript.TryGetValue(shortName, out ptrClass))
//            return ptrClass.ptr;
//        return IntPtr.Zero;
//    }

//    static void ClearCompiledScript()
//    {
//        foreach (var sc in compiledScript)
//        {
//            JSApi.JSh_RemoveScriptRoot(cx, sc.Value.pptr);
//            JSApi.JSh_DelPPointer(sc.Value.pptr);
//        }
//        compiledScript.Clear();
//    } 

    // TODO
    // delete these 2 dict
//    static Dictionary<string, IntPtrClass> compiledScript = new Dictionary<string, IntPtrClass>();
//    static Dictionary<IntPtr, IntPtr> executedScript = new Dictionary<IntPtr, IntPtr>();
    static Dictionary<string, bool> evaluatedScript = new Dictionary<string, bool>();

    // TODO 
    // delete
//    public static void AddRootedObject(IntPtr obj)
//    {
//        if (!rootedObject.ContainsKey(obj.ToInt64()))
//        {
//            IntPtrClass cls = new IntPtrClass(obj);
//            JSApi.JSh_AddObjectRoot(JSMgr.cx, cls.pptr);
//            rootedObject.Add(obj.ToInt64(), cls);
//        }
//    }
//    public static void RemoveRootedObject(IntPtr obj)
//    {
//        IntPtrClass cls;
//        if (rootedObject.TryGetValue(obj.ToInt64(), out cls))
//        {
//            JSApi.JSh_RemoveObjectRoot(JSMgr.cx, cls.pptr);
//            JSApi.JSh_DelPPointer(cls.pptr);
//            rootedObject.Remove(obj.ToInt64());
//        }
//    }
//    static void ClearRootedObject()
//    {
//        foreach (var sc in rootedObject)
//        {
//            JSApi.JSh_RemoveObjectRoot(JSMgr.cx, sc.Value.pptr);
//            JSApi.JSh_DelPPointer(sc.Value.pptr);
//        }
//        rootedObject.Clear();
//    }
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

    ////////
    ////////
    ////////
    // used for generic methods
    public static Dictionary<Type, MethodInfo[]> dictTypeMI;
    public static MethodInfo RuntimeGetMethodInfo(Type type, int i)
    {
        if (dictTypeMI == null)
        {
            dictTypeMI = new Dictionary<Type, MethodInfo[]>();
        }
        MethodInfo[] arrMI;
        if (!dictTypeMI.TryGetValue(type, out arrMI))
        {
            // flags must be the same as AddATypeInfo
            arrMI = type.GetMethods(JSMgr.BindingFlagsMethod);
            dictTypeMI.Add(type, arrMI);
        }
        if (i < arrMI.Length)
        {
            return arrMI[i];
        }
        else
        {
            Debug.LogError("RuntimeGetMethodInfo ERROR: index = " + i.ToString() + " out of range");
            return null;
        }
    }



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
        public int[] fieldsIndex;

        public PropertyInfo[] properties;
        public int[] propertiesIndex;

        public ConstructorInfo[] constructors;
        public int[] constructorsIndex;

        public MethodInfo[] methods;
        public int[] methodsIndex; // index of the method in array of type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

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

    public static BindingFlags BindingFlagsMethod = 
        BindingFlags.Public 
        | BindingFlags.Instance 
        | BindingFlags.Static 
        | BindingFlags.DeclaredOnly;

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

    public static int AddTypeInfo(Type type, out ATypeInfo tiOut)
    {
        ATypeInfo ti = new ATypeInfo();
        ti.fields = type.GetFields(BindingFlagsField);
        ti.properties = type.GetProperties(BindingFlagsProperty);
        ti.methods = type.GetMethods(BindingFlagsMethod);
        ti.constructors = type.GetConstructors();
        if (JSBindingSettings.NeedGenDefaultConstructor(type))
        {
            // 表示默认构造函数！
            var l = new List<ConstructorInfo>();
            l.Add(null);
            l.AddRange(ti.constructors);
            ti.constructors = l.ToArray();
        }
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
    public static int MethodInfoComparison(MethodInfoAndIndex mi1, MethodInfoAndIndex mi2)
    {
        MethodInfo m1 = mi1.method;
        MethodInfo m2 = mi2.method;

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

    public struct FieldInfoAndIndex
    {
        public FieldInfo method;
        public int index;
        public FieldInfoAndIndex(FieldInfo _m, int _i) { method = _m; index = _i; }
    }
    public struct MethodInfoAndIndex
    {
        public MethodInfo method;
        public int index;
        public MethodInfoAndIndex(MethodInfo _m, int _i) { method = _m; index = _i; }
    }
    public struct ConstructorInfoAndIndex
    {
        public ConstructorInfo method;
        public int index;
        public ConstructorInfoAndIndex(ConstructorInfo _m, int _i) { method = _m; index = _i; }
    }
    public struct PropertyInfoAndIndex
    {
        public PropertyInfo method;
        public int index;
        public PropertyInfoAndIndex(PropertyInfo _m, int _i) { method = _m; index = _i; }
    }
    public static void FilterTypeInfo(Type type, ATypeInfo ti)
    {
        bool bIsStaticClass = (type.IsClass && type.IsAbstract && type.IsSealed);

        List<ConstructorInfoAndIndex> lstCons = new List<ConstructorInfoAndIndex>();
        List<FieldInfoAndIndex> lstField = new List<FieldInfoAndIndex>();
        List<PropertyInfoAndIndex> lstPro = new List<PropertyInfoAndIndex>();
        Dictionary<string, int> proAccessors = new Dictionary<string, int>();
        List<MethodInfoAndIndex> lstMethod = new List<MethodInfoAndIndex>();

        for (int i = 0; i < ti.constructors.Length; i++)
        {
            if (ti.constructors[i] == null)
            {
                lstCons.Add(new ConstructorInfoAndIndex(null, i));
                continue;
            }

            // don't generate MonoBehaviour constructor
            if (type == typeof(UnityEngine.MonoBehaviour)) { 
                continue;  
            }

            if (!IsMemberObsolete(ti.constructors[i]) && !JSBindingSettings.IsDiscard(type, ti.constructors[i]))
            {
                lstCons.Add(new ConstructorInfoAndIndex(ti.constructors[i], i));
            }
        }

        for (int i = 0; i < ti.fields.Length; i++)
        {
            if (typeof(System.Delegate).IsAssignableFrom(ti.fields[i].FieldType.BaseType))
            {
                //Debug.Log("[field]" + type.ToString() + "." + ti.fields[i].Name + "is delegate!");
            }
            if (ti.fields[i].FieldType.ContainsGenericParameters)
                continue;

            if (!IsMemberObsolete(ti.fields[i]) && !JSBindingSettings.IsDiscard(type, ti.fields[i]))
            {
                lstField.Add(new FieldInfoAndIndex(ti.fields[i], i));
            }
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

            lstPro.Add(new PropertyInfoAndIndex(pro, i));
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
            {
                continue;
            }

            if (method.IsSpecialName)
            {
                if (method.Name == "op_Addition" ||
                    method.Name == "op_Subtraction" ||
                    method.Name == "op_UnaryNegation" ||
                    method.Name == "op_Multiply" ||
                    method.Name == "op_Division" ||
                    method.Name == "op_Equality" ||
                    method.Name == "op_Inequality" ||

                    method.Name == "op_LessThan" ||
                    method.Name == "op_LessThanOrEqual" ||
                    method.Name == "op_GreaterThan" ||
                    method.Name == "op_GreaterThanOrEqual" ||
                    
                    method.Name == "op_Implicit")
                {
                    if (!method.IsStatic)
                    {
                        Debug.LogWarning("IGNORE not-static special-name function: " + type.Name + "." + method.Name);
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

            //
            // ignore static method who contains T coming from class type
            // because there is no way to call it
            // SharpKit doesn't give c# the type of T
            //
            if (method.IsGenericMethodDefinition /* || method.IsGenericMethod*/
                && method.IsStatic)
            {
                bool bDiscard = false;

                 var ps = method.GetParameters();
                 for (int k = 0; k < ps.Length; k++)
                 {
                     if (ps[k].ParameterType.ContainsGenericParameters) 
                     {
                         var Ts = JSDataExchangeMgr.RecursivelyGetGenericParameters(ps[k].ParameterType);
                         foreach (var t in Ts)
                         {
                             if (t.DeclaringMethod == null) 
                             { 
                                 bDiscard = true; 
                                 break; 
                             }
                         }
                         if (bDiscard)
                            break;
                     }
                 }
                 if (bDiscard)
                 {
                     Debug.LogWarning("Ignore static method " + type.Name + "." + method.Name);
                     continue;
                 }
            }


            if (JSBindingSettings.IsDiscard(type, method))
                continue;

            lstMethod.Add(new MethodInfoAndIndex(method, i));
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

            if (i < lstMethod.Count - 1 && lstMethod[i].method.Name == lstMethod[i + 1].method.Name &&
                ((lstMethod[i].method.IsStatic && lstMethod[i + 1].method.IsStatic) ||
                (!lstMethod[i].method.IsStatic && !lstMethod[i + 1].method.IsStatic)))
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

        ti.constructors = new ConstructorInfo[lstCons.Count];
        ti.constructorsIndex = new int[lstCons.Count];
        for (var k = 0; k < lstCons.Count; k++)
        {
            ti.constructors[k] = lstCons[k].method;
            ti.constructorsIndex[k] = lstCons[k].index;
        }

        // ti.fields = lstField.ToArray();
        ti.fields = new FieldInfo[lstField.Count];
        ti.fieldsIndex = new int[lstField.Count];
        for (var k = 0; k < lstField.Count; k++)
        {
            ti.fields[k] = lstField[k].method;
            ti.fieldsIndex[k] = lstField[k].index;
        }

        // ti.properties = lstPro.ToArray();
        ti.properties = new PropertyInfo[lstPro.Count];
        ti.propertiesIndex = new int[lstPro.Count];
        for (var k = 0; k < lstPro.Count; k++)
        {
            ti.properties[k] = lstPro[k].method;
            ti.propertiesIndex[k] = lstPro[k].index;
        }

        ti.methods = new MethodInfo[lstMethod.Count];
        ti.methodsIndex = new int[lstMethod.Count];

        for (var k = 0; k < lstMethod.Count; k++)
        {
            ti.methods[k] = lstMethod[k].method;
            ti.methodsIndex[k] = lstMethod[k].index;
        }
    }

    // TODO delete
    //public static IntPtr CSOBJ = IntPtr.Zero;
    public static JSVCall vCall = new JSVCall();

    [MonoPInvokeCallbackAttribute(typeof(JSApi.CSEntry))]
    static bool CSEntry(int iOP, int slot, int index, bool isStatic, int argc)
    {
        if (JSMgr.isShutDown) return false; 
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
            return false;
        }

        return true;
    }
    /*
     * 'Call'
     * This is the entry for calling cs from js
     * 99% js calls comes here
     */
//     [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
//     static bool Call(IntPtr cx, uint argc, IntPtr vp)
//     {
//         //         if (useReflection)
//         //             return vCall.CallReflection(cx, argc, vp);
//         //         else
// 
//         if (JSMgr.isShutDown) return false;
//         try
//         {
//             vCall.CallCallback(cx, argc, vp);
//         }
//         catch (System.Exception ex)
//         {
//             /* 
//              * if exception occurs, catch it, pass the error to js, and return false
//              * js then print the error string and js call stack
//              * note: the error contains cs call stack, so now we have both cs and js call stack
//              */
//             JSApi.JSh_ReportError(cx, ex.ToString());
//             return false;
//         }
//         
//         return true;
//     }


    public static bool evaluate(string jsScriptName)
    {
        if (evaluatedScript.ContainsKey(jsScriptName))
        {
            return true;
        }
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

        bool ret = JSApi.evaluate(bytes, (uint)bytes.Length, fullName);
        return ret;
    }

    /*
     * 'require'
     * Execute a js script in js code
     * can't require a script for different obj
     */
    // TODO modify
    // TODO getJSFullName true/false 会不会有问题呢
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSNative))]
    static bool require(IntPtr cx, uint argc, IntPtr vp)
    {
        string jsScriptName = JSApi.getArgStringS(vp, 0);
        bool ret = evaluate(jsScriptName);
        JSApi.setRvalBool(vp, ret);
        return true;

// 
//         if (argc != 1 && argc != 2)
//             return true;
//         var tag = JSApi.JSh_ArgvTag(cx, vp, 0);
//         if (!jsval.isString(tag))
//             return true;
// 
//         string jsScriptName = JSApi.JSh_ArgvStringS(cx, vp, 0);
// 
//         IntPtr obj = JSMgr.glob;
//         if (argc == 2)
//         {
//             obj = JSApi.JSh_ArgvObject(cx, vp, 1);
//             if (obj == IntPtr.Zero)
//             {
//                 // obj is not valid c# object
//                 Debug.LogError("ERROR CS.require(" + jsScriptName + ", undefined)");
//                 return true;
//             }
//         }
//         IntPtr ptrScript = GetCompiledScript(jsScriptName);
//         if (ptrScript == IntPtr.Zero)
//         {
//             // require must load js SYNC
//             string fullName = JSMgr.getJSFullName(jsScriptName, false);
//             byte[] bytes = jsLoader.LoadJSSync(fullName);
//             ptrScript = JSMgr.CompileScriptContentByte(jsScriptName, bytes, obj, fullName);
//             if (ptrScript == IntPtr.Zero)
//             {
//                 JSApi.JSh_SetRvalBool(cx, vp, false);
//                 return true;
//             }
//         }
// 
//         bool b = JSMgr.ExecuteScriptGlobal(ptrScript, obj);
//         JSApi.JSh_SetRvalBool(cx, vp, b);
//         return b;
    }

    // same as require
//     public static bool ExecuteFile(string shortName)
//     {
//         IntPtr ptrScript = GetCompiledScript(shortName);
//         if (ptrScript == IntPtr.Zero)
//         {
//             // require must load js SYNC
//             string fullName = JSMgr.getJSFullName(shortName, false);
//             byte[] bytes = jsLoader.LoadJSSync(fullName);
//             ptrScript = JSMgr.CompileScriptContentByte(shortName, bytes, JSMgr.glob, fullName);
//             if (ptrScript == IntPtr.Zero)
//             {
//                 return false;
//             }
//         }
//         return JSMgr.ExecuteScriptGlobal(ptrScript, JSMgr.glob);
       

    /*
     * Create the 'CS' global object
     */
//     public static void RegisterCS(IntPtr cx, IntPtr glob)
//     {
//         IntPtr jsClass = JSApi.JSh_NewClass("CS", 0, null);
//         IntPtr obj = JSApi.JSh_InitClass(cx, glob, jsClass);
// 
//         JSApi.JSh_DefineFunction(cx, obj, "Call", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(Call)), 0/* narg */, 0);
//         JSApi.JSh_DefineFunction(cx, obj, "require", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(require)), 0/* narg */, 0);
//         JSApi.JSh_DefineFunction(cx, obj, "SetErrorReporter", Marshal.GetFunctionPointerForDelegate(new JSApi.JSNative(SetErrorReporter)), 0/* narg */, 0);
//         
//         CSOBJ = obj;
//     }

    /*
     * record js/cs relation
     */
    // TODO delete
    public class JS_CS_Relation
    {
        //
        // JS  jsObj
        //       -- __nativeObj
        //
        //  mDict1  __nativeObj -> csObj
        //  mDict2  csObj -> jsObj
        //
        //
        public IntPtr jsObj;   
        public object csObj;
        public string name;
        public int hash;
        public JS_CS_Relation(IntPtr _jsObj, object _csObj, int h)
        {
            jsObj = _jsObj;
            csObj = _csObj;
            name = csObj.GetType().Name;// csObj.ToString();
            this.hash = h;
        }
    }
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

    public static void AddJSCSRel(int jsObjID, object csObj)
    {
        int hash = csObj.GetHashCode();
        mDictionary1.Add(jsObjID, new JS_CS_Rel(jsObjID, csObj, hash));

        if (csObj.GetType().IsClass)
        {
            mDictionary2.Add(hash, new JS_CS_Rel(jsObjID, csObj, hash));
        }
    }

    // 
//     public static void addJSCSRelation(IntPtr jsObj, IntPtr nativeObj, object csObj)
//     {
//         JS_CS_Relation Rel;
//         if (mDict1.TryGetValue(nativeObj.ToInt64(), out Rel))
//         {
//             // Debug.LogWarning("mDict1 already contains key for: " + nativeObj.ToString());
//             // !!!!!
//             // when GC occurs . Same jsObj will get here before finalizer called
//             mDict1.Remove(nativeObj.ToInt64());
//             //if (Rel.csObj != null && Rel.csObj.GetType().IsClass)
//             // 直接尝试删，没删到也没事
//             mDict2.Remove(Rel.hash);
//         }
//         int hash = csObj.GetHashCode();
//         mDict1.Add(nativeObj.ToInt64(), new JS_CS_Relation(nativeObj, csObj, hash));
// 
//         if (csObj.GetType().IsClass) 
//         {
//             if (!mDict2.ContainsKey(hash))
//                 mDict2.Add(hash, new JS_CS_Relation(jsObj, csObj, hash));
//             else
//                 Debug.Log("");
//         }
//     }
    public static object getCSObj2(int jsObjID)
    {
        JS_CS_Rel obj;
        if (mDictionary1.TryGetValue(jsObjID, out obj))
            return obj.csObj;
        return null;
    }
    // TODO delete
//     public static object getCSObj(IntPtr nativeObj)
//     {
//         JS_CS_Relation obj;
//         if (mDict1.TryGetValue(nativeObj.ToInt64(), out obj))
//             return obj.csObj;
//         return null;
//     }
    public static int getJSObj2(object csObj)
    {
        if (csObj.GetType().IsValueType)
            return 0;

        JS_CS_Rel Rel;
        if (mDictionary2.TryGetValue(csObj.GetHashCode(), out Rel))
            return Rel.jsObjID;
        return 0;
    }
    // TODO delete
//     public static IntPtr getJSObj(object csObj)
//     {
//         if (csObj.GetType().IsValueType)
//             return IntPtr.Zero;
// 
//         JS_CS_Relation Rel;
//         if (mDict2.TryGetValue(csObj.GetHashCode(), out Rel))
//             return Rel.jsObj;
//         return IntPtr.Zero;
//     }
    // TODO
    // use it
    public static void changeJSObj2(int jsObjID, object csObjNew)
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
    // TODO delete
    public static void changeJSObj(IntPtr nativeObj, object csObjNew)
    {
        if (!csObjNew.GetType().IsValueType)
        {
            Debug.LogError("class can not call changeJSObj");
            return;
        }
        var Key = nativeObj.ToInt64();
        JS_CS_Relation obj;
        if (mDict1.TryGetValue(Key, out obj))
        {
            mDict1.Remove(Key);
            mDict1.Add(Key, new JS_CS_Relation(nativeObj, csObjNew, obj.GetHashCode()));
        }
//        addJSCSRelation(nativeObj, csObjNew);
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
    public static void ClearJSCSRelation2()
    {
        mDictionary1.Clear();
        mDictionary2.Clear();
    }
//     public static void ClearJSCSRelation() {
//         mDict1.Clear();
//         mDict2.Clear();
//     }
    // JS's __nativeObj -> CS csObj
    static Dictionary<long, JS_CS_Relation> mDict1 = new Dictionary<long, JS_CS_Relation>(); // key = jsObj.ToInt64()
    // CS csobj -> JS's jsObj
    // dict2 stores hashCode as key, may cause problems (2 object may share same hashCode)
    // but if use object as key, after calling 'UnityObject.Destroy(this)' in js, 
    // can't remove element from mDict2 due to csObj is null (JSObjectFinalizer)
    //
    // mDict2 存储 object.GetHashCode() -> jsObj
    // 这样有可能有问题，因为2个不同的对象可能会有相同的 hashCode
    // 但是如果使用 object 为 key，如果代码里调用了类似 Destroy(go) 的代码，导致 object 变为 null
    // 那么 mDict2 就无法再删除那个条目。(进入一种很奇怪的状态，托管不空，NATIVE为空的状况)
    // 
    static Dictionary<int, JS_CS_Relation> mDict2 = new Dictionary<int, JS_CS_Relation>(); // key = object.hashCode()

    static Dictionary<int, JS_CS_Rel> mDictionary1 = new Dictionary<int, JS_CS_Rel>(); // key = OBJID
    static Dictionary<int, JS_CS_Rel> mDictionary2 = new Dictionary<int, JS_CS_Rel>(); // key = object.GetHashCode()

    public static void GetDictCount(out int countDict1, out int countDict2)
    {
        countDict1 = mDictionary1.Count;
        countDict2 = mDictionary2.Count;
    }
    public static Dictionary<int, JS_CS_Rel> GetDict1() { return mDictionary1;  }

    // TODO delete
    [MonoPInvokeCallbackAttribute(typeof(JSApi.JSFinalizeOp))]
    static void JSObjectFinalizer(IntPtr freeOp, IntPtr nativeObj)
    {
        JS_CS_Relation Rel;
        if (mDict1.TryGetValue(nativeObj.ToInt64(), out Rel))
        {
            mDict1.Remove(nativeObj.ToInt64());
            //Debug.Log("GC: " + nativeObj.ToInt64() + " removed from mDict1");

            mDict2.Remove(Rel.hash);
        }
        else
        {
            //Debug.LogError("Finalizer: csObj not found: " + nativeObj.ToInt64().ToString());
        }
//         if (obj != null)
//             Debug.Log("-jsObj " + (mDict1.Count).ToString() + " " + obj.name);

        //if (mDict1.Count != mDict2.Count)
        //    Debug.LogError("JSObjectFinalizer / mDict1.Count != mDict2.Count");
    }

    /*
     * record registered types
     */
//    public class GlobalType
//    {
//        public IntPtr jsClass; // JSClass*
//        public IntPtr proto;   // JSObject* returned by JS_InitClass
//        public IntPtr parentProto; // JSObject* parent
//    }
//    public static void addGlobalType(Type type, IntPtr jsClass, IntPtr proto, IntPtr parentProto)
//    {
//        int hash = type.GetHashCode();
//        GlobalType gt = new GlobalType();
//        gt.jsClass = jsClass;
//        gt.proto = proto;
//        gt.parentProto = parentProto;
//        mGlobalType.Add(hash, gt);
//    }
//    public static GlobalType getGlobalType(Type type)
//    {
//        GlobalType gt;
//        if (mGlobalType.TryGetValue(type.GetHashCode(), out gt))
//        {
//            return gt;
//        }
//        return null;
//    }
//    static Dictionary<int, GlobalType> mGlobalType = new Dictionary<int, GlobalType>();
}
