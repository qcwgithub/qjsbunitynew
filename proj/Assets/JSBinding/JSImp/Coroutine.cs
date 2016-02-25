using UnityEngine;
using System.Collections;
using System;
using SharpKit.JavaScript;

namespace jsimp
{
[JsType(JsMode.Clr, "~/Assets/StreamingAssets/JavaScript/SharpKitGeneratedFiles.javascript")]
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