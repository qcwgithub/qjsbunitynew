if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestSplineTween = {
    fullname: "TestSplineTween",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.Spline = null;
            this.Duration = 5;
            this.Easing = 0;
            this.Loop = 1;
            this.target = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            if (UnityEngine.Object.op_Equality(this.Spline, null)){
                var obj = new UnityEngine.GameObject.ctor$$String("path");
                obj.get_transform().set_position(this.get_transform().get_position());
                this.Spline = obj.AddComponent$1(DaikonForge.Tween.SplineObject.ctor);
                this.Spline.AddNode$$Vector3(this.get_transform().get_position());
                var mid = UnityEngine.Vector3.op_Addition(UnityEngine.Vector3.op_Division((UnityEngine.Vector3.op_Subtraction(this.target.get_position(), this.get_transform().get_position())), 2), this.get_transform().get_position());
                var len = (UnityEngine.Vector3.op_Subtraction(this.target.get_position(), this.get_transform().get_position())).get_magnitude();
                var ddd = UnityEngine.Vector3.op_Multiply$$Vector3$$Single(UnityEngine.Quaternion.op_Multiply$$Quaternion$$Vector3(UnityEngine.Quaternion.Euler$$Single$$Single$$Single(0, 0, 90), (UnityEngine.Vector3.op_Subtraction(this.target.get_position(), mid)).get_normalized()), UnityEngine.Mathf.Clamp$$Single$$Single$$Single(len / 2, 1, 5));
                this.Spline.AddNode$$Vector3(UnityEngine.Vector3.op_Addition(mid, ddd));
                this.Spline.AddNode$$Vector3(this.target.get_position());
            }
            var tp = TweenComponentExtensions.TweenPath$$Component$$IPathIterator(this, this.Spline.Spline).SetDuration(this.Duration).SetEasing(DaikonForge.Tween.TweenEasingFunctions.GetFunction(this.Easing)).SetLoopType(this.Loop).SetTimeScaleIndependent(true).Play();
            tp.OnCompleted($CreateDelegate(this, this.onTweenComplete));
        },
        onTweenComplete: function (sender){
            UnityEngine.Debug.Log$$Object("finish");
        }
    }
};
JsTypes.push(TestSplineTween);

