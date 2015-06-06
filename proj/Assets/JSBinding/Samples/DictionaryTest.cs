using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/DictionaryTest.javascript")]
public class DictionaryTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("qiucw", 28);
            dict.Add("helj", 27);

            int age;
            if (dict.TryGetValue("qiucw", out age))
            {
                Debug.Log("age: " + age.ToString());
            }
            else
            {
                Debug.Log("not found");
            }

        }
	}
}
