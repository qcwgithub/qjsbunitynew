if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var jsimp$Misc = {
    fullname: "jsimp.Misc",
    baseTypeName: "System.Object",
    staticDefinition: {
        string_Replace: function (str, str1, str2){
            return str.replace(str1, str2);
        },
        string_Split: function (str, sep){
            return str.split(sep);
        },
        string_Split_RemoveEmptyEntries: function (str, sep){
            return str.split(sep);
        },
        Floor$$Int32: function (i){
            return Math.floor(i);
        },
        Floor$$Single: function (i){
            return Math.floor(i);
        },
        List_AddRange$1: function (T, lst, collection){
            var L = collection.length;
for (var i = 0; i < L; i++)
{
    lst.Add(collection[i]);
}
        },
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
JsTypes.push(jsimp$Misc);

