using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/PerformanceTest1.javascript")]
public class PerformanceTest1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var g = 0;
            var f = 10;
            for (var i = 0; i < 10000000; i++)
            {
                g += 1;
                f += 10;
            }
            sw.Stop();
            Debug.Log("calc result: " + (f + g).ToString());
            Debug.Log("time: " + sw.ElapsedMilliseconds + " ms");

        }
	}
}
