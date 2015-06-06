if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Rocket = {
    fullname: "Rocket",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.explosion = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Object.Destroy$$Object$$Single(this.get_gameObject(), 2);
        },
        OnExplode: function (){
            var randomRotation = UnityEngine.Quaternion.Euler$$Single$$Single$$Single(0, 0, UnityEngine.Random.Range$$Single$$Single(0, 360));
            UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.explosion, this.get_transform().get_position(), randomRotation);
        },
        OnTriggerEnter2D: function (col){
            if (col.get_tag() == "Enemy"){
                col.get_gameObject().GetComponent$1(Enemy.ctor).Hurt();
                this.OnExplode();
                UnityEngine.Object.Destroy$$Object(this.get_gameObject());
            }
            else if (col.get_tag() == "BombPickup"){
                col.get_gameObject().GetComponent$1(Bomb.ctor).Explode();
                UnityEngine.Object.Destroy$$Object(col.get_transform().get_root().get_gameObject());
                UnityEngine.Object.Destroy$$Object(this.get_gameObject());
            }
            else if (col.get_gameObject().get_tag() != "Player"){
                this.OnExplode();
                UnityEngine.Object.Destroy$$Object(this.get_gameObject());
            }
        }
    }
};
JsTypes.push(Rocket);

