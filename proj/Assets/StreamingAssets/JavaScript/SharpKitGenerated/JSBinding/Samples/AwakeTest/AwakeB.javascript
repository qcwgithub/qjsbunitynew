if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var AwakeB = {
    fullname: "AwakeB",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            var a = this.GetComponent$1(AwakeA.ctor);
            UnityEngine.MonoBehaviour.print("B call A: " + a.get_name());
        }
    }
};
JsTypes.push(AwakeB);

