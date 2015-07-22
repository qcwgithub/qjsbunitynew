using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
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
            jsimp.Reflection.SetFieldValue(obj, fieldName, fieldValue);
        }
        return obj;
    }
}
