using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/ComponentTest/ComponentTest.javascript")]
public class ComponentTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // 测试！
        // 这里调用 GetComponent<基类>()
        // 本对象绑的是 TEnemy，他继承自 TEnemyBase
        TEnemyBase eb = GetComponent<TEnemyBase>();
        if (eb != null)
        {
            eb.enemyName = "BULL";
            Debug.Log("enemyName = " + eb.enemyName);
        }
        else
        {
            Debug.Log("GetComponent<TEnemyBase>() returns null!");
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
