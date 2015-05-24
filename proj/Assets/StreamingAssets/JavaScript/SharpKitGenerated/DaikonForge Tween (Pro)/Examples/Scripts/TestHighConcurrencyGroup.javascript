if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestHighConcurrencyGroup = {
    fullname: "TestHighConcurrencyGroup",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        cctor: function (){
            TestHighConcurrencyGroup.SPRITE_COUNT = 1000;
        }
    },
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.pivot = null;
            this.spriteTemplate = null;
            this.tweens = new System.Collections.Generic.List$1.ctor(DaikonForge.Tween.TweenBase.ctor);
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            DebugMessages.Add("This sample animates the position, opacity, and scale of 1000 sprites simultaneously.");
            DebugMessages.Add("Due to the high number of simultaneous animations, it may run slowly in the browser or on mobile devices.");
        },
        OnEnable: function (){
            if (UnityEngine.Object.op_Equality(this.spriteTemplate, null))
                return;
            var radius = UnityEngine.Camera.get_main().get_orthographicSize() * 0.95;
            var duration = 0.5;
            var easingFunc = DaikonForge.Tween.TweenEasingFunctions.EaseOutSine;
            for (var i = 0; i < 1000; i++){
                var angle = UnityEngine.Random.Range$$Single$$Single(0, 6.283185);
                var endPosition = new UnityEngine.Vector3.ctor$$Single$$Single(UnityEngine.Mathf.Cos(angle) * radius, UnityEngine.Mathf.Sin(angle) * radius);
                var sprite = Cast(UnityEngine.Object.Instantiate$$Object(this.spriteTemplate), UnityEngine.GameObject.ctor);
                sprite.set_hideFlags(1);
                var animateOpacity = TweenComponentExtensions.TweenAlpha(sprite.get_renderer()).SetDuration(duration).SetEasing(easingFunc).SetStartValue(0).SetEndValue(0.8);
                var animateScale = TweenTransformExtensions.TweenScale(sprite.get_transform()).SetDuration(duration).SetEasing(easingFunc).SetStartValue(UnityEngine.Vector3.op_Multiply$$Vector3$$Single(UnityEngine.Vector3.get_one(), 0.25)).SetEndValue(UnityEngine.Vector3.get_one());
                var animatePosition = TweenTransformExtensions.TweenPosition$$Transform(sprite.get_transform()).SetDuration(duration).SetEasing(easingFunc).SetStartValue(UnityEngine.Vector3.get_zero()).SetEndValue(endPosition);
                var concurrentGroup = new DaikonForge.Tween.TweenGroup.ctor().SetMode(1).AppendTween(animateOpacity, animateScale, animatePosition);
                var sequentialGroup = new DaikonForge.Tween.TweenGroup.ctor().SetMode(0).AppendDelay(255 / i).AppendTween(concurrentGroup).SetLoopType(1);
                this.tweens.Add(sequentialGroup);
                sequentialGroup.Play();
            }
            DebugMessages.Add((5000) + " tweens created...");
            if (UnityEngine.Object.op_Inequality(this.pivot, null)){
                var animatePivot = TweenTransformExtensions.TweenRotation$$Transform(this.pivot.get_transform()).SetDuration(10).SetStartValue(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(0, -45, 0)).SetEndValue(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(0, 45, 0)).SetLoopType(2);
                this.tweens.Add(animatePivot);
                animatePivot.Play();
            }
        },
        OnGUI: function (){
            var rect = new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(10, 30, 200, UnityEngine.Screen.get_height() - 100);
            UnityEngine.GUI.Box$$Rect$$GUIContent(rect, UnityEngine.GUIContent.none);
            rect.set_xMin(rect.get_xMin() + 5);
            rect.set_yMin(rect.get_yMin() + 5);
            rect.set_xMax(rect.get_xMax() - 10);
            UnityEngine.GUILayout.BeginArea$$Rect(rect);
            UnityEngine.GUILayout.BeginVertical$$GUILayoutOption$Array();
            var tweenState = this.tweens.get_Item$$Int32(0).get_State();
            UnityEngine.GUI.set_enabled(tweenState == 0);
            if (UnityEngine.GUILayout.Button$$String$$GUILayoutOption$Array("Play")){
                DebugMessages.Add("Playing...");
                for (var i = 0; i < this.tweens.get_Count(); i++){
                    this.tweens.get_Item$$Int32(i).Rewind();
                    this.tweens.get_Item$$Int32(i).Play();
                }
            }
            if (tweenState == 2){
                UnityEngine.GUI.set_enabled(true);
                if (UnityEngine.GUILayout.Button$$String$$GUILayoutOption$Array("Pause")){
                    DebugMessages.Add("Pausing...");
                    DaikonForge.Tween.TweenManager.get_Instance().Pause();
                }
            }
            else if (tweenState == 1){
                UnityEngine.GUI.set_enabled(true);
                if (UnityEngine.GUILayout.Button$$String$$GUILayoutOption$Array("Resume")){
                    DebugMessages.Add("Resuming...");
                    DaikonForge.Tween.TweenManager.get_Instance().Resume();
                }
            }
            else {
                UnityEngine.GUI.set_enabled(false);
                UnityEngine.GUILayout.Button$$String$$GUILayoutOption$Array("Pause");
            }
            UnityEngine.GUI.set_enabled(tweenState != 0);
            if (UnityEngine.GUILayout.Button$$String$$GUILayoutOption$Array("Stop")){
                DebugMessages.Add("Stopped all tweens");
                DaikonForge.Tween.TweenManager.get_Instance().Stop();
            }
            UnityEngine.GUILayout.Space(25);
            if (tweenState != 0){
                var timeScaleSetting = this.tweens.get_Item$$Int32(0).IsTimeScaleIndependent;
                var userSetting = UnityEngine.GUILayout.Toggle$$Boolean$$String$$GUILayoutOption$Array(timeScaleSetting, "Time scale independent");
                if (userSetting != timeScaleSetting){
                    DebugMessages.Add("Toggled time scale independence");
                    for (var i = 0; i < this.tweens.get_Count(); i++){
                        this.tweens.get_Item$$Int32(i).SetIsTimeScaleIndependent(userSetting);
                    }
                }
                UnityEngine.GUILayout.Space(5);
                UnityEngine.GUILayout.Label$$String$$GUILayoutOption$Array("Time scale: " + UnityEngine.Time.get_timeScale());
                UnityEngine.Time.set_timeScale(UnityEngine.GUILayout.HorizontalSlider$$Single$$Single$$Single$$GUILayoutOption$Array(UnityEngine.Time.get_timeScale(), 0, 2));
            }
            UnityEngine.GUILayout.EndVertical();
            UnityEngine.GUILayout.EndArea();
            UnityEngine.GUI.set_enabled(true);
        }
    }
};
JsTypes.push(TestHighConcurrencyGroup);

