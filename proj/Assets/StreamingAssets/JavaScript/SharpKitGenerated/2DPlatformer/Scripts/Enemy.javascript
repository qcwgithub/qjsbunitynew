if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Enemy = {
    fullname: "Enemy",
    baseTypeName: "Enemy_Data",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.ren = null;
            this.frontCheck = null;
            this.dead = false;
            this.score = null;
            Enemy_Data.ctor.call(this);
        },
        Awake: function (){
            this.ren = this.get_transform().Find("body").GetComponent$1(UnityEngine.SpriteRenderer.ctor);
            this.frontCheck = this.get_transform().Find("frontCheck").get_transform();
            this.score = UnityEngine.GameObject.Find("Score").GetComponent$1(Score.ctor);
        },
        FixedUpdate: function (){
            var frontHits = UnityEngine.Physics2D.OverlapPointAll$$Vector2$$Int32(UnityEngine.Vector2.op_Implicit$$Vector3(this.frontCheck.get_position()), 1);
            for (var $i3 = 0,$l3 = frontHits.length,c = frontHits[$i3]; $i3 < $l3; $i3++, c = frontHits[$i3]){
                if (c.get_tag() == "Obstacle"){
                    this.Flip();
                    break;
                }
            }
            this.get_rigidbody2D().set_velocity(new UnityEngine.Vector2.ctor$$Single$$Single(this.get_transform().get_localScale().x * this.moveSpeed, this.get_rigidbody2D().get_velocity().y));
            if (this.HP == 1 && UnityEngine.Object.op_Inequality(this.damagedEnemy, null))
                this.ren.set_sprite(this.damagedEnemy);
            if (this.HP <= 0 && !this.dead)
                this.Death();
        },
        Hurt: function (){
            this.HP--;
        },
        Death: function (){
            var otherRenderers = this.GetComponentsInChildren$1(UnityEngine.SpriteRenderer.ctor);
            for (var $i4 = 0,$l4 = otherRenderers.length,s = otherRenderers[$i4]; $i4 < $l4; $i4++, s = otherRenderers[$i4]){
                s.set_enabled(false);
            }
            this.ren.set_enabled(true);
            this.ren.set_sprite(this.deadEnemy);
            this.score.score += 100;
            this.dead = true;
            this.get_rigidbody2D().set_fixedAngle(false);
            this.get_rigidbody2D().AddTorque$$Single(UnityEngine.Random.Range$$Single$$Single(this.deathSpinMin, this.deathSpinMax));
            var cols = this.GetComponents$1(UnityEngine.Collider2D.ctor);
            for (var $i5 = 0,$l5 = cols.length,c = cols[$i5]; $i5 < $l5; $i5++, c = cols[$i5]){
                c.set_isTrigger(true);
            }
            var i = UnityEngine.Random.Range$$Int32$$Int32(0, this.deathClips.length);
            UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.deathClips[i], this.get_transform().get_position());
            var scorePos;
            scorePos = this.get_transform().get_position();
            scorePos.y += 1.5;
            UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.hundredPointsUI, scorePos, UnityEngine.Quaternion.get_identity());
        },
        Flip: function (){
            var enemyScale = this.get_transform().get_localScale();
            enemyScale.x *= -1;
            this.get_transform().set_localScale(enemyScale);
        }
    }
};
JsTypes.push(Enemy);

