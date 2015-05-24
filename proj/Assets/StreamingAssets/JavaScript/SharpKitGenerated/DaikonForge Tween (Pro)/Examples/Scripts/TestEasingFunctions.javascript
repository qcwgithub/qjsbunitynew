if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestEasingFunctions = {
    fullname: "TestEasingFunctions",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.EaseType = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var func = DaikonForge.Tween.TweenEasingFunctions.GetFunction(this.EaseType);
            TweenTransformExtensions.TweenPosition$$Transform(this.get_transform()).SetEndValue(UnityEngine.Vector3.op_Addition(this.get_transform().get_position(), (UnityEngine.Vector3.op_Multiply$$Vector3$$Single(UnityEngine.Vector3.get_right(), 9)))).SetDelay$$Single$$Boolean(0.5, false).SetDuration(1.33).SetEasing(func).SetLoopType(1).Play();
        }
    }
};
JsTypes.push(TestEasingFunctions);

