if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var LayBombs = {
    fullname: "LayBombs",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.bombLaid = false;
            this.bombCount = 0;
            this.bombsAway = null;
            this.bomb = null;
            this.bombHUD = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.bombHUD = UnityEngine.GameObject.Find("ui_bombHUD").GetComponent$1(UnityEngine.GUITexture.ctor);
        },
        Update: function (){
            if (UnityEngine.Input.GetButtonDown("Fire2") && !this.bombLaid && this.bombCount > 0){
                this.bombCount--;
                this.bombLaid = true;
                UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.bombsAway, this.get_transform().get_position());
                UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.bomb, this.get_transform().get_position(), this.get_transform().get_rotation());
            }
            this.bombHUD.set_enabled(this.bombCount > 0);
        }
    }
};
JsTypes.push(LayBombs);

