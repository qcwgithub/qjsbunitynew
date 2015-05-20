if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Car = {
    fullname: "Car",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.wheels = null;
            this.goWheels = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            for (var $i9 = 0,$t9 = this.wheels,$l9 = $t9.length,w = $t9[$i9]; $i9 < $l9; $i9++, w = $t9[$i9]){
                if (UnityEngine.Object.op_Inequality(w, null)){
                    w.setSpeed(UnityEngine.Random.Range$$Single$$Single(1, 4));
                }
            }
            for (var $i10 = 0,$t10 = this.goWheels,$l10 = $t10.length,go = $t10[$i10]; $i10 < $l10; $i10++, go = $t10[$i10]){
                if (UnityEngine.Object.op_Inequality(go, null)){
                    var w = go.GetComponent$1(Wheel.ctor);
                    if (UnityEngine.Object.op_Inequality(w, null)){
                        w.setSpeed(UnityEngine.Random.Range$$Single$$Single(1, 4));
                    }
                }
            }
        }
    }
};
JsTypes.push(Car);

