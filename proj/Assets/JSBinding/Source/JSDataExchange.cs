using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

using jsval = JSApi.jsval;

public class JSDataExchangeMgr
{
    const int VALUE_LEN = -1;

    public enum eGetType
    {
        GetARGV,
        GetARGVRefOut,
        GetJSFUNRET,
        Jsval,
    }

    System.Object mTempObj;
    public void setTemp(System.Object obj)
    {
        mTempObj = obj;
    }


    #region Get Operation

    public object getObject(int e)
    {
        int jsObjID = JSApi.getObject(e);
        if (jsObjID == 0)
        {
            // no error
            return null;
        }

        object csObj = JSMgr.getCSObj(jsObjID);
        if (csObj == null)
        {
            csObj = new CSRepresentedObject(jsObjID);
        }
        return csObj;
    }

    /// <summary>
    /// Gets object by type.
    /// for concrete type e.g. setInt32 setString, they know how to return object
    /// 
    /// but for T parameters, type is unknown until runtime
    /// so this function needs a 'Type' argument
    /// which is passed through mTempObj
    /// </summary>
    /// <param name="eType">Type of the e.</param>
    /// <returns></returns>
    public object getByType(JSApi.GetType eType)
    {
        int e = (int)eType;
        Type type = (Type)mTempObj;
        if (type.IsByRef)
            type = type.GetElementType();

        if (type == typeof(string))
            return JSApi.getStringS(e);
        else if (type.IsEnum)
            return JSApi.getEnum(e);
        else if (type.IsPrimitive)
        {
            if (type == typeof(System.Boolean))
                return JSApi.getBooleanS(e);
            else if (type == typeof(System.Char))
                return JSApi.getChar(e);
            else if (type == typeof(System.Byte))
                return JSApi.getByte(e);
            else if (type == typeof(System.SByte))
                return JSApi.getSByte(e);
            else if (type == typeof(System.UInt16))
                return JSApi.getUInt16(e);
            else if (type == typeof(System.Int16))
                return JSApi.getInt16(e);
            else if (type == typeof(System.UInt32))
                return JSApi.getUInt32(e);
            else if (type == typeof(System.Int32))
                return JSApi.getInt32(e);
            else if (type == typeof(System.UInt64))
                return JSApi.getUInt64(e);
            else if (type == typeof(System.Int64))
                return JSApi.getInt64(e);
            else if (type == typeof(System.Single))
                return JSApi.getSingle(e);
            else if (type == typeof(System.Double))
                return JSApi.getDouble(e);
            else if (type == typeof(System.IntPtr))
                return JSApi.getIntPtr(e);
            else
                Debug.LogError("Unknown primitive type" + type.Name);
        }
        else if (type == typeof(Vector2))
        {
            return JSApi.getVector2S(e);
        }
        else if (type == typeof(Vector3))
        {
            return JSApi.getVector3S(e);
        }
        else
        {
            return JSApi.getObject(e);
        }
        return null;
    }
    public object getWhatever(int e)
    {
        var tag = JSApi.getTag(e);
        if (jsval.isNullOrUndefined(tag))
            return null;
        else if (jsval.isBoolean(tag))
            return JSApi.getBooleanS(e);
        else if (jsval.isInt32(tag))
            return JSApi.getInt32(e);
        else if (jsval.isDouble(tag))
            return JSApi.getSingle(e);
        else if (jsval.isString(tag))
            return JSApi.getStringS(e);
        else if (jsval.isObject(tag))
        {
            if (JSApi.isVector2S(e))
            {
                return JSApi.getVector2S(e);
            }
            else if (JSApi.isVector3S(e))
            {
                return JSApi.getVector3S(e);
            }
            else
            {
                return getObject(e);
            }
        }
        return null;
    }
    public object getDelegate(eGetType e)
    {
        switch (e)
        {
            case eGetType.GetARGV:
                {
                    // TODO check: index must ++
                    int jsObjID = JSApi.getObject((int)JSApi.GetType.Arg);
                    if (jsObjID == 0)
                        return null;

                    object csObj = JSMgr.getCSObj(jsObjID);
                    return csObj;

//                     IntPtr jsObj = JSApi.JSh_ArgvObject(JSMgr.cx, vc.vp, vc.currIndex++);
//                     if (jsObj == IntPtr.Zero)
//                         return null;
// 
//                     object csObj = JSMgr.getCSObj(jsObj);
//                     return csObj;
                }
                // break; !!
//             case eGetType.GetARGVRefOut:
//                 {
//                     jsval val = new jsval();
//                     JSApi.JSh_SetJsvalUndefined(ref val);
//                     getJSValueOfParam(ref val, vc.currIndex++);
// 
//                     IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref val);
//                     if (jsObj == IntPtr.Zero)
//                         return null;
// 
//                     JSApi.JSh_GetUCProperty(JSMgr.cx, jsObj, "__nativeObj", -1, ref val);
//                     IntPtr __nativeObj = JSApi.JSh_GetJsvalObject(ref val);
//                     if (__nativeObj == IntPtr.Zero)
//                         return null;
// 
//                     object csObj = JSMgr.getCSObj(__nativeObj);
//                     return csObj;
//                 }
//                 break;
//             case eGetType.Jsval:
//                 {
//                     jsval val = new jsval();
//                     JSApi.JSh_SetJsvalUndefined(ref val);
// 
//                     IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref vc.valTemp);
//                     if (jsObj == IntPtr.Zero)
//                         return null;
//
//                     object csObj = JSMgr.getCSObj(jsObj);
//                     return csObj;
//                 }
//                 break;
            default:
                Debug.LogError("Not Supported");
                break;
        }
        return null;
    }
    #endregion


