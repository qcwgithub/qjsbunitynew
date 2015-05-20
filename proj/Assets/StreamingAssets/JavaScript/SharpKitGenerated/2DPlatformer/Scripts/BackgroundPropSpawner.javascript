if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var BackgroundPropSpawner = {
    fullname: "BackgroundPropSpawner",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.backgroundProp = null;
            this.leftSpawnPosX = 0;
            this.rightSpawnPosX = 0;
            this.minSpawnPosY = 0;
            this.maxSpawnPosY = 0;
            this.minTimeBetweenSpawns = 0;
            this.maxTimeBetweenSpawns = 0;
            this.minSpeed = 0;
            this.maxSpeed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.Random.set_seed(System.DateTime.get_Today().get_Millisecond());
            this.StartCoroutine$$String("Spawn");
        },
        Spawn: function (){
            var $yield = [];
            var waitTime = UnityEngine.Random.Range$$Single$$Single(this.minTimeBetweenSpawns, this.maxTimeBetweenSpawns);
            $yield.push(new UnityEngine.WaitForSeconds.ctor(waitTime));
            var facingLeft = UnityEngine.Random.Range$$Int32$$Int32(0, 2) == 0;
            var posX = facingLeft ? this.rightSpawnPosX : this.leftSpawnPosX;
            var posY = UnityEngine.Random.Range$$Single$$Single(this.minSpawnPosY, this.maxSpawnPosY);
            var spawnPos = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(posX, posY, this.get_transform().get_position().z);
            var propInstance = As(UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.backgroundProp, spawnPos, UnityEngine.Quaternion.get_identity()), UnityEngine.Rigidbody2D.ctor);
            if (!facingLeft){
                var scale = propInstance.get_transform().get_localScale();
                scale.x *= -1;
                propInstance.get_transform().set_localScale(scale);
            }
            var speed = UnityEngine.Random.Range$$Single$$Single(this.minSpeed, this.maxSpeed);
            speed *= facingLeft ? -1 : 1;
            propInstance.set_velocity(new UnityEngine.Vector2.ctor$$Single$$Single(speed, 0));
            this.StartCoroutine$$IEnumerator(this.Spawn());
            while (UnityEngine.Object.op_Inequality(propInstance, null)){
                if (facingLeft){
                    if (propInstance.get_transform().get_position().x < this.leftSpawnPosX - 0.5)
                        UnityEngine.Object.Destroy$$Object(propInstance.get_gameObject());
                }
                else {
                    if (propInstance.get_transform().get_position().x > this.rightSpawnPosX + 0.5)
                        UnityEngine.Object.Destroy$$Object(propInstance.get_gameObject());
                }
                $yield.push(null);
            }
            return $yield;
        }
    }
};
JsTypes.push(BackgroundPropSpawner);

