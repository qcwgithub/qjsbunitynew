if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ListTest = {
    fullname: "ListTest",
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
                var lst = new System.Collections.Generic.List$1.ctor(System.Int32.ctor);
                lst.Add(6);
                lst.Add(95);
                var $it8 = lst.GetEnumerator();
                while ($it8.MoveNext()){
                    var v = $it8.get_Current();
                    UnityEngine.Debug.Log$$Object(v);
                }
                var vFind = lst.Find($CreateAnonymousDelegate(this, function (val){
                    return (val == 6);
                }));
                UnityEngine.Debug.Log$$Object("vFind = " + vFind);
            }
        }
    }
};
JsTypes.push(ListTest);

