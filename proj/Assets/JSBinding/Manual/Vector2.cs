using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UnityEngineManual
{
    public static bool IsJSObjVector2(IntPtr jsObj)
    {
        JSApi.jsval val = new JSApi.jsval();
        JSApi.JSh_GetUCProperty(JSMgr.cx, jsObj, "_fullname", -1, ref val);
        if (JSApi.JSh_JsvalIsString(ref val))
        {
            string s = JSApi.JSh_GetJsvalStringS(JSMgr.cx, ref val);
            return s == "UnityEngine.Vector2";
        }
        return false;
    }

    public static bool Vector2_GetHashCode(JSVCall vc, int start, int count)
    {
        Vector2 v = vc.datax.getVector2(vc.jsObj);
        vc.datax.setInt32(JSDataExchangeMgr.eSetType.SetRval, v.GetHashCode());
        return true;
    }

    public static bool Vector2_MoveTowards__Vector2__Vector2__Single(JSVCall vc, int start, int count)
    {
        UnityEngine.Vector2 arg0 = (UnityEngine.Vector2)vc.datax.getVector2(JSDataExchangeMgr.eGetType.GetARGV);
        UnityEngine.Vector2 arg1 = (UnityEngine.Vector2)vc.datax.getVector2(JSDataExchangeMgr.eGetType.GetARGV);
        System.Single arg2 = (System.Single)vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        JSMgr.vCall.datax.setVector2(JSDataExchangeMgr.eSetType.SetRval, UnityEngine.Vector2.MoveTowards(arg0, arg1, arg2));
        return true;
    }

};