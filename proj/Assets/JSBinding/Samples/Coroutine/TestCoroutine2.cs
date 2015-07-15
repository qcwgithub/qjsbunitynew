using SharpKit.JavaScript;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Coroutine/YieldYest.javascript")]
public class YieldYest
{

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Coroutine/YieldItem.javascript")]
    public class YieldItem
    {
        public int _item = 0;
        public int OnGet
        {
            get
            {
                Debug.Log("Build");
                if (_item <= 0)
                {
                    _item = 10;
                }
                return _item;
            }
        }
    }

    public YieldItem item { get; set; }
    public IEnumerator YiedlTest()
    {

        int x = 0;
        item = new YieldItem();
        Debug.Log("1");
        yield return x++;
        Debug.Log("2");
        yield return item.OnGet;
    }

    public void Run()
    {
        //
        // Can not use IEnumerator to iterate JavaScript coroutine
        // jsimp.Coroutine.Iterator is implemented in JavaScript
        //

        // (1)
        // foreach (var xitem in YiedlTest())

        // (2)
        object xitem;
        jsimp.Coroutine.Iterator it = new jsimp.Coroutine.Iterator(YiedlTest());
        while ((xitem = it.MoveNext()) != null)


        {
            item._item += 100;
            Debug.Log(xitem);
        }
    }
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Coroutine/TestCoroutine2.javascript")]
public class TestCoroutine2 : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        new YieldYest().Run();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
