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
    public static string thisClassName = null;

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
        sb.AppendFormat("static {0} {1}_{2}_GetDelegate(jsval jsFunction)\n[[\n", JSDataExchangeMgr.GetTypeFullName(field.FieldType), type.Name, field.Name);
        sb.Append("    if (jsFunction.asBits == 0)\n        return null;\n");
        sb.AppendFormat("    {0} action = ({1}) => \n", JSDataExchangeMgr.GetTypeFullName(field.FieldType), sbParamList);
        sb.AppendFormat("    [[\n");
        if (sbParamList.Length > 0)
            sb.AppendFormat("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction, {0});\n", sbParamList);
        else
            sb.Append("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction);\n");

        if (returnType != typeof(void))
            sb.Append("        return (" + JSDataExchangeMgr.GetTypeFullName(returnType) + ")" + JSDataExchangeMgr.Get_GetJSReturn(returnType) + ";\n");

        sb.AppendFormat("    ]];\n");
        sb.Append("    return action;\n");
        sb.AppendFormat("]]\n");

        return sb;
    }
    public static string GetFunctionArg_DelegateFuncionName(string className, string methodName, int methodIndex, int argIndex)
    {
        return JSDataExchangeMgr.HandleFunctionName(className + "_" + methodName + methodIndex.ToString() + "_" + argIndex.ToString() + "_GetDelegate");
    }
    public static StringBuilder BuildFunctionArg_DelegateFunction(string className, string methodName, Type delType, int methodIndex, int argIndex)
    {
        // building a closure
        // a function having a up-value: jsFunction

        methodName = JSDataExchangeMgr.HandleFunctionName(methodName);

        var sb = new StringBuilder();
        var sbParamList = new StringBuilder();
        ParameterInfo[] ps = delType.GetMethod("Invoke").GetParameters();
        Type returnType = delType.GetMethod("Invoke").ReturnType;
        for (int i = 0; i < ps.Length; i++)
        {
            sbParamList.AppendFormat("{0}{1}", ps[i].Name, (i == ps.Length - 1 ? "" : ","));
        }

        string stringTOfMethod = string.Empty;
        if (delType.IsGenericType)
        {
            var arg = new cg.args();
            foreach (var t in delType.GetGenericArguments())
            {
                arg.Add(t.Name);
            }
            stringTOfMethod = arg.Format(cg.args.ArgsFormat.GenericT);
        }

        // this function name is used in BuildFields, don't change
        sb.AppendFormat("static {0} {1}{2}(jsval jsFunction)\n[[\n", 
            JSDataExchangeMgr.GetTypeFullName(delType),  // [0]
            GetFunctionArg_DelegateFuncionName(className, methodName, methodIndex, argIndex), // [2]
            stringTOfMethod  // [1]
            );
        sb.Append("    if (jsFunction.asBits == 0)\n        return null;\n");
        sb.AppendFormat("    {0} action = ({1}) => \n", JSDataExchangeMgr.GetTypeFullName(delType), sbParamList);
        sb.AppendFormat("    [[\n");
        if (sbParamList.Length > 0)
            sb.AppendFormat("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction, {0});\n", sbParamList);
        else
            sb.Append("        JSMgr.vCall.CallJSFunctionValue(IntPtr.Zero, ref jsFunction);\n");

        if (returnType != typeof(void))
            sb.Append("        return (" + JSDataExchangeMgr.GetTypeFullName(returnType) + ")" + JSDataExchangeMgr.Get_GetJSReturn(returnType) + ";\n");

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
                            sb.AppendFormat("        (({0})vc.csObj).{1} = {2};\n", JSDataExchangeMgr.GetTypeFullName(type), field.Name, paramHandler.argName);
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
                            sb.AppendFormat("(({0})vc.csObj).{1} = {2}(vc.getJSFunctionValue());\n", JSDataExchangeMgr.GetTypeFullName(type), field.Name, getDelegateFuncitonName);
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
    public static StringBuilder BuildPropertiesTypeT(Type type, PropertyInfo[] properties, int[] propertiesIndex, ClassCallbackNames ccbn)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            var sbCall = new StringBuilder();

            PropertyInfo property = properties[i];

            bool bT = type.IsGenericTypeDefinition;
            StringBuilder sbt = null;
            if (bT)
            {
                sbt = new StringBuilder();

                sbt.AppendFormat("    PropertyInfo property = JSDataExchangeMgr.GetPropertyInfoOfGenericClass(vc.csObj.GetType(), {0}); \n",
                        propertiesIndex[i]);        // [0] methodArrIndex

                sbt.AppendFormat("    if (property == null)\n        return true;\n");
                sbt.Append("\n");

                sb.Append(sbt);
            }

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
                sbActualParam.Append("[");
                for (int j = 0; j < ps.Length; j++)
                {
                    paramHandlers[j] = JSDataExchangeMgr.Get_ParamHandler(ps[j].ParameterType, j, false, false);
                    sbActualParam.AppendFormat("{0}", paramHandlers[j].argName);
                    if (j != ps.Length - 1)
                        sbActualParam.Append(", ");
                }
                sbActualParam.Append("]");
            }

            string functionName = type.Name + "_" + property.Name;
            if (bIndexer)
            {
                foreach (var p in ps)
                {
                    functionName += "_" + p.ParameterType.Name;
                }
            }
            functionName = JSDataExchangeMgr.HandleFunctionName(functionName);

            sb.AppendFormat("static void {0}(JSVCall vc)\n[[\n", functionName);

            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;

            bool bReadOnly = !property.CanWrite;
            if (bIndexer)
            {
                for (int j = 0; j < ps.Length; j++)
                {
                    sb.Append("        " + paramHandlers[j].getter + "\n");
                }
                if (bT)
                {
                    if (isStatic)
                    {
                        sbCall.AppendFormat("{0}{1}", JSDataExchangeMgr.GetTypeFullName(type), sbActualParam);
                    }
                    else
                    {
                        sbCall.AppendFormat("(({0})vc.csObj){1}", JSDataExchangeMgr.GetTypeFullName(type), sbActualParam);
                    }
                }
                else
                {
                    if (isStatic)
                    {
                        sbCall.AppendFormat("{0}{1}", JSDataExchangeMgr.GetTypeFullName(type), sbActualParam);
                    }
                    else
                    {
                        sbCall.AppendFormat("(({0})vc.csObj){1}", JSDataExchangeMgr.GetTypeFullName(type), sbActualParam);
                    }
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
                    sbCall.AppendFormat("{0}.{1}", JSDataExchangeMgr.GetTypeFullName(type), property.Name);
                else
                    sbCall.AppendFormat("(({0})vc.csObj).{1}", JSDataExchangeMgr.GetTypeFullName(type), property.Name);
            }

            //if (type.IsValueType && !field.IsStatic)
            //    sb.AppendFormat("{0} argThis = ({0})vc.csObj;", type.Name);

            sb.AppendFormat("        {0}", JSDataExchangeMgr.Get_Return(property.PropertyType, sbCall.ToString()));
            if (!bReadOnly)
            {
                sb.Append("\n    ]]\n");
            }

            // set
            if (!bReadOnly)
            {
                sb.Append("    else [[\n");

                int ParamIndex = ps.Length;

                var paramHandler = JSDataExchangeMgr.Get_ParamHandler(property.PropertyType, ParamIndex, false, false);
                sb.Append("        " + paramHandler.getter + "\n");

                if (bIndexer)
                {
                    if (isStatic)
                        sb.AppendFormat("{0} = {1};\n", sbCall, paramHandler.argName);
                    else
                    {
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("        {0} argThis = ({0})vc.csObj;\n", JSDataExchangeMgr.GetTypeFullName(type));
                            sb.AppendFormat("argThis{0} = {1};", sbActualParam, paramHandler.argName);
                            sb.Append("        JSMgr.changeJSObj(vc.jsObj, argThis);\n");
                        }
                        else
                        {
                            sb.AppendFormat("        {0} = {1};\n", sbCall, paramHandler.argName);
                        }
                    }
                }
                else
                {
                    if (isStatic)
                        sb.AppendFormat("{0}.{1} = {2};\n", JSDataExchangeMgr.GetTypeFullName(type), property.Name, paramHandler.argName);
                    else
                    {
                        if (type.IsValueType)
                        {
                            sb.AppendFormat("        {0} argThis = ({0})vc.csObj;\n", JSDataExchangeMgr.GetTypeFullName(type));
                            sb.AppendFormat("        argThis.{0} = {1};\n", property.Name, paramHandler.argName);
                            sb.Append("        JSMgr.changeJSObj(vc.jsObj, argThis);\n");
                        }
                        else
                        {
                            sb.AppendFormat("        (({0})vc.csObj).{1} = {2};\n", JSDataExchangeMgr.GetTypeFullName(type), property.Name, paramHandler.argName);
                        }
                    }
                }
                sb.Append("    ]]\n");
            }

            sb.AppendFormat("]]\n");

            ccbn.properties.Add(functionName);
        }
        return sb;
    }
    

    public static StringBuilder BuildProperties(Type type, PropertyInfo[] properties, int[] propertiesIndex, ClassCallbackNames ccbn)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < properties.Length; i++)
        {
            var sbCall = new StringBuilder();

            PropertyInfo property = properties[i];
            MethodInfo[] accessors = property.GetAccessors();
            bool isStatic = accessors[0].IsStatic;

            bool bGenericT = type.IsGenericTypeDefinition;
            StringBuilder sbt = null;

            if (bGenericT)
            {
                sbt = new StringBuilder();

                sbt.AppendFormat("    PropertyInfo property = JSDataExchangeMgr.GetPropertyInfoOfGenericClass(vc.csObj.GetType(), {0}); \n",
                        propertiesIndex[i]);        // [0] methodArrIndex

                sbt.AppendFormat("    if (property == null)\n        return;\n");
                sbt.Append("\n");
            }

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
                //sbActualParam.Append("[");
                for (int j = 0; j < ps.Length; j++)
                {
                    paramHandlers[j] = JSDataExchangeMgr.Get_ParamHandler(ps[j].ParameterType, j, false, false);
                    sbActualParam.AppendFormat("{0}", paramHandlers[j].argName);
                    if (j != ps.Length - 1)
                    {
                        sbActualParam.Append(", ");
                    }
                }
                //sbActualParam.Append("]");
            }

            string functionName = type.Name + "_" + property.Name;
            if (bIndexer)
            {
                foreach (var p in ps)
                {
                    functionName += "_" + p.ParameterType.Name;
                }
            }
            functionName = JSDataExchangeMgr.HandleFunctionName(functionName);

            sb.AppendFormat("static void {0}(JSVCall vc)\n[[\n", functionName);

            if (bGenericT)
            {
                sb.Append(sbt);
            }


            bool bReadOnly = !property.CanWrite;
            if (bIndexer)
            {
                for (int j = 0; j < ps.Length; j++)
                {
                    sb.Append("        " + paramHandlers[j].getter + "\n");
                }
                if (bGenericT)
                {
                    if (isStatic)
                    {
                        sbCall.AppendFormat("property.GetValue(null, new object[][[{0}]])", sbActualParam);
                    }
                    else
                    {
                        sbCall.AppendFormat("property.GetValue(vc.csObj, new object[][[{0}]])", sbActualParam);
                    }
                }
                else
                {
                    if (isStatic)
                    {
                        sbCall.AppendFormat("{0}[{1}]", JSDataExchangeMgr.GetTypeFullName(type), sbActualParam);
                    }
                    else
                    {
                        sbCall.AppendFormat("(({0})vc.csObj)[{1}]", JSDataExchangeMgr.GetTypeFullName(type), sbActualParam);
                    }
                }
            }
            
            if (!bReadOnly)
            {
                sb.Append("    if (vc.bGet) [[ \n");
            }

            if (!bIndexer)
            {
                if (bGenericT)
                {
                    if (isStatic)
                    {
                        sbCall.AppendFormat("property.GetValue(null, new object[][[]])");
                    }
                    else
                    {
                        sbCall.AppendFormat("property.GetValue(vc.csObj, new object[][[]])");
                    }
                }
                else 
                {
                    // get
                    if (isStatic)
                        sbCall.AppendFormat("{0}.{1}", JSDataExchangeMgr.GetTypeFullName(type), property.Name);
                    else
                        sbCall.AppendFormat("(({0})vc.csObj).{1}", JSDataExchangeMgr.GetTypeFullName(type), property.Name);
                }
            }

            //if (type.IsValueType && !field.IsStatic)
            //    sb.AppendFormat("{0} argThis = ({0})vc.csObj;", type.Name);
                        
            sb.AppendFormat("        {0}", JSDataExchangeMgr.Get_Return(property.PropertyType, sbCall.ToString()));
            if (!bReadOnly)
            {
                sb.Append("\n    ]]\n");
            }

            // set
            if (!bReadOnly)
            {
                sb.Append("    else [[\n");

                int ParamIndex = ps.Length;

                var paramHandler = JSDataExchangeMgr.Get_ParamHandler(property.PropertyType, ParamIndex, false, false);
                sb.Append("        " + paramHandler.getter + "\n");

                if (bIndexer)
                {
                    if (bGenericT)
                    {
                        if (isStatic)
                            sb.AppendFormat("property.SetValue(null, {0}, new object[][[{1}]]);\n", paramHandler.argName, sbActualParam);
                        else
                        {
                            if (type.IsValueType || !type.IsValueType)
                            {   // struct 和 class 是一样的
                                sb.AppendFormat("property.SetValue(vc.csObj, {0}, new object[][[{1}]]);\n", paramHandler.argName, sbActualParam);
                            }
                        }
                    }
                    else
                    {
                        if (isStatic)
                            sb.AppendFormat("{0} = {1};\n", sbCall, paramHandler.argName);
                        else
                        {
                            if (type.IsValueType)
                            {
                                sb.AppendFormat("        {0} argThis = ({0})vc.csObj;\n", JSDataExchangeMgr.GetTypeFullName(type));
                                sb.AppendFormat("argThis[{0}] = {1};", sbActualParam, paramHandler.argName);
                                sb.Append("        JSMgr.changeJSObj(vc.jsObj, argThis);\n");
                            }
                            else
                            {
                                sb.AppendFormat("        {0} = {1};\n", sbCall, paramHandler.argName);
                            }
                        }
                    }
                }
                else
                {
                    if (bGenericT)
                    {
                        if (isStatic)
                            sb.AppendFormat("property.SetValue(null, {0}, new object[][[]]);\n", paramHandler.argName);
                        else
                        {
                            if (type.IsValueType || !type.IsValueType)
                            {   // struct 和 class 是一样的
                                sb.AppendFormat("property.SetValue(vc.csObj, {0}, new object[][[]]);\n", paramHandler.argName);
                            }
                        }
                    }
                    else
                    {
                        if (isStatic)
                            sb.AppendFormat("{0}.{1} = {2};\n", JSDataExchangeMgr.GetTypeFullName(type), property.Name, paramHandler.argName);
                        else
                        {
                            if (type.IsValueType)
                            {
                                sb.AppendFormat("        {0} argThis = ({0})vc.csObj;\n", JSDataExchangeMgr.GetTypeFullName(type));
                                sb.AppendFormat("        argThis.{0} = {1};\n", property.Name, paramHandler.argName);
                                sb.Append("        JSMgr.changeJSObj(vc.jsObj, argThis);\n");
                            }
                            else
                            {
                                sb.AppendFormat("        (({0})vc.csObj).{1} = {2};\n", JSDataExchangeMgr.GetTypeFullName(type), property.Name, paramHandler.argName);
                            }
                        }
                    }
                }
                sb.Append("    ]]\n");
            }

            sb.AppendFormat("]]\n");

            ccbn.properties.Add(functionName);
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
            sb.AppendFormat(fmt, t.IsByRef ? "true" : "false", p.IsOptional ? "true" : "false", t.IsArray ? "true" : "false", "typeof(" + JSDataExchangeMgr.GetTypeFullName(t) + ")", t.IsByRef ? ".MakeByRefType()" : "", "null");
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
        else if (methodName == "op_Implicit")
            strCall = "(" + JSDataExchangeMgr.GetTypeFullName(returnType) + ")" + paramHandlers[0].argName;
        else
            Debug.LogError("Unknown special name: " + methodName);

        string ret = JSDataExchangeMgr.Get_Return(returnType, strCall);
        sb.Append("    " + ret);
        return sb;
    }
    public static StringBuilder BuildNormalFunctionCall(
        int methodIndex, 
        ParameterInfo[] ps, 
        string className, 
        string methodName, 
        bool bStatic, 
        bool returnVoid, 
        Type returnType, 
        bool bConstructor,
        int TCount = 0, int methodArrIndex = -1)
    {
        StringBuilder sb = new StringBuilder();

        if (bConstructor)
        {
            if (type.IsGenericTypeDefinition)
            {
                // 不是 T 函数，但是类带T
                StringBuilder sbt = new StringBuilder();

                sbt.AppendFormat("    ConstructorInfo constructor = JSDataExchangeMgr.GetConstructorOfGenericClass(typeof({0}), vc, {1}); \n",
                        JSDataExchangeMgr.GetTypeFullName(type), methodArrIndex);        // [0] methodArrIndex

                sbt.AppendFormat("    if (constructor == null)\n        return true;\n");
                sbt.Append("\n");

                sb.Append(sbt);
            }
        }
        
        else if (TCount > 0)
        {
            StringBuilder sbt = new StringBuilder();
            sbt.Append("    // Get generic method by name and param count.\n");

            if (!bStatic) // instance method
            {
                sbt.AppendFormat("    MethodInfo method = JSDataExchangeMgr.MakeGenericFunction(vc.csObj.GetType(), \"{0}\", {1} /* method index */, {2} /* T Count */, vc); \n",
                    methodName,            // [0] 函数名
                    methodArrIndex,        // [1] methodArrIndex
                    TCount);               // [2] TCount
            }
            else // static method
            {
                sbt.AppendFormat("    MethodInfo method = JSDataExchangeMgr.MakeGenericFunction(typeof({0}), \"{1}\", {2} /* method index */, {3} /* T Count */, vc); \n",
                    JSDataExchangeMgr.GetTypeFullName(type), // [0] type
                    methodName,            // [1] 函数名
                    methodArrIndex,        // [2] methodArrIndex
                    TCount);               // [3] TCount
            }
            sbt.AppendFormat("    if (method == null)\n        return true;\n");
            sbt.Append("\n");

            sb.Append(sbt);
        }
        else if (type.IsGenericTypeDefinition)
        {
            // 不是 T 函数，但是类带T
            StringBuilder sbt = new StringBuilder();
            sbt.Append("    // Get generic method by name and param count.\n");

            if (!bStatic) // instance method
            {
                sbt.AppendFormat("    MethodInfo method = JSDataExchangeMgr.GetMethodOfGenericClass(vc.csObj.GetType(), \"{0}\", {1}); \n",
                    methodName,            // [0] 函数名
                    methodArrIndex);        // [1] methodArrIndex
            }
            else // static method
            {
                Debug.LogError("=================================ERROR");
            }
            sbt.AppendFormat("    if (method == null)\n        return true;\n");
            sbt.Append("\n");

            sb.Append(sbt);
        }
        else if (type.IsGenericType)
        {
            /////////////////////
            /// ERROR ///////////
            /////////////////////
        }

        var paramHandlers = new JSDataExchangeMgr.ParamHandler[ps.Length];        
        for (int i = 0; i < ps.Length; i++)
        {
            if (true /* !ps[i].ParameterType.IsGenericParameter */ )
            {
                // use original method's parameterinfo
                paramHandlers[i] = JSDataExchangeMgr.Get_ParamHandler(ps[i], i);
                if (ps[i].ParameterType.IsGenericParameter)
                {
                    paramHandlers[i].getter = "    vc.datax.setTemp(method.GetParameters()[" + i.ToString() + "].ParameterType);\n" + paramHandlers[i].getter;
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

        
        if (bConstructor && type.IsGenericTypeDefinition)
            sb.AppendFormat("    int len = count - {0};\n", type.GetGenericArguments().Length);
        else if (TCount == 0)
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
                {
                    string delegateGetName = GetFunctionArg_DelegateFuncionName(className, methodName, methodIndex, i);

                    if (p.ParameterType.IsGenericType)
                    {
                        // cg.args ta = new cg.args();
                        // sbGetParam.AppendFormat("foreach (var a in method.GetParameters()[{0}].ParameterType.GetGenericArguments()) ta.Add();");
                        sbGetParam.AppendFormat("        var getDelegateFun{0} = typeof({1}).GetMethod(\"{2}\").MakeGenericMethod\n", i, thisClassName, delegateGetName);
                        sbGetParam.AppendFormat("            (method.GetParameters()[{0}].ParameterType.GetGenericArguments());\n", i);
                        sbGetParam.AppendFormat("        object arg{0} = getDelegateFun{0}.Invoke(null, new object[][[{1}]]);\n", i, "vc.getJSFunctionValue()");
                    }
                    else
                    {
                        sbGetParam.AppendFormat("        {0} arg{1} = {2}(vc.getJSFunctionValue());\n",
                                                JSDataExchangeMgr.GetTypeFullName(p.ParameterType), // [0]
                                                i, // [1]
                                                delegateGetName // [2]
                                                );
                    }
                }
                else
                {
                    sbGetParam.Append("        " + paramHandlers[i].getter + "\n");
                }

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

                if (!type.IsGenericTypeDefinition)
                    sbCall.AppendFormat("new {0}({1})", JSDataExchangeMgr.GetTypeFullName(type), sbActualParam.ToString());
                else
                {
                    sbCall.AppendFormat("constructor.Invoke(null, new object[][[{0}]])", sbActualParam);
                }

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

                if (TCount == 0 && !type.IsGenericTypeDefinition)
                {
                    if (bStatic)
                        sbCall.AppendFormat("{0}.{1}({2})", JSDataExchangeMgr.GetTypeFullName(type), methodName, sbActualParam.ToString());
                    else if (!type.IsValueType)
                        sbCall.AppendFormat("(({0})vc.csObj).{1}({2})", JSDataExchangeMgr.GetTypeFullName(type), methodName, sbActualParam.ToString());
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
                        sbCall.AppendFormat("method.Invoke(vc.csObj, {0})", sbActualParamT);
                    }
                    else
                    {
                        sbCall.AppendFormat("method.Invoke(vc.csObj, {0})", sbActualParamT);
                    }
                }

                string callAndReturn = JSDataExchangeMgr.Get_Return(returnType, sbCall.ToString());

                StringBuilder sbStruct = null;
                if (type.IsValueType && !bStatic && TCount == 0 && !type.IsGenericTypeDefinition)
                {
                    sbStruct = new StringBuilder();
                    sbStruct.AppendFormat("{0} argThis = ({0})vc.csObj;", JSDataExchangeMgr.GetTypeFullName(type));
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
                 (type.IsValueType && !bStatic && TCount == 0 && !type.IsGenericTypeDefinition) ? sbStruct.ToString() : "",  // [2] if Struct, get argThis first
                 callAndReturn,  // [3] function call and return to js
                 (type.IsValueType && !bStatic && TCount == 0 && !type.IsGenericTypeDefinition) ? "JSMgr.changeJSObj(vc.jsObj, argThis);" : "",  // [4] if Struct, update 'this' object
                 sbGetParam,        // [5] get param
                 sbUpdateRefParam); // [6] update ref/out param

            }
        }

        return sb;
    }
    public static StringBuilder BuildConstructors(Type type, ConstructorInfo[] constructors, int[] constructorsIndex, ClassCallbackNames ccbn)
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

            string functionName = JSDataExchangeMgr.HandleFunctionName(type.Name + "_" + type.Name + (olIndex > 0 ? olIndex.ToString() : "") + (cons.IsStatic ? "_S" : ""));

            sb.AppendFormat(fmt, functionName,
                BuildNormalFunctionCall(i, paramS, type.Name, cons.Name, cons.IsStatic, returnVoid, null, true, 0, constructorsIndex[i]));

            ccbn.constructors.Add(functionName);
            ccbn.constructorsCSParam.Add(GenListCSParam2(paramS).ToString());
        }
        return sb;
    }
    public static StringBuilder BuildMethods(Type type, MethodInfo[] methods, int[] methodsIndex, int[] olInfo, ClassCallbackNames ccbn)
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

            functionName = JSDataExchangeMgr.HandleFunctionName(type.Name + "_" + SharpKitMethodName(method.Name, paramS, true, TCount));
            if (method.IsSpecialName && method.Name == "op_Implicit" && paramS.Length > 0)
            {
                functionName += "_to_" + method.ReturnType.Name;
            }
            if (UnityEngineManual.isManual(functionName))
            {
                sb.AppendFormat(fmt, functionName, "    UnityEngineManual." + functionName + "(vc, start, count);");
            }
            else
            {
                sb.AppendFormat(fmt, functionName,

                    method.IsSpecialName ? BuildSpecialFunctionCall(paramS, type.Name, method.Name, method.IsStatic, returnVoid, method.ReturnType)
                    : BuildNormalFunctionCall(i, paramS, type.Name, method.Name, method.IsStatic, returnVoid, method.ReturnType, 
                    false/* is constructor */, 
                    TCount, 
                    methodsIndex[i]));
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
                sbCons.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\n", ccbn.constructors[i],
                    //ccbn.constructorsCSParam[i], 
                    "null", 
                    type.Name);
            else
                sbCons.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\n", ccbn.constructors[i],
                    //ccbn.constructorsCSParam[i], 
                    "null", 
                    ti.constructors[i].Name);
        }
        for (int i = 0; i < ccbn.methods.Count; i++)
        {
            // if method is not overloaded
            // don's save the cs param array
            sbMethod.AppendFormat("        new JSMgr.MethodCallBackInfo({0}, '{2}', {1}),\n", ccbn.methods[i], 
                (ti.methodsOLInfo[i] > 0 ? 
                
                // ccbn.methodsCSParam[i] : 
                "null" : // !!!!!!!!!!!!!!!!!!!!!!!!!!!
                
                "null"), 
                ti.methods[i].Name);
        }

        sb.AppendFormat(fmt, JSDataExchangeMgr.GetTypeFullName(ccbn.type), sbField, sbProperty, sbCons, sbMethod);
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
            sbA.AppendFormat("        {0}Generated.__Register();\n", JSDataExchangeMgr.GetTypeFileName(JSBindingSettings.classes[i]));
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
            string name = JSDataExchangeMgr.GetTypeFileName(JSBindingSettings.classes[i]).Replace('.', '_');
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

        thisClassName = JSDataExchangeMgr.GetTypeFileName(type) + "Generated";

        var sbFields = BuildFields(type, ti.fields, ccbn);
        var sbProperties = BuildProperties(type, ti.properties, ti.propertiesIndex, ccbn);
        var sbMethods = BuildMethods(type, ti.methods, ti.methodsIndex, ti.methodsOLInfo, ccbn);
        var sbCons = BuildConstructors(type, ti.constructors, ti.constructorsIndex, ccbn);
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

