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
        ShutDown = false;

        int initResult = JSApi.InitJSEngine(
            new JSApi.JSErrorReporter(errorReporter), 
            new JSApi.CSEntry(JSMgr.CSEntry),
            new JSApi.JSNative(require), 
            new JSApi.OnObjCollected(onObjCollected));

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
        ShutDown = true;
        allCallbackInfo.Clear();
        JSMgr.ClearJSCSRel();
        evaluatedScript.Clear();
        JSApi.ShutdownJSEngine();
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

    // TODO
    // delete these 2 dict
//    static Dictionary<string, IntPtrClass> compiledScript = new Dictionary<string, IntPtrClass>();
//    static Dictionary<IntPtr, IntPtr> executedScript = new Dictionary<IntPtr, IntPtr>();
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

    public static JSVCall vCall = new JSVCall();

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
        JSApi.setRvalBoolS(vp, ret);
        return true;
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

    public static void removeJSCSRel(int id)
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
//     public static void ClearJSCSRelation() {
//         mDict1.Clear();
//         mDict2.Clear();
//     }
    // JS's __nativeObj -> CS csObj
    //static Dictionary<long, JS_CS_Relation> mDict1 = new Dictionary<long, JS_CS_Relation>(); // key = jsObj.ToInt64()
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
    //static Dictionary<int, JS_CS_Relation> mDict2 = new Dictionary<int, JS_CS_Relation>(); // key = object.hashCode()

    static Dictionary<int, JS_CS_Rel> mDictionary1 = new Dictionary<int, JS_CS_Rel>(); // key = OBJID
    static Dictionary<int, JS_CS_Rel> mDictionary2 = new Dictionary<int, JS_CS_Rel>(); // key = object.GetHashCode()

    public static void GetDictCount(out int countDict1, out int countDict2)
    {
        countDict1 = mDictionary1.Count;
        countDict2 = mDictionary2.Count;
    }
    public static Dictionary<int, JS_CS_Rel> GetDict1() { return mDictionary1;  }
}
