if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestCoroutine = {
    fullname: "TestCoroutine",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.value = 100;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.StartCoroutine$$IEnumerator(this.CountDown());
        },
        Update: function (){
        },
        CountDown: function (){
            var $yield = [];
            var increment = 0;
            while (true){
                if (UnityEngine.Input.GetKey$$KeyCode(113))
                    break;
                increment += UnityEngine.Time.get_deltaTime();
                this.value -= 1;
                UnityEngine.Debug.Log$$Object(this.value);
                $yield.push(new UnityEngine.WaitForSeconds.ctor(1));
            }
            return $yield;
        }
    }
};
JsTypes.push(TestCoroutine);

