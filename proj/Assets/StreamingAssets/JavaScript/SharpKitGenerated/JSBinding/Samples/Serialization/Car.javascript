if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Car = {
    fullname: "Car",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.wheels = null;
            this.goWheels = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            for (var $i7 = 0,$t7 = this.wheels,$l7 = $t7.length,w = $t7[$i7]; $i7 < $l7; $i7++, w = $t7[$i7]){
                if (UnityEngine.Object.op_Inequality(w, null)){
                    w.setSpeed(UnityEngine.Random.Range$$Single$$Single(1, 4));
                }
            }
            for (var $i8 = 0,$t8 = this.goWheels,$l8 = $t8.length,go = $t8[$i8]; $i8 < $l8; $i8++, go = $t8[$i8]){
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

