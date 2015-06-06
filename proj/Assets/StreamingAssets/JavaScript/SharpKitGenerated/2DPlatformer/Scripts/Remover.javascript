if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Remover = {
    fullname: "Remover",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.splash = null;
            this._reload = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        OnTriggerEnter2D: function (col){
            if (col.get_gameObject().get_tag() == "Player"){
                UnityEngine.GameObject.FindGameObjectWithTag("MainCamera").GetComponent$1(CameraFollow.ctor).set_enabled(false);
                if (UnityEngine.GameObject.FindGameObjectWithTag("HealthBar").get_activeSelf()){
                    UnityEngine.GameObject.FindGameObjectWithTag("HealthBar").SetActive(false);
                }
                UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.splash, col.get_transform().get_position(), this.get_transform().get_rotation());
                UnityEngine.Object.Destroy$$Object(col.get_gameObject());
                this.Pre_ReloadGame();
            }
            else {
                UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.splash, col.get_transform().get_position(), this.get_transform().get_rotation());
                UnityEngine.Object.Destroy$$Object(col.get_gameObject());
            }
        },
        Update: function (){
            if (this._reload > 0){
                this._reload -= UnityEngine.Time.get_deltaTime();
                if (this._reload <= 0){
                    this.ReloadGame();
                }
            }
        },
        Pre_ReloadGame: function (){
            this._reload = 2;
        },
        ReloadGame: function (){
            UnityEngine.Application.LoadLevel$$Int32(UnityEngine.Application.get_loadedLevel());
        }
    }
};
JsTypes.push(Remover);

