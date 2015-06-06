if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Enemy_Data = {
    fullname: "Enemy_Data",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.moveSpeed = 2;
            this.HP = 2;
            this.deadEnemy = null;
            this.damagedEnemy = null;
            this.deathClips = null;
            this.hundredPointsUI = null;
            this.deathSpinMin = -100;
            this.deathSpinMax = 100;
            UnityEngine.MonoBehaviour.ctor.call(this);
        }
    }
};
JsTypes.push(Enemy_Data);

