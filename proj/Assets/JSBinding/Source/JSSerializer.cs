using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;

public class JSSerializer : MonoBehaviour
{
    /*
     * AutoDelete: if true  will be automatically deleted when needed (when press Alt + Shift + Q)
     * DON'T change this manually
     */
    [HideInInspector]
    public bool AutoDelete = false;

    public string jsScriptName = string.Empty;
    public string[] arrString = null;
    public UnityEngine.Object[] arrObjectSingle = null;
    public UnityEngine.Object[] arrObjectArray = null;


    private struct GameObject_JSComponentName
    {
        public string valName;
        public GameObject go;
        public string scriptName;
        public GameObject_JSComponentName(string _valName, GameObject _go, string _scriptName) { valName = _valName; go = _go; scriptName = _scriptName; }
    }
    private List<GameObject_JSComponentName> cachedRefJSComponent = new List<GameObject_JSComponentName>();
    enum UnitType
    {
        ST_Unknown,

        ST_Boolean,

        ST_Byte,
        ST_SByte,
        ST_Char,
        ST_Int16,
        ST_UInt16,
        ST_Int32,
        ST_UInt32,
        ST_Int64,
        ST_UInt64,

        ST_Single,
        ST_Double,

        ST_String,

        ST_Enum,
        ST_UnityEngineObject,
        ST_MonoBehaviour,

        ST_MAX = 100,
    }
    enum AnalyzeType
    {
        Unit,

        ArrayBegin,
        ArrayObj,
        ArrayEnd,

        StructBegin,
        StructObj,
        StructEnd,

        ListBegin,
        ListObj,
        ListEnd,
    }

    bool ToJsval(UnitType eType, string strValue)
    {
        bool ret = true;
        switch ((UnitType)eType)
        {
            case UnitType.ST_Boolean:
                {
                    bool v = strValue == "True";
                    JSMgr.vCall.datax.setBoolean(JSDataExchangeMgr.eSetType.Jsval, v);
                }
                break;

            case UnitType.ST_SByte:
            case UnitType.ST_Char:
            case UnitType.ST_Int16:
            case UnitType.ST_Int32:
                {
                    int v;
                    if (int.TryParse(strValue, out v))
                    {
                        JSMgr.vCall.datax.setInt32(JSDataExchangeMgr.eSetType.Jsval, v);
                    }
                    else ret = false;
                }
                break;

            case UnitType.ST_Byte:
            case UnitType.ST_UInt16:
            case UnitType.ST_UInt32:
            case UnitType.ST_Enum:
                {
                    uint v;
                    if (uint.TryParse(strValue, out v))
                    {
                        JSMgr.vCall.datax.setUInt32(JSDataExchangeMgr.eSetType.Jsval, v);
                    }
                    else ret = false;
                }
                break;
            case UnitType.ST_Int64:
            case UnitType.ST_UInt64:
            case UnitType.ST_Single:
            case UnitType.ST_Double:
                {
                    double v;
                    if (double.TryParse(strValue, out v))
                    {
                        JSMgr.vCall.datax.setDouble(JSDataExchangeMgr.eSetType.Jsval, v);
                    }
                    else ret = false;
                }
                break;
            case UnitType.ST_String:
                {
                    JSMgr.vCall.datax.setString(JSDataExchangeMgr.eSetType.Jsval, strValue);
                }
                break;
            default:
                ret = false;
                break;
        }
        return ret;
    }
    string help_ThirdS(string s)
    {
        int x = s.IndexOf('/');
        int y = s.IndexOf('/', x + 1);
        string strValue = s.Substring(y + 1, s.Length - y - 1);
        return strValue;
    }
    // this function is called in Start
    public void initSerializedRefMonoBehaviour(IntPtr cx, IntPtr jsObj)
    {
        foreach (var rel in cachedRefJSComponent)
        {
            GameObject go = rel.go;
            JSComponent_SharpKit[] jsComs = go.GetComponents<JSComponent_SharpKit>();
            foreach (var com in jsComs)
            {
                if (com.jsScriptName == rel.scriptName)
                {
                    // 注意：最多只能绑同 一个名字的脚本一次  
                    // 现在只能支持这样
                    // 数组也不行  要注意
                    JSApi.JSh_SetJsvalObject(ref JSMgr.vCall.valTemp, com.jsObj);
                    JSApi.JSh_SetUCProperty(cx, jsObj, rel.valName, -1, ref JSMgr.vCall.valTemp);
                    break;
                }
            }
        }
        cachedRefJSComponent.Clear();
    }
    // this function is called in Awake
    public void initSerializedData(IntPtr cx, IntPtr jsObj)
    {
        if (arrString == null || arrString.Length == 0)
            return;

        for (var i = 0; i < arrString.Length; i++)
        {
            string s = arrString[i];

        }
    }
    enum SerializeType
    {
        String,
        Object,
    }
    public struct AnalyzeStructInfo
    {
        public AnalyzeType analyzeType;
        public string Name;
        public object value;
        public UnitType unitType;

