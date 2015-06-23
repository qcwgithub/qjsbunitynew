using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using SharpKit.JavaScript;

/// <summary>
/// JSSerializer
/// Serialize variables to JavaScript
/// Support: Primitive Type, string, enum, [], etc.
/// List<> is not supported now.
/// </summary>
public class JSSerializer : MonoBehaviour
{
    public string jsClassName = string.Empty;
    public string[] arrString = null;
    public UnityEngine.Object[] arrObject = null;

    public enum UnitType
    {
        ST_Unknown = 0,

        ST_Boolean = 1,

        ST_Byte = 2,
        ST_SByte = 3,
        ST_Char = 4,
        ST_Int16 = 5,
        ST_UInt16 = 6,
        ST_Int32 = 7,
        ST_UInt32 = 8,
        ST_Int64 = 9,
        ST_UInt64 = 10,

        ST_Single = 11,
        ST_Double = 12,

        ST_String = 13,

        ST_Enum = 14,
        ST_UnityEngineObject = 15,
        ST_JavaScriptMonoBehaviour = 16,

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
    /// <summary>
    /// Save a value in JavaScript and return ID
    /// </summary>
    /// <param name="eType">Data Type</param>
    /// <param name="strValue">Value of the variable in string format</param>
    /// <returns></returns>
    int toID(UnitType eType, string strValue)
    {
        switch ((UnitType)eType)
        {
            case UnitType.ST_Boolean:
                {
                    bool v = strValue == "True";
                    JSApi.setBooleanS((int)JSApi.SetType.SaveAndTempTrace, v);
                    return JSApi.getSaveID();
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
                        JSApi.setInt32((int)JSApi.SetType.SaveAndTempTrace, v);
                        return JSApi.getSaveID();
                    }
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
                        JSApi.setUInt32((int)JSApi.SetType.SaveAndTempTrace, v);
                        return JSApi.getSaveID();
                    }
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
                        JSApi.setDouble((int)JSApi.SetType.SaveAndTempTrace, v);
                        return JSApi.getSaveID();
                    }
                }
                break;
            case UnitType.ST_String:
                {
                    JSApi.setStringS((int)JSApi.SetType.SaveAndTempTrace, strValue);
                    return JSApi.getSaveID();
                }
                break;
            default:
                break;
        }
        return 0;
    }
    /// <summary>
    /// Get JSComponent ID of a GameObject by scriptName.
    /// </summary>
    /// <param name="go">The gameobject.</param>
    /// <param name="scriptName">Name of the script.</param>
    /// <returns></returns>
    public int GetGameObjectMonoBehaviourJSObj(GameObject go, string scriptName)
    {
        JSComponent[] jsComs = go.GetComponents<JSComponent>();
        foreach (var com in jsComs)
        {
            // NOTE£ºCan not bind a script to a GameObject twice!
            if (com.jsClassName == scriptName)
            {
                return com.GetJSObjID();
            }
        }
        return 0;
    }
    public class SerializeStruct
    {
        public enum SType { Root, Array, Struct, List, Unit };
        public SType type;
        public string name;
        public string typeName;
        public int __id;
        public int id
        {
            get { return __id; }
            set
            {
//                 if (value != 0)
//                     JSApi.setTrace(value, true);
                __id = value;
            }
        }
        public SerializeStruct father;
        public List<SerializeStruct> lstChildren;
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
            typeName = "WRONGTYPENAME!";
            __id = 0;
        }
        public void removeID()
        {
            if (this.id != 0)
            {
                JSApi.removeByID(this.id);
                this.id = 0;
            }
            if (lstChildren != null)
            {
                foreach (var child in lstChildren)
                {
                    child.removeID();
                }
            }
        }
        /// <summary>
        /// Calc jsval
        /// save in this.val    and return it
        /// </summary>
        /// <returns></returns>
        public int calcID()
        {
            switch (this.type)
            {
                case SType.Unit:
                    // already calucated when TraverseSerialize()
                    break;
                case SType.Array:
                    {
                        int Count = lstChildren.Count;
                        for (var i = 0; i < Count; i++)
                        {
                            int id = lstChildren[i].calcID();
                            JSApi.moveID2Arr(id, i);
                        }
                        JSApi.setArrayS((int)JSApi.SetType.SaveAndTempTrace, Count, false);
                        this.id = JSApi.getSaveID();
                    }
                    break;
                case SType.Struct:
                    {
                        //
                        // the process here is a little complex
                        // for C# class like RaycastHit, UnityEngine.RaycastHit.ctor will be called to create object, because it's C# class, ctor actually goes to C#
                        // subsequent call to setProperty actually also goes to C#
                        //
                        // for pure JavaScript class, ctor and setProperty are done in JavaScript
                        //
                        int jsObjID = JSApi.newJSClassObject(this.typeName);
                        this.id = jsObjID;
                        if (jsObjID == 0)
                        {
                            Debug.LogError("Serialize error: call \"" + this.typeName + "\".ctor return null, , did you forget to export that class?");
                        }
                        else
                        {
                            //IntPtr jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null /*JSMgr.mjsFinalizer*/);
                            for (var i = 0; lstChildren != null && i < lstChildren.Count; i++)
                            {
                                var child = lstChildren[i];
                                int id = child.calcID();
                                //JSApi.JSh_SetUCProperty(JSMgr.cx, jsObjID, child.name, -1, ref mVal);
                                JSApi.setProperty(jsObjID, child.name, id);
                            }
                        }
                    }
                    break;
                case SType.List:
                    {
                        // List is not supported now.
                        // please use [] instead.
                    }
                    break;
            }
            return this.id;
        }
    }
    /// <summary>
    /// Traverses the serialization.
    /// </summary>
    /// <param name="jsObjID">The js object identifier.</param>
    /// <param name="st">The parent struct.</param>
    public void TraverseSerialize(int jsObjID, SerializeStruct st)
    {
        while (true)
        {
            string s = NextString();
            if (s == null)
                return;

            int x = s.IndexOf('/');
            int y = s.IndexOf('/', x + 1);
            string s0 = s.Substring(0, x);
            string s1 = s.Substring(x + 1, y - x - 1);
            switch (s0)
            {
                case "ArrayBegin":
                    {
                        SerializeStruct.SType sType = SerializeStruct.SType.Array;
                        var ss = new SerializeStruct(sType, s1, st);
                        st.AddChild(ss);
                        TraverseSerialize(jsObjID, ss);
                    }
                    break;
                // StructBegin and ListBegin also contains type
                case "StructBegin":
                case "ListBegin":
                    {
                        SerializeStruct.SType sType = SerializeStruct.SType.Array;
                        if (s0 == "StructBegin") sType = SerializeStruct.SType.Struct;
                        else if (s0 == "ListBegin") sType = SerializeStruct.SType.List;
                        string s2 = s.Substring(y + 1, s.Length - y - 1);

                        var ss = new SerializeStruct(sType, s1, st);
                        ss.typeName = s2;
                        st.AddChild(ss);
                        TraverseSerialize(jsObjID, ss);
                    }
                    break;
                case "ArrayEnd":
                case "StructEnd":
                case "ListEnd":
                    {
                        // ! return here
                        return;
                    }
                    break;
                default:
                    {
                        UnitType eUnitType = (UnitType)int.Parse(s0);
                        if (eUnitType == UnitType.ST_UnityEngineObject)
                        {
                            string s2 = s.Substring(y + 1, s.Length - y - 1);
                            var valName = s1;
                            var objIndex = int.Parse(s2);
                            JSMgr.datax.setObject((int)JSApi.SetType.SaveAndTempTrace, this.arrObject[objIndex]);

                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            child.id = JSApi.getSaveID();
                            st.AddChild(child);
                        }
                        else if (eUnitType == UnitType.ST_JavaScriptMonoBehaviour)
                        {
                            var valName = s1;
                            string s2 = s.Substring(y + 1, s.Length - y - 1);
                            var arr = s2.Split('/');
                            var objIndex = int.Parse(arr[0]);
                            var scriptName = arr[1];

                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            int refJSObjID = this.GetGameObjectMonoBehaviourJSObj((GameObject)this.arrObject[objIndex], scriptName);
                            if (refJSObjID == 0)
                            {
                                child.id = 0;
                            }
                            else
                            {
                                JSApi.setObject((int)JSApi.SetType.SaveAndTempTrace, refJSObjID);
                                child.id = JSApi.getSaveID();
                            }

                            st.AddChild(child);
                        }
                        else
                        {
                            string s2 = s.Substring(y + 1, s.Length - y - 1);
                            var valName = s1;
                            int id = toID(eUnitType, s2);
                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            //child.val = JSMgr.vCall.valTemp;
                            child.id = id;
                            st.AddChild(child);
                        }
                    }
                    break;
            }
        }
    }
    int arrStringIndex = 0;
    string NextString()
    {
        if (arrString == null) return null;
        if (arrStringIndex >= 0 && arrStringIndex < arrString.Length)
        {
            return arrString[arrStringIndex++];
        }
        return null;
    }
    /// <summary>
    /// Initializes the serialized data.
    /// </summary>
    /// <param name="jsObjID">The js object identifier.</param>
    public void initSerializedData(int jsObjID)
    {
        if (arrString == null || arrString.Length == 0)
        {
            return;
        }

        var root = new SerializeStruct(SerializeStruct.SType.Root, "this-name-doesn't-matter", null);
        TraverseSerialize(jsObjID, root);
        if (root.lstChildren != null)
        {
            foreach (var child in root.lstChildren)
            {
                child.calcID();
                JSApi.setProperty(jsObjID, child.name, child.id);
            }
        }
        root.removeID();
    }
}