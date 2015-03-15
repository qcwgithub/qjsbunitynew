using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;


public static class JSGenerator2
{
    static string thisString = "this.__nativeObj";
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

    public static string SharpKitTypeName(Type type)
    {
        string name = string.Empty;
        if (type.IsByRef)
        {
            name = SharpKitTypeName(type.GetElementType());
        }
        else if (type.IsArray)
        {
            while (type.IsArray)
            {
                Type subt = type.GetElementType();
                name += SharpKitTypeName(subt) + '$';
                type = subt;
            }
            name += "Array";
        }
        else if (type.IsGenericType)
        {
            name = type.Name;
            Type[] ts = type.GetGenericArguments();
            for (int i = 0; i < ts.Length; i++)
            {
                name += "$" + SharpKitTypeName(ts[i]);
            }
        }
        else
        {
            name = type.Name;
        }
        return name;

    }
    public static string SharpKitMethodName(string methodName, ParameterInfo[] paramS, bool overloaded)
    {
        string name = methodName;
        if (overloaded)
        {
            for (int i = 0; i < paramS.Length; i++)
            {
                Type type = paramS[i].ParameterType;
                name += "$$" + SharpKitTypeName(type);
            }
            name = name.Replace("`", "$");
        }
        return name;
    }

    public static StringBuilder BuildFields(Type type, FieldInfo[] fields, int slot)
    {
        string fmt2 = @"
_jstype.{7}.get_{0} = function() [[ return CS.Call({1}, {3}, {4}, {5}{6}); ]]
_jstype.{7}.set_{0} = function(v) [[ return CS.Call({2}, {3}, {4}, {5}{6}, v); ]]
";

        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];

            sb.AppendFormat(fmt2, 
                field.Name,
                (int)JSVCall.Oper.GET_FIELD,
                (int)JSVCall.Oper.SET_FIELD, 
                slot, 
                i,
                (field.IsStatic ? "true" : "false"),
                (field.IsStatic ? "" : ", " + thisString), 
                (field.IsStatic ? "staticDefinition" : "definition"));
        }
        return sb;
    }
    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties, int slot)
    {
        string fmt2 = @"
_jstype.{7}.get_{0} = function() [[ return CS.Call({1}, {3}, {4}, {5}{6}); ]]
_jstype.{7}.set_{0} = function(v) [[ return CS.Call({2}, {3}, {4}, {5}{6}, v); ]]
";

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];
            if (property.Name == "Item") //[] not support
                continue;

            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;
            sb.AppendFormat(fmt2, 
                property.Name,                  // [0]
                (int)JSVCall.Oper.GET_PROPERTY, // [1] op
                (int)JSVCall.Oper.SET_PROPERTY, // [2] op
                slot,                           // [3]
                i,                              // [4]
                (isStatic ? "true" : "false"),  // [5] isStatic
                (isStatic ? "" : ", " + thisString),     // [6] this
                (isStatic ? "staticDefinition" : "definition"));                 // [7]
        }
        return sb;
    }
    public static StringBuilder BuildTail()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"


