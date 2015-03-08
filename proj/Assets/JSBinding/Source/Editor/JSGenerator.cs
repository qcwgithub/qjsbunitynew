using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


public static class JSGenerator
{
    // input
    static StringBuilder sb = null;
    public static Type type = null;

    static StreamWriter enumWriter;
    static string enumFile = JSBindingSettings.jsGeneratedDir + "/enum" + JSBindingSettings.jsExtension;
    //static string tempFile = JSBindingSettings.jsDir + "/temp"+JSBindingSettings.jsExtension;

    public static void OnBegin()
    {
        JSMgr.ClearTypeInfo();

        if (Directory.Exists(JSBindingSettings.jsGeneratedDir))
        {
            // delete all last generated files
            string[] files = Directory.GetFiles(JSBindingSettings.jsGeneratedDir);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }
        else
        {
            // create directory
            Directory.CreateDirectory(JSBindingSettings.jsGeneratedDir);
        }

        // clear generated enum files
        enumWriter = OpenFile(enumFile, false);
        enumWriter.Write("this.Enum = {};\n");
    }
    public static void OnEnd()
    {
        enumWriter.Close();
    }

    public static StringBuilder BuildFields(Type type, FieldInfo[] fields, int slot)
    {
        /*
        * fields
        * 0 class name
        * 1 field name
        * 2 slot
        * 3 index
        * 4 GET_FIELD
        * 5 SET_FIELD
        * 6 field type
        * 7 READ only / WRITE only / READ & WRITE
        */
        string fmt = @"
/* {7} {6} */
Object.defineProperty({0}.prototype, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, false, this); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, false, this, v); ]]
]]);
";
        string fmtStatic = @"
/* {7} static {6} */
Object.defineProperty({0}, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, true); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, true, v); ]]
]]);
";
        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            if (!field.IsStatic)
                sb.AppendFormat(fmt, className, field.Name, slot, i, (int)JSVCall.Oper.GET_FIELD, (int)JSVCall.Oper.SET_FIELD, field.FieldType.Name,
                    (field.IsInitOnly || field.IsLiteral) ? "ReadOnly" : ""
                    );
            else
                sb.AppendFormat(fmtStatic, className, field.Name, slot, i, (int)JSVCall.Oper.GET_FIELD, (int)JSVCall.Oper.SET_FIELD, field.FieldType.Name,
                    (field.IsInitOnly || field.IsLiteral) ? "ReadOnly" : ""
                    );
        }
        return sb;
    }
    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties, int slot)
    {
        /*
        * properties
        * 0 class name
        * 1 property name
        * 2 slot
        * 3 index in field array
        * 4 GET_PROPERTY
        * 5 SET_PROPERTY
        * 6 return type
        * 7 READ only / WRITE only
        * 8 isStatic
        */
        string fmt = @"
/* {7} {6} */
Object.defineProperty({0}.prototype, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, {8}, this); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, {8}, this, v); ]]
]]);
";
        string fmtStatic = @"
