if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var AwakeA = {
    fullname: "AwakeA",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.valueOfA = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            var b = this.GetComponent$1(AwakeB.ctor);
            UnityEngine.MonoBehaviour.print(System.String.Format$$String$$Object$$Object("A.GetComponent<B>: {0}, value: {1}", b.get_name(), b.valueOfB));
        }
    }
};
JsTypes.push(AwakeA);

