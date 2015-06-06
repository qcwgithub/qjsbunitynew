if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var DebugMessages$Message = {
    fullname: "DebugMessages.Message",
    baseTypeName: "System.Object",
    staticDefinition: {
        cctor: function (){
            DebugMessages.Message.pool = new System.Collections.Generic.List$1.ctor(DebugMessages.Message.ctor);
        },
        Obtain: function (){
            if (DebugMessages.Message.pool.get_Count() > 0){
                var instance = DebugMessages.Message.pool.get_Item$$Int32(0);
                DebugMessages.Message.pool.RemoveAt(0);
                return instance;
            }
            var gameObject = new UnityEngine.GameObject.ctor$$String("__Message__");
            var newInstance = new DebugMessages.Message.ctor();
            newInstance.guiText = gameObject.AddComponent$1(UnityEngine.GUIText.ctor);
            newInstance.tween = TweenTextExtensions.TweenAlpha$$GUIText(newInstance.guiText).SetDelay$$Single(5).SetDuration(0.5).SetStartValue(1).SetEndValue(0).SetIsTimeScaleIndependent(true).OnCompleted(function (tween){
                newInstance.Release();
            });
            return newInstance;
        }
    },
    assemblyName: "SharpKitProj",
    Kind: "Class",
    definition: {
        ctor: function (){
            this.guiText = null;
            this.tween = null;
            System.Object.ctor.call(this);
        },
        Release: function (){
            DebugMessages.messages.Remove(this);
        }
    }
};
JsTypes.push(DebugMessages$Message);

