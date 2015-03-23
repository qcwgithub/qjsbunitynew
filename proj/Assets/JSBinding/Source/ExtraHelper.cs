using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ExtraHelper : MonoBehaviour
{
    public string scriptName;
    public string[] arrString = null;
    public UnityEngine.Object[] arrObject = new UnityEngine.Object[1];

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
    }

    public void initSerializedData(IntPtr cx, IntPtr jsObj)
    {
        for (var i = 0; i < arrString.Length; i++)
        {
            string s = arrString[i];
            int x = s.IndexOf('/');
            int y = s.IndexOf('/', x + 1);
            int eType = int.Parse(s.Substring(0, x));
            string name = s.Substring(x + 1, y - x);
            string strValue = s.Substring(y + 1, s.Length - y);

            switch ((SType)eType)
            {
            case SType.ST_Boolean:
                    {
                        bool v = strValue == "True";
                        JSMgr.vCall.datax.setBoolean(JSDataExchangeMgr.eSetType.Jsval, v);
                        JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
                    }
                    break;

            case SType.ST_SByte:
            case SType.ST_Char:
            case SType.ST_Int16:
            case SType.ST_Int32:
                    {
                        uint v;
                        if (uint.TryParse(strValue, out v))
                        {
                            JSMgr.vCall.datax.setUInt32(JSDataExchangeMgr.eSetType.Jsval, v);
                            JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
                        }
                    }
                    break;

            case SType.ST_Byte:
            case SType.ST_UInt16:
            case SType.ST_UInt32:
            case SType.ST_Enum:
                    {
                        int v;
                        if (int.TryParse(strValue, out v))
                        {
                            JSMgr.vCall.datax.setInt32(JSDataExchangeMgr.eSetType.Jsval, v);
                            JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
                        }
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
                            JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
                        }
                    }
                    break;
            case SType.ST_String:
                    {
                        JSMgr.vCall.datax.setString(JSDataExchangeMgr.eSetType.Jsval, strValue);
                        JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
                    }
                    break;
            case SType.ST_Vector2:
                    {
                        string[] xy = strValue.Split('/');
                        var v = new Vector2();
                        float.TryParse(xy[0], out v.x);
                        float.TryParse(xy[1], out v.y);
                        JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, v);
                        JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
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
                        JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
                    }
                    break;
                default:
                    break;
            }
        }

        for (var i = 0; i < arrObject.Length; i++)
        {
            UnityEngine.Object obj = arrObject[i];
            JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, obj);
            JSApi.JSh_SetUCProperty(cx, jsObj, name, -1, ref JSMgr.vCall.valTemp);
        }
    }

    
    static Dictionary<Type, SType> sDict;
    static SType GetTypeIndex(Type type)
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
            Debug.LogError("GetIndex: Unknown type: " + type.Name);
            return SType.ST_Unknown;
        }
        return ret;
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
			Type fieldType = field.FieldType;
            if (fieldType.IsArray)
            {
                continue;
            }

            SType eType = GetTypeIndex(fieldType);
            if (eType == SType.ST_Unknown)
            {
                continue;
            }

            sb.Remove(0, sb.Length);

            if (fieldType.IsPrimitive)
            {
                sb.AppendFormat("{0}/{1}/{2}", (int)eType, field.Name, field.GetValue(behaviour).ToString());
                lstString.Add(sb.ToString());
            }
            else if (fieldType.IsEnum)
            {
                sb.AppendFormat("{0}/{1}/{2}", (int)eType, field.Name, field.GetValue(behaviour).ToString());
                lstString.Add(sb.ToString());
            }
            else if (fieldType == typeof(string))
            {
                sb.AppendFormat("{0}/{1}/{2}", (int)eType, field.Name, field.GetValue(behaviour).ToString());
                lstString.Add(sb.ToString());
            }
            else if (fieldType == typeof(Vector2))
            {
                Vector2 v2 = (Vector2)field.GetValue(behaviour);
				sb.AppendFormat("{0}/{1}/{2}/{3}", (int)eType, field.Name, v2.x, v2.y);
				lstString.Add(sb.ToString());
			}
			else if (fieldType == typeof(Vector3))
            {
                Vector3 v3 = (Vector3)field.GetValue(behaviour);
				sb.AppendFormat("{0}/{1}/{2}/{3}/{4}", (int)eType, field.Name, v3.x, v3.y, v3.z);
				lstString.Add(sb.ToString());
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType))
            {
                lstObjs.Add((UnityEngine.Object)field.GetValue(behaviour));
            }
            else
            {
                return;
            }
        }

        helper.scriptName = behaviour.GetType().Name;
        helper.arrString = lstString.ToArray();
        helper.arrObject = lstObjs.ToArray();
    }
    public static void CopyGameObject<T>(GameObject go) where T : ExtraHelper
    {
        // delete original ExtraHelper(s)
        foreach (var eh in go.GetComponents<ExtraHelper>()) 
        {
            DestroyImmediate(eh);
        }

        var coms = go.GetComponents<MonoBehaviour>();
        for (var i = 0; i < coms.Length; i++)
        {
            var com = coms[i];
            ExtraHelper helper = (ExtraHelper)go.AddComponent<T>();
            CopyBehaviour(com, helper);
        }
    }
}