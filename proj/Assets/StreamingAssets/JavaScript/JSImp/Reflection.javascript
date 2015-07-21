if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Reflection = {
    fullname: "jsimp.Reflection",
    baseTypeName: "System.Object",
    staticDefinition: {
        // 这2个函数，如果T是C#类型，比如说 GameObject，是否仍然有效？
        // 答案是：有点神奇，我认为是有效的！
        // 得测试一下
        CreateInstance$1: function (T){
            //return jsb_CallObjectCtor(T.getNativeType());
            //print("77777777 " + T._type.fullname);
            var ret = new T();
            return ret;
        },
        CreateInstance$$Type: function (type){
            return new type.ctor();
        },
        SetFieldValue: function (obj, fieldName, value){
            if (obj != null) {
                //if (obj.hasOwnProperty(fieldName))
                {
                    obj[fieldName] = value;
                    return true;
                }
            }
            return false;
        },
        GetFieldType: function (type, fieldName){
            if (type != null) {
                var typeStr = type.ctor.prototype[fieldName + "$$"];
                //print(type.fullname + "." + fieldName + " = " + typeStr);
                if (typeStr != undefined) {
                    if (typeStr == "System.Int32[]") {
                        return Int32Array;
                    } else {
                        var fieldType = JsTypeHelper.GetType(typeStr);
                        //print(fieldType.fullname);
                        return fieldType;
                    }
                }
            }
            return null;
        },
        SimpleTEquals$1: function (T, a, b){
            return (a == b);
        },
        TypeIsEnum: function (type){
            return type.Kind == "Enum";
        },
        TypeIsIntArray: function (type){
            return type == Int32Array;
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

// replace old Reflection
jsb_ReplaceOrPushJsType(jsimp$Reflection);