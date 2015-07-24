using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public partial class UnityEngineManual
{
    static string typeString;
    static System.Type type;

    static Dictionary<string, bool> dict = new Dictionary<string, bool>();
    public static void initManual() 
    {
        dict.Clear();
    }
    public static bool isManual(string functionName)
    {
        bool ret = false;
        if (dict.TryGetValue(functionName, out ret))
            return ret;

        MethodInfo method = typeof(UnityEngineManual).GetMethod(functionName);
        ret = (method != null);
        dict[functionName] = ret;
        return ret;
    }

    public static void afterUse()
    {
        var sb = new StringBuilder();
        foreach (var v in dict)
        {
            if (v.Value)
                sb.AppendFormat("Manual: {0}\n", v.Key);
        }
        Debug.Log(sb);
    }

    static bool isCSMonoBehaviour(System.Type type)
    {
        if (type == null)
        {
            return false;
        }
        if (type.Namespace != null && type.Namespace.IndexOf("UnityEngine") >= 0)
        {
            return true;
        }
        // This is useful if source c# file still exists in project
        if (type.GetCustomAttributes(typeof(SharpKit.JavaScript.JsTypeAttribute), false).Length > 0)
        {
            return false;
        }
        if (!typeof(MonoBehaviour).IsAssignableFrom(type))
            return false;
		return true;
        //return !typeof(MonoBehaviour).IsAssignableFrom(type);
    }
}
