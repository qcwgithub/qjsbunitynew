using SharpKit.JavaScript;
using UnityEngine;
using DaikonForge.Tween;
using DaikonForge.Tween.Interpolation;
using System.Collections;
using System.Collections.Generic;
[JsType(JsMode.Clr,"../StreamingAssets/JavaScript/Test1.javascript")]
public class Test1 : MonoBehaviour {
    public hellons.Test2 test2;

	// Use this for initialization
	void Start () {

        Debug.Log("测试tween类！");
       /* this.transform
          .TweenPosition()
          .SetEndValue(new Vector3(100, 100, 0))
          .SetDuration(10)
          .Play();*/


        List<string> lst = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}