        public AnalyzeStructInfo(AnalyzeType at, string name, object v = null, UnitType ut = UnitType.ST_Unknown) 
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
                case AnalyzeType.ArrayBegin:
                    serizlizeIndex = AllocString("ArrayBegin");
                    break;
                case AnalyzeType.ArrayEnd:
                    serizlizeIndex = AllocString("ArrayEnd");
                    break;
                case AnalyzeType.StructBegin:
                    serizlizeIndex = AllocString("StructBegin");
                    break;
                case AnalyzeType.StructEnd:
                    serizlizeIndex = AllocString("StructEnd");
                    break;
                case AnalyzeType.ListBegin:
                    serizlizeIndex = AllocString("ListBegin");
                    break;
                case AnalyzeType.ListEnd:
                    serizlizeIndex = AllocString("ListEnd");
                    break;
                case AnalyzeType.Unit:
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
                        }
                        else
                        {
                            string str = ValueToString(this.value, this.value.GetType(), this.unitType, this.Name);
                            AllocString(str);
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
        UnitType unitType = GetUnitType(type);
        if (unitType != UnitType.ST_Unknown)
        {
            lst.Insert(index, new AnalyzeStructInfo(AnalyzeType.Unit, name, value, unitType));
            return 1;
        }
        else
        {
            if (type.IsArray)
            {
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ArrayBegin, name));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ArrayObj, name, value));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ArrayEnd, name));
                return 3;
            }
            else if (type.IsClass || type.IsValueType)
            {
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ListBegin, name));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ListObj, name, value));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ListEnd, name));
                return 3;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.StructBegin, name));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.StructObj, name, value));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.StructEnd, name));
                return 3;
            }
        }
        return 0;
    }


    static Dictionary<Type, UnitType> sDict;
    static UnitType GetUnitType(Type type)
    {
        if (sDict == null)
        {
            sDict = new Dictionary<Type, UnitType>();

            sDict.Add(typeof(Boolean), UnitType.ST_Boolean);

            sDict.Add(typeof(Byte), UnitType.ST_Byte);
            sDict.Add(typeof(SByte), UnitType.ST_SByte);
            sDict.Add(typeof(Char), UnitType.ST_Char);
            sDict.Add(typeof(Int16), UnitType.ST_Int16);
            sDict.Add(typeof(UInt16), UnitType.ST_UInt16);
            sDict.Add(typeof(Int32), UnitType.ST_Int32);
            sDict.Add(typeof(UInt32), UnitType.ST_UInt32);
            sDict.Add(typeof(Int64), UnitType.ST_Int64);
            sDict.Add(typeof(UInt64), UnitType.ST_UInt64);

            sDict.Add(typeof(Single), UnitType.ST_Single);
            sDict.Add(typeof(Double), UnitType.ST_Double);


            sDict.Add(typeof(String), UnitType.ST_String);
        }

        if (type.IsEnum)
        {
            return UnitType.ST_Enum;
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
            return UnitType.ST_Unknown;
        }
        return ret;
    }

    static string ValueToString(object value, Type type, UnitType eType, string name)
    {
        //
        // eType / name / value
        //
        StringBuilder sb = new StringBuilder();
        if (type.IsPrimitive)
        {
            sb.AppendFormat("{0}/{1}/{2}", (int)eType, name, value.ToString());
        }
        else if (type.IsEnum)
        {
            sb.AppendFormat("{0}/{1}/{2}", (int)eType, name, (int)Enum.Parse(type, value.ToString()));
        }
        else if (type == typeof(string))
        {
            sb.AppendFormat("{0}/{1}/{2}", (int)eType, name, value.ToString());
        }
        else if (type == typeof(Vector2))
        {
            Vector2 v2 = (Vector2)value;
            sb.AppendFormat("{0}/{1}/{2}/{3}", (int)eType, name, v2.x, v2.y);
        }
        else if (type == typeof(Vector3))
        {
            Vector3 v3 = (Vector3)value;
            sb.AppendFormat("{0}/{1}/{2}/{3}/{4}", (int)eType, name, v3.x, v3.y, v3.z);
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
                case AnalyzeType.ArrayObj:
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
                case AnalyzeType.StructObj:
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
                case AnalyzeType.ListObj:
                    {
                        bContinueTraverse = true;
                        var list = info.value;
                        var listType = list.GetType();
                        var listElementType = listType.GetGenericArguments()[0];

                        int Count = (int)listType.GetProperty("Count").GetValue(list, new object[] { });
                        PropertyInfo pro = listType.GetProperty("Item");
                        
                        for (var j = 0; j < Count; j++)
                        {
                            var value = pro.GetValue(list, new object[]{ j });
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

            DestroyImmediate(com, true);
        }
    }
}