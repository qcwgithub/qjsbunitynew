if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var FollowPlayer = {
    fullname: "FollowPlayer",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.offset = new UnityEngine.Vector3.ctor();
            this.player = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.player = UnityEngine.GameObject.FindGameObjectWithTag("Player").get_transform();
        },
        Update: function (){
            this.get_transform().set_position(UnityEngine.Vector3.op_Addition(this.player.get_position(), this.offset));
        }
    }
};
JsTypes.push(FollowPlayer);

