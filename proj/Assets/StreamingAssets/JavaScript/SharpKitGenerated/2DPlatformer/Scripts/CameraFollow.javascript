if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var CameraFollow = {
    fullname: "CameraFollow",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.xMargin = 1;
            this.yMargin = 1;
            this.xSmooth = 8;
            this.ySmooth = 8;
            this.maxXAndY = new UnityEngine.Vector2.ctor();
            this.minXAndY = new UnityEngine.Vector2.ctor();
            this.player = null;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        Awake: function (){
            this.player = UnityEngine.GameObject.FindGameObjectWithTag("Player").get_transform();
        },
        CheckXMargin: function (){
            return UnityEngine.Mathf.Abs$$Single(this.get_transform().get_position().x - this.player.get_position().x) > this.xMargin;
        },
        CheckYMargin: function (){
            return UnityEngine.Mathf.Abs$$Single(this.get_transform().get_position().y - this.player.get_position().y) > this.yMargin;
        },
        FixedUpdate: function (){
            this.TrackPlayer();
        },
        TrackPlayer: function (){
            var targetX = this.get_transform().get_position().x;
            var targetY = this.get_transform().get_position().y;
            if (this.CheckXMargin())
                targetX = UnityEngine.Mathf.Lerp(this.get_transform().get_position().x, this.player.get_position().x, this.xSmooth * UnityEngine.Time.get_deltaTime());
            if (this.CheckYMargin())
                targetY = UnityEngine.Mathf.Lerp(this.get_transform().get_position().y, this.player.get_position().y, this.ySmooth * UnityEngine.Time.get_deltaTime());
            targetX = UnityEngine.Mathf.Clamp$$Single$$Single$$Single(targetX, this.minXAndY.x, this.maxXAndY.x);
            targetY = UnityEngine.Mathf.Clamp$$Single$$Single$$Single(targetY, this.minXAndY.y, this.maxXAndY.y);
            this.get_transform().set_position(new UnityEngine.Vector3.ctor$$Single$$Single$$Single(targetX, targetY, this.get_transform().get_position().z));
        }
    }
};
JsTypes.push(CameraFollow);

