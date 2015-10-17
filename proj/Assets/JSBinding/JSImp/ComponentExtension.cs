using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/ComponentExtension.javascript")]
public static class ComponentExtension
{
    [JsMethod(Code = @"return com.GetComponent$1(T);")]
    public static T GetComponentI<T>(this Component com) where T : class
    {
        return com.GetComponent(typeof(T)) as T;
    }
    [JsMethod(Code = @"return go.GetComponent$1(T);")]
    public static T GetComponentI<T>(this GameObject go) where T : class
    {
        return go.GetComponent(typeof(T)) as T;
    }
}