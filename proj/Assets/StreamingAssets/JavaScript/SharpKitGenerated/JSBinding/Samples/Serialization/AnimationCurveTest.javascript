if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var AnimationCurveTest = {
    fullname: "AnimationCurveTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.curve = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var s = "";
            s += "length " + this.curve.get_length() + "\n";
            s += "prevWrapMode " + this.curve.get_preWrapMode() + "\n";
            s += "postWrapMode " + this.curve.get_postWrapMode() + "\n";
            s += "\n";
            for (var f = -3; f < 3; f += 0.4){
                s += f.toString() + " = " + this.curve.Evaluate(f) + "\n";
            }
            UnityEngine.MonoBehaviour.print(s);
        },
        Update: function (){
        }
    }
};
JsTypes.push(AnimationCurveTest);

