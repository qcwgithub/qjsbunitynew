if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var EncodingTest = {
    fullname: "EncodingTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    staticDefinition: {
        byteConverString: function (data, index, count){
            var d = System.Text.Encoding.get_UTF8().GetDecoder();
            var arrSize = d.GetCharCount$$Byte$Array$$Int32$$Int32(data, index, count);
            var chars = new Array(arrSize);
            var charSize = d.GetChars$$Byte$Array$$Int32$$Int32$$Char$Array$$Int32(data, index, count, chars, 0);
            var str = new System.String.ctor$$Char$Array$$Int32$$Int32(chars, 0, charSize);
            return str;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var bytes = new Uint8Array([65, 66, 67, 0]);
            var str = EncodingTest.byteConverString(bytes, 0, 3);
            UnityEngine.Debug.Log$$Object(str);
        }
    }
};
JsTypes.push(EncodingTest);

