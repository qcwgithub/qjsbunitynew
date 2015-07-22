using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System;

namespace jsimp
{

[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/Coroutine.javascript")]
    public class Coroutine
    {
        public static void UpdateCoroutineAndInvoke(MonoBehaviour mb)
        {
            // NOTHING TO DO
        }
[JsType(JsMode.Clr,"../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/JSImp/Iterator.javascript")]
        public class Iterator
        {
            IEnumerator ie;
            public Iterator(IEnumerator ie)
            {
                this.ie = ie;
            }
            public object MoveNext()
            {
                if (ie.MoveNext())
                    return ie.Current;
                return null;
            }
        }
    }
}