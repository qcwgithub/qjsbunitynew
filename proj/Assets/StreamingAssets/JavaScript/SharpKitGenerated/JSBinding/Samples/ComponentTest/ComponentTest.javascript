if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ComponentTest = {
    fullname: "ComponentTest",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            var eb = this.GetComponent$1(TEnemyBase.ctor);
            if (UnityEngine.Object.op_Inequality(eb, null)){
                eb.set_enemyName("BULL");
                UnityEngine.Debug.Log$$Object("enemyName = " + eb.get_enemyName());
            }
            else {
                UnityEngine.Debug.Log$$Object("GetComponent<TEnemyBase>() returns null!");
            }
            this.get_gameObject().AddComponent$1(MentosKXT.ctor);
        },
        Update: function (){
        }
    }
};
JsTypes.push(ComponentTest);

