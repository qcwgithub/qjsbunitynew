using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.SocialPlatforms;
using System.Runtime.InteropServices;

public static class CSGenerator2
{
    // input
    static StringBuilder sb = null;
    public static Type type = null;

    static string tempFile = JSBindingSettings.jsDir + "/temp" + JSBindingSettings.jsExtension;

    public static void OnBegin()
    {
        JSMgr.ClearTypeInfo();

        if (Directory.Exists(JSBindingSettings.csGeneratedDir))
        {
            string[] files = Directory.GetFiles(JSBindingSettings.csGeneratedDir);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }
        else
        {
            Directory.CreateDirectory(JSBindingSettings.csGeneratedDir);
        }
    }
    public static void OnEnd()
    {

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
    public static string SharpKitMethodName(string methodName, ParameterInfo[] paramS, bool overloaded, int TCounts = 0)
    {
        string name = methodName;
        if (overloaded)
        {
            if (TCounts > 0)
                name += "T" + TCounts.ToString();
            for (int i = 0; i < paramS.Length; i++)
            {
                Type type = paramS[i].ParameterType;
                name += "$$" + SharpKitTypeName(type);
            }
            name = name.Replace("`", "T");
        }
        name = name.Replace("$", "_");
        return name;
    }
    public static string BuildRetriveJSReturnValue(Type paramType)
    {
        if (paramType == typeof(object)) return "JSMgr.vCall.getRtObject()";
        else if (paramType == typeof(Boolean)) return "JSMgr.vCall.getRtBool()";
        else if (paramType == typeof(String)) return "JSMgr.vCall.getRtString()";
        else if (paramType == typeof(Char)) return "JSMgr.vCall.getRtChar()";
        else if (paramType == typeof(Byte)) return "JSMgr.vCall.getRtByte()";
        else if (paramType == typeof(SByte)) return "JSMgr.vCall.getRtSByte()";
        else if (paramType == typeof(UInt16)) return "JSMgr.vCall.getRtUInt16()";
        else if (paramType == typeof(Int16)) return "JSMgr.vCall.getRtInt16()";
        else if (paramType == typeof(UInt32)) return "JSMgr.vCall.getRtUInt32()";
        else if (paramType == typeof(Int32)) return "JSMgr.vCall.getRtInt32()";
        else if (paramType == typeof(UInt64)) return "JSMgr.vCall.getRtUInt64()";
        else if (paramType == typeof(Int64)) return "JSMgr.vCall.getRtInt64()";
        else if (paramType.IsEnum) return "(" + GetTypeFullName(paramType) + ")" + "JSMgr.vCall.getRtEnum()";
        else if (paramType == typeof(Single)) return "JSMgr.vCall.getRtFloat()";
        else if (paramType == typeof(Double)) return "JSMgr.vCall.getRtDouble()";
        else if (paramType.IsArray) return "(" + GetTypeFullName(paramType) + ")" + "JSMgr.vCall.getRtObject(typeof(" + GetTypeFullName(paramType) + "))";
        else return "(" + GetTypeFullName(paramType) + ")" + "JSMgr.vCall.getRtObject()";
    }
    public static StringBuilder BuildField_DelegateFunction(Type type, FieldInfo field)
    {
        // building a closure
        // a function having a up-value: jsFunction

        var sb = new StringBuilder();
        var sbParamList = new StringBuilder();
        ParameterInfo[] ps = field.FieldType.GetMethod("Invoke").GetParameters();
        Type returnType = field.FieldType.GetMethod("Invoke").ReturnType;
        for (int i = 0; i < ps.Length; i++)
        {
            sbParamList.AppendFormat("{0}{1}", ps[i].Name, (i == ps.Length - 1 ? "" : ","));
        }

        // this function name is used in BuildFields, don't change
        sb.AppendFormat("static {0} {1}_{2}_GetDelegate(jsval jsFunction)\n[[\n", GetTypeFullName(field.FieldType), type.Name, field.Name);
        sb.Append("    if (jsFunction.asBits == 0)\n        return null;\n");
        sb.AppendFormat("    {0} action = ({1}) => \n", GetTypeFullName(field.FieldType), sbParamList);
        sb.AppendFormat("    [[\n");
        if (sbParamList.Length > 0)
            sb.AppendFormat("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction, {0});\n", sbParamList);
        else
            sb.Append("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction);\n");

        if (returnType != typeof(void))
            sb.Append("        return (" + GetTypeFullName(returnType) + ")" + JSDataExchangeMgr.Get_GetJSReturn(returnType) + ";\n");

        sb.AppendFormat("    ]];\n");
        sb.Append("    return action;\n");
        sb.AppendFormat("]]\n");

        return sb;
    }
    public static string GetFunctionArg_DelegateFuncionName(string className, string functionName, int methodIndex, int argIndex)
    {
        return className + "_" + functionName + methodIndex.ToString() + "_" + argIndex.ToString() + "_GetDelegate";
    }
    public static StringBuilder BuildFunctionArg_DelegateFunction(string className, string functionName, Type delType, int methodIndex, int argIndex)
    {
        // building a closure
        // a function having a up-value: jsFunction

        var sb = new StringBuilder();
        var sbParamList = new StringBuilder();
        ParameterInfo[] ps = delType.GetMethod("Invoke").GetParameters();
        Type returnType = delType.GetMethod("Invoke").ReturnType;
        for (int i = 0; i < ps.Length; i++)
        {
            sbParamList.AppendFormat("{0}{1}", ps[i].Name, (i == ps.Length - 1 ? "" : ","));
        }

        // this function name is used in BuildFields, don't change
        sb.AppendFormat("static {0} {1}(jsval jsFunction)\n[[\n", GetTypeFullName(delType), GetFunctionArg_DelegateFuncionName(className, functionName, methodIndex, argIndex));
        sb.Append("    if (jsFunction.asBits == 0)\n        return null;\n");
        sb.AppendFormat("    {0} action = ({1}) => \n", GetTypeFullName(delType), sbParamList);
        sb.AppendFormat("    [[\n");
        if (sbParamList.Length > 0)
            sb.AppendFormat("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction, {0});\n", sbParamList);
        else
            sb.Append("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction);\n");

        if (returnType != typeof(void))
            sb.Append("        return (" + GetTypeFullName(returnType) + ")" + JSDataExchangeMgr.Get_GetJSReturn(returnType) + ";\n");

        sb.AppendFormat("    ]];\n");
        sb.Append("    return action;\n");
        sb.AppendFormat("]]\n");

        return sb;
    }
    public static StringBuilder BuildFields(Type type, FieldInfo[] fields, ClassCallbackNames ccbn)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < fields.Length; i++)
        {
            var sbCall = new StringBuilder();

            FieldInfo field = fields[i];
            bool isDelegate = (typeof(System.Delegate).IsAssignableFrom(field.FieldType));
            if (isDelegate)
            {
                sb.Append(BuildField_DelegateFunction(type, field));
            }

            


            sb.AppendFormat("static void {0}_{1}(JSVCall vc)\n[[\n", type.Name, field.Name);

            bool bReadOnly = (field.IsInitOnly || field.IsLiteral);
            if (!bReadOnly)
            {
                sb.Append("    if (vc.bGet) [[\n");
            }

            
            //if (type.IsValueType && !field.IsStatic)
            //    sb.AppendFormat("{0} argThis = ({0})vc.csObj;", type.Name);

            // get
            if (field.IsStatic)
                sbCall.AppendFormat("{0}.{1}", type.Name, field.Name);
            else
                sbCall.AppendFormat("(({0})vc.csObj).{1}", type.Name, field.Name);

            
            sb.AppendFormat("        {0}\n", JSDataExchangeMgr.Get_Return(field.FieldType, sbCall.ToString()));

            // set
            if (!bReadOnly)
            {
                sb.Append("    ]]\n    else [[\n");

                if (!isDelegate)
                {
                    var paramHandler = JSDataExchangeMgr.Get_ParamHandler(field);
                    sb.Append("        " + paramHandler.getter + "\n");

                    if (field.IsStatic)
                        sb.AppendFormat("        {0}.{1} = {2};\n", type.Name, field.Name, paramHandler.argName);
                    else
                    {
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("        {0} argThis = ({0})vc.csObj;\n", type.Name);
                            sb.AppendFormat("        argThis.{0} = {1};\n", field.Name, paramHandler.argName);
                            sb.Append("        JSMgr.changeJSObj(vc.jsObj, argThis);\n");
                        }
                        else
                        {
                            sb.AppendFormat("        (({0})vc.csObj).{1} = {2};\n", GetTypeFullName(type), field.Name, paramHandler.argName);
                        }
                    }
                }
                else
                {
                    var getDelegateFuncitonName = new StringBuilder();
                    getDelegateFuncitonName.AppendFormat("{0}_{1}_GetDelegate", type.Name, field.Name);

                    if (field.IsStatic)
                    {
                        sb.AppendFormat("{0}.{1} = {2}(vc.getJSFunctionValue());\n", type.Name, field.Name, getDelegateFuncitonName);
                    }
                    else
                    {
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("\n    {0} argThis = ({0})vc.csObj;\n", type.Name);
                            sb.AppendFormat("    argThis.{0} = {1}(vc.getJSFunctionValue());\n", field.Name, getDelegateFuncitonName);
                            sb.Append("    JSMgr.changeJSObj(vc.jsObj, argThis);\n\n");
                        }
                        else
                        {
                            sb.AppendFormat("(({0})vc.csObj).{1} = {2}(vc.getJSFunctionValue());\n", GetTypeFullName(type), field.Name, getDelegateFuncitonName);
                        }
                    }
                }
                sb.Append("    ]]\n");
            }

            sb.AppendFormat("]]\n");

