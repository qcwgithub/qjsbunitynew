if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var JSImpTest1 = {
    fullname: "JSImpTest1",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            if (UnityEngine.Input.GetMouseButtonDown(0)){
                var dict = new System.Collections.Generic.Dictionary$2.ctor(System.String.ctor, System.String.ctor);
                dict.Add("firstName", "De Hua");
                dict.Add("lastName", "Liu");
                dict.Add("age", "55");
                var person = XmlParser.ComvertType$1(JSImpTest1.Person.ctor, dict);
                UnityEngine.Debug.Log$$Object(new System.Text.StringBuilder.ctor().AppendFormat$$String$$Object$$Object$$Object("{0} {1}, {2}", person.lastName, person.firstName, person.age));
            }
        }
    }
};
JsTypes.push(JSImpTest1);

