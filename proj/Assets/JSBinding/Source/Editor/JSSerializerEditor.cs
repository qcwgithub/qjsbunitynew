using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;

public static class JSSerializerEditor
{
    public enum SerializeType
    {
        String,
        Object,
    }
    public struct AnalyzeStructInfo
    {
        public JSSerializer.AnalyzeType analyzeType;
        public string Name; // 字段名字。当 analyzeType == xxObj xxEnd 时 或者数组元素时这个没鸟用
        public Type type; // 类型 有时候 value == null
        public object value;
        public JSSerializer.UnitType unitType;// 单元类型。当 analyzeType == xxBegin xxObj xxEnd 时 这个没鸟用

        public AnalyzeStructInfo(JSSerializer.AnalyzeType at,
            string name, Type type, object v = null, 
            JSSerializer.UnitType ut = JSSerializer.UnitType.ST_Unknown)
        {
            analyzeType = at;
            Name = name;
            this.type = type;
            value = v;
            unitType = ut;

            eSerialize = SerializeType.String;
        }
        // string or object?
        public SerializeType eSerialize;
        public void Alloc(JSSerializer serializer)
        {
            eSerialize = SerializeType.String;
            switch (analyzeType)
            {
                case JSSerializer.AnalyzeType.ArrayBegin:
                    AllocString("ArrayBegin/" + this.Name + "/");
                    break;
                case JSSerializer.AnalyzeType.ArrayEnd:
                    AllocString("ArrayEnd/" + this.Name + "/");
                    break;
                case JSSerializer.AnalyzeType.StructBegin:
                    AllocString("StructBegin/" + this.Name + "/" + JSNameMgr.GetTypeFullName(this.type));
                    break;
                case JSSerializer.AnalyzeType.StructEnd:
                    AllocString("StructEnd/" + this.Name + "/" + JSNameMgr.GetTypeFullName(this.type));
                    break;
                case JSSerializer.AnalyzeType.ListBegin:
                    AllocString("ListBegin/" + this.Name + "/" + JSNameMgr.GetTypeFullName(typeof(List<>)));
                    break;
                case JSSerializer.AnalyzeType.ListEnd:
                    AllocString("ListEnd/" + this.Name + "/" + JSNameMgr.GetTypeFullName(typeof(List<>)));
                    break;
                case JSSerializer.AnalyzeType.Unit:
                    {
                        var sb = new StringBuilder();

                        // this.value could be null
                        Type objectType = this.type; // this.value.GetType();
                        if (typeof(UnityEngine.Object).IsAssignableFrom(objectType))
                        {
                            eSerialize = SerializeType.Object;

                            if (typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(objectType))
                            {
                                // if a monobehaviour is refer
                                // and this monobehaviour will be translated to js later
                                //  ST_MonoBehaviour
                                if (WillTypeBeTranslatedToJavaScript(objectType))
                                {
                                    // add game object
                                    var index = AllocObject(((MonoBehaviour)this.value).gameObject);

                                    // UnitType / Name / object Index / MonoBehaviour Name
                                    sb.AppendFormat("{0}/{1}/{2}/{3}", (int)this.unitType, this.Name, index, JSNameMgr.GetTypeFullName(objectType));
                                    AllocString(sb.ToString());
                                }
                                else
                                {
                                    // not supported
                                }
                            }
                            else
                            {
                                // UnitType / Name / object Index
                                sb.AppendFormat("{0}/{1}/{2}", (int)this.unitType, this.Name, AllocObject((UnityEngine.Object)this.value));
                                AllocString(sb.ToString());
                            }
                        }
                        else
                        {
                            sb.AppendFormat("{0}/{1}/{2}", (int)this.unitType, this.Name, 
                                ValueToString(this.value, this.type));
                            AllocString(sb.ToString());
                        }
                    }
                    break;
            }
        }
    }

    static void AllocString(string str) { 
		lstString.Add(str); 
	}
    static int AllocObject(UnityEngine.Object obj) { 
		lstObjs.Add(obj);
        return lstObjs.Count - 1; 
	}
    /// <summary>
    /// lstString lstObjs 存储序列化要用的字符串和对象列表。
    /// </summary>
    static List<string> lstString = new List<string>();
    static List<UnityEngine.Object> lstObjs = new List<UnityEngine.Object>();