    public enum eSetType
    {
        SetRval,
        UpdateARGVRefOut,
        Jsval,
        //GetJSFUNRET
    }

    #region Set Operation

    // for generic type
    // type is assigned during runtime
    public void setWhatever(int e, object obj)
    {
//         Type type = (Type)mTempObj;
//         if (type.IsByRef)
//             type = type.GetElementType();

        // ?? TODO use mTempObj or not?

        // TODO check
        if (obj == null)
        {
            JSApi.setUndefined(e);
            return;
        }

		if (obj != null && (obj is UnityEngine.Object))
		{
			if (obj.Equals(null))
			{
				JSApi.setUndefined(e);
				return;
			}
		}

        Type type = obj.GetType();

        if (type == typeof(string))
            JSApi.setStringS(e, (string)obj);
        else if (type.IsEnum)
            JSApi.setEnum(e, (int)obj);
        else if (type.IsPrimitive)
        {
            if (type == typeof(System.Boolean))
                JSApi.setBooleanS(e, (bool)obj);
            else if (type == typeof(System.Char))
                JSApi.setChar(e, (char)obj);
            else if (type == typeof(System.Byte))
                JSApi.setByte(e, (Byte)obj);
            else if (type == typeof(System.SByte))
                JSApi.setSByte(e, (SByte)obj);
            else if (type == typeof(System.UInt16))
                JSApi.setUInt16(e, (UInt16)obj);
            else if (type == typeof(System.Int16))
                JSApi.setInt16(e, (Int16)obj);
            else if (type == typeof(System.UInt32))
                JSApi.setUInt32(e, (UInt32)obj);
            else if (type == typeof(System.Int32))
                JSApi.setInt32(e, (Int32)obj);
            else if (type == typeof(System.UInt64))
                JSApi.setUInt64(e, (UInt64)obj);
            else if (type == typeof(System.Int64))
                JSApi.setInt64(e, (Int64)obj);
            else if (type == typeof(System.Single))
                JSApi.setSingle(e, (Single)obj);
            else if (type == typeof(System.Double))
                JSApi.setDouble(e, (Double)obj);
            else
                Debug.LogError("Unknown primitive type");
        }
        else if (type == typeof(Vector3))
        {
            JSApi.setVector3S(e, (Vector3)obj);
        }
        else if (type == typeof(Vector2))
        {
            JSApi.setVector2S(e, (Vector2)obj);
        }
        else
        {
            setObject(e, obj);
        }
    }

