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
    class SerializeStruct
    {
        public enum SType { Root, Array, Struct, List, Unit };
        public SType type;
        public string name;
        public JSApi.jsval val;
        public SerializeStruct father;
        List<SerializeStruct> lstChildren;
        public void AddChild(SerializeStruct ss)
        {
            if (lstChildren == null) 
                lstChildren = new List<SerializeStruct>();
            lstChildren.Add(ss);
        }
        public SerializeStruct(SType t, string name, SerializeStruct father)
        {
            type = t;
            this.name = name;
            this.father = father;
            val.asBits = 0;
        }
    }
    public int TraverseSerialize(IntPtr cx, IntPtr jsObj, int index, SerializeStruct st)
    {
        var i = index;
        for (/* */; i < arrString.Length; /* i++ */)
        {
            string s = arrString[i];
            int x = s.IndexOf('/');
            int y = s.IndexOf('/', x + 1);
            string s0 = s.Substring(0, x);
            string s1 = s.Substring(x + 1, y - x - 1);
            switch (s0)
            {
                case "ArrayBegin":
                case "StructBegin":
                case "ListBegin":
                    {
                        SerializeStruct.SType sType = SerializeStruct.SType.Array;
                        if (s0 == "StructBegin") sType = SerializeStruct.SType.Struct;
                        else if (s0 == "ListBegin") sType = SerializeStruct.SType.List;

                        var ss = new SerializeStruct(sType, s1, st);
                        st.AddChild(ss);
                        i += TraverseSerialize(cx, jsObj, i + 1, ss);
                    }
                    break;
                case "ArrayEnd":
                case "StructEnd":
                case "ListEnd":
                    {
                        i += TraverseSerialize(cx, jsObj, i + 1, st.father);
                    }
                    break;
                default:
                    {
                        UnitType eUnitType = (UnitType)int.Parse(s0);
                        if (eUnitType == UnitType.ST_UnityEngineObject)
                        {
                            var arr = s1.Split('/');
                            var valName = arr[0];
                            var objIndex = int.Parse(arr[1]);
                            JSMgr.vCall.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, this.arrObjectArray[objIndex]);

                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            child.val = JSMgr.vCall.valTemp;
                            st.AddChild(child);
                        }
                        else if (eUnitType == UnitType.ST_MonoBehaviour)
                        {
// TODO 最后再做
//                             var arr = s1.Split('/');
//                             var valName = arr[0];
//                             var objIndex = int.Parse(arr[1]);
//                             var scriptName = arr[2];
// 
//                             UnityEngine.Object obj = this.arrObjectArray[objIndex];
//                             cachedRefJSComponent.Add(new GameObject_JSComponentName(valName, (GameObject)obj, scriptName));
// 
//                             var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
//                             child.val = JSMgr.vCall.valTemp;
//                             st.AddChild(child);
                        }
                        else
                        {
                            var arr = s1.Split('/');
                            var valName = arr[0];
                            ToJsval(eUnitType, arr[1]);
                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            child.val = JSMgr.vCall.valTemp;
                            st.AddChild(child);
                        }
                        // !
                        i++;
                    }
                    break;
            }
        }
        return i - index;
    }
    // this function is called in Awake
    public void initSerializedData(IntPtr cx, IntPtr jsObj)
    {
        if (arrString == null || arrString.Length == 0)
        {
            return;
        }

        TraverseSerialize(cx, jsObj, 0, new SerializeStruct(SerializeStruct.SType.Root, "root", null));
    }
}