if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var BackgroundParallax = {
    fullname: "BackgroundParallax",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.backgrounds = null;
            this.parallaxScale = 0;
            this.parallaxReductionFactor = 0;
            this.smoothing = 0;
            this.cam = null;
            this.previousCamPos = new UnityEngine.Vector3.ctor();
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.cam = UnityEngine.Camera.get_main().get_transform();
        },
        Start: function (){
            this.previousCamPos = this.cam.get_position();
        },
        Update: function (){
            var parallax = (this.previousCamPos.x - this.cam.get_position().x) * this.parallaxScale;
            for (var i = 0; i < this.backgrounds.length; i++){
                var backgroundTargetPosX = this.backgrounds[i].get_position().x + parallax * (i * this.parallaxReductionFactor + 1);
                var backgroundTargetPos = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(backgroundTargetPosX, this.backgrounds[i].get_position().y, this.backgrounds[i].get_position().z);
                this.backgrounds[i].set_position(UnityEngine.Vector3.Lerp(this.backgrounds[i].get_position(), backgroundTargetPos, this.smoothing * UnityEngine.Time.get_deltaTime()));
            }
            this.previousCamPos = this.cam.get_position();
        }
    }
};
JsTypes.push(BackgroundParallax);