    /// <summary>
    /// Should return base type object?
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    /// <returns></returns>
//     public bool shouldReturnBaseTypeObject(string typeName)
//     {
// 
//     }

    /// <summary>
    /// Sets the object.
    /// if e == UpdateRefARGV, currIndex must be set before this function
    /// 
    /// operation of this function:
    /// 1) if JavaScript already exists, return that JavaScript object.
    /// 2) else, create a new JavaScript object and return it.
    /// </summary>
    /// <param name="e">The e.</param>
    /// <param name="csObj">The cs object.</param>
    /// <returns></returns>
    public int setObject(/* JSApi.SetType */int e, object csObj)
    {
		// http://answers.unity3d.com/questions/1087158/unityengineobject-is-null-but-systemobject-is-not.html
		if (csObj != null && (csObj is UnityEngine.Object))
		{
			if (csObj.Equals(null))
				csObj = null;
		}

        int jsObjID = 0;
        if (csObj != null)
        {
            Type csType = csObj.GetType();
            JSCache.TypeInfo typeInfo = JSCache.GetTypeInfo(csType);

            if (typeInfo.IsClass)
            {
                jsObjID = JSMgr.getJSObj(csObj, typeInfo);
            }
            if (jsObjID == 0)
            {
                if (csObj is CSRepresentedObject)
                {
                    jsObjID = ((CSRepresentedObject)csObj).jsObjID;
                }
                else
                {
                    if (typeInfo.IsDelegate)
                    {
                        jsObjID = JSMgr.getFunIDByDelegate((Delegate)csObj);
                    }
                    if (jsObjID == 0)
                    {
                        string typeName = string.Empty;
                        // create a JSRepresentedObject object in JS to represent a C# delegate object
                        if (typeInfo.IsDelegate)
                        {
                            typeName = "JSRepresentedObject";
                            jsObjID = JSApi.createJSClassObject(typeName);
                        }
                        else
                        {
                            typeName = typeInfo.JSTypeFullName;
                            jsObjID = JSApi.createJSClassObject(typeName);
                            if (jsObjID == 0)
                            {
                                Type baseType = csType.BaseType;
                                JSCache.TypeInfo baseTypeInfo = JSCache.GetTypeInfo(baseType);
                                if (baseType != null)
                                {
                                    var baseTypeName = baseTypeInfo.JSTypeFullName;
                                    jsObjID = JSApi.createJSClassObject(baseTypeName);
                                    if (jsObjID != 0)
                                    {
                                        Debug.LogWarning("WARNING: Return a \"" + typeName + "\" to JS failed. Return base type\"" + baseTypeName + "\" instead.");
                                    }
                                }
                            }
                        }

                        if (jsObjID != 0)
                            JSMgr.addJSCSRel(jsObjID, csObj);
                        else
                            Debug.LogError("Return a \"" + typeName + "\" to JS failed. Did you forget to export that class?");
                    }
                }
            }
        }

        JSApi.setObject(e, jsObjID);
        return jsObjID;
    }
    // TODO what?
//     public void setDelegate(eSetType e, object csObj)
//     {
//         switch (e)
//         {
//             case eSetType.Jsval:
//             case eSetType.SetRval:
//                 {
//                     JSApi.JSh_SetJsvalUndefined(ref vc.valReturn);
//                     if (csObj != null)
//                     {
//                         IntPtr jsObj = IntPtr.Zero;
//                         Type csType = csObj.GetType();
//                         if (csType.IsClass && (jsObj = JSMgr.getJSObj(csObj)) != IntPtr.Zero)
//                         {
//                             JSApi.JSh_SetJsvalObject(ref vc.valReturn, jsObj);
//                         }
//                         else
//                         {
//                             string typeName = JSNameMgr.GetJSTypeFullName(csType);
//                             IntPtr jstypeObj = JSDataExchangeMgr.GetJSObjectByname(typeName);
//                             if (jstypeObj != IntPtr.Zero)
//                             {
//                                 jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null /*JSMgr.mjsFinalizer*/);
// 
//                                 // __nativeObj
//                                 IntPtr __nativeObj = JSApi.JSh_NewMyClass(JSMgr.cx, JSMgr.mjsFinalizer);
//                                 JSMgr.addJSCSRelation(jsObj, __nativeObj, csObj);
// 
//                                 // jsObj.__nativeObj = __nativeObj
//                                 jsval val = new jsval();
//                                 JSApi.JSh_SetJsvalObject(ref val, __nativeObj);
//                                 JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, "__nativeObj", -1, ref val);
// 
//                                 JSApi.JSh_SetJsvalObject(ref vc.valReturn, jsObj);
//                             }
//                             else
//                             {
//                                 Debug.LogError("Return a \"" + typeName + "\" to JS failed. Did you forget to export that class?");
//                             }
//                         }
//                     }
// 
//                     if (e == eSetType.Jsval)
//                         vc.valTemp = vc.valReturn;
//                     else if (e == eSetType.SetRval)
//                         JSApi.JSh_SetRvalJSVAL(JSMgr.cx, vc.vp, ref vc.valReturn);
//                 }
//                 break;
//             case eSetType.UpdateARGVRefOut:
//                 {
//                     jsval val = new jsval(); val.asBits = 0;
//                     IntPtr argvJSObj = JSApi.JSh_ArgvObject(JSMgr.cx, vc.vp, vc.currIndex);
//                     if (argvJSObj != IntPtr.Zero)
//                     {
//                         bool success = false;
// 
//                         IntPtr jsObj = IntPtr.Zero;
//                         Type csType = csObj.GetType();
//                         if (csType.IsClass && (jsObj = JSMgr.getJSObj(csObj)) != IntPtr.Zero)
//                         {
//                             // 3)
//                             // argvObj.Value = jsObj
//                             //
//                             JSApi.JSh_SetJsvalObject(ref val, jsObj);
//                             JSApi.JSh_SetUCProperty(JSMgr.cx, argvJSObj, "Value", -1, ref val);
//                             success = true;
//                         }
//                         else
//                         {
//                             // csObj must not be null
//                             IntPtr jstypeObj = JSDataExchangeMgr.GetJSObjectByname(JSNameMgr.GetTypeFullName(csObj.GetType()));
//                             if (jstypeObj != IntPtr.Zero)
//                             {
//                                 // 1)
//                                 // jsObj: prototype  
//                                 // __nativeObj: csObj + finalizer
//                                 // 
//                                 jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null /*JSMgr.mjsFinalizer*/);
//                                 // __nativeObj
//                                 IntPtr __nativeObj = JSApi.JSh_NewMyClass(JSMgr.cx, JSMgr.mjsFinalizer);
//                                 JSMgr.addJSCSRelation(jsObj, __nativeObj, csObj);
// 
//                                 //
//                                 // 2)
//                                 // jsObj.__nativeObj = __nativeObj
//                                 //
//                                 JSApi.JSh_SetJsvalObject(ref val, __nativeObj);
//                                 JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, "__nativeObj", -1, ref val);
// 
//                                 // 3)
//                                 // argvObj.Value = jsObj
//                                 //
//                                 JSApi.JSh_SetJsvalObject(ref val, jsObj);
//                                 JSApi.JSh_SetUCProperty(JSMgr.cx, argvJSObj, "Value", -1, ref val);
//                                 success = true;
//                             }
//                             else
//                             {
//                                 Debug.LogError("Return a \"" + JSNameMgr.GetTypeFullName(csObj.GetType()) + "\" to JS failed. Did you forget to export that class?");
//                             }
//                         }
// 
//                         if (!success)
//                         {
//                             JSApi.JSh_SetJsvalUndefined(ref val);
//                             JSApi.JSh_SetUCProperty(JSMgr.cx, argvJSObj, "Value", -1, ref val);
//                         }
//                     }
//                 }
//                 break;
//             default:
//                 Debug.LogError("Not Supported");
//                 break;
//         }
//     }

