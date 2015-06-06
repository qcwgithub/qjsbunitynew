if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Spawner = {
    fullname: "Spawner",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.spawnTime = 5;
            this.spawnDelay = 3;
            this.enemies = null;
            this._spawnTime = 0;
            this._spawnDelay = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            this._spawnTime = this.spawnTime;
            this._spawnDelay = this.spawnDelay;
        },
        Update: function (){
            if (this._spawnDelay > 0){
                this._spawnDelay -= UnityEngine.Time.get_deltaTime();
                return;
            }
            if (this._spawnTime > 0){
                this._spawnTime -= UnityEngine.Time.get_deltaTime();
                if (this._spawnTime <= 0){
                    this._spawnTime = this.spawnTime;
                    this.Spawn();
                }
            }
        },
        Spawn: function (){
            var enemyIndex = UnityEngine.Random.Range$$Int32$$Int32(0, this.enemies.length);
            UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.enemies[enemyIndex], this.get_transform().get_position(), this.get_transform().get_rotation());
            for (var $i8 = 0,$t8 = this.GetComponentsInChildren$1(UnityEngine.ParticleSystem.ctor),$l8 = $t8.length,p = $t8[$i8]; $i8 < $l8; $i8++, p = $t8[$i8]){
                p.Play();
            }
        }
    }
};
JsTypes.push(Spawner);

