using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

/// <summary>
/// 
/// 
/// All classes in namespace 'jsimp' have separate implement in C# and JavaScript
/// These classes also need 'JsType' attribute.
/// 
/// 
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

