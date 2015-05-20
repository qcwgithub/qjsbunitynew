if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Score = {
    fullname: "Score",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.score = 0;
            this.playerControl = null;
            this.previousScore = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.playerControl = UnityEngine.GameObject.FindGameObjectWithTag("Player").GetComponent$1(PlayerControl.ctor);
        },
        Update: function (){
            this.get_guiText().set_text("Score: " + this.score);
            if (this.previousScore != this.score){
                this.playerControl.PreTaunt();
            }
            this.previousScore = this.score;
        }
    }
};
JsTypes.push(Score);

