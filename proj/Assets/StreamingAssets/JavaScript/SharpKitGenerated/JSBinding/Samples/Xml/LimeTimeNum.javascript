if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var LimeTimeNum = {
    fullname: "LimeTimeNum",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    interfaceNames: ["IShopLime"],
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Interval = 0;
            this._Count = 0;
            System.Object.ctor.call(this);
        },
        Interval$$: "System.Int32",
        get_Interval: function (){
            return this._Interval;
        },
        set_Interval: function (value){
            this._Interval = value;
        },
        Count$$: "System.Int32",
        get_Count: function (){
            return this._Count;
        },
        set_Count: function (value){
            this._Count = value;
        }
    }
};
JsTypes.push(LimeTimeNum);