//             string f = fmt;
//             if (field.IsStatic) f = bReadOnly ? fmtStaticReadOnly : fmtStatic;
//             else if (bReadOnly) f = fmtReadOnly;
//             else if (type.IsValueType) f = fmtValueType;
// 
//             sb.AppendFormat(f, type.Name, field.Name, field.FieldType);
            ccbn.fields.Add(type.Name + "_" + field.Name);
        }

        return sb;
    }
    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties, ClassCallbackNames ccbn)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            var sbCall = new StringBuilder();

            PropertyInfo property = properties[i];

            //
            // check to see if this is a indexer
            //
            ParameterInfo[] ps = property.GetIndexParameters();
            bool bIndexer = (ps.Length > 0);
            StringBuilder sbActualParam = null;
            JSDataExchangeMgr.ParamHandler[] paramHandlers = null;
            if (bIndexer)
            {
                sbActualParam = new StringBuilder();
                paramHandlers = new JSDataExchangeMgr.ParamHandler[ps.Length];
                for (int j = 0; j < ps.Length; j++)
                {
                    paramHandlers[j] = JSDataExchangeMgr.Get_ParamHandler(ps[j].ParameterType, j, false);
                    sbActualParam.AppendFormat("[{0}]", paramHandlers[j].argName);
                }
            }

            sb.AppendFormat("static void {0}_{1}(JSVCall vc)\n[[\n", type.Name, property.Name);            

            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;

            bool bReadOnly = !property.CanWrite;
            if (bIndexer)
            {
                for (int j = 0; j < ps.Length; j++)
                {
                    sb.Append("        " + paramHandlers[j].getter + "\n");
                }
                if (isStatic)
                {
                    sbCall.AppendFormat("{0}{1}", GetTypeFullName(type), sbActualParam);
                }
                else
                {
                    sbCall.AppendFormat("(({0})vc.csObj){1}", GetTypeFullName(type), sbActualParam);
                }
            }
            
            if (!bReadOnly)
            {
                sb.Append("    if (vc.bGet) [[ \n");
            }

            if (!bIndexer)
            {
                // get
                if (isStatic)
                    sbCall.AppendFormat("{0}.{1}", GetTypeFullName(type), property.Name);
                else
                    sbCall.AppendFormat("(({0})vc.csObj).{1}", GetTypeFullName(type), property.Name);
            }

            //if (type.IsValueType && !field.IsStatic)
            //    sb.AppendFormat("{0} argThis = ({0})vc.csObj;", type.Name);
                        
            sb.AppendFormat("        {0}", JSDataExchangeMgr.Get_Return(property.PropertyType, sbCall.ToString()));
            if (!bReadOnly)
                sb.Append("\n    ]]\n");

            // set
            if (!bReadOnly)
            {
                sb.Append("    else [[\n");

                int ParamIndex = ps.Length;

                var paramHandler = JSDataExchangeMgr.Get_ParamHandler(property.PropertyType, ParamIndex, false);
                sb.Append("        " + paramHandler.getter + "\n");

                if (bIndexer)
                {
                    if (isStatic)
                        sb.AppendFormat("{0} = {2};\n", sbCall, paramHandler.argName);
                    else
                    {
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("        {0} argThis = ({0})vc.csObj;\n", GetTypeFullName(type));
                            sb.AppendFormat("argThis{0} = {1};", sbActualParam, paramHandler.argName);
                            sb.Append("        JSMgr.changeJSObj(vc.jsObj, argThis);\n");
                        }
                        else
                        {
                            sb.AppendFormat("        {0} = {2};\n", sbCall, paramHandler.argName);
                        }
                    }
                }
                else
                {
                    if (isStatic)
                        sb.AppendFormat("{0}.{1} = {2};\n", GetTypeFullName(type), property.Name, paramHandler.argName);
                    else
                    {
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("        {0} argThis = ({0})vc.csObj;\n", GetTypeFullName(type));
                            sb.AppendFormat("        argThis.{0} = {1};\n", property.Name, paramHandler.argName);
                            sb.Append("        JSMgr.changeJSObj(vc.jsObj, argThis);\n");
                        }
                        else
                        {
                            sb.AppendFormat("        (({0})vc.csObj).{1} = {2};\n", GetTypeFullName(type), property.Name, paramHandler.argName);
                        }
                    }
                }
                sb.Append("    ]]\n");
            }

            sb.AppendFormat("]]\n");

            ccbn.properties.Add(type.Name + "_" + property.Name);
        }
        return sb;
    }
    
    static StringBuilder GenListCSParam(ParameterInfo[] ps)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(@"    vc.lstCSParam.Clear();
");

        string fmt = @"    vc.lstCSParam.Add(new JSVCall.CSParam({0}, {1}, {2}, {3}{4}, {5}));
";
        for (int i = 0; i < ps.Length; i++)
        {
            ParameterInfo p = ps[i];
            Type t = p.ParameterType;
            sb.AppendFormat(fmt, t.IsByRef?"true":"false", p.IsOptional?"true":"false", t.IsArray?"true":"false", "typeof("+GetTypeFullName(t)+")", t.IsByRef?".MakeByRefType()":"","null");
        }
        return sb;
    }
    static StringBuilder GenListCSParam2(ParameterInfo[] ps)
    {
        StringBuilder sb = new StringBuilder();

        string fmt = "new JSVCall.CSParam({0}, {1}, {2}, {3}{4}, {5}), ";
        for (int i = 0; i < ps.Length; i++)
        {
            ParameterInfo p = ps[i];
            Type t = p.ParameterType;
            sb.AppendFormat(fmt, t.IsByRef ? "true" : "false", p.IsOptional ? "true" : "false", t.IsArray ? "true" : "false", "typeof(" + GetTypeFullName(t) + ")", t.IsByRef ? ".MakeByRefType()" : "", "null");
        }
        fmt = "new JSVCall.CSParam[][[{0}]]";
        StringBuilder sbX = new StringBuilder();
        sbX.AppendFormat(fmt, sb);
        return sbX;
    }
    public static StringBuilder BuildSpecialFunctionCall(ParameterInfo[] ps, string className, string methodName, bool bStatic, bool returnVoid, Type returnType)
    {
        StringBuilder sb = new StringBuilder();
        var paramHandlers = new JSDataExchangeMgr.ParamHandler[ps.Length];
        for (int i = 0; i < ps.Length; i++)
        {
            paramHandlers[i] = JSDataExchangeMgr.Get_ParamHandler(ps[i], i);
            sb.Append("    " + paramHandlers[i].getter + "\n");
        }

        string strCall = string.Empty;

        // must be static
        if (methodName == "op_Addition")
            strCall = paramHandlers[0].argName + " + " + paramHandlers[1].argName;
        else if (methodName == "op_Subtraction")
            strCall = paramHandlers[0].argName + " - " + paramHandlers[1].argName;
        else if (methodName == "op_Multiply")
            strCall = paramHandlers[0].argName + " * " + paramHandlers[1].argName;
        else if (methodName == "op_Division")
            strCall = paramHandlers[0].argName + " / " + paramHandlers[1].argName;
        else if (methodName == "op_Equality")
            strCall = paramHandlers[0].argName + " == " + paramHandlers[1].argName;
        else if (methodName == "op_Inequality")
            strCall = paramHandlers[0].argName + " != " + paramHandlers[1].argName;

        else if (methodName == "op_UnaryNegation")
            strCall = "-" + paramHandlers[0].argName;

        else if (methodName == "op_LessThan")
            strCall = paramHandlers[0].argName + " < " + paramHandlers[1].argName;
        else if (methodName == "op_LessThanOrEqual")
            strCall = paramHandlers[0].argName + " <= " + paramHandlers[1].argName;
        else if (methodName == "op_GreaterThan")
            strCall = paramHandlers[0].argName + " > " + paramHandlers[1].argName;
        else if (methodName == "op_GreaterThanOrEqual")
            strCall = paramHandlers[0].argName + " >= " + paramHandlers[1].argName;

        else
            Debug.LogError("Unknown special name: " + methodName);

        string ret = JSDataExchangeMgr.Get_Return(returnType, strCall);
        sb.Append("    " + ret);
        return sb;
    }
    // expression getting parameter
    public static string BuildRetriveParam(Type paramType)
    {
        if (paramType == typeof(object)) return "vc.getWhatever()";
        else if (paramType == typeof(Boolean)) return "vc.getBool()";
        else if (paramType == typeof(String)) return "vc.getString()";
        else if (paramType == typeof(Char)) return "vc.getChar()";
        else if (paramType == typeof(Byte)) return "vc.getByte()";
        else if (paramType == typeof(SByte)) return "vc.getSByte()";
        else if (paramType == typeof(UInt16)) return "vc.getUInt16()";
        else if (paramType == typeof(Int16)) return "vc.getInt16()";
        else if (paramType == typeof(UInt32)) return "vc.getUInt32()";
        else if (paramType == typeof(Int32)) return "vc.getInt32()";
        else if (paramType == typeof(UInt64)) return "vc.getUInt64()";
        else if (paramType == typeof(Int64)) return "vc.getInt64()";
        else if (paramType.IsEnum) return "(" + GetTypeFullName(paramType) + ")" + "vc.getEnum()";
        else if (paramType == typeof(Single)) return "vc.getFloat()";
        else if (paramType == typeof(Double)) return "vc.getDouble()";
        else if (paramType.IsArray) return "(" + GetTypeFullName(paramType) + ")" + "vc.getObject(typeof(" + GetTypeFullName(paramType) + "))";
        else return "(" + GetTypeFullName(paramType) + ")" + "vc.getObject()";
    }
    public static string BuildReturnObject(Type paramType, string callString)
    {
        if (paramType == typeof(Boolean)) return "vc.returnBool(" + callString + ")";
        else if (paramType == typeof(String)) return "vc.returnString(" + callString + ")";
        else if (paramType == typeof(Char)) return "vc.returnChar(" + callString + ")";
        else if (paramType == typeof(Byte)) return "vc.returnByte(" + callString + ")";
        else if (paramType == typeof(SByte)) return "vc.returnSByte(" + callString + ")";
        else if (paramType == typeof(UInt16)) return "vc.returnUInt16(" + callString + ")";
        else if (paramType == typeof(Int16)) return "vc.returnInt16(" + callString + ")";
        else if (paramType == typeof(UInt32)) return "vc.returnUInt32(" + callString + ")";
        else if (paramType == typeof(Int32)) return "vc.returnInt32(" + callString + ")";
        else if (paramType == typeof(UInt64)) return "vc.returnUInt64(" + callString + ")";
        else if (paramType == typeof(Int64)) return "vc.returnInt64(" + callString + ")";
        else if (paramType.IsEnum) return "vc.returnEnum((Int32)" + callString + ")";
        else if (paramType == typeof(Single)) return "vc.returnFloat(" + callString + ")";
        else if (paramType == typeof(Double)) return "vc.returnDouble(" + callString + ")";
        else return "vc.returnObject(\"" + paramType.Name + "\", " + callString + ")";
    }
    // is directly return
    // true -> 'returnBool(...)' or 'returnInt(...)'
    // false -> a name must be specified for 'returnObject(name, ...)'
    public static bool IsDirectReturn(Type paramType)
    {
        if (paramType == typeof(Boolean)) return true;
        else if (paramType == typeof(String)) return true;
        else if (paramType == typeof(Char)) return true;
        else if (paramType == typeof(Byte)) return true;
        else if (paramType == typeof(SByte)) return true;
        else if (paramType == typeof(UInt16)) return true;
        else if (paramType == typeof(Int16)) return true;
        else if (paramType == typeof(UInt32)) return true;
        else if (paramType == typeof(Int32)) return true;
        else if (paramType == typeof(UInt64)) return true;
        else if (paramType == typeof(Int64)) return true;
        else if (paramType.IsEnum) return true;
        else if (paramType == typeof(Single)) return true;
        else if (paramType == typeof(Double)) return true;
        else return false;
    }
    public static StringBuilder BuildNormalFunctionCall(int methodIndex, ParameterInfo[] ps, string className, string methodName, bool bStatic, bool returnVoid, Type returnType, bool bConstructor,
        int TCount = 0)
    {
        StringBuilder sb = new StringBuilder();
        if (TCount > 0)
        {
            StringBuilder sbt = new StringBuilder();
            sbt.Append("    // Get generic method by name and param count.\n");
            sbt.AppendFormat("    MethodInfo method = JSDataExchangeMgr.MakeGenericFunction(typeof({0}), \"{1}\", {2}, {3}, vc); \n",
                GetTypeFullName(type), // [0] typeName
                methodName,            // [1] method name
                TCount,                // [2] t count
                ps.Length);            // [3] param count
            sbt.AppendFormat("    if (method == null)\n        return true;\n");
            sbt.Append("\n");

            sb.Append(sbt);

//            var tHandlers = new JSDataExchangeMgr.ParamHandler[TCount];
//
//            /*
//             * Get Method T
//             */
//            sbt.Append("    // Get generic method by name and param count.\n");
//            sbt.AppendFormat("    MethodInfo methodT = JSDataExchangeMgr.GetGenericMethodInfo(typeof({0}), \"{1}\", {2});\n",
//                GetTypeFullName(type), // [0] type full name
//                methodName,            // [1] method name
//                ps.Length);            // [2] parameter count
//
//            sbt.AppendFormat("    if (methodT == null)\n        return true;\n");
//            sbt.Append("\n");
//            sbt.Append("    // Get generic types from js.\n");
//
//            /*
//             * Make Generic Method
//             */
//            string actualParam = string.Empty;
//            for (int i = 0; i < TCount; i++) 
//            {
//                tHandlers[i] = JSDataExchangeMgr.Get_TType(i);
//
//                sbt.Append("    " + tHandlers[i].getter + "\n");
//                sbt.AppendFormat("    if ({0} == null)\n        return true;\n", tHandlers[i].argName);
//
//                actualParam += tHandlers[i].argName;
//                if (i != TCount - 1) 
//                    actualParam += ", ";
//            }
//            sbt.Append("\n");
//            sbt.Append("    // Make generic method.\n");
//            sbt.AppendFormat("    MethodInfo method = methodT.MakeGenericMethod(new Type[][[{0}]]);\n", actualParam);
//            sbt.AppendFormat("    if (method == null)\n        return true;\n");
//            sbt.Append("\n");
//
//            /*
//             * if there is T in params
//             * call method.getparameters
//             */
//            for (int i = 0; i < ps.Length; i++)
//            {
//                if (ps[i].ParameterType.IsGenericParameter)
//                {
//                    sbt.Append("    ParameterInfo[] ps = method.GetParameters();\n");
//                    break;
//                }
//            }
//
//            sb.Append(sbt);
        }


        bool directReturn = true;
        if (!bConstructor)
            directReturn = IsDirectReturn(returnType);

        var paramHandlers = new JSDataExchangeMgr.ParamHandler[ps.Length];        
        for (int i = 0; i < ps.Length; i++)
        {
            if (true /* !ps[i].ParameterType.IsGenericParameter */ )
            {
                // use original method's parameterinfo
                paramHandlers[i] = JSDataExchangeMgr.Get_ParamHandler(ps[i], i);
                if (ps[i].ParameterType.IsGenericParameter)
                {
                    paramHandlers[i].getter = "    vc.datax.setTemp(ps[" + i.ToString() + "]);\n" + paramHandlers[i].getter;
                }
            }
        }

        // minimal params needed
        int minNeedParams = 0;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].IsOptional) { break; }
            minNeedParams++;
        }


        if (TCount == 0)
            sb.AppendFormat("    int len = count;\n");
        else
            sb.AppendFormat("    int len = count - {0};\n", TCount);

        for (int j = minNeedParams; j <= ps.Length; j++)
        {
            StringBuilder sbGetParam = new StringBuilder();
            StringBuilder sbActualParam = new StringBuilder();
            StringBuilder sbUpdateRefParam = new StringBuilder();

            // receive arguments first
            for (int i = 0; i < j; i++)
            {
                ParameterInfo p = ps[i];
                if (typeof(System.Delegate).IsAssignableFrom(p.ParameterType))
                    sbGetParam.AppendFormat("        {0} arg{1} = {2}(vc.getJSFunctionValue());\n", GetTypeFullName(p.ParameterType), i, GetFunctionArg_DelegateFuncionName(className, methodName, methodIndex, i));
                else
                    sbGetParam.Append("        " + paramHandlers[i].getter + "\n");

                // value type array
                // no 'out' nor 'ref'
                if ((p.ParameterType.IsByRef || p.IsOut) && !p.ParameterType.IsArray)
                    sbActualParam.AppendFormat("{0} arg{1}{2}", (p.IsOut) ? "out" : "ref", i, (i == j - 1 ? "" : ", "));
                else
                    sbActualParam.AppendFormat("arg{0}{1}", i, (i == j - 1 ? "" : ", "));

                // updater
                sbUpdateRefParam.Append(paramHandlers[i].updater);
            }

            /*
             * 0 parameters count
             * 1 class name
             * 2 function name
             * 3 actual parameters
             */
            if (bConstructor)
            {
                StringBuilder sbCall = new StringBuilder();
                sbCall.AppendFormat("new {0}({1})", GetTypeFullName(type), sbActualParam.ToString());
                string callAndReturn = JSDataExchangeMgr.Get_Return(type/*don't use returnType*/, sbCall.ToString());
                sb.AppendFormat(@"    {1}if (len == {0}) 
    [[
{2}
        {3}
{4}
    ]]
",
                 j,                  // [0] param length
                 (j == minNeedParams) ? "" : "else ", // [1] else
                 sbGetParam,         // [2] get param
                 callAndReturn,      // [3] 
                 sbUpdateRefParam);  // [4] update ref/out params
            }
            else
            {
                StringBuilder sbCall = new StringBuilder();

                if (TCount == 0)
                {
                    if (bStatic)
                        sbCall.AppendFormat("{0}.{1}({2})", GetTypeFullName(type), methodName, sbActualParam.ToString());
                    else if (!type.IsValueType)
                        sbCall.AppendFormat("(({0})vc.csObj).{1}({2})", GetTypeFullName(type), methodName, sbActualParam.ToString());
                    else
                        sbCall.AppendFormat("argThis.{0}({1})", methodName, sbActualParam.ToString());
                }
                else
                {
                    var sbActualParamT = new StringBuilder();
                    if (ps.Length > 0) sbActualParamT.AppendFormat("new object[][[{0}]]", sbActualParam);
                    else sbActualParamT.Append("null");

                    if (bStatic) {
                        sbCall.AppendFormat("method.Invoke(null, {0})", sbActualParamT);
                    }
                    else if (!type.IsValueType)
                    {
                        sbCall.AppendFormat("method.Invoke(({1})vc.csObj, {0})", sbActualParamT, GetTypeFullName(type));
                    }
                    else
                    {
                        sbCall.AppendFormat("method.Invoke(argThis, {0})", sbActualParamT);
                    }
                }

                string callAndReturn = JSDataExchangeMgr.Get_Return(returnType, sbCall.ToString());

                StringBuilder sbStruct = null;
                if (type.IsValueType && !bStatic)
                {
                    sbStruct = new StringBuilder();
                    sbStruct.AppendFormat("{0} argThis = ({0})vc.csObj;", GetTypeFullName(type));
                }

                sb.AppendFormat(@"    {1}if (len == {0}) 
    [[
{5}
        {2}
        {3}
        {4}
{6}
    ]]
",
                 j, // [0] param count
                 (j == minNeedParams) ? "" : "else ",  // [1] else
                 (type.IsValueType && !bStatic) ? sbStruct.ToString() : "",  // [2] if Struct, get argThis first
                 callAndReturn,  // [3] function call and return to js
                 (type.IsValueType && !bStatic) ? "JSMgr.changeJSObj(vc.jsObj, argThis);" : "",  // [4] if Struct, update 'this' object
                 sbGetParam,        // [5] get param
                 sbUpdateRefParam); // [6] update ref/out param

            }
        }

        return sb;
    }
    public static string GetTypeFullName(Type type)
    {
        if (type.IsByRef)
            type = type.GetElementType();

        if (!type.IsGenericType)
        {
            string rt = type.FullName;
            rt = rt.Replace('+', '.');
            return rt;
        }
        else
        {
            string fatherName = type.Name.Substring(0, type.Name.Length - 2);
            Type[] ts = type.GetGenericArguments();
            fatherName += "<";
            for (int i = 0; i < ts.Length; i++)
            {
                fatherName += ts[i].Name;
                if (i != ts.Length - 1)
                    fatherName += ", ";
            }
            fatherName += ">";
            fatherName.Replace('+', '.');
            return fatherName;
        }
    }
    public static StringBuilder BuildConstructors(Type type, ConstructorInfo[] constructors, ClassCallbackNames ccbn)
    {
        /*
        * methods
        * 0 function name
        * 1 list<CSParam> generation
        * 2 function call
        */
        string fmt = @"
static bool {0}(JSVCall vc, int start, int count)
[[
{1}
    return true;
]]
";
        StringBuilder sb = new StringBuilder();
        if (constructors.Length == 0 && JSBindingSettings.IsGeneratedDefaultConstructor(type) &&
            (type.IsValueType || (type.IsClass && !type.IsAbstract && !type.IsInterface)))
        {
            int olIndex = 1;
            bool returnVoid = false;
            string functionName = type.Name + "_" + type.Name +
                (olIndex > 0 ? olIndex.ToString() : "") + "";// (cons.IsStatic ? "_S" : "");
            sb.AppendFormat(fmt, functionName,
                BuildNormalFunctionCall(0, new ParameterInfo[0], type.Name, type.Name, false, returnVoid, null, true));

            ccbn.constructors.Add(functionName);
            ccbn.constructorsCSParam.Add(GenListCSParam2(new ParameterInfo[0]).ToString());        
        }

        for (int i = 0; i < constructors.Length; i++)
        {
            ConstructorInfo cons = constructors[i];
            ParameterInfo[] paramS = cons.GetParameters();

            int olIndex = i + 1; // for constuctors, they are always overloaded
            bool returnVoid = false;

            string functionName = type.Name + "_" + type.Name + (olIndex > 0 ? olIndex.ToString() : "") + (cons.IsStatic ? "_S" : "");


            sb.AppendFormat(fmt, functionName,
                BuildNormalFunctionCall(i, paramS, type.Name, cons.Name, cons.IsStatic, returnVoid, null, true));

            ccbn.constructors.Add(functionName);
            ccbn.constructorsCSParam.Add(GenListCSParam2(paramS).ToString());
        }
        return sb;
    }
    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods, int[] olInfo, ClassCallbackNames ccbn)
    {
        /*
        * methods
        * 0 function name
        * 1 list<CSParam> generation
        * 2 function call
        */
        string fmt = @"
static bool {0}(JSVCall vc, int start, int count)
[[
{1}
    return true;
]]
";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < methods.Length; i++)
        {
            MethodInfo method = methods[i];
            ParameterInfo[] paramS = method.GetParameters();

            for (int j = 0; j < paramS.Length; j++)
            {
                if (typeof(System.Delegate).IsAssignableFrom(paramS[j].ParameterType))
                {
                    StringBuilder sbD = BuildFunctionArg_DelegateFunction(type.Name, method.Name, paramS[j].ParameterType, i, j);
                    sb.Append(sbD);
                }
            }

            int olIndex = olInfo[i];
            bool returnVoid = (method.ReturnType == typeof(void));

            string functionName = type.Name + "_" + method.Name + (olIndex > 0 ? olIndex.ToString() : "") + (method.IsStatic ? "_S" : "");
            
            int TCount = 0;
            if (method.IsGenericMethodDefinition) {
                TCount = method.GetGenericArguments().Length;
            }

            functionName = type.Name + "_" + SharpKitMethodName(method.Name, paramS, true, TCount);
            if (UnityEngineManual.isManual(functionName))
            {
                sb.AppendFormat(fmt, functionName, "    UnityEngineManual." + functionName + "(vc, start, count);");
            }
            else
            {
                sb.AppendFormat(fmt, functionName,

                    method.IsSpecialName ? BuildSpecialFunctionCall(paramS, type.Name, method.Name, method.IsStatic, returnVoid, method.ReturnType)
                    : BuildNormalFunctionCall(i, paramS, type.Name, method.Name, method.IsStatic, returnVoid, method.ReturnType, false, TCount));
            }

            ccbn.methods.Add(functionName);
            ccbn.methodsCSParam.Add(GenListCSParam2(paramS).ToString());
        }
        return sb;
    }
    public static StringBuilder BuildClass(Type type, StringBuilder sbFields, StringBuilder sbProperties, StringBuilder sbMethods, StringBuilder sbConstructors, StringBuilder sbRegister)
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
////////////////////// {0} ///////////////////////////////////////
// constructors
{4}
// fields
{1}
// properties
{2}
// methods
{3}

