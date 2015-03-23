using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class ExtraHelper : MonoBehaviour
{
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
    }

    static Dictionary<Type, SType> sDict;
    SType GetIndex(Type type)
    {
        if (sDict == null)
        {
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

        SType ret = SType.ST_Unknown;
        if (!sDict.TryGetValue(type, out ret)) 
        {
            Debug.LogError("GetIndex: Unknown type: " + type.Name);
            return SType.ST_Unknown;
        }
        return ret;
    }


    public GameObject go;
    void CopyOther(MonoBehaviour behaviour)
    {
        GameObject go = behaviour.gameObject;
        Type type = behaviour.GetType();

        List<string> lstString = new List<string>();
        List<UnityEngine.Object> lstObjs = new List<UnityEngine.Object>();
        StringBuilder sb = new StringBuilder();

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Instance /* | BindingFlags.Static */ );
        foreach (var field in fields)
        {
            Type fieldType = field.GetType();
            SType eType = this.GetIndex(fieldType);
            if (eType == SType.ST_Unknown)
            {
                return;
            }

            sb.Remove(0, sb.Length);

            if (fieldType.IsPrimitive)
            {
                sb.AppendFormat("{0}/{1}={2}", (int)eType, field.Name, field.GetValue(behaviour).ToString());
                lstString.Add(sb.ToString());
            }
            else if (fieldType.IsEnum)
            {
                sb.AppendFormat("{0}/{1}={2}", (int)eType, field.Name, field.GetValue(behaviour).ToString());
                lstString.Add(sb.ToString());
            }
            else if (fieldType == typeof(string))
            {
                sb.AppendFormat("{0}/{1}={2}", (int)eType, field.Name, field.GetValue(behaviour).ToString());
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

        this.arrString = lstString.ToArray();
        this.arrObject = lstObjs.ToArray();
    }
}