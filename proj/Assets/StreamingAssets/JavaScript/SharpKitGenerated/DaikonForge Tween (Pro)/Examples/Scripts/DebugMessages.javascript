if (typeof(JsTypes) == "undefined")
    var JsTypes = [];
var DebugMessages = {
    fullname: "DebugMessages",
    baseTypeName: "System.Object",
    staticDefinition: {
        cctor: function (){
            DebugMessages.messages = new System.Collections.Generic.List$1.ctor(DebugMessages.Message.ctor);
            DebugMessages.startLeft = 5;
            DebugMessages.startTop = 20;
            DebugMessages.lineHeight = 20;
        },
        Add: function (text){
            for (var i = 0; i < DebugMessages.messages.get_Count(); i++){
                var item = DebugMessages.messages.get_Item$$Int32(i);
                var position = item.guiText.get_pixelOffset();
                position.y += 20;
                item.guiText.set_pixelOffset(position);
            }
            var message = DebugMessages.Message.Obtain();
            message.guiText.set_pixelOffset(new UnityEngine.Vector2.ctor$$Single$$Single(5, 20));
            message.guiText.set_text(text);
            message.tween.Play();
            DebugMessages.messages.Add(message);
        }
    },
    assemblyName: "SharpKitProj1",
    Kind: "Class",
    definition: {
        ctor: function (){
            System.Object.ctor.call(this);
        }
    }
};
JsTypes.push(DebugMessages);

