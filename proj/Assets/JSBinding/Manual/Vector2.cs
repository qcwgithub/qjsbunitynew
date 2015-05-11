using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using jsval = JSApi.jsval;

public partial class UnityEngineManual
{
    public static bool Vector2_GetHashCode(JSVCall vc, int start, int count)
    {
        int hash = ((Vector2)vc.csObj).GetHashCode();
        JSApi.setInt32(JSApi.SetType.Rval, hash);
        return true;
    }

    public static bool Vector2_MoveTowards__Vector2__Vector2__Single(JSVCall vc, int start, int count)
    {
        UnityEngine.Vector2 arg0 = JSApi.getVector2S(JSApi.GetType.Arg);
        UnityEngine.Vector2 arg1 = JSApi.getVector2S(JSApi.GetType.Arg);
        System.Single arg2 = (System.Single)JSApi.getSingle(JSApi.GetType.Arg);
        JSApi.setVector2(JSApi.SetType.Rval, UnityEngine.Vector2.MoveTowards(arg0, arg1, arg2));
        return true;
    }
};