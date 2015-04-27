using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UnityEngineManual
{
    public static bool Vector3_GetHashCode(JSVCall vc, int start, int count)
    {
        Vector3 v = vc.datax.getVector3(vc.jsObj);
        vc.datax.setInt32(JSDataExchangeMgr.eSetType.SetRval, v.GetHashCode());
        return true;
    }

    public static bool Vector3_MoveTowards__Vector3__Vector3__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        float a2 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.MoveTowards(a0, a1, a2);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }

    public static bool Vector3_OrthoNormalize__Vector3__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        Vector3 a2 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        Vector3.OrthoNormalize(ref a0, ref a1, ref a2);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a0);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a1);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a2);
        return true;
    }

    public static bool Vector3_OrthoNormalize__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        Vector3.OrthoNormalize(ref a0, ref a1);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a0);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a1);
        return true;
    }

    public static bool Vector3_Project__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.Project(a0, a1);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }

    public static bool Vector3_ProjectOnPlane__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.ProjectOnPlane(a0, a1);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }
    public static bool Vector3_Reflect__Vector3__Vector3(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.Reflect(a0, a1);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }

    public static bool Vector3_RotateTowards__Vector3__Vector3__Single__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        float a2 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        float a3 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.RotateTowards(a0, a1, a2, a3);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }

    public static bool Vector3_Slerp__Vector3__Vector3__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        float a2 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.Slerp(a0, a1, a2);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }


    public static bool Vector3_SmoothDamp__Vector3__Vector3__Vector3__Single__Single__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a2 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        float a3 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        float a4 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        float a5 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.SmoothDamp(a0, a1, ref a2, a3, a4, a5);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a2);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }

    public static bool Vector3_SmoothDamp__Vector3__Vector3__Vector3__Single__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a2 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        float a3 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        float a4 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.SmoothDamp(a0, a1, ref a2, a3, a4);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a2);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }

    public static bool Vector3_SmoothDamp__Vector3__Vector3__Vector3__Single(JSVCall vc, int start, int count)
    {
        Vector3 a0 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a1 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 a2 = vc.datax.getVector3(JSDataExchangeMgr.eGetType.GetARGVRefOut);
        float a3 = vc.datax.getSingle(JSDataExchangeMgr.eGetType.GetARGV);
        Vector3 ret = Vector3.SmoothDamp(a0, a1, ref a2, a3);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.UpdateARGVRefOut, a2);
        vc.datax.setVector3(JSDataExchangeMgr.eSetType.SetRval, ret);
        return true;
    }
};