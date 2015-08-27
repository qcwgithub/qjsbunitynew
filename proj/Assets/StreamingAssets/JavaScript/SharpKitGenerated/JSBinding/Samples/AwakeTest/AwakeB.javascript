if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var AwakeB = {
    fullname: "AwakeB",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.valueOfB = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            var a = this.GetComponent$1(AwakeA.ctor);
            UnityEngine.MonoBehaviour.print(System.String.Format$$String$$Object$$Object("B.GetComponent<A>: {0}, value: {1}", a.get_name(), a.valueOfA));
            var c = this.get_gameObject().AddComponent$1(AwakeC.ctor);
            UnityEngine.MonoBehaviour.print("c.valueOfC = " + c.valueOfC);
        }
    }
};
JsTypes.push(AwakeB);