    #endregion

    
    // return true if don't generate default constructor
//    public static bool DontGenDefaultConstructor(Type type)
//    {
//        bool bDontGenDefaultConstructor =
//            // type.GetConstructors().Length == 0 && 
//            type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Length > 0;
//        return bDontGenDefaultConstructor;
//    }

    static Dictionary<string, Type> typeCache = new Dictionary<string,Type>();
    public static Type GetTypeByName(string typeName, Type defaultType = null)
    {
        Type t = null;
        if (!typeCache.TryGetValue(typeName, out t))
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = a.GetType(typeName);
                if (t != null)
                {
                    // !!!
                    // if a type is marked with JsTypeAttribute
                    // don't return it
                    if (t.GetCustomAttributes(typeof(SharpKit.JavaScript.JsTypeAttribute), false).Length > 0)
                    {
                        t = null;
                    }
                    break;
                }
            }
            typeCache[typeName] = t; // perhaps null
            //if (t == null)
            //{
            //    Debug.LogError("GetType of \"" + typeName + "\" is null. Did you export that class to JavaScript?");
            //}
        }
        if (t == null)
        {
            return defaultType;// typeof(CSRepresentedObject);
        }
        return t;
    }


    /*
    // Runtime Only
    // type: class type
    // methodName: method name
    // TCount: generic parameter count
    // vc: JSVCall instance
    public static MethodInfo MakeGenericConstructor(Type type, int TCount, int paramCount, JSVCall vc)
    {
        // Get generic method by name and param count.
        ConstructorInfo conT = JSDataExchangeMgr.GetGenericConstructorInfo(type, TCount, paramCount);
        if (conT == null)
        {
            return null;
        }

        // get T types
        Type[] genericTypes = new Type[TCount];
        for (int i = 0; i < TCount; i++)
        {
            // Get generic types from js.
            System.Type t = JSDataExchangeMgr.GetTypeByName(JSMgr.datax.getString(JSDataExchangeMgr.eGetType.GetARGV));
            genericTypes[i] = t;
            if (t == null)
            {
                return null;
            }
        }

        // Make generic method.
        MethodInfo method = methodT.MakeGenericMethod(genericTypes);
        return method;
    }
    // Runtime Only
    // called by MakeGenericConstructor
    // get generic Constructor by matching TCount,paramCount, if more than 1 match, return null.
    static ConstructorInfo GetGenericConstructorInfo(Type type, int TCount, int paramCount)
    {
        ConstructorInfo[] constructors = type.GetConstructors();
        if (constructors == null || constructors.Length == 0)
        {
            return null;
        }

        ConstructorInfo con = null;
        for (int i = 0; i < constructors.Length; i++)
        {
            if (constructors[i].IsGenericMethodDefinition &&
                constructors[i].GetGenericArguments().Length == TCount &&
                constructors[i].GetParameters().Length == paramCount)
            {
                if (con == null)
                    con = constructors[i];
                else
                {
                    Debug.LogError("More than 1 Generic Constructor found!!! " + GetTypeFullName(type) + "." + name);
                    return null;
                }
            }
        }
        if (con == null)
        {
            Debug.LogError("No generic constructor found! " + GetTypeFullName(type));
        }
        return con;
    }*/
    public static ConstructorInfo makeGenericConstructor(Type type, ConstructorID constructorID)
    {
        int tCount = type.GetGenericArguments().Length;
        Type[] genericTypes = new Type[tCount];
        for (int i = 0; i < tCount; i++)
        {
            string typeName = JSApi.getStringS((int)JSApi.GetType.Arg);
            System.Type t = JSDataExchangeMgr.GetTypeByName(typeName, typeof(CSRepresentedObject));
            genericTypes[i] = t;
            if (t == null)
            {
                return null;
            }
        }

        var exactType = type.MakeGenericType(genericTypes);
        var exactConstructor = GenericTypeCache.getConstructor(exactType, constructorID);
        return exactConstructor;
    }

    /// <summary>
    /// Makes the generic method.
    /// not run in Editor mode, only runtime.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="methodID">The method identifier.</param>
    /// <param name="TCount">The t count.</param>
    /// <returns></returns>
    public static MethodInfo makeGenericMethod(Type type, MethodID methodID, int TCount)
    {
        MethodInfo methodT = GenericTypeCache.getMethod(type, methodID);
        if (methodT == null)
        {
            return null;
        }

        Type[] genericTypes = new Type[TCount];
        for (int i = 0; i < TCount; i++)
        {
            string typeName = JSApi.getStringS((int)JSApi.GetType.Arg);
            System.Type t = JSDataExchangeMgr.GetTypeByName(typeName, typeof(CSRepresentedObject));
            genericTypes[i] = t;
            if (t == null)
            {
                return null;
            }
        }

        MethodInfo method = methodT.MakeGenericMethod(genericTypes);
        return method;
    }
    //
    // 
    //
    public static Type[] RecursivelyGetGenericParameters(Type type, List<Type> lst = null)
    {
        if (lst == null)
            lst = new List<Type>();

        if (type.ContainsGenericParameters)
        {
            if (type.IsGenericParameter)
            {
                lst.Add(type);
            }
            else if (type.HasElementType)
            {
                RecursivelyGetGenericParameters(type.GetElementType(), lst);
            }
            else if (type.IsGenericType)
            {
                Type[] genericArguments = type.GetGenericArguments();
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    RecursivelyGetGenericParameters(genericArguments[i], lst);
                }
            }
        }
        return lst.ToArray();
    }
