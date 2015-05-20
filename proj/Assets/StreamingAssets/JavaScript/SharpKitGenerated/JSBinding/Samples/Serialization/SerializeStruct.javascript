if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var SerializeStruct = {
    fullname: "SerializeStruct",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.appleInfo = new SerializeStruct.AppleInfo.ctor();
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Debug.Log$$Object("age: " + this.appleInfo.age);
            if (UnityEngine.Object.op_Inequality(this.appleInfo.go, null))
                UnityEngine.Debug.Log$$Object("go: " + this.appleInfo.go.get_name());
            else
                UnityEngine.Debug.Log$$Object("go: null");
            UnityEngine.Debug.Log$$Object("firstName: " + this.appleInfo.firstName);
            UnityEngine.Debug.Log$$Object("doYouLoveMe: " + (this.appleInfo.doYouLoveMe ? "true" : "false"));
        },
        Update: function (){
        }
    }
};
JsTypes.push(SerializeStruct);

