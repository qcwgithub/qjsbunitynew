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
            this.StartCoroutine$$IEnumerator(this.DoTest());
        },
        Update: function (){
        },
        LateUpdate: function (){
            jsimp.Coroutine.UpdateMonoBehaviourCoroutine(this);
        },
        WaitForCangJingKong: function (){
            var $yield = [];
            $yield.push(new UnityEngine.WaitForSeconds.ctor(2));
            return $yield;
        },
        DoTest: function (){
            var $yield = [];
            UnityEngine.Debug.Log$$Object(1);
            $yield.push(null);
            UnityEngine.Debug.Log$$Object(2);
            $yield.push(new UnityEngine.WaitForSeconds.ctor(1));
            var www = new UnityEngine.WWW.ctor$$String("file://" + UnityEngine.Application.get_dataPath() + "/JSBinding/Samples/Coroutine/CoroutineReadme.txt");
            $yield.push(www);
            UnityEngine.Debug.Log$$Object("Text from WWW: " + www.get_text());
            $yield.push(this.StartCoroutine$$IEnumerator(this.WaitForCangJingKong()));
            UnityEngine.Debug.Log$$Object("Wait for CangJingKong finished!");
            return $yield;
        }
    }
};
JsTypes.push(TestCoroutine);

