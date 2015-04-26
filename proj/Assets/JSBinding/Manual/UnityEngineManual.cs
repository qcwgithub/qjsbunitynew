using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnityEngineManual
{
    static string typeString;
    static System.Type type;

    static Dictionary<string, int> dict = new Dictionary<string, int>();
    public static void initManual() 
    {
        dict.Clear();

        dict.Add("GameObject_AddComponentT1", 0);
        dict.Add("Component_GetComponentT1", 0);
        dict.Add("GameObject_GetComponentT1", 0);
        dict.Add("Component_GetComponentsT1", 0);
        dict.Add("GameObject_GetComponentsT1", 0);
        dict.Add("Component_GetComponentInChildrenT1", 0);
        dict.Add("GameObject_GetComponentInChildrenT1", 0);
        dict.Add("Component_GetComponentsInChildrenT1", 0);
        dict.Add("GameObject_GetComponentsInChildrenT1", 0);
        dict.Add("Component_GetComponentsInChildrenT1__Boolean", 0);
        dict.Add("GameObject_GetComponentsInChildrenT1__Boolean", 0);
        dict.Add("Component_GetComponentInParentT1", 0);
        dict.Add("GameObject_GetComponentInParentT1", 0);
        dict.Add("Component_GetComponentsInParentT1", 0);
        dict.Add("GameObject_GetComponentsInParentT1", 0);
        dict.Add("Component_GetComponentsInParentT1__Boolean", 0);
        dict.Add("GameObject_GetComponentsInParentT1__Boolean", 0);
    }
    public static bool isManual(string functionName)
    {
        int b;
        if (dict.TryGetValue(functionName, out b)) {
            dict[functionName]++;
            return true; 
        }
        return false;
    }
    public static void afterUse()
    {
        bool problem = false;
        foreach (var v in dict)
        {
            if (v.Value == 0)
            { Debug.LogWarning("Manual C#: " + v.Key + " not used."); problem = true;  }
            else if (v.Value > 1)
            { Debug.LogWarning("Manual C#: " + v.Key + " used " + v.Value.ToString() + " times."); problem = true;  }
        }
        if (!problem) {
            Debug.Log("Manual C#: all used!");
        }
    }

    static void help_retComArr(JSVCall vc, Component[] arrRet)
    {
        var arrVal = new JSApi.jsval[arrRet.Length];
        for (int i = 0; i < arrRet.Length; i++)
        {
            vc.datax.setObject(JSDataExchangeMgr.eSetType.Jsval, arrRet[i]);
            arrVal[i] = vc.valTemp;
        }
        vc.datax.setArray(JSDataExchangeMgr.eSetType.SetRval, arrVal);
    }
    static void help_searchAndRetCom(JSVCall vc, JSComponent_SharpKit[] jsComs, string typeString)
    {
        foreach (var jsCom in jsComs)
        {
            if (jsCom.jsScriptName == typeString)
            {
                // vc.datax.setObject(JSDataExchangeMgr.eSetType.SetRval, jsCom);
                JSApi.JSh_SetJsvalObject(ref vc.valReturn, jsCom.jsObj);
                JSApi.JSh_SetRvalJSVAL(JSMgr.cx, vc.vp, ref vc.valReturn);
                break;
            }
        }
    }
    static void help_searchAndRetComs(JSVCall vc, JSComponent_SharpKit[] com, string typeString)
    {
        List<JSComponent_SharpKit> lst = new List<JSComponent_SharpKit>();
        foreach (var c in com)
        {
            if (c.jsScriptName == typeString)
            {
                lst.Add(c);
            }
        }
        var arrVal = new JSApi.jsval[lst.Count];
        for (int i = 0; i < lst.Count; i++)
        {
            JSApi.JSh_SetJsvalObject(ref arrVal[i], lst[i].jsObj);
        }
        vc.datax.setArray(JSDataExchangeMgr.eSetType.SetRval, arrVal);
    }

    static bool isCSType(System.Type type)
    {
        if (type == null)
        {
            return false;
        }
        if (type.Namespace != null && type.Namespace.IndexOf("UnityEngine") >= 0)
        {
            return true;
        }
        // 当 源文件还未从工程中删除时这个有用
        if (type.GetCustomAttributes(typeof(SharpKit.JavaScript.JsTypeAttribute), false).Length > 0)
        {
            return false;
        }
        return !typeof(MonoBehaviour).IsAssignableFrom(type);
    }

/// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //    Game Object
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

    static GameObject go = null;
    static GameObject goFromComponent = null;
    static void help_getGoAndType(JSVCall vc)
    {
        go = goFromComponent;
        if (go == null)
        {
            go = (UnityEngine.GameObject)vc.csObj;
        }
        typeString = vc.datax.getString(JSDataExchangeMgr.eGetType.GetARGV);
        type = JSDataExchangeMgr.GetTypeByName(typeString);
    }

    static void help_getComponentGo(JSVCall vc)
    {
        goFromComponent = ((UnityEngine.Component)vc.csObj).gameObject;
    }

    /* 
     * GameObject.AddComponent<T>()
     */
    public static bool GameObject_AddComponentT1(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);

        if (isCSType(type))
        {
            Component com = go.AddComponent(type);
            vc.datax.setObject(JSDataExchangeMgr.eSetType.SetRval, com);
        }
        else
        {
            JSComponent_SharpKit jsComp = go.AddComponent<JSComponent_SharpKit>();
            jsComp.jsScriptName = typeString;
            jsComp.Awake();

            JSApi.JSh_SetRvalObject(vc.cx, vc.vp, jsComp.jsObj);
        }
        return true;
    }
    /*
     * GameObject.GetComponent<T>()
     */
    public static bool Component_GetComponentT1(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentT1(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentT1(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);

        if (isCSType(type))
        {
            Component com = go.GetComponent(type);
            vc.datax.setObject(JSDataExchangeMgr.eSetType.SetRval, com);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponents<JSComponent_SharpKit>();
            help_searchAndRetCom(vc, com, typeString);
        }
        return true;
    }
    /*
     * GameObject.GetComponents<T>()
     */
    public static bool Component_GetComponentsT1(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsT1(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsT1(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);

        if (isCSType(type))
        {
            Component[] arrRet = go.GetComponents(type);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponents<JSComponent_SharpKit>();
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GameObject.GetComponentInChildren<T>()
     */
    public static bool Component_GetComponentInChildrenT1(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentInChildrenT1(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentInChildrenT1(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);

        if (isCSType(type))
        {
            Component com = go.GetComponentInChildren(type);
            vc.datax.setObject(JSDataExchangeMgr.eSetType.SetRval, com);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponentsInChildren<JSComponent_SharpKit>();
            help_searchAndRetCom(vc, com, typeString);
        }
        return true;
    }
    /*
     * GetComponentsInChildren<T>()
     */
    public static bool Component_GetComponentsInChildrenT1(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInChildrenT1(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInChildrenT1(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);

        if (isCSType(type))
        {
            Component[] arrRet = go.GetComponentsInChildren(type);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponentsInChildren<JSComponent_SharpKit>();
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GetComponentsInChildren<T>(bool includeInactive)
     */
    public static bool Component_GetComponentsInChildrenT1__Boolean(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInChildrenT1__Boolean(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInChildrenT1__Boolean(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);
        bool includeInactive = vc.datax.getBoolean(JSDataExchangeMgr.eGetType.GetARGV);

        if (isCSType(type))
        {
            Component[] arrRet = go.GetComponentsInChildren(type, includeInactive);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponentsInChildren<JSComponent_SharpKit>(includeInactive);
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GameObject.GetComponentInParent<T>()
     */
    public static bool Component_GetComponentInParentT1(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentInParentT1(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentInParentT1(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);

        if (isCSType(type))
        {
            Component com = go.GetComponentInParent(type);
            vc.datax.setObject(JSDataExchangeMgr.eSetType.SetRval, com);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponentsInParent<JSComponent_SharpKit>();
            help_searchAndRetCom(vc, com, typeString);
        }
        return true;
    }
    /*
    * GetComponentsInParent<T>()
    */
    public static bool Component_GetComponentsInParentT1(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInParentT1(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInParentT1(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);

        if (isCSType(type))
        {
            Component[] arrRet = go.GetComponentsInParent(type);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponentsInParent<JSComponent_SharpKit>();
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GetComponentsInParent<T>(bool includeInactive)
     */
    public static bool Component_GetComponentsInParentT1__Boolean(JSVCall vc, int start, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInParentT1__Boolean(vc, start, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInParentT1__Boolean(JSVCall vc, int start, int count)
    {
        help_getGoAndType(vc);
        bool includeInactive = vc.datax.getBoolean(JSDataExchangeMgr.eGetType.GetARGV);

        if (isCSType(type))
        {
            Component[] arrRet = go.GetComponentsInParent(type, includeInactive);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent_SharpKit[] com = go.GetComponentsInParent<JSComponent_SharpKit>(includeInactive);
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
}
