if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestCoroutine = {
    fullname: "TestCoroutine",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.StartCoroutine$$IEnumerator(this.CountDown());
        },
        Update: function (){
        },
        CountDown: function (){
            var $yield = [];
            var V = 0;
            $yield.push(new UnityEngine.WaitForSeconds.ctor(1));
            V++;
            UnityEngine.Debug.Log$$Object(V);
            $yield.push(new UnityEngine.WaitForSeconds.ctor(1));
            V++;
            UnityEngine.Debug.Log$$Object(V);
            $yield.push(new UnityEngine.WaitForSeconds.ctor(1));
            V++;
            UnityEngine.Debug.Log$$Object(V);
            return $yield;
        }
    }
};
JsTypes.push(TestCoroutine);

