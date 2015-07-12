using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Coroutine/TestCoroutine.javascript")]
public class TestCoroutine : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(DoTest());
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
    void LateUpdate()
    {
        jsimp.Coroutine.UpdateMonoBehaviourCoroutine(this);
    }
    IEnumerator WaitForCangJingKong()
    {
        yield return new WaitForSeconds(2f);
    }
    IEnumerator DoTest()
    {
        // test null
        Debug.Log(1);
        yield return null;

        // test WaitForSeconds
        Debug.Log(2);
        yield return new WaitForSeconds(1f);

        // test WWW
        WWW www = new WWW("file://" + Application.dataPath + "/JSBinding/Samples/Coroutine/CoroutineReadme.txt");
        yield return www;
        Debug.Log("Text from WWW: " + www.text);

        // test another coroutine
        yield return StartCoroutine(WaitForCangJingKong());
        Debug.Log("Wait for CangJingKong finished!");
    }  
}
