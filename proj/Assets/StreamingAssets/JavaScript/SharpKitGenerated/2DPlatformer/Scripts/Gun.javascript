if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Gun = {
    fullname: "Gun",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.rocketGO = null;
            this.speed = 20;
            this.playerCtrl = null;
            this.anim = null;
            this.rocket = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.anim = this.get_transform().get_root().get_gameObject().GetComponent$1(UnityEngine.Animator.ctor);
            this.playerCtrl = this.get_transform().get_root().GetComponent$1(PlayerControl.ctor);
            this.rocket = this.rocketGO.get_rigidbody2D();
        },
        Update: function (){
            if (UnityEngine.Input.GetButtonDown("Fire1")){
                this.anim.SetTrigger$$String("Shoot");
                this.get_audio().Play();
                if (this.playerCtrl.facingRight){
                    var bulletInstance = As(UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.rocket, this.get_transform().get_position(), UnityEngine.Quaternion.Euler$$Vector3(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(0, 0, 0))), UnityEngine.Rigidbody2D.ctor);
                    bulletInstance.set_velocity(new UnityEngine.Vector2.ctor$$Single$$Single(this.speed, 0));
                }
                else {
                    var bulletInstance = As(UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.rocket, this.get_transform().get_position(), UnityEngine.Quaternion.Euler$$Vector3(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(0, 0, 180))), UnityEngine.Rigidbody2D.ctor);
                    bulletInstance.set_velocity(new UnityEngine.Vector2.ctor$$Single$$Single(-this.speed, 0));
                }
            }
        }
    }
};
JsTypes.push(Gun);

