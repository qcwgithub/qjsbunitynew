if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestShakeObject = {
    fullname: "TestShakeObject",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.Camera = null;
            this.shake = null;
            this.fallTween = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.shake = DaikonForge.Tween.TweenShake.Obtain().SetStartValue(this.Camera.get_position()).SetDuration(1).SetShakeMagnitude(0.25).SetShakeSpeed(13).SetTimeScaleIndependent(true).OnExecute($CreateAnonymousDelegate(this, function (result){
                this.Camera.set_position(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(result.x, result.y, this.Camera.get_position().z));
            }));
            this.fallTween = TweenComponentExtensions.TweenPosition$$Component(this).SetStartValue(UnityEngine.Vector3.op_Addition(this.get_transform().get_position(), UnityEngine.Vector3.op_Multiply$$Vector3$$Single(UnityEngine.Vector3.get_up(), 5))).SetEasing(DaikonForge.Tween.TweenEasingFunctions.EaseInExpo).SetDuration(1).SetDelay$$Single(0.25).SetTimeScaleIndependent(true).OnCompleted($CreateAnonymousDelegate(this, function (sender){
                this.shake.Play();
            })).Play();
        },
        OnGUI: function (){
            UnityEngine.GUILayout.Label$$String$$GUILayoutOption$Array(" Press SPACE to restart ");
        },
        Update: function (){
            if (UnityEngine.Input.GetKeyDown$$KeyCode(276)){
                UnityEngine.Debug.Log$$Object("Time scale: SLOW");
                UnityEngine.Time.set_timeScale(0.15);
            }
            else if (UnityEngine.Input.GetKeyDown$$KeyCode(275)){
                UnityEngine.Debug.Log$$Object("Time scale: NORMAL");
                UnityEngine.Time.set_timeScale(1);
            }
            if (UnityEngine.Input.GetKeyDown$$KeyCode(32)){
                this.fallTween.Stop().Rewind().Play();
            }
        }
    }
};
JsTypes.push(TestShakeObject);

