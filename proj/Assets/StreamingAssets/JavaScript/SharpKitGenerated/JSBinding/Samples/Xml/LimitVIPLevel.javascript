if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var LimitVIPLevel = {
    fullname: "LimitVIPLevel",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Level = 0;
            System.Object.ctor.call(this);
        },
        Level$$: "System.Int32",
        get_Level: function (){
            return this._Level;
        },
        set_Level: function (value){
            this._Level = value;
        }
    }
};
JsTypes.push(LimitVIPLevel);

