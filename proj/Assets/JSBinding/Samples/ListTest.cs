using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// test System.Collections.Generic.List

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/ListTest.javascript")]
public class ListTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            List<int> lst = new List<int>();
            lst.Add(6);
            lst.Add(95);
            foreach (var v in lst)
            {
                Debug.Log(v);
            }
            int vFind = lst.Find((val) => (val == 6));
            Debug.Log("vFind = " + vFind);

            var lstS = lst.ConvertAll<string>((v) => "s: " + v);
            foreach (var v in lstS)
            {
                Debug.Log(v);
            }
            Debug.Log(lstS[0]);
            Debug.Log(lstS[1]);
        }
	}
}
