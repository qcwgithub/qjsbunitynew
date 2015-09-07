using UnityEngine;
using System.Collections;

public class CallJSTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // 调用全局函数，使用 id 0
        JSMgr.vCall.CallJSFunctionName(0, "CreateJSBindingInfo");

        // 从返回值获取对象
        object obj = JSMgr.datax.getObject((int)JSApi.GetType.JSFunRet);
        CSRepresentedObject csObj = (CSRepresentedObject)obj;

        // 获得字符串属性
        JSApi.getProperty(csObj.jsObjID, "Version");
        string s = JSApi.getStringS((int)JSApi.GetType.SaveAndRemove);
        print(s);

        // 获得整数属性
        JSApi.getProperty(csObj.jsObjID, "QQGroup");
        int i = JSApi.getInt32((int)JSApi.GetType.SaveAndRemove);
        print(i);

        // 调用这个obj的函数
        JSMgr.vCall.CallJSFunctionName(csObj.jsObjID, "getDocumentUrl");
        s = JSApi.getStringS((int)JSApi.GetType.JSFunRet);
        print(s);


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
