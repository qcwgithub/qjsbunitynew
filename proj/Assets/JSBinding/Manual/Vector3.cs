using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using jsval = JSApi.jsval;

public partial class UnityEngineManual
{
    public static bool Vector3_GetHashCode(JSVCall vc, int start, int count)
    {
        int hash = ((Vector3)vc.csObj).GetHashCode();
        JSApi.setInt32((int)JSApi.SetType.Rval, hash);
        return true;
    }

    public static bool Vector3_MoveTowards__Vector3__Vector3__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        float a2 = JSApi.getSingle(JSApi.GetType.Arg);
        Vector3 ret = Vector3.MoveTowards(a0, a1, a2);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }

    public static bool Vector3_OrthoNormalize__Vector3__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        Vector3 a2 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        Vector3.OrthoNormalize(ref a0, ref a1, ref a2);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a0);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a1);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a2);
        return true;
    }

    public static bool Vector3_OrthoNormalize__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        Vector3.OrthoNormalize(ref a0, ref a1);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a0);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a1);
        return true;
    }

    public static bool Vector3_Project__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 ret = Vector3.Project(a0, a1);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }

#if UNITY_4_6
    public static bool Vector3_ProjectOnPlane__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 ret = Vector3.ProjectOnPlane(a0, a1);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }
#endif
    public static bool Vector3_Reflect__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 ret = Vector3.Reflect(a0, a1);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }

    public static bool Vector3_RotateTowards__Vector3__Vector3__Single__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        float a2 = JSApi.getSingle(JSApi.GetType.Arg);
        float a3 = JSApi.getSingle(JSApi.GetType.Arg);
        Vector3 ret = Vector3.RotateTowards(a0, a1, a2, a3);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }

    public static bool Vector3_Slerp__Vector3__Vector3__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        float a2 = JSApi.getSingle(JSApi.GetType.Arg);
        Vector3 ret = Vector3.Slerp(a0, a1, a2);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }


    public static bool Vector3_SmoothDamp__Vector3__Vector3__Vector3__Single__Single__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a2 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        float a3 = JSApi.getSingle(JSApi.GetType.Arg);
        float a4 = JSApi.getSingle(JSApi.GetType.Arg);
        float a5 = JSApi.getSingle(JSApi.GetType.Arg);
        Vector3 ret = Vector3.SmoothDamp(a0, a1, ref a2, a3, a4, a5);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a2);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }

    public static bool Vector3_SmoothDamp__Vector3__Vector3__Vector3__Single__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a2 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        float a3 = JSApi.getSingle(JSApi.GetType.Arg);
        float a4 = JSApi.getSingle(JSApi.GetType.Arg);
        Vector3 ret = Vector3.SmoothDamp(a0, a1, ref a2, a3, a4);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a2);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }

    public static bool Vector3_SmoothDamp__Vector3__Vector3__Vector3__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a1 = JSApi.getVector3S(JSApi.GetType.Arg);
        Vector3 a2 = JSApi.getVector3S(JSApi.GetType.ArgRef);
        float a3 = JSApi.getSingle(JSApi.GetType.Arg);
        Vector3 ret = Vector3.SmoothDamp(a0, a1, ref a2, a3);
        JSApi.setVector3(JSApi.SetType.UpdateArgRef, a2);
        JSApi.setVector3(JSApi.SetType.Rval, ret);
        return true;
    }
};