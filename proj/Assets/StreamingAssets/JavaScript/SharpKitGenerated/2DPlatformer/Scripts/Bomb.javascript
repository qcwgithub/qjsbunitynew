if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Bomb = {
    fullname: "Bomb",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.bombRadius = 10;
            this.bombForce = 100;
            this.boom = null;
            this.fuse = null;
            this.fuseTime = 1.5;
            this.explosion = null;
            this.layBombs = null;
            this.pickupSpawner = null;
            this.explosionFX = null;
            this._fuseTime = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.explosionFX = UnityEngine.GameObject.FindGameObjectWithTag("ExplosionFX").GetComponent$1(UnityEngine.ParticleSystem.ctor);
            this.pickupSpawner = UnityEngine.GameObject.Find("pickupManager").GetComponent$1(PickupSpawner.ctor);
            if (UnityEngine.Object.op_Implicit(UnityEngine.GameObject.FindGameObjectWithTag("Player")))
                this.layBombs = UnityEngine.GameObject.FindGameObjectWithTag("Player").GetComponent$1(LayBombs.ctor);
        },
        Start: function (){
            if (UnityEngine.Object.op_Equality(this.get_transform().get_root(), this.get_transform())){
                this.Pre_BombDetonation();
            }
        },
        Update: function (){
            if (this._fuseTime > 0){
                this._fuseTime -= UnityEngine.Time.get_deltaTime();
                if (this._fuseTime <= 0){
                    this.BombDetonation();
                }
            }
        },
        Pre_BombDetonation: function (){
            UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.fuse, this.get_transform().get_position());
            this._fuseTime = this.fuseTime;
        },
        BombDetonation: function (){
            this.Explode();
        },
        Explode: function (){
            this.layBombs.bombLaid = false;
            this.pickupSpawner.Pre_DeliverPickup();
            var enemies = UnityEngine.Physics2D.OverlapCircleAll$$Vector2$$Single$$Int32(UnityEngine.Vector2.op_Implicit$$Vector3(this.get_transform().get_position()), this.bombRadius, 1 << UnityEngine.LayerMask.NameToLayer("Enemies"));
            for (var $i2 = 0,$l2 = enemies.length,en = enemies[$i2]; $i2 < $l2; $i2++, en = enemies[$i2]){
                var rb = en.get_rigidbody2D();
                if (UnityEngine.Object.op_Inequality(rb, null) && rb.get_tag() == "Enemy"){
                    rb.get_gameObject().GetComponent$1(Enemy.ctor).HP = 0;
                    var deltaPos = UnityEngine.Vector3.op_Subtraction(rb.get_transform().get_position(), this.get_transform().get_position());
                    var force = UnityEngine.Vector3.op_Multiply$$Vector3$$Single(deltaPos.get_normalized(), this.bombForce);
                    rb.AddForce$$Vector2(UnityEngine.Vector2.op_Implicit$$Vector3(force));
                }
            }
            this.explosionFX.get_transform().set_position(this.get_transform().get_position());
            this.explosionFX.Play();
            UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.explosion, this.get_transform().get_position(), UnityEngine.Quaternion.get_identity());
            UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.boom, this.get_transform().get_position());
            UnityEngine.Object.Destroy$$Object(this.get_gameObject());
        }
    }
};
JsTypes.push(Bomb);

