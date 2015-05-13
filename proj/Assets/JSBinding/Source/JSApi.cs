using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Text;
using System.Security;

public class JSApi
{
    /*
     * ****************** Dll define *************************
     */

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
    const string JSDll = "mozjswrap";
#elif UNITY_IPHONE
    const string JSDll = "__Internal";
#else
    const string JSDll = "mozjswrap";
#endif

    /*
     * ****************** Callback definition ****************** 
     */

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate void JSFinalizeOp(IntPtr freeOp, IntPtr obj);

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate bool JSNative(IntPtr cx, uint argc, IntPtr vp);

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate int JSErrorReporter(IntPtr cx, string message, IntPtr report);

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate bool CSEntry(int op, int slot, int index, bool bStatic, int argc);

    /*
     * ****************** jsval definition ****************** 
     */

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

    /*
     * ****************** Debugger stuff ****************** 
     */

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern void updateDebugger();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void cleanupDebugger();    
    
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void enableDebugger(string[] paths, UInt32 nums, int port);

    // TODO
    // 检查所有使用的地方看是否都正确使用（会有死循环的情况存在！）
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int NewJSClassObject(string name);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool IsJSClassObjectFunctionExist(int objID, string functionName);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int InitJSEngine(JSErrorReporter er, CSEntry csEntry, JSNative req);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool initErrorHandler();
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void ShutdownJSEngine();

    /*
     * ****************** Error handle ****************** 
     */

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void reportError(string err);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int getErroReportLineNo(IntPtr report);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr getErroReportFileName(IntPtr report);

    public static string getErroReportFileNameS(IntPtr report)
    {
        IntPtr str = getErroReportFileName(report);
        return Marshal.PtrToStringAnsi(str);
    }

    /*
     * ****************** Calling stack ****************** 
     */

    // TODO
    // CHECK: exactly same value as C?
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
        ArgRef = 1,
        TempVal = 2,
    }

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getArgIndex();
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setArgIndex(int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int incArgIndex();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint argTag(int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern short getChar(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern char getSByte(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte getByte(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern short getInt16(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort getUInt16(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getInt32(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint getUInt32(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern long getInt64(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong getUInt64(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getEnum(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern float getSingle(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern double getDouble(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern long getIntPtr(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool getBoolean(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern IntPtr getString(int e);
    public static string getStringS(int e)
    {
        return Marshal.PtrToStringUni(JSApi.getString(e));
    }

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void setFloatPtr2(ref float f0, ref float f1);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool getVector2(int e);
    public static Vector2 getVector2S(int e)
    {
        // TODO does this work?
        // ref v.x, ref v.y  <-- TODO is it OK?
        float x = 0, y = 0;
        setFloatPtr2(ref x, ref y);
        getVector2(e);
        return new Vector2(x, y);
    }
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void setFloatPtr3(ref float f0, ref float f1, ref float f2);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern bool getVector3(int e);
    public static Vector2 getVector3S(int e)
    {
        // TODO does this work?
        float x = 0, y = 0, z = 0;
        setFloatPtr3(ref x, ref y, ref z);
        getVector3(e);
        return new Vector3(x, y, z);
    }
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getObject(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool isFunction(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getFunction(int e);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setUndefined(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setChar(int e, char v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setSByte(int e, sbyte v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setByte(int e, byte v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setInt16(int e, short v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setUInt16(int e, ushort v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setInt32(int e, int v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setUInt32(int e, uint v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setInt64(int e, long v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setUInt64(int e, ulong v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setEnum(int e, int v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setSingle(int e, float v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setDouble(int e, double v);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void setIntPtr(int e, long v);
    public static void setIntPtr(int e, IntPtr v)
    {
        setIntPtr(e, v.ToInt64());
    }
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setBoolean(int e, bool v);
    // TODO 字符串
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private static extern void setString(int e, string value);
    public static void setStringS(int e, string value)
    {
        setString(e, value);
    }
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void setVector2(int e, float x, float y);
    public static void setVector2S(int e, Vector2 v)
    {
        setVector2(e, v.x, v.y);
    }
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern void setVector3(int e, float x, float y, float z);
    public static void setVector3S(int e, Vector3 v)
    {
        setVector3(e, v.x, v.y, v.z);
    }
    // TODO
    // 这个函数应该只有 JSDataExchangeMgr.setObject 调用？
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setObject(int e, int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setFunction(int e, int funID);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setArray(int e, int count);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool isVector2(int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool isVector3(int i);


    /*
     * ****************** Other APIs ****************** 
     */

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void moveTempVal2Arr(int i);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool callFunctionValue(int jsObjID, int funID, int argCount);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool addObjectRoot(int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool removeObjectRoot(int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool addValueRoot(int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool removeValueRoot(int id);


    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int moveVal2HeapMap();
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void removeHeapMapVal(int index);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool moveValFromMap2Arr(int iMap, int iArr);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool setProperty(int id, string name, int iMap);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool getElement(int id, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int getArrayLength(int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void gc();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool evaluate(byte[] ascii, uint length, string filename);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    private static extern IntPtr getArgString(IntPtr vp, int i);
    public static string getArgStringS(IntPtr vp, int i)
    {
        IntPtr pJSChar = getArgString(vp, i);
        if (pJSChar == IntPtr.Zero)
        {
            Debug.LogError("getArgString return NULL.");
            return string.Empty;
        }
        return Marshal.PtrToStringUni(pJSChar);
    }
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getObjFunction(int id, string fname);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setRvalBool(IntPtr vp, bool v);
}