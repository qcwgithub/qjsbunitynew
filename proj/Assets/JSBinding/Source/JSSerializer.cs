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
    public string jsScriptName = string.Empty;
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
        ST_MonoBehaviour = 16,

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
    // TODO 修改注释
    // and check
    /// <summary>
    /// 根据 eType 将 strValue 转换为 jsval
    /// </summary>
    /// <param name="eType"></param>
    /// <param name="strValue"></param>
    /// <returns></returns>
    int toID(UnitType eType, string strValue)
    {
        switch ((UnitType)eType)
        {
            case UnitType.ST_Boolean:
                {
                    bool v = strValue == "True";
                    JSApi.setBoolean((int)JSApi.SetType.SaveAndTempTrace, v);
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
                    // TODO check
                    // JSMgr.vCall.datax.setString(JSDataExchangeMgr.eSetType.Jsval, strValue);
                    JSApi.setStringS((int)JSApi.SetType.SaveAndTempTrace, strValue);
                    return JSApi.getSaveID();
                }
                break;
            default:
                break;
        }
        return 0;
    }
    public int GetGameObjectMonoBehaviourJSObj(GameObject go, string scriptName)
    {
        JSComponent[] jsComs = go.GetComponents<JSComponent>();
        foreach (var com in jsComs)
        {
            // 注意：同一个GameObject不能绑相同名字的脚本2次以上
            if (com.jsScriptName == scriptName)
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
                if (value != 0)
                    JSApi.setTrace(value, true);
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
                    // 在 TraverseSerialize() 的时候就已经计算好了
                    break;
                case SType.Array:
                    {
                        int Count = lstChildren.Count;
                        for (var i = 0; i < Count; i++)
                        {
                            int id = lstChildren[i].calcID();
                            JSApi.moveID2Arr(id, i);
                        }
                        JSApi.setArray((int)JSApi.SetType.SaveAndTempTrace, Count, false);
                        this.id = JSApi.getSaveID();
                    }
                    break;
                case SType.Struct:
                    {
                        /*
                         * 这里的过程比较复杂，可以同时支持在 CS 里的类和 JS 里的类
                         * 以 Vector3 为例，会先调用 UnityEngine.Vector3.ctor 创建对象，他是C#的类，所以实际会走到C#去生成对象
                         * 后面调用 SetUCProperty 时，也会触发 JS 的 property，实际也是调用到C#去了
                         * 
                         * 如果不是 C# 的类，那么新建类对象和SetUCProperty都不会走到C#来，在JS中完成
                         */
                        //JSApi.jsval valParam = new JSApi.jsval(); valParam.asBits = 0;
                        //JSApi.JSh_SetJsvalString(JSMgr.cx, ref valParam, this.typeName);
                        //JSApi.JSh_CallFunctionName(JSMgr.cx, JSMgr.glob, "jsb_CallObjectCtor", 1, new JSApi.jsval[]{valParam}, ref JSMgr.vCall.rvalCallJS);
                        //IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref JSMgr.vCall.rvalCallJS);
                        int jsObjID = JSApi.newJSClassObject(this.typeName);
                        this.id = jsObjID;
                        if (jsObjID == 0)
                        {
                            Debug.LogError("Serialize error: call \"" + this.typeName + "\".ctor return null, , did you forget to export that class?");
                            //JSApi.JSh_SetJsvalUndefined(ref this.val);
                            //this.id = 0;
                        }
                        else
                        {
                            //IntPtr jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null /*JSMgr.mjsFinalizer*/);
                            for (var i = 0; i < lstChildren.Count; i++)
                            {
                                var child = lstChildren[i];
                                int id = child.calcID();
                                //JSApi.JSh_SetUCProperty(JSMgr.cx, jsObjID, child.name, -1, ref mVal);
                                JSApi.setProperty(jsObjID, child.name, id);
                            }
                            //JSApi.JSh_SetJsvalObject(ref this.val, jsObj);
//                             JSApi.setObject((int)JSApi.SetType.Save, jsObjID);
//                             this.id = JSApi.getSaveID();
                        }
                        
                        /*
                        IntPtr jstypeObj = JSDataExchangeMgr.GetJSObjectByname(this.typeName);
                        if (jstypeObj == IntPtr.Zero)
                        {
                            Debug.LogError("JSSerialize fail. New object \"" + this.typeName + "\" fail, did you forget to export that class?");
                            this.val.asBits = 0;
                        }
                        else
                        {
                            JSApi.jsval valFun; valFun.asBits = 0;
                            JSApi.GetProperty(JSMgr.cx, jstypeObj, "ctor", -1, ref valFun);
                            if (valFun.asBits == 0 || JSApi.JSh_JsvalIsNullOrUndefined(ref valFun))
                            {
                                Debug.LogError("Serialize error: " + this.typeName + ".ctor is not a function");
                                JSApi.JSh_SetJsvalUndefined(ref this.val);
                            }
                            else
                            {
                                JSMgr.vCall.CallJSFunctionValue(jstypeObj, ref valFun);
                                IntPtr jsObj = JSApi.JSh_GetJsvalObject(ref JSMgr.vCall.rvalCallJS);
                                if (jsObj == IntPtr.Zero)
                                {
                                    Debug.LogError("Serialize error: call " + this.typeName + ".ctor return null");
                                    JSApi.JSh_SetJsvalUndefined(ref this.val);
                                }
                                else
                                {
                                    //IntPtr jsObj = JSApi.JSh_NewObjectAsClass(JSMgr.cx, jstypeObj, "ctor", null);// JSMgr.mjsFinalizer);
                                    for (var i = 0; i < lstChildren.Count; i++)
                                    {
                                        var child = lstChildren[i];
                                        JSApi.jsval mVal = child.CalcJSVal();
                                        JSApi.JSh_SetUCProperty(JSMgr.cx, jsObj, child.name, -1, ref mVal);
                                    }
                                    JSApi.JSh_SetJsvalObject(ref this.val, jsObj);
                                }
                            }
                        }*/
                    }
                    break;
                case SType.List:
                    {
                        // 这里要处理成 List 是 C# 的还是 JS 的？
                        // 如果是 C# 的要 使用 先创建一个 List 对象 然后JSDataExchangeMgr.setObject 
                        // 如果是 JS 的 会比较简单一点  参考上面的
                    }
                    break;
            }
            return this.id;
        }
    }
    /// <summary>
    /// 遍历 arrString 逐级处理序列化数据
    /// index: arrString 索引
    /// st: 当前父结点
    /// </summary>
    /// <param name="cx"></param>
    /// <param name="jsObj"></param>
    /// <param name="index"></param>
    /// <param name="st"></param>
    /// <returns></returns>
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
                // 这2个还带有类型
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
                            JSMgr.vCall.datax.setObject((int)JSApi.SetType.SaveAndTempTrace, this.arrObject[objIndex]);

                            var child = new SerializeStruct(SerializeStruct.SType.Unit, valName, st);
                            child.id = JSApi.getSaveID();
                            st.AddChild(child);
                        }
                        else if (eUnitType == UnitType.ST_MonoBehaviour)
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
    /// 在脚本的 Awake 时会调用这个函数来初始化序列化数据给JS。
    /// </summary>
    /// <param name="cx"></param>
    /// <param name="jsObj"></param>
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