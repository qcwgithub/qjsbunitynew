if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Coroutine$Iterator = {
    fullname: "jsimp.Coroutine.Iterator",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj2010",
    Kind: "Class",
    definition: {
        ctor: function (ie){
            this.ie = null;
            System.Object.ctor.call(this);
            this.ie = ie;
        },
        MoveNext: function (){
            //if (this.ie.MoveNext())
            //    return this.ie.get_Current();
            var obj = this.ie.next();
            if (!obj.done)
                return obj.value;
            return null;
        }
    }
};
jsb_ReplaceOrPushJsType(jsimp$Coroutine$Iterator);