/* {7} {6} */
Object.defineProperty({0}, '{1}', 
[[
    get: function() [[ return CS.Call({4}, {2}, {3}, {8}); ]],
    set: function(v) [[ return CS.Call({5}, {2}, {3}, {8}, v); ]]
]]);
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];
            if (property.Name == "Item") //[] not support
                continue;

            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;

            //             if (property.Name == "Item")
            //             {
            //                 Debug.Log("");
            //             }
            //             if (property.IsSpecialName)
            //             {
            //                 if (!mDictJJ.ContainsKey(property.Name))
            //                     mDictJJ.Add(property.Name, "");
            //             }

            sb.AppendFormat(isStatic ? fmtStatic : fmt, className, property.Name, slot, i, (int)JSVCall.Oper.GET_PROPERTY, (int)JSVCall.Oper.SET_PROPERTY, property.PropertyType.Name,
                (property.CanRead && property.CanWrite) ? "" : (property.CanRead ? "ReadOnly" : "WriteOnly"), (isStatic ? "true" : "false")
                );
        }
        return sb;
    }
    //static Dictionary<string, string> mDictJJ = new Dictionary<string, string>();
    public static StringBuilder BuildConstructors(Type type, ConstructorInfo[] constructors, int slot)
    {
        /*
         * 0 op
         * 1 slot
         * 2 index
         * 3 true (isStatic)
         * 4 args
         * 5 Class name
         * 6 overload count
         * 7 formal parameters
         */
        //string fmt = @"{5} = MakeNS({9}).{5} = function({7}) [[
        string fmt = @"{5} = function({7}) [[
    /* overloaded {6} */
    return CS.Call({0}, {1}, {2}, {3}, {8}{4});
]]";
        bool bOverload = constructors.Length > 0;
        int overloadedMaxParamCount = 0;
        if (constructors.Length == 0)
        {
            //Debug.Log("&&&&&&&  [" + type.Name + "] has no constructor!");
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < constructors.Length; i++)
        {
            ConstructorInfo con = constructors[i];
            ParameterInfo[] ps = con.GetParameters();
            overloadedMaxParamCount = Math.Max(ps.Length, overloadedMaxParamCount);

        }
        StringBuilder sbFormalParam = new StringBuilder();
        StringBuilder sbActualParam = new StringBuilder();
        for (int j = 0; j < overloadedMaxParamCount; j++)
        {
            sbFormalParam.AppendFormat("a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "));
            sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "), (j == 0 ? ", " : ""));
        }

        sb.AppendFormat(fmt, (int)JSVCall.Oper.CONSTRUCTOR, slot, 0, "true", sbActualParam, className, constructors.Length, sbFormalParam, bOverload ? "true" : "false", 
            "\""+type.Namespace+"\"" /* namespace string*/);

        return sb;
    }
    public static string SharpKitConstructorName(ConstructorInfo constructor, int howmanyConstructors)
    {
        ParameterInfo[] paramS = constructor.GetParameters();
        string name = "ctor";
        if (howmanyConstructors == 1)
        {
            // if there is only one constructor in original class
            // the constructor name is "ctor", no suffix
            return name;
        }
        for (int i = 0; i < paramS.Length; i++)
        {
            Type t = paramS[i].ParameterType;
            if (!t.IsArray)
            {
                name += "$$" + t.Name;
            }
            else
            {
                name += "$$";
                while (t.IsArray)
                {
                    Type subt = t.GetElementType();
                    name += subt.Name + "$";
                    t = subt;
                }
                name += "Array";
            }
        }
        return name;
    }
    public static StringBuilder BuildConstructors__forsharpkit(Type type, ConstructorInfo[] constructors, int slot, int howmanyConstructors)
    {
        string fmt = @"
{4}.{5} = function({6}) [[
    return CS.Call({0}, {1}, {2}, {3}, {7}{8});
]]";

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < constructors.Length; i++)
        {
            ConstructorInfo con = constructors[i];
            ParameterInfo[] ps = con.GetParameters();

            StringBuilder sbFormalParam = new StringBuilder();
            StringBuilder sbActualParam = new StringBuilder();
            for (int j = 0; j < ps.Length; j++)
            {
                sbFormalParam.AppendFormat("a{0}{1}", j, (j == ps.Length - 1 ? "" : ", "));
                sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == ps.Length - 1 ? "" : ", "), (j == 0 ? ", " : ""));
            }
            sb.AppendFormat(fmt, (int)JSVCall.Oper.CONSTRUCTOR, slot, i/* index */, "true"/* isStatic */,
                className, SharpKitConstructorName(con, howmanyConstructors), sbFormalParam,
                "false", /*isOverloaded*/ 
                sbActualParam);
        }
        return sb;
    }
    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods, int slot)
    {
        /*
        * methods
        * 0 class name
        * 1 method name
        * 2 formal parameters
        * 3 slot
        * 4 index
        * 5 actual parameters
        * 6 return type
        * 7 op
        * 8 is override
         * 9 some information
        */
        string fmt = @"
/* {6} {9} */
{0}.prototype.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, false, this, {8}{5}); ]]";
        string fmtStatic = @"
