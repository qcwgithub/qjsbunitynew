if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var SampleViewer$stScene = {
    fullname: "SampleViewer.stScene",
    baseTypeName: "System.ValueType",
    assemblyName: "SharpKitProj",
    Kind: "Struct",
    definition: {
        ctor$$String$$String: function (a, b){
            this.levelName = null;
            this.showText = null;
            System.ValueType.ctor.call(this);
            this.levelName = a;
            this.showText = (b.length > 0 ? b : a);
        },
        ctor: function (){
            this.levelName = null;
            this.showText = null;
            System.ValueType.ctor.call(this);
        }
    }
};
JsTypes.push(SampleViewer$stScene);

