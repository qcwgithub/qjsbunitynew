if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var MiscTest = {
    fullname: "MiscTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            PerTest.testString(null, "abc");
        },
        PrintStrings: function (s, strs){
            for (var $i6 = 0,$l6 = strs.length,v = strs[$i6]; $i6 < $l6; $i6++, v = strs[$i6])
                UnityEngine.MonoBehaviour.print(v);
        }
    }
};
JsTypes.push(MiscTest);

