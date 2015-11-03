using UnityEngine;
using System.Collections;
using System;
using SharpKit.JavaScript;

namespace jsimp
{
[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/Coroutine.javascript")]
    public class Coroutine
	{
		[JsMethod(Code = @"var elapsed = UnityEngine.Time.get_deltaTime();
            mb.$UpdateAllCoroutines(elapsed);
            mb.$UpdateAllInvokes(elapsed);")]
        public static void UpdateCoroutineAndInvoke(MonoBehaviour mb)
        {
            // NOTHING TO DO
        }
    }
}