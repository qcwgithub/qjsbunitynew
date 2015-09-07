using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/CallJSTest/CallJSTest.javascript")]
public class CallJSTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //1---------------------------------------------------------------
        // 调用全局函数，使用 id 0
        JSMgr.vCall.CallJSFunctionName(0, "CreateJSBindingInfo");

        object obj = JSMgr.datax.getObject((int)JSApi.GetType.JSFunRet);
        CSRepresentedObject csObj = (CSRepresentedObject)obj;

        PrintJSBindingInfo(csObj.jsObjID);


        //2---------------------------------------------------------------

        int id = JSApi.newJSClassObject("JSBindingInfoFun");
        JSApi.setTraceS(id, true);

        PrintJSBindingInfo(id);
        JSApi.setTraceS(id, false);
	}

    void PrintJSBindingInfo(int objID)
    {
        // 获得字符串属性
        JSApi.getProperty(objID, "Version");
        string s = JSApi.getStringS((int)JSApi.GetType.SaveAndRemove);
        print(s);

        // 获得整数属性
        JSApi.getProperty(objID, "QQGroup");
        int i = JSApi.getInt32((int)JSApi.GetType.SaveAndRemove);
        print(i);

        // 调用这个obj的函数
        JSMgr.vCall.CallJSFunctionName(objID, "getDocumentUrl");
        s = JSApi.getStringS((int)JSApi.GetType.JSFunRet);
        print(s);
    }

	void Update () {
	
	}
}
