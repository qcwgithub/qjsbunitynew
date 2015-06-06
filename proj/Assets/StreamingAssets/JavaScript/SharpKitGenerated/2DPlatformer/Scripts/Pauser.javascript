if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Pauser = {
    fullname: "Pauser",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.paused = false;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Update: function (){
            if (UnityEngine.Input.GetKeyUp$$KeyCode(112)){
                this.paused = !this.paused;
            }
            if (this.paused)
                UnityEngine.Time.set_timeScale(0);
            else
                UnityEngine.Time.set_timeScale(1);
        }
    }
};
JsTypes.push(Pauser);

