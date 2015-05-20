if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var PlayerControl = {
    fullname: "PlayerControl",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.facingRight = true;
            this.jump = false;
            this.moveForce = 365;
            this.maxSpeed = 5;
            this.jumpClips = null;
            this.jumpForce = 1000;
            this.taunts = null;
            this.tauntProbability = 50;
            this.tauntDelay = 1;
            this.tauntIndex = 0;
            this.groundCheck = null;
            this.grounded = false;
            this.anim = null;
            this._tauntDelay = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.groundCheck = this.get_transform().Find("groundCheck");
            this.anim = this.GetComponent$1(UnityEngine.Animator.ctor);
        },
        FixedUpdate: function (){
            var h = UnityEngine.Input.GetAxis("Horizontal");
            this.anim.SetFloat$$String$$Single("Speed", UnityEngine.Mathf.Abs$$Single(h));
            if (h * this.get_rigidbody2D().get_velocity().x < this.maxSpeed)
                this.get_rigidbody2D().AddForce$$Vector2(UnityEngine.Vector2.op_Multiply$$Vector2$$Single(UnityEngine.Vector2.op_Multiply$$Vector2$$Single(UnityEngine.Vector2.get_right(), h), this.moveForce));
            if (UnityEngine.Mathf.Abs$$Single(this.get_rigidbody2D().get_velocity().x) > this.maxSpeed)
                this.get_rigidbody2D().set_velocity(new UnityEngine.Vector2.ctor$$Single$$Single(UnityEngine.Mathf.Sign(this.get_rigidbody2D().get_velocity().x) * this.maxSpeed, this.get_rigidbody2D().get_velocity().y));
            if (h > 0 && !this.facingRight)
                this.Flip();
            else if (h < 0 && this.facingRight)
                this.Flip();
            if (this.jump){
                this.anim.SetTrigger$$String("Jump");
                var i = UnityEngine.Random.Range$$Int32$$Int32(0, this.jumpClips.length);
                UnityEngine.AudioSource.PlayClipAtPoint$$AudioClip$$Vector3(this.jumpClips[i], this.get_transform().get_position());
                this.get_rigidbody2D().AddForce$$Vector2(new UnityEngine.Vector2.ctor$$Single$$Single(0, this.jumpForce));
                this.jump = false;
            }
        },
        Flip: function (){
            this.facingRight = !this.facingRight;
            var theScale = this.get_transform().get_localScale();
            theScale.x *= -1;
            this.get_transform().set_localScale(theScale);
        },
        Update: function (){
            this.grounded = UnityEngine.RaycastHit2D.op_Implicit(UnityEngine.Physics2D.Linecast$$Vector2$$Vector2$$Int32(UnityEngine.Vector2.op_Implicit$$Vector3(this.get_transform().get_position()), UnityEngine.Vector2.op_Implicit$$Vector3(this.groundCheck.get_position()), 1 << UnityEngine.LayerMask.NameToLayer("Ground")));
            if (UnityEngine.Input.GetButtonDown("Jump") && this.grounded)
                this.jump = true;
            if (this._tauntDelay > 0){
                this._tauntDelay -= UnityEngine.Time.get_deltaTime();
                if (this._tauntDelay <= 0){
                    this.Taunt();
                }
            }
        },
        PreTaunt: function (){
            var tauntChance = UnityEngine.Random.Range$$Single$$Single(0, 100);
            if (tauntChance > this.tauntProbability){
                this._tauntDelay = this.tauntDelay;
            }
        },
        Taunt: function (){
            if (!this.get_audio().get_isPlaying()){
                this.tauntIndex = this.TauntRandom();
                this.get_audio().set_clip(this.taunts[this.tauntIndex]);
                this.get_audio().Play();
            }
        },
        TauntRandom: function (){
            var i = UnityEngine.Random.Range$$Int32$$Int32(0, this.taunts.length);
            if (i == this.tauntIndex)
                return this.TauntRandom();
            else
                return i;
        }
    }
};
JsTypes.push(PlayerControl);