//     public static string GetMetatypeKeyword(Type type, out bool isWhatever)
//     {
//         string str = GetMetatypeKeyword(type);
//         isWhatever = (str == "Whatever");
//         return str;
//     }
    public static string GetMetatypeKeyword(Type type)
    {
        string ret = string.Empty;
        if (type.IsArray)
        {
            Debug.LogError("Array should not call GetMetatypeKeyword()");
            return ret;
        }

        if (type == typeof(string))
            ret = "JSApi.getStringS";
        else if (type.IsEnum)
            ret = "JSApi.getEnum";
        else if (type.IsPrimitive)
        {
            if (type == typeof(System.Boolean))
                ret = "JSApi.getBooleanS";
            else if (type == typeof(System.Char))
                ret = "JSApi.getChar";
            else if (type == typeof(System.Byte))
                ret = "JSApi.getByte";
            else if (type == typeof(System.SByte))
                ret = "JSApi.getSByte";
            else if (type == typeof(System.UInt16))
                ret = "JSApi.getUInt16";
            else if (type == typeof(System.Int16))
                ret = "JSApi.getInt16";
            else if (type == typeof(System.UInt32))
                ret = "JSApi.getUInt32";
            else if (type == typeof(System.Int32))
                ret = "JSApi.getInt32";
            else if (type == typeof(System.UInt64))
                ret = "JSApi.getUInt64";
            else if (type == typeof(System.Int64))
                ret = "JSApi.getInt64";
            else if (type == typeof(System.Single))
                ret = "JSApi.getSingle";
            else if (type == typeof(System.Double))
                ret = "JSApi.getDouble";
            else if (type == typeof(System.IntPtr))
                ret = "JSApi.getIntPtr";
            else
                Debug.LogError("444 Unknown primitive type");
        }
        else if (type == typeof(System.Object) || type.IsGenericParameter)
            ret = "JSMgr.datax.getWhatever";
        else if (type == typeof(Vector3))
            ret = "JSApi.getVector3S";
        else if (type == typeof(Vector2))
            ret = "JSApi.getVector2S";
        else
            ret = "JSMgr.datax.getObject";

        return ret;
    }

    public delegate T DGetV<T>();
    public static T GetJSArg<T>(DGetV<T> del) { return del(); }
}

