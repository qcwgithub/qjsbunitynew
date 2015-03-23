if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var JSSerializationTest = {
    fullname: "JSSerializationTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitWebApp1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.enum1 = 8;
            this.floatV = 0;
            this.intV = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Debug.Log$$Object(this.enum1.toString());
            UnityEngine.Debug.Log$$Object(this.floatV.toString());
            UnityEngine.Debug.Log$$Object(this.intV.toString());
        },
        Update: function (){
        }
    }
};
JsTypes.push(JSSerializationTest);