//register
{5}
";
        var sb = new StringBuilder();
        sb.AppendFormat(fmt, type.Name, sbFields.ToString(), sbProperties.ToString(), sbMethods.ToString(), sbConstructors.ToString(), sbRegister.ToString());
        return sb;
    }

    // used for record information
    public class ClassCallbackNames
    {
        // class type
        public Type type;

        public List<string> fields;
        public List<string> properties;
        public List<string> constructors;
        public List<string> methods;

        // genetated, generating CSParam code
        public List<string> constructorsCSParam;
        public List<string> methodsCSParam;
    }
    public static List<ClassCallbackNames> allClassCallbackNames;
    static StringBuilder BuildRegisterFunction(ClassCallbackNames ccbn, JSMgr.ATypeInfo ti)
    {
        string fmt = @"
public static void __Register()
[[
    JSMgr.CallbackInfo ci = new JSMgr.CallbackInfo();
    ci.type = typeof({0});
    ci.fields = new JSMgr.CSCallbackField[]
    [[
{1}
    ]];
    ci.properties = new JSMgr.CSCallbackProperty[]
    [[
{2}
    ]];
    ci.constructors = new JSMgr.MethodCallBackInfo[]
    [[
{3}
    ]];
    ci.methods = new JSMgr.MethodCallBackInfo[]
    [[
{4}
    ]];
    JSMgr.allCallbackInfo.Add(ci);
]]
";
        StringBuilder sb = new StringBuilder();

        StringBuilder sbField = new StringBuilder();
        StringBuilder sbProperty = new StringBuilder();
        StringBuilder sbCons = new StringBuilder();
        StringBuilder sbMethod = new StringBuilder();

        for (int i = 0; i < ccbn.fields.Count; i++)
            sbField.AppendFormat("        {0},\n", ccbn.fields[i]);
        for (int i = 0; i < ccbn.properties.Count; i++)
            sbProperty.AppendFormat("        {0},\n", ccbn.properties[i]);
        for (int i = 0; i < ccbn.constructors.Count; i++)
        {
            if (ccbn.constructors.Count == 1 && ti.constructors.Length == 0) // no constructors   add a default  so ...
                sbCons.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\n", ccbn.constructors[i], ccbn.constructorsCSParam[i], type.Name);
            else
                sbCons.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\n", ccbn.constructors[i], ccbn.constructorsCSParam[i], ti.constructors[i].Name);
        }
        for (int i = 0; i < ccbn.methods.Count; i++)
        {
            // if method is not overloaded
            // don's save the cs param array
            sbMethod.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\n", ccbn.methods[i], 
                (ti.methodsOLInfo[i] > 0 ? ccbn.methodsCSParam[i] : "null"), 
                ti.methods[i].Name);
        }

        sb.AppendFormat(fmt, GetTypeFullName(ccbn.type), sbField, sbProperty, sbCons, sbMethod);
        return sb;
    }
    public static void GenerateRegisterAll()
    {
        string fmt = @"
public class CSharpGenerated
[[
    public static void RegisterAll()
    [[
{0}
    ]]
]]
";
        StringBuilder sbA = new StringBuilder();
        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            sbA.AppendFormat("        {0}Generated.__Register();\n", JSDataExchangeMgr.GetTypeFullName(JSBindingSettings.classes[i]).Replace('.', '_'));
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(fmt, sbA);
        HandleStringFormat(sb);

        sb.Replace("\r\n", "\n");

        string fileName = JSBindingSettings.csGeneratedDir + "/" + "CSharpGenerated.cs";
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sb.ToString());
        writer2.Close();
    }
    public static void GenerateAllJSFileNames()
    {
//         if (!JSGenerator2.typeClassName.ContainsKey(typeof(UnityEngine.Object)))
//             JSGenerator2.typeClassName.Add(typeof(UnityEngine.Object), "UnityObject");

        string fmt = @"
public class JSGeneratedFileNames
[[
    public static string[] names = new string[]
    [[
{0}
    ]];
]]
";
        StringBuilder sbA = new StringBuilder();
        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            string name = JSDataExchangeMgr.GetTypeFullName(JSBindingSettings.classes[i]).Replace('.', '_');
            if (JSGenerator2.typeClassName.ContainsKey(JSBindingSettings.classes[i]))
                name = JSGenerator2.typeClassName[JSBindingSettings.classes[i]];
            sbA.AppendFormat("        \"{0}\",\n", name);
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(fmt, sbA);
        HandleStringFormat(sb);

        sb.Replace("\r\n", "\n");

        string fileName = JSBindingSettings.csGeneratedDir + "/" + "AllJSFileNames.cs";
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sb.ToString());
        writer2.Close();
    }
    public static void GenerateClass()
    {
        /*if (type.IsInterface)
        {
            Debug.Log("Interface: " + type.ToString() + " ignored.");
            return;
        }*/

        JSMgr.ATypeInfo ti;
        /*int slot = */JSMgr.AddTypeInfo(type, out ti);
//         var sbCons = BuildConstructors(type, ti.constructors, slot);
//         var sbFields = BuildFields(type, ti.fields, slot);
//         var sbProperties = BuildProperties(type, ti.properties, slot);
//         var sbMethods = BuildMethods(type, ti.methods, slot);
//         var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods, sbCons);
//         HandleStringFormat(sbClass);

        ClassCallbackNames ccbn = new ClassCallbackNames();
        {
            ccbn.type = type;
            ccbn.fields = new List<string>(ti.fields.Length);
            ccbn.properties = new List<string>(ti.properties.Length);
            ccbn.constructors = new List<string>(ti.constructors.Length);
            ccbn.methods = new List<string>(ti.methods.Length);

            ccbn.constructorsCSParam = new List<string>(ti.constructors.Length);
            ccbn.methodsCSParam = new List<string>(ti.methods.Length);
        }

        var sbFields = BuildFields(type, ti.fields, ccbn);
        var sbProperties = BuildProperties(type, ti.properties, ccbn);
        var sbMethods = BuildMethods(type, ti.methods, ti.methodsOLInfo, ccbn);
        var sbCons = BuildConstructors(type, ti.constructors, ccbn);
        var sbRegister = BuildRegisterFunction(ccbn, ti);
        var sbClass = BuildClass(type, sbFields, sbProperties, sbMethods, sbCons, sbRegister);

        /*
         * 0 typeName
         * 1 class contents
         * 2 type namespace
         */
        string fmtFile = @"
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
{2}

using jsval = JSApi.jsval;

public class {0}Generated
[[
{1}
]]
";
        var sbFile = new StringBuilder();
        string nameSpaceString = string.Empty;
        if (type.Namespace != null)
        {
            nameSpaceString = type.Namespace;
            if (nameSpaceString == "UnityEngine")
                nameSpaceString = string.Empty;
        }
        sbFile.AppendFormat(fmtFile, JSDataExchangeMgr.GetTypeFullName(type).Replace('.', '_'), sbClass, nameSpaceString.Length > 0 ? "using " + nameSpaceString + ";" : "");
        HandleStringFormat(sbFile);

        sbFile.Replace("\r\n", "\n");

        string fileName = JSBindingSettings.csGeneratedDir + "/" +
            JSDataExchangeMgr.GetTypeFullName(type).Replace('.', '_') + 
            "Generated.cs";
        var writer2 = OpenFile(fileName, false);
        writer2.Write(sbFile.ToString());
        writer2.Close();
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
        return new StreamWriter(fileName, bAppend, Encoding.UTF8);
    }

    static void HandleStringFormat(StringBuilder sb)
    {
        sb.Replace("[[", "{");
        sb.Replace("]]", "}");
        sb.Replace("'", "\"");
    }

    /* 
     * Some classes have another name
     * for example: js has 'Object'
     */
    //static Dictionary<Type, string> typeClassName = new Dictionary<Type, string>();
    //static string className = string.Empty;

