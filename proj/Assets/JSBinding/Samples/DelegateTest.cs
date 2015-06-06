using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/DelegateTest.javascript")]
public class DelegateTest : MonoBehaviour {

	// Use this for initialization
    List<int> lst;
    int mi = 0;
    void Awake()
    {
        lst = new List<int>();
        for (var i = 0; i < 10; i++)
        {
            lst.Add(i);
        }
    }
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            var f = lst.Find((v) => v == mi);
            Debug.Log("Found: " + f);

            mi++;
            if (mi >= 10) mi -= 10;
        }
	}
}
