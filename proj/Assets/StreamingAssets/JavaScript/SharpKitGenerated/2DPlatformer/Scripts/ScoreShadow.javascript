if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var ScoreShadow = {
    fullname: "ScoreShadow",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.guiCopy = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            var behindPos = this.get_transform().get_position();
            behindPos = new UnityEngine.Vector3.ctor$$Single$$Single$$Single(this.guiCopy.get_transform().get_position().x, this.guiCopy.get_transform().get_position().y - 0.005, this.guiCopy.get_transform().get_position().z - 1);
            this.get_transform().set_position(behindPos);
        },
        Update: function (){
            this.GetComponent$1(UnityEngine.GUIText.ctor).set_text(this.guiCopy.GetComponent$1(UnityEngine.GUIText.ctor).get_text());
        }
    }
};
JsTypes.push(ScoreShadow);