//     public class TEST2
//     {
//         public void Add()
//        
    public static void MakeJJJ(ref int i)
    {

    }
    //[MenuItem("JSBinding/Generate CS Bindings")]
    public static void GenerateClassBindings()
    {
//         typeClassName.Add(typeof(UnityEngine.Object), "UnityObject");

//         Type t = typeof(Dictionary<int,string>);
//         Debug.Log(t);
//         Debug.Log(t.Name);
//         Debug.Log(t.FullName);
//         Debug.Log(t.ToString());
//         Type tD = t.GetGenericTypeDefinition();
//         Debug.Log(tD);
//         Debug.Log(tD.Name);
//         Debug.Log(tD.FullName);
//         Debug.Log(tD.ToString());
        //int op = 1;
        //object oj = op;
        //Debug.Log(GetTypeFullName(typeof(bool).MakeByRefType()));
        //MakeJJJ(ref oj);
//         {
//             Type t = typeof(GameObject);
//             MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
//             for (int i = 0; i < methods.Length; i++ )
//             {
//                 MethodInfo method = methods[i];
//                 if (method.Name != "AddComponent" || method.IsGenericMethod || method.IsGenericMethodDefinition)
//                     continue;
// 
//                 ParameterInfo[] ps = method.GetParameters();
//                 bool b1 = ps[0].ParameterType.IsGenericParameter;
//                 bool b2 = ps[0].ParameterType.IsGenericType;
//                 bool b3 = ps[0].ParameterType.IsGenericTypeDefinition;
//                 Type[] ga = ps[0].ParameterType.GetGenericArguments();
//                 Debug.Log(b1.ToString() + b2.ToString() + b3.ToString());
//             }
//         }
        //return;
// 

        /*Type t = typeof(Kekoukele);
        FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Static);
		for (int i = 0; i < fields.Length; i++)
		{
			// if ( typeof(System.Delegate).IsAssignableFrom(fields[i].FieldType))
            if (fields[i].FieldType.BaseType == typeof(System.MulticastDelegate))
			{
				Debug.Log (fields[i].FieldType.ToString () + " is delegate!"); 
			}
		}
        return;*/



        CSGenerator2.OnBegin();

        allClassCallbackNames = null;
        allClassCallbackNames = new List<ClassCallbackNames>(JSBindingSettings.classes.Length);

        for (int i = 0; i < JSBindingSettings.classes.Length; i++)
        {
            CSGenerator2.Clear();
            CSGenerator2.type = JSBindingSettings.classes[i];
            CSGenerator2.GenerateClass();
        }
        GenerateRegisterAll();
        GenerateAllJSFileNames();

        CSGenerator2.OnEnd();

        Debug.Log("Generate CS Bindings OK. total = " + JSBindingSettings.classes.Length.ToString());
    }

    // Alt + Shift + Q
    [MenuItem("JSB/Copy GameObject MonoBehaviours &#q")]
    public static void CopyGameObjectMonoBehaviours()
    {
        GameObject go = Selection.activeGameObject;
        ExtraHelper.CopyGameObject<JSComponent_SharpKit>(go);
    }

    [MenuItem("Assets/JSBinding/Read serialized data test")]
    public static void ReadSerializedDataTest()
    {
        AssetDatabase.FindAssets("backgrounds", new string[]{"Assets/Prefabs/Environment"});
    }
    [MenuItem("Assets/JSBinding/Generate JS and CS Bindings2")]
    public static void GenerateJSCSBindings()
    {
        JSDataExchangeMgr.reset();
        UnityEngineManual.initManual();
        CSGenerator2.GenerateClassBindings();
        JSGenerator2.GenerateClassBindings();
        UnityEngineManual.afterUse();
        AssetDatabase.Refresh();
    }

    
    [MenuItem("Assets/JSBinding/1Output All T Functions")]
    public static void OutputAllTFunctionsInUnityEngine()
    {
        var writer = new StreamWriter(tempFile, false, Encoding.UTF8);
        var asm = typeof(GameObject).Assembly;
        var alltypes = asm.GetTypes();

        for (int i = 0; i < alltypes.Length; i++)
        {
            Type type = alltypes[i];
            // if (type.IsClass)
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            for (int j = 0; j < methods.Length; j++)
            {
                var method = methods[j];
                if (method.IsGenericMethod || method.IsGenericMethodDefinition)
                {
                    writer.WriteLine(type.ToString() + "." + method.Name + "\n");
                }
            }
        }
        writer.Close();

        Debug.Log("Output All T Functions finish, file: " + tempFile);
    }

    [MenuItem("Assets/JSBinding/Output All Types in UnityEngine2")]
    public static void OutputAllTypesInUnityEngine()
    {
        var asm = typeof(GameObject).Assembly;
        var alltypes = asm.GetTypes();
        var writer = new StreamWriter(tempFile, false, Encoding.UTF8);

        writer.WriteLine("// enum");
        writer.WriteLine("");
        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if (alltypes[i].IsEnum)
                writer.WriteLine(alltypes[i].ToString());
        }

        writer.WriteLine("");
        writer.WriteLine("// interface");
        writer.WriteLine("");

        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if (alltypes[i].IsInterface)
                writer.WriteLine(alltypes[i].ToString());
        }

        writer.WriteLine("");
        writer.WriteLine("// class");
        writer.WriteLine("");

        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
                alltypes[i].IsClass)
                writer.WriteLine(alltypes[i].ToString());
        }


        writer.WriteLine("");
        writer.WriteLine("// ValueType");
        writer.WriteLine("");

        for (int i = 0; i < alltypes.Length; i++)
        {
            if (!alltypes[i].IsPublic && !alltypes[i].IsNestedPublic)
                continue;

            if ((!alltypes[i].IsEnum && !alltypes[i].IsInterface) &&
                !alltypes[i].IsClass && alltypes[i].IsValueType)
                writer.WriteLine(alltypes[i].ToString());
        }

        writer.Close();

        Debug.Log("Output All Types in UnityEngine finish, file: " + tempFile);
        return;
    }
}