public class JSDataExchange_Arr
{
    public Type elementType = null;

    public string Get_GetParam(Type t)
    {
        elementType = t.GetElementType();
        if (elementType.IsArray) { 
            //...error
        }
        StringBuilder sb = new StringBuilder();
        string getVal = JSDataExchangeMgr.GetMetatypeKeyword(elementType);

        var arrayFullName = string.Empty;
        var elementFullName = string.Empty;
        if (elementType.IsGenericParameter)
        {
            arrayFullName = "object[]";
            elementFullName = "object";
        }
        else
        {
            arrayFullName = JSNameMgr.GetTypeFullName(t);
            elementFullName = JSNameMgr.GetTypeFullName(elementType);
        }
        sb.AppendFormat("JSDataExchangeMgr.GetJSArg<{0}>(() =>\n", arrayFullName)
            .Append("    [[\n")
        .AppendFormat("    int jsObjID = JSApi.getObject((int)JSApi.GetType.Arg);\n")
        .AppendFormat("    int length = JSApi.getArrayLength(jsObjID);\n")
        .AppendFormat("    var ret = new {0}[length];\n", elementFullName)
        .AppendFormat("    for (var i = 0; i < length; i++) [[\n")
        .AppendFormat("        JSApi.getElement(jsObjID, i);\n")
        .AppendFormat("        ret[i] = ({0}){1}((int)JSApi.GetType.SaveAndRemove);\n", elementFullName, getVal)
        .AppendFormat("    ]]\n")
        .AppendFormat("    return ret;\n")
        .AppendFormat("]])\n");

//         sb.AppendFormat("JSDataExchangeMgr.GetJSArg<{0}>(() => [[\n", arrayFullName)
//         .AppendFormat("    IntPtr jsObj = JSApi.JSh_ArgvObject(JSMgr.cx, vc.vp, vc.currIndex++);\n")
//         .AppendFormat("    int length = JSApi.JSh_GetArrayLength(JSMgr.cx, jsObj);\n")
//         .AppendFormat("    var ret = new {0}[length];\n", elementFullName)
//         .AppendFormat("    for (var i = 0; i < length; i++) [[\n")
//         .AppendFormat("        JSApi.JSh_GetElement(JSMgr.cx, jsObj, (uint)i, ref vc.valTemp);\n")
//         .AppendFormat("        ret[i] = ({0}){1}((int)JSApi.GetType.Jsval);\n", elementFullName, getVal)
//         .AppendFormat("    ]]\n")
//         .AppendFormat("    return ret;\n")
//         .AppendFormat("]])\n");

        sb.Replace("[[", "{");
        sb.Replace("]]", "}");

        return sb.ToString(); 
    }

