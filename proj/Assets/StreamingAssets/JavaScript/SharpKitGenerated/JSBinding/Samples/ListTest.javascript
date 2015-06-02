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
                var lstS = lst.ConvertAll$1(System.String.ctor, $CreateAnonymousDelegate(this, function (v){
                    return "s: " + v;
                }));
                var $it9 = lstS.GetEnumerator();
                while ($it9.MoveNext()){
                    var v = $it9.get_Current();
                    UnityEngine.Debug.Log$$Object(v);
                }
                UnityEngine.Debug.Log$$Object(lstS.get_Item$$Int32(0));
                UnityEngine.Debug.Log$$Object(lstS.get_Item$$Int32(1));
            }
        }
    }
};
JsTypes.push(ListTest);

