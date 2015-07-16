if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var LiuYan = {
    fullname: "LiuYan",
    baseTypeName: "System.Object",
    assemblyName: "SharpKitProj2010",
    interfaceNames: ["IFuckable"],
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        },
        Fuck: function (){
            UnityEngine.Debug.Log$$Object("oh fuck");
        },
        Love: function (){
            UnityEngine.Debug.Log$$Object("oh love");
        }
    }
};
JsTypes.push(LiuYan);

