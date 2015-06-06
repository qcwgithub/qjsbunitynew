using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/TestCoroutine.javascript")]
public class TestCoroutine : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(CountDown());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    IEnumerator CountDown()
    {
        var V = 0;

        yield return new WaitForSeconds(1f);
        V++;
        Debug.Log(V);

        yield return new WaitForSeconds(1f);
        V++;
        Debug.Log(V);

        yield return new WaitForSeconds(1f);
        V++;
        Debug.Log(V);
    }  
}