/* static {6} {9} */
{0}.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, true, {8}{5}); ]]";

        int overloadedIndex = 0;
        int overloadedCount = 0;
        int overloadedMaxParamCount = 0;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];

            // here assumes static functions don't have same name with instance functions
            ParameterInfo[] paramS = method.GetParameters();
            if (i < methods.Length - 1 && method.Name == methods[i + 1].Name)
            {
                if (overloadedCount == 0)
                {
                    overloadedCount = 2;
                    overloadedIndex = i;
                }
                else
                {
                    overloadedCount++;
                }
                overloadedMaxParamCount = Math.Max(overloadedMaxParamCount, paramS.Length);
                continue;
            }
            StringBuilder sbFormalParam = new StringBuilder();
            StringBuilder sbActualParam = new StringBuilder();

            if (overloadedCount > 0)
            {
                for (int j = 0; j < overloadedMaxParamCount; j++)
                {
                    sbFormalParam.AppendFormat("a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "));
                    sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == overloadedMaxParamCount - 1 ? "" : ", "), (j == 0 ? ", " : ""));
                }
                sb.AppendFormat(@"
/* overloaded {0} */", overloadedCount);
                if (!method.IsStatic)
                    sb.AppendFormat(fmt, className, method.Name, sbFormalParam.ToString(), slot, overloadedIndex, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "true", "");
                else
                    sb.AppendFormat(fmtStatic, className, method.Name, sbFormalParam.ToString(), slot, overloadedIndex, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "true", "");
            }
            else
            {
                StringBuilder sbInfo = new StringBuilder();
                sbInfo.AppendFormat("{0}", method.IsSpecialName ? "SPECIAL" : "");

                for (int j = 0; j < paramS.Length; j++)
                {
                    ParameterInfo param = paramS[j];
                    sbFormalParam.AppendFormat("a{0}/* {1} */{2}", j, param.ParameterType.Name, (j == paramS.Length - 1 ? "" : ", "));
                    sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == paramS.Length - 1 ? "" : ", "), (j == 0 ? ", " : ""));
                }
                if (!method.IsStatic)
                    sb.AppendFormat(fmt, className, method.Name, sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "false", sbInfo);
                else
                    sb.AppendFormat(fmtStatic, className, method.Name, sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "false", sbInfo);
            }

            overloadedCount = 0;
            overloadedIndex = 0;
        }
        return sb;
    }
    public static string SharpKitMethodName(MethodInfo method, bool constructor, bool overloaded /* must be true */)
    {
        if (!overloaded) return string.Empty;
        ParameterInfo[] paramS = method.GetParameters();
        string name = constructor ? "ctor" : method.Name;
        for (int i = 0; i < paramS.Length; i++)
        {
            name += "$$" + paramS[i].ParameterType.Name;
        }
        return name;
    }
    public static StringBuilder BuildMethods__forsharpkit(Type type, MethodInfo[] methods, int slot)
    {
        /*
        * methods
        * 0 class name
        * 1 method name
        * 2 formal parameters
        * 3 slot
        * 4 index
        * 5 actual parameters
        * 6 return type
        * 7 op
        * 8 is override
         * 9 some information
        */
        string fmt = @"
/* {6} {9} */
{0}.prototype.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, false, this, {8}{5}); ]]";
        string fmtStatic = @"
/* static {6} {9} */
{0}.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, true, {8}{5}); ]]";

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];

            if ((i > 0 && method.Name == methods[i - 1].Name) ||
                (i < methods.Length - 1 && method.Name == methods[i + 1].Name))
            {
                StringBuilder sbFormalParam = new StringBuilder();
                StringBuilder sbActualParam = new StringBuilder();
                ParameterInfo[] paramS = method.GetParameters();
                int L = paramS.Length;
                for (int j = 0; j < L; j++)
                {
                    sbFormalParam.AppendFormat("a{0}{1}", j, (j == L - 1 ? "" : ", "));
                    sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == L - 1 ? "" : ", "), (j == 0 ? ", " : ""));
                }
                if (!method.IsStatic)
                    sb.AppendFormat(fmt, className, SharpKitMethodName(method, false, true), sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "false", "");
                else
                    sb.AppendFormat(fmtStatic, className, SharpKitMethodName(method, false, true), sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "false", "");
            }
        }
        return sb;
    }
    public static StringBuilder BuildClass(Type type, StringBuilder sbFields, StringBuilder sbProperties, StringBuilder sbMethods, StringBuilder sbConstructors)
    {
        /*
        * class
        * 0 class name
        * 1 fields
        * 2 properties
        * 3 methods
        * 4 constructors
        */
        string fmt = @"
{4}

// fields
{1}
// properties
{2}
// methods
{3}
";
        var sb = new StringBuilder();
        sb.AppendFormat(fmt, className, sbFields.ToString(), sbProperties.ToString(), sbMethods.ToString(), sbConstructors.ToString());
        return sb;
    }

    public static void GenerateClass()
    {
        /*if (type.IsInterface)
        {
            Debug.Log("Interface: " + type.ToString() + " ignored.");
            return;
        }*/

        JSMgr.ATypeInfo ti;
        int slot = JSMgr.AddTypeInfo(type, out ti);
        var sbCons = BuildConstructors(type, ti.constructors, slot);
        var sbCons__forsharpkit = BuildConstructors__forsharpkit(type, ti.constructors, slot, ti.howmanyConstructors);
        //sbCons.Append(sbCons__forsharpkit);
        var sbFields = BuildFields(type, ti.fields, slot);
        var sbProperties = BuildProperties(type, ti.properties, slot);
        var sbMethods = BuildMethods(type, ti.methods, slot);
        var sbMethods__forsharpkit = BuildMethods__forsharpkit(type, ti.methods, slot);
        sbMethods.Append(sbMethods__forsharpkit);
        var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods, sbCons);
        HandleStringFormat(sbClass);

        string fileName = JSBindingSettings.jsGeneratedDir + "/" + className + JSBindingSettings.jsExtension;
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sbClass.ToString());
        writer2.Close();
    }

    static void GenerateEnum()
    {
        var sb = new StringBuilder();

        // comment line
        string fmtComment = @"// {0}
";
        sb.AppendFormat(fmtComment, type.ToString());

        // remove name space
        string typeName = type.ToString();
        int lastDot = typeName.LastIndexOf('.');
        if (lastDot >= 0)
        {
            typeName = typeName.Substring(lastDot + 1);
        }

        if (typeName.IndexOf('+') >= 0)
            return;

        string fmt = "this.Enum.{0} = [[\n";
        sb.AppendFormat(fmt, typeName);


        FieldInfo[] fields = type.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.Static);
        string fmtField = "    {0}: {1}{2}\n";
        for (int i = 0; i < fields.Length; i++)
        {
            sb.AppendFormat(fmtField, fields[i].Name, (int)fields[i].GetValue(null), i==fields.Length-1?"":",");
        }
        string fmtEnter = "]];\n";
        sb.Append(fmtEnter);

        HandleStringFormat(sb);
        enumWriter.Write(sb.ToString());
    }

    public static void Clear()
    {
        type = null;
        sb = new StringBuilder();
    }
    static void GenEnd()
    {
        string fmt = @"
]]
";
        sb.Append(fmt);
    }

    static void WriteUsingSection(StreamWriter writer)
    {
        string fmt = @"using System;
using UnityEngine;
";
        writer.Write(fmt);
    }
    static StreamWriter OpenFile(string fileName, bool bAppend = false)
    {
        // IMPORTANT
        // Bom (byte order mark) is not needed
        Encoding utf8NoBom = new UTF8Encoding(false);
        return new StreamWriter(fileName, bAppend, utf8NoBom);
    }

    static void HandleStringFormat(StringBuilder sb)
    {
        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");
    }

