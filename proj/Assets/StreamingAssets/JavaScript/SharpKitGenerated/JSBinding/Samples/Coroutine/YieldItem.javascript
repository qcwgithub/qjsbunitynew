if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var YieldYest$YieldItem = {
    fullname: "YieldYest.YieldItem",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._item = 0;
            System.Object.ctor.call(this);
        },
        OnGet$$: "System.Int32",
        get_OnGet: function (){
            UnityEngine.Debug.Log$$Object("Build");
            if (this._item <= 0){
                this._item = 10;
            }
            return this._item;
        }
    }
};
JsTypes.push(YieldYest$YieldItem);

