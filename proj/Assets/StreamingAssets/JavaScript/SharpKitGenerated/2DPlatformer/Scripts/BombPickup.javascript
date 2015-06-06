if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var BombPickup = {
    fullname: "BombPickup",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.pickupClip = null;
            this.anim = null;
            this.landed = false;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.anim = this.get_transform().get_root().GetComponent$1(UnityEngine.Animator.ctor);
        },
        OnTriggerEnter2D: function (other){
            if (other.get_tag() == "Player"){
                UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.pickupClip, this.get_transform().get_position());
                other.GetComponent$1(LayBombs.ctor).bombCount++;
                UnityEngine.Object.Destroy$$Object(this.get_transform().get_root().get_gameObject());
            }
            else if (other.get_tag() == "ground" && !this.landed){
                this.anim.SetTrigger$$String("Land");
                this.get_transform().set_parent(null);
                this.get_gameObject().AddComponent$1(UnityEngine.Rigidbody2D.ctor);
                this.landed = true;
            }
        }
    }
};
JsTypes.push(BombPickup);

