if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var HealthPickup = {
    fullname: "HealthPickup",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.healthBonus = 0;
            this.collect = null;
            this.pickupSpawner = null;
            this.anim = null;
            this.landed = false;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.pickupSpawner = UnityEngine.GameObject.Find("pickupManager").GetComponent$1(PickupSpawner.ctor);
            this.anim = this.get_transform().get_root().GetComponent$1(UnityEngine.Animator.ctor);
        },
        OnTriggerEnter2D: function (other){
            if (other.get_tag() == "Player"){
                var playerHealth = other.GetComponent$1(PlayerHealth.ctor);
                playerHealth.health += this.healthBonus;
                playerHealth.health = UnityEngine.Mathf.Clamp$$Single$$Single$$Single(playerHealth.health, 0, 100);
                playerHealth.UpdateHealthBar();
                this.pickupSpawner.Pre_DeliverPickup();
                UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.collect, this.get_transform().get_position());
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
JsTypes.push(HealthPickup);

