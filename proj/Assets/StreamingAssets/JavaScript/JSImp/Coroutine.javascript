if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Coroutine = {
    fullname: "jsimp.Coroutine",
    baseTypeName: "System.Object",
    staticDefinition: {
        UpdateMonoBehaviourCoroutine: function (mb){
            mb.$UpdateAllCoroutines(UnityEngine.Time.get_deltaTime());
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

// replace old Coroutine
jsb_ReplaceOrPushJsType(jsimp$Coroutine);

