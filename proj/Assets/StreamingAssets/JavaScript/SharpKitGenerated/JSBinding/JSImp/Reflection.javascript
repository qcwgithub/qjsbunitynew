if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Reflection = {
    fullname: "jsimp.Reflection",
    baseTypeName: "System.Object",
    staticDefinition: {
        CreateInstance$1: function (T){
            return System.Activator.CreateInstance$1(T);
        },
        SetField: function (obj, fieldName, value){
            if (obj != null){
                var type = obj.GetType();
                var field = type.GetField$$String(fieldName);
                if (System.Reflection.FieldInfo.op_Inequality$$FieldInfo$$FieldInfo(field, null)){
                    field.SetValue$$Object$$Object(obj, value);
                    return true;
                }
            }
            return false;
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
JsTypes.push(jsimp$Reflection);

