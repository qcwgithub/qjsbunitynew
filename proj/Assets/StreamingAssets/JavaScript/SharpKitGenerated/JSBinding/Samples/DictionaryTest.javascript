if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var DictionaryTest = {
    fullname: "DictionaryTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.elapsed = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
        },
        Update: function (){
            this.elapsed += UnityEngine.Time.get_deltaTime();
            if (this.elapsed > 1){
                this.elapsed = 0;
                var dict = new System.Collections.Generic.Dictionary$2.ctor(System.String.ctor, System.Int32.ctor);
                dict.Add("qiucw", 28);
                dict.Add("helj", 27);
                var age;
                if ((function (){
                    var $1 = {
                        Value: age
                    };
                    var $res = dict.TryGetValue("qiucw", $1);
                    age = $1.Value;
                    return $res;
                }).call(this)){
                    UnityEngine.Debug.Log$$Object("age: " + age.toString());
                }
                else {
                    UnityEngine.Debug.Log$$Object("not found");
                }
                var $it1 = dict.GetEnumerator();
                while ($it1.MoveNext()){
                    var v = $it1.get_Current();
                    UnityEngine.Debug.Log$$Object(v.get_Key().toString() + "->" + v.get_Value().toString());
                }
            }
        }
    }
};
JsTypes.push(DictionaryTest);

