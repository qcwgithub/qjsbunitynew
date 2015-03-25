using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ExtraHelper : MonoBehaviour
{
    /*
     * AutoDelete: if true  will be automatically deleted when needed (when press Alt + Shift + Q)
     * DON'T change this manually
     */
    [HideInInspector]
    public bool AutoDelete = false;

    public string jsScriptName = string.Empty;
    public string[] arrString = null;
    public UnityEngine.Object[] arrObject = null;

    enum SType
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

        ST_Vector2,
        ST_Vector3,

        ST_MAX = 100,
    }

    bool ToJsval(SType eType, string strValue)
    {
        bool ret = true;
        switch ((SType)eType)
        {
            case SType.ST_Boolean:
                {
                    bool v = strValue == "True";
                    JSMgr.vCall.datax.setBoolean(JSDataExchangeMgr.eSetType.Jsval, v);
                }
                break;

            case SType.ST_SByte:
            case SType.ST_Char:
            case SType.ST_Int16:
            case SType.ST_Int32:
                {
                    int v;
                    if (int.TryParse(strValue, out v))
                    {
                        JSMgr.vCall.datax.setInt32(JSDataExchangeMgr.eSetType.Jsval, v);
                    }
                    else ret = false;
                }
                break;

            case SType.ST_Byte:
            case SType.ST_UInt16:
            case SType.ST_UInt32:
            case SType.ST_Enum:
                {
                    uint v;
                    if (uint.TryParse(strValue, out v))
                    {
                        JSMgr.vCall.datax.setUInt32(JSDataExchangeMgr.eSetType.Jsval, v);
                    }
                    else ret = false;
                }
                break;
            case SType.ST_Int64:
            case SType.ST_UInt64:
            case SType.ST_Single:
            case SType.ST_Double:
                {
                    double v;
                    if (double.TryParse(strValue, out v))
                    {
                        JSMgr.vCall.datax.setDouble(JSDataExchangeMgr.eSetType.Jsval, v);
                    }
                    else ret = false;
                }
                break;
            case SType.ST_String:
                {
                    JSMgr.vCall.datax.setString(JSDataExchangeMgr.eSetType.Jsval, strValue);
                }
                break;
            case SType.ST_Vector2:
                {
                    string[] xy = strValue.Split('/');
                    var v = new Vector2();
                    float.TryParse(xy[0], out v.x);
                    float.TryParse(xy[1], out v.y);
                    JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, v);
                }
                break;
            case SType.ST_Vector3:
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
    public void initSerializedData(IntPtr cx, IntPtr jsObj)
    {
        int arrObjectIndex = 0;

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
                SType eType = (SType)int.Parse(s0);
                string name = s1;
                string strValue = s.Substring(y + 1, s.Length - y - 1);

                if (ToJsval(eType, strValue))
                {
                    JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
                }
            }
            else
            {
                SType eType = (SType)int.Parse(s1);
                string[] s2 = s.Substring(y + 1, s.Length - y - 1).Split('/');
                if (s2.Length != 2)
                {
                    // !
                    return;
                }
                string name = s2[0];
                int Count = 0;
                if (!int.TryParse(s2[1], out Count))
                {
                    // !
                    return;
                }

                var arrVal = new JSApi.jsval[Count];
                for (int j = 0; j < Count; j++)
                {
                    if (eType == SType.ST_UnityEngineObject)
                    {
                        // arrObjectIndex++ here
                        UnityEngine.Object obj = arrObject[arrObjectIndex++];
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
                JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);

                if (eType != SType.ST_UnityEngineObject)
                {
                    i += Count;
                }
            }
        }

        for (var i = arrObjectIndex; i < arrObject.Length; i++)
        {
            UnityEngine.Object obj = arrObject[i];
            JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, obj);
            JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
        }
    }

    
    static Dictionary<Type, SType> sDict;
    static SType GetSType(Type type)
    {
        if (sDict == null)
        {
            sDict = new Dictionary<Type, SType>();

            sDict.Add(typeof(Boolean), SType.ST_Boolean);

            sDict.Add(typeof(Byte), SType.ST_Byte);
            sDict.Add(typeof(SByte), SType.ST_SByte);
            sDict.Add(typeof(Char), SType.ST_Char);
            sDict.Add(typeof(Int16), SType.ST_Int16);
            sDict.Add(typeof(UInt16), SType.ST_UInt16);
            sDict.Add(typeof(Int32), SType.ST_Int32);
            sDict.Add(typeof(UInt32), SType.ST_UInt32);
            sDict.Add(typeof(Int64), SType.ST_Int64);
            sDict.Add(typeof(UInt64), SType.ST_UInt64);

            sDict.Add(typeof(Single), SType.ST_Single);
            sDict.Add(typeof(Double), SType.ST_Double);


            sDict.Add(typeof(String), SType.ST_String);
        }

        if (type.IsEnum)
        {
            return SType.ST_Enum;        
        }

        if ((typeof(UnityEngine.Object).IsAssignableFrom(type)))
        {
            return SType.ST_UnityEngineObject;
        }

        if (type == typeof(Vector2)) return SType.ST_Vector2;
        if (type == typeof(Vector3)) return SType.ST_Vector3;

        SType ret = SType.ST_Unknown;
        if (!sDict.TryGetValue(type, out ret)) 
        {
            // Debug.LogError("GetIndex: Unknown type: " + type.Name);
            return SType.ST_Unknown;
        }
        return ret;
    }

    static string ValueToString(object value, Type type, SType eType, string name)
    {
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

    static void CopyBehaviour(MonoBehaviour behaviour, ExtraHelper helper)
    {
        GameObject go = behaviour.gameObject;
        Type type = behaviour.GetType();

        List<string> lstString = new List<string>();
        List<UnityEngine.Object> lstObjs = new List<UnityEngine.Object>();
        StringBuilder sb = new StringBuilder();

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance /* | BindingFlags.Static */ );
        foreach (var field in fields)
        {
            if (!field.FieldType.IsArray)
            {
                SType eType = GetSType(field.FieldType);
                if (eType == SType.ST_Unknown)
                {
                    continue;
                }

                sb.Remove(0, sb.Length);
                if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
                {
                    lstObjs.Add((UnityEngine.Object)field.GetValue(behaviour));
                }
                else
                {
                    string str = ValueToString(field.GetValue(behaviour), field.FieldType, eType, field.Name);
                    lstString.Add(str);
                }
            }
            else
            {
                Type elementType = field.FieldType.GetElementType();
                SType eType = GetSType(elementType);
                if (eType == SType.ST_Unknown)
                {
                    continue;
                }

                Array arr = (Array)field.GetValue(behaviour);

                // Array / eType / fildName / Count
                lstString.Add("Array/" + ((int)eType).ToString() + "/" + field.Name + "/" + arr.Length.ToString());

                for (var i = 0; i < arr.Length; i++)
                {
                    object value = arr.GetValue(i);
                    if (typeof(UnityEngine.Object).IsAssignableFrom(elementType))
                    {
                        lstObjs.Add((UnityEngine.Object)value);
                    }
                    else
                    {
                        string str = ValueToString(value, elementType, eType, "[" + i.ToString() + "]");
                        lstString.Add(str);
                    }
                }
            }
        }

        helper.AutoDelete = true;
        helper.jsScriptName = behaviour.GetType().Name;
        helper.arrString = lstString.ToArray();
        helper.arrObject = lstObjs.ToArray();
    }
    public static void CopyGameObject<T>(GameObject go) where T : ExtraHelper
    {
        // delete original ExtraHelper(s)
        foreach (var eh in go.GetComponents<ExtraHelper>()) 
        {
            if (eh.AutoDelete)
            {
                // only delete when Auto is true
                DestroyImmediate(eh);
            }
        }

        var coms = go.GetComponents<MonoBehaviour>();
        for (var i = 0; i < coms.Length; i++)
        {
            var com = coms[i];
            // must ignore ExtraHandler here
            if (com is ExtraHelper) 
                continue;

            ExtraHelper helper = (ExtraHelper)go.AddComponent<T>();
            CopyBehaviour(com, helper);
        }
    }
}