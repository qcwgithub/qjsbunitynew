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
    int value = 100;  
    IEnumerator CountDown()
    {
        float increment = 0f;
        while (true)
        {
            if (Input.GetKey(KeyCode.Q))
                break;
            increment += Time.deltaTime;
            value -= 1;
            Debug.Log(value);
            //This coroutine returns a yield instruction  
            yield return new WaitForSeconds(1f);
        }
    }  
}
