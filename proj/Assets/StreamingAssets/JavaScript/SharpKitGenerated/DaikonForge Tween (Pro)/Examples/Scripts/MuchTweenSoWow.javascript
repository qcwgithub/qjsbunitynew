if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var MuchTweenSoWow = {
    fullname: "MuchTweenSoWow",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.doge = null;
            this.text = null;
            this.logo = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var timeline = new DaikonForge.Tween.TweenTimeline.ctor();
            var dogeScale = TweenReflectionExtensions.TweenProperty$1$$Object$$String(UnityEngine.Vector3.ctor, this.doge.get_transform(), "localScale").SetStartValue(UnityEngine.Vector3.get_zero()).SetDuration(0.5).SetDelay$$Single(0.5).SetInterpolator(DaikonForge.Tween.Interpolation.EulerInterpolator.get_Default()).SetEasing(DaikonForge.Tween.TweenEasingFunctions.Spring);
            timeline.Add(0, dogeScale);
            for (var i = 0; i < this.text.length; i++){
                this.text[i].set_color(new UnityEngine.Color.ctor$$Single$$Single$$Single$$Single(1, 1, 1, 0));
                var alphaTween = TweenTextExtensions.TweenAlpha$$TextMesh(this.text[i]).SetAutoCleanup(true).SetDuration(0.33).SetStartValue(0).SetEndValue(1);
                var rotTween = TweenComponentExtensions.TweenRotation$$Component(this.text[i]).SetAutoCleanup(true).SetEndValue(UnityEngine.Vector3.get_zero()).SetDuration(0.5).SetEasing(DaikonForge.Tween.TweenEasingFunctions.Spring);
                timeline.Add(1.5 + 0.75 * i, alphaTween, rotTween);
            }
            var cameraSlide = TweenComponentExtensions.TweenPosition$$Component(this).SetEndValue(UnityEngine.Vector3.op_Addition(this.get_transform().get_position(), new UnityEngine.Vector3.ctor$$Single$$Single$$Single(0, -1, 0))).SetDuration(0.5).SetEasing(DaikonForge.Tween.TweenEasingFunctions.EaseInOutQuad);
            var logoSlide = TweenComponentExtensions.TweenPosition$$Component(this.logo).SetStartValue(UnityEngine.Vector3.op_Subtraction(this.logo.get_transform().get_position(), (UnityEngine.Vector3.op_Multiply$$Vector3$$Single(UnityEngine.Vector3.get_up(), 5)))).SetDuration(1).SetEasing(DaikonForge.Tween.TweenEasingFunctions.Bounce);
            var logoAlphaTween = TweenTextExtensions.TweenAlpha$$TextMesh(this.logo).SetStartValue(0).SetEndValue(1).SetDuration(0.75);
            this.logo.set_color(new UnityEngine.Color.ctor$$Single$$Single$$Single$$Single(1, 1, 1, 0));
            timeline.Add(3.75, cameraSlide);
            timeline.Add(4, logoSlide, logoAlphaTween);
            timeline.Play();
        }
    }
};
JsTypes.push(MuchTweenSoWow);

