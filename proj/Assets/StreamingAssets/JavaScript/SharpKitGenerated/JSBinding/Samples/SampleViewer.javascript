if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var SampleViewer = {
    fullname: "SampleViewer",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        cctor: function (){
            SampleViewer.inst = null;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.showScenesList = true;
            this.scenes = [new SampleViewer.stScene.ctor$$String$$String("V3Test", ""), new SampleViewer.stScene.ctor$$String$$String("V3Test_JS", ""), new SampleViewer.stScene.ctor$$String$$String("PerformanceTest1", ""), new SampleViewer.stScene.ctor$$String$$String("PerformanceTest1_JS", ""), new SampleViewer.stScene.ctor$$String$$String("ListTest", ""), new SampleViewer.stScene.ctor$$String$$String("ListTest_JS", ""), new SampleViewer.stScene.ctor$$String$$String("DictionaryTest", ""), new SampleViewer.stScene.ctor$$String$$String("DictionaryTest_JS", ""), new SampleViewer.stScene.ctor$$String$$String("DelegateTest", ""), new SampleViewer.stScene.ctor$$String$$String("DelegateTest_JS", ""), new SampleViewer.stScene.ctor$$String$$String("TestCoroutine", ""), new SampleViewer.stScene.ctor$$String$$String("TestCoroutine_JS", ""), new SampleViewer.stScene.ctor$$String$$String("JSImpTest1", ""), new SampleViewer.stScene.ctor$$String$$String("JSImpTest1_JS", ""), new SampleViewer.stScene.ctor$$String$$String("Car", ""), new SampleViewer.stScene.ctor$$String$$String("Car_JS", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeSimple", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeSimple_JS", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeStruct", ""), new SampleViewer.stScene.ctor$$String$$String("SerializeStruct_JS", ""), new SampleViewer.stScene.ctor$$String$$String("XmlTest", ""), new SampleViewer.stScene.ctor$$String$$String("XmlTest_JS", "")];
            this.scrollPosition = UnityEngine.Vector2.get_zero();
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            if (UnityEngine.Object.op_Inequality(JSEngine.inst, null)){
                UnityEngine.Object.Destroy$$Object(JSEngine.inst.get_gameObject());
            }
            if (UnityEngine.Object.op_Inequality(SampleViewer.inst, null)){
                UnityEngine.Object.Destroy$$Object(this.get_gameObject());
            }
            else {
                SampleViewer.inst = this;
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
                    UnityEngine.Application.LoadLevel$$String("SampleViewer");
                }
            }
        }
    }
};
JsTypes.push(SampleViewer);

