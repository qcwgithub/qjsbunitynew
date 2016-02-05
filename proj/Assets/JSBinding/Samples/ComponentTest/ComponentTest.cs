using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;

[JsType(JsMode.Clr, "~/../Assets/StreamingAssets/JavaScript/SharpKitGeneratedFiles.javascript")]
public class ComponentTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // ���ԣ�
        // �������� GetComponent<����>()
        // ������������ TEnemy�����̳��� TEnemyBase
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

        gameObject.AddComponent<MentosKXT>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
