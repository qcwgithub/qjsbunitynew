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
            case UnitType.ST_Vector2:
                {
                    string[] xy = strValue.Split('/');
                    var v = new Vector2();
                    float.TryParse(xy[0], out v.x);
                    float.TryParse(xy[1], out v.y);
                    JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, v);
                }
                break;
            case UnitType.ST_Vector3:
                {
                    string[] xyz = strValue.Split('/');
                    var v = new Vector3();
                    float.TryParse(xyz[0], out v.x);
                    float.TryParse(xyz[1], out v.y);
                    float.TryParse(xyz[2], out v.z);
                    JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, v);
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
        int arrObjectIndex = 0;
        int arrObjectIndexOfArray = 0;

        if (arrString == null)
            return;

        //
        // handle arrString first
        //
        for (var i = 0; i < arrString.Length; i++)
        {
            string s = arrString[i];
            int x = s.IndexOf('/');
            int y = s.IndexOf('/', x + 1);

            if (x < 0 || y < 0) continue;

            string s0 = s.Substring(0, x);
            string s1 = s.Substring(x + 1, y - x - 1);

            if (s0 != "Array")
            {
                UnitType eType = (UnitType)int.Parse(s0);
                string valName = s1;

                if (eType == UnitType.ST_UnityEngineObject)
                {
                    // arrObjectIndex++ here
                    UnityEngine.Object obj = arrObjectSingle[arrObjectIndex++];
                    JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, obj);
                    JSApi.JSh_SetUCProperty(cx, jsObj, valName, -1, ref JSMgr.vCall.valTemp);
                }
                else if (eType == UnitType.ST_MonoBehaviour)
                {
                    // eType / Name / lstObjsSingle's Index / Monobehaviour name
                    //
                    //
                    string strValue = s.Substring(y + 1, s.Length - y - 1);

                    x = strValue.IndexOf('/');
                    s0 = strValue.Substring(0, x);
                    if (arrObjectIndex != int.Parse(s0)) Debug.LogError("Serialize MonoBehaviour Error: arrObjectIndex != int.Parse(s0)");
                    string scriptName = strValue.Substring(x + 1, strValue.Length - x - 1);

                    UnityEngine.Object obj = arrObjectSingle[arrObjectIndex++];
                    cachedRefJSComponent.Add(new GameObject_JSComponentName(valName, (GameObject)obj, scriptName));
                }
                else
                {
                    string strValue = s.Substring(y + 1, s.Length - y - 1);
                    if (ToJsval(eType, strValue))
                        JSApi.JSh_SetUCProperty(cx, jsObj, valName, -1, ref JSMgr.vCall.valTemp);
                }
            }
            else
            {
                UnitType eType = (UnitType)int.Parse(s1);
                string[] s2 = s.Substring(y + 1, s.Length - y - 1).Split('/');
                if (s2.Length != 2)
                {
                    // !
                    return;
                }
                string valName = s2[0];
                int Count = 0;
                if (!int.TryParse(s2[1], out Count))
                {
                    // !
                    return;
                }

                var arrVal = new JSApi.jsval[Count];
                for (int j = 0; j < Count; j++)
                {
                    if (eType == UnitType.ST_UnityEngineObject)
                    {
                        // arrObjectIndex++ here
                        UnityEngine.Object obj = arrObjectArray[arrObjectIndexOfArray++];
                        JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, obj);
                    }
                    else
                    {
                        string strValue = help_ThirdS(arrString[i + 1 + j]);
                        ToJsval(eType, strValue);
                    }
                    arrVal[j] = JSMgr.vCall.valTemp;
                }
                JSMgr.vCall.datax.setArray(JSDataExchangeMgr.eSetType.Jsval, arrVal);
                JSApi.JSh_SetUCProperty(cx, jsObj, valName, -1, ref JSMgr.vCall.valTemp);

                if (eType != UnitType.ST_UnityEngineObject)
                {
                    i += Count;
                }
            }
        }

        if (arrObjectIndex == arrObjectSingle.Length &&
            arrObjectIndexOfArray == arrObjectArray.Length)
        {
            Debug.Log(name + " serialized perfect");
        }
        else
        {
            Debug.LogError(name + " serialized FAIL");
        }
    }

    public struct AnalyzeStructInfo
    {
        public AnalyzeType analyzeType;
        public object value;
        public UnitType unitType;
        public AnalyzeStructInfo(AnalyzeType at, object v = null, UnitType ut = UnitType.ST_Unknown) {
            analyzeType = at;
            value = v;
            unitType = ut;
        }
    }

    static List<AnalyzeStructInfo> lst = new List<AnalyzeStructInfo>();
    public static int AddAnalyze(Type type, object value, int index = -1)
    {
        if (index == -1) index = lst.Count;
        UnitType unitType = GetUnitType(type);
        if (unitType != UnitType.ST_Unknown)
        {
            lst.Insert(index, new AnalyzeStructInfo(AnalyzeType.Unit, value, unitType));
            return 1;
        }
        else
        {
            if (type.IsArray)
            {
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ArrayBegin));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ArrayObj, value));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ArrayEnd));
                return 3;
            }
            else if (type.IsClass || type.IsValueType)
            {
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ListBegin));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ListObj, value));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.ListEnd));
                return 3;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.StructBegin));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.StructObj, value));
                lst.Insert(index++, new AnalyzeStructInfo(AnalyzeType.StructEnd));
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

        if (type == typeof(Vector2)) return UnitType.ST_Vector2;
        if (type == typeof(Vector3)) return UnitType.ST_Vector3;

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
        //         else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
        //         {
        //             lstObjs.Add((UnityEngine.Object)value);
        //         }
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
    static void CopyBehaviour(MonoBehaviour behaviour, JSSerializer helper)
    {
        GameObject go = behaviour.gameObject;
        Type type = behaviour.GetType();

        FieldInfo[] fields = GetMonoBehaviourSerializedFields(behaviour);
        foreach (FieldInfo field in fields)
        {
            AddAnalyze(field.FieldType, field.GetValue(behaviour));
        }
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