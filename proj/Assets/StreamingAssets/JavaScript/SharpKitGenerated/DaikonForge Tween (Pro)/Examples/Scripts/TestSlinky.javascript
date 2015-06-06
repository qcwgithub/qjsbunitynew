if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestSlinky = {
    fullname: "TestSlinky",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.tweens = new System.Collections.Generic.List$1.ctor(DaikonForge.Tween.Tween$1.ctor);
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        OnEnable: function (){
            var rings = UnityEngine.Object.FindObjectsOfType$1(UnityEngine.SpriteRenderer.ctor) instanceof Array ? UnityEngine.Object.FindObjectsOfType$1(UnityEngine.SpriteRenderer.ctor) : null;
            System.Array.Sort$1$$T$Array$$Comparison$1(UnityEngine.SpriteRenderer.ctor, rings, $CreateAnonymousDelegate(this, function (lhs, rhs){
                return lhs.get_sortingOrder().CompareTo$$Int32(rhs.get_sortingOrder());
            }));
            for (var i = 0; i < rings.length; i++){
                var obj = rings[i];
                var tween = TweenComponentExtensions.TweenPosition$$Component(obj).SetDelay$$Single(i * 0.02).SetDuration(0.5).SetEasing(DaikonForge.Tween.TweenEasingFunctions.Spring);
                tween.Play();
                this.tweens.Add(tween);
            }
        },
        OnGUI: function (){
            var rect = new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(10, 10, 600, 25);
            UnityEngine.GUI.Label$$Rect$$String(rect, "Click anywhere with the mouse to move rings. SPACE: Slow, ENTER: Fast");
        },
        Update: function (){
            if (UnityEngine.Input.GetKeyDown$$KeyCode(32)){
                UnityEngine.Debug.Log$$Object("Time scale: SLOW");
                UnityEngine.Time.set_timeScale(0.15);
            }
            else if (UnityEngine.Input.GetKeyDown$$KeyCode(13)){
                UnityEngine.Debug.Log$$Object("Time scale: NORMAL");
                UnityEngine.Time.set_timeScale(1);
            }
            if (!UnityEngine.Input.GetMouseButtonDown(0))
                return;
            var plane = new UnityEngine.Plane.ctor$$Vector3$$Vector3(UnityEngine.Vector3.get_back(), UnityEngine.Vector3.get_zero());
            var ray = UnityEngine.Camera.get_main().ScreenPointToRay(UnityEngine.Input.get_mousePosition());
            var distance = 0;
            (function (){
                var $1 = {
                    Value: distance
                };
                var $res = plane.Raycast(ray, $1);
                distance = $1.Value;
                return $res;
            }).call(this);
            var endPosition = ray.GetPoint(distance);
            for (var i = 0; i < this.tweens.get_Count(); i++){
                var tween = this.tweens.get_Item$$Int32(i);
                tween.SetStartValue(tween.get_CurrentValue()).SetEndValue(endPosition).Play();
            }
        }
    }
};
JsTypes.push(TestSlinky);

