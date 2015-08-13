if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var TEnemyBase = {
    fullname: "TEnemyBase",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._enemyID = 0;
            this._enemyName = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        enemyID$$: "System.Int32",
        get_enemyID: function (){
            return this._enemyID;
        },
        set_enemyID: function (value){
            this._enemyID = value;
        },
        enemyName$$: "System.String",
        get_enemyName: function (){
            return this._enemyName;
        },
        set_enemyName: function (value){
            this._enemyName = value;
        },
        Start: function (){
        },
        Update: function (){
        }
    }
};
JsTypes.push(TEnemyBase);

