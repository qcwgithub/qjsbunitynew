using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//using SharpKit.JavaScript;
//[JsType(Filename = "RotateObject5.js")]

public class Rotate : MonoBehaviour
{
    float speed = 0.1f;
    Transform mTrans;
    Vector3 vec = new Vector3(6, 99, 888);

    // Use this for initialization
    void Start()
    {
		vec.x = 3333;
        Debug.Log(vec.x);

        //var cam = Camera.main.gameObject.GetComponent<Camera>();
        //Debug.Log(cam.name);

        // test something in js clrlibrary
        var lst = new List<GameObject>();
        lst.Add(gameObject);
        lst.Add(Camera.main.gameObject);
        Debug.Log(lst[0].name);
        Debug.Log(lst[1].name);

//         Camera[] cams = gameObject.GetComponentsInChildren<Camera>(true);
// 
//         var arrObjs = GameObject.FindGameObjectsWithTag("Respawn");
//         for (var i = 0; i < arrObjs.Length; i++)
//         {
//             Debug.Log("GameObject.FindGameObjectsWithTag(\"Respawn\"): [" + i + "] = " + arrObjs[i].name);
//         }
    }

    // Update is called once per frame
    void Update()
    {
        if (mTrans == null)
        {
            mTrans = this.transform;
            vec = Vector3.forward;
        }
        mTrans.Rotate(vec * speed);
    }

    void OnGUI()
    {
        GUILayout.TextArea("Click the big cube!");
    }
}
