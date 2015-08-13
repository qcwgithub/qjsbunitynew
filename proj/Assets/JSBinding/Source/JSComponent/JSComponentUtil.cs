using UnityEngine;
using System.Collections;

public class JSComponentUtil : JSComponent
{
    int idIsInheritanceRel;
    protected override void initMemberFunction()
    {
        base.initMemberFunction();
        idIsInheritanceRel = JSApi.getObjFunction(jsObjID, "IsInheritanceRel");
    }
    // 判断2个类是不是继承关系！
    public bool IsInheritanceRel(string baseClassName, string subClassName)
    {
        bool ret = false;
        if (JSMgr.vCall.CallJSFunctionValue(jsObjID, idIsInheritanceRel, baseClassName, subClassName))
        {
            ret = (System.Boolean)JSApi.getBooleanS((int)JSApi.GetType.JSFunRet);
        }
        return ret;
    }
}
