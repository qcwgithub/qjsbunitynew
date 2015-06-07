if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Viewer = {
    fullname: "Viewer",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        cctor: function (){
            Viewer.inst = null;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.showScenesList = true;
            this.scenes = [new Viewer.stScene.ctor$$String$$String("Level", "2D Platformer"), new Viewer.stScene.ctor$$String$$String("ListTest", ""), new Viewer.stScene.ctor$$String$$String("ListTest_JS", ""), new Viewer.stScene.ctor$$String$$String("DictionaryTest", ""), new Viewer.stScene.ctor$$String$$String("DictionaryTest_JS", ""), new Viewer.stScene.ctor$$String$$String("PerformanceTest1", ""), new Viewer.stScene.ctor$$String$$String("PerformanceTest1_JS", ""), new Viewer.stScene.ctor$$String$$String("TestCoroutine", ""), new Viewer.stScene.ctor$$String$$String("V3Test", ""), new Viewer.stScene.ctor$$String$$String("V3Test_JS", "V3Test_JS"), new Viewer.stScene.ctor$$String$$String("SerializeSimple", ""), new Viewer.stScene.ctor$$String$$String("SerializeSimple_JS", ""), new Viewer.stScene.ctor$$String$$String("SerializeStruct", ""), new Viewer.stScene.ctor$$String$$String("SerializeStruct_JS", ""), new Viewer.stScene.ctor$$String$$String("Car", ""), new Viewer.stScene.ctor$$String$$String("Car_JS", ""), new Viewer.stScene.ctor$$String$$String("Camera Shake", "DF Tween: Camera Shake"), new Viewer.stScene.ctor$$String$$String("Camera Shake_JS", "DF Tween: Camera Shake_JS"), new Viewer.stScene.ctor$$String$$String("Doge", "DF Tween: Doge"), new Viewer.stScene.ctor$$String$$String("Doge_JS", "DF Tween: Doge_JS"), new Viewer.stScene.ctor$$String$$String("Easing Functions", "DF Tween: Easing Functions"), new Viewer.stScene.ctor$$String$$String("Easing Functions_JS", "DF Tween: Easing Functions_JS"), new Viewer.stScene.ctor$$String$$String("Slinky", "DF Tween: Slinky"), new Viewer.stScene.ctor$$String$$String("Tween Along Path", "DF Tween: Tween Along Path"), new Viewer.stScene.ctor$$String$$String("Tween Along Path_JS", "DF Tween: Tween Along Path_JS")];
            this.scrollPosition = UnityEngine.Vector2.get_zero();
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            if (UnityEngine.Object.op_Inequality(JSEngine.inst, null)){
                UnityEngine.Object.Destroy$$Object(JSEngine.inst.get_gameObject());
            }
            if (UnityEngine.Object.op_Inequality(Viewer.inst, null)){
                UnityEngine.Object.Destroy$$Object(this.get_gameObject());
            }
            else {
                Viewer.inst = this;
                UnityEngine.Object.DontDestroyOnLoad(this.get_gameObject());
            }
        },
        Start: function (){
        },
        Update: function (){
        },
        OnGUI: function (){
            if (this.showScenesList){
                var h = UnityEngine.Screen.get_height() / 10;
                this.scrollPosition = UnityEngine.GUI.BeginScrollView$$Rect$$Vector2$$Rect$$Boolean$$Boolean(new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(0, 0, UnityEngine.Screen.get_width(), UnityEngine.Screen.get_height()), this.scrollPosition, new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(0, 0, UnityEngine.Screen.get_width(), this.scenes.length * h), false, false);
                for (var i = 0; i < this.scenes.length; i++){
                    if (UnityEngine.GUI.Button$$Rect$$String(new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(UnityEngine.Screen.get_width() / 4, h * i, UnityEngine.Screen.get_width() / 2, h), this.scenes[i].showText)){
                        this.showScenesList = false;
                        UnityEngine.Application.LoadLevel$$String(this.scenes[i].levelName);
                        break;
                    }
                }
                UnityEngine.GUI.EndScrollView();
            }
            else {
                var w = UnityEngine.Screen.get_width() / 10;
                var h = UnityEngine.Screen.get_height() / 10;
                if (w < 100)
                    w = 150;
                if (UnityEngine.GUI.Button$$Rect$$String(new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(UnityEngine.Screen.get_width() - w, 0, w, h), "Back To Scene List")){
                    this.showScenesList = true;
                    UnityEngine.Application.LoadLevel$$String("Viewer");
                }
            }
        }
    }
};
JsTypes.push(Viewer);

