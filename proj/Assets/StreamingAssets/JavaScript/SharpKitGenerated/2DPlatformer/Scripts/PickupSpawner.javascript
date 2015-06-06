if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var PickupSpawner = {
    fullname: "PickupSpawner",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.pickups = null;
            this.pickupDeliveryTime = 5;
            this.dropRangeLeft = 0;
            this.dropRangeRight = 0;
            this.highHealthThreshold = 75;
            this.lowHealthThreshold = 25;
            this.playerHealth = null;
            this._pickupDeliveryTime = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.playerHealth = UnityEngine.GameObject.FindGameObjectWithTag("Player").GetComponent$1(PlayerHealth.ctor);
        },
        Start: function (){
            this.Pre_DeliverPickup();
        },
        Update: function (){
            if (this._pickupDeliveryTime > 0){
                this._pickupDeliveryTime -= UnityEngine.Time.get_deltaTime();
                if (this._pickupDeliveryTime <= 0){
                    this.DeliverPickup();
                }
            }
        },
        Pre_DeliverPickup: function (){
            this._pickupDeliveryTime = this.pickupDeliveryTime;
        },
        DeliverPickup: function (){
            var dropPosX = UnityEngine.Random.Range$$Single$$Single(this.dropRangeLeft, this.dropRangeRight);
            var dropPos = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(dropPosX, 15, 1);
            if (this.playerHealth.health >= this.highHealthThreshold)
                UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.pickups[0], dropPos, UnityEngine.Quaternion.get_identity());
            else if (this.playerHealth.health <= this.lowHealthThreshold)
                UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.pickups[1], dropPos, UnityEngine.Quaternion.get_identity());
            else {
                var pickupIndex = UnityEngine.Random.Range$$Int32$$Int32(0, this.pickups.length);
                UnityEngine.Object.Instantiate$$Object$$Vector3$$Quaternion(this.pickups[pickupIndex], dropPos, UnityEngine.Quaternion.get_identity());
            }
        }
    }
};
JsTypes.push(PickupSpawner);

