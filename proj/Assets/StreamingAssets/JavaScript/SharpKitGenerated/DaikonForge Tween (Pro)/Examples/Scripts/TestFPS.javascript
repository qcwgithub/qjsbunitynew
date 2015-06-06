if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TestFPS = {
    fullname: "TestFPS",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        cctor: function (){
            TestFPS.UPDATE_INTERVAL = 0.2;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.editorOnly = true;
            this.labelText = "00";
            this.labelColor = UnityEngine.Color.get_white();
            this.labelStyle = null;
            this.accum = 0;
            this.numFrames = 0;
            this.timeleft = 0;
            this.lastFrameTime = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this.timeleft = 0.2;
        },
        OnGUI: function (){
            if (this.editorOnly && !UnityEngine.Application.get_isEditor())
                return;
            if (this.labelStyle == null){
                this.labelStyle = new UnityEngine.GUIStyle.ctor$$GUIStyle(UnityEngine.GUIStyle.op_Implicit("label"));
                this.labelStyle.set_alignment(4);
            }
            var savedColor = UnityEngine.GUI.get_color();
            var rect = new UnityEngine.Rect.ctor$$Single$$Single$$Single$$Single(0, 0, 100, 25);
            UnityEngine.GUI.set_color(UnityEngine.Color.get_black());
            UnityEngine.GUI.Box$$Rect$$GUIContent(rect, UnityEngine.GUIContent.none);
            UnityEngine.GUI.set_color(this.labelColor);
            this.labelStyle.get_normal().set_textColor(this.labelColor);
            UnityEngine.GUI.Label$$Rect$$String$$GUIStyle(rect, this.labelText, this.labelStyle);
            UnityEngine.GUI.set_color(savedColor);
        },
        Update: function (){
            var realDeltaTime = UnityEngine.Time.get_realtimeSinceStartup() - this.lastFrameTime;
            this.lastFrameTime = UnityEngine.Time.get_realtimeSinceStartup();
            if (UnityEngine.Time.get_timeScale() <= 0.01){
                this.labelColor = UnityEngine.Color.get_yellow();
                this.labelText = "Pause";
                return;
            }
            this.timeleft -= realDeltaTime;
            this.accum += 1 / realDeltaTime;
            this.numFrames += 1;
            if (this.timeleft <= 0){
                var fps = UnityEngine.Mathf.CeilToInt(this.accum / this.numFrames);
                this.labelText = System.String.Format$$String$$Object("{0:F0} FPS", fps);
                if (fps < 30)
                    this.labelColor = UnityEngine.Color.get_yellow();
                else if (fps < 10)
                    this.labelColor = UnityEngine.Color.get_red();
                else
                    this.labelColor = UnityEngine.Color.get_green();
                this.timeleft = 0.2;
                this.accum = 0;
                this.numFrames = 0;
            }
        }
    }
};
JsTypes.push(TestFPS);

