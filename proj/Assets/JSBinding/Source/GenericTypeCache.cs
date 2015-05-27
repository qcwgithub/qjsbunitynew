using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

class MemberID
{
    // -2: initial 
    // -1: failed
    // >=0: success
    public int index = -2; 
}

enum TypeFlag
{
    IsRef = 1, // only for parameter
    IsOut = 2, // only for parameter
    IsT = 4, // for example: T Load<T>();  the return type is T
}
class ConstructorID : MemberID
{
    public Type[] parameterTypes; // can be null
    public TypeFlag[] parameterFlags; // can be null
    // constructor
    public ConstructorID(Type[] parameterTypes, TypeFlag[] parameterFlags)
    {
        this.parameterTypes = parameterTypes;
        this.parameterFlags = parameterFlags;
    }
}
class FieldID : MemberID
{
    public string name; //memberName
    // property method
    public FieldID(string name)
    {
        this.name = name;
    }
}
class PropertyID : ConstructorID
{
    public string name; //memberName
    public Type returnType;
    public TypeFlag returnTypeFlag;

    public PropertyID(string name, Type returnType, TypeFlag returnTypeFlag, Type[] parameterTypes, TypeFlag[] typeFlags)
        : base(parameterTypes, typeFlags)
    {
        this.name = name;
        this.returnType = returnType;
        this.returnTypeFlag = returnTypeFlag;
    }
}
class MethodID : PropertyID
{
    public MethodID(string name, Type returnType, TypeFlag returnTypeFlag, Type[] parameterTypes, TypeFlag[] typeFlags)
        : base(name, returnType, returnTypeFlag, parameterTypes, typeFlags)
    {
    }
}

class GenericTypeCache
{
    class TypeMembers
    {
        public ConstructorInfo[] cons = null;
        public FieldInfo[] fields = null;
        public PropertyInfo[] properties = null;
        public MethodInfo[] methods = null;
    }
    static Dictionary<Type, TypeMembers> dict;
    static TypeMembers getMembers(Type type)
    {
        TypeMembers tm;
        if (dict.TryGetValue(type, out tm))
        {
            return tm;
        }
        tm = new TypeMembers();
        tm.cons = type.GetConstructors();
        tm.fields = type.GetFields(JSMgr.BindingFlagsField);
        tm.properties = type.GetProperties(JSMgr.BindingFlagsProperty);
        tm.methods = type.GetMethods(JSMgr.BindingFlagsMethod);

        dict.Add(type, tm);
        return tm;
    }

    static bool matchParameters(ParameterInfo[] pi, Type[] parameterTypes, TypeFlag[] typeFlags)
    {
        if (pi == null || pi.Length == 0)
        {
            return (parameterTypes == null || parameterTypes.Length == 0);
        }

        if (parameterTypes == null || parameterTypes.Length != pi.Length)
        {
            return false;
        }

        for (var i = 0; i < pi.Length; i++)
        {
            Type t = pi[i].ParameterType;
            if (t != parameterTypes[i])
            {
                return false;
            }
            if (t.IsByRef)
            {
                if (typeFlags == null || typeFlags.Length <= i || 0 == (typeFlags[i] & TypeFlag.IsRef))
                    return false;
            }
            if (pi[i].IsOut)
            {
                if (typeFlags == null || typeFlags.Length <= i || 0 == (typeFlags[i] & TypeFlag.IsOut))
                    return false;
            }
        }
        return true;
    }

    static ConstructorInfo getConstructor(Type type, ConstructorID id)
    {
        if (id.index >= 0)
        {
            return dict[type].cons[id.index];
        }
        if (id.index == -2)
        {
            TypeMembers tmember = getMembers(type);
            if (tmember.cons != null)
            {
                for (var i = 0; i < tmember.cons.Length; i++)
                {
                    if (matchParameters(tmember.cons[i].GetParameters(), id.parameterTypes, id.parameterFlags))
                    {
                        id.index = i;
                        return tmember.cons[i];
                    }
                }
            }
        }
        id.index = -1;
        return null;
    }
    static FieldInfo getField(Type type, FieldID id)
    {
        if (id.index >= 0)
        {
            return dict[type].fields[id.index];
        }
        if (id.index == -2)
        {
            TypeMembers tmember = getMembers(type);
            if (tmember.fields != null)
            {
                for (var i = 0; i < tmember.fields.Length; i++)
                {
                    if (tmember.fields[i].Name == id.name)
                    {
                        id.index = i;
                        return tmember.fields[i];
                    }
                }
            }
        }
        id.index = -1;
        return null;
    }
    static PropertyInfo getProperty(Type type, PropertyID id)
    {
        if (id.index >= 0)
        {
            return dict[type].properties[id.index];
        }
        if (id.index == -2)
        {
            TypeMembers tmember = getMembers(type);
            if (tmember.properties != null)
            {
                for (var i = 0; i < tmember.properties.Length; i++)
                {
                    PropertyInfo pro = tmember.properties[i];
                    if (pro.Name == id.name &&
                        pro.PropertyType == id.returnType &&
                        matchParameters(pro.GetIndexParameters(), id.parameterTypes, id.parameterFlags))
                    {
                        id.index = i;
                        return pro;
                    }
                }
            }
        }
        id.index = -1;
        return null;
    }
    static MethodInfo getMethod(Type type, MethodID id)
    {
        if (id.index >= 0)
        {
            return dict[type].methods[id.index];
        }
        if (id.index == -2)
        {
            TypeMembers tmember = getMembers(type);
            if (tmember.methods != null)
            {
                for (var i = 0; i < tmember.methods.Length; i++)
                {
                    MethodInfo method = tmember.methods[i];
                    if (method.Name == id.name &&
                        method.ReturnType == id.returnType &&
                        matchParameters(method.GetParameters(), id.parameterTypes, id.parameterFlags))
                    {
                        id.index = i;
                        return method;
                    }
                }
            }
        }
        id.index = -1;
        return null;
    }
}