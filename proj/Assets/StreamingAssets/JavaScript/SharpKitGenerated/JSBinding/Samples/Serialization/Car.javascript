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
            for (var $i11 = 0,$t11 = this.wheels,$l11 = $t11.length,w = $t11[$i11]; $i11 < $l11; $i11++, w = $t11[$i11]){
                if (UnityEngine.Object.op_Inequality(w, null)){
                    w.setSpeed(UnityEngine.Random.Range$$Single$$Single(1, 4));
                }
            }
            for (var $i12 = 0,$t12 = this.goWheels,$l12 = $t12.length,go = $t12[$i12]; $i12 < $l12; $i12++, go = $t12[$i12]){
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

