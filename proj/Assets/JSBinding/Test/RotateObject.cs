using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour 
{

    Transform mTrans;
    Vector3 rotateVar;
    float fAccum = 0;

    void Awake()
    {
        // example to test value type
        // also demostrate to call function with object parameter
        // output: 100,66666
        var v1 = Vector3.one;
        var v2 = Vector3.one;
        v1.x = 100;
        v2.x = 66666;
        Debug.Log("Vector1.x = " + v1.x.ToString());
        Debug.Log("Vector2.x = " + v2.x.ToString());

        //
        var arrObjs = GameObject.FindGameObjectsWithTag("Respawn");
        for (var i = 0; i < arrObjs.Length; i++) {
            Debug.Log("GameObject.FindGameObjectsWithTag(\"Respawn\"): [" + i + "] = " + arrObjs[i].name);
        }


        // 
        //var myAdd1 = MyAdd(199, 200);
        //Debug.Log("Test CS.require: 199 + 200 = " + myAdd1.ToString());
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    // here
        // 'this' means a GameObject

        // rotate gameobject
        if (mTrans == null)
        {
            mTrans = transform;
            rotateVar = new Vector3(0.5f, 0.5f, 0f);
        }
        mTrans.Rotate(rotateVar);

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // call function with out struct
            // must call new to create object at first
            var hit = new RaycastHit();
            Physics.Raycast(ray, out hit);
            var t = hit.transform;
        
            //if (t !== undefined)
            //{
            //    Debug.Log('Hit ' + t.name);
            //}
            if (t != null && t == mTrans)
            {
                Debug.Log("Hit " + t.name);

                var prefab = GameObject.Find("CubeInit");
                if (prefab == null)
                {
                    Debug.Log("prefab is undefined!");
                    return;
                }
                GameObject newGo = (GameObject)Object.Instantiate(prefab);
                newGo.SetActive(true);
                var tn = newGo.transform;
                tn.localScale = new Vector3(0.3f,0.3f,0.3f);
                tn.position = new Vector3(Random.Range(-2.1f, 2.1f), Random.Range(-1.1f, 1.1f), -7.24f);
                //CS.AddJSComponent(newGo, "RotateObject2");
                newGo.AddComponent("RotateObject2");
            }
        }

        // destroy this GameObject after 10 seconds
        fAccum += Time.deltaTime;
        if (fAccum > 10)
        {
            // JavaScript has 'Object' as key word
            // so UnityEngine.Object is renamed to 'UnityObject'
            //UnityObject.Destroy(this);
        }
	}
    void Destroy()
    {
        Debug.Log("js Destroy Called!");
    }

    void OnGUI()
    {
        GUILayout.TextArea("Click the big cube!");
    }
}

