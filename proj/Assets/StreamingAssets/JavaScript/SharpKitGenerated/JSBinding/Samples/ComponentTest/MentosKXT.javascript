if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var MentosKXT = {
    fullname: "MentosKXT",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.bUpdatePrinted = false;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Start: function (){
            UnityEngine.MonoBehaviour.print("Hello this is MentosKXT Start()");
        },
        Update: function (){
            if (!this.bUpdatePrinted){
                this.bUpdatePrinted = true;
                UnityEngine.MonoBehaviour.print("Hello this is MentosKXT Update()");
            }
        }
    }
};
JsTypes.push(MentosKXT);

