using UnityEngine;

// public class SMDLL : MonoBehaviour {
// 
// 	// Use this for initialization
// 	void Start () {
// 	
// 	}
// 	
// 	// Update is called once per frame
// 	void Update () {
// 	
// 	}
// }

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;

//using jsval = System.UInt64;

public class JSApi
{

    /***********************************************************************
        *
        * Part I
        * Constants & Types & Delegates
        *    
    ************************************************************************/
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
    const string JSDll = "mozjswrap";
#elif UNITY_IPHONE
    const string JSDll = "__Internal";
#else
    const string JSDll = "mozjswrap";
#endif

    enum JSConstant
    {

    }

    public static int JS_TRUE = 1;
    public static int JS_FALSE = 0;

    public struct JSHandleObject { public IntPtr _; }
    public struct JSHandleId { public IntPtr _; }
    public struct JSMutableHandleValue { public IntPtr _; }

    //[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //public delegate int JSPROPERTYOP(IntPtr cx, JSHandleObject obj, JSHandleId id, JSMutableHandleValue vp);

    // 
    //     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate int JS_STRICTPROPERTYSTUB(IntPtr cx, JSHandleObject obj, JSHandleId id, int strict, JSMutableHandleValue vp);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern int JS_EnumerateStub(IntPtr cx, JSHandleObject obj);
    //     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate int JS_ENUMERATESTUB(IntPtr cx, JSHandleObject obj);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern int JS_ResolveStub(IntPtr cx, JSHandleObject obj, JSHandleId id);
    //     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate int JS_RESOLVESTUB(IntPtr cx, JSHandleObject obj, JSHandleId id);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern int JS_ConvertStub(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);
    //     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate int JS_CONVERTSTUB(IntPtr cx, JSHandleObject obj, int type, JSMutableHandleValue vp);

    public static void sc_finalize(IntPtr freeOp, IntPtr obj) { }
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate void SC_FINALIZE(IntPtr freeOp, IntPtr obj);

    //     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate int JSCheckAccessOp(IntPtr cx, JSHandleObject obj, JSHandleId id, int mode, IntPtr vp);

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate bool JSNative(IntPtr cx, uint argc, IntPtr vp);

    // CSEntry
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate bool CSEntry(int op, int slot, int index, bool bStatic, int argc);

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate void JSGCCallback(/*JSRuntime **/IntPtr rt, /*JSGCStatus*/int status, /*void **/ IntPtr data);
    

    //     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate int JSHasInstanceOp(IntPtr cx, JSHandleObject obj, int v, IntPtr bp);

    //     [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public delegate void JSTraceOp(IntPtr trc, IntPtr obj);

    /*[StructLayout(LayoutKind.Sequential)]
    public class JSClass
    {
        public string name;
        public uint flags;

        // Mandatory non-null function pointer members. 
        public JSPROPERTYOP addProperty;
        public JSPROPERTYOP delProperty;
        public JSPROPERTYOP getProperty;
        public JS_STRICTPROPERTYSTUB setProperty;
        public JS_ENUMERATESTUB enumerate;
        public JS_RESOLVESTUB resolve;
        public JS_CONVERTSTUB convert;
        public SC_FINALIZE finalize;

        // Optionally non-null members start here. 
        public JSCheckAccessOp checkAccess;
        public JSNative call;
        public JSHasInstanceOp hasInstance;
        public JSNative construct;
        public JSTraceOp trace;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public UInt64[] reserved;
    };*/


