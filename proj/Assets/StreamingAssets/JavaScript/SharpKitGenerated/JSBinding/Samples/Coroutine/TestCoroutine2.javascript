if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestCoroutine2 = {
    fullname: "TestCoroutine2",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            new YieldYest.ctor().Run();
        },
        Update: function (){
        }
    }
};
JsTypes.push(TestCoroutine2);

