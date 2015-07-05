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
            return new T();
        },
        SetField: function (obj, fieldName, value){
            if (obj != null) {
                if (obj.hasOwnProperty(fieldName)) {
                    obj[fieldName] = value;
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

// replace old Reflection
jsb_ReplaceOrPushJsType(jsimp$Reflection);