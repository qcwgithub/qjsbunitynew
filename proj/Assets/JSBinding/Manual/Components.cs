using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UnityEngineManual
{
    static void help_retComArr(JSVCall vc, Component[] arrRet)
    {
        int Count = arrRet.Length;
        for (int i = 0; i < arrRet.Length; i++)
        {
            vc.datax.setObject((int)JSApi.SetType.SaveAndTempTrace, arrRet[i]);
            JSApi.moveSaveID2Arr(i);
        }
        JSApi.setArrayS((int)JSApi.SetType.Rval, Count, true);
    }
    static void help_searchAndRetCom(JSVCall vc, JSComponent[] jsComs, string typeString)
    {
        foreach (var jsCom in jsComs)
        {
            if (jsCom.jsScriptName == typeString)
            {
                JSApi.setObject((int)JSApi.SetType.Rval, jsCom.jsObjID);
                break;
            }
        }
    }
    static void help_searchAndRetComs(JSVCall vc, JSComponent[] com, string typeString)
    {
        List<JSComponent> lst = new List<JSComponent>();
        foreach (var c in com)
        {
            if (c.jsScriptName == typeString)
            {
                lst.Add(c);
            }
        }
        for (int i = 0; i < lst.Count; i++)
        {
            JSApi.setObject((int)JSApi.SetType.SaveAndTempTrace, lst[i].jsObjID);
            JSApi.moveSaveID2Arr(i);
        }
        JSApi.setArrayS((int)JSApi.SetType.Rval, lst.Count, true);

//         var arrVal = new JSApi.jsval[lst.Count];
//         for (int i = 0; i < lst.Count; i++)
//         {
//             JSApi.JSh_SetJsvalObject(ref arrVal[i], lst[i].jsObj);
//         }
//         vc.datax.setArray(JSDataExchangeMgr.eSetType.SetRval, arrVal);
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
        typeString = JSApi.getStringS((int)JSApi.GetType.Arg);
        type = JSDataExchangeMgr.GetTypeByName(typeString);
    }

    static void help_getComponentGo(JSVCall vc)
    {
        goFromComponent = ((UnityEngine.Component)vc.csObj).gameObject;
    }

    /* 
     * GameObject.AddComponent<T>()
     */
    public static bool GameObject_AddComponentT1(JSVCall vc, int count)
    {
        help_getGoAndType(vc);

        if (isCSMonoBehaviour(type))
        {
            Component com = go.AddComponent(type);
            vc.datax.setObject((int)JSApi.SetType.Rval, com);
        }
        else
        {
            JSComponent jsComp = go.AddComponent<JSComponent>();
            jsComp.jsScriptName = typeString;
            jsComp.Awake();

            //JSApi.JSh_SetRvalObject(vc.cx, vc.vp, jsComp.jsObj);
            JSApi.setObject((int)JSApi.SetType.Rval, jsComp.jsObjID);
        }
        return true;
    }
    /*
     * GameObject.GetComponent<T>()
     */
    public static bool Component_GetComponentT1(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentT1(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentT1(JSVCall vc, int count)
    {
        help_getGoAndType(vc);

        if (isCSMonoBehaviour(type))
        {
            Component com = go.GetComponent(type);
            vc.datax.setObject((int)JSApi.SetType.Rval, com);
        }
        else
        {
            JSComponent[] com = go.GetComponents<JSComponent>();
            help_searchAndRetCom(vc, com, typeString);
        }
        return true;
    }
    /*
     * GameObject.GetComponents<T>()
     */
    public static bool Component_GetComponentsT1(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsT1(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsT1(JSVCall vc, int count)
    {
        help_getGoAndType(vc);

        if (isCSMonoBehaviour(type))
        {
            Component[] arrRet = go.GetComponents(type);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent[] com = go.GetComponents<JSComponent>();
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GameObject.GetComponentInChildren<T>()
     */
    public static bool Component_GetComponentInChildrenT1(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentInChildrenT1(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentInChildrenT1(JSVCall vc, int count)
    {
        help_getGoAndType(vc);

        if (isCSMonoBehaviour(type))
        {
            Component com = go.GetComponentInChildren(type);
            vc.datax.setObject((int)JSApi.SetType.Rval, com);
        }
        else
        {
            JSComponent[] com = go.GetComponentsInChildren<JSComponent>();
            help_searchAndRetCom(vc, com, typeString);
        }
        return true;
    }
    /*
     * GetComponentsInChildren<T>()
     */
    public static bool Component_GetComponentsInChildrenT1(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInChildrenT1(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInChildrenT1(JSVCall vc, int count)
    {
        help_getGoAndType(vc);

        if (isCSMonoBehaviour(type))
        {
            Component[] arrRet = go.GetComponentsInChildren(type);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent[] com = go.GetComponentsInChildren<JSComponent>();
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GetComponentsInChildren<T>(bool includeInactive)
     */
    public static bool Component_GetComponentsInChildrenT1__Boolean(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInChildrenT1__Boolean(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInChildrenT1__Boolean(JSVCall vc, int count)
    {
        help_getGoAndType(vc);
        // TODO check
//        bool includeInactive = vc.datax.getBoolean(JSDataExchangeMgr.eGetType.GetARGV);
        bool includeInactive = JSApi.getBooleanS((int)JSApi.GetType.Arg);

        if (isCSMonoBehaviour(type))
        {
            Component[] arrRet = go.GetComponentsInChildren(type, includeInactive);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent[] com = go.GetComponentsInChildren<JSComponent>(includeInactive);
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GameObject.GetComponentInParent<T>()
     */
    public static bool Component_GetComponentInParentT1(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentInParentT1(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentInParentT1(JSVCall vc, int count)
    {
        help_getGoAndType(vc);

        if (isCSMonoBehaviour(type))
        {
            Component com = go.GetComponentInParent(type);
            vc.datax.setObject((int)JSApi.SetType.Rval, com);
        }
        else
        {
            JSComponent[] com = go.GetComponentsInParent<JSComponent>();
            help_searchAndRetCom(vc, com, typeString);
        }
        return true;
    }
    /*
    * GetComponentsInParent<T>()
    */
    public static bool Component_GetComponentsInParentT1(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInParentT1(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInParentT1(JSVCall vc, int count)
    {
        help_getGoAndType(vc);

        if (isCSMonoBehaviour(type))
        {
            Component[] arrRet = go.GetComponentsInParent(type);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent[] com = go.GetComponentsInParent<JSComponent>();
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
    /*
     * GetComponentsInParent<T>(bool includeInactive)
     */
    public static bool Component_GetComponentsInParentT1__Boolean(JSVCall vc, int count)
    {
        help_getComponentGo(vc);
        GameObject_GetComponentsInParentT1__Boolean(vc, count);
        goFromComponent = null;
        return true;
    }
    public static bool GameObject_GetComponentsInParentT1__Boolean(JSVCall vc, int count)
    {
        help_getGoAndType(vc);
        // TODO check
//        bool includeInactive = vc.datax.getBoolean(JSDataExchangeMgr.eGetType.GetARGV);
        bool includeInactive = JSApi.getBooleanS((int)JSApi.GetType.Arg);

        if (isCSMonoBehaviour(type))
        {
            Component[] arrRet = go.GetComponentsInParent(type, includeInactive);
            help_retComArr(vc, arrRet);
        }
        else
        {
            JSComponent[] com = go.GetComponentsInParent<JSComponent>(includeInactive);
            help_searchAndRetComs(vc, com, typeString);
        }
        return true;
    }
}