    static List<AnalyzeStructInfo> lstAnalyze = new List<AnalyzeStructInfo>();
    public static int AddAnalyze(Type type, string name, object value, int index = -1)
    {
        if (index == -1) index = lstAnalyze.Count;
        JSSerializer.UnitType unitType = GetUnitType(type);
        if (unitType != JSSerializer.UnitType.ST_Unknown)
        {
            lstAnalyze.Insert(index, new AnalyzeStructInfo(JSSerializer.AnalyzeType.Unit, name, type, value, unitType));
            return 1;
        }
        else
        {
            if (type.IsArray)
            {
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ArrayBegin, name, type));
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ArrayObj, name, type, value));
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ArrayEnd, name, type));
                return 3;
            }
            else if (type.IsClass || type.IsValueType)
            {
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.StructBegin, name, type));
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.StructObj, name, type, value));
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.StructEnd, name, type));
                return 3;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ListBegin, name, type));
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ListObj, name, type, value));
                lstAnalyze.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ListEnd, name, type));
                return 3;
            }
        }
        return 0;
    }


    /// <summary>
    /// sDict 存储类型和枚举JSSerializer的对应关系
    /// </summary>
    static Dictionary<Type, JSSerializer.UnitType> sDict;
    static JSSerializer.UnitType GetUnitType(Type type)
    {
        if (sDict == null)
        {
            sDict = new Dictionary<Type, JSSerializer.UnitType>();

            sDict.Add(typeof(Boolean), JSSerializer.UnitType.ST_Boolean);

            sDict.Add(typeof(Byte), JSSerializer.UnitType.ST_Byte);
            sDict.Add(typeof(SByte), JSSerializer.UnitType.ST_SByte);
            sDict.Add(typeof(Char), JSSerializer.UnitType.ST_Char);
            sDict.Add(typeof(Int16), JSSerializer.UnitType.ST_Int16);
            sDict.Add(typeof(UInt16), JSSerializer.UnitType.ST_UInt16);
            sDict.Add(typeof(Int32), JSSerializer.UnitType.ST_Int32);
            sDict.Add(typeof(UInt32), JSSerializer.UnitType.ST_UInt32);
            sDict.Add(typeof(Int64), JSSerializer.UnitType.ST_Int64);
            sDict.Add(typeof(UInt64), JSSerializer.UnitType.ST_UInt64);

            sDict.Add(typeof(Single), JSSerializer.UnitType.ST_Single);
            sDict.Add(typeof(Double), JSSerializer.UnitType.ST_Double);


            sDict.Add(typeof(String), JSSerializer.UnitType.ST_String);
        }

        if (type.IsEnum)
        {
            return JSSerializer.UnitType.ST_Enum;
        }

        if ((typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(type)))
        {
            return JSSerializer.UnitType.ST_MonoBehaviour;
        }
        if ((typeof(UnityEngine.Object).IsAssignableFrom(type)))
        {
            return JSSerializer.UnitType.ST_UnityEngineObject;
        }

        JSSerializer.UnitType ret = JSSerializer.UnitType.ST_Unknown;
        if (!sDict.TryGetValue(type, out ret))
        {
            // Debug.LogError("GetIndex: Unknown type: " + type.Name);
            return JSSerializer.UnitType.ST_Unknown;
        }
        return ret;
    }
    /// <summary>
    /// 将值转换为字符串表示
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    static string ValueToString(object value, Type type)
    {
        //
        // value
        //
        StringBuilder sb = new StringBuilder();
        if (type.IsPrimitive)
        {
            sb.AppendFormat("{0}", value.ToString());
        }
        else if (type.IsEnum)
        {
            sb.AppendFormat("{0}", (int)Enum.Parse(type, value.ToString()));
        }
        else if (type == typeof(string))
        {
            sb.AppendFormat("{0}", value == null ? "" : value.ToString());
        }
        else if (type == typeof(Vector2))
        {
            Vector2 v2 = (Vector2)value;
            sb.AppendFormat("{0}/{1}", v2.x, v2.y);
        }
        else if (type == typeof(Vector3))
        {
            Vector3 v3 = (Vector3)value;
            sb.AppendFormat("{0}/{1}/{2}", v3.x, v3.y, v3.z);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 取出一个脚本中需要序列化的字段。目前是取出所有 public 变量。可能有误
    /// </summary>
    /// <param name="behaviour"></param>
    /// <returns></returns>
    public static FieldInfo[] GetMonoBehaviourSerializedFields(MonoBehaviour behaviour)
    {
        Type type = behaviour.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance /* | BindingFlags.Static */ );
        return fields;
    }
    public static FieldInfo[] GetTypeSerializedFields(Type type)
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance /* | BindingFlags.Static */ );
        return fields;
    }
    static void TraverseAnalyze()
    {
        bool bContinueTraverse = false;
        for (var i = 0; i < lstAnalyze.Count; i++)
        {
            AnalyzeStructInfo info = lstAnalyze[i];
            bool bBreakFor = true;
            int Pos = i + 1;
            switch (info.analyzeType)
            {
                case JSSerializer.AnalyzeType.ArrayObj:
                    {
                        bContinueTraverse = true;
                        if (info.value != null)
                        {
                            Array arr = (Array)info.value;
                            Type arrayElementType = info.value.GetType().GetElementType();
                            for (var j = 0; j < arr.Length; j++)
                            {
                                object value = arr.GetValue(j);
                                Pos += AddAnalyze(arrayElementType, "["+j.ToString()+"]", value, Pos);
                            }
                        }
                        lstAnalyze.RemoveAt(i);
                    }
                    break;
                case JSSerializer.AnalyzeType.StructObj:
                    {
                        bContinueTraverse = true;
                        var structure = info.value;
                        FieldInfo[] fields = GetTypeSerializedFields(structure.GetType());
                        foreach (FieldInfo field in fields)
                        {
                            Pos += AddAnalyze(field.FieldType, field.Name, field.GetValue(structure), Pos);
                        }
                        lstAnalyze.RemoveAt(i);
                    }
                    break;
                case JSSerializer.AnalyzeType.ListObj:
                    {
                        bContinueTraverse = true;
                        if (info.value != null)
                        {
                            var list = info.value;
                            var listType = list.GetType();
                            var listElementType = listType.GetGenericArguments()[0];

                            int Count = (int)listType.GetProperty("Count").GetValue(list, new object[] { });
                            PropertyInfo pro = listType.GetProperty("Item");

                            for (var j = 0; j < Count; j++)
                            {
                                var value = pro.GetValue(list, new object[] { j });
                                Pos += AddAnalyze(listElementType, j.ToString(), value, Pos);
                            }
                        }
                        lstAnalyze.RemoveAt(i);
                    }
                    break;
                default:
                    {
                        bBreakFor = false;
                    }
                    break;
            }
            if (bBreakFor)
                break;
        }
        if (bContinueTraverse)
            TraverseAnalyze();
    }
    static void CopyBehaviour(MonoBehaviour behaviour, JSSerializer serizlizer)
    {
        lstAnalyze.Clear();
        lstString.Clear();
        lstObjs.Clear();

        GameObject go = behaviour.gameObject;
        Type type = behaviour.GetType();

        FieldInfo[] fields = GetMonoBehaviourSerializedFields(behaviour);
        foreach (FieldInfo field in fields)
        {
            AddAnalyze(field.FieldType, field.Name, field.GetValue(behaviour));
        }

        TraverseAnalyze();

        for (var i = 0; i < lstAnalyze.Count; i++)
        {
            lstAnalyze[i].Alloc(serizlizer);
        }
        serizlizer.jsScriptName = JSNameMgr.GetTypeFullName(behaviour.GetType());
        serizlizer.arrString = lstString.ToArray();
        serizlizer.arrObject = lstObjs.ToArray();
    }
    public static bool WillTypeBeAvailableInJavaScript(Type type)
    {
        return WillTypeBeTranslatedToJavaScript(type) || WillTypeBeExportedToJavaScript(type);
    }
    public static bool WillTypeBeTranslatedToJavaScript(Type type)
    {
        System.Object[] attrs = type.GetCustomAttributes(typeof(JsTypeAttribute), false);
        bool bToJS = attrs.Length > 0;
        return bToJS;
    }
    public static bool WillTypeBeExportedToJavaScript(Type type)
    {
        foreach (var t in JSBindingSettings.classes)
        {
            if (t == type)
                return true;
        }
        return false;
    }
    public static void CopyGameObject<T>(GameObject go) where T : JSSerializer
    {
        // delete original JSSerializer(s)
        //         foreach (var eh in go.GetComponents<JSSerializer>()) 
        //         {
        //             if (eh.AutoDelete)
        //             {
        //                 // only delete when Auto is true
        //                 DestroyImmediate(eh, true);
        //             }
        //         }

        var behaviours = go.GetComponents<MonoBehaviour>();
        for (var i = 0; i < behaviours.Length; i++)
        {
            var behav = behaviours[i];
            // ignore ExtraHandler here
            if (behav is JSSerializer)
            {
                continue;
            }

            if (WillTypeBeTranslatedToJavaScript(behav.GetType()))
            {   // if this MonoBehaviour is going to be translated to JavaScript
                // replace this behaviour with JSComponent
                // copy the serialized data if needed
                JSSerializer helper = (JSSerializer)go.AddComponent<T>();
                CopyBehaviour(behav, helper);
            }
        }
    }
    public static void RemoveOtherMonoBehaviours(GameObject go)
    {
        var coms = go.GetComponents<MonoBehaviour>();
        for (var i = 0; i < coms.Length; i++)
        {
            var com = coms[i];
            // must ignore ExtraHandler here
            if (com is JSSerializer)
                continue;

            UnityEngine.Object.DestroyImmediate(com, true);
        }
    }
}