public class {0}
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
        sbFile.AppendFormat(fmtFile, thisClassName, sbClass, nameSpaceString.Length > 0 ? "using " + nameSpaceString + ";" : "");
        HandleStringFormat(sbFile);

        sbFile.Replace("\r\n", "\n");

        string fileName = JSBindingSettings.csGeneratedDir + "/" +
            JSDataExchangeMgr.GetTypeFileName(type) + 
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
    
    public static bool CheckClassBindings()
    {
        Dictionary<Type, bool> clrLibrary = new Dictionary<Type, bool>();
        {
            //
            // these types are defined in clrlibrary.javascript
            //
            clrLibrary.Add(typeof(System.Object), true);
            clrLibrary.Add(typeof(System.Exception), true);
            clrLibrary.Add(typeof(System.SystemException), true);
            clrLibrary.Add(typeof(System.ValueType), true);
        }

        Dictionary<Type, bool> dict = new Dictionary<Type, bool>();
        var sb = new StringBuilder();
        bool ret = true;

        // 检查类型有没有重复
        foreach (var type in JSBindingSettings.classes)
        {
            if (dict.ContainsKey(type))
            {
                sb.AppendFormat(
                    "Operation fail. There are more than 1 \"{0}\" in JSBindingSettings.classes, please check.\n",
                    JSDataExchangeMgr.GetTypeFullName(type));
                ret = false;
            }
            else
            {
                dict.Add(type, true);
            }
        }

        // 检查有没有基类没导出
        foreach (var typeb in dict)
        {
            Type type = typeb.Key;
            if (type.BaseType == null) { continue;  }
            // System.Object is already defined in SharpKit clrlibrary
            if (!clrLibrary.ContainsKey(type.BaseType) && !dict.ContainsKey(type.BaseType))
            {
                sb.AppendFormat("\"{0}\"\'s base type \"{1}\" must also be in JSBindingSettings.classes.\n",
                    JSDataExchangeMgr.GetTypeFullName(type),
                    JSDataExchangeMgr.GetTypeFullName(type.BaseType));
                ret = false;
            }
//             foreach (var Interface in type.GetInterfaces())
//             {
//                 if (!dict.ContainsKey(Interface))
//                 {
//                     sb.AppendFormat("Interface \"{0}\" of \"{1}\" must also be in JSBindingSettings.classes.",
//                         JSDataExchangeMgr.GetTypeFullName(Interface),
//                         JSDataExchangeMgr.GetTypeFullName(type));
//                     Debug.LogError(sb);
//                     return false;
//                 }
            //             }
        }
        if (!ret)
        {
            Debug.LogError(sb);
        }
        return ret;
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

    public static void OuputGameObjectHierachy(StringBuilder sb, GameObject go, int tab)
    {
        for (var t = 0; t < tab; t++)
            sb.Append("    ");
        sb.Append(go.name + "  (");

        var coms = go.GetComponents(typeof(Component));
        for (var c = 0; c < coms.Length; c++)
        {
            sb.Append(coms[c].GetType().Name);
            if (c != coms.Length - 1)
            {
                sb.Append(" | ");
            }
        }
        sb.Append(")\n");

        var childCount = go.transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            OuputGameObjectHierachy(sb, child.gameObject, tab + 1);
        }
    }

