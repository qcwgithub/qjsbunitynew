if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var DelegateTest = {
    fullname: "DelegateTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.lst = null;
            this.mi = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.lst = new System.Collections.Generic.List$1.ctor(System.Int32.ctor);
            for (var i = 0; i < 10; i++){
                this.lst.Add(i);
            }
        },
        Start: function (){
        },
        Update: function (){
            if (UnityEngine.Input.GetMouseButtonDown(0)){
                var f = this.lst.Find($CreateAnonymousDelegate(this, function (v){
                    return v == this.mi;
                }));
                UnityEngine.Debug.Log$$Object("Found: " + f);
                this.mi++;
                if (this.mi >= 10)
                    this.mi -= 10;
            }
        }
    }
};
JsTypes.push(DelegateTest);

