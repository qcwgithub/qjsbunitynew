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
    public enum UnitType
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
    public enum AnalyzeType
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

}