    public string Get_GetJSReturn()
    {
        return "null";
    }
    public string Get_Return(string expVar)
    {
        if (elementType == null)
        {
            Debug.LogError("JSDataExchange_Arr elementType == null !!");
            return "";
        }

        StringBuilder sb = new StringBuilder();
        string getValMethod = JSDataExchangeMgr.GetMetatypeKeyword(elementType).Replace("get", "set");

        // 2015.Sep.2
        // +判断arrRet为null的情况
        if (elementType.ContainsGenericParameters)
        {
            sb.AppendFormat("    var arrRet = (Array){0};\n", expVar)
            .AppendFormat("    for (int i = 0; arrRet != null && i < arrRet.Length; i++)\n")
            .Append("    [[\n")
            .AppendFormat("        {0}((int)JSApi.SetType.SaveAndTempTrace, arrRet.GetValue(i));\n", getValMethod)
            .AppendFormat("        JSApi.moveSaveID2Arr(i);\n")
            .AppendFormat("    ]]\n")
            .AppendFormat("    JSApi.setArrayS((int)JSApi.SetType.Rval, (arrRet != null ? arrRet.Length : 0), true);");
        }
        else
        {
            sb.AppendFormat("    var arrRet = ({0}[]){1};\n", JSNameMgr.GetTypeFullName(elementType), expVar)
            .AppendFormat("    for (int i = 0; arrRet != null && i < arrRet.Length; i++)\n")
            .Append("    [[\n")
            .AppendFormat("        {0}((int)JSApi.SetType.SaveAndTempTrace, {1}arrRet[i]);\n", getValMethod, elementType.IsEnum ? "(int)" : "")
            .AppendFormat("        JSApi.moveSaveID2Arr(i);\n")
            .AppendFormat("    ]]\n")
            .AppendFormat("    JSApi.setArrayS((int)JSApi.SetType.Rval, (arrRet != null ? arrRet.Length : 0), true);");
        }

        sb.Replace("[[", "{");
        sb.Replace("]]", "}");

        return sb.ToString();
    }
}

