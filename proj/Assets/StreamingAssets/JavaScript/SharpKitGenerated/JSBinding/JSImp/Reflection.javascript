if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Reflection = {
    fullname: "jsimp.Reflection",
    baseTypeName: "System.Object",
    staticDefinition: {
        CreateInstance$1: function (T){
            return System.Activator.CreateInstance$1(T);
        },
        CreateInstance$$Type: function (type){
            return System.Activator.CreateInstance$$Type(type);
        },
        SetFieldValue: function (obj, fieldName, value){
            if (obj != null){
                var type = obj.GetType();
                var field = type.GetField$$String(fieldName);
                if (System.Reflection.FieldInfo.op_Inequality$$FieldInfo$$FieldInfo(field, null)){
                    field.SetValue$$Object$$Object(obj, value);
                    return true;
                }
            }
            return false;
        },
        GetFieldType: function (type, fieldName){
            if (type != null){
                var field = type.GetField$$String(fieldName);
                if (System.Reflection.FieldInfo.op_Inequality$$FieldInfo$$FieldInfo(field, null)){
                    return field.get_FieldType();
                }
            }
            return null;
        },
        SetPropertyValue: function (obj, propertyName, value){
            if (obj != null){
                var type = obj.GetType();
                var property = type.GetProperty$$String(propertyName);
                if (System.Reflection.PropertyInfo.op_Inequality$$PropertyInfo$$PropertyInfo(property, null)){
                    property.SetValue$$Object$$Object$$Object$Array(obj, value, null);
                    return true;
                }
            }
            return false;
        },
        GetPropertyType: function (type, propertyName){
            if (type != null){
                var property = type.GetProperty$$String(propertyName);
                if (System.Reflection.PropertyInfo.op_Inequality$$PropertyInfo$$PropertyInfo(property, null)){
                    return property.get_PropertyType();
                }
            }
            return null;
        },
        PropertyTypeIsIntArray: function (type, propertyName){
            if (type != null){
                var property = type.GetProperty$$String(propertyName);
                if (System.Reflection.PropertyInfo.op_Inequality$$PropertyInfo$$PropertyInfo(property, null)){
                    return (property.get_PropertyType() == Typeof(Int32Array));
                }
            }
            return false;
        },
        SimpleTEquals$1: function (T, a, b){
            return a.Equals$$Object(b);
        },
        TypeIsIntArray: function (type){
            return type == Typeof(Int32Array);
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