JsTypes.push(_jstype);");
        return sb;
    }
    public static StringBuilder BuildHeader(Type type)
    {
        string fmt =@"if (typeof(JsTypes) == 'undefined')
    var JsTypes = [];

// {0}
_jstype = 
[[
    definition: [[]],
    staticDefinition: [[]],
    assemblyName: '{1}',
    Kind: '{2}',
    fullname: '{3}',
    {4}
]];

";
        string jsTypeName = JSDataExchangeMgr.GetTypeFullName(type);
        jsTypeName = jsTypeName.Replace('.', '$');

        string assemblyName = "";
        string Kind = "unknown";
        if (type.IsClass) { Kind = "Class"; }
        else if (type.IsEnum) { Kind = "Enum"; }
        else if (type.IsValueType) { Kind = "Struct"; }

        string fullname = JSDataExchangeMgr.GetTypeFullName(type);
        string baseTypeName = JSDataExchangeMgr.GetTypeFullName(type.BaseType);

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(fmt, 
            jsTypeName, 
            assemblyName, 
            Kind, 
            fullname, 
            baseTypeName.Length > 0 ? "baseTypeName: '" + baseTypeName + "'" : "");

        return sb;
    }
    public static StringBuilder BuildConstructors(Type type, ConstructorInfo[] constructors, int slot, int howmanyConstructors)
    {
        string fmt = @"
_jstype.definition.{4} = function({5}) [[ {8} = CS.Call({0}, {1}, {2}, {3}, {6}{7}).__nativeObj; ]]";

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
            sb.AppendFormat(fmt, 
                (int)JSVCall.Oper.CONSTRUCTOR,  // [1]
                slot,                           // [2]
                i/* index */,                   // [3]
                "true"/* isStatic */,           // [4]
                SharpKitMethodName("ctor", ps, (howmanyConstructors > 1)), sbFormalParam, // [5]
                "false", /*isOverloaded*/   // [6] isOverloaded
                sbActualParam,              // [7] actual param
                thisString);                // [8] thisString
        }
        return sb;
    }

    // can handle all methods
    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods, int slot)
    {
        string fmt = @"
/* {6} */
_jstype.definition.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, false, {9}, {8}{5}); ]]";
        string fmtStatic = @"
/* static {6} {9} */
_jstype.staticDefinition.{1} = function({2}) [[ return CS.Call({7}, {3}, {4}, true, {8}{5}); ]]";

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];

            bool bOverloaded = ((i > 0 && method.Name == methods[i - 1].Name) ||
                (i < methods.Length - 1 && method.Name == methods[i + 1].Name));

            StringBuilder sbFormalParam = new StringBuilder();
            StringBuilder sbActualParam = new StringBuilder();
            ParameterInfo[] paramS = method.GetParameters();
            int L = paramS.Length;
            for (int j = 0; j < L; j++)
            {
                sbFormalParam.AppendFormat("a{0}/* {1} */{2}", j, paramS[j].ParameterType.Name, (j == L - 1 ? "" : ", "));
                sbActualParam.AppendFormat("{2}a{0}{1}", j, (j == L - 1 ? "" : ", "), (j == 0 ? ", " : ""));
            }
            if (!method.IsStatic)
                sb.AppendFormat(fmt, 
                    className, 
                    SharpKitMethodName(method.Name, paramS, bOverloaded), 
                    sbFormalParam.ToString(), 
                    slot, 
                    i, 
                    sbActualParam, 
                    method.ReturnType.Name, 
                    (int)JSVCall.Oper.METHOD, 
                    "false", 
                    thisString);
            else
                sb.AppendFormat(fmtStatic, className, SharpKitMethodName(method.Name, paramS, bOverloaded), sbFormalParam.ToString(), slot, i, sbActualParam, method.ReturnType.Name, (int)JSVCall.Oper.METHOD, "false", "");
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
        var sbHeader = BuildHeader(type);
        var sbCons = sbHeader.Append(BuildConstructors(type, ti.constructors, slot, ti.howmanyConstructors));
        var sbFields = BuildFields(type, ti.fields, slot);
        var sbProperties = BuildProperties(type, ti.properties, slot);
        var sbMethods = BuildMethods(type, ti.methods, slot);
        sbMethods.Append(BuildTail());
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
    //         JSGenerator2.OnBegin();
// 
//         for (int i = 0; i < JSBindingSettings.enums.Length; i++)
//         {
    //             JSGenerator2.Clear();
    //             JSGenerator2.type = JSBindingSettings.enums[i];
    //             JSGenerator2.GenerateEnum();
//         }
// 
    //         JSGenerator2.OnEnd();
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

        JSGenerator2.OnBegin();

        // enums
        for (int i = 0; i < JSBindingSettings.enums.Length; i++)
        {
            JSGenerator2.Clear();
            JSGenerator2.type = JSBindingSettings.enums[i];
            JSGenerator2.GenerateEnum();
        }

        // classes
        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            JSGenerator2.Clear();
            JSGenerator2.type = JSBindingSettings.classes[i];
            if (!typeClassName.TryGetValue(type, out className))
                className = type.Name;
            JSGenerator2.GenerateClass();
        }

        JSGenerator2.OnEnd();

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