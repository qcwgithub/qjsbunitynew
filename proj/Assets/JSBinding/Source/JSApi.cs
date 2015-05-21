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
    public delegate bool CSEntry(int op, int slot, int index, int bStatic, int argc);
    
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
#endif
    public delegate void OnObjCollected(int id);

    /*
     * ****************** jsval definition ****************** 
     */
    enum eValueTag
    {
        tagUNDEFINED = 1 << 0,
        tagNULL = 1 << 1,
        tagINT32 = 1 << 2,
        tagDOUBLE = 1 << 3,
        tagBOOLEAN = 1 << 4,
        tagSTRING = 1 << 5,
        tagNUMBER = 1 << 6,
        tagOBJECT = 1 << 7,
    }
    public struct jsval
    {
        public static bool isUndefined(uint tag) { return 0 != (tag & (uint)eValueTag.tagUNDEFINED); }
        public static bool isNull(uint tag) { return 0 != (tag & (uint)eValueTag.tagNULL); }
        public static bool isInt32(uint tag) { return 0 != (tag & (uint)eValueTag.tagINT32); }
        public static bool isDouble(uint tag) { return 0 != (tag & (uint)eValueTag.tagDOUBLE); }
        public static bool isBoolean(uint tag) { return 0 != (tag & (uint)eValueTag.tagBOOLEAN); }
        public static bool isString(uint tag) { return 0 != (tag & (uint)eValueTag.tagSTRING); }
        public static bool isNumber(uint tag) { return 0 != (tag & (uint)eValueTag.tagNUMBER); }
        public static bool isObject(uint tag) { return 0 != (tag & (uint)eValueTag.tagOBJECT); }
        public static bool isNullOrUndefined(uint tag) { return isUndefined(tag) || isNull(tag); }
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

    // only used in Constructors!
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool attachFinalizerObject(int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int createJSClassObject(string name);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int newJSClassObject(string name);
    
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int InitJSEngine(JSErrorReporter er, CSEntry csEntry, JSNative req, OnObjCollected onObjCollected);
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
        SaveAndRemove = 3,
    }
    public enum SetType
    {
        Rval = 0,
        ArgRef = 1,
        SaveAndTempTrace = 2,
    }

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getArgIndex();
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setArgIndex(int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int incArgIndex();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint getTag(int e);
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
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr getString(int e);
    public static string getStringS(int e)
    {
        IntPtr ptrS = JSApi.getString(e);
        string s = Marshal.PtrToStringUni(ptrS);
        return s;
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
    public static Vector3 getVector3S(int e)
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
    // TODO
    // protect
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    private static extern int getFunction(int e);
    public static int getFunctionS(int e)
    {
        int funID = JSApi.getFunction(e);
        if (JSEngine.inst != null && JSEngine.inst.forceProtectJSFunction)
        {
            JSApi.setTrace(funID, true);
        }
        return funID;
    }

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
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
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
    public static extern void setArray(int e, int count, bool bClear);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool isVector2(int e);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool isVector3(int e);


    /*
     * ****************** Other APIs ****************** 
     */


    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool callFunctionValue(int jsObjID, int funID, int argCount);

    // setTrace is currently for JSComponent object and JSApi.getFunctionS
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool setTrace(int id, bool trace);

    // val movement
    // any call to move**2Arr is for subsequent call to JSApi.setArray
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void moveSaveID2Arr(int arrIndex);

    // these 3 functions are only used by JSSerializer
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getSaveID();
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void removeByID(int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool moveID2Arr(int id, int arrIndex);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool setProperty(int id, string name, int valueID);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool getElement(int id, int i);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int getArrayLength(int id);
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern void gc();

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern bool evaluate(byte[] ascii, uint length, string filename);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
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
    // only used by JSComponent
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern int getObjFunction(int id, string fname);

    // only used by JSMgr.require
    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void setRvalBool(IntPtr vp, bool v);

    [DllImport(JSDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int getValueMapSize();
}