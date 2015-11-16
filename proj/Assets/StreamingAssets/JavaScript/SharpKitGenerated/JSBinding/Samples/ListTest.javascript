if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ListTest = {
    fullname: "ListTest",
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
                var lst = new System.Collections.Generic.List$1.ctor(System.Int32.ctor);
                lst.Add(6);
                lst.Add(95);
                var $it3 = lst.GetEnumerator();
                while ($it3.MoveNext()){
                    var v = $it3.get_Current();
                    UnityEngine.Debug.Log$$Object(v);
                }
                var vFind = lst.Find($CreateAnonymousDelegate(this, function (val){
                    return (val == 6);
                }));
                UnityEngine.Debug.Log$$Object("vFind = " + vFind);
                var lstS = lst.ConvertAll$1(System.String.ctor, $CreateAnonymousDelegate(this, function (v){
                    return "s: " + v;
                }));
                var $it4 = lstS.GetEnumerator();
                while ($it4.MoveNext()){
                    var v = $it4.get_Current();
                    UnityEngine.Debug.Log$$Object(v);
                }
                UnityEngine.Debug.Log$$Object(lstS.get_Item$$Int32(0));
                UnityEngine.Debug.Log$$Object(lstS.get_Item$$Int32(1));
            }
        }
    }
};
JsTypes.push(ListTest);

