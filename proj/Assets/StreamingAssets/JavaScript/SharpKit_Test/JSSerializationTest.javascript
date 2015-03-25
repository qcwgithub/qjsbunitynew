if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var JSSerializationTest = {
    fullname: "JSSerializationTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitWebApp2",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.enum1 = 8;
            this.floatV = 0;
            this.arrIntV = null;
            this.arrGameObject = null;
            this.arrVec = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Debug.Log$$Object(this.enum1.toString());
            UnityEngine.Debug.Log$$Object(this.floatV.toString());
            var s = "arrIntV = ";
            for (var i = 0; i < this.arrIntV.length; i++){
                s += this.arrIntV[i].toString() + ", ";
            }
            UnityEngine.Debug.Log$$Object(s);
            s = "arrGameObject = ";
            for (var i = 0; i < this.arrGameObject.length; i++){
                s += this.arrGameObject[i].get_name() + ", ";
            }
            UnityEngine.Debug.Log$$Object(s);
            s = "arrVec = ";
            for (var i = 0; i < this.arrVec.length; i++){
                s += this.arrVec[i].toString() + ", ";
            }
            UnityEngine.Debug.Log$$Object(s);
            this.GetComponent$1(QiucwAttribute.ctor).Print();
        },
        Update: function (){
        }
    }
};
JsTypes.push(JSSerializationTest);