    /***********************************************************************
     
     * Part II
     * SpiderMonkey functions
     
    ************************************************************************/

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_Init();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewRuntime(UInt32 maxbytes, int useHelperThreads);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewContext(IntPtr rt, int stackChunkSize);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_InitStandardClasses(IntPtr cx, IntPtr obj);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_InitReflect(IntPtr cx, IntPtr global);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewObject(IntPtr cx, IntPtr clasp, IntPtr proto, IntPtr parent);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_DestroyContext(IntPtr cx);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_DestroyRuntime(IntPtr rt);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_ShutDown();

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern void JSh_Finish(IntPtr rt);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_InitClass(IntPtr cx, IntPtr obj, IntPtr clasp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_SetErrorReporter(IntPtr cx, JSErrorReporter er);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewArrayObject(IntPtr cx, int length, IntPtr vector);
    public static IntPtr JSh_NewArrayObjectS(IntPtr cx, int length) { return JSh_NewArrayObject(cx, length, IntPtr.Zero); }

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_IsArrayObject(IntPtr cx, IntPtr obj);

    // return -1: fail
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_GetArrayLength(IntPtr cx, IntPtr obj);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_GetElement(IntPtr cx, IntPtr obj, UInt32 index, ref jsval val);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetElement(IntPtr cx, IntPtr obj, uint index, ref jsval vp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_GetProperty(IntPtr cx, IntPtr obj, string name, ref jsval val);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_SetProperty(IntPtr cx, IntPtr obj, string name, ref jsval pVal);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern bool JSh_GetUCProperty(IntPtr cx, IntPtr obj, string name, int nameLen, ref jsval val);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern bool JSh_SetUCProperty(IntPtr cx, IntPtr obj, string name, int nameLen, ref jsval pVal);


    public static bool GetProperty(IntPtr cx, IntPtr obj, string name, int nameLen, ref jsval val)
    {
        return JSh_GetUCProperty(cx, obj, name, nameLen, ref val);

        if (!JSApi.JSh_IsArrayObject(cx, obj))
        {
            Debug.LogError(" NOT ARRAY OBJECT ");
            return false;
        }
        JSApi.JSh_GetElement(cx, obj, (uint)0, ref val);
        return true;
    }
    public static bool SetProperty(IntPtr cx, IntPtr obj, string name, int nameLen, ref jsval val)
    {
        return JSh_SetUCProperty(cx, obj, name, nameLen, ref val);

        if (!JSApi.JSh_IsArrayObject(cx, obj))
        {
            Debug.LogError(" NOT ARRAY OBJECT ");
            return false;
        }
        JSApi.JSh_SetElement(cx, obj, (uint)0, ref val);
        return true;
    }

    public delegate IntPtr JSh_NewGlobalObject_Del(IntPtr cx, IntPtr clasp, IntPtr principals);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern bool JSh_IsArrayObject(IntPtr cx, IntPtr obj);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewGlobalObject(IntPtr cx, int hookOption);

    

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern IntPtr JSh_InitClass(IntPtr cx, IntPtr jsClass);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_ThisObject(IntPtr cx, IntPtr vp);

    /* Remember to propagate changes to the C defines below. */

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern uint JSh_ArgvTag(IntPtr cx, IntPtr vp, int i);
    
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvBool(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern double JSh_ArgvDouble(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_ArgvInt(IntPtr cx, IntPtr vp, int i);

    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_ArgvObject(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_ArgvFunction(IntPtr cx, IntPtr vp, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ArgvFunctionValue(IntPtr cx, IntPtr vp, int i, ref jsval pval);

    /*
     * Return values to JavaScript
     */
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalBool(IntPtr cx, IntPtr vp, bool value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalDouble(IntPtr cx, IntPtr vp, double value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalInt(IntPtr cx, IntPtr vp, int value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalUInt(IntPtr cx, IntPtr vp, UInt32 value);

    
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalObject(IntPtr cx, IntPtr vp, IntPtr value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalUndefined(IntPtr cx, IntPtr vp);
    /*
     * 生成 jsval，有些函数需要传递 jsval，或者 jsval*
     */
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalBool(ref jsval vp, bool value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalDouble(ref jsval vp, double value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalInt(ref jsval vp, int value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalUInt(ref jsval vp, UInt32 value);

    
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalObject(ref jsval vp, IntPtr value);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetJsvalUndefined(ref jsval vp);


    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_GetJsvalBool(ref jsval vp);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern double JSh_GetJsvalDouble(ref jsval vp);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_GetJsvalInt(ref jsval vp);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern UInt32 JSh_GetJsvalUInt(ref jsval vp);

    
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_GetJsvalObject(ref jsval vp);


    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_SetRvalJSVAL(IntPtr cx, IntPtr vp, ref jsval value);

    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_GetErroReportLineNo(IntPtr report);

    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewMyClass(IntPtr cx, SC_FINALIZE finalizeOp);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern bool JSh_SetClassFinalize(IntPtr cx, string className, SC_FINALIZE finalizeOp);

    public enum JSValueType
    {
        JSVAL_TYPE_DOUBLE = 0x00,
        JSVAL_TYPE_INT32 = 0x01,
        JSVAL_TYPE_UNDEFINED = 0x02,
        JSVAL_TYPE_BOOLEAN = 0x03,
        JSVAL_TYPE_MAGIC = 0x04,
        JSVAL_TYPE_STRING = 0x05,
        JSVAL_TYPE_NULL = 0x06,
        JSVAL_TYPE_OBJECT = 0x07,

        /* These never appear in a jsval; they are only provided as an out-of-band value. */
        JSVAL_TYPE_UNKNOWN = 0x20,
        JSVAL_TYPE_MISSING = 0x21
    }

    public struct JSValueTag
    {
        public static uint JSVAL_TAG_CLEAR = 0xFFFFFF80;
        public static uint JSVAL_TAG_INT32 = JSVAL_TAG_CLEAR | (uint)JSValueType.JSVAL_TYPE_INT32;
        public static uint JSVAL_TAG_UNDEFINED = JSVAL_TAG_CLEAR | (uint)JSValueType.JSVAL_TYPE_UNDEFINED;
        public static uint JSVAL_TAG_STRING = JSVAL_TAG_CLEAR | (uint)JSValueType.JSVAL_TYPE_STRING;
        public static uint JSVAL_TAG_BOOLEAN = JSVAL_TAG_CLEAR | (uint)JSValueType.JSVAL_TYPE_BOOLEAN;
        public static uint JSVAL_TAG_MAGIC = JSVAL_TAG_CLEAR | (uint)JSValueType.JSVAL_TYPE_MAGIC;
        public static uint JSVAL_TAG_NULL = JSVAL_TAG_CLEAR | (uint)JSValueType.JSVAL_TYPE_NULL;
        public static uint JSVAL_TAG_OBJECT = JSVAL_TAG_CLEAR | (uint)JSValueType.JSVAL_TYPE_OBJECT;
    }
    public struct jsval
    {
        public UInt64 asBits;
        public static bool isUndefined(uint tag) { return JSValueTag.JSVAL_TAG_UNDEFINED == tag; }
        public static bool isNull(uint tag) { return JSValueTag.JSVAL_TAG_NULL == tag; }
        public static bool isNullOrUndefined(uint tag) { return isUndefined(tag) || isNull(tag); }
        public static bool isInt32(uint tag) { return JSValueTag.JSVAL_TAG_INT32 == tag; }
        public static bool isDouble(uint tag) { return JSValueTag.JSVAL_TAG_CLEAR >= tag; }
        public static bool isBoolean(uint tag) { return JSValueTag.JSVAL_TAG_BOOLEAN == tag; }
        public static bool isString(uint tag) { return JSValueTag.JSVAL_TAG_STRING == tag; }
        public static bool isNumber(uint tag) { return JSValueTag.JSVAL_TAG_INT32 >= tag; }
        public static bool isObject(uint tag) { return JSValueTag.JSVAL_TAG_OBJECT == tag; }


        public uint tag { 
            get {
                return (uint)(asBits >> 32);
            } 
        }
    }

    //
    // DONT use this function, it doesn't support Chinese or other non-english chars
    //
//     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public static extern int JSh_EvaluateScript(IntPtr cx, IntPtr obj,
//                  string bytes, uint length,
//                  string filename, uint lineno,
//                  ref jsval rval);


    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_ExecuteScript(IntPtr cx, IntPtr obj, IntPtr script, ref jsval rval);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern IntPtr JSh_ValueToString(IntPtr cx, jsval v);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern IntPtr JSh_EncodeString(IntPtr cx, IntPtr str);

    //     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //     public static extern int JSh_ValueToInt32(IntPtr cx, jsval v, ref int ip);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int JSh_GC(IntPtr rt);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_EnterCompartment(IntPtr cx, IntPtr target);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_LeaveCompartment(IntPtr cx, IntPtr oldCompartment);

    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_CallFunction(IntPtr cx, IntPtr obj, IntPtr fun, UInt32 argc,
        /*[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] */jsval[] argv,
        ref jsval rval);

    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_CallFunctionValue(IntPtr cx, IntPtr obj, ref jsval fval, UInt32 argc,
        /*[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] */jsval[] argv,
        ref jsval rval);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_AddObjectRoot(IntPtr cx, IntPtr/*JSObject***/ rp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_AddValueRoot(IntPtr cx, ref jsval val);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_RemoveObjectRoot(IntPtr cx, IntPtr/*JSObject***/ rp);

    

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_RemoveScriptRoot(IntPtr cx, IntPtr rp);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetNativeStackQuota(IntPtr rt, UInt32 systemCodeStackSize, UInt32 trustedScriptStackSize, UInt32 untrustedScriptStackSize);

    

	[DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void JSh_UpdateDebugger();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_CleanupDebugger();

    //[DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //public static extern void JSh_DumpBacktrace(IntPtr cx);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewPPointer(IntPtr value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_DelPPointer(IntPtr pp);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetPPointer(IntPtr pp, IntPtr value);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_GetPPointer(IntPtr pp);
    
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewHeapObject(/* JSObject* */IntPtr obj);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_DelHeapObject(/* JS::Heap<JSObject*>* */IntPtr heapObj);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_SetGCCallback(/*JSRuntime* */ IntPtr rt, JSGCCallback cb, IntPtr data);


    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void InitPersistentObject(IntPtr rt, IntPtr cx, IntPtr global, SC_FINALIZE finalizer);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int NewJSClassObject(string name);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool RemoveJSClassObject(int odjID);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool IsJSClassObjectFunctionExist(int objID, string functionName);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void SetVector2(IntPtr jsObj, float x, float y, /*JSObject**/ IntPtr objWrap);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void SetVector3(IntPtr jsObj, float x, float y, float z, /*JSObject**/ IntPtr objWrap);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int InitJSEngine(JSErrorReporter er, CSEntry csEntry, JSNative req);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void ShutdownJSEngine();
//     [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
//     public static extern void SetCSEntry(CSEntry entry);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern /*JSContext**/ IntPtr GetContext();
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern /*JSObject**/ IntPtr GetGlobal();
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern /*JSRuntime**/ IntPtr GetRuntime();




    public enum GetType
    {
        Arg = 0,
        ArgRef = 1,
        JSFunRet = 2,
        Jsval = 3,
    }
    public enum SetType
    {
        Rval = 0,
        UpdateArgRef = 1,
        Jsval = 2,
    }

    //////////////////////////////////////////////////////////////////////////
    // 先写上去吧。

    public static  int getCurrIndex();
    public static void setCurIndex(int i);
    public static uint argTag(int i);

    public static short getChar(GetType e);
    public static char getSByte(GetType e);
    public static byte getByte(GetType e);
    public static short getInt16(GetType e);
    public static ushort getUInt16(GetType e);
    public static int getInt32(GetType e);
    public static uint getUInt32(GetType e);
    public static long getInt64(GetType e);
    public static ulong getUInt64(GetType e);
    public static int getEnum(GetType e);
    public static float getSingle(GetType e);
    public static double getDouble(GetType e);
    public static long getIntPtr(GetType e);
    // TODO 如果e没填，就用 ARGV
    public static bool getBoolean(GetType e);
    public static IntPtr getString(GetType e);
    public static string getStringS(GetType e)
    {
        return Marshal.PtrToStringUni(getString(e));
    }

    public static  void setFloatPtr2(ref float f0, ref float f1);
    public static  void setFloatPtr3(ref float f0, ref float f1, ref float f2);
    public static bool getVector2(GetType e);
    public static Vector2 getVector2S(GetType e)
    {
        // TODO does this work?
        // ref v.x, ref v.y  <-- TODO is it OK?
        float x = 0, y = 0;
        setFloatPtr2(ref x, ref y);
        getVector2(e);
        return new Vector2(x, y);
    }
    public static bool getVector3(GetType e);
    public static Vector2 getVector3S(GetType e)
    {
        // TODO does this work?
        float x = 0, y = 0, z = 0;
        setFloatPtr3(ref x, ref y, ref z);
        getVector3(e);
        return new Vector3(x, y, z);
    }
    public static  int getObject(GetType e);

    // TODO 改成 (GetType e, ...)
    public static  void setChar    (SetType e, char v);
    public static  void setSByte   (SetType e, sbyte v);
    public static  void setByte    (SetType e, byte v);
    public static  void setInt16   (SetType e, short v);
    public static  void setUInt16  (SetType e, ushort v);
    public static  void setInt32   (SetType e, int v);
    public static  void setUInt32  (SetType e, uint v);
    public static  void setInt64   (SetType e, long v);
    public static  void setUInt64  (SetType e, ulong v);
    public static  void setEnum    (SetType e, int v);
    public static  void setSingle  (SetType e, float v);
    public static  void setDouble  (SetType e, double v);
    public static  void setIntPtr  (SetType e, long v);
    public static  void setBoolean(SetType e, bool v);
    // TODO 字符串
    public static void setString(SetType e, string value);
    public static void setVector2(SetType e, float x, float y);
    public static void setVector2(SetType e, Vector2 v)
    {
        setVector2(e, v.x, v.y);
    }
    public static void setVector3(SetType e, float x, float y, float z);
    public static void setVector3(SetType e, Vector3 v)
    {
        setVector3(e, v.x, v.y, v.z);
    }
    public static void setObject(SetType e, int id);

    public static bool isVector2(int i);
    public static bool isVector3(int i);

    public static void moveTempVal2Arr(int i);

    
    public static bool callFunction(int jsObjID, string functionName, int argCount);

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////// String Marshal ////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_EnableDebugger(IntPtr cx, IntPtr/*JSObject***/ glob, string[] paths, UInt32 nums, int port);
    //
    // string marshal: OK
    // C/Ansi -> C#/Unicode
    //
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate int JSErrorReporter(IntPtr cx, string message, IntPtr report);
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_DefineFunction(IntPtr cx, IntPtr obj, string name, /*JSNative*/IntPtr call, UInt32 nargs, UInt32 flags);
    //
    // string marshal: OK
    // C#/string (Unicode) -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewClass(string name, UInt32 flag, SC_FINALIZE finalizeOp);
    //
    // string marshal: OK
    // C/Unicode pointer -> C#/IntPtr
    // 
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr JSh_ArgvString(IntPtr cx, IntPtr vp, int i);
    public static string JSh_ArgvStringS(IntPtr cx, IntPtr vp, int i)
    {
        return Marshal.PtrToStringUni(JSh_ArgvString(cx, vp, i));
    }
    //
    // string marshal: OK
    // C#/Unicode -> C/Unicode
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int JSh_SetRvalString(IntPtr cx, IntPtr vp, string value);
    //
    // string marshal: OK
    // C#/Unicode -> C/Unicode
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern void JSh_SetJsvalString(IntPtr cx, ref jsval vp, string value);
    //
    // string marshal: OK
    // C/Unicode pointer -> C#/IntPtr
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr JSh_GetJsvalString(IntPtr cx, ref jsval vp);
    public static string JSh_GetJsvalStringS(IntPtr cx, ref jsval vp)
    {
        return Marshal.PtrToStringUni(JSh_GetJsvalString(cx, ref vp));
    }
    //
    // string marshal: OK
    // C/Ansi pointer -> C#/IntPtr
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_GetErroReportFileName(IntPtr report);
    public static string JSh_GetErroReportFileNameS(IntPtr report)
    {
        IntPtr str = JSh_GetErroReportFileName(report);
        return Marshal.PtrToStringAnsi(str);
    }
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_NewObjectAsClass(IntPtr cx, IntPtr glob, string className, SC_FINALIZE finalizeOp);
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    //public static extern IntPtr JSh_CompileScript(IntPtr cx, IntPtr obj, string bytes, uint length, string filename, uint lineno);
    public static extern IntPtr JSh_CompileScript(IntPtr cx, IntPtr obj, byte[] bytes, uint length, string filename, uint lineno);
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr JSh_GetFunction(IntPtr cx, IntPtr obj, string name);
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_GetFunctionValue(IntPtr cx, IntPtr obj, string name, ref jsval val);
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_CallFunctionName(IntPtr cx, IntPtr obj, string fname, UInt32 argc,
        /*[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] */jsval[] argv,
        ref jsval rval);
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool JSh_AddNamedScriptRoot(IntPtr cx, IntPtr rp, string name);
    //
    // string marshal: OK
    // C#/Unicode -> C/Ansi
    //
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void JSh_ReportError(IntPtr cx, string sErr);
}