//     [MenuItem("JS for Unity/Generate JS Enum Bindings")]
//     public static void GenerateEnumBindings()
//     {
//         JSGenerator.OnBegin();
// 
//         for (int i = 0; i < JSBindingSettings.enums.Length; i++)
//         {
//             JSGenerator.Clear();
//             JSGenerator.type = JSBindingSettings.enums[i];
//             JSGenerator.GenerateEnum();
//         }
// 
//         JSGenerator.OnEnd();
// 
//         Debug.Log("Generate JS Enum Bindings finish. total = " + JSBindingSettings.enums.Length.ToString());
//     }

    /* 
     * Some classes have another name
     * for example: js has 'Object'
     */
    public static Dictionary<Type, string> typeClassName = new Dictionary<Type, string>();
    static string className = string.Empty;


    //[MenuItem("JSBinding/Generate JS Bindings")]
    public static void GenerateClassBindings()
    {
        if (!typeClassName.ContainsKey(typeof(UnityEngine.Object)))
            typeClassName.Add(typeof(UnityEngine.Object), "UnityObject");

        JSGenerator.OnBegin();

        // enums
        for (int i = 0; i < JSBindingSettings.enums.Length; i++)
        {
            JSGenerator.Clear();
            JSGenerator.type = JSBindingSettings.enums[i];
            JSGenerator.GenerateEnum();
        }

        // classes
        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            JSGenerator.Clear();
            JSGenerator.type = JSBindingSettings.classes[i];
            if (!typeClassName.TryGetValue(type, out className))
                className = type.Name;
            JSGenerator.GenerateClass();
        }

        JSGenerator.OnEnd();

        Debug.Log("Generate JS Bindings OK. enum " + JSBindingSettings.enums.Length.ToString() + ", class " + JSBindingSettings.classes.Length.ToString());
    }

    //     [MenuItem("JS for Unity/Output All Types in UnityEngine")]
    //     public static void OutputAllTypesInUnityEngine()
    //     {
    //         var asm = typeof(GameObject).Assembly;
    //         var alltypes = asm.GetTypes();
    //         var writer = new StreamWriter(tempFile, false, Encoding.UTF8);
    // 
    //         writer.WriteLine("// enum");
    //         writer.WriteLine("");
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if (alltypes[i].IsEnum)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    //         writer.WriteLine("");
    //         writer.WriteLine("// interface");
    //         writer.WriteLine("");
    // 
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if (alltypes[i].IsInterface)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    //         writer.WriteLine("");
    //         writer.WriteLine("// class");
    //         writer.WriteLine("");
    // 
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
    //                 alltypes[i].IsClass)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    // 
    //         writer.WriteLine("");
    //         writer.WriteLine("// ValueType");
    //         writer.WriteLine("");
    // 
    //         for (int i = 0; i < alltypes.Length; i++)
    //         {
    //             if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
    //                 continue;
    // 
    //             if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
    //                 !alltypes[i].IsClass && alltypes[i].IsValueType)
    //                 writer.WriteLine(alltypes[i].ToString());
    //         }
    // 
    //         writer.Close();
    // 
    //         Debug.Log("Output All Types in UnityEngine finish, file: " + tempFile);
    //         return;
    //    }
}