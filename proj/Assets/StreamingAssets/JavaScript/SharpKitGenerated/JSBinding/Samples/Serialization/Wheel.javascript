if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var Wheel = {
    fullname: "Wheel",
    baseTypeName: "UnityEngine.MonoBehaviour",
    assemblyName: "SharpKitProj2010",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.ePos = 0;
            this.speed = 1;
            this.accum = 0;
            UnityEngine.MonoBehaviour.ctor.call(this);
        },
        setSpeed: function (speed){
            this.speed = speed;
        },
        Start: function (){
        },
        Update: function (){
            this.accum += UnityEngine.Time.get_deltaTime();
            if (this.accum > this.speed){
                switch (this.ePos){
                    case 0:
                        UnityEngine.Debug.Log$$Object("LeftFront go..");
                        break;
                    case 1:
                        UnityEngine.Debug.Log$$Object("RightFront go..");
                        break;
                    case 2:
                        UnityEngine.Debug.Log$$Object("LeftBack go..");
                        break;
                    case 3:
                        UnityEngine.Debug.Log$$Object("RightBack go..");
                        break;
                }
                this.accum = 0;
            }
        }
    }
};
JsTypes.push(Wheel);

