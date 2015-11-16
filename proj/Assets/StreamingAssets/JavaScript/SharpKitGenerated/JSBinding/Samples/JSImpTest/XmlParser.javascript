if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var XmlParser = {
    fullname: "XmlParser",
    baseTypeName: "System.Object",
    staticDefinition: {
        ComvertType$1: function (T, dict){
            var obj = jsimp.Reflection.CreateInstance$1(T);
            var $it2 = dict.GetEnumerator();
            while ($it2.MoveNext()){
                var ele = $it2.get_Current();
                var fieldName = ele.get_Key();
                var fieldValue = ele.get_Value();
                jsimp.Reflection.SetFieldValue(obj, fieldName, fieldValue);
            }
            return obj;
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
JsTypes.push(XmlParser);

