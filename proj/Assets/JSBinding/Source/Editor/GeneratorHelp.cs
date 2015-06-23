using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public static class GeneratorHelp
{
    /// <summary>
    /// usage
    /// 1 used for generating javascript code
    /// 2 used for generating c# code
    /// </summary>
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

    public static int AddTypeInfo(Type type, out ATypeInfo tiOut)
    {
        ATypeInfo ti = new ATypeInfo();
        ti.fields = type.GetFields(JSMgr.BindingFlagsField);
        ti.properties = type.GetProperties(JSMgr.BindingFlagsProperty);
        ti.methods = type.GetMethods(JSMgr.BindingFlagsMethod);
        ti.constructors = type.GetConstructors();
        if (JSBindingSettings.NeedGenDefaultConstructor(type))
        {
            // null means it's default constructor
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
            if (type == typeof(UnityEngine.MonoBehaviour))
            {
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

    /// <summary>
    /// Method is overloaded or not?
    /// Used in JSGenerator2
    /// No matter it's static or not
    /// including NonPublic methods
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns></returns>
    public static bool MethodIsOverloaded(Type type, string methodName)
    {
        MethodInfo[] methods = type.GetMethods(JSMgr.BindingFlagsMethod2);
        int N = 0;
        foreach (var m in methods)
        {
            if (m.Name == methodName)
            {
                N++;
                if (N >= 2)
                    return true;
            }
        }
        return false;
    }
}