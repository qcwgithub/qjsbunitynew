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
            for (var $i5 = 0,$t5 = this.wheels,$l5 = $t5.length,w = $t5[$i5]; $i5 < $l5; $i5++, w = $t5[$i5]){
                if (UnityEngine.Object.op_Inequality(w, null)){
                    w.setSpeed(UnityEngine.Random.Range$$Single$$Single(1, 4));
                }
            }
            for (var $i6 = 0,$t6 = this.goWheels,$l6 = $t6.length,go = $t6[$i6]; $i6 < $l6; $i6++, go = $t6[$i6]){
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