/// <summary>
/// CSRepresentedObject
/// if we have a JavaScript Message, like this:
/// var Message = 
/// {
///     id: 4,
///     str: "hello, world"
/// }
/// when it comes to C#, (e.g. store in a C# version List<>), there is no C# type matching it
/// so we make a CSRepresentedObject to wrap it
/// </summary>
public class CSRepresentedObject
{
    public static int s_objCount = 0;
    public static int s_funCount = 0;

//     public static bool operator !=(CSRepresentedObject x, CSRepresentedObject y)
//     {
//         return !(x == y);
//     }
//     public static bool operator ==(CSRepresentedObject x, CSRepresentedObject y)
//     {
//         // If both are null, or both are same instance, return true.
//         if (System.Object.ReferenceEquals(x, y))
//         {
//             return true;
//         }
//         return x.jsObjID == y.jsObjID;
//     }
//     public override bool Equals(object obj)
//     {
//         // If parameter is null return false.
//         if (obj == null)
//         {
//             return false;
//         }
// 
//         // If parameter cannot be cast to Point return false.
//         CSRepresentedObject p = obj as CSRepresentedObject;
//         if ((System.Object)p == null)
//         {
//             return false;
//         }
//         return this.jsObjID == p.jsObjID;
//     }

    // don't create this object directly, should use JSDataExchangeMgr.getObject
    public CSRepresentedObject(int jsObjID, bool bFunction = false)
    {
        this.jsEngineRound = JSMgr.jsEngineRound;
        this.jsObjID = jsObjID;
        this.bFunction = bFunction;
        JSMgr.addJSCSRel(jsObjID, this, true);

        if (bFunction) 
            s_funCount++;
        else 
            s_objCount++;

        // !
        // inc 之后 refCount 可能 > 1
        // getCSObj 可能检查 WeakReference.Target == null，表明 ~CSRepresentedObject 未被调用
        // 此时我们继续创建另一个 CSRepresentedObject 对象
        // 那么 refCount 就会 > 1

        //int refCount = 
            JSApi.incRefCount(jsObjID);
        //Debug.Log(new StringBuilder().AppendFormat("+ CSRepresentedObject {0} Ref[{1}] Fun[{1}]", jsObjID, refCount, bFunction ? 1 : 0));
    }
    ~CSRepresentedObject()
    {
        if (JSMgr.IsShutDown)
            return;

        Action action = CreateDestructAction(this.bFunction, this.jsObjID, this.jsEngineRound);
        JSEngine.inst.DoThreadSafeAction(action);

        //Debug.Log(new StringBuilder().AppendFormat("- CSRepresentedObject {0} Ref[{1}] Fun[{1}]", jsObjID, refCount, bFunction ? 1 : 0));
    }

    Action CreateDestructAction(bool bFunction, int jsObjID, int round)
    {
        return () =>
            {
                if (bFunction)
                    s_funCount--;
                else
                    s_objCount--;
                        
                // !
                // 由于 refCount 可能 > 1，这里必须判断 refCount <= 0 才能 JSMgr.removeJSCSRel

                int refCount = JSApi.decRefCount(jsObjID);
                if (refCount <= 0)
                {
                    JSMgr.removeJSCSRel(jsObjID, round);
                    if (bFunction)
                    {
                        JSMgr.removeJSFunCSDelegateRel(jsObjID);
                    }
                }
//                else
//                {
//                    Debug.LogError(";;;//IIL.x&");
//                }
            };
    }

    public int jsObjID;
    public bool bFunction;
    int jsEngineRound;
}