using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/MiscTest/MiscTest.javascript")]
public class MiscTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PerTest.test123(1.5, 1.3);
        PerTest.testString("abc", "ÇïÌì");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
