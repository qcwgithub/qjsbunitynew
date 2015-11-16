if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Coroutine = {
    fullname: "jsimp.Coroutine",
    baseTypeName: "System.Object",
    staticDefinition: {
        UpdateCoroutineAndInvoke: function (mb){
            var elapsed = UnityEngine.Time.get_deltaTime();
            mb.$UpdateAllCoroutines(elapsed);
            mb.$UpdateAllInvokes(elapsed);
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
JsTypes.push(jsimp$Coroutine);

