using UnityEngine;
using System.Collections;

public class CallJSTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        JSMgr.vCall.CallJSFunctionName(0, "CreateJSBindingInfo");
        object obj = JSMgr.datax.getObject((int)JSApi.GetType.JSFunRet);
        CSRepresentedObject csObj = (CSRepresentedObject)obj;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
