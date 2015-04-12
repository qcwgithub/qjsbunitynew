using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public static class JSNameMgr
{
    public static string GetTypeFileName(Type type)
    {
        string fullName = GetTypeFullName(type);
        return fullName.Replace('`', '_').Replace('.', '_').Replace('<', '7').Replace('>', '7').Replace(',', '_');
    }

    public static string HandleFunctionName(string functionName)
    {
        return functionName.Replace('<', '7').Replace('>', '7').Replace('`', 'A');
    }

    public static string[] GenTSuffix = new string[] { "`1", "`2", "`3", "`4", "`5" };
    public static string[] GenTSuffixReplaceCS = new string[] { "<>", "<,>", "<,,>", "<,,,>", "<,,,,>" };
    public static string[] GenTSuffixReplaceJS = new string[] { "$1", "$2", "$3", "$4", "$5" };

    public static string GetTypeFullName(Type type)
    {
        if (type == null) return "";

        if (type.IsByRef)
            type = type.GetElementType();

        if (type.IsGenericParameter)
        {  // T
            return "object";
        }
        else if (!type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            string rt = type.FullName;
            if (rt == null)
            {
                rt = ">>>>>>>>>>>?????????????????/";
            }
            rt = rt.Replace('+', '.');
            return rt;
        }
        else if (type.IsGenericTypeDefinition)
        {
            var t = type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition();
            string ret = t.FullName;
            for (var i = 0; i < GenTSuffix.Length; i++)
                ret = ret.Replace(GenTSuffix[i], GenTSuffixReplaceCS[i]);
            return ret.Replace('+', '.');

            // 后面是 `1 或 `2 之类的
            //            string rt = type.FullName;

            //            rt = rt.Substring(0, rt.Length - 2);
            //            rt += "<";
            //            int TCount = type.GetGenericArguments().Length;
            //            for (int i = 0; i < TCount - 1; i++)
            //            {
            //                //这里不要加空格了
            //                rt += ",";
            //            }
            //            rt += ">";
            //            rt = rt.Replace('+', '.');
            //            return rt;
        }
        else
        {
            // 通常不会进这里
            string fatherName = type.Name.Substring(0, type.Name.Length - 2);
            Type[] ts = type.GetGenericArguments();
            fatherName += "<";
            for (int i = 0; i < ts.Length; i++)
            {
                fatherName += ts[i].Name; // 这里是T
                if (i != ts.Length - 1)
                    fatherName += ", ";
            }
            fatherName += ">";
            fatherName.Replace('+', '.');
            return fatherName;
        }
    }
    public static string GetJSTypeFullName(Type type)
    {
        if (type == null) return "";

        if (type.IsByRef)
            type = type.GetElementType();

        if (type.IsGenericParameter)
        {
            return "object";
        }
        else if (!type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            string rt = type.FullName;
            if (rt == null)
            {
                rt = ">>>>>>>>>>>?????????????????/";
            }
            rt = rt.Replace('+', '.');
            return rt;
        }
        else if (type.IsGenericTypeDefinition || type.IsGenericType)
        {
            // 注意：
            // typeof(List<>).FullName    是 System.Collections.Generic.List`1
            // typeof(List<int>).FullName 是 Systcem.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]

            // 后面是 `1 或 `2 之类的
            //            string rt = string.Empty;
            //            if (type.IsGenericTypeDefinition)
            //                rt = type.FullName;
            //            else
            //                rt = type.GetGenericTypeDefinition().FullName;
            //            rt = rt.Substring(0, rt.Length - 2);
            //            int TCount = type.GetGenericArguments().Length;
            //            rt += "$" + TCount.ToString();
            //return rt;

            Type t = type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition();
            return t.FullName.Replace('`', '$').Replace('+', '.');
        }
        else
        {
            // 通常不会进这里
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
}