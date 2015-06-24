using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class UnityEngineManual
{
    public static bool Object_op_Inequality__Object__Object(JSVCall vc, int argc)
    {
        object arg0 = JSMgr.datax.getObject((int)JSApi.GetType.Arg);
        object arg1 = JSMgr.datax.getObject((int)JSApi.GetType.Arg);
        JSApi.setBooleanS((int)JSApi.SetType.Rval, (System.Boolean)(arg0 != arg1));
        return true;
    }

    public static bool Object_op_Equality__Object__Object(JSVCall vc, int argc)
    {
        object arg0 = JSMgr.datax.getObject((int)JSApi.GetType.Arg);
        object arg1 = JSMgr.datax.getObject((int)JSApi.GetType.Arg);
        JSApi.setBooleanS((int)JSApi.SetType.Rval, (System.Boolean)(arg0 == arg1));
        return true;
    }
};