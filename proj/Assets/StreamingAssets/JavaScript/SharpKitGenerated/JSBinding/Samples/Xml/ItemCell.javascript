if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ItemCell = {
    fullname: "ItemCell",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this._ID = 0;
            this._icon = null;
            this._iconAtlas = null;
            this._name = null;
            this._number = 0;
            this._phases = 0;
            this._description = null;
            this._type = 0;
            this._value = 0;
            this._maxStack = 0;
            this._timeLimit = 0;
            this._bSold = false;
            this._bDestoryed = false;
            this._sArtNamePath = null;
            this._color = ColorSign.White;
            this._expSupply = 0;
            System.Object.ctor.call(this);
        },
        ID$$: "System.Int32",
        get_ID: function (){
            return this._ID;
        },
        set_ID: function (value){
            this._ID = value;
        },
        icon$$: "System.String",
        get_icon: function (){
            return this._icon;
        },
        set_icon: function (value){
            this._icon = value;
        },
        iconAtlas$$: "System.String",
        get_iconAtlas: function (){
            return this._iconAtlas;
        },
        set_iconAtlas: function (value){
            this._iconAtlas = value;
        },
        name$$: "System.String",
        get_name: function (){
            return this._name;
        },
        set_name: function (value){
            this._name = value;
        },
        number$$: "System.Int32",
        get_number: function (){
            return this._number;
        },
        set_number: function (value){
            this._number = value;
        },
        phases$$: "System.Int32",
        get_phases: function (){
            return this._phases;
        },
        set_phases: function (value){
            this._phases = value;
        },
        description$$: "System.String",
        get_description: function (){
            return this._description;
        },
        set_description: function (value){
            this._description = value;
        },
        type$$: "System.Int32",
        get_type: function (){
            return this._type;
        },
        set_type: function (value){
            this._type = value;
        },
        value$$: "System.Int32",
        get_value: function (){
            return this._value;
        },
        set_value: function (value){
            this._value = value;
        },
        maxStack$$: "System.Int32",
        get_maxStack: function (){
            return this._maxStack;
        },
        set_maxStack: function (value){
            this._maxStack = value;
        },
        timeLimit$$: "System.Int32",
        get_timeLimit: function (){
            return this._timeLimit;
        },
        set_timeLimit: function (value){
            this._timeLimit = value;
        },
        bSold$$: "System.Boolean",
        get_bSold: function (){
            return this._bSold;
        },
        set_bSold: function (value){
            this._bSold = value;
        },
        bDestoryed$$: "System.Boolean",
        get_bDestoryed: function (){
            return this._bDestoryed;
        },
        set_bDestoryed: function (value){
            this._bDestoryed = value;
        },
        sArtNamePath$$: "System.String",
        get_sArtNamePath: function (){
            return this._sArtNamePath;
        },
        set_sArtNamePath: function (value){
            this._sArtNamePath = value;
        },
        color$$: "ColorSign",
        get_color: function (){
            return this._color;
        },
        set_color: function (value){
            this._color = value;
        },
        expSupply$$: "System.Int32",
        get_expSupply: function (){
            return this._expSupply;
        },
        set_expSupply: function (value){
            this._expSupply = value;
        }
    }
};
JsTypes.push(ItemCell);

