if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var LimitBuyNumPrices = {
    fullname: "LimitBuyNumPrices",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    interfaceNames: ["IShopLime"],
    Kind: "Class",
    definition: {
        ctor: function (){
            this._Prices = null;
            System.Object.ctor.call(this);
        },
        Prices$$: "System.Int32[]",
        get_Prices: function (){
            return this._Prices;
        },
        set_Prices: function (value){
            this._Prices = value;
        }
    }
};
JsTypes.push(LimitBuyNumPrices);

