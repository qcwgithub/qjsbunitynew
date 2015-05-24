if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestHighConcurrency = {
    fullname: "TestHighConcurrency",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.BounceRange = 9;
            this.tweenRotation = null;
            this.tweenPosition = null;
            this.tweenPunchScale = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        OnEnable: function (){
            this.tweenPosition = TweenComponentExtensions.TweenPosition$$Component(this).SetStartValue(this.get_transform().get_position()).SetEndValue(UnityEngine.Vector3.op_Subtraction(this.get_transform().get_position(), (UnityEngine.Vector3.op_Multiply$$Vector3$$Single(UnityEngine.Vector3.get_up(), this.BounceRange)))).SetDuration(1).SetEasing(DaikonForge.Tween.TweenEasingFunctions.EaseInOutBack).SetDelay$$Single(UnityEngine.Random.Range$$Single$$Single(0.1, 0.5));
            this.tweenRotation = TweenComponentExtensions.TweenRotation$$Component$$Boolean(this, false).SetEndValue(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(0, 360, 0)).SetDuration(0.25).SetDelay$$Single(UnityEngine.Random.Range$$Single$$Single(0, 0.5));
            this.tweenPunchScale = TweenComponentExtensions.TweenScale(this).SetStartValue(this.get_transform().get_localScale()).SetEndValue(UnityEngine.Vector3.op_Multiply$$Vector3$$Single(this.get_transform().get_localScale(), 2)).SetDelay$$Single(UnityEngine.Random.Range$$Single$$Single(0.25, 0.75)).SetEasing(DaikonForge.Tween.TweenEasingFunctions.Punch);
            this.tweenPosition.Play().Chain$$TweenBase(this.tweenRotation).Wait(0.5).Chain$$TweenBase(this.tweenPunchScale).Wait(0.5).Chain$$TweenBase$$Action(this.tweenPosition, $CreateAnonymousDelegate(this, function (){
                this.tweenPosition.SetLoopType(2).SetDelay$$Single(1.33).SetDuration(1.25).ReversePlayDirection().Play();
            }));
        }
    }
};
JsTypes.push(TestHighConcurrency);

