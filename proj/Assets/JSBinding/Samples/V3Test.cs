using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/V3Test.javascript")]
public class V3Test : MonoBehaviour 
{
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    float elapsed = 0;
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > 1f)
        {
            elapsed = 0f;
            Vector3 v = new Vector3(2, 3, 6);
            //v.x = 5;
            Debug.Log(v.x);
        }
	}
}
