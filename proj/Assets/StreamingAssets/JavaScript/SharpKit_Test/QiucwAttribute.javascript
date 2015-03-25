if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var QiucwAttribute = {
    fullname: "QiucwAttribute",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitWebApp2",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.age = 0;
            this.name = null;
            this.sex = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
        },
        Print: function (){
            UnityEngine.Debug.Log$$Object("QiucwAttribute: " + this.age.toString() + " " + this.name.toString() + " " + this.sex.toString());
        }
    }
};
JsTypes.push(QiucwAttribute);

