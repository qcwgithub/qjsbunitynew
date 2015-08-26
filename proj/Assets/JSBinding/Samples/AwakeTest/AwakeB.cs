using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/AwakeTest/AwakeB.javascript")]
public class AwakeB : MonoBehaviour {

    void Awake()
    {
        var a = GetComponent<AwakeA>();
        print("B call A: " + a.name);
	}
	
}
