if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ComponentExtension = {
    fullname: "ComponentExtension",
    baseTypeName: "System.Object",
    staticDefinition: {
        GetComponentI$1$$Component: function (T, com){
            return com.GetComponent$1(T);
        },
        GetComponentI$1$$GameObject: function (T, go){
            return go.GetComponent$1(T);
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(ComponentExtension);

