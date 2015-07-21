using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// 所有在 jsimp 命名空间里的类，都将在 CS 和 JS 分别实现
/// 当代码运行在 CS 时，就跑 CS 的类；当运行 JS 时，就跑 JS 的类。
/// 注意，Reflection类本来应该没有 JsType 标签，因为他是在JS手动实现的
/// 加了也没有关系， SharpKit 编译后会产生 StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/Reflection.javascript (A)
/// 手动实现的JS在 StreamingAssets/JavaScript/JSImp/Reflection.javascript (B)
/// includes.javascript 里将会同时 require A 和 B 两个文件。但是 B 排在 A 后面，并且在B里写了 jsb_ReplaceOrPushJsType 函数，所以 B 将会覆盖掉 A
/// 那么产生A有什么好处吗？有。因为手写JS必须要符合 SharpKit 规则的，所以，要写B，就先复制一份A，再把里面的函数实现给改了。
/// </summary>
namespace jsimp
{

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/Reflection.javascript")]
    public class Reflection
    {
        public static T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
        public static bool SetFieldValue(object obj, string fieldName, object value)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                FieldInfo field = type.GetField(fieldName);
                if (field != null)
                {
                    field.SetValue(obj, value);
                    return true;
                }
            }
            return false;
        }
        public static Type GetFieldType(Type type, string fieldName)
        {
            if (type != null)
            {
                FieldInfo field = type.GetField(fieldName);
                if (field != null)
                {
                    return field.FieldType;
                }
            }
            return null;
        }
        public static bool SetPropertyValue(object obj, string propertyName, object value)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                PropertyInfo property = type.GetProperty(propertyName);
                if (property != null)
                {
                    property.SetValue(obj, value, null);
                    return true;
                }
            }
            return false;
        }
        public static Type GetPropertyType(Type type, string propertyName)
        {
            if (type != null)
            {
                PropertyInfo property = type.GetProperty(propertyName);
                if (property != null)
                {
                    return property.PropertyType;
                }
            }
            return null;
        }
        public static bool PropertyTypeIsIntArray(Type type, string propertyName)
        {
            if (type != null)
            {
                PropertyInfo property = type.GetProperty(propertyName);
                if (property != null)
                {
                    return (property.PropertyType == typeof(int[]));
                }
            }
            return false;
        }
        // in JavaScript, it will be simply
        // return (a == b);
        // call this function only when it's OK for JavaScript to do (a == b)
        public static bool SimpleTEquals<T>(T a, T b)
        {
            return a.Equals(b);
        }
        public static bool TypeIsIntArray(Type type)
        {
            return type == typeof(int[]);
        }
    }
}

