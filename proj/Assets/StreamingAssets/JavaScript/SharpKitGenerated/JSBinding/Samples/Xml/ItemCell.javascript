if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ItemCell = {
    fullname: "ItemCell",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.ID = -1;
            this.icon = "";
            this.iconAtlas = "";
            this.name = "";
            this.number = 0;
            this.phases = 0;
            this.description = "";
            this.type = -1;
            this.value = 0;
            this.maxStack = -1;
            this.timeLimit = 0;
            this.bSold = false;
            this.bDestoryed = false;
            this.sArtNamePath = "";
            this.color = 0;
            this.expSupply = 0;
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(ItemCell);

