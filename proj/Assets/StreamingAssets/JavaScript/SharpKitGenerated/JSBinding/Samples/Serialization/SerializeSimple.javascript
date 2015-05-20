if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var SerializeSimple = {
    fullname: "SerializeSimple",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.age = 0;
            this.shortAge = 0;
            this.go = null;
            this.firstName = "QIU";
            this.doYouLoveMe = false;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Debug.Log$$Object("age: " + this.age);
            UnityEngine.Debug.Log$$Object("shortAge: " + this.shortAge);
            if (UnityEngine.Object.op_Inequality(this.go, null))
                UnityEngine.Debug.Log$$Object("go: " + this.go.get_name());
            else
                UnityEngine.Debug.Log$$Object("go: null");
            UnityEngine.Debug.Log$$Object("firstName: " + this.firstName);
            UnityEngine.Debug.Log$$Object("doYouLoveMe: " + (this.doYouLoveMe ? "true" : "false"));
        },
        Update: function (){
        }
    }
};
JsTypes.push(SerializeSimple);

