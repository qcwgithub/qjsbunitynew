if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var AwakeA = {
    fullname: "AwakeA",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            var b = this.GetComponent$1(AwakeB.ctor);
            UnityEngine.MonoBehaviour.print("A call B: " + b.get_name());
        }
    }
};
JsTypes.push(AwakeA);

