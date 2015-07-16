if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ShopItemData = {
    fullname: "ShopItemData",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj2010",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.ID = null;
            this.ItemType = 1;
            this.ItemId = null;
            this.Currency = 0;
            this.OldPrice = 0;
            this.CurPrice = 0;
            this.Recommend = false;
            this.RefreshRate = 0;
            this.Globe = 0;
            this.IsPack = false;
            this.ItemNum = 0;
            this.item = null;
            this.TimeNum = null;
            this.VipDayNum = null;
            this.BuyNumPrice = null;
            this.VIPLevel = null;
            this.DayNum = null;
            this.hadBuyNum = 0;
            this.maxBuyNum = -1;
            this.ShopCategory = 0;
            System.Object.ctor.call(this);
        },
        Clear: function (){
            this.item = null;
            this.TimeNum = null;
            this.VipDayNum = null;
            this.BuyNumPrice = null;
        }
    }
};
JsTypes.push(ShopItemData);

