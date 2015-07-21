if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ShopItemData = {
    fullname: "ShopItemData",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._ID = null;
            this._ItemType = ItemType.Normal;
            this._ItemId = null;
            this._Currency = enumMoneyType.None;
            this._OldPrice = 0;
            this._CurPrice = 0;
            this._Recommend = false;
            this._RefreshRate = 0;
            this._Globe = 0;
            this._IsPack = false;
            this._ItemNum = 0;
            this._item = null;
            this._TimeNum = null;
            this._VipDayNum = null;
            this._BuyNumPrice = null;
            this._VIPLevel = null;
            this._DayNum = null;
            this._hadBuyNum = 0;
            this._maxBuyNum = 0;
            this._ShopCategory = 0;
            System.Object.ctor.call(this);
        },
        ID$$: "System.String",
        get_ID: function (){
            return this._ID;
        },
        set_ID: function (value){
            this._ID = value;
        },
        ItemType$$: "ItemType",
        get_ItemType: function (){
            return this._ItemType;
        },
        set_ItemType: function (value){
            this._ItemType = value;
        },
        ItemId$$: "System.String",
        get_ItemId: function (){
            return this._ItemId;
        },
        set_ItemId: function (value){
            this._ItemId = value;
        },
        Currency$$: "enumMoneyType",
        get_Currency: function (){
            return this._Currency;
        },
        set_Currency: function (value){
            this._Currency = value;
        },
        OldPrice$$: "System.Int32",
        get_OldPrice: function (){
            return this._OldPrice;
        },
        set_OldPrice: function (value){
            this._OldPrice = value;
        },
        CurPrice$$: "System.Int32",
        get_CurPrice: function (){
            return this._CurPrice;
        },
        set_CurPrice: function (value){
            this._CurPrice = value;
        },
        Recommend$$: "System.Boolean",
        get_Recommend: function (){
            return this._Recommend;
        },
        set_Recommend: function (value){
            this._Recommend = value;
        },
        RefreshRate$$: "System.Single",
        get_RefreshRate: function (){
            return this._RefreshRate;
        },
        set_RefreshRate: function (value){
            this._RefreshRate = value;
        },
        Globe$$: "System.Int32",
        get_Globe: function (){
            return this._Globe;
        },
        set_Globe: function (value){
            this._Globe = value;
        },
        IsPack$$: "System.Boolean",
        get_IsPack: function (){
            return this._IsPack;
        },
        set_IsPack: function (value){
            this._IsPack = value;
        },
        ItemNum$$: "System.Int32",
        get_ItemNum: function (){
            return this._ItemNum;
        },
        set_ItemNum: function (value){
            this._ItemNum = value;
        },
        item$$: "ItemCell",
        get_item: function (){
            return this._item;
        },
        set_item: function (value){
            this._item = value;
        },
        TimeNum$$: "LimeTimeNum",
        get_TimeNum: function (){
            return this._TimeNum;
        },
        set_TimeNum: function (value){
            this._TimeNum = value;
        },
        VipDayNum$$: "LimitVipDayNum",
        get_VipDayNum: function (){
            return this._VipDayNum;
        },
        set_VipDayNum: function (value){
            this._VipDayNum = value;
        },
        BuyNumPrice$$: "LimitBuyNumPrices",
        get_BuyNumPrice: function (){
            return this._BuyNumPrice;
        },
        set_BuyNumPrice: function (value){
            this._BuyNumPrice = value;
        },
        VIPLevel$$: "LimitVIPLevel",
        get_VIPLevel: function (){
            return this._VIPLevel;
        },
        set_VIPLevel: function (value){
            this._VIPLevel = value;
        },
        DayNum$$: "LimitDayNum",
        get_DayNum: function (){
            return this._DayNum;
        },
        set_DayNum: function (value){
            this._DayNum = value;
        },
        hadBuyNum$$: "System.Int32",
        get_hadBuyNum: function (){
            return this._hadBuyNum;
        },
        set_hadBuyNum: function (value){
            this._hadBuyNum = value;
        },
        maxBuyNum$$: "System.Int32",
        get_maxBuyNum: function (){
            return this._maxBuyNum;
        },
        set_maxBuyNum: function (value){
            this._maxBuyNum = value;
        },
        ShopCategory$$: "System.Int32",
        get_ShopCategory: function (){
            return this._ShopCategory;
        },
        set_ShopCategory: function (value){
            this._ShopCategory = value;
        },
        Clear: function (){
            this.set_item(null);
            this.set_TimeNum(null);
            this.set_VipDayNum(null);
            this.set_BuyNumPrice(null);
        }
    }
};
JsTypes.push(ShopItemData);

