using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/AwakeTest/AwakeA.javascript")]
public class AwakeA : MonoBehaviour {
	void Awake () {
        var b = GetComponent<AwakeB>();
        print("A call B: " + b.name);
	}
	
}
