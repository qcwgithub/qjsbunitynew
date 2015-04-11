using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;

public static class JSSerializerEditor
{
    private struct GameObject_JSComponentName
    {
        public string valName;
        public GameObject go;
        public string scriptName;
        public GameObject_JSComponentName(string _valName, GameObject _go, string _scriptName) { valName = _valName; go = _go; scriptName = _scriptName; }
    }
    private List<GameObject_JSComponentName> cachedRefJSComponent = new List<GameObject_JSComponentName>();
    enum SerializeType
    {
        String,
        Object,
    }
    public struct AnalyzeStructInfo
    {
        public JSSerializer.AnalyzeType analyzeType;
        public string Name;
        public object value;
        public JSSerializer.UnitType unitType;

        public AnalyzeStructInfo(JSSerializer.AnalyzeType at, 
            string name, object v = null, 
            JSSerializer.UnitType ut = JSSerializer.UnitType.ST_Unknown)
        {
            analyzeType = at;
            Name = name;
            value = v;
            unitType = ut;

            eSerialize = SerializeType.String;
            serizlizeIndex = 0;
        }
        // string or object?
        public SerializeType eSerialize;
        // if string  it's index of arrString
        // if object  it's index of arrObject
        public int serizlizeIndex;
        public void Alloc(JSSerializer serializer)
        {
            eSerialize = SerializeType.String;
            switch (analyzeType)
            {
                case JSSerializer.AnalyzeType.ArrayBegin:
                    serizlizeIndex = AllocString("ArrayBegin/" + this.Name);
                    break;
                case JSSerializer.AnalyzeType.ArrayEnd:
                    serizlizeIndex = AllocString("ArrayEnd/" + this.Name);
                    break;
                case JSSerializer.AnalyzeType.StructBegin:
                    serizlizeIndex = AllocString("StructBegin/" + this.Name);
                    break;
                case JSSerializer.AnalyzeType.StructEnd:
                    serizlizeIndex = AllocString("StructEnd/" + this.Name);
                    break;
                case JSSerializer.AnalyzeType.ListBegin:
                    serizlizeIndex = AllocString("ListBegin/" + this.Name);
                    break;
                case JSSerializer.AnalyzeType.ListEnd:
                    serizlizeIndex = AllocString("ListEnd/" + this.Name);
                    break;
                case JSSerializer.AnalyzeType.Unit:
                    {
                        var sb = new StringBuilder();

                        Type objectType = this.value.GetType();
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

                                    // eType / Name / object Index / MonoBehaviour name
                                    sb.AppendFormat("{0}/{1}/{2}/{3}", (int)this.unitType, this.Name, index, JSDataExchangeMgr.GetTypeFullName(objectType));
                                    AllocString(sb.ToString());
                                }
                                else
                                {
                                    // not supported
                                }
                            }
                            else
                            {
                                // eType / Name / object Index
                                sb.AppendFormat("{0}/{1}/{2}", (int)this.unitType, this.Name, JSDataExchangeMgr.GetTypeFullName(objectType));
                                AllocString(sb.ToString());
                            }
                        }
                        else
                        {
                            sb.AppendFormat("{0}/{1}/{2}", (int)this.unitType, this.Name, 
                                ValueToString(this.value, this.value.GetType()));
                            AllocString(sb.ToString());
                        }
                    }
                    break;
            }
        }
    }

    static int AllocString(string str) { lstString.Add(str); return lstString.Count - 1; }
    static int AllocObject(UnityEngine.Object obj) { lstObjs.Add(obj); return lstString.Count - 1; }
    static List<string> lstString = new List<string>();
    static List<UnityEngine.Object> lstObjs = new List<UnityEngine.Object>();

    static List<AnalyzeStructInfo> lst = new List<AnalyzeStructInfo>();
    public static int AddAnalyze(Type type, string name, object value, int index = -1)
    {
        if (index == -1) index = lst.Count;
        JSSerializer.UnitType unitType = GetUnitType(type);
        if (unitType != JSSerializer.UnitType.ST_Unknown)
        {
            lst.Insert(index, new AnalyzeStructInfo(JSSerializer.AnalyzeType.Unit, name, value, unitType));
            return 1;
        }
        else
        {
            if (type.IsArray)
            {
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ArrayBegin, name));
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ArrayObj, name, value));
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ArrayEnd, name));
                return 3;
            }
            else if (type.IsClass || type.IsValueType)
            {
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ListBegin, name));
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ListObj, name, value));
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.ListEnd, name));
                return 3;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.StructBegin, name));
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.StructObj, name, value));
                lst.Insert(index++, new AnalyzeStructInfo(JSSerializer.AnalyzeType.StructEnd, name));
                return 3;
            }
        }
        return 0;
    }


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
            return UnitType.ST_MonoBehaviour;
        }
        if ((typeof(UnityEngine.Object).IsAssignableFrom(type)))
        {
            return UnitType.ST_UnityEngineObject;
        }

        UnitType ret = UnitType.ST_Unknown;
        if (!sDict.TryGetValue(type, out ret))
        {
            // Debug.LogError("GetIndex: Unknown type: " + type.Name);
            return JSSerializer.UnitType.ST_Unknown;
        }
        return ret;
    }

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
            sb.AppendFormat("{0}", value.ToString());
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
        for (var i = 0; i < lst.Count; i++)
        {
            AnalyzeStructInfo info = lst[i];
            int Pos = i + 1;
            bool bBreakFor = true;
            switch (info.analyzeType)
            {
                case JSSerializer.AnalyzeType.ArrayObj:
                    {
                        bContinueTraverse = true;
                        Array arr = (Array)info.value;
                        Type arrayElementType = info.value.GetType().GetElementType();
                        for (var j = 0; j < arr.Length; j++)
                        {
                            object value = arr.GetValue(j);
                            Pos += AddAnalyze(arrayElementType, j.ToString(), value, Pos);
                        }
                        lst.RemoveAt(i);
                    }
                    break;
                case JSSerializer.AnalyzeType.StructObj:
                    {
                        bContinueTraverse = true;
                        var structure = info.value;
                        FieldInfo[] fields = GetTypeSerializedFields(structure.GetType());
                        foreach (FieldInfo field in fields)
                        {
                            Pos += AddAnalyze(field.FieldType, field.Name, field.GetValue(structure));
                        }
                        lst.RemoveAt(i);
                    }
                    break;
                case JSSerializer.AnalyzeType.ListObj:
                    {
                        bContinueTraverse = true;
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
                        lst.RemoveAt(i);
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
        GameObject go = behaviour.gameObject;
        Type type = behaviour.GetType();

        FieldInfo[] fields = GetMonoBehaviourSerializedFields(behaviour);
        foreach (FieldInfo field in fields)
        {
            AddAnalyze(field.FieldType, field.Name, field.GetValue(behaviour));
        }

        TraverseAnalyze();

        for (var i = 0; i < lst.Count; i++)
        {
            lst[i].Alloc(serizlizer);
        }
        serizlizer.AutoDelete = true;
        serizlizer.jsScriptName = JSDataExchangeMgr.GetTypeFullName(behaviour.GetType());
        serizlizer.arrString = lstString.ToArray();
        serizlizer.arrObjectSingle = lstObjs.ToArray();
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
                // replace this behaviour with JSComponent_SharpKit
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