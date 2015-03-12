/*
* this is a simple callback js
* functions (except delegate function) are called with 'this' set to a GameObject
*/

// Test for CS.require
// 2nd param means 'this' object for the script. default value is global
CS.require("MyAdd", this)

// these variables belongs to 'this' object
// 
var mTrans = undefined
var rotateVar = undefined
var fAccum = 0
function Awake()
{
    // example to test value type
    // also demostrate to call function with object parameter
    // output: 100,66666
    var v1 = Vector3.one
    var v2 = Vector3.one
    v1.x = 100
    v2.x = 66666
    Debug.Log("Vector1.x = " + v1.x.toString());
    Debug.Log("Vector2.x = " + v2.x.toString());

    //
    var arrObjs = GameObject.FindGameObjectsWithTag('Respawn')
    for (var i = 0; i < arrObjs.length; i++) {
        Debug.Log("GameObject.FindGameObjectsWithTag('Respawn'): [" + i + "] = " + arrObjs[i].name);
    }


    // 
    var myAdd1 = MyAdd(199, 200);
    Debug.Log("Test CS.require: 199 + 200 = " + myAdd1.toString());
}
function Start()
{
    // expample to call out parameter by wrapping it
    // expected output is 899
    // 'CS' is defined in c#
    var k = new Kekoukele();
    var v = {Value: 1}
    k.getValue(v);
    Debug.Log("ref/out changed: 1->" + v.Value.toString())

    //var arr = k.getArr();
    //for (var i = 0; i < arr.length; i++) {
    //    Debug.Log("Array from C#: arr[" + i + "] = " + arr[i].toString());
    //}

    // not supported now.
    // k.inputIntArr([999,888,777,666,555])
}

function Update()
{
    // here
    // 'this' means a GameObject

    // rotate gameobject
    if (mTrans === undefined)
    {
        mTrans = transform;
        rotateVar = new Vector3(0.5, 0.5, 0);
    }
    mTrans.Rotate(rotateVar);

    if (Input.GetMouseButtonDown(1) === true)
    {
        // test SharpKit jsclr.js
        var lst = new System.Collections.Generic.List$1.ctor(System.Int32.ctor);
        lst.Add(5);
        lst.Add(8);
        var arr = lst.ToArray();
        for (var i = 0; i < arr.length; i++){
            Debug.Log("Arr[" + i.toString() + "] = " + arr[i].toString());
        }
    }

    if (Input.GetMouseButtonDown(0) === true)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // call function with out struct
        // must call new to create object at first
        var hit = new RaycastHit();

		var _out = {Value: hit}
        Physics.Raycast(ray, _out);
		hit = _out.Value;

        var t = hit.transform;
        
        //if (t !== undefined)
        //{
        //    Debug.Log('Hit ' + t.name);
        //}
        if (t !== undefined && t.Equals(mTrans))
        {
            Debug.Log('Hit ' + t.name);

            var prefab = GameObject.Find('CubeInit');
            if (prefab === undefined)
            {
                Debug.Log('prefab is undefined!');
                return;
            }
            var newGo = UnityObject.Instantiate(prefab);
            newGo.SetActive(true);
            var tn = newGo.transform;
            tn.localScale = new Vector3(0.3,0.3,0.3);
            tn.position = new Vector3(Random.Range(-2.1,2.1), Random.Range(-1.1,1.1), -7.24)
            CS.AddJSComponent(newGo, "RotateObject2");
        }
    }

    // destroy this GameObject after 10 seconds
    fAccum += Time.deltaTime
    if (fAccum > 10)
    {
        // JavaScript has 'Object' as key word
        // so UnityEngine.Object is renamed to 'UnityObject'
        //UnityObject.Destroy(this);
    }
}

function Destroy()
{
	return;
    Debug.Log('js Destroy Called!')
}

function OnGUI()
{
	return;
    GUILayout.TextArea('1) Left click the big cube to test basic function!', null);
    GUILayout.TextArea('2) Right click anywhere to test SharpKit\'s jsclr!', null);
}