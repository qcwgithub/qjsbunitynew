if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var YieldYest = {
    fullname: "YieldYest",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._item = null;
            System.Object.ctor.call(this);
        },
        item$$: "YieldYest+YieldItem",
        get_item: function (){
            return this._item;
        },
        set_item: function (value){
            this._item = value;
        },
        YiedlTest: function* (){
            
            var x = 0;
            this.set_item(new YieldYest.YieldItem.ctor());
            UnityEngine.Debug.Log$$Object("1");
            yield (x++);
            UnityEngine.Debug.Log$$Object("2");
            yield (this.get_item().get_OnGet());
            
        },
        Run: function (){
            var xitem;
            var it = new jsimp.Coroutine.Iterator.ctor(this.YiedlTest());
            while ((xitem = it.MoveNext()) != null){
                this.get_item()._item += 100;
                UnityEngine.Debug.Log$$Object(xitem);
            }
        }
    }
};
JsTypes.push(YieldYest);