//     [MenuItem("JSB/IterateAllPrefabs")]
//     public static void IterateAllPrefabs()
//     {
//         StringBuilder sb = new StringBuilder();
//         string[] GUIDs = AssetDatabase.FindAssets("t:prefab");
//         foreach (var guid in GUIDs)
//         {
//             //UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(path);
//             //sb.Append(obj.name + "\n");
// 
//             string path = AssetDatabase.GUIDToAssetPath(guid);
// 
//             UnityEngine.Object mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
//             if (mainAsset is GameObject)
//             {
//                 // sb.Append(mainAsset.GetType().Name + " / " + mainAsset.name + "\n");
//                 OuputGameObjectHierachy(sb, (GameObject)mainAsset, 1);
//             }
// 
//             sb.Append("\n");
//         }
//         Debug.Log(sb);
//        
//    }
    //[MenuItem("Assets/JSBinding/Read serialized data test")]
    public static void ReadSerializedDataTest()
    {
        AssetDatabase.FindAssets("backgrounds", new string[]{"Assets/Prefabs/Environment"});
    }
    [MenuItem("JSB/Generate JS and CS Bindings")]
    public static void GenerateJSCSBindings()
	{
		if (!CheckClassBindings())
			return;
        JSDataExchangeMgr.reset();
        UnityEngineManual.initManual();
        CSGenerator2.GenerateClassBindings();
        JSGenerator2.GenerateClassBindings();
        UnityEngineManual.afterUse();
        AssetDatabase.Refresh();
    }

    
    //[MenuItem("Assets/JSBinding/1Output All T Functions")]
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

    [MenuItem("JSB/Output All Types in UnityEngine.dll")]
    public static void OutputAllTypesInUnityEngine()
    {
        var asm = typeof(GameObject).Assembly;
        OutputAllTypesInAssembly(asm);
    }


    [MenuItem("JSB/Output All Types in UnityEngine.UI.dll")]
    public static void OutputAllTypesInUnityEngineUI()
    {
        //var asm = typeof(UnityEngine.EventSystems.UIBehaviour).Assembly;
        //OutputAllTypesInAssembly(asm);
    }

    public static void OutputAllTypesInAssembly(Assembly asm)
    {
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
            {
                string s = alltypes[i].GetConstructors().Length.ToString() 
                    + "/" +
                    alltypes[i].GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Length.ToString();

                writer.WriteLine(alltypes[i].ToString() + " " + s);
            }
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
