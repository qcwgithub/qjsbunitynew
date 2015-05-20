if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var PlayerHealth = {
    fullname: "PlayerHealth",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.health = 100;
            this.repeatDamagePeriod = 2;
            this.ouchClips = null;
            this.hurtForce = 10;
            this.damageAmount = 10;
            this.healthBar = null;
            this.lastHitTime = 0;
            this.healthScale = new UnityEngine.Vector3.ctor();
            this.playerControl = null;
            this.anim = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.playerControl = this.GetComponent$1(PlayerControl.ctor);
            this.healthBar = UnityEngine.GameObject.Find("HealthBar").GetComponent$1(UnityEngine.SpriteRenderer.ctor);
            this.anim = this.GetComponent$1(UnityEngine.Animator.ctor);
            this.healthScale = this.healthBar.get_transform().get_localScale();
        },
        OnCollisionEnter2D: function (col){
            if (col.get_gameObject().get_tag() == "Enemy"){
                if (UnityEngine.Time.get_time() > this.lastHitTime + this.repeatDamagePeriod){
                    if (this.health > 0){
                        this.TakeDamage(col.get_transform());
                        this.lastHitTime = UnityEngine.Time.get_time();
                    }
                    else {
                        var cols = this.GetComponents$1(UnityEngine.Collider2D.ctor);
                        for (var $i6 = 0,$l6 = cols.length,c = cols[$i6]; $i6 < $l6; $i6++, c = cols[$i6]){
                            c.set_isTrigger(true);
                        }
                        var spr = this.GetComponentsInChildren$1(UnityEngine.SpriteRenderer.ctor);
                        for (var $i7 = 0,$l7 = spr.length,s = spr[$i7]; $i7 < $l7; $i7++, s = spr[$i7]){
                            s.set_sortingLayerName("UI");
                        }
                        this.GetComponent$1(PlayerControl.ctor).set_enabled(false);
                        this.GetComponentInChildren$1(Gun.ctor).set_enabled(false);
                        this.anim.SetTrigger$$String("Die");
                    }
                }
            }
        },
        TakeDamage: function (enemy){
            this.playerControl.jump = false;
            var hurtVector = UnityEngine.Vector3.op_Addition(UnityEngine.Vector3.op_Subtraction(this.get_transform().get_position(), enemy.get_position()), UnityEngine.Vector3.op_Multiply$$Vector3$$Single(UnityEngine.Vector3.get_up(), 5));
            this.get_rigidbody2D().AddForce$$Vector2(UnityEngine.Vector2.op_Implicit$$Vector3(UnityEngine.Vector3.op_Multiply$$Vector3$$Single(hurtVector, this.hurtForce)));
            this.health -= this.damageAmount;
            this.UpdateHealthBar();
            var i = UnityEngine.Random.Range$$Int32$$Int32(0, this.ouchClips.length);
            UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.ouchClips[i], this.get_transform().get_position());
        },
        UpdateHealthBar: function (){
            this.healthBar.get_material().set_color(UnityEngine.Color.Lerp(UnityEngine.Color.get_green(), UnityEngine.Color.get_red(), 1 - this.health * 0.01));
            this.healthBar.get_transform().set_localScale(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(this.healthScale.x * this.health * 0.01, 1, 1));
        }
    }
};
JsTypes.push(PlayerHealth);

