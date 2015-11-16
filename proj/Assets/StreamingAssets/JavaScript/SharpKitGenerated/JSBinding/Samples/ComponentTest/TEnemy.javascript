if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TEnemy = {
    fullname: "TEnemy",
    baseTypeName: "TEnemyBase",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.pos = new UnityEngine.Vector3.ctor();
            TEnemyBase.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
        }
    }
};
JsTypes.push(TEnemy);

