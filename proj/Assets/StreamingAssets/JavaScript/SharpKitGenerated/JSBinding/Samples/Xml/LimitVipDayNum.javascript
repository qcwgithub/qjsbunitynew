if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var LimitVipDayNum = {
    fullname: "LimitVipDayNum",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    interfaceNames: ["IShopLime"],
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Count = null;
            System.Object.ctor.call(this);
        },
        Count$$: "System.Int32[]",
        get_Count: function (){
            return this._Count;
        },
        set_Count: function (value){
            this._Count = value;
        }
    }
};
JsTypes.push(LimitVipDayNum);

