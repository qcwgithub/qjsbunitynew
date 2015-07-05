using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// XmlParser
/// 这是一个示例，示例如何使用 jsimp 里的函数。这个类将要被编译成JavaScript。
/// 本来这个类里面是使用 Activator.CreateInstance<T> 创建对象的，但是这样编译成JS后不能正常执行
/// 所以换成 jsimp.Reflection.CreateInstance ，就可以在JS中执行了
/// </summary>
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/JSImpTest/XmlParser.javascript")]
public class XmlParser
{
    public static T ComvertType<T>(Dictionary<string, string> dict)
    {
        T obj = jsimp.Reflection.CreateInstance<T>();
        foreach (var ele in dict)
        {
            var fieldName = ele.Key;
            var fieldValue = ele.Value;
            jsimp.Reflection.SetField(obj, fieldName, fieldValue);
        }
        return obj;
    }
}
