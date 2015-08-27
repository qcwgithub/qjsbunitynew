if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var AwakeC = {
    fullname: "AwakeC",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.valueOfC = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.valueOfC = 8;
        }
    }
};
JsTypes.push(AwakeC);

